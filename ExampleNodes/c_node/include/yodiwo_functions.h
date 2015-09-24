#ifndef __YODIWO_FUNCTIONS_H__
#define __YODIWO_FUNCTIONS_H__

#ifdef __cplusplus
extern "C" {
#endif

#include <stdlib.h>
#include "yodiwo_api.h"

typedef int (func_ToJson)(char *, size_t, void *);
typedef Yodiwo_Plegma_Json_e (func_FromJson)(char *, size_t, void *);

typedef struct
{
    char *name;
    func_ToJson *toJson;
    func_FromJson *fromJson;
    int (*msg_handler)(char *, size_t);
} message_translators;

typedef int (*msg_sender)(char *msg, int msg_len, char *msg_type);

void init_yodiwo(char *nodeName, Array_Yodiwo_Plegma_Thing_t *things, msg_sender sender);
int yodiwo_handle_message(char *message, int message_len, char *topic, int topic_len);

typedef int (*portevent_handler_func)(Yodiwo_Plegma_PortEvent_t *event);
int register_portevent_handler(char *portKey, portevent_handler_func handler);

int handle_portevent(Yodiwo_Plegma_PortEvent_t *event);

int portevents(Array_Yodiwo_Plegma_PortEvent_t *events);

#ifdef __cplusplus
}
#endif

#endif /* __YODIWO_FUNCTIONS_H__ */
