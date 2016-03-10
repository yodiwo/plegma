/**
 * Created by ApiGenerator Tool (C) on 07-Mar-16 3:40:54 PM.
 */

#ifndef _Yodiwo_Plegma_H_
#define _Yodiwo_Plegma_H_

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>
#include <stdint.h>

	/* ========================================================================*/
	/* Enum                                                                    */
	/* ========================================================================*/
	typedef enum
	{
		Yodiwo_eDriverType_Unknown = 0,
		Yodiwo_eDriverType_LCD = 1,
	} Yodiwo_Plegma_eDriverType;

	typedef enum
	{
		Yodiwo_ePortType_Undefined = 0,
		Yodiwo_ePortType_Integer = 1,
		Yodiwo_ePortType_Decimal = 2,
		Yodiwo_ePortType_DecimalHigh = 3,
		Yodiwo_ePortType_Boolean = 4,
		Yodiwo_ePortType_Color = 5,
		Yodiwo_ePortType_String = 6,
	} Yodiwo_Plegma_ePortType;

	typedef enum
	{
		Yodiwo_ioPortDirection_Undefined = 0,
		Yodiwo_ioPortDirection_InputOutput = 1,
		Yodiwo_ioPortDirection_Output = 2,
		Yodiwo_ioPortDirection_Input = 3,
	} Yodiwo_Plegma_ioPortDirection;

	typedef enum
	{
		Yodiwo_ePortConf_None = 0,
		Yodiwo_ePortConf_ReceiveAllEvents = 1,
		Yodiwo_ePortConf_IsTrigger = 2,
	} Yodiwo_Plegma_ePortConf;

	typedef enum
	{
		Yodiwo_BinaryResourceContentType_Undefined = 0,
		Yodiwo_BinaryResourceContentType_Data = 1,
		Yodiwo_BinaryResourceContentType_Text = 2,
		Yodiwo_BinaryResourceContentType_Image = 3,
		Yodiwo_BinaryResourceContentType_Audio = 4,
		Yodiwo_BinaryResourceContentType_Video = 5,
	} Yodiwo_Plegma_BinaryResourceContentType;

	typedef enum
	{
		Yodiwo_BinaryResourceLocationType_Undefined = 0,
		Yodiwo_BinaryResourceLocationType_Http = 1,
		Yodiwo_BinaryResourceLocationType_RedisDB = 2,
	} Yodiwo_Plegma_BinaryResourceLocationType;

	typedef enum
	{
		Yodiwo_RestServiceType_Undefined = 0,
		Yodiwo_RestServiceType_Dropbox = 1,
		Yodiwo_RestServiceType_Pastebin = 2,
		Yodiwo_RestServiceType_GoogleDrive = 3,
	} Yodiwo_Plegma_RestServiceType;

	typedef enum
	{
		Yodiwo_ImageFileFormat_Undefined = 0,
		Yodiwo_ImageFileFormat_PNG = 1,
		Yodiwo_ImageFileFormat_TIFF = 2,
		Yodiwo_ImageFileFormat_GIF = 3,
		Yodiwo_ImageFileFormat_BMP = 4,
		Yodiwo_ImageFileFormat_SVG = 5,
	} Yodiwo_Plegma_ImageFileFormat;

	typedef enum
	{
		Yodiwo_ImageType_Raster = 0,
		Yodiwo_ImageType_Vector = 1,
	} Yodiwo_Plegma_ImageType;

	typedef enum
	{
		Yodiwo_eConnectionFlags_CreateNewEndpoint = 1,
		Yodiwo_eConnectionFlags_IsMasterEndpoint = 2,
	} Yodiwo_Plegma_eConnectionFlags;

	typedef enum
	{
		Yodiwo_eNodeType_Unknown = 0,
		Yodiwo_eNodeType_Gateway = 1,
		Yodiwo_eNodeType_EndpointSingle = 2,
		Yodiwo_eNodeType_TestGateway = 3,
		Yodiwo_eNodeType_TestEndpoint = 4,
		Yodiwo_eNodeType_WSEndpoint = 5,
		Yodiwo_eNodeType_Android = 6,
		Yodiwo_eNodeType_WSSample = 200,
	} Yodiwo_Plegma_eNodeType;

	typedef enum
	{
		Yodiwo_eNodeCapa_None = 0,
		Yodiwo_eNodeCapa_SupportsGraphSolving = 1,
	} Yodiwo_Plegma_eNodeCapa;

	typedef enum
	{
		Yodiwo_eUnpairReason_Unknown = 0,
		Yodiwo_eUnpairReason_InvalidOperation = 1,
		Yodiwo_eUnpairReason_UserRequested = 2,
		Yodiwo_eUnpairReason_TooManyAuthFailures = 3,
	} Yodiwo_Plegma_eUnpairReason;

	typedef enum
	{
		Yodiwo_eNodeSyncOperation_GetEndpoints = 1,
		Yodiwo_eNodeSyncOperation_SetEndpoint = 2,
	} Yodiwo_Plegma_eNodeSyncOperation;

	typedef enum
	{
		Yodiwo_eThingsOperation_Invalid = 0,
		Yodiwo_eThingsOperation_Update = 1,
		Yodiwo_eThingsOperation_Overwrite = 2,
		Yodiwo_eThingsOperation_Delete = 3,
		Yodiwo_eThingsOperation_Get = 4,
		Yodiwo_eThingsOperation_Scan = 5,
		Yodiwo_eThingsOperation_Sync = 6,
	} Yodiwo_Plegma_eThingsOperation;

	typedef enum
	{
		Yodiwo_ePortStateOperation_Invalid = 0,
		Yodiwo_ePortStateOperation_SpecificKeys = 1,
		Yodiwo_ePortStateOperation_ActivePortStates = 2,
		Yodiwo_ePortStateOperation_AllPortStates = 3,
	} Yodiwo_Plegma_ePortStateOperation;

	typedef enum
	{
		Yodiwo_eA2mcuCtrlType_Reset = 0,
		Yodiwo_eA2mcuCtrlType_SetValue = 1,
		Yodiwo_eA2mcuCtrlType_WriteDriconf = 2,
	} Yodiwo_Plegma_eA2mcuCtrlType;

	typedef enum
	{
		Yodiwo_PairingStates_Initial = 0,
		Yodiwo_PairingStates_StartRequested = 1,
		Yodiwo_PairingStates_TokensRequested = 2,
		Yodiwo_PairingStates_TokensSentToNode = 3,
		Yodiwo_PairingStates_Token2SentToUser = 4,
		Yodiwo_PairingStates_Token2PostedToServer = 5,
		Yodiwo_PairingStates_UUIDEntryRedirect = 6,
		Yodiwo_PairingStates_Phase1Complete = 7,
		Yodiwo_PairingStates_NextRequested = 8,
		Yodiwo_PairingStates_Token1PostedToServer = 9,
		Yodiwo_PairingStates_KeysSentToNode = 10,
		Yodiwo_PairingStates_Paired = 11,
		Yodiwo_PairingStates_Failed = 12,
	} Yodiwo_Plegma_NodePairing_PairingStates;

	typedef enum
	{
		Yodiwo_eMsgFlags_None = 0,
		Yodiwo_eMsgFlags_Request = 1,
		Yodiwo_eMsgFlags_Response = 2,
	} Yodiwo_Plegma_WrapperMsg_eMsgFlags;

	typedef enum
	{
		Yodiwo_eWSMsgId_None = 0,
		Yodiwo_eWSMsgId_Pairing = 1,
		Yodiwo_eWSMsgId_Api = 2,
	} Yodiwo_Plegma_WebSocketMsg_eWSMsgId;



	/* ========================================================================*/
	/* Struct Prototypes                                                       */
	/* ========================================================================*/
	struct Yodiwo_API_MediaStreaming_AudioMediaDescriptor;
	struct Yodiwo_API_MediaStreaming_AudioDataReq;
	struct Yodiwo_API_MediaStreaming_AudioDataResp;
	struct Yodiwo_API_MediaStreaming_AudioAuthenticationRequest;
	struct Yodiwo_API_MediaStreaming_AudioAuthenticationResponse;
	struct Yodiwo_API_MediaStreaming_AudioData;
	struct Yodiwo_API_MediaStreaming_AudioServerFeedRequest;
	struct Yodiwo_API_MediaStreaming_AudioServerConnectRequest;
	struct Yodiwo_API_MediaStreaming_AudioServerDisconnectRequest;
	struct Yodiwo_API_MediaStreaming_AudioServerDisconnectResponse;
	struct Yodiwo_API_MediaStreaming_AudioServerConnectResponse;
	struct Yodiwo_API_MediaStreaming_AudioServerFeedResponse;
	struct Yodiwo_API_MediaStreaming_InfoMessage;
	struct Yodiwo_API_MediaStreaming_ErrorMessage;
	struct Yodiwo_API_MediaStreaming_OngoingMediaStreamDescriptor;
	struct Yodiwo_API_MediaStreaming_VideoDescriptor;
	struct Yodiwo_API_MediaStreaming_VideoMediaDescriptor;
	struct Yodiwo_API_MediaStreaming_VideoDataReq;
	struct Yodiwo_API_MediaStreaming_VideoDataResp;
	struct Yodiwo_API_MediaStreaming_VideoAuthenticationRequest;
	struct Yodiwo_API_MediaStreaming_VideoAuthenticationResponse;
	struct Yodiwo_API_MediaStreaming_VideoData;
	struct Yodiwo_API_MediaStreaming_VideoServerFeedResponse;
	struct Yodiwo_API_MediaStreaming_VideoServerFeedRequest;
	struct Yodiwo_API_MediaStreaming_VideoServerConnectRequest;
	struct Yodiwo_API_MediaStreaming_VideoServerDisconnectRequest;
	struct Yodiwo_API_MediaStreaming_VideoServerDisconnectResponse;
	struct Yodiwo_API_MediaStreaming_VideoServerConnectResponse;
	struct Yodiwo_Plegma_UserKey;
	struct Yodiwo_Plegma_NodeKey;
	struct Yodiwo_Plegma_ThingKey;
	struct Yodiwo_Plegma_PortKey;
	struct Yodiwo_Plegma_GraphDescriptorBaseKey;
	struct Yodiwo_Plegma_GraphDescriptorKey;
	struct Yodiwo_Plegma_GraphKey;
	struct Yodiwo_Plegma_BlockKey;
	struct Yodiwo_Plegma_GroupKey;
	struct Yodiwo_Plegma_DriverKey;
	struct Yodiwo_Plegma_WrapperMsg;
	struct Yodiwo_Plegma_WebSocketMsg;
	struct Yodiwo_Plegma_MqttMsg;
	struct Yodiwo_Plegma_YColor;
	struct Yodiwo_Plegma_Port;
	struct Yodiwo_Plegma_BinaryResourceDescriptor;
	struct Yodiwo_Plegma_HttpLocationDescriptor;
	struct Yodiwo_Plegma_RedisDBLocationDescriptor;
	struct Yodiwo_Plegma_DataContentDescriptor;
	struct Yodiwo_Plegma_TextContentDescriptor;
	struct Yodiwo_Plegma_ImageContentDescriptor;
	struct Yodiwo_Plegma_AudioContentDescriptor;
	struct Yodiwo_Plegma_VideoContentDescriptor;
	struct Yodiwo_Plegma_ConfigParameter;
	struct Yodiwo_Plegma_ThingUIHints;
	struct Yodiwo_Plegma_Thing;
	struct Yodiwo_Plegma_GenericRsp;
	struct Yodiwo_Plegma_LoginReq;
	struct Yodiwo_Plegma_LoginRsp;
	struct Yodiwo_Plegma_StateDescription;
	struct Yodiwo_Plegma_ConfigDescription;
	struct Yodiwo_Plegma_PortDescription;
	struct Yodiwo_Plegma_ThingModelType;
	struct Yodiwo_Plegma_ThingType;
	struct Yodiwo_Plegma_NodeInfoReq;
	struct Yodiwo_Plegma_NodeInfoRsp;
	struct Yodiwo_Plegma_NodeUnpairedReq;
	struct Yodiwo_Plegma_NodeUnpairedRsp;
	struct Yodiwo_Plegma_EndpointSyncReq;
	struct Yodiwo_Plegma_EndpointSyncRsp;
	struct Yodiwo_Plegma_ThingsGet;
	struct Yodiwo_Plegma_ThingsSet;
	struct Yodiwo_Plegma_PingReq;
	struct Yodiwo_Plegma_PingRsp;
	struct Yodiwo_Plegma_PortEvent;
	struct Yodiwo_Plegma_PortEventMsg;
	struct Yodiwo_Plegma_VirtualBlockEvent;
	struct Yodiwo_Plegma_VirtualBlockEventMsg;
	struct Yodiwo_Plegma_PortStateReq;
	struct Yodiwo_Plegma_PortState;
	struct Yodiwo_Plegma_PortStateRsp;
	struct Yodiwo_Plegma_ActivePortKeysMsg;
	struct Yodiwo_Plegma_LocallyDeployedGraphsMsg;
	struct Yodiwo_Plegma_GraphDeploymentReq;
	struct Yodiwo_Plegma_A2mcuActiveDriver;
	struct Yodiwo_Plegma_A2mcuActiveDriversReq;
	struct Yodiwo_Plegma_A2mcuConcurrentCommands;
	struct Yodiwo_Plegma_A2mcuSequencedCommands;
	struct Yodiwo_Plegma_A2mcuCtrl;
	struct Yodiwo_Plegma_A2mcuCtrlReq;
	struct Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest;
	struct Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest;
	struct Yodiwo_Plegma_NodePairing_PairingServerTokensResponse;
	struct Yodiwo_Plegma_NodePairing_PairingServerKeysResponse;
	struct Yodiwo_Plegma_NodePairing_PairingNodePhase1Response;
	struct APIGenerator_AdditionalTypes_CNodeConfig;
	struct APIGenerator_AdditionalTypes_CNodeCConfig;

	/* Array helper structs */
	struct Array_byte;
	struct Array_Yodiwo_Plegma_ConfigParameter;
	struct Array_Yodiwo_Plegma_Port;
	struct Array_Yodiwo_Plegma_ConfigDescription;
	struct Array_Yodiwo_Plegma_PortDescription;
	struct Array_Yodiwo_Plegma_ThingModelType;
	struct Array_Yodiwo_Plegma_ThingType;
	struct Array_string;
	struct Array_Yodiwo_Plegma_Thing;
	struct Array_Yodiwo_Plegma_PortEvent;
	struct Array_Yodiwo_Plegma_VirtualBlockEvent;
	struct Array_Yodiwo_Plegma_PortState;
	struct Array_Yodiwo_Plegma_A2mcuActiveDriver;
	struct Array_Yodiwo_Plegma_A2mcuCtrl;
	struct Array_APIGenerator_AdditionalTypes_CNodeConfig;

	/* ========================================================================*/
	/* Struct Helpers                                                          */
	/* ========================================================================*/

	typedef struct Array_byte
	{
		int num;
		struct byte* elems;
	} Array_byte_t;

	typedef struct Array_Yodiwo_Plegma_ConfigParameter
	{
		int num;
		struct Yodiwo_Plegma_ConfigParameter* elems;
	} Array_Yodiwo_Plegma_ConfigParameter_t;

	typedef struct Array_Yodiwo_Plegma_Port
	{
		int num;
		struct Yodiwo_Plegma_Port* elems;
	} Array_Yodiwo_Plegma_Port_t;

	typedef struct Array_Yodiwo_Plegma_ConfigDescription
	{
		int num;
		struct Yodiwo_Plegma_ConfigDescription* elems;
	} Array_Yodiwo_Plegma_ConfigDescription_t;

	typedef struct Array_Yodiwo_Plegma_PortDescription
	{
		int num;
		struct Yodiwo_Plegma_PortDescription* elems;
	} Array_Yodiwo_Plegma_PortDescription_t;

	typedef struct Array_Yodiwo_Plegma_ThingModelType
	{
		int num;
		struct Yodiwo_Plegma_ThingModelType* elems;
	} Array_Yodiwo_Plegma_ThingModelType_t;

	typedef struct Array_Yodiwo_Plegma_ThingType
	{
		int num;
		struct Yodiwo_Plegma_ThingType* elems;
	} Array_Yodiwo_Plegma_ThingType_t;

	typedef struct Array_string
	{
		int num;
		char** elems;
	} Array_string;

	typedef struct Array_Yodiwo_Plegma_Thing
	{
		int num;
		struct Yodiwo_Plegma_Thing* elems;
	} Array_Yodiwo_Plegma_Thing_t;

	typedef struct Array_Yodiwo_Plegma_PortEvent
	{
		int num;
		struct Yodiwo_Plegma_PortEvent* elems;
	} Array_Yodiwo_Plegma_PortEvent_t;

	typedef struct Array_Yodiwo_Plegma_VirtualBlockEvent
	{
		int num;
		struct Yodiwo_Plegma_VirtualBlockEvent* elems;
	} Array_Yodiwo_Plegma_VirtualBlockEvent_t;

	typedef struct Array_Yodiwo_Plegma_PortState
	{
		int num;
		struct Yodiwo_Plegma_PortState* elems;
	} Array_Yodiwo_Plegma_PortState_t;

	typedef struct Array_Yodiwo_Plegma_A2mcuActiveDriver
	{
		int num;
		struct Yodiwo_Plegma_A2mcuActiveDriver* elems;
	} Array_Yodiwo_Plegma_A2mcuActiveDriver_t;

	typedef struct Array_Yodiwo_Plegma_A2mcuCtrl
	{
		int num;
		struct Yodiwo_Plegma_A2mcuCtrl* elems;
	} Array_Yodiwo_Plegma_A2mcuCtrl_t;

	typedef struct Array_APIGenerator_AdditionalTypes_CNodeConfig
	{
		int num;
		struct APIGenerator_AdditionalTypes_CNodeConfig* elems;
	} Array_APIGenerator_AdditionalTypes_CNodeConfig_t;


	/* ========================================================================*/
	/* Struct Definitions                                                      */
	/* ========================================================================*/
	///<summary>Globally unique identifier of a User</summary>
	typedef struct Yodiwo_Plegma_UserKey
	{
		char* UserID;
	} Yodiwo_Plegma_UserKey_t;

	///<summary>Globally unique identifier of a Node</summary>
	typedef struct Yodiwo_Plegma_NodeKey
	{
		Yodiwo_Plegma_UserKey_t UserKey;
		uint32_t NodeID;
	} Yodiwo_Plegma_NodeKey_t;

	///<summary>Globally unique identifier of a Yodiwo.API.Plegma.Thing</summary>
	typedef struct Yodiwo_Plegma_ThingKey
	{
		Yodiwo_Plegma_NodeKey_t NodeKey;
		char* ThingUID;
	} Yodiwo_Plegma_ThingKey_t;

	///<summary>Globally unique identifier of a Yodiwo.API.Plegma.Thing's Yodiwo.API.Plegma.Port</summary>
	typedef struct Yodiwo_Plegma_PortKey
	{
		Yodiwo_Plegma_ThingKey_t ThingKey;
		char* PortUID;
	} Yodiwo_Plegma_PortKey_t;

	typedef struct Yodiwo_Plegma_GraphDescriptorBaseKey
	{
		Yodiwo_Plegma_UserKey_t UserKey;
		char* Id;
	} Yodiwo_Plegma_GraphDescriptorBaseKey_t;

	///<summary>Globally unique identifier of a GraphDescriptor</summary>
	typedef struct Yodiwo_Plegma_GraphDescriptorKey
	{
		Yodiwo_Plegma_GraphDescriptorBaseKey_t BaseKey;
		int32_t Revision;
	} Yodiwo_Plegma_GraphDescriptorKey_t;

	///<summary>Globally unique identifier of a Graph</summary>
	typedef struct Yodiwo_Plegma_GraphKey
	{
		Yodiwo_Plegma_GraphDescriptorKey_t GraphDescriptorKey;
		uint32_t NodeId;
	} Yodiwo_Plegma_GraphKey_t;

	///<summary>Globally unique identifier of a Graph's Block</summary>
	typedef struct Yodiwo_Plegma_BlockKey
	{
		Yodiwo_Plegma_GraphKey_t GraphKey;
		int32_t BlockId;
	} Yodiwo_Plegma_BlockKey_t;

	///<summary>Globally unique identifier of a GroupInfo</summary>
	typedef struct Yodiwo_Plegma_GroupKey
	{
		Yodiwo_Plegma_UserKey_t UserKey;
		int32_t GroupID;
	} Yodiwo_Plegma_GroupKey_t;

	///<summary>Globally unique identifier of a Graph's Block</summary>
	typedef struct Yodiwo_Plegma_DriverKey
	{
		Yodiwo_Plegma_eDriverType Type;
		int32_t Id;
	} Yodiwo_Plegma_DriverKey_t;

	///<summary>Wrapper class mainly for providing synchronization services to sync-less protocols (mqtt, websockets, etc)</summary>
	typedef struct Yodiwo_Plegma_WrapperMsg
	{
		Yodiwo_Plegma_WrapperMsg_eMsgFlags Flags;
		int32_t SyncId;
		char* Payload;
		int32_t PayloadSize;
	} Yodiwo_Plegma_WrapperMsg_t;

	///<summary>Websocket protocol wrapper. Inherits from base WrapperMsg, adds Id and Subid</summary>
	typedef struct Yodiwo_Plegma_WebSocketMsg
	{
		Yodiwo_Plegma_WrapperMsg_eMsgFlags Flags;
		int32_t SyncId;
		char* Payload;
		int32_t PayloadSize;
		Yodiwo_Plegma_WebSocketMsg_eWSMsgId Id;
		char* SubId;
	} Yodiwo_Plegma_WebSocketMsg_t;

	///<summary>Mqtt message encapsulation class.</summary>
	typedef struct Yodiwo_Plegma_MqttMsg
	{
		Yodiwo_Plegma_WrapperMsg_eMsgFlags Flags;
		int32_t SyncId;
		char* Payload;
		int32_t PayloadSize;
	} Yodiwo_Plegma_MqttMsg_t;

	typedef struct Yodiwo_Plegma_YColor
	{
		float R;
		float G;
		float B;
		float A;
	} Yodiwo_Plegma_YColor_t;

	///<summary>Basic Input/Output entity of a Thing Creates and sends messages towards the Yodiwo cloud service,  or receives and handles messages from the cloud. Both events occur via the Yodiwo.API.Plegma.PortEventMsg message</summary>
	typedef struct Yodiwo_Plegma_Port
	{
		char* PortKey;
		char* Name;
		char* Description;
		Yodiwo_Plegma_ioPortDirection ioDirection;
		Yodiwo_Plegma_ePortType Type;
		char* State;
		uint32_t RevNum;
		Yodiwo_Plegma_ePortConf ConfFlags;
	} Yodiwo_Plegma_Port_t;

	typedef struct Yodiwo_Plegma_HttpLocationDescriptor
	{
		char* Uri;
		Yodiwo_Plegma_RestServiceType RestServiceType;
	} Yodiwo_Plegma_HttpLocationDescriptor_t;

	typedef struct Yodiwo_Plegma_RedisDBLocationDescriptor
	{
		char* ConnectionAddress;
		char* DatabaseName;
	} Yodiwo_Plegma_RedisDBLocationDescriptor_t;

	typedef struct Yodiwo_Plegma_DataContentDescriptor
	{
	} Yodiwo_Plegma_DataContentDescriptor_t;

	typedef struct Yodiwo_Plegma_TextContentDescriptor
	{
	} Yodiwo_Plegma_TextContentDescriptor_t;

	typedef struct Yodiwo_Plegma_ImageContentDescriptor
	{
		Yodiwo_Plegma_ImageType Type;
		Yodiwo_Plegma_ImageFileFormat Format;
		int32_t PixelSizeX;
		int32_t PixelSizeY;
		int32_t ColorDepth;
	} Yodiwo_Plegma_ImageContentDescriptor_t;

	typedef struct Yodiwo_Plegma_AudioContentDescriptor
	{
	} Yodiwo_Plegma_AudioContentDescriptor_t;

	typedef struct Yodiwo_Plegma_VideoContentDescriptor
	{
	} Yodiwo_Plegma_VideoContentDescriptor_t;

	///<summary>Configuration parameters for the thing in generic name-value pairs</summary>
	typedef struct Yodiwo_Plegma_ConfigParameter
	{
		char* Name;
		char* Value;
	} Yodiwo_Plegma_ConfigParameter_t;

	///<summary>Collection of instructions ("hints") for how to present this thing in the Cyan UI</summary>
	typedef struct Yodiwo_Plegma_ThingUIHints
	{
		char* IconURI;
		char* Description;
	} Yodiwo_Plegma_ThingUIHints_t;

	///<summary>Main representation of a Thing that can interact with the Yodiwo cloud service</summary>
	typedef struct Yodiwo_Plegma_Thing
	{
		char* ThingKey;
		char* Name;
		Array_Yodiwo_Plegma_ConfigParameter_t Config;
		Array_Yodiwo_Plegma_Port_t Ports;
		char* Type;
		char* BlockType;
		bool Removable;
		Yodiwo_Plegma_ThingUIHints_t UIHints;
	} Yodiwo_Plegma_Thing_t;

	typedef struct Yodiwo_Plegma_GenericRsp
	{
		int32_t SeqNo;
		bool IsSuccess;
		int32_t StatusCode;
		char* Message;
	} Yodiwo_Plegma_GenericRsp_t;

	///<summary>Login Request to be used only for transports that require explicit authentication via the API itself Direction: Cloud to Node</summary>
	typedef struct Yodiwo_Plegma_LoginReq
	{
		int32_t SeqNo;
	} Yodiwo_Plegma_LoginReq_t;

	///<summary>Login Response
