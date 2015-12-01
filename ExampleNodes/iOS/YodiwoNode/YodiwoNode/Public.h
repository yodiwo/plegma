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

    EnumAPIType_ThingsReq = 5,
    EnumAPIType_ThingsRsp = 6,

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
 * @discussion Reserved for future use; ignore
 */
@property (strong, nonatomic) NodeThingType<Optional> *RequestedThingType;

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

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ThingsReq : APIMsg

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumThingsOperation Operation;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *ThingKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<Thing, Optional> *Data; // of Thing

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ThingsRsp : APIMsg

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
@property (strong, nonatomic) NSMutableArray<PortEvent> *PortEvents;

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
@property (strong, nonatomic) NSMutableArray<PortState> *PortStates;

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

