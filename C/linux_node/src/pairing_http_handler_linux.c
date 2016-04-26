#include <string.h>
#include <stdbool.h>

#include <netinet/in.h>

#include "pairing_backend.h"
#include "pairing_http_handler.h"

#include "system.h"

#include "mongoose.h"

pairing_context pairing_state;
onPaired_callback pairing_done_cb;

static char s_http_port[6] = "8000";
static struct mg_serve_http_opts s_http_server_opts;
struct mg_mgr mgr;
thread_t http_thread;

static int keep_running = 0;

static void ev_handler(struct mg_connection *nc, int ev, void *ev_data);
void *http_server_thread_func(void *args);
void startpairing(struct mg_connection *nc, struct http_message *hm);
void next(struct mg_connection *nc, struct http_message *hm);

int start_pairing_http_server(void *ifdata, int port, onPaired_callback onPaired)
{
	struct mg_connection *nc;

	if (port < 1 || port > 65535)
		return -1;
	sprintf(s_http_port, "%d", port);

	keep_running = 0;
    pairing_done_cb = onPaired;

    mg_mgr_init(&mgr, NULL);
    nc = mg_bind(&mgr, s_http_port, ev_handler);

    // Set up HTTP server parameters
    mg_set_protocol_http_websocket(nc);
    s_http_server_opts.document_root = "www/";  // Serve current directory
    s_http_server_opts.enable_directory_listing = "yes";
    thread_run(&http_thread, http_server_thread_func, NULL, 0, 0);

    return 0;
}

void stop_pairing_http_server()
{
	keep_running = 0;
}

void *http_server_thread_func(void *args)
{
	keep_running = 1;
    while(keep_running) {
      mg_mgr_poll(&mgr, 1000);
    }
    mg_mgr_free(&mgr);
    return NULL;
}

static void ev_handler(struct mg_connection *nc, int ev, void *ev_data)
{
	struct http_message *hm = (struct http_message *) ev_data;
	switch (ev) {
		case NS_HTTP_REQUEST:
			printf("uri: %.*s\n", (unsigned int)hm->uri.len, hm->uri.p);
			if (mg_vcmp(&hm->uri, "/pairing/startpairing") == 0) {
				startpairing(nc, hm);
			} else if (mg_vcmp(&hm->uri, "/pairing/next") == 0) {
				next(nc, hm);
			} else {
		        mg_serve_http(nc, hm, s_http_server_opts);
			}
			break;
		default:
			break;
	}
}

int get_server_ip_address(struct mg_connection *con, char **ip, int *port)
{
	static char	server_ip[50+1] = {0};
	struct sockaddr_in sin;
	socklen_t len = sizeof(sin);
	if (getsockname(con->sock, (struct sockaddr *)&sin, &len) == -1) {
		return -1;
	}
	*port = ntohs(sin.sin_port);
	*ip = inet_ntoa(sin.sin_addr);
	return 0;
}

void startpairing(struct mg_connection *nc, struct http_message *hm)
{
    printf("starting pairing...\n");
    pairing_context_init_from_config(&pairing_state, pairing_done_cb);
    int idx = 0;
    int i;
    char urlBase[100];
    char *hostname;
    int port;
    i = 0;

    const char *uri = hm->uri.p;
    while (i < hm->uri.len) {
        if (uri[i] == '/') {
            idx = i;
        }
        i++;
    }
    urlBase[0] = '\0';
    strncat(urlBase, uri, idx + 1);
    i = get_server_ip_address(nc, &hostname, &port);
    if (i < 0) {
    	goto error;
    }
    pairing_state.next_url = get_next_url(hostname, port, urlBase);
    i = pairing_get_tokens(&pairing_state);
    if (i < 0)
    	goto error;
    char *redirect = get_server_phase2_url(&pairing_state);
    printf("redirect url: %s\n", redirect);
    if (redirect) {
    	mg_printf(nc, "HTTP/1.1 302 It's over there\r\nLocation: %s\r\nContent-Length: 0\r\n\r\n", redirect);
        return;
    } else {
    	goto error;
    }
    return;
error:
	mg_printf(nc, "HTTP/1.1 500 Ραν τάιμ έρροContent-Length: 0\r\n\r\n");
}

void next(struct mg_connection *nc, struct http_message *hm)
{
	int ret;
	char buf[300];
	ret = pairing_get_keys(&pairing_state);
	pairing_done_cb(pairing_state.nodeKey, pairing_state.secretKey);
	sprintf(buf, "OK\r\nnodeKey: %s\r\nsecretKey: %s\r\n", pairing_state.nodeKey, pairing_state.secretKey);
	mg_printf(nc, "HTTP/1.1 200 OK\r\nContent-Length: %d\r\n\r\n%s\r\n\r\n", strlen(buf), buf);
}