///sends node and secret keys
///to be used only for transports that require explicit authentication via the API itself Direction: Node to Cloud</summary>
	typedef struct Yodiwo_Plegma_LoginRsp
	{
		int32_t SeqNo;
		char* NodeKey;
		char* SecretKey;
		Yodiwo_Plegma_eConnectionFlags Flags;
		char* DesiredEndpoint;
	} Yodiwo_Plegma_LoginRsp_t;

	typedef struct Yodiwo_Plegma_StateDescription
	{
		double Minimum;
		double Maximum;
		double Step;
		char* Pattern;
		bool ReadOnly;
		Yodiwo_Plegma_ePortType Type;
	} Yodiwo_Plegma_StateDescription_t;

	///<summary>Describes restrictions and gives information of a configuration parameter.</summary>
	typedef struct Yodiwo_Plegma_ConfigDescription
	{
		char* DefaultValue;
		char* Description;
		char* Label;
		char* Name;
		bool Required;
		char* Type;
		double Minimum;
		double Maximum;
		double Stepsize;
		bool ReadOnly;
	} Yodiwo_Plegma_ConfigDescription_t;

	///<summary>Describes restrictions and gives information of a port Yodiwo.API.Plegma.Port.</summary>
	typedef struct Yodiwo_Plegma_PortDescription
	{
		char* Description;
		char* Id;
		char* Label;
		char* Category;
		Yodiwo_Plegma_StateDescription_t State;
	} Yodiwo_Plegma_PortDescription_t;

	///<summary>Base class that describes a Model of a Thing Yodiwo.API.Plegma.Thing</summary>
	typedef struct Yodiwo_Plegma_ThingModelType
	{
		char* Id;
		char* Name;
		char* Description;
		Array_Yodiwo_Plegma_ConfigDescription_t Config;
		Array_Yodiwo_Plegma_PortDescription_t Port;
	} Yodiwo_Plegma_ThingModelType_t;

	///<summary>Base class that describes a group of Thing Models Yodiwo.API.Plegma.ThingModelType</summary>
	typedef struct Yodiwo_Plegma_ThingType
	{
		char* Type;
		bool Searchable;
		char* Description;
		Array_Yodiwo_Plegma_ThingModelType_t Model;
	} Yodiwo_Plegma_ThingType_t;

	///<summary>Node Info Request Sent by cloud to a node, it is to request capabilities and supported types from the node
