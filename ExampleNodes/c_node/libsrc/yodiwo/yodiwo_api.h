/**
 * Created by ApiGenerator Tool (C) on 15/9/2015 4:32:53 &#956;&#956;.
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
        Yodiwo_eNodeType_Unknown = 0,
        Yodiwo_eNodeType_Gateway = 1,
        Yodiwo_eNodeType_EndpointSingle = 2,
        Yodiwo_eNodeType_TestGateway = 3,
        Yodiwo_eNodeType_TestEndpoint = 4,
        Yodiwo_eNodeType_WSEndpoint = 5,
    } Yodiwo_Plegma_eNodeType;

    typedef enum
    {
        Yodiwo_eNodeCapa_None = 0,
        Yodiwo_eNodeCapa_SupportsGraphSplitting = 1,
    } Yodiwo_Plegma_eNodeCapa;

    typedef enum
    {
        Yodiwo_eThingsOperation_Invalid = 0,
        Yodiwo_eThingsOperation_Update = 1,
        Yodiwo_eThingsOperation_Overwrite = 2,
        Yodiwo_eThingsOperation_Delete = 3,
        Yodiwo_eThingsOperation_Get = 4,
        Yodiwo_eThingsOperation_Scan = 5,
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
    } Yodiwo_Plegma_PairingStates;



    /* ========================================================================*/
    /* Struct Prototypes                                                       */
    /* ========================================================================*/
    struct Yodiwo_Plegma_UserKey;
    struct Yodiwo_Plegma_NodeKey;
    struct Yodiwo_Plegma_ThingKey;
    struct Yodiwo_Plegma_PortKey;
    struct Yodiwo_Plegma_GraphDescriptorBaseKey;
    struct Yodiwo_Plegma_GraphDescriptorKey;
    struct Yodiwo_Plegma_GraphKey;
    struct Yodiwo_Plegma_BlockKey;
    struct Yodiwo_Plegma_TimelineDescriptorKey;
    struct Yodiwo_Plegma_Mqtt_MqttAPIMessage;
    struct Yodiwo_Plegma_Port;
    struct Yodiwo_Plegma_ConfigParameter;
    struct Yodiwo_Plegma_ThingUIHints;
    struct Yodiwo_Plegma_Thing;
    struct Yodiwo_Plegma_LoginReq;
    struct Yodiwo_Plegma_LoginRsp;
    struct Yodiwo_Plegma_StateDescription;
    struct Yodiwo_Plegma_ConfigDescription;
    struct Yodiwo_Plegma_PortDescription;
    struct Yodiwo_Plegma_NodeModelType;
    struct Yodiwo_Plegma_NodeThingType;
    struct Yodiwo_Plegma_NodeInfoReq;
    struct Yodiwo_Plegma_NodeInfoRsp;
    struct Yodiwo_Plegma_ThingsReq;
    struct Yodiwo_Plegma_ThingsRsp;
    struct Yodiwo_Plegma_PortEvent;
    struct Yodiwo_Plegma_PortEventMsg;
    struct Yodiwo_Plegma_PortStateReq;
    struct Yodiwo_Plegma_PortState;
    struct Yodiwo_Plegma_PortStateRsp;
    struct Yodiwo_Plegma_ActivePortKeysMsg;
    struct Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest;
    struct Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest;
    struct Yodiwo_Plegma_NodePairing_PairingServerTokensResponse;
    struct Yodiwo_Plegma_NodePairing_PairingServerKeysResponse;
    struct Yodiwo_Plegma_NodePairing_PairingNodePhase1Response;
    struct Yodiwo_Tools_APIGenerator_CNodeConfig;
    struct Yodiwo_Tools_APIGenerator_CNodeYConfig;

    /* Array helper structs */
    struct Array_Yodiwo_Plegma_ConfigParameter;
    struct Array_Yodiwo_Plegma_Port;
    struct Array_Yodiwo_Plegma_ConfigDescription;
    struct Array_Yodiwo_Plegma_PortDescription;
    struct Array_Yodiwo_Plegma_NodeModelType;
    struct Array_Yodiwo_Plegma_NodeThingType;
    struct Array_Yodiwo_Plegma_Thing;
    struct Array_Yodiwo_Plegma_PortEvent;
    struct Array_string;
    struct Array_Yodiwo_Plegma_PortState;
    struct Array_Yodiwo_Tools_APIGenerator_CNodeConfig;

    /* ========================================================================*/
    /* Struct Helpers                                                          */
    /* ========================================================================*/

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

    typedef struct Array_Yodiwo_Plegma_NodeModelType
    {
        int num;
        struct Yodiwo_Plegma_NodeModelType* elems;
    } Array_Yodiwo_Plegma_NodeModelType_t;

    typedef struct Array_Yodiwo_Plegma_NodeThingType
    {
        int num;
        struct Yodiwo_Plegma_NodeThingType* elems;
    } Array_Yodiwo_Plegma_NodeThingType_t;

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

    typedef struct Array_string
    {
        int num;
        char** elems;
    } Array_string;

    typedef struct Array_Yodiwo_Plegma_PortState
    {
        int num;
        struct Yodiwo_Plegma_PortState* elems;
    } Array_Yodiwo_Plegma_PortState_t;

    typedef struct Array_Yodiwo_Tools_APIGenerator_CNodeConfig
    {
        int num;
        struct Yodiwo_Tools_APIGenerator_CNodeConfig* elems;
    } Array_Yodiwo_Tools_APIGenerator_CNodeConfig_t;


    /* ========================================================================*/
    /* Struct Definitions                                                      */
    /* ========================================================================*/
    ///<summary>Globally unique identifier of a </summary>
    typedef struct Yodiwo_Plegma_UserKey
    {
        char* UserID;
    } Yodiwo_Plegma_UserKey_t;

    ///<summary>Globally unique identifier of a Node</summary>
    typedef struct Yodiwo_Plegma_NodeKey
    {
        Yodiwo_Plegma_UserKey_t UserKey;
        int32_t NodeID;
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
        Yodiwo_Plegma_UserKey_t UserKey;
        char* Id;
        int32_t Revision;
    } Yodiwo_Plegma_GraphDescriptorKey_t;

    ///<summary>Globally unique identifier of a Graph</summary>
    typedef struct Yodiwo_Plegma_GraphKey
    {
        Yodiwo_Plegma_GraphDescriptorKey_t GraphDescriptorKey;
        int32_t GraphId;
    } Yodiwo_Plegma_GraphKey_t;

    ///<summary>Globally unique identifier of a Graph's Block</summary>
    typedef struct Yodiwo_Plegma_BlockKey
    {
        Yodiwo_Plegma_GraphKey_t GraphKey;
        int32_t BlockId;
    } Yodiwo_Plegma_BlockKey_t;

    ///<summary>Globally unique identifier of a TimelineDescriptor</summary>
    typedef struct Yodiwo_Plegma_TimelineDescriptorKey
    {
        Yodiwo_Plegma_UserKey_t UserKey;
        char* Id;
    } Yodiwo_Plegma_TimelineDescriptorKey_t;

    ///<summary>Mqtt message encapsulation class.</summary>
    typedef struct Yodiwo_Plegma_Mqtt_MqttAPIMessage
    {
        int32_t ResponseToSeqNo;
        char* Payload;
    } Yodiwo_Plegma_Mqtt_MqttAPIMessage_t;

    ///<summary>Basic Input/Output entity of a Thing Creates and sends messages towards the Yodiwo cloud service,  or receives and handles messages from the cloud. Both events occur via the Yodiwo.API.Plegma.PortEventMsg message</summary>
    typedef struct Yodiwo_Plegma_Port
    {
        char* PortKey;
        char* Name;
        char* Description;
        Yodiwo_Plegma_ioPortDirection ioDirection;
        Yodiwo_Plegma_ePortType Type;
        char* State;
        int32_t RevNum;
        Yodiwo_Plegma_ePortConf ConfFlags;
    } Yodiwo_Plegma_Port_t;

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
        Yodiwo_Plegma_ThingUIHints_t UIHints;
    } Yodiwo_Plegma_Thing_t;

    ///<summary>Login Request to be used only for transports that require explicit authentication via the API itself</summary>
    typedef struct Yodiwo_Plegma_LoginReq
    {
        int32_t SeqNo;
    } Yodiwo_Plegma_LoginReq_t;

    ///<summary>Login Response
    ///sends node and secret keys
    ///to be used only for transports that require explicit authentication via the API itself</summary>
    typedef struct Yodiwo_Plegma_LoginRsp
    {
        int32_t SeqNo;
        char* NodeKey;
        char* SecretKey;
    } Yodiwo_Plegma_LoginRsp_t;

    typedef struct Yodiwo_Plegma_StateDescription
    {
        double Minimum;
        double Maximum;
        double Step;
        char* Pattern;
        bool ReadOnly;
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
    typedef struct Yodiwo_Plegma_NodeModelType
    {
        char* Id;
        char* Name;
        char* Description;
        Array_Yodiwo_Plegma_ConfigDescription_t Config;
        Array_Yodiwo_Plegma_PortDescription_t Port;
    } Yodiwo_Plegma_NodeModelType_t;

    ///<summary>Base class that describes a group of Thing Models Yodiwo.API.Plegma.NodeModelType</summary>
    typedef struct Yodiwo_Plegma_NodeThingType
    {
        char* Type;
        bool Searchable;
        char* Description;
        Array_Yodiwo_Plegma_NodeModelType_t Model;
    } Yodiwo_Plegma_NodeThingType_t;

    ///<summary>Node Info Request If sent by cloud to a node, it is to request capabilities and supported types from the node If sent by a node to the cloud, then Yodiwo.API.Plegma.NodeInfoReq.RequestedThingType must be set             and can be used to perform discovery with the user's connected nodes (currently unavailable)
    ///Direction: bidirectional (Node->Cloud and Cloud->Node)
    ///Receiving end must reply with a Yodiwo.API.Plegma.NodeInfoRsp
    ///</summary>
    typedef struct Yodiwo_Plegma_NodeInfoReq
    {
        int32_t SeqNo;
        Yodiwo_Plegma_NodeThingType_t RequestedThingType;
    } Yodiwo_Plegma_NodeInfoReq_t;

    ///<summary>Node Info Response Message that contains gneral information about a node including supported Node Types and Capabilities
    ///Direction: bidirectional (Node->Cloud and Cloud->Node)
    ///In response to a Yodiwo.API.Plegma.NodeInfoReq</summary>
    typedef struct Yodiwo_Plegma_NodeInfoRsp
    {
        int32_t SeqNo;
        char* Name;
        Yodiwo_Plegma_eNodeType Type;
        Yodiwo_Plegma_eNodeCapa Capabilities;
        Array_Yodiwo_Plegma_NodeThingType_t ThingTypes;
    } Yodiwo_Plegma_NodeInfoRsp_t;

    ///<summary>Node Things Request Used to request a Yodiwo.API.Plegma.Things related operation from the other end.
    ///Receiving side *must* reply with a Yodiwo.API.Plegma.ThingsRsp.              Its ApiMsg.ResponseToSeqNo field *must* be set to this message's Yodiwo.API.Plegma.ApiMsg.SeqNo
    ///Direction: bidirectional (Node->Cloud and Cloud->Node)
    ///</summary>
    typedef struct Yodiwo_Plegma_ThingsReq
    {
        int32_t SeqNo;
        Yodiwo_Plegma_eThingsOperation Operation;
        char* ThingKey;
        Array_Yodiwo_Plegma_Thing_t Data;
    } Yodiwo_Plegma_ThingsReq_t;

    ///<summary>Node Things Response Response to a Yodiwo.API.Plegma.ThingsReq request
    ///a ThingsRsp message should have:  - Yodiwo.API.Plegma.ThingsRsp.Operation set to ThingReq's operation              - ApiMsg.ResponseToSeqNo set to ThingReq's Yodiwo.API.Plegma.ApiMsg.SeqNo- Yodiwo.API.Plegma.ThingsRsp.Status set to True if ThingsReq was successfully handled and this Msg has valid data, False otherwise              - if Yodiwo.API.Plegma.ThingsRsp.Status is True, Yodiwo.API.Plegma.ThingsRsp.Data set to correspond to requested Req's operation, set to Null otherwise. Yodiwo.API.Plegma.ThingsRsp.Data is allowed to be null if originally requested operation does not expect back data, only status
    ///Direction: bidirectional (Node->Cloud and Cloud->Node)
    ///</summary>
    typedef struct Yodiwo_Plegma_ThingsRsp
    {
        int32_t SeqNo;
        Yodiwo_Plegma_eThingsOperation Operation;
        bool Status;
        Array_Yodiwo_Plegma_Thing_t Data;
    } Yodiwo_Plegma_ThingsRsp_t;

    ///<summary>Port Event class: used to describe a new event that should trigger en endpoint, either towards a node or the Cloud Services</summary>
    typedef struct Yodiwo_Plegma_PortEvent
    {
        char* PortKey;
        char* State;
        int32_t RevNum;
    } Yodiwo_Plegma_PortEvent_t;

    ///<summary>asynchronous Port Event message The main API message to exchange events between Nodes and the Yodiwo Cloud Service
    ///Direction: bidirectional (Node->Cloud and Cloud->Node)
    ///</summary>
    typedef struct Yodiwo_Plegma_PortEventMsg
    {
        int32_t SeqNo;
        Array_Yodiwo_Plegma_PortEvent_t PortEvents;
    } Yodiwo_Plegma_PortEventMsg_t;

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
        int32_t RevNum;
        bool IsDeployed;
    } Yodiwo_Plegma_PortState_t;

    ///<summary>Active Port Keys Msg Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).  Should be used to 1. supress events from inactive ports, allowing more efficient use of medium, 2. sync Port states with the server
    ///Can be either asynchronous (e.g. at Node connection) or as a response to a PortUpdateReq
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

    typedef struct Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest
    {
        char* uuid;
        char* name;
    } Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t;

    typedef struct Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest
    {
        char* uuid;
        char* token1;
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

    typedef struct Yodiwo_Tools_APIGenerator_CNodeConfig
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
    } Yodiwo_Tools_APIGenerator_CNodeConfig_t;

    typedef struct Yodiwo_Tools_APIGenerator_CNodeYConfig
    {
        int32_t ActiveID;
        Array_Yodiwo_Tools_APIGenerator_CNodeConfig_t Configs;
    } Yodiwo_Tools_APIGenerator_CNodeYConfig_t;



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
    int Yodiwo_Plegma_TimelineDescriptorKey_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_TimelineDescriptorKey_t *value);
    int Yodiwo_Plegma_Mqtt_MqttAPIMessage_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_Mqtt_MqttAPIMessage_t *value);
    int Yodiwo_Plegma_Port_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_Port_t *value);
    int Yodiwo_Plegma_ConfigParameter_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigParameter_t *value);
    int Yodiwo_Plegma_ThingUIHints_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingUIHints_t *value);
    int Yodiwo_Plegma_Thing_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_Thing_t *value);
    int Yodiwo_Plegma_LoginReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginReq_t *value);
    int Yodiwo_Plegma_LoginRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginRsp_t *value);
    int Yodiwo_Plegma_StateDescription_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_StateDescription_t *value);
    int Yodiwo_Plegma_ConfigDescription_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigDescription_t *value);
    int Yodiwo_Plegma_PortDescription_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortDescription_t *value);
    int Yodiwo_Plegma_NodeModelType_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeModelType_t *value);
    int Yodiwo_Plegma_NodeThingType_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeThingType_t *value);
    int Yodiwo_Plegma_NodeInfoReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoReq_t *value);
    int Yodiwo_Plegma_NodeInfoRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoRsp_t *value);
    int Yodiwo_Plegma_ThingsReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsReq_t *value);
    int Yodiwo_Plegma_ThingsRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsRsp_t *value);
    int Yodiwo_Plegma_PortEvent_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEvent_t *value);
    int Yodiwo_Plegma_PortEventMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEventMsg_t *value);
    int Yodiwo_Plegma_PortStateReq_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateReq_t *value);
    int Yodiwo_Plegma_PortState_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortState_t *value);
    int Yodiwo_Plegma_PortStateRsp_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateRsp_t *value);
    int Yodiwo_Plegma_ActivePortKeysMsg_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_ActivePortKeysMsg_t *value);
    int Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t *value);
    int Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value);
    int Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t *value);
    int Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t *value);
    int Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_ToJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t *value);
    int Yodiwo_Tools_APIGenerator_CNodeConfig_ToJson(char* json, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeConfig_t *value);
    int Yodiwo_Tools_APIGenerator_CNodeYConfig_ToJson(char* json, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeYConfig_t *value);


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
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_TimelineDescriptorKey_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_TimelineDescriptorKey_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_Mqtt_MqttAPIMessage_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Mqtt_MqttAPIMessage_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_Port_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Port_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigParameter_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigParameter_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingUIHints_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingUIHints_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_Thing_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_Thing_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginReq_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_LoginRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_LoginRsp_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_StateDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_StateDescription_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_ConfigDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ConfigDescription_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortDescription_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortDescription_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeModelType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeModelType_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeThingType_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeThingType_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoReq_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodeInfoRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodeInfoRsp_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsReq_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_ThingsRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ThingsRsp_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEvent_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEvent_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortEventMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortEventMsg_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateReq_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateReq_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortState_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortState_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_PortStateRsp_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_PortStateRsp_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_ActivePortKeysMsg_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_ActivePortKeysMsg_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetTokensRequest_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodeGetKeysRequest_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_FromJson(char* json, size_t jsonSize, Yodiwo_Plegma_NodePairing_PairingNodePhase1Response_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Tools_APIGenerator_CNodeConfig_FromJson(char* json, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeConfig_t *value);
    Yodiwo_Plegma_Json_e Yodiwo_Tools_APIGenerator_CNodeYConfig_FromJson(char* json, size_t jsonSize, Yodiwo_Tools_APIGenerator_CNodeYConfig_t *value);


#ifdef __cplusplus
}
#endif

#endif /* _Yodiwo_Plegma_H_ */
