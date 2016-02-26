//
//  Public.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/2/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "JSONModel.h"

@protocol Thing;

//******************************************************************************

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumStateOperation)
{
    EnumStateOperation_Invalid = 0,
    EnumStateOperation_SpecificKeys = 1,
    EnumStateOperation_ActivePortStates = 2,
    EnumStateOperation_AllPortStates = 3,
};

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumThingsOperation)
{
    EnumThingsOperation_Invalid = 0,
    EnumThingsOperation_Update = 1,
    EnumThingsOperation_Overwrite = 2,
    EnumThingsOperation_Delete = 3,
    EnumThingsOperation_Get = 4,
    EnumThingsOperation_Scan = 5,
};

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumAPIType)
{
    EnumAPIType_Invalid = 0,

    EnumAPIType_LoginReq = 1,
    EnumAPIType_LoginRsp = 2,

    EnumAPIType_NodeInfoReq = 3,
    EnumAPIType_NodeInfoRsp = 4,

    EnumAPIType_ThingsGet = 5,
    EnumAPIType_ThingsSet = 6,

    EnumAPIType_PortEventMsg = 7,
    EnumAPIType_PortStateReq = 8,
    EnumAPIType_PortStateRsp = 9,
    EnumAPIType_ActivePortKeysMsg = 10,

    EnumAPIType_StreamOpenReq = 20,
    EnumAPIType_StreamOpenRsp = 21,
    EnumAPIType_StreamCloseReq = 22,
    EnumAPIType_StreamCloseRsp = 23,
    EnumAPIType_MjpegServerStartReq = 24,
    EnumAPIType_MjpegServerStartRsp = 25,
    EnumAPIType_MjpegServerStopReq = 26,
    EnumAPIType_MjpegServerStopRsp = 27,

    EnumAPIType_GenericRsp = 28
};

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumENodeType)
{
    EnumENodeType_Unknown = 0,
    EnumENodeType_Gateway = 1,
    EnumENodeType_EndpointSingle = 2,

    EnumENodeType_TestGateway = 3,
    EnumENodeType_TestEndpoint = 4,
    EnumENodeType_WSEndpoint = 5
};

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSUInteger, EnumENodeCapa)
{
    EnumENodeCapa_None = 0,
    EnumENodeCapa_SupportsGraphSplitting = 1,
};

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSUInteger, EnumNodeUnpairedReasonCodes)
{
    EnumNodeUnpairedReasonCodes_Unknown = 0,
    EnumNodeUnpairedReasonCodes_InvalidOperation = 1,
    EnumNodeUnpairedReasonCodes_UserRequested = 2,
    EnumNodeUnpairedReasonCodes_TooManyAuthFailures = 3
};

//******************************************************************************

/*!
 * @discussion Abstract class
 */
@interface APIMsg : JSONModel

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger SeqNo;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface StateDescription : JSONModel

/*!
 * @discussion Minimum value
 */
@property (strong, nonatomic) NSNumber *Minimum; // double

/*!
 * @discussion Maximum value
 */
@property (strong, nonatomic) NSNumber *Maximum; // double

/*!
 * @discussion Change step size
 */
@property (strong, nonatomic) NSNumber *Step; // double

/*!
 * @discussion Pattern to display (can be null)
 */
@property (strong, nonatomic) NSString *Pattern;

/*!
 * @discussion Specifies whether the state is read only
 */
@property (nonatomic) BOOL ReadOnly;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ConfigDescription : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *DefaultValue;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Description;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Label;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Name;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) BOOL Required;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Type;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSNumber *Minimum; // double

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSNumber *Maximum; // double

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSNumber *Stepsize; // double

/*!
 * @discussion Unavailable
 */
@property (nonatomic) BOOL ReadOnly;

@end

/*!
 * @discussion Unavailable
 */
@protocol ConfigDescription

@end

//******************************************************************************

/*!
 * @discussion Describes restrictions and gives information of a port <see cref="Port"/>
 */
@interface PortDescription : JSONModel

/*!
 * @discussion Human readable description for this port (can be null)
 */
@property (strong, nonatomic) NSString *Description;

/*!
 * @discussion The unique identifier which identifies this port (must neither be null, nor empty)
 */
@property (strong, nonatomic) NSString *Id;

/*!
 * @discussion Human readable label (can be null)
 */
@property (strong, nonatomic) NSString *Label;

/*!
 * @discussion The category of this port , e.g. "TEMPERATURE"
 */
@property (strong, nonatomic) NSString *Category;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) StateDescription *Stepsize; // double

@end

/*!
 * @discussion Unavailable
 */
@protocol PortDescription

@end

//******************************************************************************

/*!
 * @discussion Base class that describes a Model of a Thing <see cref="Thing"/>
 */
@interface NodeModelType : JSONModel

/*!
 * @discussion The unique identifier which identifies this model (must neither be null, nor empty)
 */
