/**
* Created by ApiGenerator Tool (C) on 17/9/2015 10:39:37 &#960;&#956;.
*/

// This is only for windows testing
#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif


#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include <string.h>
#include <malloc.h>
#include "jsmn.h"
#include "yodiwo_api.h"
#include "yodiwo_helpers.h"

/* ======================================================================================================================= */
/* ToJson Functions                                                                                                        */
/* ======================================================================================================================= */


// Helper functions to print arrays
// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_ConfigParameter_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_ConfigParameter_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_ConfigParameter_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_Port_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_Port_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_Port_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_ConfigDescription_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_ConfigDescription_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_ConfigDescription_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_PortDescription_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_PortDescription_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_PortDescription_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_NodeModelType_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_NodeModelType_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_NodeModelType_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_NodeThingType_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_NodeThingType_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_NodeThingType_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_Thing_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_Thing_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_Thing_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_PortEvent_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_PortEvent_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_PortEvent_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_PortState_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_PortState_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_PortState_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Tools_APIGenerator_CNodeConfig_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Tools_APIGenerator_CNodeConfig_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Tools_APIGenerator_CNodeConfig_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}




// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_UserKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_UserKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"UserID\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->UserID); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodeKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodeKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"UserKey\" : ");
	if ((len = Yodiwo_Plegma_UserKey_ToJson(json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"NodeID\" : %d", value->NodeID); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ThingKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"NodeKey\" : ");
	if ((len = Yodiwo_Plegma_NodeKey_ToJson(json, jsonEnd - json, &value->NodeKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"ThingUID\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->ThingUID); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"ThingKey\" : ");
	if ((len = Yodiwo_Plegma_ThingKey_ToJson(json, jsonEnd - json, &value->ThingKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"PortUID\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->PortUID); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_GraphDescriptorBaseKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorBaseKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"UserKey\" : ");
	if ((len = Yodiwo_Plegma_UserKey_ToJson(json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"Id\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Id); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_GraphDescriptorKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"UserKey\" : ");
	if ((len = Yodiwo_Plegma_UserKey_ToJson(json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"Id\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Id); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Revision\" : %d", value->Revision); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_GraphKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_GraphKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"GraphDescriptorKey\" : ");
	if ((len = Yodiwo_Plegma_GraphDescriptorKey_ToJson(json, jsonEnd - json, &value->GraphDescriptorKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"GraphId\" : %d", value->GraphId); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_BlockKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_BlockKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"GraphKey\" : ");
	if ((len = Yodiwo_Plegma_GraphKey_ToJson(json, jsonEnd - json, &value->GraphKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"BlockId\" : %d", value->BlockId); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_TimelineDescriptorKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_TimelineDescriptorKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"UserKey\" : ");
	if ((len = Yodiwo_Plegma_UserKey_ToJson(json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"Id\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Id); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_Mqtt_MqttAPIMessage_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_Mqtt_MqttAPIMessage_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"ResponseToSeqNo\" : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Payload\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Payload); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_Port_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_Port_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"PortKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->PortKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Description); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"ioDirection\" : %d", value->ioDirection); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Type\" : %d", value->Type); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"State\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->State); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %d", value->RevNum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"ConfFlags\" : %d", value->ConfFlags); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ConfigParameter_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ConfigParameter_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Value\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Value); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ThingUIHints_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingUIHints_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"IconURI\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->IconURI); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Description); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_Thing_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_Thing_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"ThingKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->ThingKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Config\" : ");
	if ((len = Array_Yodiwo_Plegma_ConfigParameter_ToJson(json, jsonEnd - json, &value->Config) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Ports\" : ");
	if ((len = Array_Yodiwo_Plegma_Port_ToJson(json, jsonEnd - json, &value->Ports) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"Type\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Type); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"BlockType\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->BlockType); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"UIHints\" : ");
	if ((len = Yodiwo_Plegma_ThingUIHints_ToJson(json, jsonEnd - json, &value->UIHints) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_LoginReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_LoginReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_LoginRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_LoginRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"NodeKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->NodeKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"SecretKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->SecretKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_StateDescription_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_StateDescription_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Minimum\" : %lf", value->Minimum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Maximum\" : %lf", value->Maximum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Step\" : %lf", value->Step); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Pattern\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Pattern); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"ReadOnly\" : %s", (value->ReadOnly) ? "true" : "false"); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ConfigDescription_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ConfigDescription_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"DefaultValue\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->DefaultValue); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Description); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Label\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Label); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Required\" : %s", (value->Required) ? "true" : "false"); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Type\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Type); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Minimum\" : %lf", value->Minimum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Maximum\" : %lf", value->Maximum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Stepsize\" : %lf", value->Stepsize); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"ReadOnly\" : %s", (value->ReadOnly) ? "true" : "false"); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortDescription_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortDescription_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Description); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Id\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Id); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Label\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Label); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Category\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Category); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"State\" : ");
	if ((len = Yodiwo_Plegma_StateDescription_ToJson(json, jsonEnd - json, &value->State) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodeModelType_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodeModelType_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Id\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Id); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Description); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Config\" : ");
	if ((len = Array_Yodiwo_Plegma_ConfigDescription_ToJson(json, jsonEnd - json, &value->Config) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Port\" : ");
	if ((len = Array_Yodiwo_Plegma_PortDescription_ToJson(json, jsonEnd - json, &value->Port) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodeThingType_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodeThingType_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Type\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Type); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Searchable\" : %s", (value->Searchable) ? "true" : "false"); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Description); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Model\" : ");
	if ((len = Array_Yodiwo_Plegma_NodeModelType_ToJson(json, jsonEnd - json, &value->Model) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodeInfoReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodeInfoReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"RequestedThingType\" : ");
	if ((len = Yodiwo_Plegma_NodeThingType_ToJson(json, jsonEnd - json, &value->RequestedThingType) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodeInfoRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodeInfoRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Type\" : %d", value->Type); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Capabilities\" : %d", value->Capabilities); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"ThingTypes\" : ");
	if ((len = Array_Yodiwo_Plegma_NodeThingType_ToJson(json, jsonEnd - json, &value->ThingTypes) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ThingsReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingsReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Operation\" : %d", value->Operation); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"ThingKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->ThingKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Data\" : ");
	if ((len = Array_Yodiwo_Plegma_Thing_ToJson(json, jsonEnd - json, &value->Data) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ThingsRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingsRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Operation\" : %d", value->Operation); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Status\" : %s", (value->Status) ? "true" : "false"); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Data\" : ");
	if ((len = Array_Yodiwo_Plegma_Thing_ToJson(json, jsonEnd - json, &value->Data) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortEvent_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortEvent_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"PortKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->PortKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"State\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->State); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %d", value->RevNum); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortEventMsg_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortEventMsg_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"PortEvents\" : ");
	if ((len = Array_Yodiwo_Plegma_PortEvent_ToJson(json, jsonEnd - json, &value->PortEvents) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortStateReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortStateReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Operation\" : %d", value->Operation); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"PortKeys\" : ");
	if ((len = Array_string_ToJson(json, jsonEnd - json, &value->PortKeys) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortState_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortState_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"PortKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->PortKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"State\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->State); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %d", value->RevNum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"IsDeployed\" : %s", (value->IsDeployed) ? "true" : "false"); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortStateRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortStateRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Operation\" : %d", value->Operation); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"PortStates\" : ");
	if ((len = Array_Yodiwo_Plegma_PortState_ToJson(json, jsonEnd - json, &value->PortStates) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ActivePortKeysMsg_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ActivePortKeysMsg_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"ActivePortKeys\" : ");
	if ((len = Array_string_ToJson(json, jsonEnd - json, &value->ActivePortKeys) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"uuid\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->uuid); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"uuid\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->uuid); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"token1\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->token1); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"token1\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->token1); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"token2\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->token2); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"nodeKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->nodeKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"secretKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->secretKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"userNodeRegistrationUrl\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->userNodeRegistrationUrl); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"token2\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->token2); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Tools_APIGenerator_CNodeConfig_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeConfig_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Uuid\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Uuid); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Name\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Name); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"NodeKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->NodeKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"NodeSecret\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->NodeSecret); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"PairingServerUrl\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->PairingServerUrl); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"YPChannelServer\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->YPChannelServer); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"YPChannelServerPort\" : %d", value->YPChannelServerPort); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"WebPort\" : %d", value->WebPort); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"MqttBrokerHostname\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->MqttBrokerHostname); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"MqttBrokerPort\" : %d", value->MqttBrokerPort); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"MqttBrokerCertFile\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->MqttBrokerCertFile); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Tools_APIGenerator_CNodeYConfig_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeYConfig_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"ActiveID\" : %d", value->ActiveID); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Configs\" : ");
	if ((len = Array_Yodiwo_Tools_APIGenerator_CNodeConfig_ToJson(json, jsonEnd - json, &value->Configs) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}





/* ======================================================================================================================= */
/* FromJson Functions                                                                                                      */
/* ======================================================================================================================= */
// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_ConfigParameter_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_ConfigParameter_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_ConfigParameter_t *)malloc(array->num*sizeof(Yodiwo_Plegma_ConfigParameter_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_ConfigParameter_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_Port_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_Port_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_Port_t *)malloc(array->num*sizeof(Yodiwo_Plegma_Port_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_Port_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_ConfigDescription_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_ConfigDescription_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_ConfigDescription_t *)malloc(array->num*sizeof(Yodiwo_Plegma_ConfigDescription_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_ConfigDescription_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_PortDescription_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_PortDescription_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_PortDescription_t *)malloc(array->num*sizeof(Yodiwo_Plegma_PortDescription_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_PortDescription_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_NodeModelType_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_NodeModelType_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_NodeModelType_t *)malloc(array->num*sizeof(Yodiwo_Plegma_NodeModelType_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_NodeModelType_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_NodeThingType_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_NodeThingType_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_NodeThingType_t *)malloc(array->num*sizeof(Yodiwo_Plegma_NodeThingType_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_NodeThingType_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_Thing_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_Thing_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_Thing_t *)malloc(array->num*sizeof(Yodiwo_Plegma_Thing_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_Thing_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_PortEvent_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_PortEvent_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_PortEvent_t *)malloc(array->num*sizeof(Yodiwo_Plegma_PortEvent_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_PortEvent_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_PortState_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_PortState_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_PortState_t *)malloc(array->num*sizeof(Yodiwo_Plegma_PortState_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_PortState_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Tools_APIGenerator_CNodeConfig_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Tools_APIGenerator_CNodeConfig_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Tools_APIGenerator_CNodeConfig_t *)malloc(array->num*sizeof(Yodiwo_Tools_APIGenerator_CNodeConfig_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Tools_APIGenerator_CNodeConfig_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_UserKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_UserKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_UserKey_t));
	ParseTable table[] = {
		{ "UserID", 6, Parse_String, NULL, (void **)&value->UserID },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeKey_t));
	ParseTable table[] = {
		{ "UserKey", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_UserKey_FromJson, (void **)&value->UserKey },
		{ "NodeID", 6, Parse_Int, NULL, (void **)&value->NodeID },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingKey_t));
	ParseTable table[] = {
		{ "NodeKey", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_NodeKey_FromJson, (void **)&value->NodeKey },
		{ "ThingUID", 8, Parse_String, NULL, (void **)&value->ThingUID },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortKey_t));
	ParseTable table[] = {
		{ "ThingKey", 8, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_ThingKey_FromJson, (void **)&value->ThingKey },
		{ "PortUID", 7, Parse_String, NULL, (void **)&value->PortUID },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDescriptorBaseKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorBaseKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_GraphDescriptorBaseKey_t));
	ParseTable table[] = {
		{ "UserKey", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_UserKey_FromJson, (void **)&value->UserKey },
		{ "Id", 2, Parse_String, NULL, (void **)&value->Id },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDescriptorKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_GraphDescriptorKey_t));
	ParseTable table[] = {
		{ "UserKey", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_UserKey_FromJson, (void **)&value->UserKey },
		{ "Id", 2, Parse_String, NULL, (void **)&value->Id },
		{ "Revision", 8, Parse_Int, NULL, (void **)&value->Revision },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_GraphKey_t));
	ParseTable table[] = {
		{ "GraphDescriptorKey", 18, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_GraphDescriptorKey_FromJson, (void **)&value->GraphDescriptorKey },
		{ "GraphId", 7, Parse_Int, NULL, (void **)&value->GraphId },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_BlockKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_BlockKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_BlockKey_t));
	ParseTable table[] = {
		{ "GraphKey", 8, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_GraphKey_FromJson, (void **)&value->GraphKey },
		{ "BlockId", 7, Parse_Int, NULL, (void **)&value->BlockId },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_TimelineDescriptorKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_TimelineDescriptorKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_TimelineDescriptorKey_t));
	ParseTable table[] = {
		{ "UserKey", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_UserKey_FromJson, (void **)&value->UserKey },
		{ "Id", 2, Parse_String, NULL, (void **)&value->Id },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_Mqtt_MqttAPIMessage_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Mqtt_MqttAPIMessage_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_Mqtt_MqttAPIMessage_t));
	ParseTable table[] = {
		{ "ResponseToSeqNo", 15, Parse_Int, NULL, (void **)&value->ResponseToSeqNo },
		{ "Payload", 7, Parse_String, NULL, (void **)&value->Payload },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_Port_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Port_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_Port_t));
	ParseTable table[] = {
		{ "PortKey", 7, Parse_String, NULL, (void **)&value->PortKey },
		{ "Name", 4, Parse_String, NULL, (void **)&value->Name },
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
		{ "ioDirection", 11, Parse_Int, NULL, (void **)&value->ioDirection },
		{ "Type", 4, Parse_Int, NULL, (void **)&value->Type },
		{ "State", 5, Parse_String, NULL, (void **)&value->State },
		{ "RevNum", 6, Parse_Int, NULL, (void **)&value->RevNum },
		{ "ConfFlags", 9, Parse_Int, NULL, (void **)&value->ConfFlags },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigParameter_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigParameter_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ConfigParameter_t));
	ParseTable table[] = {
		{ "Name", 4, Parse_String, NULL, (void **)&value->Name },
		{ "Value", 5, Parse_String, NULL, (void **)&value->Value },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingUIHints_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingUIHints_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingUIHints_t));
	ParseTable table[] = {
		{ "IconURI", 7, Parse_String, NULL, (void **)&value->IconURI },
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_Thing_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Thing_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_Thing_t));
	ParseTable table[] = {
		{ "ThingKey", 8, Parse_String, NULL, (void **)&value->ThingKey },
		{ "Name", 4, Parse_String, NULL, (void **)&value->Name },
		{ "Config", 6, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_ConfigParameter_FromJson, (void **)&value->Config },
		{ "Ports", 5, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_Port_FromJson, (void **)&value->Ports },
		{ "Type", 4, Parse_String, NULL, (void **)&value->Type },
		{ "BlockType", 9, Parse_String, NULL, (void **)&value->BlockType },
		{ "UIHints", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_ThingUIHints_FromJson, (void **)&value->UIHints },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_LoginReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_LoginRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "NodeKey", 7, Parse_String, NULL, (void **)&value->NodeKey },
		{ "SecretKey", 9, Parse_String, NULL, (void **)&value->SecretKey },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_StateDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_StateDescription_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_StateDescription_t));
	ParseTable table[] = {
		{ "Minimum", 7, Parse_Double, NULL, (void **)&value->Minimum },
		{ "Maximum", 7, Parse_Double, NULL, (void **)&value->Maximum },
		{ "Step", 4, Parse_Double, NULL, (void **)&value->Step },
		{ "Pattern", 7, Parse_String, NULL, (void **)&value->Pattern },
		{ "ReadOnly", 8, Parse_Bool, NULL, (void **)&value->ReadOnly },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigDescription_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ConfigDescription_t));
	ParseTable table[] = {
		{ "DefaultValue", 12, Parse_String, NULL, (void **)&value->DefaultValue },
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
		{ "Label", 5, Parse_String, NULL, (void **)&value->Label },
		{ "Name", 4, Parse_String, NULL, (void **)&value->Name },
		{ "Required", 8, Parse_Bool, NULL, (void **)&value->Required },
		{ "Type", 4, Parse_String, NULL, (void **)&value->Type },
		{ "Minimum", 7, Parse_Double, NULL, (void **)&value->Minimum },
		{ "Maximum", 7, Parse_Double, NULL, (void **)&value->Maximum },
		{ "Stepsize", 8, Parse_Double, NULL, (void **)&value->Stepsize },
		{ "ReadOnly", 8, Parse_Bool, NULL, (void **)&value->ReadOnly },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortDescription_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortDescription_t));
	ParseTable table[] = {
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
		{ "Id", 2, Parse_String, NULL, (void **)&value->Id },
		{ "Label", 5, Parse_String, NULL, (void **)&value->Label },
		{ "Category", 8, Parse_String, NULL, (void **)&value->Category },
		{ "State", 5, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_StateDescription_FromJson, (void **)&value->State },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeModelType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeModelType_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeModelType_t));
	ParseTable table[] = {
		{ "Id", 2, Parse_String, NULL, (void **)&value->Id },
		{ "Name", 4, Parse_String, NULL, (void **)&value->Name },
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
		{ "Config", 6, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_ConfigDescription_FromJson, (void **)&value->Config },
		{ "Port", 4, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_PortDescription_FromJson, (void **)&value->Port },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeThingType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeThingType_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeThingType_t));
	ParseTable table[] = {
		{ "Type", 4, Parse_String, NULL, (void **)&value->Type },
		{ "Searchable", 10, Parse_Bool, NULL, (void **)&value->Searchable },
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
		{ "Model", 5, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_NodeModelType_FromJson, (void **)&value->Model },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeInfoReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "RequestedThingType", 18, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_NodeThingType_FromJson, (void **)&value->RequestedThingType },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeInfoRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Name", 4, Parse_String, NULL, (void **)&value->Name },
		{ "Type", 4, Parse_Int, NULL, (void **)&value->Type },
		{ "Capabilities", 12, Parse_Int, NULL, (void **)&value->Capabilities },
		{ "ThingTypes", 10, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_NodeThingType_FromJson, (void **)&value->ThingTypes },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingsReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Operation", 9, Parse_Int, NULL, (void **)&value->Operation },
		{ "ThingKey", 8, Parse_String, NULL, (void **)&value->ThingKey },
		{ "Data", 4, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_Thing_FromJson, (void **)&value->Data },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingsRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Operation", 9, Parse_Int, NULL, (void **)&value->Operation },
		{ "Status", 6, Parse_Bool, NULL, (void **)&value->Status },
		{ "Data", 4, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_Thing_FromJson, (void **)&value->Data },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEvent_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEvent_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortEvent_t));
	ParseTable table[] = {
		{ "PortKey", 7, Parse_String, NULL, (void **)&value->PortKey },
		{ "State", 5, Parse_String, NULL, (void **)&value->State },
		{ "RevNum", 6, Parse_Int, NULL, (void **)&value->RevNum },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEventMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEventMsg_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortEventMsg_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "PortEvents", 10, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_PortEvent_FromJson, (void **)&value->PortEvents },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortStateReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Operation", 9, Parse_Int, NULL, (void **)&value->Operation },
		{ "PortKeys", 8, NULL, (ParseFuncSubStruct *)Array_string_FromJson, (void **)&value->PortKeys },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortState_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortState_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortState_t));
	ParseTable table[] = {
		{ "PortKey", 7, Parse_String, NULL, (void **)&value->PortKey },
		{ "State", 5, Parse_String, NULL, (void **)&value->State },
		{ "RevNum", 6, Parse_Int, NULL, (void **)&value->RevNum },
		{ "IsDeployed", 10, Parse_Bool, NULL, (void **)&value->IsDeployed },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortStateRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Operation", 9, Parse_Int, NULL, (void **)&value->Operation },
		{ "PortStates", 10, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_PortState_FromJson, (void **)&value->PortStates },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ActivePortKeysMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ActivePortKeysMsg_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ActivePortKeysMsg_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "ActivePortKeys", 14, NULL, (ParseFuncSubStruct *)Array_string_FromJson, (void **)&value->ActivePortKeys },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t));
	ParseTable table[] = {
		{ "uuid", 4, Parse_String, NULL, (void **)&value->uuid },
		{ "name", 4, Parse_String, NULL, (void **)&value->name },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t));
	ParseTable table[] = {
		{ "uuid", 4, Parse_String, NULL, (void **)&value->uuid },
		{ "token1", 6, Parse_String, NULL, (void **)&value->token1 },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t));
	ParseTable table[] = {
		{ "token1", 6, Parse_String, NULL, (void **)&value->token1 },
		{ "token2", 6, Parse_String, NULL, (void **)&value->token2 },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t));
	ParseTable table[] = {
		{ "nodeKey", 7, Parse_String, NULL, (void **)&value->nodeKey },
		{ "secretKey", 9, Parse_String, NULL, (void **)&value->secretKey },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t));
	ParseTable table[] = {
		{ "userNodeRegistrationUrl", 23, Parse_String, NULL, (void **)&value->userNodeRegistrationUrl },
		{ "token2", 6, Parse_String, NULL, (void **)&value->token2 },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Tools_APIGenerator_CNodeConfig_FromJson(char* json, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeConfig_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Tools_APIGenerator_CNodeConfig_t));
	ParseTable table[] = {
		{ "Uuid", 4, Parse_String, NULL, (void **)&value->Uuid },
		{ "Name", 4, Parse_String, NULL, (void **)&value->Name },
		{ "NodeKey", 7, Parse_String, NULL, (void **)&value->NodeKey },
		{ "NodeSecret", 10, Parse_String, NULL, (void **)&value->NodeSecret },
		{ "PairingServerUrl", 16, Parse_String, NULL, (void **)&value->PairingServerUrl },
		{ "YPChannelServer", 15, Parse_String, NULL, (void **)&value->YPChannelServer },
		{ "YPChannelServerPort", 19, Parse_Int, NULL, (void **)&value->YPChannelServerPort },
		{ "WebPort", 7, Parse_Int, NULL, (void **)&value->WebPort },
		{ "MqttBrokerHostname", 18, Parse_String, NULL, (void **)&value->MqttBrokerHostname },
		{ "MqttBrokerPort", 14, Parse_Int, NULL, (void **)&value->MqttBrokerPort },
		{ "MqttBrokerCertFile", 18, Parse_String, NULL, (void **)&value->MqttBrokerCertFile },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Tools_APIGenerator_CNodeYConfig_FromJson(char* json, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeYConfig_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Tools_APIGenerator_CNodeYConfig_t));
	ParseTable table[] = {
		{ "ActiveID", 8, Parse_Int, NULL, (void **)&value->ActiveID },
		{ "Configs", 7, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Tools_APIGenerator_CNodeConfig_FromJson, (void **)&value->Configs },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}


