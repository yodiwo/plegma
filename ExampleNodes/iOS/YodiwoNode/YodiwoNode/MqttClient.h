//
//  MqttClientService.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/30/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MqttClient : NSObject

- (void)start;

- (void)disconnect;

- (void)connect;

- (void)subscribe;

- (void)publishMessage:(NSString *)msg inTopic:(NSString *)topic;

@end
