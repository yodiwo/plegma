#include <stdint.h>
#include <stdbool.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <errno.h>

#include "yodiwo_functions.h"
#include "jsmn.h"
#include "yodiwo_helpers.h"

#define MAX_TOPIC_LEN 100
#define MAX_PORTS 30 //for handler list

int handle_nodeinforeq(char *json, size_t len, int syncId);
int handle_thingsget(char *json, size_t len, int syncId);
int handle_portstateset(char *json, size_t len, int syncId);
int handle_activeportkeysmsg(char *json, size_t len, int syncId);
int handle_porteventmsg(char *json, size_t len, int syncId);

typedef int (*portevent_handler_func)(Yodiwo_Plegma_Port_t *port, char* state, int revNum);

typedef struct
{
    Yodiwo_Plegma_Port_t *port;
    portevent_handler_func handler;
    bool active;
} portevent_handler;

portevent_handler portevent_handlers[MAX_PORTS] = { 0 };
int total_ports;
    
message_translators translators[] = {
//    {"loginreq", (func_ToJson *)Yodiwo_Plegma_LoginReq_ToJson, (func_FromJson *)Yodiwo_Plegma_LoginReq_FromJson, handle_loginreq},
//    {"loginrsp", (func_ToJson *)Yodiwo_Plegma_LoginRsp_ToJson, (func_FromJson *)Yodiwo_Plegma_LoginRsp_FromJson},
    {s_NodeInfoReq, (func_ToJson *)Yodiwo_Plegma_NodeInfoReq_ToJson, (func_FromJson *)Yodiwo_Plegma_NodeInfoReq_FromJson, handle_nodeinforeq},
//    {"nodeinforsp", (func_ToJson *)Yodiwo_Plegma_NodeInfoRsp_ToJson, (func_FromJson *)Yodiwo_Plegma_NodeInfoRsp_FromJson, handle_nodeinforsp},
    {s_ThingsGet, (func_ToJson *)Yodiwo_Plegma_ThingsGet_ToJson, (func_FromJson *)Yodiwo_Plegma_ThingsGet_FromJson, handle_thingsget},
//    {"thingsrsp", (func_ToJson *)Yodiwo_Plegma_ThingsRsp_ToJson, (func_FromJson *)Yodiwo_Plegma_ThingsRsp_FromJson, handle_thingsrsp},
    {s_PortEventMsg, (func_ToJson *)Yodiwo_Plegma_PortEventMsg_ToJson, (func_FromJson *)Yodiwo_Plegma_PortEventMsg_FromJson, handle_porteventmsg},
//    {"portstatereq", (func_ToJson *)Yodiwo_Plegma_PortStateReq_ToJson, (func_FromJson *)Yodiwo_Plegma_PortStateReq_FromJson},
    {s_PortStateSet, (func_ToJson *)Yodiwo_Plegma_PortStateSet_ToJson, (func_FromJson *)Yodiwo_Plegma_PortStateSet_FromJson, handle_portstateset},
    {s_ActivePortKeysMsg, (func_ToJson *)Yodiwo_Plegma_ActivePortKeysMsg_ToJson, (func_FromJson *)Yodiwo_Plegma_ActivePortKeysMsg_FromJson, handle_activeportkeysmsg},
};

char node_name[50] = "";
static Array_Yodiwo_Plegma_Thing_t *array_things = NULL;
msg_sender send_func = NULL;
static active_port_handler_func active_port_handler;

int translators_num = sizeof(translators) / sizeof(translators[0]);

int api_seq_no;

void init_yodiwo(char *nodeName, Array_Yodiwo_Plegma_Thing_t *things, msg_sender sender, active_port_handler_func aph)
{
    strcpy(node_name, nodeName);
    array_things = things;
    send_func = sender;
    active_port_handler = aph;
    init_port_handlers();
    api_seq_no = 1;
}

void get_topic_token(char *dst, char *src, int max_len, int token_idx)
{
    char buf[MAX_TOPIC_LEN];
    char *cur = buf;
    int i;
    buf[0] = '\0';
    strncat(buf, src, max_len);
    for(i = 0; i < token_idx + 1; i++) {
        cur = strtok((i==0) ? buf : NULL, "/");
//        printf("a token: %s\n", cur);
        if (cur == NULL) {
            *dst = '\0';
            return;
        }
    }
    strcpy(dst, cur);
}

int yodiwo_handle_message(char *message, int message_len, char *topic, int topic_len)
{
    int i;
    Yodiwo_Plegma_MqttMsg_t api_msg;
    char token[40];
    
    i = Yodiwo_Plegma_MqttMsg_FromJson(message, message_len, &api_msg);
    if (i < 0) {
        printf("Error deserializing MqttApiMessage\n");
        return -1;
    }
    
    get_topic_token(token, topic, topic_len, 4);
    
    printf("found token: %s\n", token);
    for (i = 0; i < translators_num; i++) {
        printf("checking for %s\n", translators[i].name);
        if (!strcmp(token, translators[i].name)) {
            printf("matched message type: %s\n", token);
            translators[i].msg_handler(api_msg.Payload, strlen(api_msg.Payload), api_msg.SyncId);
            break;
        }
    }
    // free api_msg internal
    return 0;
}

