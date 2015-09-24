/*
 * system.h
 *
 *  Created on: Sep 21, 2015
 *      Author: mits
 */

#ifndef SYSTEM_H_
#define SYSTEM_H_

#ifdef linux

#include <pthread.h>
typedef pthread_t thread_t;
typedef void * (*thread_func)(void *);
#endif

/*
 * These functions must be implemented for the node platform
 * for linux, thread_* functions are wrappers for pthead functions
 * and http_post uses curl to do an http post and return the response code and body to the caller
 * implementation is in system_linux.c
 */

int thread_run(thread_t *ctx, thread_func func, void * args, int priority, int stack_size);
void thread_wait(int ms);
void thread_join(thread_t *ctx);

int http_post(char *url, char *post_fields, char *response, size_t max_size);

#endif /* SYSTEM_H_ */
