//
//  NodeThingsRegistry.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/5/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NodeThingsRegistry : NSObject

// ThingIn
extern NSString *const ThingNameVirtualSwitch;
extern NSString *const ThingNameLocationGPS;
extern NSString *const ThingNameLocationBeacon;
extern NSString *const ThingNameLocationProximity;
extern NSString *const ThingNameVirtualSlider;
extern NSString *const ThingNameVirtualTextInput;
extern NSString *const ThingNameShakeDetector;
extern NSString *const ThingNameActivityTracker;

// ThingOut
extern NSString *const ThingNameVirtualText;
extern NSString *const ThingNameVirtualLight1;
extern NSString *const ThingNameVirtualLight2;
extern NSString *const ThingNameAVTorch;

+(id)sharedNodeThingsRegistry;

- (void)populate;

@end