///Direction: Cloud->Node
///Node must reply with a Yodiwo.API.Plegma.NodeInfoRsp
///</summary>
	typedef struct Yodiwo_Plegma_NodeInfoReq
	{
		int32_t SeqNo;
		int32_t LatestApiRev;
		char* AssignedEndpoint;
		int32_t ThingsRevNum;
	} Yodiwo_Plegma_NodeInfoReq_t;

	///<summary>Node Info Response Message that contains general information about a node including supported Node Types and Capabilities
///Direction: bidirectional (Node->Cloud and Cloud->Node)
///In response to a Yodiwo.API.Plegma.NodeInfoReq</summary>
	typedef struct Yodiwo_Plegma_NodeInfoRsp
	{
		int32_t SeqNo;
		char* Name;
		Yodiwo_Plegma_eNodeType Type;
		Yodiwo_Plegma_eNodeCapa Capabilities;
		Array_Yodiwo_Plegma_ThingType_t ThingTypes;
		int32_t ThingsRevNum;
		Array_string BlockLibraries;
	} Yodiwo_Plegma_NodeInfoRsp_t;

	///<summary>Unpairing request, stating reason code and a possible custom message Direction: Cloud->Node</summary>
	typedef struct Yodiwo_Plegma_NodeUnpairedReq
	{
		int32_t SeqNo;
		Yodiwo_Plegma_eUnpairReason ReasonCode;
		char* Message;
	} Yodiwo_Plegma_NodeUnpairedReq_t;

	///<summary>Unpairing Response.  Allowed to be empty, exists to make sure that node does receive message before being forcefully disconnected</summary>
	typedef struct Yodiwo_Plegma_NodeUnpairedRsp
	{
		int32_t SeqNo;
	} Yodiwo_Plegma_NodeUnpairedRsp_t;

	///<summary>Endpoint Sync request, providing way for individual Node Links to become aware / influence Node operation Direction: Node(link) -> Cloud</summary>
	typedef struct Yodiwo_Plegma_EndpointSyncReq
	{
		int32_t SeqNo;
		Yodiwo_Plegma_eNodeSyncOperation op;
		char* DesiredEndpoint;
	} Yodiwo_Plegma_EndpointSyncReq_t;

	///<summary>Endpoint Sync response to previous request Direction: Cloud -> Node(link)</summary>
	typedef struct Yodiwo_Plegma_EndpointSyncRsp
	{
		int32_t SeqNo;
		Yodiwo_Plegma_eNodeSyncOperation op;
		Array_string Endpoints;
		bool Accepted;
	} Yodiwo_Plegma_EndpointSyncRsp_t;

	///<summary>Node Things Request Used to request a Yodiwo.API.Plegma.Things related operation from the other end.
