//
//  NodeController.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/3/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "YodiwoApi.h"

@interface NodeController : NSObject

+ (id)sharedNodeController;

- (BOOL)addThing:(Thing *)thing;

- (void)sendPortEventMsgFromThing:(NSString *)thingName
                         withData:(NSArray *)data;

- (void)sendSinglePortEventMsgFromThing:(NSString *)thingName
                          fromPortIndex:(NSInteger)portIndex
                              withState:(NSString *)state;

- (void)sendApiMsgOfType:(NSString *)apiMsgName
          withParameters:(NSArray *)params
                 andData:(NSArray *)data;

- (void)populateNodeThingsRegistry;

- (void)connectToCloudService;

- (void)handleMqttApiMsgWithPayload:(NSString *)payload
                            atTopic:(NSString *)topic
                           retained:(BOOL)retained;

- (void)startLocationManagerModule;

- (void)startMotionManagerModule;

- (void)startBluetoothManagerModule;

@end
