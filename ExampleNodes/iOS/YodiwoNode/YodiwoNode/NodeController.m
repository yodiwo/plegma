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


@interface NodeController ()

@property (strong, nonatomic) MqttClient *mqttClient;
@property (strong, nonatomic) LocationManagerModule *locaModule;
@property (strong, nonatomic) MotionManagerModule *motionModule;

@property (strong, nonatomic) NSMutableDictionary *thingsDict;
@property (strong, nonatomic) NSMutableDictionary *portKeyToPortDict;
@property (strong, nonatomic) NSMutableDictionary *portKeyToThingDict;

@property (strong, nonatomic) dispatch_queue_t serialMqttClientPublishQueue;

@property (strong, nonatomic) NSDictionary *apiMsgNameStrToIntHelperDict;
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

        _apiMsgNameStrToIntHelperDict = [NSDictionary dictionaryWithObjectsAndKeys:

                // TODO: Uncomment when implemented
                /*[NSNumber numberWithInteger:EnumApiMessages_LoginReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[LoginReq class]],
                //[NSNumber numberWithInteger:EnumApiMessages_LoginRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[LoginRsp class]],
                //[NSNumber numberWithInteger:EnumApiMessages_NodeInfoReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[NodeInfoReq class]],
                //[NSNumber numberWithInteger:EnumApiMessages_NodeInfoRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[NodeInfoRsp class]],*/
                [NSNumber numberWithInteger:EnumApiMessages_ThingsReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[ThingsReq class]],
                [NSNumber numberWithInteger:EnumApiMessages_ThingsRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[ThingsRsp class]],
                [NSNumber numberWithInteger:EnumApiMessages_PortEventMsg],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PortEventMsg class]],
                [NSNumber numberWithInteger:EnumApiMessages_PortStateReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PortStateReq class]],
                [NSNumber numberWithInteger:EnumApiMessages_PortStateRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PortStateRsp class]],
                [NSNumber numberWithInteger:EnumApiMessages_ActivePortKeysMsg],
                                         [[PlegmaApi apiMsgNames] objectForKey:[ActivePortKeysMsg class]],
                nil];
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


    // Construct PortEventMsg
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

    // Convert to JSON for encapsulation
    NSString *payload = [msg toJSONString];

    // Construct final MqttAPIMessage
    MqttAPIMessage *mqttMsg = [[MqttAPIMessage alloc] init];
    mqttMsg.Payload = payload;
    mqttMsg.ResponseToSeqNo = 0;

    // Send, unless none of the thing's ports is part of a deployed graph
    if (msg.PortEvents.count > 0) {
        dispatch_async(self.serialMqttClientPublishQueue, ^{
            NSString *apiMsgJson = [mqttMsg toJSONString];
            NSLog(@"Sending PortEventMsg: %@", apiMsgJson);

            NSString *topic = [[PlegmaApi apiMsgNames] objectForKey:[PortEventMsg class]];
            [self.mqttClient publishMessage:apiMsgJson
                                    inTopic:topic];
        });
    }

    // TODO: Move MqttAPIMessage construction within publishMessage:inTopic, changing
    // it to publishMessageWithPayload:(NSString *)payload
    //               asResponseToSeqNo:(NSInteger)seqNo
    //                         inTopic:(NSString *)topic
}

- (void)sendNodeThingsMsg {

    if (self.thingsDict.count == 0) {
        return;
    }

    // Construct ThingsMsg
    ThingsReq *msg = [[ThingsReq alloc] init];
    msg.Operation = EnumThingsOperation_Update;
    msg.Data = (id)[NSMutableArray new];

    // Add all node things
    for (Thing *thing in [self.thingsDict allValues]) {
        [msg.Data addObject:thing];
    }

    // Convert to JSON for encapsulation
    NSString *payload = [msg toJSONString];

    // Construct final MqttAPIMessage
    MqttAPIMessage *mqttMsg = [[MqttAPIMessage alloc] init];
    mqttMsg.Payload = payload;
    mqttMsg.ResponseToSeqNo = 0;

    // Send
    NSString *topic = [[PlegmaApi apiMsgNames] objectForKey:[ThingsReq class]];
    [self.mqttClient publishMessage:[mqttMsg toJSONString]
                            inTopic:topic];
}

- (void)handleMqttApiMsgWithPayload:(NSString *)payload
                            atTopic:(NSString *)topic
                           retained:(BOOL)retained {

    NSString *apiMsgName = [[topic componentsSeparatedByString:@"/"] lastObject];

    switch ([[self.apiMsgNameStrToIntHelperDict objectForKey:apiMsgName] integerValue]) {
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