///Receiving side *must* reply with a Yodiwo.API.Plegma.ThingsSet. 
///Direction: bidirectional (Node->Cloud and Cloud->Node)
///</summary>
	typedef struct Yodiwo_Plegma_ThingsGet
	{
		int32_t SeqNo;
		Yodiwo_Plegma_eThingsOperation Operation;
		char* ThingKey;
		int32_t RevNum;
	} Yodiwo_Plegma_ThingsGet_t;

	///<summary>Node Things Response Response to a ThingsReq request
///a ThingsRsp message should have:  - ThingsRsp.Operation set to ThingReq's operation              - ThingsRsp.Status set to True if ThingsReq was successfully handled and this Msg has valid data, False otherwise              - if ThingsRsp.Status is True, ThingsRsp.Data set to correspond to requested Req's operation, set to Null otherwise. ThingsRsp.Data is allowed to be null if originally requested operation does not expect back data, only status
///Direction: bidirectional (Node->Cloud and Cloud->Node)
///</summary>
	typedef struct Yodiwo_Plegma_ThingsSet
	{
		int32_t SeqNo;
		Yodiwo_Plegma_eThingsOperation Operation;
		bool Status;
		Array_Yodiwo_Plegma_Thing_t Data;
		int32_t RevNum;
	} Yodiwo_Plegma_ThingsSet_t;

	///<summary>Node Ping Request</summary>
	typedef struct Yodiwo_Plegma_PingReq
	{
		int32_t SeqNo;
		int32_t Data;
	} Yodiwo_Plegma_PingReq_t;

	typedef struct Yodiwo_Plegma_PingRsp
	{
		int32_t SeqNo;
		int32_t Data;
	} Yodiwo_Plegma_PingRsp_t;

	///<summary>Port Event class: used to describe a new event that should trigger an endpoint, either towards a node or the Cloud Services</summary>
	typedef struct Yodiwo_Plegma_PortEvent
	{
		char* PortKey;
		char* State;
		uint32_t RevNum;
		uint64_t Timestamp;
	} Yodiwo_Plegma_PortEvent_t;

	///<summary>asynchronous Port Event message The main API message to exchange events between Nodes and the Yodiwo Cloud Service