@property (strong, nonatomic) NSString *Id;

/*!
 * @discussion Human readable name for this model
 */
@property (strong, nonatomic) NSString *Name;

/*!
 * @discussion Human readable description for this model
 */
@property (strong, nonatomic) NSString *Description;

/*!
 * @discussion Describes the configuration parameter(s) of this model<see cref="ConfigDescription"/>
 */
@property (strong, nonatomic) NSMutableArray<ConfigDescription, Optional> *Config; // of ConfigDescription

/*!
 * @discussion Describes the port(s) of this model<see cref="PortDescription"/>
 */
@property (strong, nonatomic) NSMutableArray<PortDescription, Optional> *Port; // of PortDescription

@end

/*!
 * @discussion Unavailable
 */
@protocol NodeModelType

@end

//******************************************************************************

/*!
 * @discussion Base class that describes a group of Thing Models <see cref="NodeModelType"/>
 */
@interface NodeThingType : JSONModel

/*!
 * @discussion The unique identifier which identifies this model (must neither be null, nor empty)
 */
@property (strong, nonatomic) NSString *Type;

/*!
 * @discussion Specifies whether model(s) of this group can automatically be discovered
 */
@property (nonatomic) BOOL Searchable;

/*!
 * @discussion Human readable description for this group
 */
@property (strong, nonatomic) NSString *Description;

/*!
 * @discussion Describes the model(s) of this group<see cref="NodeModelType"/>
 */
@property (strong, nonatomic) NSMutableArray<NodeModelType, Optional> *Model; // of NodeModelType

@end

/*!
 * @discussion Unavailable
 */
@protocol NodeThingType

@end

//******************************************************************************

/*!
 * @discussion Node Info Request
 *
 */
@interface NodeInfoReq : APIMsg

/*!
 * @discussion Informs of latest Plegma API Revision
 */
@property (nonatomic) NSInteger LatestApiRev;

/*!
 * @discussion Endpoint that the node link used to send this message belongs to
 */
@property (strong, nonatomic) NSString<Optional> *AssignedEndpoint;

/*!
 * @discussion Revision number of Cloud server's entry for Node's Things
 */
@property (nonatomic) NSInteger ThingsRevNum;

@end

//******************************************************************************

/*!
 * @discussion Node Info Response
 * Message that contains general information about a node including supported Node Types and Capabilities
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 * In response to a NodeInfoReq
 */
@interface NodeInfoRsp : APIMsg

/*!
* @discussion Friendly name of responding Node
*/
@property (strong, nonatomic) NSString *Name;

/*!
 * @discussion Type of responding Node
 */
@property (nonatomic) EnumENodeType Type;

/*!
 * @discussion Capabilities of this node
 */
@property (nonatomic) EnumENodeCapa Capabilities;

/*!
 * @discussion List of NodeThingType that this Node presents and implements
 */
@property (strong, nonatomic) NSMutableArray<NodeThingType, Optional> *ThingTypes; // of NodeThingType

/*!
 * @discussion Revision number of responding Node's Things
 */
@property (nonatomic) NSInteger ThingsRevNum;


@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ThingsGet : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumThingsOperation Operation;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *ThingKey;

/*!
 * @discussion Things revision number of sender; 0 if not available or applicable
 */
@property (nonatomic) NSInteger RevNum;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ThingsSet : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumThingsOperation Operation;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) BOOL Status;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<Thing,Optional> *Data; // of Thing

/*!
 * @discussion Things revision number of responder to a previous request; can be 0 if not available or applicable
 */
@property (nonatomic) NSInteger RevNum;

@end


//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PortEvent : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *PortKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *State;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger RevNum;

@end

/*!
 * @discussion Unavailable
 */
@protocol PortEvent

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PortEventMsg : APIMsg

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<PortEvent,Optional> *PortEvents;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PortState : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *PortKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *State;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger RevNum;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) BOOL IsDeployed;

@end

/*!
 * @discussion Unavailable
 */
@protocol PortState

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PortStateReq : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumStateOperation Operation;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray *PortKeys;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PortStateRsp : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumStateOperation Operation;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<PortState,Optional> *PortStates;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ActivePortKeysMsg : APIMsg

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<Optional> *ActivePortKeys;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PingReq : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger Data;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PingRsp : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger Data;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface NodeUnpairedReq : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumNodeUnpairedReasonCodes ReasonCode;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *Message;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface NodeUnpairedRsp : APIMsg

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface GenericRsp : APIMsg

/*!
 * @discussion Indicate that the requested action was successful or not
 */
@property (nonatomic) BOOL IsSuccess;

/*!
 * @discussion An optional code for the result
 */
@property (nonatomic) NSInteger StatusCode;

/*!
 * @discussion Ann optional message for the result
 */
@property (strong, nonatomic) NSString *Message;

@end