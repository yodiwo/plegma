#ifndef __MQTT_HELPERS_H__
#define __MQTT_HELPERS_H__

#include "MQTTClient.h"

#include "yodiwo_api.h"
#include <stdlib.h>

#define MAX_MSG_LEN 2500
#define MAX_TOPIC_LEN 150

int mqtt_init(char *hostname, int port, char *certfile, char *nodeKey, char *nodeSecret);

int on_mqtt_message(void *context, char *topicName, int topicLen, MQTTClient_message *message);
void on_mqtt_disconnected(void *context, char *cause);

int button_event(int buttonId, bool pressed);

int publisher(char *msg, int msg_len, char *msg_type);


#endif /* __MQTT_HELPERS_H__ */