///Direction: bidirectional (Node->Cloud and Cloud->Node)
///</summary>
	typedef struct Yodiwo_Plegma_PortEventMsg
	{
		int32_t SeqNo;
		Array_Yodiwo_Plegma_PortEvent_t PortEvents;
	} Yodiwo_Plegma_PortEventMsg_t;

	///<summary>VirtualBlock Event class: used to describe a new event that should trigger a virtual block endpoint, either towards a node or the Cloud Services</summary>
	typedef struct Yodiwo_Plegma_VirtualBlockEvent
	{
		char* BlockKey;
		Array_byte_t Indices;
		Array_string Values;
		uint64_t RevNum;
	} Yodiwo_Plegma_VirtualBlockEvent_t;

	///<summary>asynchronous Port Event message The main API message to exchange events between Nodes and the Yodiwo Cloud Service
///Direction: bidirectional (Node->Cloud and Cloud->Node)
///</summary>
	typedef struct Yodiwo_Plegma_VirtualBlockEventMsg
	{
		int32_t SeqNo;
		Array_Yodiwo_Plegma_VirtualBlockEvent_t BlockEvents;
	} Yodiwo_Plegma_VirtualBlockEventMsg_t;

	///<summary>Port State Request. Will result in a response of type Yodiwo.API.Plegma.PortStateRsp
