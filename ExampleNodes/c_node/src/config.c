#include <string.h>
#include <stdio.h>
#include <errno.h>
#include <stdbool.h>
#include "jsmn.h"
#include "config.h"

//Yodiwo_Plegma_Json_e Configuration_FromJson(char* json, size_t jsonSize, config_t *config)
//{
//    memset(config, 0, sizeof(config_t));
//    ParseTable table[] = {
//        { "uuid",                 4, Parse_String, NULL, (void **)&config->uuid },
//        { "name",                 4, Parse_String, NULL, (void **)&config->name },
//        { "nodeKey",              7, Parse_String, NULL, (void **)&config->nodeKey },
//        { "nodeSecret",          10, Parse_String, NULL, (void **)&config->nodeSecret },
//        { "pairingServerUrl",    16, Parse_String, NULL, (void **)&config->pairingServerUrl },
//        { "ypchannelServer",     15, Parse_String, NULL, (void **)&config->ypchannelServer },
//        { "ypchannelServerPort", 19, Parse_Int,    NULL, (void **)&config->ypchannelServerPort },
//        { "webPort",              7, Parse_Int,    NULL, (void **)&config->webPort },
//        { "mqttBrokerHostname",  18, Parse_String, NULL, (void **)&config->mqttBrokerHostname },
//        { "mqttBrokerPort",      14, Parse_Int,    NULL, (void **)&config->mqttBrokerPort },
//        { "mqttBrokerCertFile",  18, Parse_String, NULL, (void **)&config->mqttBrokerCertFile },
//    };
//    return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
//}
//
//int Configuration_ToJson(char* jsonStart, size_t jsonSize, config_t *config)
//{
//    char *json = jsonStart, *jsonEnd = json + jsonSize;
//    json += snprintf(json, jsonEnd - json, "{ \"uuid\" : \"%s\"", config->uuid); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"name\" : \"%s\"", config->name); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"nodeKey\" : \"%s\"", config->nodeKey); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"nodeSecret\" : \"%s\"", config->nodeSecret); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"pairingServerUrl\" : \"%s\"", config->pairingServerUrl); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"ypchannelServer\" : \"%s\"", config->ypchannelServer); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"ypchannelServerPort\" : %d", config->ypchannelServerPort); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"webPort\" : %d", config->webPort); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"mqttBrokerHostname\" : \"%s\"", config->mqttBrokerHostname); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"mqttBrokerPort\" : %d", config->mqttBrokerPort); if (json >= jsonEnd) return -1;
//    json += snprintf(json, jsonEnd - json, ", \"mqttBrokerCertFile\" : \"%s\"", config->mqttBrokerCertFile); if (json >= jsonEnd) return -1;
//    *json = '}'; json++;
//    *json = '\0'; json++;
//    return json - jsonStart;
//}

int read_config(Yodiwo_Tools_APIGenerator_CNodeYConfig_t *config, char *filename)
{
    FILE *f;
    int size;
    char *buf;
    int r;
    f = fopen(filename, "r");
    if (f == NULL) {
        return -1;    
    }
    fseek(f, 0L, SEEK_END);
    size = ftell(f);
    fseek(f, 0L, SEEK_SET);
    
    buf = (char *)malloc(sizeof(char) * (size + 10));
    if (buf == NULL) {
        r = -ENOMEM;
        goto exit;
    }
    size = fread(buf, 1, size, f);
    buf[size] = '\0';
    fclose(f);
    f = NULL;
    printf("config file size was %d\n", size);
    printf("gonna convert from json...\n");
    r = Yodiwo_Tools_APIGenerator_CNodeYConfig_FromJson(buf, size, config);
exit:
    free(buf);
    if (f != NULL)
    	fclose(f);
    return (r < 0) ? r : 0;
}

int write_config(Yodiwo_Tools_APIGenerator_CNodeYConfig_t *config, char *filename)
{
    FILE *f;
    char *buf;
    int r;
    f = fopen(filename, "w");
    if (f == NULL) {
        return -1;    
    }
    buf = (char *)malloc(2048); //TODO: proper
    if (buf == NULL) {
        r = -ENOMEM;
        goto exit;
    }
    r = Yodiwo_Tools_APIGenerator_CNodeYConfig_ToJson(buf, 2048, config);
    if (r < 0) 
        goto exit;
    fwrite(buf, r - 1, 1, f);
exit:
    free(buf);
    fclose(f);
    return (r < 0) ? r : 0;
}
