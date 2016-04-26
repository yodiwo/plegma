/*
 * main.c
 *
 *  Created on: Sep 21, 2015
 *      Author: mits
 */


#include <stdio.h>

#include <pthread.h>

#include "system.h"
#include "jsmn.h"
#include "config.h"
#include "yodiwo_functions.h"
#include "yodiwo_helpers.h"
#include "mqtt_helpers.h"
#include "seanode_things.h"
#include "pairing_http_handler.h"

APIGenerator_AdditionalTypes_CNodeCConfig_t configuration;
APIGenerator_AdditionalTypes_CNodeConfig_t *activeConfig;

void pairing_done(char *nodeKey, char *secretKey);
int start_main_agent();

int main(int argc, char *argv[])
{

    int r = read_config(&configuration, "config.json");
//    r = write_config(&configuration, "config.json");
    printf("read config returned: %d\n", r);
    activeConfig = get_active_config(&configuration);

    if (!activeConfig->NodeKey) {
        printf("not paired, launching HTTP server for pairing\n");
        start_pairing_http_server(NULL, 8080, pairing_done);
    } else {
        printf("found NodeKey: %s, proceeding to connection\n", activeConfig->NodeKey);
        start_main_agent();
    }
    thread_run(&sensor_thread_ctx, sensor_thread, NULL, 0, 0);
    thread_run(&text_reader_thread_ctx, text_reader_thread, NULL, 0, 0);
    while(true) {
    	thread_wait(2000);
    }
}

int start_main_agent()
{
    int r;
    initialize_things(activeConfig->NodeKey);
    init_yodiwo(activeConfig->Name, &things, publisher, active_handler);
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
        start_main_agent();
    }
}