///Direction: node->cloud
///</summary>
	typedef struct Yodiwo_Plegma_PortStateReq
	{
		int32_t SeqNo;
		Yodiwo_Plegma_ePortStateOperation Operation;
		Array_string PortKeys;
	} Yodiwo_Plegma_PortStateReq_t;

	///<summary>internal state of a referenced Port</summary>
	typedef struct Yodiwo_Plegma_PortState
	{
		char* PortKey;
		char* State;
		uint32_t RevNum;
		bool IsDeployed;
	} Yodiwo_Plegma_PortState_t;

	///<summary>Active Port Keys Msg Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).  Should be used to 1. supress events from inactive ports, allowing more efficient use of medium, 2. sync Port states with the server
///Direction: Cloud -> Node
///</summary>
	typedef struct Yodiwo_Plegma_PortStateRsp
	{
		int32_t SeqNo;
		Yodiwo_Plegma_ePortStateOperation Operation;
		Array_Yodiwo_Plegma_PortState_t PortStates;
	} Yodiwo_Plegma_PortStateRsp_t;

	///<summary>Active Port Keys Msg Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).  Should be used by Nodes to supress events from inactive ports, allowing more efficient use of medium
///Direction: Cloud -> Node
///</summary>
	typedef struct Yodiwo_Plegma_ActivePortKeysMsg
	{
		int32_t SeqNo;
		Array_string ActivePortKeys;
	} Yodiwo_Plegma_ActivePortKeysMsg_t;

	///<summary>Inform server for local deployed graphs (to sync up on connect)</summary>
	typedef struct Yodiwo_Plegma_LocallyDeployedGraphsMsg
	{
		int32_t SeqNo;
		Array_string DeployedGraphKeys;
	} Yodiwo_Plegma_LocallyDeployedGraphsMsg_t;

	///<summary>Node Graph Deploy/Undeploy Request (respond with Yodiwo.API.Plegma.GenericRsp)</summary>
	typedef struct Yodiwo_Plegma_GraphDeploymentReq
	{
		int32_t SeqNo;
		char* GraphKey;
		bool IsDeployed;
		char* GraphDescriptor;
	} Yodiwo_Plegma_GraphDeploymentReq_t;

	typedef struct Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest
	{
		char* uuid;
		char* name;
		char* RedirectUri;
		char* image;
		char* description;
		char* pathcss;
		Array_byte_t PublicKey;
	} Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t;

	typedef struct Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest
	{
		char* token1;
		char* token2;
	} Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t;

	typedef struct Yodiwo_Plegma_NodePairing_PairingServerTokensResponse
	{
		char* token1;
		char* token2;
	} Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t;

	typedef struct Yodiwo_Plegma_NodePairing_PairingServerKeysResponse
	{
		char* nodeKey;
		char* secretKey;
	} Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t;

	typedef struct Yodiwo_Plegma_NodePairing_PairingNodePhase1Response
	{
		char* userNodeRegistrationUrl;
		char* token2;
	} Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t;

	typedef struct APIGenerator_AdditionalTypes_CNodeConfig
	{
		char* Uuid;
		char* Name;
		char* NodeKey;
		char* NodeSecret;
		char* PairingServerUrl;
		char* YPChannelServer;
		int32_t YPChannelServerPort;
		int32_t WebPort;
		char* MqttBrokerHostname;
		int32_t MqttBrokerPort;
		char* MqttBrokerCertFile;
	} APIGenerator_AdditionalTypes_CNodeConfig_t;

	typedef struct APIGenerator_AdditionalTypes_CNodeCConfig
	{
		int ActiveID;
		Array_APIGenerator_AdditionalTypes_CNodeConfig_t Configs;
	} APIGenerator_AdditionalTypes_CNodeCConfig_t;



	/* ========================================================================*/
	/* ToJson Functions Prototypes                                             */
	/* ========================================================================*/

	int Yodiwo_Plegma_UserKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_UserKey_t *value);
	int Yodiwo_Plegma_NodeKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeKey_t *value);
	int Yodiwo_Plegma_ThingKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingKey_t *value);
	int Yodiwo_Plegma_PortKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortKey_t *value);
	int Yodiwo_Plegma_GraphDescriptorBaseKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorBaseKey_t *value);
	int Yodiwo_Plegma_GraphDescriptorKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorKey_t *value);
	int Yodiwo_Plegma_GraphKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphKey_t *value);
	int Yodiwo_Plegma_BlockKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_BlockKey_t *value);
	int Yodiwo_Plegma_GroupKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_GroupKey_t *value);
	int Yodiwo_Plegma_DriverKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_DriverKey_t *value);
	int Yodiwo_Plegma_WrapperMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_WrapperMsg_t *value);
	int Yodiwo_Plegma_WebSocketMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_WebSocketMsg_t *value);
	int Yodiwo_Plegma_MqttMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_MqttMsg_t *value);
	int Yodiwo_Plegma_YColor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_YColor_t *value);
	int Yodiwo_Plegma_Port_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_Port_t *value);
	int Yodiwo_Plegma_HttpLocationDescriptor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_HttpLocationDescriptor_t *value);
	int Yodiwo_Plegma_RedisDBLocationDescriptor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_RedisDBLocationDescriptor_t *value);
	int Yodiwo_Plegma_DataContentDescriptor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_DataContentDescriptor_t *value);
	int Yodiwo_Plegma_TextContentDescriptor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_TextContentDescriptor_t *value);
	int Yodiwo_Plegma_ImageContentDescriptor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ImageContentDescriptor_t *value);
	int Yodiwo_Plegma_AudioContentDescriptor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_AudioContentDescriptor_t *value);
	int Yodiwo_Plegma_VideoContentDescriptor_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_VideoContentDescriptor_t *value);
	int Yodiwo_Plegma_ConfigParameter_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigParameter_t *value);
	int Yodiwo_Plegma_ThingUIHints_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingUIHints_t *value);
	int Yodiwo_Plegma_Thing_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_Thing_t *value);
	int Yodiwo_Plegma_GenericRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_GenericRsp_t *value);
	int Yodiwo_Plegma_LoginReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginReq_t *value);
	int Yodiwo_Plegma_LoginRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginRsp_t *value);
	int Yodiwo_Plegma_StateDescription_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_StateDescription_t *value);
	int Yodiwo_Plegma_ConfigDescription_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigDescription_t *value);
	int Yodiwo_Plegma_PortDescription_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortDescription_t *value);
	int Yodiwo_Plegma_ThingModelType_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingModelType_t *value);
	int Yodiwo_Plegma_ThingType_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingType_t *value);
	int Yodiwo_Plegma_NodeInfoReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoReq_t *value);
	int Yodiwo_Plegma_NodeInfoRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoRsp_t *value);
	int Yodiwo_Plegma_NodeUnpairedReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedReq_t *value);
	int Yodiwo_Plegma_NodeUnpairedRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedRsp_t *value);
	int Yodiwo_Plegma_EndpointSyncReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_EndpointSyncReq_t *value);
	int Yodiwo_Plegma_EndpointSyncRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_EndpointSyncRsp_t *value);
	int Yodiwo_Plegma_ThingsGet_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsGet_t *value);
	int Yodiwo_Plegma_ThingsSet_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsSet_t *value);
	int Yodiwo_Plegma_PingReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PingReq_t *value);
	int Yodiwo_Plegma_PingRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PingRsp_t *value);
	int Yodiwo_Plegma_PortEvent_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEvent_t *value);
	int Yodiwo_Plegma_PortEventMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEventMsg_t *value);
	int Yodiwo_Plegma_VirtualBlockEvent_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEvent_t *value);
	int Yodiwo_Plegma_VirtualBlockEventMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEventMsg_t *value);
	int Yodiwo_Plegma_PortStateReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateReq_t *value);
	int Yodiwo_Plegma_PortState_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortState_t *value);
	int Yodiwo_Plegma_PortStateRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateRsp_t *value);
	int Yodiwo_Plegma_ActivePortKeysMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ActivePortKeysMsg_t *value);
	int Yodiwo_Plegma_LocallyDeployedGraphsMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_LocallyDeployedGraphsMsg_t *value);
	int Yodiwo_Plegma_GraphDeploymentReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDeploymentReq_t *value);
	int Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t *value);
	int Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value);
	int Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t *value);
	int Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t *value);
	int Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t *value);
	int APIGenerator_AdditionalTypes_CNodeConfig_ToJson(char* json, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeConfig_t *value);
	int APIGenerator_AdditionalTypes_CNodeCConfig_ToJson(char* json, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeCConfig_t *value);


	/* ========================================================================*/
	/* FromJson Functions Prototypes                                           */
	/* ========================================================================*/


	typedef enum {
		Yodiwo_JsonSuccessParse = 0,
		Yodiwo_JsonFailedToParse = -1,
		Yodiwo_JsonFailedObjectExpected = -2,
	} Yodiwo_Plegma_Json_e;




	Yodiwo_Plegma_Json_e Yodiwo_Plegma_UserKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_UserKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDescriptorBaseKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorBaseKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDescriptorKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDescriptorKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_BlockKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_BlockKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_GroupKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GroupKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_DriverKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_DriverKey_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_WrapperMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_WrapperMsg_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_WebSocketMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_WebSocketMsg_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_MqttMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_MqttMsg_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_YColor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_YColor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_Port_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Port_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_HttpLocationDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_HttpLocationDescriptor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_RedisDBLocationDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_RedisDBLocationDescriptor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_DataContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_DataContentDescriptor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_TextContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_TextContentDescriptor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ImageContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ImageContentDescriptor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_AudioContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_AudioContentDescriptor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_VideoContentDescriptor_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_VideoContentDescriptor_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigParameter_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigParameter_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingUIHints_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingUIHints_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_Thing_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Thing_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_GenericRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GenericRsp_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginReq_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginRsp_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_StateDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_StateDescription_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigDescription_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortDescription_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingModelType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingModelType_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingType_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoReq_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoRsp_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeUnpairedReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedReq_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeUnpairedRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeUnpairedRsp_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_EndpointSyncReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_EndpointSyncReq_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_EndpointSyncRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_EndpointSyncRsp_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsGet_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsGet_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsSet_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsSet_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PingReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PingReq_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PingRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PingRsp_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEvent_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEvent_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEventMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEventMsg_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_VirtualBlockEvent_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEvent_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_VirtualBlockEventMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_VirtualBlockEventMsg_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateReq_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortState_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortState_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateRsp_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_ActivePortKeysMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ActivePortKeysMsg_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_LocallyDeployedGraphsMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LocallyDeployedGraphsMsg_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_GraphDeploymentReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_GraphDeploymentReq_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t *value);
	Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t *value);
	Yodiwo_Plegma_Json_e APIGenerator_AdditionalTypes_CNodeConfig_FromJson(char* json, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeConfig_t *value);
	Yodiwo_Plegma_Json_e APIGenerator_AdditionalTypes_CNodeCConfig_FromJson(char* json, size_t jsonSize, APIGenerator_AdditionalTypes_CNodeCConfig_t *value);


#ifdef __cplusplus
}
#endif

#endif /* _Yodiwo_Plegma_H_ */
