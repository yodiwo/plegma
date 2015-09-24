/*
 * system.c
 *
 *  Created on: Sep 21, 2015
 *      Author: mits
 */

#include <string.h>
#include "pthread.h"
#include "system.h"

#include "curl/curl.h"

int thread_run(thread_t *ctx, thread_func func, void * args, int priority, int stack_size)
{
	if (stack_size) {
		pthread_attr_t attrs;
		pthread_attr_init(&attrs);
		pthread_attr_setstacksize(&attrs, stack_size);
		return pthread_create(ctx, &attrs, func, args);
	} else {
		return pthread_create(ctx, NULL, func, args);
	}
//	return 0;
}

void thread_wait(int ms)
{
	usleep(ms * 1000);
}

void thread_join(thread_t *ctx)
{
	pthread_join(*ctx, NULL);
}

struct curl_write_buffer
{
	char *buf;
	size_t data_size;
	size_t max_size;
};

static size_t curl_write_cb(void *contents, size_t size, size_t nmemb, void *userdata)
{
  size_t realsize = size * nmemb;
  struct curl_write_buffer *mem = (struct curl_write_buffer *)userdata;
  if (mem->data_size + realsize + 1> mem->max_size)
	  return 0;

  memcpy(&(mem->buf[mem->data_size]), contents, realsize);
  mem->data_size += realsize;
  mem->buf[mem->data_size] = 0;

  return realsize;
}

int http_post(char *url, char *post_fields, char *response, size_t max_size)
{
	CURL *curl;
	CURLcode res;
	int r;
	struct curl_write_buffer memblock;
	long http_code = 0;

	memblock.buf = response;
	memblock.max_size = max_size;
	memblock.data_size = 0;

	curl_global_init(CURL_GLOBAL_ALL);

	curl = curl_easy_init();
	if(curl) {
		curl_easy_setopt(curl, CURLOPT_URL, url);
		curl_easy_setopt(curl, CURLOPT_POSTFIELDS, post_fields);
	    curl_easy_setopt(curl, CURLOPT_POST, 1L);
	    curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, curl_write_cb);
	    curl_easy_setopt(curl, CURLOPT_WRITEDATA, &memblock);
	    curl_easy_setopt(curl, CURLOPT_VERBOSE, 1L);

		res = curl_easy_perform(curl);
		if(res != CURLE_OK) {
			fprintf(stderr, "curl_easy_perform() failed: %s\n",
					curl_easy_strerror(res));
			curl_easy_cleanup(curl);
			curl_global_cleanup();
			return -1;
		}
		curl_easy_getinfo (curl, CURLINFO_RESPONSE_CODE, &http_code);
		curl_easy_cleanup(curl);
	}

	curl_global_cleanup();
	return http_code;
}
