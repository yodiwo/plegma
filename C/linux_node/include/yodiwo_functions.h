#ifndef __YODIWO_FUNCTIONS_H__
#define __YODIWO_FUNCTIONS_H__

#ifdef __cplusplus
extern "C" {
#endif

#include <stdlib.h>
#include "yodiwo_api.h"

//TODO: generate these?
#define s_LoginReq          "loginreq"
#define s_LoginRsp          "loginrsp"
#define s_NodeInfoReq       "nodeinforeq"
#define s_NodeInfoRsp       "nodeinforsp"
#define s_NodeUnpairedReq   "nodeunpairedreq"
#define s_NodeUnpairedRsp   "nodeunpairedrsp"
#define s_EndpointSyncReq   "endpointsyncreq"
#define s_EndpointSyncRsp   "endpointsyncrsp"
#define s_ThingsGet         "thingsget"
#define s_ThingsSet         "thingsset"
#define s_PortEventMsg      "porteventmsg"
#define s_PortStateGet      "portstateget"
#define s_PortStateSet      "portstateset"
#define s_ActivePortKeysMsg "activeportkeysmsg"
#define s_PingReq           "pingreq"
#define s_PingRsp           "pingrsp"

#define s_GenericRsp        "genericrsp"

#define s_GraphDeploymentReq   "graphdeploymentreq"
#define s_GraphActionResultRsp "graphactionresultrsp"

typedef int (func_ToJson)(char *, size_t, void *);
typedef Yodiwo_Plegma_Json_e (func_FromJson)(char *, size_t, void *);

typedef struct
{
    char *name;
    func_ToJson *toJson;
    func_FromJson *fromJson;
    int (*msg_handler)(char *, size_t, int);
} message_translators;

typedef int (*msg_sender)(char *msg, int msg_len, char *msg_type);

typedef void (*active_port_handler_func)(Yodiwo_Plegma_Port_t *port, bool active);

void init_yodiwo(char *nodeName, Array_Yodiwo_Plegma_Thing_t *things, msg_sender sender, active_port_handler_func aph);
void init_port_handlers();
int yodiwo_handle_message(char *message, int message_len, char *topic, int topic_len);

typedef int (*portevent_handler_func)(Yodiwo_Plegma_Port_t *port, char* state, int revNum);
int register_portevent_handler(Yodiwo_Plegma_Port_t *port, portevent_handler_func handler);

int handle_portevent(Yodiwo_Plegma_PortEvent_t *event);

int portevents(Array_Yodiwo_Plegma_PortEvent_t *events);

int publish_helper(char *msg, size_t len, char *subtopic, int syncId, Yodiwo_Plegma_WrapperMsg_eMsgFlags flags);

#ifdef __cplusplus
}
#endif

#endif /* __YODIWO_FUNCTIONS_H__ */
