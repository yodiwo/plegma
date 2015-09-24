/*
 * main.c
 *
 *  Created on: Sep 21, 2015
 *      Author: mits
 */


#include <stdio.h>
#include "jsmn.h"
#include "config.h"
#include "yodiwo_functions.h"
#include "yodiwo_helpers.h"
#include "mqtt_helpers.h"
#include "seanode_things.h"

Yodiwo_Tools_APIGenerator_CNodeYConfig_t configuration;
Yodiwo_Tools_APIGenerator_CNodeConfig_t *activeConfig;

void pairing_done(char *nodeKey, char *secretKey);
int launch_mqtt();

int main(int argc, char *argv[])
{

    int r = read_config(&configuration, "config.json");
//    r = write_config(&configuration, "config.json");
    printf("read config returned: %d\n", r);
    activeConfig = &configuration.Configs.elems[configuration.ActiveID];

    if (!activeConfig->NodeKey) {
        printf("not paired, launching HTTP server for pairing\n");
        start_pairing_http_server(NULL, 8080, pairing_done);
    } else {
        printf("found NodeKey: %s, proceeding to connection\n", activeConfig->NodeKey);
        launch_mqtt();
    }

    char line[200];
    while(true) {
    	scanf("%s", line);
    	send_text(line);
    }
}

void *sensor_thread(void *args)
{
    while(true) {
    	thread_wait(5000);
    	/*
    	 * read_and_send_sensor_event executes a command
    	 * (default here is a filtered output of lm-sensors)
    	 * then sends its output as a port event message
    	 */
    	read_and_send_sensor_event();
    }
    return NULL;
}

int launch_mqtt()
{
    int r;
    initialize_things(activeConfig->NodeKey);
    init_yodiwo(activeConfig->Name, &things, publisher);
    register_event_handlers();
    r = mqtt_init(activeConfig->MqttBrokerHostname,
                     activeConfig->MqttBrokerPort,
                     activeConfig->MqttBrokerCertFile,
                     activeConfig->NodeKey,
                     activeConfig->NodeSecret
                     );
    if (r >= 0) {
    	printf("YoDiWo connection ok!\n");
    }
    return r;

}

void pairing_done(char *nodeKey, char *secretKey)
{
    printf("pairing done!\n");
    printf("NokeKey: %s\n", nodeKey);
    printf("SecretKey: %s\n", secretKey);
    activeConfig->NodeKey = nodeKey;
    activeConfig->NodeSecret = secretKey;
    int r = write_config(&configuration, "config.json");
    if (r < 0) {
        printf("error writing config to file\n");
    } else {
        printf("successfully written new config to file\n");
        printf("launching mqtt now...\n");
        launch_mqtt();
    }
}
