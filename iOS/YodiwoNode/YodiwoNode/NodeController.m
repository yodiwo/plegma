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
#import "BluetoothManagerModule.h"

#import "Helper.h"

#include <libkern/OSAtomic.h>

@interface NodeController ()

@property (strong, nonatomic) MqttClient *mqttClient;
@property (strong, nonatomic) LocationManagerModule *locaModule;
@property (strong, nonatomic) MotionManagerModule *motionModule;
@property (strong, nonatomic) BluetoothManagerModule *btModule;

@property (strong, nonatomic) NSMutableDictionary *thingsDict;
@property (strong, nonatomic) NSMutableDictionary *portKeyToPortDict;
@property (strong, nonatomic) NSMutableDictionary *portKeyToThingDict;
@property (strong, nonatomic) NSMutableSet *activePortKeysSet;

@property (strong, nonatomic) dispatch_queue_t serialMqttClientPublishQueue;

@property (strong, nonatomic) NSDictionary *apiMsgNameStrToIntHelperDict;

@property (strong, nonatomic) NSDictionary *customMsgNameStrToIntHelperDict;

@property (strong, nonatomic) NSString *unknownRspStr;

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
    EnumApiMessages_ThingsGet = 4,
    EnumApiMessages_ThingsSet = 5,
    EnumApiMessages_PortEventMsg = 6,
    EnumApiMessages_PortStateReq = 7,
    EnumApiMessages_PortStateRsp = 8,
    EnumApiMessages_ActivePortKeysMsg = 9,
    EnumApiMessages_PingReq = 10,
    EnumApiMessages_PingRsp = 11,
    EnumApiMessages_NodeUnpairedReq = 12,
    EnumApiMessages_NodeUnpairedRsp = 13,
    EnumApiMessages_GenericRsp = 14
};

typedef NS_ENUM(NSInteger, EnumCustomMessages)
{
    EnumCustomMessages_UnknownRsp = 0
};

@implementation NodeController

volatile NSInteger __monotonicId = 0;


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

        _btModule = [[BluetoothManagerModule alloc] init];

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
                [NSNumber numberWithInteger:EnumApiMessages_ThingsGet],
                                         [[PlegmaApi apiMsgNames] objectForKey:[ThingsGet class]],
                [NSNumber numberWithInteger:EnumApiMessages_ThingsSet],
                                         [[PlegmaApi apiMsgNames] objectForKey:[ThingsSet class]],
                [NSNumber numberWithInteger:EnumApiMessages_PortEventMsg],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PortEventMsg class]],
                [NSNumber numberWithInteger:EnumApiMessages_PortStateReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PortStateReq class]],
                [NSNumber numberWithInteger:EnumApiMessages_PortStateRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PortStateRsp class]],
                [NSNumber numberWithInteger:EnumApiMessages_ActivePortKeysMsg],
                                         [[PlegmaApi apiMsgNames] objectForKey:[ActivePortKeysMsg class]],
                [NSNumber numberWithInteger:EnumApiMessages_PingReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PingReq class]],
                [NSNumber numberWithInteger:EnumApiMessages_PingRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[PingRsp class]],
                [NSNumber numberWithInteger:EnumApiMessages_NodeUnpairedReq],
                                         [[PlegmaApi apiMsgNames] objectForKey:[NodeUnpairedReq class]],
                [NSNumber numberWithInteger:EnumApiMessages_NodeUnpairedRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[NodeUnpairedRsp class]],
                [NSNumber numberWithInteger:EnumApiMessages_GenericRsp],
                                         [[PlegmaApi apiMsgNames] objectForKey:[GenericRsp class]],
                nil];

        _unknownRspStr = @"UnknownRsp";

        _customMsgNameStrToIntHelperDict = [NSDictionary dictionaryWithObjectsAndKeys:[NSNumber numberWithInteger:EnumCustomMessages_UnknownRsp], _unknownRspStr, nil];
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

- (void)clearThings {

    [self.thingsDict removeAllObjects];
    [self.portKeyToPortDict removeAllObjects];
    [self.portKeyToThingDict removeAllObjects];
}

- (void)removeThing:(NSString *)thingKey {
    if (thingKey == nil) {
        return;
    }

    Thing *thing = [self.thingsDict objectForKey:thingKey];
    for (Port *port in thing.ports) {
        [self.portKeyToPortDict removeObjectForKey:port.portKey];
        [self.portKeyToThingDict removeObjectForKey:port.portKey];
    }

    [self.thingsDict removeObjectForKey:thingKey];
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

    if ([[SettingsVault sharedSettingsVault] isNodeApiConnected] == NO) {
        return;
    }

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

    if (msg.PortEvents == nil || [msg.PortEvents count] == 0) {
        return;
    }

    // Convert to JSON for encapsulation
    NSString *payload = [msg toJSONString];

    [self transportSendApiMsg:payload forApiMsgName:[[PlegmaApi apiMsgNames] objectForKey:[PortEventMsg class]]];
}

