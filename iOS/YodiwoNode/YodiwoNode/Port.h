//
//  Port.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/2/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "JSONModel.h"

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumPortType)
{
    EnumPortType_Undefined = 0,
    EnumPortType_Integer = 1,
    EnumPortType_Decimal = 2,
    EnumPortType_DecimalHigh = 3,
    EnumPortType_Boolean = 4,
    EnumPortType_Color = 5,
    EnumPortType_String = 6,
};

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumIOPortDirection)
{
    EnumIOPortDirection_Undefined = 0,
    EnumIOPortDirection_InputOutput = 1,
    EnumIOPortDirection_Output = 2,
    EnumIOPortDirection_Input = 3,
};

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumPortConf)
{
    /*!
     * @discussion No configuration set
     */
    EnumPortConf_No = 0,

    /*!
     * @discussion Port should receive all event , not only dirty ones
     */
    EnumPortConf_PropagateAllEvents = 1,

    /*!
     * @discussion Mark the port as a trigger port
     */
    EnumPortConf_IsTrigger = 2,

    /*!
     * @discussion Enable this flag to force raw values for the port.
     */
    EnumPortConf_DoNotNormalize = 4,

    /*!
     * @discussion If set port will only propagate "dirty" events, where the value actually changed and was not just triggered.
     */
    EnumPortConf_SupressIdenticalEvents = 8,
};

/*!
 * @discussion Unavailable
 */
@interface Port : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *PortKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *Name;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *Description;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumIOPortDirection ioDirection;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *Semantics;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumPortType Type;

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
@property (nonatomic) EnumPortConf ConfFlags;

@end

/*!
 * @discussion Unavailable
 */
@protocol Port

@end

//******************************************************************************


