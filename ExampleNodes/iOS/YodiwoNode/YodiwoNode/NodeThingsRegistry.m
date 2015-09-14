//
//  NodeThingsRegistry.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/5/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "NodeThingsRegistry.h"

#import "SettingsVault.h"
#import "YodiwoApi.h"
#import "NodeController.h"

#import <UIKit/UIKit.h>


@implementation NodeThingsRegistry

NSString *const ThingNameVirtualSwitch = @"iOSSwitch";
NSString *const ThingNameLocationGPS = @"iOSLocation";
NSString *const ThingNameLocationBeacon = @"iOSBeacon";
NSString *const ThingNameLocationProximity = @"iOSProximity";
NSString *const ThingNameVirtualSlider = @"iOSSlider";
NSString *const ThingNameVirtualTextInput = @"iOSTextInput";
NSString *const ThingNameShakeDetector = @"iOSShakeDetector";
NSString *const ThingNameActivityTracker = @"iOSActivityTracker";

NSString *const ThingNameVirtualText = @"iOSText";
NSString *const ThingNameVirtualLight1 = @"iOSLight1";
NSString *const ThingNameVirtualLight2 = @"iOSLight2";
NSString *const ThingNameAVTorch = @"iOSTorchLight";


+(id)sharedNodeThingsRegistry {
    static NodeThingsRegistry *internalSharedNodeThingsRegistry = nil;
    static dispatch_once_t once_token;

    dispatch_once(&once_token, ^{
        internalSharedNodeThingsRegistry =[[self alloc] init];
    });

    return internalSharedNodeThingsRegistry;
}


