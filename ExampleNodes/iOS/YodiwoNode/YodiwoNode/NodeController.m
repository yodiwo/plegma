//
//  NodeController.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/3/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "NodeController.h"
#import "MqttClient.h"
#import "YodiwoApi.h"
#import "SettingsVault.h"
#import "NodeThingsRegistry.h"
#import "LocationManagerModule.h"
#import "MotionManagerModule.h"

@interface NodeController ()

@property (strong, nonatomic) MqttClient *mqttClient;
@property (strong, nonatomic) LocationManagerModule *locaModule;
@property (strong, nonatomic) MotionManagerModule *motionModule;

@property (strong, nonatomic) NSMutableDictionary *thingsDict;
@property (strong, nonatomic) NSMutableDictionary *portKeyToPortDict;
@property (strong, nonatomic) NSMutableDictionary *portKeyToThingDict;

@property (strong, nonatomic) dispatch_queue_t serialMqttClientPublishQueue;

@end

@implementation NodeController

///***** Singleton

+(id)sharedNodeController {
    static NodeController *internalSharedNodeController = nil;
    static dispatch_once_t once_token;

    dispatch_once(&once_token, ^{
        internalSharedNodeController =[[self alloc] init];
    });

    return internalSharedNodeController;
}

- (instancetype)init {
    if (self = [super init]) {
        _serialMqttClientPublishQueue = dispatch_queue_create("serialMqttClientPublishQueue",
                                                              DISPATCH_QUEUE_SERIAL);
        _mqttClient = [[MqttClient alloc] init];

        _locaModule = [[LocationManagerModule alloc] init];

        _motionModule = [[MotionManagerModule alloc] init];
    }

    return self;
}
//******************************************************************************



///***** Override synthesized getters, lazy instantiate

- (NSMutableDictionary *)thingsDict {
    if(_thingsDict == nil) {
        _thingsDict = [[NSMutableDictionary alloc] init];
    }

    return _thingsDict;
}

- (NSMutableDictionary *)portKeyToPortDict {
    if(_portKeyToPortDict == nil) {
        _portKeyToPortDict = [[NSMutableDictionary alloc] init];
    }

    return _portKeyToPortDict;
}


- (NSMutableDictionary *)portKeyToThingDict {
    if(_portKeyToThingDict == nil) {
        _portKeyToThingDict = [[NSMutableDictionary alloc] init];
    }

    return _portKeyToThingDict;
}

//******************************************************************************



///***** Public api
- (void)connectToCloudService {
    [self.mqttClient start];
}

- (void)populateNodeThingsRegistry {
    [[NodeThingsRegistry sharedNodeThingsRegistry] populate];
}

- (BOOL)addThing:(Thing *)thing {

    if (thing == nil) {
        return false;
    }

    [self.thingsDict setObject:thing forKey:thing.thingKey];

    for (Port *port in thing.ports) {
        [self.portKeyToPortDict setObject:port forKey:port.portKey];
        [self.portKeyToThingDict setObject:thing forKey:port.portKey];
    }

    return true;
}

- (void)sendPortEventMsgFromThing:(NSString *)thingName
                         withData:(NSArray *)data {

    NSString *thingKey =
        [ThingKey createKeyFromNodeKey:[[SettingsVault sharedSettingsVault] getPairingNodeKey]
                            thingName:thingName];


    // Construct and send PortEventMsg to cloud service
    PortEventMsg *msg = [[PortEventMsg alloc] init];
    msg.PortEvents = (id)[NSMutableArray new];


    NSUInteger portIndex = 0;
    for (Port *port in ((Thing *)self.thingsDict[thingKey]).ports) {
        //if (port.numOfActiveGraphs > 0) { // TODO: uncomment when update of field from cloud implememented

        // Update port state
        port.state = [data objectAtIndex:portIndex];

        // Construct port event
        PortEvent *portEvent = [[PortEvent alloc] init];
        portEvent.PortKey =  port.portKey;
        portEvent.State = [data objectAtIndex:portIndex];
        if (portEvent.State == nil) {
            NSAssert(true, @"About to send PortEvent with nil state");
        }
        portEvent.RevNum = 0;
        [msg.PortEvents addObject:portEvent];
        //}
        portIndex++;
    }

    // Send, unless none of the thing's ports is part of a deployed graph
    if (msg.PortEvents.count > 0) {
        dispatch_async(self.serialMqttClientPublishQueue, ^{
            NSString *apiMsgJson = [msg toJSONString];
            NSLog(@"Sending PortEventMsg: %@", apiMsgJson);

            NSString *topic = [[[PlegmaApi apiMsgNames] allKeysForObject:
                                [NSNumber numberWithInteger:EnumApiMessages_PortEventMsg]] lastObject];
            [self.mqttClient publishMessage:apiMsgJson
                                    inTopic:topic];
        });
    }
}

