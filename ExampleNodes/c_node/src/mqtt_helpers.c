#include <stdint.h>
#include <stdbool.h>
#include <string.h>

#include "MQTTClient.h"

#include "jsmn.h"
#include "yodiwo_functions.h"
#include "yodiwo_helpers.h"

#include "mqtt_helpers.h"
#include "config.h"

char mqtt_topic_sub[MAX_TOPIC_LEN];
char mqtt_topic_pub[MAX_TOPIC_LEN];

MQTTClient client;

int mqtt_init(char *hostname, int port, char *certfile, char *nodeKeyS, char *nodeSecret)
{   
	int rc;
	char hostaddress[50];

    Yodiwo_Plegma_NodeKey_t nodeKey;
    if (nodeKeyS == NULL || nodeSecret == NULL) {
    	return -1;
    }
    NodeKey_FromString(&nodeKey, nodeKeyS);
    sprintf(mqtt_topic_sub, "/api/out/" YODIWO_API_VERSION_STR "/%s/#", nodeKeyS);
    sprintf(mqtt_topic_pub, "/api/in/" YODIWO_API_VERSION_STR "/%s/%s/", nodeKey.UserKey.UserID, nodeKeyS);
    printf("topic to subscribe: %s\n", mqtt_topic_sub);

    MQTTClient_connectOptions conn_opts = MQTTClient_connectOptions_initializer;
    MQTTClient_deliveryToken token;

    sprintf(hostaddress, "%s:%d", hostname, port);

    rc = MQTTClient_create(&client, hostaddress, nodeKeyS, MQTTCLIENT_PERSISTENCE_NONE, NULL);
    if (rc != MQTTCLIENT_SUCCESS) {
    	return -1;
    }

    conn_opts.keepAliveInterval = 20;
    conn_opts.cleansession = 1;
    conn_opts.MQTTVersion = 4;
    conn_opts.username = nodeKeyS;
    conn_opts.password = nodeSecret;

    MQTTClient_setCallbacks(client, NULL, on_mqtt_disconnected, on_mqtt_message, NULL);

    printf("connecting to MQTT broker: %s:%d\n", hostname, port);
    if ((rc = MQTTClient_connect(client, &conn_opts)) != MQTTCLIENT_SUCCESS)
    {
        printf("Failed to connect, return code %d\n", rc);
        return -1;
    }
    printf("MQTT connected\n") ;
    if ((rc = MQTTClient_subscribe(client, mqtt_topic_sub, 2)) != MQTTCLIENT_SUCCESS) {
        printf("rc from MQTT subscribe is %d\n", rc);
        return rc;
    }
    printf("Subscribed\n");


    return 0;
}

int on_mqtt_message(void *context, char *topicName, int topicLen, MQTTClient_message *message)
{
	if (!topicLen)
		topicLen = strlen(topicName);
    printf("%.*s\n", topicLen, topicName);
    printf("Message arrived: qos %d, retained %d, dup %d, packetid %d\n",
    		message->qos, message->retained, message->dup, message->msgid);
    printf("Payload %.*s\n", message->payloadlen, (char*)message->payload);
    yodiwo_handle_message((char *)message->payload, message->payloadlen, topicName, topicLen);
    return true;
}

void on_mqtt_disconnected(void *context, char *cause)
{

}

int publisher(char *msg, int msg_len, char *msg_type)
{
    int r;
    char topic[MAX_TOPIC_LEN];
    
    strcpy(topic, mqtt_topic_pub);
    strcat(topic, msg_type);
    printf("publishing to %s\n", topic);
    printf("content: %.*s\n", msg_len, msg);
    printf("length: %d\n", msg_len);
    r = MQTTClient_publish(client, topic, msg_len -1, msg, 2, false, NULL);
    printf("publish returned %d\n", r);
    return r;
}