int publish_helper(char *msg, size_t len, char *subtopic, int syncId, Yodiwo_Plegma_WrapperMsg_eMsgFlags flags)
{    
    int r;
    int final_len;
    Yodiwo_Plegma_MqttMsg_t mqtt_api_msg;
    
    char *final_msg = (char *)malloc(len + 500); // TODO: more accurate / proper?
    if (final_msg == NULL) {
        printf("nomem\n");
        return -ENOMEM;
    }
    mqtt_api_msg.Flags = flags;
    mqtt_api_msg.SyncId = syncId;
    mqtt_api_msg.Payload = msg;
    mqtt_api_msg.PayloadSize = len;
    final_len = Yodiwo_Plegma_MqttMsg_ToJson(final_msg, len + 500,  &mqtt_api_msg);
    
    r = send_func(final_msg, final_len, subtopic);
    if (r <= 0) {
        free(final_msg);
        return r;
    }
    // freeing handled by sender function
    return 0;
}

void init_port_handlers()
{
    int i;
    int j;
    total_ports = 0;
    for (i = 0; i < array_things->num; i++) {
        for (j = 0; j < array_things->elems[i].Ports.num; j++) {
            portevent_handlers[total_ports].port = &array_things->elems[i].Ports.elems[j];
            portevent_handlers[total_ports].handler = NULL;
            portevent_handlers[total_ports].active = false;
            total_ports++;
        }
    }
}

int handle_nodeinforeq(char *json, size_t len, int syncId)
{
    int r;
    Yodiwo_Plegma_NodeInfoReq_t msg;
    Yodiwo_Plegma_NodeInfoRsp_t rsp;
    r = Yodiwo_Plegma_NodeInfoReq_FromJson(json, len, &msg);
    printf("fromjson: %d\n", r);
    if (r)
        return r;
    char *json_rsp = (char *)malloc(200); //TODO proper size
    if (json_rsp == NULL) {
        return -ENOMEM;
    }
    rsp.SeqNo = api_seq_no++;
    rsp.Name = node_name;
    rsp.Type = Yodiwo_eNodeType_TestEndpoint;
    rsp.ThingsRevNum = 0;
    rsp.ThingTypes.num = 0;
    rsp.Capabilities = Yodiwo_eNodeCapa_None;  
    rsp.BlockLibraries.num = 0;
    r = Yodiwo_Plegma_NodeInfoRsp_ToJson(json_rsp, 200, &rsp);
    printf("tojson: %d\n", r);    
    if (r < 0)
        return r;
    r =  publish_helper(json_rsp, r, s_NodeInfoRsp, syncId, Yodiwo_eMsgFlags_Response);
    
    free(json_rsp);
    return r;
}

int handle_thingsget(char *json, size_t len, int syncId)
{
    int r;
    Yodiwo_Plegma_ThingsGet_t msg;
    Yodiwo_Plegma_ThingsSet_t rsp;
    char *sendbuf = NULL;
    r = Yodiwo_Plegma_ThingsGet_FromJson(json, len, &msg);
    printf("thingsget json: %s\n", json);
    printf("fromjson: %d\n", r);
    if (r)
        goto exit;
    
    rsp.SeqNo = api_seq_no++;
    printf("thingsget operation: %d\n", msg.Operation);
    if (msg.Operation == Yodiwo_eThingsOperation_Get) {
        rsp.Data = *array_things;
        rsp.Status = 1;
        rsp.Operation = Yodiwo_eThingsOperation_Overwrite;
    } else {
        rsp.Data.num = 0;
        rsp.Data.elems = NULL;
        rsp.Status = 0;
    }
    
    sendbuf = (char *)malloc(2500 * sizeof(char));
    if (sendbuf == NULL) {
        r = -ENOMEM;
        goto exit;
    }
    r = Yodiwo_Plegma_ThingsSet_ToJson(sendbuf, 2500, &rsp);
    printf("thingsset json: %s\n", sendbuf);
    printf("tojson: %d\n", r);    
    if (r < 0)
        goto exit;
    r = publish_helper(sendbuf, r, s_ThingsSet, syncId, Yodiwo_eMsgFlags_Response);
exit:
    free(sendbuf);
    //free thingsreq internals
    return r;
}

