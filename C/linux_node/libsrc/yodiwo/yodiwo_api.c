/**
* Created by ApiGenerator Tool (C) on 26-Apr-16 11:24:51 AM.
*/

// This is only for windows testing
#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif


#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include <string.h>
#include <stdlib.h>
#include "jsmn.h"
#include "yodiwo_api.h"
#include "yodiwo_helpers.h"

/* ======================================================================================================================= */
/* ToJson Functions                                                                                                        */
/* ======================================================================================================================= */


// Helper functions to print arrays
	// -----------------------------------------------------------------------------------------------------------------------
int Array_byte_ToJson(char* jsonStart, size_t jsonSize, Array_byte_t *array)
{
	int error = 1 / 0;
	//TODO: implement this, should be something like a base64 encoder
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
//	*json = '['; json++;
//	if (array != NULL) {
//		for (i = 0; i < array->num; i++) {
//			if ((len = byte_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
//			*json = ','; json++;
//		}
//		if (i > 0) json--; // remove last ,
//	}
//	*json = ']'; json++;
//	*json = '\0'; json++;
	return json - jsonStart;
}

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
int Array_Yodiwo_Plegma_ThingModelType_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_ThingModelType_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_ThingModelType_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Array_Yodiwo_Plegma_ThingType_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_ThingType_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_ThingType_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
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
int Array_Yodiwo_Plegma_VirtualBlockEvent_ToJson(char* jsonStart, size_t jsonSize, Array_Yodiwo_Plegma_VirtualBlockEvent_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = Yodiwo_Plegma_VirtualBlockEvent_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
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
int Array_APIGenerator_AdditionalTypes_CNodeConfig_ToJson(char* jsonStart, size_t jsonSize, Array_APIGenerator_AdditionalTypes_CNodeConfig_t *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num; i++) {
			if ((len = APIGenerator_AdditionalTypes_CNodeConfig_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1) < 0) return -1; else json += len;
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
	json += snprintf(json, jsonEnd - json, ", \"NodeID\" : %u", value->NodeID); if (json >= jsonEnd) return -1;
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
	json += snprintf(json, jsonEnd - json, "%s", "{ \"BaseKey\" : ");
	if ((len = Yodiwo_Plegma_GraphDescriptorBaseKey_ToJson(json, jsonEnd - json, &value->BaseKey) - 1) < 0) return -1; else json += len;
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
	json += snprintf(json, jsonEnd - json, ", \"NodeId\" : %u", value->NodeId); if (json >= jsonEnd) return -1;
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
int Yodiwo_Plegma_GroupKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_GroupKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "%s", "{ \"UserKey\" : ");
	if ((len = Yodiwo_Plegma_UserKey_ToJson(json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"GroupID\" : %d", value->GroupID); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_DriverKey_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_DriverKey_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Type\" : %d", value->Type); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Id\" : %d", value->Id); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_WrapperMsg_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_WrapperMsg_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Flags\" : %d", value->Flags); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"SyncId\" : %d", value->SyncId); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Payload\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Payload); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"PayloadSize\" : %d", value->PayloadSize); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_WebSocketMsg_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_WebSocketMsg_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Flags\" : %d", value->Flags); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"SyncId\" : %d", value->SyncId); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Payload\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Payload); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"PayloadSize\" : %d", value->PayloadSize); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Id\" : %d", value->Id); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"SubId\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->SubId); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_MqttMsg_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_MqttMsg_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Flags\" : %d", value->Flags); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"SyncId\" : %d", value->SyncId); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Payload\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Payload); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"PayloadSize\" : %d", value->PayloadSize); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_YColor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_YColor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"R\" : %f", value->R); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"G\" : %f", value->G); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"B\" : %f", value->B); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"A\" : %f", value->A); if (json >= jsonEnd) return -1;
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
	json += snprintf(json, jsonEnd - json, ", \"Semantics\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Semantics); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"State\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->State); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %u", value->RevNum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"ConfFlags\" : %d", value->ConfFlags); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_HttpLocationDescriptor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_HttpLocationDescriptor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Uri\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Uri); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"RestServiceType\" : %d", value->RestServiceType); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_RedisDBLocationDescriptor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_RedisDBLocationDescriptor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"ConnectionAddress\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->ConnectionAddress); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"DatabaseName\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->DatabaseName); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_DataContentDescriptor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_DataContentDescriptor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_TextContentDescriptor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_TextContentDescriptor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ImageContentDescriptor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ImageContentDescriptor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Type\" : %d", value->Type); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Format\" : %d", value->Format); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"PixelSizeX\" : %d", value->PixelSizeX); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"PixelSizeY\" : %d", value->PixelSizeY); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"ColorDepth\" : %d", value->ColorDepth); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_AudioContentDescriptor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_AudioContentDescriptor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_VideoContentDescriptor_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_VideoContentDescriptor_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
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
	json += snprintf(json, jsonEnd - json, ", \"Description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Description); if (json + 1 >= jsonEnd) return -1;
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
	json += snprintf(json, jsonEnd - json, ", \"Removable\" : %s", (value->Removable) ? "true" : "false"); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"RESTUri\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->RESTUri); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"UIHints\" : ");
	if ((len = Yodiwo_Plegma_ThingUIHints_ToJson(json, jsonEnd - json, &value->UIHints) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_GenericRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_GenericRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"IsSuccess\" : %s", (value->IsSuccess) ? "true" : "false"); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"StatusCode\" : %d", value->StatusCode); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Message\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Message); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
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
	json += snprintf(json, jsonEnd - json, ", \"Flags\" : %d", value->Flags); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"DesiredEndpoint\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->DesiredEndpoint); if (json + 1 >= jsonEnd) return -1;
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
	json += snprintf(json, jsonEnd - json, ", \"Type\" : %d", value->Type); if (json >= jsonEnd) return -1;
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
int Yodiwo_Plegma_ThingModelType_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingModelType_t *value)
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
int Yodiwo_Plegma_ThingType_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingType_t *value)
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
	if ((len = Array_Yodiwo_Plegma_ThingModelType_ToJson(json, jsonEnd - json, &value->Model) - 1) < 0) return -1; else json += len;
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
	json += snprintf(json, jsonEnd - json, ", \"LatestApiRev\" : %d", value->LatestApiRev); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"AssignedEndpoint\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->AssignedEndpoint); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"ThingsRevNum\" : %d", value->ThingsRevNum); if (json >= jsonEnd) return -1;
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
	if ((len = Array_Yodiwo_Plegma_ThingType_ToJson(json, jsonEnd - json, &value->ThingTypes) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"ThingsRevNum\" : %d", value->ThingsRevNum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"SupportedApiRev\" : %d", value->SupportedApiRev); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"BlockLibraries\" : ");
	if ((len = Array_string_ToJson(json, jsonEnd - json, &value->BlockLibraries) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodeUnpairedReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"ReasonCode\" : %d", value->ReasonCode); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Message\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Message); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodeUnpairedRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_EndpointSyncReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_EndpointSyncReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"op\" : %d", value->op); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"DesiredEndpoint\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->DesiredEndpoint); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_EndpointSyncRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_EndpointSyncRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"op\" : %d", value->op); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Endpoints\" : ");
	if ((len = Array_string_ToJson(json, jsonEnd - json, &value->Endpoints) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"Accepted\" : %s", (value->Accepted) ? "true" : "false"); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ThingsGet_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingsGet_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Operation\" : %d", value->Operation); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"ThingKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->ThingKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"Key\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->Key); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %d", value->RevNum); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ThingsSet_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingsSet_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Operation\" : %d", value->Operation); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Status\" : %s", (value->Status) ? "true" : "false"); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Data\" : ");
	if ((len = Array_Yodiwo_Plegma_Thing_ToJson(json, jsonEnd - json, &value->Data) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %d", value->RevNum); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PingReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PingReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Data\" : %d", value->Data); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PingRsp_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PingRsp_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Data\" : %d", value->Data); if (json >= jsonEnd) return -1;
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
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %u", value->RevNum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Timestamp\" : %lu", value->Timestamp); if (json >= jsonEnd) return -1;
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
int Yodiwo_Plegma_VirtualBlockEvent_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEvent_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"BlockKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->BlockKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Indices\" : ");
	if ((len = Array_byte_ToJson(json, jsonEnd - json, &value->Indices) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Values\" : ");
	if ((len = Array_string_ToJson(json, jsonEnd - json, &value->Values) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %lu", value->RevNum); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_VirtualBlockEventMsg_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEventMsg_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"BlockEvents\" : ");
	if ((len = Array_Yodiwo_Plegma_VirtualBlockEvent_ToJson(json, jsonEnd - json, &value->BlockEvents) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortStateGet_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortStateGet_t *value)
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
	json += snprintf(json, jsonEnd - json, ", \"RevNum\" : %u", value->RevNum); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"IsDeployed\" : %s", (value->IsDeployed) ? "true" : "false"); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortStateSet_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortStateSet_t *value)
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
int Yodiwo_Plegma_LocallyDeployedGraphsMsg_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_LocallyDeployedGraphsMsg_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"DeployedGraphKeys\" : ");
	if ((len = Array_string_ToJson(json, jsonEnd - json, &value->DeployedGraphKeys) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_GraphDeploymentReq_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_GraphDeploymentReq_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"SeqNo\" : %d", value->SeqNo); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"GraphKey\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->GraphKey); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"IsDeployed\" : %s", (value->IsDeployed) ? "true" : "false"); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"GraphDescriptor\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->GraphDescriptor); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ThingTypeLibrary_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ThingTypeLibrary_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_ModelTypeLibrary_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_ModelTypeLibrary_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
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
	json += snprintf(json, jsonEnd - json, ", \"RedirectUri\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->RedirectUri); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"image\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->image); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"description\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->description); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"pathcss\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->pathcss); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, ", \"PairingCompletionInstructions\" : \""); if (json >= jsonEnd) return -1;
	json += strcpy_escaped(json, value->PairingCompletionInstructions); if (json + 1 >= jsonEnd) return -1;
	*json = '\"'; json++;
	json += snprintf(json, jsonEnd - json, "%s", ", \"PublicKey\" : ");
	if ((len = Array_byte_ToJson(json, jsonEnd - json, &value->PublicKey) - 1) < 0) return -1; else json += len;
	json += snprintf(json, jsonEnd - json, ", \"NoUUIDAuthentication\" : %s", (value->NoUUIDAuthentication) ? "true" : "false"); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value)
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
int Yodiwo_Plegma_PortStateSemantics_Decimal_Range_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortStateSemantics_Decimal_Range_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Min\" : %lf", value->Min); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Max\" : %lf", value->Max); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int Yodiwo_Plegma_PortStateSemantics_Integer_Range_ToJson(char* jsonStart, size_t jsonSize, Yodiwo_Plegma_PortStateSemantics_Integer_Range_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"Min\" : %ld", value->Min); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, ", \"Max\" : %ld", value->Max); if (json >= jsonEnd) return -1;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}

