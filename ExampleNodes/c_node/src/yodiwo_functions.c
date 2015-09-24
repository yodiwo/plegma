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
#define MAX_PORTEVENT_HANDLERS 5

int SeqNo = 0;

int handle_nodeinforeq(char *json, size_t len);
int handle_thingsreq(char *json, size_t len);
int handle_portstatersp(char *json, size_t len);
int handle_activeportkeysmsg(char *json, size_t len);
int handle_porteventmsg(char *json, size_t len);

typedef int (*portevent_handler_func)(Yodiwo_Plegma_PortEvent_t *event);

typedef struct
{
    char *portKey;
    portevent_handler_func handler;
} portevent_handler;

portevent_handler portevent_handlers[MAX_PORTEVENT_HANDLERS] = { 0 };
    
message_translators translators[] = {
//    {"loginreq", (func_ToJson *)Yodiwo_Plegma_LoginReq_ToJson, (func_FromJson *)Yodiwo_Plegma_LoginReq_FromJson, handle_loginreq},
//    {"loginrsp", (func_ToJson *)Yodiwo_Plegma_LoginRsp_ToJson, (func_FromJson *)Yodiwo_Plegma_LoginRsp_FromJson},
    {"nodeinforeq", (func_ToJson *)Yodiwo_Plegma_NodeInfoReq_ToJson, (func_FromJson *)Yodiwo_Plegma_NodeInfoReq_FromJson, handle_nodeinforeq},
//    {"nodeinforsp", (func_ToJson *)Yodiwo_Plegma_NodeInfoRsp_ToJson, (func_FromJson *)Yodiwo_Plegma_NodeInfoRsp_FromJson, handle_nodeinforsp},
    {"thingsreq", (func_ToJson *)Yodiwo_Plegma_ThingsReq_ToJson, (func_FromJson *)Yodiwo_Plegma_ThingsReq_FromJson, handle_thingsreq},
//    {"thingsrsp", (func_ToJson *)Yodiwo_Plegma_ThingsRsp_ToJson, (func_FromJson *)Yodiwo_Plegma_ThingsRsp_FromJson, handle_thingsrsp},
    {"porteventmsg", (func_ToJson *)Yodiwo_Plegma_PortEventMsg_ToJson, (func_FromJson *)Yodiwo_Plegma_PortEventMsg_FromJson, handle_porteventmsg},
//    {"portstatereq", (func_ToJson *)Yodiwo_Plegma_PortStateReq_ToJson, (func_FromJson *)Yodiwo_Plegma_PortStateReq_FromJson},
    {"portstatersp", (func_ToJson *)Yodiwo_Plegma_PortStateRsp_ToJson, (func_FromJson *)Yodiwo_Plegma_PortStateRsp_FromJson, handle_portstatersp},
    {"activeportkeysmsg", (func_ToJson *)Yodiwo_Plegma_ActivePortKeysMsg_ToJson, (func_FromJson *)Yodiwo_Plegma_ActivePortKeysMsg_FromJson, handle_activeportkeysmsg},
};

char node_name[50] = "";
static Array_Yodiwo_Plegma_Thing_t *array_things = NULL;
msg_sender send_func = NULL;

int translators_num = sizeof(translators) / sizeof(translators[0]);


void init_yodiwo(char *nodeName, Array_Yodiwo_Plegma_Thing_t *things, msg_sender sender)
{
    strcpy(node_name, nodeName);
    array_things = things;
    send_func = sender;
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
    Yodiwo_Plegma_Mqtt_MqttAPIMessage_t api_msg;
    char token[40];
    
    i = Yodiwo_Plegma_Mqtt_MqttAPIMessage_FromJson(message, message_len, &api_msg);
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
            translators[i].msg_handler(api_msg.Payload, strlen(api_msg.Payload));
            break;
        }
    }
    // free api_msg internal
    return 0;
}