- (void)sendSinglePortEventMsgFromThing:(NSString *)thingName
                          fromPortIndex:(NSInteger)portIndex
                               withState:(NSString *)state {

    if ([[SettingsVault sharedSettingsVault] isNodeApiConnected] == NO) {
        return;
    }

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

    if (msg.PortEvents == nil || [msg.PortEvents count] == 0) {
        return;
    }

    // Convert to JSON for encapsulation
    NSString *payload = [msg toJSONString];

    [self transportSendApiMsg:payload forApiMsgName:[[PlegmaApi apiMsgNames] objectForKey:[PortEventMsg class]]];
}

- (void)sendApiMsgOfType:(NSString *)apiMsgName
              withSyncId:(NSInteger)syncId
          withParameters:(NSArray *)params
                 andData:(NSArray *)data {

    switch ([[self.apiMsgNameStrToIntHelperDict objectForKey:apiMsgName] integerValue]) {

        // NodeInfoRsp
        case EnumApiMessages_NodeInfoRsp:
        {
            // Construct
            NodeInfoRsp *rsp = [NodeInfoRsp new];

            rsp.Name = [[SettingsVault sharedSettingsVault] getPairingParamsNodeName];
            rsp.Capabilities = EnumENodeCapa_None;
            rsp.Type = EnumENodeType_TestEndpoint;
            rsp.ThingTypes = (id)[NSMutableArray new];
            rsp.ThingsRevNum = [[SettingsVault sharedSettingsVault] getNodeThingsRevNum];
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

            [self transportSendApiRsp:payload forApiMsgName:apiMsgName withSyncId:syncId];

            // API-connected at this point
            if ([[SettingsVault sharedSettingsVault] isNodeApiConnected] == NO) {
                NSString *notName = @"yodiwoNodeIsApiConnectedNotification";
                [[NSNotificationCenter defaultCenter] postNotificationName:notName
                                                                    object:self
                                                                  userInfo:nil];
            }

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

            [self transportSendApiReq:payload forApiMsgName:apiMsgName];
            
            break;
        }

        // PortStateRsp
        case EnumApiMessages_PortStateRsp:
        {
            PortStateRsp *rsp = [[PortStateRsp alloc] init];
            rsp.Operation = EnumStateOperation_ActivePortStates;
            rsp.PortStates = (id)[NSMutableArray new];

            for (NSString *pk in self.activePortKeysSet) {
                Port *p = [self.portKeyToPortDict objectForKey:pk];

                PortState *ps = [PortState new];
                ps.PortKey = pk;
                ps.State = p.state;
                ps.RevNum = 0;
                ps.IsDeployed = YES;

                [rsp.PortStates addObject:ps];
            }

            NSString *payload = [rsp toJSONString];

            [self transportSendApiRsp:payload forApiMsgName:apiMsgName withSyncId:syncId];

            break;
        }

        // ThingsGet
        case EnumApiMessages_ThingsGet:
        {
            // Construct ThingsReq
            ThingsGet *msg = [[ThingsGet alloc] init];
            msg.Operation = EnumThingsOperation_Get;

            // Convert to JSON for encapsulation
            NSString *payload = [msg toJSONString];

            [self transportSendApiReq:payload forApiMsgName:apiMsgName];

            break;
        }

        // ThingsSet
        case EnumApiMessages_ThingsSet:
        {
            if (self.thingsDict.count == 0) {
                break;
            }

            // Get params
            ThingsGet *req = [params firstObject];

            // Construct ThingsSet
            ThingsSet *msg = [[ThingsSet alloc] init];
            msg.Data = (id)[NSMutableArray new];
            msg.RevNum = [[SettingsVault sharedSettingsVault] getNodeThingsRevNum];

            if (req.Operation == EnumThingsOperation_Get) {

                msg.Operation = EnumThingsOperation_Update;

                // Add all node things
                for (Thing *thing in [self.thingsDict allValues]) {
                    [msg.Data addObject:thing];
                }

                msg.Status = YES;
            }
            else {
                msg.Operation = req.Operation;
                msg.Status = NO;
            }

            // Convert to JSON for encapsulation
            NSString *payload = [msg toJSONString];

            [self transportSendApiRsp:payload forApiMsgName:apiMsgName withSyncId:syncId];

            break;
        }

        // PingReq
        case EnumApiMessages_PingRsp:
        {
            // Get params
            PingReq *req = [params firstObject];

            // Construct response
            PingRsp *rsp = [[PingRsp alloc] init];
            rsp.Data = req.Data;

            // Convert to JSON for encapsulation
            NSString *payload = [rsp toJSONString];

            [self transportSendApiRsp:payload forApiMsgName:apiMsgName withSyncId:syncId];
            
            break;
        }

        // NodeUnpairedRsp
        case EnumApiMessages_NodeUnpairedRsp:
        {
            // Construct response
            NodeUnpairedRsp *rsp = [[NodeUnpairedRsp alloc] init];

            // Convert to JSON for encapsulation
            NSString *payload = [rsp toJSONString];

            [self transportSendApiRsp:payload forApiMsgName:apiMsgName withSyncId:syncId];

            break;
        }

        case EnumCustomMessages_UnknownRsp:
        {
            UnknownRsp *rsp = [[UnknownRsp alloc] init];
            NSString *payload = [rsp toJSONString];

            [self transportSendApiRsp:payload forApiMsgName:apiMsgName withSyncId:syncId];
        }

        default:
        {
            break;
        }
    }
}