// -----------------------------------------------------------------------------------------------------------------------
int APIGenerator_AdditionalTypes_CNodeConfig_ToJson(char* jsonStart, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeConfig_t *value)
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
int APIGenerator_AdditionalTypes_CNodeCConfig_ToJson(char* jsonStart, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeCConfig_t *value)
{
	char *json = jsonStart, *jsonEnd = json + jsonSize;
	int len;
	json += snprintf(json, jsonEnd - json, "{ \"ActiveID\" : %d", value->ActiveID); if (json >= jsonEnd) return -1;
	json += snprintf(json, jsonEnd - json, "%s", ", \"Configs\" : ");
	if ((len = Array_APIGenerator_AdditionalTypes_CNodeConfig_ToJson(json, jsonEnd - json, &value->Configs) - 1) < 0) return -1; else json += len;
	*json = '}'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}





/* ======================================================================================================================= */
/* FromJson Functions                                                                                                      */
/* ======================================================================================================================= */
		// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_byte_FromJson(char* json, size_t jsonSize, Array_byte_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	int error = 1 / 0;
	//TODO: implement this, should be something like a base64 decoder
//	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
//	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;
//
//	array->num = Helper_Json_ParseArray(t, r);
//	array->elems = (byte_t *)malloc(array->num * sizeof(byte_t));
//	for (i = 0; i < array->num; i++) {
//		if ((res = byte_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
//	}
	return res;
}

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
	array->elems = (Yodiwo_Plegma_ConfigParameter_t *)malloc(array->num * sizeof(Yodiwo_Plegma_ConfigParameter_t));
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
	array->elems = (Yodiwo_Plegma_Port_t *)malloc(array->num * sizeof(Yodiwo_Plegma_Port_t));
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
	array->elems = (Yodiwo_Plegma_ConfigDescription_t *)malloc(array->num * sizeof(Yodiwo_Plegma_ConfigDescription_t));
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
	array->elems = (Yodiwo_Plegma_PortDescription_t *)malloc(array->num * sizeof(Yodiwo_Plegma_PortDescription_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_PortDescription_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_ThingModelType_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_ThingModelType_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_ThingModelType_t *)malloc(array->num * sizeof(Yodiwo_Plegma_ThingModelType_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_ThingModelType_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_ThingType_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_ThingType_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_ThingType_t *)malloc(array->num * sizeof(Yodiwo_Plegma_ThingType_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_ThingType_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
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
	array->elems = (Yodiwo_Plegma_Thing_t *)malloc(array->num * sizeof(Yodiwo_Plegma_Thing_t));
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
	array->elems = (Yodiwo_Plegma_PortEvent_t *)malloc(array->num * sizeof(Yodiwo_Plegma_PortEvent_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_PortEvent_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_VirtualBlockEvent_FromJson(char* json, size_t jsonSize, Array_Yodiwo_Plegma_VirtualBlockEvent_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (Yodiwo_Plegma_VirtualBlockEvent_t *)malloc(array->num * sizeof(Yodiwo_Plegma_VirtualBlockEvent_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_VirtualBlockEvent_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
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
	array->elems = (Yodiwo_Plegma_PortState_t *)malloc(array->num * sizeof(Yodiwo_Plegma_PortState_t));
	for (i = 0; i < array->num; i++) {
		if ((res = Yodiwo_Plegma_PortState_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
	}
	return res;
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_APIGenerator_AdditionalTypes_CNodeConfig_FromJson(char* json, size_t jsonSize, Array_APIGenerator_AdditionalTypes_CNodeConfig_t *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArray(t, r);
	array->elems = (APIGenerator_AdditionalTypes_CNodeConfig_t *)malloc(array->num * sizeof(APIGenerator_AdditionalTypes_CNodeConfig_t));
	for (i = 0; i < array->num; i++) {
		if ((res = APIGenerator_AdditionalTypes_CNodeConfig_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
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
		{ "BaseKey", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_GraphDescriptorBaseKey_FromJson, (void **)&value->BaseKey },
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
		{ "NodeId", 6, Parse_Int, NULL, (void **)&value->NodeId },
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
Yodiwo_Plegma_Json_e Yodiwo_Plegma_GroupKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GroupKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_GroupKey_t));
	ParseTable table[] = {
		{ "UserKey", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_UserKey_FromJson, (void **)&value->UserKey },
		{ "GroupID", 7, Parse_Int, NULL, (void **)&value->GroupID },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_DriverKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_DriverKey_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_DriverKey_t));
	ParseTable table[] = {
		{ "Type", 4, Parse_Int, NULL, (void **)&value->Type },
		{ "Id", 2, Parse_Int, NULL, (void **)&value->Id },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_WrapperMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_WrapperMsg_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_WrapperMsg_t));
	ParseTable table[] = {
		{ "Flags", 5, Parse_Int, NULL, (void **)&value->Flags },
		{ "SyncId", 6, Parse_Int, NULL, (void **)&value->SyncId },
		{ "Payload", 7, Parse_String, NULL, (void **)&value->Payload },
		{ "PayloadSize", 11, Parse_Int, NULL, (void **)&value->PayloadSize },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_WebSocketMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_WebSocketMsg_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_WebSocketMsg_t));
	ParseTable table[] = {
		{ "Flags", 5, Parse_Int, NULL, (void **)&value->Flags },
		{ "SyncId", 6, Parse_Int, NULL, (void **)&value->SyncId },
		{ "Payload", 7, Parse_String, NULL, (void **)&value->Payload },
		{ "PayloadSize", 11, Parse_Int, NULL, (void **)&value->PayloadSize },
		{ "Id", 2, Parse_Int, NULL, (void **)&value->Id },
		{ "SubId", 5, Parse_String, NULL, (void **)&value->SubId },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_MqttMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_MqttMsg_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_MqttMsg_t));
	ParseTable table[] = {
		{ "Flags", 5, Parse_Int, NULL, (void **)&value->Flags },
		{ "SyncId", 6, Parse_Int, NULL, (void **)&value->SyncId },
		{ "Payload", 7, Parse_String, NULL, (void **)&value->Payload },
		{ "PayloadSize", 11, Parse_Int, NULL, (void **)&value->PayloadSize },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_YColor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_YColor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_YColor_t));
	ParseTable table[] = {
		{ "R", 1, Parse_Float, NULL, (void **)&value->R },
		{ "G", 1, Parse_Float, NULL, (void **)&value->G },
		{ "B", 1, Parse_Float, NULL, (void **)&value->B },
		{ "A", 1, Parse_Float, NULL, (void **)&value->A },
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
		{ "Semantics", 9, Parse_String, NULL, (void **)&value->Semantics },
		{ "State", 5, Parse_String, NULL, (void **)&value->State },
		{ "RevNum", 6, Parse_Int, NULL, (void **)&value->RevNum },
		{ "ConfFlags", 9, Parse_Int, NULL, (void **)&value->ConfFlags },
		{ "LastUpdatedTimestamp", 20, Parse_Int, NULL, (void **)&value->LastUpdatedTimestamp },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_HttpLocationDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_HttpLocationDescriptor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_HttpLocationDescriptor_t));
	ParseTable table[] = {
		{ "Uri", 3, Parse_String, NULL, (void **)&value->Uri },
		{ "RestServiceType", 15, Parse_Int, NULL, (void **)&value->RestServiceType },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_RedisDBLocationDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_RedisDBLocationDescriptor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_RedisDBLocationDescriptor_t));
	ParseTable table[] = {
		{ "ConnectionAddress", 17, Parse_String, NULL, (void **)&value->ConnectionAddress },
		{ "DatabaseName", 12, Parse_String, NULL, (void **)&value->DatabaseName },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_DataContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_DataContentDescriptor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_DataContentDescriptor_t));
	ParseTable table[] = {
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_TextContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_TextContentDescriptor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_TextContentDescriptor_t));
	ParseTable table[] = {
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ImageContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ImageContentDescriptor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ImageContentDescriptor_t));
	ParseTable table[] = {
		{ "Type", 4, Parse_Int, NULL, (void **)&value->Type },
		{ "Format", 6, Parse_Int, NULL, (void **)&value->Format },
		{ "PixelSizeX", 10, Parse_Int, NULL, (void **)&value->PixelSizeX },
		{ "PixelSizeY", 10, Parse_Int, NULL, (void **)&value->PixelSizeY },
		{ "ColorDepth", 10, Parse_Int, NULL, (void **)&value->ColorDepth },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_AudioContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_AudioContentDescriptor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_AudioContentDescriptor_t));
	ParseTable table[] = {
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_VideoContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_VideoContentDescriptor_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_VideoContentDescriptor_t));
	ParseTable table[] = {
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
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
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
		{ "Removable", 9, Parse_Bool, NULL, (void **)&value->Removable },
		{ "RESTUri", 7, Parse_String, NULL, (void **)&value->RESTUri },
		{ "UIHints", 7, NULL, (ParseFuncSubStruct *)Yodiwo_Plegma_ThingUIHints_FromJson, (void **)&value->UIHints },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_GenericRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GenericRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_GenericRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "IsSuccess", 9, Parse_Bool, NULL, (void **)&value->IsSuccess },
		{ "StatusCode", 10, Parse_Int, NULL, (void **)&value->StatusCode },
		{ "Message", 7, Parse_String, NULL, (void **)&value->Message },
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
		{ "Flags", 5, Parse_Int, NULL, (void **)&value->Flags },
		{ "DesiredEndpoint", 15, Parse_String, NULL, (void **)&value->DesiredEndpoint },
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
		{ "Type", 4, Parse_Int, NULL, (void **)&value->Type },
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
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingModelType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingModelType_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingModelType_t));
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
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingType_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingType_t));
	ParseTable table[] = {
		{ "Type", 4, Parse_String, NULL, (void **)&value->Type },
		{ "Searchable", 10, Parse_Bool, NULL, (void **)&value->Searchable },
		{ "Description", 11, Parse_String, NULL, (void **)&value->Description },
		{ "Model", 5, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_ThingModelType_FromJson, (void **)&value->Model },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeInfoReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "LatestApiRev", 12, Parse_Int, NULL, (void **)&value->LatestApiRev },
		{ "AssignedEndpoint", 16, Parse_String, NULL, (void **)&value->AssignedEndpoint },
		{ "ThingsRevNum", 12, Parse_Int, NULL, (void **)&value->ThingsRevNum },
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
		{ "ThingTypes", 10, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_ThingType_FromJson, (void **)&value->ThingTypes },
		{ "ThingsRevNum", 12, Parse_Int, NULL, (void **)&value->ThingsRevNum },
		{ "SupportedApiRev", 15, Parse_Int, NULL, (void **)&value->SupportedApiRev },
		{ "BlockLibraries", 14, NULL, (ParseFuncSubStruct *)Array_string_FromJson, (void **)&value->BlockLibraries },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeUnpairedReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeUnpairedReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "ReasonCode", 10, Parse_Int, NULL, (void **)&value->ReasonCode },
		{ "Message", 7, Parse_String, NULL, (void **)&value->Message },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeUnpairedRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodeUnpairedRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_EndpointSyncReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_EndpointSyncReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_EndpointSyncReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "op", 2, Parse_Int, NULL, (void **)&value->op },
		{ "DesiredEndpoint", 15, Parse_String, NULL, (void **)&value->DesiredEndpoint },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_EndpointSyncRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_EndpointSyncRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_EndpointSyncRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "op", 2, Parse_Int, NULL, (void **)&value->op },
		{ "Endpoints", 9, NULL, (ParseFuncSubStruct *)Array_string_FromJson, (void **)&value->Endpoints },
		{ "Accepted", 8, Parse_Bool, NULL, (void **)&value->Accepted },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsGet_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsGet_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingsGet_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Operation", 9, Parse_Int, NULL, (void **)&value->Operation },
		{ "ThingKey", 8, Parse_String, NULL, (void **)&value->ThingKey },
		{ "Key", 3, Parse_String, NULL, (void **)&value->Key },
		{ "RevNum", 6, Parse_Int, NULL, (void **)&value->RevNum },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsSet_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsSet_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_ThingsSet_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Operation", 9, Parse_Int, NULL, (void **)&value->Operation },
		{ "Status", 6, Parse_Bool, NULL, (void **)&value->Status },
		{ "Data", 4, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_Thing_FromJson, (void **)&value->Data },
		{ "RevNum", 6, Parse_Int, NULL, (void **)&value->RevNum },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PingReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PingReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PingReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Data", 4, Parse_Int, NULL, (void **)&value->Data },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PingRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PingRsp_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PingRsp_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "Data", 4, Parse_Int, NULL, (void **)&value->Data },
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
		{ "Timestamp", 9, Parse_Int, NULL, (void **)&value->Timestamp },
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
Yodiwo_Plegma_Json_e Yodiwo_Plegma_VirtualBlockEvent_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEvent_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_VirtualBlockEvent_t));
	ParseTable table[] = {
		{ "BlockKey", 8, Parse_String, NULL, (void **)&value->BlockKey },
		{ "Indices", 7, NULL, (ParseFuncSubStruct *)Array_byte_FromJson, (void **)&value->Indices },
		{ "Values", 6, NULL, (ParseFuncSubStruct *)Array_string_FromJson, (void **)&value->Values },
		{ "RevNum", 6, Parse_Int, NULL, (void **)&value->RevNum },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_VirtualBlockEventMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEventMsg_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_VirtualBlockEventMsg_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "BlockEvents", 11, NULL, (ParseFuncSubStruct *)Array_Yodiwo_Plegma_VirtualBlockEvent_FromJson, (void **)&value->BlockEvents },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateGet_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateGet_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortStateGet_t));
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
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateSet_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateSet_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortStateSet_t));
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
Yodiwo_Plegma_Json_e Yodiwo_Plegma_LocallyDeployedGraphsMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LocallyDeployedGraphsMsg_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_LocallyDeployedGraphsMsg_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "DeployedGraphKeys", 17, NULL, (ParseFuncSubStruct *)Array_string_FromJson, (void **)&value->DeployedGraphKeys },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDeploymentReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDeploymentReq_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_GraphDeploymentReq_t));
	ParseTable table[] = {
		{ "SeqNo", 5, Parse_Int, NULL, (void **)&value->SeqNo },
		{ "GraphKey", 8, Parse_String, NULL, (void **)&value->GraphKey },
		{ "IsDeployed", 10, Parse_Bool, NULL, (void **)&value->IsDeployed },
		{ "GraphDescriptor", 15, Parse_String, NULL, (void **)&value->GraphDescriptor },
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
		{ "RedirectUri", 11, Parse_String, NULL, (void **)&value->RedirectUri },
		{ "image", 5, Parse_String, NULL, (void **)&value->image },
		{ "description", 11, Parse_String, NULL, (void **)&value->description },
		{ "pathcss", 7, Parse_String, NULL, (void **)&value->pathcss },
		{ "PairingCompletionInstructions", 29, Parse_String, NULL, (void **)&value->PairingCompletionInstructions },
		{ "PublicKey", 9, NULL, (ParseFuncSubStruct *)Array_byte_FromJson, (void **)&value->PublicKey },
		{ "NoUUIDAuthentication", 20, Parse_Bool, NULL, (void **)&value->NoUUIDAuthentication },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t));
	ParseTable table[] = {
		{ "token1", 6, Parse_String, NULL, (void **)&value->token1 },
		{ "token2", 6, Parse_String, NULL, (void **)&value->token2 },
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
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateSemantics_Decimal_Range_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateSemantics_Decimal_Range_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortStateSemantics_Decimal_Range_t));
	ParseTable table[] = {
		{ "Min", 3, Parse_Double, NULL, (void **)&value->Min },
		{ "Max", 3, Parse_Double, NULL, (void **)&value->Max },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateSemantics_Integer_Range_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateSemantics_Integer_Range_t *value)
{
	memset(value, 0, sizeof(Yodiwo_Plegma_PortStateSemantics_Integer_Range_t));
	ParseTable table[] = {
		{ "Min", 3, Parse_Int, NULL, (void **)&value->Min },
		{ "Max", 3, Parse_Int, NULL, (void **)&value->Max },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}

// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e APIGenerator_AdditionalTypes_CNodeConfig_FromJson(char* json, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeConfig_t *value)
{
	memset(value, 0, sizeof(APIGenerator_AdditionalTypes_CNodeConfig_t));
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
Yodiwo_Plegma_Json_e APIGenerator_AdditionalTypes_CNodeCConfig_FromJson(char* json, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeCConfig_t *value)
{
	memset(value, 0, sizeof(APIGenerator_AdditionalTypes_CNodeCConfig_t));
	ParseTable table[] = {
		{ "ActiveID", 8, Parse_Int, NULL, (void **)&value->ActiveID },
		{ "Configs", 7, NULL, (ParseFuncSubStruct *)Array_APIGenerator_AdditionalTypes_CNodeConfig_FromJson, (void **)&value->Configs },
	};
	return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
}