- (void)sendNodeThingsMsg {

    if (self.thingsDict.count == 0) {
        return;
    }

    // Construct and send ThingsMsg to cloud service
    ThingsReq *msg = [[ThingsReq alloc] init];
    msg.Operation = EnumThingsOperation_Update;
    msg.Data = (id)[NSMutableArray new];

    // Add all node things
    for (Thing *thing in [self.thingsDict allValues]) {
        [msg.Data addObject:thing];
    }

    // Send
    NSString *topic = [[[PlegmaApi apiMsgNames] allKeysForObject:
                        [NSNumber numberWithInteger:EnumApiMessages_ThingsReq]] lastObject];
    [self.mqttClient publishMessage:[msg toJSONString]
                            inTopic:topic];
}

- (void)handleMqttApiMsgWithPayload:(NSString *)payload
                            atTopic:(NSString *)topic
                           retained:(BOOL)retained {

    NSString *apiMsgName = [[topic componentsSeparatedByString:@"/"] lastObject];

    switch ([[[PlegmaApi apiMsgNames] objectForKey:apiMsgName] integerValue]) {
        case EnumApiMessages_LoginReq:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_LoginRsp:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_NodeInfoRsp:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_NodeInfoReq:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_PortEventMsg:
        {
            JSONModelError *error;
            PortEventMsg *msg = [[PortEventMsg alloc] initWithString:payload error:&error];

            for (PortEvent *pevent in msg.PortEvents) {

                Port *port = [self.portKeyToPortDict objectForKey:pevent.PortKey];
                if (port.ioDirection == EnumIOPortDirection_Output ||
                    port.ioDirection == EnumIOPortDirection_Undefined) {

                    NSLog(@"****** PortEvent discarded (target port not input) ******");
                    continue;
                }

                // Update state
                port.state = pevent.State;

                // Post notification to be picked up by interested ViewControllers
                Thing *t = [self.portKeyToThingDict objectForKey:port.portKey];
                NSString *notName = @"yodiwoThingUpdateNotification";
                ThingKey *thingKey = [[ThingKey alloc] initFromString:t.thingKey];
                NSString *thingName = thingKey.thingUID;
                NSNumber *portIndex = [NSNumber numberWithUnsignedInteger:[t.ports indexOfObjectIdenticalTo:port]];
                NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                                            thingName, @"thingName",
                                                            portIndex, @"portIndex",
                                                            port.state, @"newState",
                                                            nil];

                [[NSNotificationCenter defaultCenter] postNotificationName:notName
                                                                    object:self
                                                                  userInfo:notParams];
            }

            break;
        }
        case EnumApiMessages_PortStateRsp:
        {
            JSONModelError *error;
            PortStateRsp *msg = [[PortStateRsp alloc] initWithString:payload error:&error];

            NSInteger op = msg.Operation;
            NSLog(@"PortStateMsg operation %ld", (long)op);

            for (PortState *ps in msg.PortStates) {
                // Update thing states
                NSLog(@"PortKey: %@, state: %@", ps.PortKey, ps.State);
            }

            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);

            break;
        }
        case EnumApiMessages_PortStateReq:
        {
            JSONModelError *error;
            PortStateReq *msg = [[PortStateReq alloc] initWithString:payload error:&error];

            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);

            break;
        }
        case EnumApiMessages_ThingsRsp:
        {
            JSONModelError *error;
            ThingsRsp *msg = [[ThingsRsp alloc] initWithString:payload error:&error];

            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);

            break;
        }
        case EnumApiMessages_ThingsReq:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_ActivePortKeysMsg:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        default:
        {
            NSLog(@"Invalid ApiMessage type: %@", apiMsgName);
            break;
        }
    }
}

- (void)startLocationManagerModule {
    [self.locaModule start];
}

- (void)startMotionManagerModule {
    [self.motionModule start];
}
//******************************************************************************

@end
