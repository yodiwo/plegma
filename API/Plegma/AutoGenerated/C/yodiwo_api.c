/**
* Created by ApiGenerator Tool (C) on 11/08/2015 18:56:39.
*/

// This is only for windows testing
#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif


#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include "jsmn.h"
#include "yodiwo_api.h"
#include "yodiwo_helpers.h"

/* ======================================================================================================================= */
/* ToJson Functions                                                                                                        */
/* ======================================================================================================================= */


// Helper functions to print arrays
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_ConfigParameter_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_ConfigParameter_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_ConfigParameter_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_Port_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_Port_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_Port_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_ConfigDescription_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_ConfigDescription_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_ConfigDescription_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_PortDescription_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_PortDescription_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_PortDescription_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_NodeModelType_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_NodeModelType_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_NodeModelType_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_NodeThingType_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_NodeThingType_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_NodeThingType_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_Thing_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_Thing_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_Thing_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_PortEvent_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_PortEvent_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_PortEvent_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_char*_ToJson( char* jsonStart, size_t jsonSize,Array_char* *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = char*_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Array_Yodiwo_Plegma_PortState_ToJson( char* jsonStart, size_t jsonSize,Array_Yodiwo_Plegma_PortState_t *array)
    {
      int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
      *json = '['; json++;
      if(array!=NULL) {
          for (i = 0; i < array->num; i++) {
              if((len = Yodiwo_Plegma_PortState_ToJson(json, jsonEnd - json - 2, &array->elems[i]) - 1)< 0) return -1; else json += len;
              *json = ','; json++;
          }
          if (i > 0) json--; // remove last ,
      }
      *json = ']'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    



    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PairingNodeGetTokensRequest_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PairingNodeGetTokensRequest_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ uuid : \"%s\"", value->uuid); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", name : \"%s\"", value->name); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PairingNodeGetKeysRequest_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PairingNodeGetKeysRequest_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ uuid : \"%s\"", value->uuid); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", token1 : \"%s\"", value->token1); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PairingServerResponseTokens_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PairingServerResponseTokens_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ token1 : \"%s\"", value->token1); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", token2 : \"%s\"", value->token2); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PairingServerResponseKeys_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PairingServerResponseKeys_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ nodeKey : \"%s\"", value->nodeKey); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", secretKey : \"%s\"", value->secretKey); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PairingNodeResponsePhase1_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PairingNodeResponsePhase1_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ userNodeRegistrationUrl : \"%s\"", value->userNodeRegistrationUrl); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", token2 : \"%s\"", value->token2); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_UserKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_UserKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ UserID : \"%s\"", value->UserID); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_NodeKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_NodeKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ UserKey : ");
            if(( len = Yodiwo_Plegma_UserKey_ToJson( json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", NodeID : %d", value->NodeID); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_ThingKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_ThingKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ NodeKey : ");
            if(( len = Yodiwo_Plegma_NodeKey_ToJson( json, jsonEnd - json, &value->NodeKey) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", ThingUID : \"%s\"", value->ThingUID); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PortKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PortKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ ThingKey : ");
            if(( len = Yodiwo_Plegma_ThingKey_ToJson( json, jsonEnd - json, &value->ThingKey) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", PortUID : \"%s\"", value->PortUID); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_GraphDescriptorBaseKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_GraphDescriptorBaseKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ UserKey : ");
            if(( len = Yodiwo_Plegma_UserKey_ToJson( json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : \"%s\"", value->Id); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_GraphDescriptorKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_GraphDescriptorKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ UserKey : ");
            if(( len = Yodiwo_Plegma_UserKey_ToJson( json, jsonEnd - json, &value->UserKey) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : \"%s\"", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Revision : %d", value->Revision); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_GraphKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_GraphKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ GraphDescriptorKey : ");
            if(( len = Yodiwo_Plegma_GraphDescriptorKey_ToJson( json, jsonEnd - json, &value->GraphDescriptorKey) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", GraphId : %d", value->GraphId); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_BlockKey_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_BlockKey_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ GraphKey : ");
            if(( len = Yodiwo_Plegma_GraphKey_ToJson( json, jsonEnd - json, &value->GraphKey) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", BlockId : %d", value->BlockId); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_Port_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_Port_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ PortKey : \"%s\"", value->PortKey); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Name : \"%s\"", value->Name); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ioDirection : %d", value->ioDirection); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Type : %d", value->Type); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", NumOfActiveGraphs : %d", value->NumOfActiveGraphs); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_ConfigParameter_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_ConfigParameter_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Name : \"%s\"", value->Name); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Value : \"%s\"", value->Value); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_ThingUIHints_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_ThingUIHints_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ IconURI : \"%s\"", value->IconURI); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_Thing_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_Thing_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ ThingKey : \"%s\"", value->ThingKey); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Name : \"%s\"", value->Name); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", Config : ");
            if(( len = Array_Yodiwo_Plegma_ConfigParameter_ToJson( json, jsonEnd - json, &value->Config) - 1) < 0) return -1; else json+=len;
            json += snprintf(json, jsonEnd - json, "%s", ", Ports : ");
            if(( len = Array_Yodiwo_Plegma_Port_ToJson( json, jsonEnd - json, &value->Ports) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Type : \"%s\"", value->Type); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", BlockType : \"%s\"", value->BlockType); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", UIHints : ");
            if(( len = Yodiwo_Plegma_ThingUIHints_ToJson( json, jsonEnd - json, &value->UIHints) - 1) < 0) return -1; else json+=len;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_LoginReq_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_LoginReq_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_LoginRsp_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_LoginRsp_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ NodeKey : \"%s\"", value->NodeKey); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SecretKey : \"%s\"", value->SecretKey); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_StateDescription_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_StateDescription_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Minimum : %lf", value->Minimum); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Maximum : %lf", value->Maximum); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Step : %lf", value->Step); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Pattern : \"%s\"", value->Pattern); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ReadOnly : %s", (value->ReadOnly)?"true":"false"); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_ConfigDescription_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_ConfigDescription_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ DefaultValue : \"%s\"", value->DefaultValue); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Description : \"%s\"", value->Description); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Label : \"%s\"", value->Label); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Name : \"%s\"", value->Name); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Required : %s", (value->Required)?"true":"false"); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Type : \"%s\"", value->Type); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Minimum : %lf", value->Minimum); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Maximum : %lf", value->Maximum); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Stepsize : %lf", value->Stepsize); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ReadOnly : %s", (value->ReadOnly)?"true":"false"); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PortDescription_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PortDescription_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Description : \"%s\"", value->Description); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Id : \"%s\"", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Label : \"%s\"", value->Label); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Category : \"%s\"", value->Category); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", State : ");
            if(( len = Yodiwo_Plegma_StateDescription_ToJson( json, jsonEnd - json, &value->State) - 1) < 0) return -1; else json+=len;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_NodeModelType_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_NodeModelType_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Id : \"%s\"", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Name : \"%s\"", value->Name); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Description : \"%s\"", value->Description); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", Config : ");
            if(( len = Array_Yodiwo_Plegma_ConfigDescription_ToJson( json, jsonEnd - json, &value->Config) - 1) < 0) return -1; else json+=len;
            json += snprintf(json, jsonEnd - json, "%s", ", Port : ");
            if(( len = Array_Yodiwo_Plegma_PortDescription_ToJson( json, jsonEnd - json, &value->Port) - 1) < 0) return -1; else json+=len;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_NodeThingType_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_NodeThingType_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Type : \"%s\"", value->Type); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Searchable : %s", (value->Searchable)?"true":"false"); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Description : \"%s\"", value->Description); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", Model : ");
            if(( len = Array_Yodiwo_Plegma_NodeModelType_ToJson( json, jsonEnd - json, &value->Model) - 1) < 0) return -1; else json+=len;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_NodeInfoReq_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_NodeInfoReq_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ RequestedThingType : ");
            if(( len = Yodiwo_Plegma_NodeThingType_ToJson( json, jsonEnd - json, &value->RequestedThingType) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_NodeInfoRsp_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_NodeInfoRsp_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Name : \"%s\"", value->Name); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Type : %d", value->Type); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Capabilities : %d", value->Capabilities); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", ThingTypes : ");
            if(( len = Array_Yodiwo_Plegma_NodeThingType_ToJson( json, jsonEnd - json, &value->ThingTypes) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_ThingsReq_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_ThingsReq_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Operation : %d", value->Operation); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ThingKey : \"%s\"", value->ThingKey); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", Data : ");
            if(( len = Array_Yodiwo_Plegma_Thing_ToJson( json, jsonEnd - json, &value->Data) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_ThingsRsp_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_ThingsRsp_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Operation : %d", value->Operation); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Status : %s", (value->Status)?"true":"false"); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", Data : ");
            if(( len = Array_Yodiwo_Plegma_Thing_ToJson( json, jsonEnd - json, &value->Data) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PortEvent_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PortEvent_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ PortKey : \"%s\"", value->PortKey); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", State : \"%s\"", value->State); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", RevNum : %d", value->RevNum); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PortEventMsg_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PortEventMsg_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ PortEvents : ");
            if(( len = Array_Yodiwo_Plegma_PortEvent_ToJson( json, jsonEnd - json, &value->PortEvents) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PortStateReq_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PortStateReq_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Operation : %d", value->Operation); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", PortKeys : ");
            if(( len = Array_char*_ToJson( json, jsonEnd - json, &value->PortKeys) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PortState_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PortState_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ PortKey : \"%s\"", value->PortKey); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", State : \"%s\"", value->State); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", RevNum : %d", value->RevNum); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", IsDeployed : %s", (value->IsDeployed)?"true":"false"); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_PortStateRsp_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_PortStateRsp_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
                json += snprintf(json, jsonEnd - json, "{ Operation : %d", value->Operation); if (json >= jsonEnd) return -1;
            json += snprintf(json, jsonEnd - json, "%s", ", PortStates : ");
            if(( len = Array_Yodiwo_Plegma_PortState_ToJson( json, jsonEnd - json, &value->PortStates) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    
    // -----------------------------------------------------------------------------------------------------------------------
    int Yodiwo_Plegma_ActivePortKeysMsg_ToJson( char* jsonStart, size_t jsonSize,Yodiwo_Plegma_ActivePortKeysMsg_t *value)
    {
      char *json = jsonStart, *jsonEnd = json + jsonSize;
      int len;
            json += snprintf(json, jsonEnd - json, "%s", "{ ActivePortKeys : ");
            if(( len = Array_char*_ToJson( json, jsonEnd - json, &value->ActivePortKeys) - 1) < 0) return -1; else json+=len;
                json += snprintf(json, jsonEnd - json, ", Id : %d", value->Id); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", Version : %d", value->Version); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", SeqNo : %d", value->SeqNo); if (json >= jsonEnd) return -1;
                json += snprintf(json, jsonEnd - json, ", ResponseToSeqNo : %d", value->ResponseToSeqNo); if (json >= jsonEnd) return -1;
      *json = '}'; json++;
      *json = '\0'; json++;
      return json - jsonStart;
    }
    




