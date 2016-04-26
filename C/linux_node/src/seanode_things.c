#include <mqtt_helpers.h>
#include <stdint.h>
#include <stdbool.h>
#include <string.h>

#include "system.h"
#include "jsmn.h"
#include "yodiwo_functions.h"
#include "yodiwo_helpers.h"

#define SENSOR_COMMAND "sensors | grep -oP 'Physical\\ id\\ 0:\\ *\\K([^\\s]*)'"
//#define SENSOR_COMMAND "date"

#define THING_NUM 3

///////////////////////////////// THINGS
Yodiwo_Plegma_Port_t text_reader_port;
Yodiwo_Plegma_Port_t text_logger_port;
Yodiwo_Plegma_Port_t sensor_port;

Yodiwo_Plegma_Thing_t *text_reader;
Yodiwo_Plegma_Thing_t *text_logger;
Yodiwo_Plegma_Thing_t *sensor;

Yodiwo_Plegma_Thing_t _things[THING_NUM];
bool text_reader_active;
bool sensor_active;
Array_Yodiwo_Plegma_Thing_t things;

int send_text(char *text)
{
    printf("text to send as port event: %s\n", text);

    Yodiwo_Plegma_PortEvent_t event;
    Array_Yodiwo_Plegma_PortEvent_t array_events;

    event.PortKey = text_reader->Ports.elems[0].PortKey;
    event.State = text;
    event.RevNum = 1;
    event.Timestamp = 0;

    array_events.num = 1;
    array_events.elems = &event;

    return portevents(&array_events);
}

void read_sensor(char *out, int maxlen)
{
	FILE* pipe = popen(SENSOR_COMMAND, "r");
	if (pipe)
	{
		while(!feof(pipe)) {
			if(fgets(out, maxlen, pipe) != NULL){
			}
		}
	pclose(pipe);
	out[strlen(out)-1] = '\0';
	}
}

int read_and_send_sensor_event()
{
	char buf[100];
	read_sensor(buf, 100);
    printf("sensor data: %s\n", buf);

    Yodiwo_Plegma_PortEvent_t event;
    Array_Yodiwo_Plegma_PortEvent_t array_events;

    event.PortKey = sensor->Ports.elems[0].PortKey;
    event.State = buf;
    event.RevNum = 1;
    event.Timestamp = 0;

    array_events.num = 1;
    array_events.elems = &event;

    return portevents(&array_events);
}

int handle_text_log(Yodiwo_Plegma_Port_t *port, char* state, int revNum)
{
	printf("LOG: %s\n", state);
	return 0;
}

void active_handler(Yodiwo_Plegma_Port_t *port, bool active)
{
	if (port == &text_reader->Ports.elems[0]) {
		text_reader_active = active;
	} else if (port == &sensor->Ports.elems[0]) {
		sensor_active = active;
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
    	if (sensor_active)
    		read_and_send_sensor_event();
    }
    return NULL;
}

void *text_reader_thread(void *args)
{
    char line[200];
    while(true) {
    	scanf("%s", line);
    	if (text_reader_active)
    		send_text(line);
    }
    return NULL;
}

void initialize_things(char *nodeKey)
{
    // init thing pointers
    text_reader = &_things[0];
    text_logger = &_things[1];
    sensor = &_things[2];

    //TEXT READER
    text_reader_port.ConfFlags = Yodiwo_ePortConf_IsTrigger;
    text_reader_port.Description = "text reader text";
    text_reader_port.Name = "text";
    text_reader_port.Type = Yodiwo_ePortType_String;
    text_reader_port.State = "";
    text_reader_port.RevNum = 1;
    text_reader_port.ioDirection = Yodiwo_ioPortDirection_Output;
    text_reader_port.PortKey = "";

    text_reader->ThingKey = "";
    text_reader->Name = "text reader";
    text_reader->Config.num = 0;
    text_reader->Config.elems = NULL;
    text_reader->Ports.num = 1;
    text_reader->Ports.elems = &text_reader_port;
    text_reader->Type = "";
    text_reader->BlockType = "";
    text_reader->UIHints.Description = "";
    text_reader->UIHints.IconURI = "/Content/VirtualGateway/img/icon-thing-genericbutton.png";

    //TEXT LOGGER
    text_logger_port.ConfFlags = Yodiwo_ePortConf_IsTrigger;
    text_logger_port.Description = "logging text to stdout";
    text_logger_port.Name = "log";
    text_logger_port.Type = Yodiwo_ePortType_String;
    text_logger_port.State = "";
    text_logger_port.RevNum = 1;
    text_logger_port.ioDirection = Yodiwo_ioPortDirection_Input;
    text_logger_port.PortKey = "";

    text_logger->ThingKey = "";
    text_logger->Name = "text logger";
    text_logger->Config.num = 0;
    text_logger->Config.elems = NULL;
    text_logger->Ports.num = 1;
    text_logger->Ports.elems = &text_logger_port;
    text_logger->Type = "";
    text_logger->BlockType = "";
    text_logger->UIHints.Description = "";
    text_logger->UIHints.IconURI = "/Content/VirtualGateway/img/accelerometer.jpg";

    //SENSOR
    sensor_port.ConfFlags = Yodiwo_ePortConf_None; //why not
    sensor_port.Description = "sensor data";
    sensor_port.Name = "sensor data";
    sensor_port.Type = Yodiwo_ePortType_String;
    sensor_port.State = "False";
    sensor_port.RevNum = 1;
    sensor_port.ioDirection = Yodiwo_ioPortDirection_Output;
    sensor_port.PortKey = "";

    sensor->ThingKey = "";
    sensor->Name = "sensor reader";
    sensor->Config.num = 0;
    sensor->Config.elems = NULL;
    sensor->Ports.num = 1;
    sensor->Ports.elems = &sensor_port;
    sensor->Type = "";
    sensor->BlockType = "";
    sensor->UIHints.Description = "";
    sensor->UIHints.IconURI = "/Content/VirtualGateway/img/icon-thing-genericlight.png";

    things.num = THING_NUM;
    things.elems = _things;

    sensor_active = false;
    text_reader_active = false;

    if (nodeKey != NULL) {
        int r;
        int j;
        for (j = 0; j < things.num; j++) {
            r = fill_Thing_Keys(&things.elems[j], nodeKey, j + 1);
        }
        printf("fill keys returned: %d\n", r);
    }
}

void register_event_handlers()
{
	register_portevent_handler(&text_logger->Ports.elems[0], handle_text_log);
}
