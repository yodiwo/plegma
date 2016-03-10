#ifndef __SEANODE_THINGS_H__
#define __SEANODE_THINGS_H__

#include "yodiwo_api.h"
#include <stdlib.h>

#define MAX_MSG_LEN 2500
#define MAX_TOPIC_LEN 150


typedef int (func_ToJson)(char *, size_t, void *);
typedef Yodiwo_Plegma_Json_e (func_FromJson)(char *, size_t, void *);

int button_event(int buttonId, bool pressed);

void initialize_things(char *nodeKey);

int handle_text_log(Yodiwo_Plegma_PortEvent_t *event);

extern Array_Yodiwo_Plegma_Thing_t things;

void register_event_handlers();

void active_handler(Yodiwo_Plegma_Port_t *port, bool active);

int read_and_send_sensor_event();
int send_text(char *text);

thread_t sensor_thread_ctx;
thread_t text_reader_thread_ctx;
void *sensor_thread(void *args);
void *text_reader_thread(void *args);

#endif /* __SEANODE_THINGS_H__ */
