//
//  ProtocolWrappers.h
//  YodiwoHub
//
//  Created by r00tb00t on 12/2/15.
//  Copyright © 2015 yodiwo. All rights reserved.
//

#import "JSONModel.h"

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumMessageFlags)
{
    EnumMessageFlags_Message = 0,
    EnumMessageFlags_Request = 1,
    EnumMessageFlags_Response = 2
};

@interface WrapperMsg : JSONModel

/*!
 * @discussion Unavailable
 */
@property (nonatomic) EnumMessageFlags Flags;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger SyncId;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Payload;


@end