int publish_helper(char *msg, size_t len, char *subtopic, int responseToSeqNo)
{    
    int r;
    int final_len;
    Yodiwo_Plegma_Mqtt_MqttAPIMessage_t mqtt_api_msg;
    
    char *final_msg = (char *)malloc(len + 500); // TODO: more accurate / proper?
    if (final_msg == NULL) {
        printf("nomem\n");
        return -ENOMEM;
    }
    
    mqtt_api_msg.ResponseToSeqNo = responseToSeqNo;
    mqtt_api_msg.Payload = msg;
    final_len = Yodiwo_Plegma_Mqtt_MqttAPIMessage_ToJson(final_msg, len + 500,  &mqtt_api_msg);
    
    r = send_func(final_msg, final_len, subtopic);
    
    free(final_msg);
    return r;
}

int handle_nodeinforeq(char *json, size_t len)
{
    int r;
    Yodiwo_Plegma_NodeInfoReq_t msg;
    Yodiwo_Plegma_NodeInfoRsp_t rsp;
    r = Yodiwo_Plegma_NodeInfoReq_FromJson(json, len, &msg);
    printf("fromjson: %d\n", r);
    if (r)
        return r;
    char *json_rsp = (char *)malloc(100); //TODO proper size
    if (json_rsp == NULL) {
        return -ENOMEM;   
    }
    rsp.SeqNo = msg.SeqNo;
    rsp.Name = node_name;
    rsp.Type = Yodiwo_eNodeType_TestEndpoint;
    rsp.ThingTypes.num = 0;
    rsp.Capabilities = Yodiwo_eNodeCapa_None;  
    r = Yodiwo_Plegma_NodeInfoRsp_ToJson(json_rsp, 100, &rsp);
    printf("tojson: %d\n", r);    
    if (r < 0)
        return r;
    r =  publish_helper(json_rsp, r, "nodeinforsp", msg.SeqNo);
    
    free(json_rsp);
    return r;
}

int handle_thingsreq(char *json, size_t len)
{
    int r;
    Yodiwo_Plegma_ThingsReq_t msg;
    Yodiwo_Plegma_ThingsRsp_t rsp;
    char *sendbuf = NULL;
    r = Yodiwo_Plegma_ThingsReq_FromJson(json, len, &msg);
    printf("thingsreq json: %s\n", json);
    printf("fromjson: %d\n", r);
    if (r)
        goto exit;
    
    rsp.SeqNo = msg.SeqNo;
    rsp.Operation = msg.Operation;
    printf("thingsreq operation: %d\n", msg.Operation);
    if (msg.Operation == Yodiwo_eThingsOperation_Get) {
        rsp.Data = *array_things;
        rsp.Status = 1;
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
    r = Yodiwo_Plegma_ThingsRsp_ToJson(sendbuf, 2500, &rsp);
    printf("thingsrsp json: %s\n", sendbuf);
    printf("tojson: %d\n", r);    
    if (r < 0)
        goto exit;
    r = publish_helper(sendbuf, r, "thingsrsp", msg.SeqNo);
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
    msg.SeqNo = SeqNo++;
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
    r =  publish_helper(msgbuf, r, "porteventmsg", 0);
    
    free(msgbuf);
    return r;
}

int register_portevent_handler(char *portKey, portevent_handler_func handler)
{
    int i;
    for (i = 0; i < MAX_PORTEVENT_HANDLERS; i++) {
        if (portevent_handlers[i].portKey == NULL) {
            portevent_handlers[i].portKey = portKey;
            portevent_handlers[i].handler = handler;
            return 0;
        }
    }
    return -1;
}


int handle_portevent(Yodiwo_Plegma_PortEvent_t *event)
{
    int i, r;
    for (i = 0; i < MAX_PORTEVENT_HANDLERS; i++) {
        printf("checking handler for %s, event from %s\n", portevent_handlers[i].portKey, event->PortKey);
        if (portevent_handlers[i].portKey != NULL && !strcmp(portevent_handlers[i].portKey, event->PortKey)) {
            r = portevent_handlers[i].handler(event);
            if (r < 0)
                return r;
        }
    }
    return 0;
}

int handle_portstatersp(char *json, size_t len)
{
    return 0;
}

int handle_activeportkeysmsg(char *json, size_t len)
{
    return 0;
}

int handle_porteventmsg(char *json, size_t len)
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
