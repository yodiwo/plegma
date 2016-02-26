//
//  PlegmaApi.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/7/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface PlegmaApi : NSObject

+(NSArray *)apiMessages;

+(NSDictionary *)apiMsgNames;

+(NSInteger)apiVersion;

+(NSString *)keySeparator;

@end
