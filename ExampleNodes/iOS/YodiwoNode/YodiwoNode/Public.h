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

//******************************************************************************

/*!
 * @discussion Abstract class
 */
@interface APIMsg : JSONModel

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumAPIType Id;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger Version;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger SeqNo;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger ResponseToSeqNo;

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

