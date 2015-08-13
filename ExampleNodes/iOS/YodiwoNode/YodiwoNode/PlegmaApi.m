//
//  PlegmaApi.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/7/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "PlegmaApi.h"


@implementation PlegmaApi

+(NSInteger)apiVersion {
    return 1;
}

+(NSString *)keySeparator {
    return @"-";
}

+(NSDictionary *)apiMsgNames {
    static NSDictionary *_apiMsgNames = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _apiMsgNames = [NSDictionary dictionaryWithObjectsAndKeys:
                        [NSNumber numberWithInteger:EnumApiMessages_LoginReq], @"loginreq",
                        [NSNumber numberWithInteger:EnumApiMessages_LoginRsp], @"loginrsp",
                        [NSNumber numberWithInteger:EnumApiMessages_NodeInfoReq], @"nodeinforeq",
                        [NSNumber numberWithInteger:EnumApiMessages_NodeInfoRsp], @"nodeinforsp",
                        [NSNumber numberWithInteger:EnumApiMessages_ThingsReq], @"thingsreq",
                        [NSNumber numberWithInteger:EnumApiMessages_ThingsRsp], @"thingsrsp",
                        [NSNumber numberWithInteger:EnumApiMessages_PortEventMsg], @"porteventmsg",
                        [NSNumber numberWithInteger:EnumApiMessages_PortStateReq], @"portstatereq",
                        [NSNumber numberWithInteger:EnumApiMessages_PortStateRsp], @"portstatersp",
                        [NSNumber numberWithInteger:EnumApiMessages_ActivePortKeysMsg], @"activeportkeysmsg",
                        nil];
    });

    return _apiMsgNames;
}

@end