- (void)handleApiMsg:(NSString *)apiMsgName withPayload:(NSString *)payload withSyncId:(NSInteger)syncId {

    if (apiMsgName == nil) {
        [self sendApiMsgOfType:self.unknownRspStr withSyncId:syncId withParameters:nil andData:nil];
        return;
    }

    switch ([[self.apiMsgNameStrToIntHelperDict objectForKey:apiMsgName] integerValue]) {
        case EnumApiMessages_LoginReq:
        {
            [self sendApiMsgOfType:self.unknownRspStr withSyncId:syncId withParameters:nil andData:nil];

            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_LoginRsp:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_NodeInfoReq:
        {
            JSONModelError *error;
            NodeInfoReq *req = [[NodeInfoReq alloc] initWithString:payload error:&error];

            if (error != nil) {
                NSLog(@" %@ deserialization error: %@ (Sending UnknownRsp)", apiMsgName, error.description);

                [self sendApiMsgOfType:self.unknownRspStr withSyncId:syncId withParameters:nil andData:nil];

                break;
            }

            // Respond with NodeInfoRsp
            [self sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[NodeInfoRsp class]]
                        withSyncId:syncId
                    withParameters:[NSArray arrayWithObject:req]
                           andData:nil];

            // Send ThingsGet if out of sync
            if (req.ThingsRevNum > [[SettingsVault sharedSettingsVault] getNodeThingsRevNum]) {
                [self sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[ThingsGet class]]
                            withSyncId:0
                        withParameters:nil
                               andData:nil];
            }
            
            break;
        }
        case EnumApiMessages_NodeInfoRsp:
        {
            NSLog(@"Handler not implemented for ApiMessage of type: %@", apiMsgName);
            break;
        }
        case EnumApiMessages_PortStateReq:
        {
            JSONModelError *error;
            PortStateReq *req = [[PortStateReq alloc] initWithString:payload error:&error];

            if (error != nil) {
                NSLog(@" %@ deserialization error: %@ (Sending UnknownRsp)", apiMsgName, error.description);

                [self sendApiMsgOfType:self.unknownRspStr withSyncId:syncId withParameters:nil andData:nil];

                break;
            }

            [self sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[PortStateRsp class]]
                        withSyncId:syncId
                    withParameters:[NSArray arrayWithObject:req]
                           andData:nil];
            break;
        }
        case EnumApiMessages_PortStateRsp:
        {
            JSONModelError *error;
            PortStateRsp *msg = [[PortStateRsp alloc] initWithString:payload error:&error];

            NSInteger op = msg.Operation;
            NSLog(@"PortStateRsp operation %ld", (long)op);

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
        case EnumApiMessages_ThingsGet:
        {
            JSONModelError *error;
            ThingsGet *req = [[ThingsGet alloc] initWithString:payload error:&error];

            if (error != nil) {
                NSLog(@" %@ deserialization error: %@ (Sending UnknownRsp)", apiMsgName, error.description);

                [self sendApiMsgOfType:self.unknownRspStr withSyncId:syncId withParameters:nil andData:nil];

                break;
            }

            [[NodeController sharedNodeController] sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[ThingsSet class]]
                                                         withSyncId:syncId
                                                     withParameters:[NSArray arrayWithObject:req]
                                                            andData:nil];
            
            break;
        }
        case EnumApiMessages_ThingsSet:
        {
            JSONModelError *error;
            ThingsSet *msg = [[ThingsSet alloc] initWithString:payload error:&error];

            if (msg.Operation == EnumThingsOperation_Update) {
                [[SettingsVault sharedSettingsVault] setNodeThingsRevNum:msg.RevNum];

                for (Thing *thing in msg.Data) {
                    [self removeThing:thing.thingKey];
                    [self addThing:thing];
                }
            }
            else if (msg.Operation == EnumThingsOperation_Overwrite) {
                [[SettingsVault sharedSettingsVault] setNodeThingsRevNum:msg.RevNum];

                [self clearThings];

                for (Thing *thing in msg.Data) {
                    [self addThing:thing];
                }
            }

            break;
        }
        case EnumApiMessages_PingReq:
        {
            JSONModelError *error;
            PingReq *msg = [[PingReq alloc] initWithString:payload error:&error];

            if (error != nil) {
                NSLog(@" %@ deserialization error: %@ (Sending UnknownRsp)", apiMsgName, error.description);

                [self sendApiMsgOfType:self.unknownRspStr withSyncId:syncId withParameters:nil andData:nil];

                break;
            }

            // Send PingRsp
            [self sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[PingRsp class]]
                        withSyncId:syncId
                    withParameters:[NSArray arrayWithObject:msg]
                           andData:nil];

            break;
        }
        case EnumApiMessages_PingRsp:
        {
            JSONModelError *error;
            PingRsp *msg = [[PingRsp alloc] initWithString:payload error:&error];

            NSLog(@"Rx ping response: %d", msg.Data);
            
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
        case EnumApiMessages_NodeUnpairedReq:
        {
            JSONModelError *error;
            NodeUnpairedReq *req = [[NodeUnpairedReq alloc] initWithString:payload error:&error];

            if (error != nil) {
                NSLog(@" %@ deserialization error: %@ (Sending UnknownRsp)", apiMsgName, error.description);

                [self sendApiMsgOfType:self.unknownRspStr withSyncId:syncId withParameters:nil andData:nil];

                break;
            }

            // Send response
            [self sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[NodeUnpairedRsp class]]
                        withSyncId:syncId
                    withParameters:[NSArray arrayWithObject:req]
                           andData:nil];

            // Inform interested controllers
            NSString *rcStr = req.ReasonCode == EnumNodeUnpairedReasonCodes_InvalidOperation ? @"Invalid Operation" :
                              req.ReasonCode == EnumNodeUnpairedReasonCodes_TooManyAuthFailures ? @"Too many Authentication Failures" :
                              req.ReasonCode == EnumNodeUnpairedReasonCodes_UserRequested ? @"User Requested" : @"Unknown Reason Code";

            NSString *notName = @"yodiwoNodeUnpairedNotification";
            NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                       rcStr, @"reasoncode",
                                       req.Message, @"message",
                                       nil];

            [[NSNotificationCenter defaultCenter] postNotificationName:notName
                                                                object:self
                                                              userInfo:notParams];

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

