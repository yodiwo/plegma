//
//  PlegmaApi.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/7/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>

/*!
 * @discussion Unavailable
 */
typedef NS_ENUM(NSInteger, EnumApiMessages)
{
    EnumApiMessages_LoginReq = 0,
    EnumApiMessages_LoginRsp = 1,
    EnumApiMessages_NodeInfoReq = 2,
    EnumApiMessages_NodeInfoRsp = 3,
    EnumApiMessages_ThingsReq = 4,
    EnumApiMessages_ThingsRsp = 5,
    EnumApiMessages_PortEventMsg = 6,
    EnumApiMessages_PortStateReq = 7,
    EnumApiMessages_PortStateRsp = 8,
    EnumApiMessages_ActivePortKeysMsg = 9,
};


@interface PlegmaApi : NSObject

+(NSDictionary *)apiMsgNames;

+(NSInteger)apiVersion;

+(NSString *)keySeparator;

@end
