//
//  Thing.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/2/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "Thing.h"

@implementation Thing

-(instancetype)initWithThingKey:(NSString *)thingKey
                           name:(NSString *)name
                         config:(NSMutableArray<ConfigParameter> *)config
                          ports:(NSMutableArray<Port> *)ports
                           type:(NSString *)type
                      blockType:(NSString *)blockType
                        uiHints:(ThingUIHints *)uiHints {
    if (self = [super init]) {
        _ThingKey = thingKey;
        _Name = name;
        _Config = config;
        _Ports = ports;
        _Type = type;
        _BlockType = blockType;
        _UIHints = uiHints;
    }
    
    return self;
}

@end

//******************************************************************************

@implementation ConfigParameter

@end

//******************************************************************************

@implementation ThingUIHints

@end