- (void)populate {
    NSString *nodeKeyString = [[SettingsVault sharedSettingsVault] getPairingNodeKey];
    NodeKey *nodeKey = [[NodeKey alloc] initWithNodeKeyString:nodeKeyString];
    NSString *deviceName = [[UIDevice currentDevice].name stringByAppendingString:@" "];


    ///***** Graph input things

    // Virtual switch
    {
        NSString *thingUID = ThingNameVirtualSwitch;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Switch state";
        port.ioDirection = EnumIOPortDirection_Output;
        port.type = EnumPortType_Boolean;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/switch.png";

        [[NodeController sharedNodeController]
                addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                                    name:[deviceName stringByAppendingString:thingUID]
                                                  config:nil
                                                   ports:ports
                                                    type:@"iOSVirtual"
                                               blockType:@""
                                                 uiHints:uiHints]];
    }

    // Virtual slider
    {
        NSString *thingUID = ThingNameVirtualSlider;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Slider value";
        port.ioDirection = EnumIOPortDirection_Output;
        port.type = EnumPortType_Decimal;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-slider.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSVirtual"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Virtual text input
    {
        NSString *thingUID = ThingNameVirtualTextInput;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Text";
        port.ioDirection = EnumIOPortDirection_Output;
        port.type = EnumPortType_String;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-text.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSVirtual"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Location finder
    {
        NSString *thingUID = ThingNameLocationGPS;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        NSMutableArray *config = (id)[NSMutableArray new];
        ConfigParameter *cp = [[ConfigParameter alloc] init];
        cp.name = @"Location update interval";
        cp.value = @"";
        [config addObject:cp];

        NSMutableArray *ports = (id)[NSMutableArray new];
        Port *port0 = [[Port  alloc] init];
        port0.name = @"Friendly location name";
        port0.ioDirection = EnumIOPortDirection_Output;
        port0.type = EnumPortType_String;
        port0.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];

        Port *port1 = [[Port  alloc] init];
        port1.name = @"Latitude";
        port1.ioDirection = EnumIOPortDirection_Output;
        port1.type = EnumPortType_String;
        port1.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"1"] toString];

        Port *port2 = [[Port  alloc] init];
        port2.name = @"Longitude";
        port2.ioDirection = EnumIOPortDirection_Output;
        port2.type = EnumPortType_String;
        port2.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"2"] toString];

        [ports addObject:port0];
        [ports addObject:port1];
        [ports addObject:port2];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/gps.png";

        [[NodeController sharedNodeController]
                addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                                    name:[deviceName stringByAppendingString:thingUID]
                                                  config:config
                                                   ports:ports
                                                    type:@"iOSSensor"
                                               blockType:@""
                                                 uiHints:uiHints]];
    }

    // iBeaconReceiver
    {
        NSString *thingUID = ThingNameLocationBeacon;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        NSMutableArray *config = (id)[NSMutableArray new];
        ConfigParameter *cp = [[ConfigParameter alloc] init];
        cp.name = @"TODO: List of iBeacons to range";
        cp.value = @"";
        [config addObject:cp];

        NSMutableArray *ports = (id)[NSMutableArray new];
        Port *port0 = [[Port  alloc] init];
        port0.name = @"iBeacon UUID";
        port0.ioDirection = EnumIOPortDirection_Output;
        port0.type = EnumPortType_String;
        port0.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"0"] toString];

        Port *port1 = [[Port  alloc] init];
        port1.name = @"iBeacon major value";
        port1.ioDirection = EnumIOPortDirection_Output;
        port1.type = EnumPortType_Integer;
        port1.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"1"] toString];

        Port *port2 = [[Port  alloc] init];
        port2.name = @"iBeacon minor value";
        port2.ioDirection = EnumIOPortDirection_Output;
        port2.type = EnumPortType_Integer;
        port2.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"2"] toString];

        Port *port3 = [[Port  alloc] init];
        port3.name = @"Proximity level";
        port3.ioDirection = EnumIOPortDirection_Output;
        port3.type = EnumPortType_String;
        port3.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"3"] toString];

        Port *port4 = [[Port  alloc] init];
        port4.name = @"RSSI";
        port4.ioDirection = EnumIOPortDirection_Output;
        port4.type = EnumPortType_Integer;
        port4.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"4"] toString];

        [ports addObject:port0];
        [ports addObject:port1];
        [ports addObject:port2];
        [ports addObject:port3];
        [ports addObject:port4];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/ibeacon.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:config
                                            ports:ports
                                             type:@"iOSSensor"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Proximity sensor
    {
        NSString *thingUID = ThingNameLocationProximity;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"User proximity state";
        port.ioDirection = EnumIOPortDirection_Output;
        port.type = EnumPortType_Boolean;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-proximity.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSSensor"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Accelerometer (shake detector)
    {
        NSString *thingUID = ThingNameShakeDetector;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Shake event";
        port.ioDirection = EnumIOPortDirection_Output;
        port.type = EnumPortType_Boolean;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-shake.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSSensor"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Accelerometer, Gyro (Activity tracker)
    {
        NSString *thingUID = ThingNameActivityTracker;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Detected user activity";
        port.ioDirection = EnumIOPortDirection_Output;
        port.type = EnumPortType_String;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-activitytracker.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSSensor"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    //**************************************************************************




    ///***** Graph output things

    // Virtual text
    {
        NSString *thingUID = ThingNameVirtualText;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Text to show";
        port.ioDirection = EnumIOPortDirection_Input;
        port.type = EnumPortType_String;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-text.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSVirtual"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Virtual light 1
    {
        NSString *thingUID = ThingNameVirtualLight1;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Input";
        port.ioDirection = EnumIOPortDirection_Input;
        port.type = EnumPortType_Boolean;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-genericlight.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSVirtual"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Virtual light 2
    {
        NSString *thingUID = ThingNameVirtualLight2;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Input";
        port.ioDirection = EnumIOPortDirection_Input;
        port.type = EnumPortType_Boolean;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-genericlight.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSVirtual"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Torch
    {
        NSString *thingUID = ThingNameAVTorch;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.name = @"Torch light state";
        port.ioDirection = EnumIOPortDirection_Input;
        port.type = EnumPortType_Boolean;
        port.portKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.iconUri = @"/Content/VirtualGateway/img/icon-thing-generictorch.svg";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSActuator"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    //**************************************************************************
}

@end
