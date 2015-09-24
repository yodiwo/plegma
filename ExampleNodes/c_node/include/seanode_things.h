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

#endif /* __SEANODE_THINGS_H__ */
