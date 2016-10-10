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
NSString *const ThingNameWiFiStatus = @"iOSWiFiStatus";
NSString *const ThingNameBluetoothStatus = @"iOSBluetoothStatus";

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
        port.Name = @"Switch state";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_Boolean;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/switch.png";

        [[NodeController sharedNodeController]
                addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                                    name:[deviceName stringByAppendingString:thingUID]
                                                  config:nil
                                                   ports:ports
                                                    type:@"com.yodiwo.input.switches.onoff"
                                               blockType:@""
                                                 uiHints:uiHints]];
    }

    // Virtual slider
    {
        NSString *thingUID = ThingNameVirtualSlider;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Slider value";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_Decimal;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-slider.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"com.yodiwo.output.seekbars"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Virtual text input
    {
        NSString *thingUID = ThingNameVirtualTextInput;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Text";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_String;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-text.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"com.yodiwo.output.text"
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
        cp.Name = @"Location update interval";
        cp.Value = @"";
        [config addObject:cp];

        NSMutableArray *ports = (id)[NSMutableArray new];
        Port *port0 = [[Port  alloc] init];
        port0.Name = @"Friendly location name";
        port0.ioDirection = EnumIOPortDirection_Output;
        port0.Type = EnumPortType_String;
        port0.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];

        Port *port1 = [[Port  alloc] init];
        port1.Name = @"Latitude";
        port1.ioDirection = EnumIOPortDirection_Output;
        port1.Type = EnumPortType_String;
        port1.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"1"] toString];

        Port *port2 = [[Port  alloc] init];
        port2.Name = @"Longitude";
        port2.ioDirection = EnumIOPortDirection_Output;
        port2.Type = EnumPortType_String;
        port2.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"2"] toString];

        [ports addObject:port0];
        [ports addObject:port1];
        [ports addObject:port2];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/gps.png";

        [[NodeController sharedNodeController]
                addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                                    name:[deviceName stringByAppendingString:thingUID]
                                                  config:config
                                                   ports:ports
                                                    type:@"iOSLocation"
                                               blockType:@""
                                                 uiHints:uiHints]];
    }

    // iBeaconReceiver
    {
        NSString *thingUID = ThingNameLocationBeacon;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        NSMutableArray *config = (id)[NSMutableArray new];
        ConfigParameter *cp1 = [[ConfigParameter alloc] init];
        cp1.Name = @"RangedUUIDList";
        cp1.Value = @"";
        cp1.Description = @"Comma separated list of iBeacon UUIDs to be ranged";
        [config addObject:cp1];
        ConfigParameter *cp2 = [[ConfigParameter alloc] init];
        cp2.Name = @"MonitoredUUIDList";
        cp2.Value = @"";
        cp2.Description = @"Comma separated list of iBeacon UUIDs to be monitor";
        [config addObject:cp2];

        NSMutableArray *ports = (id)[NSMutableArray new];
        Port *port0 = [[Port  alloc] init];
        port0.Name = @"iBeacon UUID";
        port0.ioDirection = EnumIOPortDirection_Output;
        port0.Type = EnumPortType_String;
        port0.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"0"] toString];
        port0.ConfFlags = EnumPortConf_SupressIdenticalEvents;

        Port *port1 = [[Port  alloc] init];
        port1.Name = @"iBeacon major value";
        port1.ioDirection = EnumIOPortDirection_Output;
        port1.Type = EnumPortType_Integer;
        port1.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"1"] toString];
        port1.ConfFlags = EnumPortConf_SupressIdenticalEvents;


        Port *port2 = [[Port  alloc] init];
        port2.Name = @"iBeacon minor value";
        port2.ioDirection = EnumIOPortDirection_Output;
        port2.Type = EnumPortType_Integer;
        port2.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"2"] toString];
        port2.ConfFlags = EnumPortConf_SupressIdenticalEvents;


        Port *port3 = [[Port  alloc] init];
        port3.Name = @"Proximity level";
        port3.ioDirection = EnumIOPortDirection_Output;
        port3.Type = EnumPortType_String;
        port3.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"3"] toString];
        port3.ConfFlags = EnumPortConf_SupressIdenticalEvents;

        Port *port4 = [[Port  alloc] init];
        port4.Name = @"RSSI";
        port4.ioDirection = EnumIOPortDirection_Output;
        port4.Type = EnumPortType_Integer;
        port4.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"4"] toString];
        port4.ConfFlags = EnumPortConf_SupressIdenticalEvents;

        [ports addObject:port0];
        [ports addObject:port1];
        [ports addObject:port2];
        [ports addObject:port3];
        [ports addObject:port4];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/ibeacon.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:config
                                            ports:ports
                                             type:@"com.yodiwo.output.beacon"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Proximity sensor
    {
        NSString *thingUID = ThingNameLocationProximity;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"User proximity state";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_Boolean;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-proximity.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"com.yodiwo.output.sensors.proximity"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Accelerometer (shake detector)
    {
        NSString *thingUID = ThingNameShakeDetector;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Shake event";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_Boolean;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-shake.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"com.yodiwo.output.shakedetectors"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Accelerometer, Gyro (Activity tracker)
    {
        NSString *thingUID = ThingNameActivityTracker;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Detected user activity";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_String;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-activitytracker.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSActivityTracker"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // WiFiStatus
    {
        NSString *thingUID = ThingNameWiFiStatus;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"WiFi status";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_String;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-genericwifi.svg";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSWiFiStatus"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // BluetoothStatus
    {
        NSString *thingUID = ThingNameBluetoothStatus;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Bluetooth status";
        port.ioDirection = EnumIOPortDirection_Output;
        port.Type = EnumPortType_String;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];

        Port *port1 = [[Port  alloc] init];
        port1.Name = @"Discovered peripheral";
        port1.ioDirection = EnumIOPortDirection_Output;
        port1.Type = EnumPortType_String;
        port1.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"1"] toString];

        Port *port2 = [[Port  alloc] init];
        port2.Name = @"RSSI";
        port2.ioDirection = EnumIOPortDirection_Output;
        port2.Type = EnumPortType_Integer;
        port2.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"2"] toString];

        Port *port3 = [[Port  alloc] init];
        port3.Name = @"Connected peripheral";
        port3.ioDirection = EnumIOPortDirection_Output;
        port3.Type = EnumPortType_String;
        port3.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                                andPortUid:@"3"] toString];

        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];
        [ports addObject:port1];
        [ports addObject:port2];
        [ports addObject:port3];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-genericbluetooth.svg";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"iOSBlouetoothStatus"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    //
    // TODO: BluetoothControl thing for start/stop discovery ?
    //

    //**************************************************************************




    ///***** Graph output things

    // Virtual text
    {
        NSString *thingUID = ThingNameVirtualText;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Text to show";
        port.ioDirection = EnumIOPortDirection_Input;
        port.Type = EnumPortType_String;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        port.ConfFlags = EnumPortConf_PropagateAllEvents;

        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-text.png";

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
        port.Name = @"Input";
        port.ioDirection = EnumIOPortDirection_Input;
        port.Type = EnumPortType_Boolean;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        port.ConfFlags = EnumPortConf_No;

        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-genericlight.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"com.yodiwo.input.lights.onoff"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Virtual light 2
    {
        NSString *thingUID = ThingNameVirtualLight2;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Input";
        port.ioDirection = EnumIOPortDirection_Input;
        port.Type = EnumPortType_Boolean;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        port.ConfFlags = EnumPortConf_No;

        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-genericlight.png";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"com.yodiwo.input.lights.onoff"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    // Torch
    {
        NSString *thingUID = ThingNameAVTorch;
        ThingKey *thingKey = [[ThingKey alloc] initWithNodeKey:nodeKey
                                                   andThingUid:thingUID];

        Port *port = [[Port  alloc] init];
        port.Name = @"Torch light state";
        port.ioDirection = EnumIOPortDirection_Input;
        port.Type = EnumPortType_Boolean;
        port.PortKey = [[[PortKey alloc] initWithThingKey:thingKey
                                               andPortUid:@"0"] toString];
        port.ConfFlags = EnumPortConf_No;
        
        NSMutableArray *ports = (id)[NSMutableArray new];
        [ports addObject:port];

        ThingUIHints *uiHints = [[ThingUIHints alloc] init];
        uiHints.IconURI = @"/Content/VirtualGateway/img/icon-thing-generictorch.svg";

        [[NodeController sharedNodeController]
         addThing:[[Thing alloc] initWithThingKey:[thingKey toString]
                                             name:[deviceName stringByAppendingString:thingUID]
                                           config:nil
                                            ports:ports
                                             type:@"com.yodiwo.input.torches"
                                        blockType:@""
                                          uiHints:uiHints]];
    }

    //**************************************************************************
}

@end
