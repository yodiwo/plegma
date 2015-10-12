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
@property (strong, nonatomic) NSMutableSet *activePortKeysSet;

@property (strong, nonatomic) dispatch_queue_t serialMqttClientPublishQueue;

@property (strong, nonatomic) NSDictionary *apiMsgNameStrToIntHelperDict;

@end

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
                                         [[PlegmaApi apiMsgNames] objectForKey:[LoginRsp class]],*/
                [NSNumber numberWithInteger:EnumApiMessages_NodeInfoReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[NodeInfoReq class]],
                [NSNumber numberWithInteger:EnumApiMessages_NodeInfoRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[NodeInfoRsp class]],
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

- (NSMutableSet *)activePortKeysSet {
    if(_activePortKeysSet == nil) {
        _activePortKeysSet = [NSMutableSet new];
    }

    return _activePortKeysSet;
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
        if ([self.activePortKeysSet containsObject:port.portKey]) {

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
        }
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

- (void)sendSinglePortEventMsgFromThing:(NSString *)thingName
                          fromPortIndex:(NSInteger)portIndex
                               withState:(NSString *)state {

    NSString *thingKey =
    [ThingKey createKeyFromNodeKey:[[SettingsVault sharedSettingsVault] getPairingNodeKey]
                         thingName:thingName];


    // Construct PortEventMsg
    PortEventMsg *msg = [[PortEventMsg alloc] init];
    msg.PortEvents = (id)[NSMutableArray new];


    Port *port = [((Thing *)self.thingsDict[thingKey]).ports objectAtIndex:portIndex];
    if ([self.activePortKeysSet containsObject:port.portKey]) {

        // Update port state
        port.state = state;

        // Construct port event
        PortEvent *portEvent = [[PortEvent alloc] init];
        portEvent.PortKey =  port.portKey;
        portEvent.State = state;
        NSAssert(portEvent.State != nil,
                 @"About to send PortEvent with nil state");
        portEvent.RevNum = 0;

        [msg.PortEvents addObject:portEvent];
    }

    // Convert to JSON for encapsulation
    NSString *payload = [msg toJSONString];

    // Construct final MqttAPIMessage
    MqttAPIMessage *mqttMsg = [[MqttAPIMessage alloc] init];
    mqttMsg.Payload = payload;
    mqttMsg.ResponseToSeqNo = 0;

    // Send, unless none of the thing's ports is part of a deployed graph
    dispatch_async(self.serialMqttClientPublishQueue, ^{
        NSString *apiMsgJson = [mqttMsg toJSONString];
        NSString *topic = [[PlegmaApi apiMsgNames] objectForKey:[PortEventMsg class]];
        [self.mqttClient publishMessage:apiMsgJson inTopic:topic];
    });
}

- (void)sendApiMsgOfType:(NSString *)apiMsgName
          withParameters:(NSArray *)params
                 andData:(NSArray *)data {

    switch ([[self.apiMsgNameStrToIntHelperDict objectForKey:apiMsgName] integerValue]) {

        // NodeInfoRsp
        case EnumApiMessages_NodeInfoRsp:
        {
            // Get params
            NodeInfoReq *req = [params firstObject];

            // Construct
            NodeInfoRsp *rsp = [NodeInfoRsp new];

            rsp.Name = [[SettingsVault sharedSettingsVault] getPairingParamsNodeName];
            rsp.Capabilities = EnumENodeCapa_None;
            rsp.Type = EnumENodeType_TestEndpoint;
            rsp.ThingTypes = (id)[NSMutableArray new];
            for (Thing *thing in [self.thingsDict allValues]) {
                NodeThingType *nodeThingType = [NodeThingType new];
                nodeThingType.Type = thing.type;
                nodeThingType.Searchable = YES;
                nodeThingType.Description = @"";

                nodeThingType.Model = nil;

                [rsp.ThingTypes addObject:nodeThingType];
            }

            // Convert to JSON for encapsulation
            NSString *payload = [rsp toJSONString];

            // Construct final MqttAPIMessage
            MqttAPIMessage *mqttMsg = [[MqttAPIMessage alloc] init];
            mqttMsg.Payload = payload;
            mqttMsg.ResponseToSeqNo = req.SeqNo;

            dispatch_async(self.serialMqttClientPublishQueue, ^{
                NSString *topic = [[PlegmaApi apiMsgNames] objectForKey:[NodeInfoRsp class]];
                [self.mqttClient publishMessage:[mqttMsg toJSONString] inTopic:topic];
            });

            break;
        }

        // PortStateReq
        case EnumApiMessages_PortStateReq:
        {
            // Construct PortStateReq
            PortStateReq *msg = [[PortStateReq alloc] init];
            msg.Operation = EnumStateOperation_ActivePortStates;
            msg.PortKeys = nil;

            // Convert to JSON for encapsulation
            NSString *payload = [msg toJSONString];

            // Construct final MqttAPIMessage
            MqttAPIMessage *mqttMsg = [[MqttAPIMessage alloc] init];
            mqttMsg.Payload = payload;
            mqttMsg.ResponseToSeqNo = 0;

            // Send
            NSString *topic = [[PlegmaApi apiMsgNames] objectForKey:[PortStateReq class]];
            dispatch_async(self.serialMqttClientPublishQueue, ^{
                [self.mqttClient publishMessage:[mqttMsg toJSONString] inTopic:topic];
            });
            
            break;
        }

        // PortStateRsp
        case EnumApiMessages_PortStateRsp:
        {
            break;
        }

        // ThingsReq
        case EnumApiMessages_ThingsReq:
        {
            // Construct ThingsReq
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
            dispatch_async(self.serialMqttClientPublishQueue, ^{
                [self.mqttClient publishMessage:[mqttMsg toJSONString] inTopic:topic];
            });

            break;
        }

        // ThingsRsp
        case EnumApiMessages_ThingsRsp:
        {
            if (self.thingsDict.count == 0) {
                break;
            }

            // Get params
            ThingsReq *req = [params firstObject];

            // Construct ThingsMsg
            ThingsRsp *msg = [[ThingsRsp alloc] init];
            msg.Operation = req.Operation;
            msg.Data = (id)[NSMutableArray new];
            msg.Status = YES;

            // Add all node things
            for (Thing *thing in [self.thingsDict allValues]) {
                [msg.Data addObject:thing];
            }

            // Convert to JSON for encapsulation
            NSString *payload = [msg toJSONString];

            // Construct final MqttAPIMessage
            MqttAPIMessage *mqttMsg = [[MqttAPIMessage alloc] init];
            mqttMsg.Payload = payload;
            mqttMsg.ResponseToSeqNo = req.SeqNo;

            // Send
            NSString *topic = [[PlegmaApi apiMsgNames] objectForKey:[ThingsRsp class]];
            dispatch_async(self.serialMqttClientPublishQueue, ^{
                [self.mqttClient publishMessage:[mqttMsg toJSONString] inTopic:topic];
            });
            
            break;
        }

        default:
        {
            break;
        }
    }
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
            JSONModelError *error;
            NodeInfoReq *req = [[NodeInfoReq alloc] initWithString:payload error:&error];

            // Respond with NodeInfoRsp
            [self sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[NodeInfoRsp class]]
                    withParameters:[NSArray arrayWithObject:req]
                           andData:nil];

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

            // Clear existing set of active port keys
            [self.activePortKeysSet removeAllObjects];

            for (PortState *ps in msg.PortStates) {

                NSLog(@"PortKey: %@, state: %@", ps.PortKey, ps.State);

                // Update port states
                Port *p = [self.portKeyToPortDict objectForKey:ps.PortKey];
                p.state = ps.State;

                // Inform UI
                Thing *t = [self.portKeyToThingDict objectForKey:ps.PortKey];
                NSString *notName = @"yodiwoThingUpdateNotification";
                ThingKey *thingKey = [[ThingKey alloc] initFromString:t.thingKey];
                NSString *thingName = thingKey.thingUID;
                NSNumber *portIndex = [NSNumber numberWithUnsignedInteger:[t.ports indexOfObjectIdenticalTo:p]];
                NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                           thingName, @"thingName",
                                           portIndex, @"portIndex",
                                           p.state, @"newState",
                                           nil];

                [[NSNotificationCenter defaultCenter] postNotificationName:notName
                                                                    object:self
                                                                  userInfo:notParams];

                // Update active port keys set
                if (ps.IsDeployed == YES) {
                    [self.activePortKeysSet addObject:ps.PortKey];
                }
            }

            break;
        }
        case EnumApiMessages_PortStateReq:
        {
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
            // TODO: Check that operation is Get
            JSONModelError *error;
            ThingsReq *req = [[ThingsReq alloc] initWithString:payload error:&error];

            [[NodeController sharedNodeController] sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[ThingsRsp class]]
                                                     withParameters:[NSArray arrayWithObject:req]
                                                            andData:nil];

            break;
        }
        case EnumApiMessages_ActivePortKeysMsg:
        {
            JSONModelError *error;
            ActivePortKeysMsg *msg = [[ActivePortKeysMsg alloc] initWithString:payload error:&error];

            // Clear existing set of active port keys
            [self.activePortKeysSet removeAllObjects];

            // Update from message array
            [self.activePortKeysSet addObjectsFromArray:msg.ActivePortKeys];
            
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