/* ======================================================================================================================= */
/* FromJson Functions                                                                                                      */
/* ======================================================================================================================= */
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_ConfigParameter_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_ConfigParameter_t *array)
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
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_Port_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_Port_t *array)
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
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_ConfigDescription_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_ConfigDescription_t *array)
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
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_PortDescription_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_PortDescription_t *array)
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
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_NodeModelType_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_NodeModelType_t *array)
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
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_NodeThingType_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_NodeThingType_t *array)
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
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_Thing_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_Thing_t *array)
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
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_PortEvent_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_PortEvent_t *array)
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
        Yodiwo_Plegma_Json_e Array_char*_FromJson( char* json, size_t jsonSize,Array_char* *array)
        {
          int i = 0, r;
          Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
          jsmntok_t t[64]; /* We expect no more than 128 tokens */
          jsmn_parser p;
          jsmn_init(&p);
          if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
          if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;
        
          array->num = Helper_Json_ParseArray(t, r);
          array->elems = (char* *)malloc(array->num*sizeof(char*));
          for (i = 0; i < array->num; i++) {
              if ((res = char*_FromJson(&json[t[i].start], t[i].end - t[i].start, &array->elems[i])) != Yodiwo_JsonSuccessParse) break;
          }
          return res;
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Array_Yodiwo_Plegma_PortState_FromJson( char* json, size_t jsonSize,Array_Yodiwo_Plegma_PortState_t *array)
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
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PairingNodeGetTokensRequest_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PairingNodeGetTokensRequest_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PairingNodeGetTokensRequest_t));
        ParseTable table[] = {
            { "uuid", 4, Parse_String, NULL, &value->uuid },
            { "name", 4, Parse_String, NULL, &value->name },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PairingNodeGetKeysRequest_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PairingNodeGetKeysRequest_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PairingNodeGetKeysRequest_t));
        ParseTable table[] = {
            { "uuid", 4, Parse_String, NULL, &value->uuid },
            { "token1", 6, Parse_String, NULL, &value->token1 },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PairingServerResponseTokens_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PairingServerResponseTokens_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PairingServerResponseTokens_t));
        ParseTable table[] = {
            { "token1", 6, Parse_String, NULL, &value->token1 },
            { "token2", 6, Parse_String, NULL, &value->token2 },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PairingServerResponseKeys_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PairingServerResponseKeys_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PairingServerResponseKeys_t));
        ParseTable table[] = {
            { "nodeKey", 7, Parse_String, NULL, &value->nodeKey },
            { "secretKey", 9, Parse_String, NULL, &value->secretKey },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PairingNodeResponsePhase1_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PairingNodeResponsePhase1_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PairingNodeResponsePhase1_t));
        ParseTable table[] = {
            { "userNodeRegistrationUrl", 23, Parse_String, NULL, &value->userNodeRegistrationUrl },
            { "token2", 6, Parse_String, NULL, &value->token2 },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_UserKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_UserKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_UserKey_t));
        ParseTable table[] = {
            { "UserID", 6, Parse_String, NULL, &value->UserID },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_NodeKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_NodeKey_t));
        ParseTable table[] = {
            { "UserKey", 7, NULL, Yodiwo_Plegma_UserKey_FromJson, &value->UserKey },
            { "NodeID", 6, Parse_Int, NULL, &value->NodeID },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_ThingKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_ThingKey_t));
        ParseTable table[] = {
            { "NodeKey", 7, NULL, Yodiwo_Plegma_NodeKey_FromJson, &value->NodeKey },
            { "ThingUID", 8, Parse_String, NULL, &value->ThingUID },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PortKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PortKey_t));
        ParseTable table[] = {
            { "ThingKey", 8, NULL, Yodiwo_Plegma_ThingKey_FromJson, &value->ThingKey },
            { "PortUID", 7, Parse_String, NULL, &value->PortUID },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDescriptorBaseKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_GraphDescriptorBaseKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_GraphDescriptorBaseKey_t));
        ParseTable table[] = {
            { "UserKey", 7, NULL, Yodiwo_Plegma_UserKey_FromJson, &value->UserKey },
            { "Id", 2, Parse_String, NULL, &value->Id },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDescriptorKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_GraphDescriptorKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_GraphDescriptorKey_t));
        ParseTable table[] = {
            { "UserKey", 7, NULL, Yodiwo_Plegma_UserKey_FromJson, &value->UserKey },
            { "Id", 2, Parse_String, NULL, &value->Id },
            { "Revision", 8, Parse_Int, NULL, &value->Revision },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_GraphKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_GraphKey_t));
        ParseTable table[] = {
            { "GraphDescriptorKey", 18, NULL, Yodiwo_Plegma_GraphDescriptorKey_FromJson, &value->GraphDescriptorKey },
            { "GraphId", 7, Parse_Int, NULL, &value->GraphId },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_BlockKey_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_BlockKey_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_BlockKey_t));
        ParseTable table[] = {
            { "GraphKey", 8, NULL, Yodiwo_Plegma_GraphKey_FromJson, &value->GraphKey },
            { "BlockId", 7, Parse_Int, NULL, &value->BlockId },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_Port_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_Port_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_Port_t));
        ParseTable table[] = {
            { "PortKey", 7, Parse_String, NULL, &value->PortKey },
            { "Name", 4, Parse_String, NULL, &value->Name },
            { "ioDirection", 11, Parse_Int, NULL, &value->ioDirection },
            { "Type", 4, Parse_Int, NULL, &value->Type },
            { "NumOfActiveGraphs", 17, Parse_Int, NULL, &value->NumOfActiveGraphs },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigParameter_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_ConfigParameter_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_ConfigParameter_t));
        ParseTable table[] = {
            { "Name", 4, Parse_String, NULL, &value->Name },
            { "Value", 5, Parse_String, NULL, &value->Value },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingUIHints_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_ThingUIHints_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_ThingUIHints_t));
        ParseTable table[] = {
            { "IconURI", 7, Parse_String, NULL, &value->IconURI },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_Thing_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_Thing_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_Thing_t));
        ParseTable table[] = {
            { "ThingKey", 8, Parse_String, NULL, &value->ThingKey },
            { "Name", 4, Parse_String, NULL, &value->Name },
            { "Config", 6, NULL, Array_Yodiwo_Plegma_ConfigParameter_FromJson, &value->Config },
            { "Ports", 5, NULL, Array_Yodiwo_Plegma_Port_FromJson, &value->Ports },
            { "Type", 4, Parse_String, NULL, &value->Type },
            { "BlockType", 9, Parse_String, NULL, &value->BlockType },
            { "UIHints", 7, NULL, Yodiwo_Plegma_ThingUIHints_FromJson, &value->UIHints },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginReq_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_LoginReq_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_LoginReq_t));
        ParseTable table[] = {
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginRsp_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_LoginRsp_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_LoginRsp_t));
        ParseTable table[] = {
            { "NodeKey", 7, Parse_String, NULL, &value->NodeKey },
            { "SecretKey", 9, Parse_String, NULL, &value->SecretKey },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_StateDescription_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_StateDescription_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_StateDescription_t));
        ParseTable table[] = {
            { "Minimum", 7, Parse_Double, NULL, &value->Minimum },
            { "Maximum", 7, Parse_Double, NULL, &value->Maximum },
            { "Step", 4, Parse_Double, NULL, &value->Step },
            { "Pattern", 7, Parse_String, NULL, &value->Pattern },
            { "ReadOnly", 8, Parse_Bool, NULL, &value->ReadOnly },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigDescription_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_ConfigDescription_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_ConfigDescription_t));
        ParseTable table[] = {
            { "DefaultValue", 12, Parse_String, NULL, &value->DefaultValue },
            { "Description", 11, Parse_String, NULL, &value->Description },
            { "Label", 5, Parse_String, NULL, &value->Label },
            { "Name", 4, Parse_String, NULL, &value->Name },
            { "Required", 8, Parse_Bool, NULL, &value->Required },
            { "Type", 4, Parse_String, NULL, &value->Type },
            { "Minimum", 7, Parse_Double, NULL, &value->Minimum },
            { "Maximum", 7, Parse_Double, NULL, &value->Maximum },
            { "Stepsize", 8, Parse_Double, NULL, &value->Stepsize },
            { "ReadOnly", 8, Parse_Bool, NULL, &value->ReadOnly },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortDescription_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PortDescription_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PortDescription_t));
        ParseTable table[] = {
            { "Description", 11, Parse_String, NULL, &value->Description },
            { "Id", 2, Parse_String, NULL, &value->Id },
            { "Label", 5, Parse_String, NULL, &value->Label },
            { "Category", 8, Parse_String, NULL, &value->Category },
            { "State", 5, NULL, Yodiwo_Plegma_StateDescription_FromJson, &value->State },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeModelType_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_NodeModelType_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_NodeModelType_t));
        ParseTable table[] = {
            { "Id", 2, Parse_String, NULL, &value->Id },
            { "Name", 4, Parse_String, NULL, &value->Name },
            { "Description", 11, Parse_String, NULL, &value->Description },
            { "Config", 6, NULL, Array_Yodiwo_Plegma_ConfigDescription_FromJson, &value->Config },
            { "Port", 4, NULL, Array_Yodiwo_Plegma_PortDescription_FromJson, &value->Port },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeThingType_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_NodeThingType_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_NodeThingType_t));
        ParseTable table[] = {
            { "Type", 4, Parse_String, NULL, &value->Type },
            { "Searchable", 10, Parse_Bool, NULL, &value->Searchable },
            { "Description", 11, Parse_String, NULL, &value->Description },
            { "Model", 5, NULL, Array_Yodiwo_Plegma_NodeModelType_FromJson, &value->Model },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoReq_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_NodeInfoReq_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_NodeInfoReq_t));
        ParseTable table[] = {
            { "RequestedThingType", 18, NULL, Yodiwo_Plegma_NodeThingType_FromJson, &value->RequestedThingType },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoRsp_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_NodeInfoRsp_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_NodeInfoRsp_t));
        ParseTable table[] = {
            { "Name", 4, Parse_String, NULL, &value->Name },
            { "Type", 4, Parse_Int, NULL, &value->Type },
            { "Capabilities", 12, Parse_Int, NULL, &value->Capabilities },
            { "ThingTypes", 10, NULL, Array_Yodiwo_Plegma_NodeThingType_FromJson, &value->ThingTypes },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsReq_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_ThingsReq_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_ThingsReq_t));
        ParseTable table[] = {
            { "Operation", 9, Parse_Int, NULL, &value->Operation },
            { "ThingKey", 8, Parse_String, NULL, &value->ThingKey },
            { "Data", 4, NULL, Array_Yodiwo_Plegma_Thing_FromJson, &value->Data },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsRsp_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_ThingsRsp_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_ThingsRsp_t));
        ParseTable table[] = {
            { "Operation", 9, Parse_Int, NULL, &value->Operation },
            { "Status", 6, Parse_Bool, NULL, &value->Status },
            { "Data", 4, NULL, Array_Yodiwo_Plegma_Thing_FromJson, &value->Data },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEvent_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PortEvent_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PortEvent_t));
        ParseTable table[] = {
            { "PortKey", 7, Parse_String, NULL, &value->PortKey },
            { "State", 5, Parse_String, NULL, &value->State },
            { "RevNum", 6, Parse_Int, NULL, &value->RevNum },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEventMsg_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PortEventMsg_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PortEventMsg_t));
        ParseTable table[] = {
            { "PortEvents", 10, NULL, Array_Yodiwo_Plegma_PortEvent_FromJson, &value->PortEvents },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateReq_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PortStateReq_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PortStateReq_t));
        ParseTable table[] = {
            { "Operation", 9, Parse_Int, NULL, &value->Operation },
            { "PortKeys", 8, NULL, Array_char*_FromJson, &value->PortKeys },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortState_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PortState_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PortState_t));
        ParseTable table[] = {
            { "PortKey", 7, Parse_String, NULL, &value->PortKey },
            { "State", 5, Parse_String, NULL, &value->State },
            { "RevNum", 6, Parse_Int, NULL, &value->RevNum },
            { "IsDeployed", 10, Parse_Bool, NULL, &value->IsDeployed },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateRsp_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_PortStateRsp_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_PortStateRsp_t));
        ParseTable table[] = {
            { "Operation", 9, Parse_Int, NULL, &value->Operation },
            { "PortStates", 10, NULL, Array_Yodiwo_Plegma_PortState_FromJson, &value->PortStates },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        
        // -----------------------------------------------------------------------------------------------------------------------
        Yodiwo_Plegma_Json_e Yodiwo_Plegma_ActivePortKeysMsg_FromJson( char* json, size_t jsonSize,Yodiwo_Plegma_ActivePortKeysMsg_t *value)
        {
        memset(value, 0, sizeof(Yodiwo_Plegma_ActivePortKeysMsg_t));
        ParseTable table[] = {
            { "ActivePortKeys", 14, NULL, Array_char*_FromJson, &value->ActivePortKeys },
            { "Id", 2, Parse_Int, NULL, &value->Id },
            { "Version", 7, Parse_Int, NULL, &value->Version },
            { "SeqNo", 5, Parse_Int, NULL, &value->SeqNo },
            { "ResponseToSeqNo", 15, Parse_Int, NULL, &value->ResponseToSeqNo },
        };
        return HelperJsonParseExec(json, jsonSize, table, sizeof(table) / sizeof(table[0]));
        }
        