- (void)startBluetoothManagerModule {
    [self.btModule start];
}
//******************************************************************************


//***** Helpers

// Monotonically increasing synchronization id generation
- (NSInteger)getNextSyncId {
    return (NSInteger)OSAtomicIncrement32Barrier(((volatile int32_t *)&__monotonicId));
}

//******************************************************************************


//***** Transport abstraction layer

- (void)transportSendApiReq:(NSString *)payload forApiMsgName:(NSString *)apiMsgName {

    //*** MQTT

    MqttMsg *mqttMsg = [[MqttMsg alloc] init];
    mqttMsg.Payload = payload;
    mqttMsg.SyncId = [self getNextSyncId]; // Fill with monotonically increasing id
    mqttMsg.Flags = EnumMessageFlags_Request;

    dispatch_async(self.serialMqttClientPublishQueue, ^{
        [self.mqttClient publishMessage:[mqttMsg toJSONString] inTopic:apiMsgName];
    });

    // TODO: Add support for more transports
}

- (void)transportSendApiRsp:(NSString *)payload forApiMsgName:(NSString *)apiMsgName withSyncId:(NSUInteger)syncId {

    //*** MQTT

    MqttMsg *mqttMsg = [[MqttMsg alloc] init];
    mqttMsg.Payload = payload;
    mqttMsg.SyncId = syncId;
    mqttMsg.Flags = EnumMessageFlags_Response;

    dispatch_async(self.serialMqttClientPublishQueue, ^{
        [self.mqttClient publishMessage:[mqttMsg toJSONString] inTopic:apiMsgName];
    });

    // TODO: Add support for more transports
}

- (void)transportSendApiMsg:(NSString *)payload forApiMsgName:(NSString *)apiMsgName {

    //*** MQTT

    MqttMsg *mqttMsg = [[MqttMsg alloc] init];
    mqttMsg.Payload = payload;
    mqttMsg.SyncId = 0;
    mqttMsg.Flags = EnumMessageFlags_Message;

    dispatch_async(self.serialMqttClientPublishQueue, ^{
        [self.mqttClient publishMessage:[mqttMsg toJSONString] inTopic:apiMsgName];
    });

    // TODO: Add support for more transports
}
//******************************************************************************

@end