//int handle_thingsreq2(char *json, size_t len)
//{
//    int r;
//    Yodiwo_Plegma_ThingsReq_t msg;
//    Yodiwo_Plegma_ThingsRsp_t rsp;
//    char *sendbuf = NULL;
//    r = Yodiwo_Plegma_ThingsReq_FromJson(json, len, &msg);
//    printf("thingsreq json: %s\n", json);
//    printf("fromjson: %d\n", r);
//    if (r)
//        goto exit;
//    
//    rsp.SeqNo = msg.SeqNo;
//    rsp.Operation = msg.Operation;
//    printf("thingsrep operation: %d\n", msg.Operation);
//    if (msg.Operation == Yodiwo_eThingsOperation_Get) {
//        rsp.Data.num = 0;
//        rsp.Data.elems = 0;
//        rsp.Status = 1;
//    } else {
//        rsp.Data.num = 0;
//        rsp.Data.elems = NULL;
//        rsp.Status = 0;
//    }
//    
//    sendbuf = (char *)malloc(1200 * sizeof(char));
//    if (sendbuf == NULL) {
//        r = -ENOMEM;
//        goto exit;
//    }
//    r = Yodiwo_Plegma_ThingsRsp_ToJson(sendbuf, 1200, &rsp);
//    printf("thingsrsp json: %s\n", sendbuf);
//    printf("tojson: %d\n", r);    
//    if (r < 0)
//        goto exit;
//    r = publish_helper(sendbuf, r, "thingsrsp", msg.SeqNo);
//exit:
//    free(sendbuf);
//    //free thingsreq internals
//    return r;
//}

int portevents(Array_Yodiwo_Plegma_PortEvent_t *events)
{
    Yodiwo_Plegma_PortEventMsg_t msg;
    char *msgbuf;
    int r;
    msg.SeqNo = api_seq_no++;
    msg.PortEvents = *events;
    
    msgbuf = (char *)malloc(200); //proper
    if (msgbuf == NULL) {
        printf("can't remember\n");
        return -ENOMEM;
    }
    r = Yodiwo_Plegma_PortEventMsg_ToJson(msgbuf, 200, &msg);
    if (r < 0) {
        printf("i'm afraid json couldn't make it\n");
        free(msgbuf);
        return r;
    }
    r =  publish_helper(msgbuf, r, s_PortEventMsg, 0, Yodiwo_eMsgFlags_None);
    
    free(msgbuf);
    return r;
}

int register_portevent_handler(Yodiwo_Plegma_Port_t *port, portevent_handler_func handler)
{
    int i;
    for (i = 0; i < total_ports; i++) {
        if (portevent_handlers[i].port == port) {
            portevent_handlers[i].handler = handler;
            return 0;
        }
    }
    return -1;
}


int handle_portevent(Yodiwo_Plegma_PortEvent_t *event)
{
    int i, r;
    for (i = 0; i < total_ports; i++) {
        printf("checking handler for %s, event from %s\n", portevent_handlers[i].port->PortKey, event->PortKey);
        if (!strcmp(portevent_handlers[i].port->PortKey, event->PortKey)) {
            r = portevent_handlers[i].handler(portevent_handlers[i].port, event->State, event->RevNum);
            if (r < 0)
                return r;
        }
    }
    return 0;
}

int handle_portstateset(char *json, size_t len, int syncId)
{
    return 0;
}

int handle_activeportkeysmsg(char *json, size_t len, int syncId)
{
    int r;
    int i;
    int j;
    bool new_actives[MAX_PORTS];
    Yodiwo_Plegma_ActivePortKeysMsg_t msg;
    r = Yodiwo_Plegma_ActivePortKeysMsg_FromJson(json, len, &msg);
    printf("fromjson: %d\n", r);
    if (r)
        return r;
    for (i = 0; i < total_ports; i++) {
        new_actives[i] = false;
    }
    for (i = 0; i < msg.ActivePortKeys.num; i++) {
        for (j = 0; j < total_ports; j++) {
            if (!new_actives[j] &&
                    !strcmp(msg.ActivePortKeys.elems[i], portevent_handlers[j].port->PortKey)) {
                new_actives[j] = true;
                break;
            }
        }
    }
    for (i = 0; i < total_ports; i++) {
        if (new_actives[i] != portevent_handlers[i].active) {
            portevent_handlers[i].active = new_actives[i];
            if (active_port_handler) {
                active_port_handler(portevent_handlers[i].port, portevent_handlers[i].active);
            }
        }
    }
    return 0;
}

int handle_porteventmsg(char *json, size_t len, int syncId)
{
    int i, r;
    Yodiwo_Plegma_PortEventMsg_t msg;
    r = Yodiwo_Plegma_PortEventMsg_FromJson(json, len, &msg);
    printf("fromjson: %d\n", r);
    if (r)
        return r;
    for (i = 0; i < msg.PortEvents.num; i++) {
        r = handle_portevent(&msg.PortEvents.elems[i]);
        if (r < 0)
            goto exit;
    }

exit:
    //TODO: free msg internals
    return r;
}
