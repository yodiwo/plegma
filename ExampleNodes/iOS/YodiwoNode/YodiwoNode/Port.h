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
    EnumPortConf_ReceiveAllEvents = 1,

    /*!
     * @discussion Mark the port as a trigger port
     */
    EnumPortConf_IsTrigger = 2,
};

/*!
 * @discussion Unavailable
 */
@interface Port : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *portKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *name;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumIOPortDirection ioDirection;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumPortType type;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *state;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger revNum;

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


