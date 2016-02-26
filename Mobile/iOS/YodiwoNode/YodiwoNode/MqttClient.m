//
//  MqttClientService.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/30/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "MqttClient.h"
#import "SettingsVault.h"
#import "NodeController.h"
#import "YodiwoApi.h"

#define kMqttTopic_publish @"/api/in/"
#define kMqttTopic_subscribe @"/api/out/"


@interface MqttClient ()

@property (strong, nonatomic) MQTTSession *mqttClient;
@property (strong, nonatomic) NSString *subscribeTopic;
@property (strong, nonatomic) NSString *publishTopic;

@property (nonatomic) BOOL hasDisconnected;

@end


@implementation MqttClient

-(instancetype)init {
    if (self = [super init]) {
        
        _mqttClient = [[MQTTSession alloc] initWithClientId:[[SettingsVault sharedSettingsVault] getPairingNodeKey]
                                                   userName:[[SettingsVault sharedSettingsVault] getPairingNodeKey]
                                                   password:[[SettingsVault sharedSettingsVault] getPairingSecretKey]
                                                  keepAlive:60
                                               cleanSession:NO
                                                       will:NO
                                                  willTopic:nil
                                                    willMsg:nil
                                                    willQoS:0
                                             willRetainFlag:NO
                                              protocolLevel:4
                                                    runLoop:nil
                                                    forMode:nil
                                             securityPolicy:nil
                                               certificates:nil];
    }

    return self;
}

// Start MQTT client
- (void)start
{
    // Add current API version to base subscribe/publish topics
    NSString *baseSubscribeTopic = [[kMqttTopic_subscribe stringByAppendingString:[NSString stringWithFormat:@"%ld", (long)[PlegmaApi apiVersion]]]
                                    stringByAppendingString:@"/"];

    NSString *basePublishTopic = [[kMqttTopic_publish stringByAppendingString:[NSString stringWithFormat:@"%ld", (long)[PlegmaApi apiVersion]]]
                                  stringByAppendingString:@"/"];


    _subscribeTopic = [[baseSubscribeTopic stringByAppendingString:[[SettingsVault sharedSettingsVault] getPairingNodeKey]]
                       stringByAppendingString:@"/#"];

    _publishTopic = [[[[basePublishTopic stringByAppendingString:[[SettingsVault sharedSettingsVault] getUserKey]]
                       stringByAppendingString:@"/"]
                       stringByAppendingString:[[SettingsVault sharedSettingsVault] getPairingNodeKey] ]
                       stringByAppendingString:@"/"];


    // Update client authentication information
    self.mqttClient.clientId = [[SettingsVault sharedSettingsVault] getPairingNodeKey];
    self.mqttClient.userName = [[SettingsVault sharedSettingsVault] getPairingNodeKey];
    self.mqttClient.password = [[SettingsVault sharedSettingsVault] getPairingSecretKey];

    [self.mqttClient setDelegate:self];

    [self connect];
}

// Disconnect client from MQTT broker
- (void)disconnect{
    [self.mqttClient close];
}

// Connect client to MQTT broker
- (void)connect {

    [self.mqttClient connectToHost:[[SettingsVault sharedSettingsVault] getMqttParamsBrokerAddress]
                              port:([[SettingsVault sharedSettingsVault] getMqttParamsSecureConnection] ? 8883 : 1883)
                          usingSSL:[[SettingsVault sharedSettingsVault] getMqttParamsSecureConnection]
                    connectHandler:^(NSError *error) {

                        if (error == nil) {
                            NSLog(@"MQTT client: connected with id: %@", self.mqttClient.clientId);
                        }
                        else {
                            NSLog(@"MQTT client: error connecting to broker --> %@", [error localizedDescription]);
                        }
    }];
}

// Subscribe client to topic
- (void)subscribe {

    NSString *topic = [self subscribeTopic];

    [self.mqttClient subscribeToTopic:topic
                              atLevel:MQTTQosLevelExactlyOnce
                     subscribeHandler:^(NSError *error, NSArray<NSNumber *> *gQoss) {

                         if (error != nil) {
                             NSLog(@"MQTT client: Error subscribing to topic: %@ --> %@", topic, [error localizedDescription]);
                             return;
                         }

                         dispatch_async(dispatch_get_main_queue(), ^{
                             [[NSNotificationCenter defaultCenter]
                              postNotificationName:@"yodiwoConnectedToCloudServiceNotification"
                              object:self
                              userInfo:nil];
                         });

                         NSLog(@"MQTT client: subscribed to topic: %@ (QoS ==> %@)", topic, gQoss[0]);
    }];
}

// Publish message
- (void)publishMessage:(NSString *)msg inTopic:(NSString *)topic {

    [self.mqttClient publishData:[msg dataUsingEncoding:NSUTF8StringEncoding]
                         onTopic:[self.publishTopic stringByAppendingString:topic]
                          retain:NO
                             qos:MQTTQosLevelExactlyOnce publishHandler:^(NSError *error) {

        if (error != nil) {
            NSLog(@"MQTT client: Error publishing message: %@ in topic:%@", msg, topic);
            return;
        }

        NSLog(@"===> MQTT client: Published message: %@ in topic:%@", msg, topic);
    }];
}

//***** Delegates

- (void)newMessage:(MQTTSession *)session data:(NSData *)data onTopic:(NSString *)topic qos:(MQTTQosLevel)qos retained:(BOOL)retained mid:(unsigned int)mid {

    // Strip MqttMsg and send encapsulated ApiMsg to NodeController
    JSONModelError *error;
    MqttMsg *mqttMsg = [[MqttMsg alloc] initWithData:data error:&error];

    NSLog(@"MQTT client: message received: topic: %@ %@", topic, mqttMsg.Payload);

    NSString *apiMsgName = [[topic componentsSeparatedByString:@"/"] lastObject];

    // Respond with empty message if this is a request of unsupported type
    if(mqttMsg.Flags == EnumMessageFlags_Request && [[[PlegmaApi apiMsgNames] allValues] containsObject:apiMsgName] == NO) {
        NSLog(@"Warning: Mqtt client received unknown request");

        [[NodeController sharedNodeController] handleApiMsg:nil withPayload:nil withSyncId:mqttMsg.SyncId];
        return;
    }

    [[NodeController sharedNodeController] handleApiMsg:apiMsgName
                                            withPayload:mqttMsg.Payload
                                             withSyncId:mqttMsg.SyncId];
}

- (void)handleEvent:(MQTTSession *)session event:(MQTTSessionEvent)eventCode error:(NSError *)error {
    switch (eventCode) {
        case MQTTSessionEventConnected:
        {
            NSLog(@"MQTT client: Connection established");

            self.hasDisconnected = NO;

            [self subscribe];

            break;
        }
        case MQTTSessionEventConnectionClosed:
        {

            NSLog(@"MQTT client: Connection closed");

            if (self.hasDisconnected == NO) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    [[NSNotificationCenter defaultCenter]
                     postNotificationName:@"yodiwoDisconnectedFromCloudServiceNotification"
                     object:nil
                     userInfo:nil];
                });

                self.hasDisconnected = YES;
            }

            [self connect];

            break;
        }
        case MQTTSessionEventConnectionClosedByBroker:
        {
            NSLog(@"MQTT client: Connection closed by broker");
            break;
        }
        case MQTTSessionEventConnectionError:
        {
            NSLog(@"MQTT client: Connection error");
            break;
        }
        case MQTTSessionEventConnectionRefused:
        {
            NSLog(@"MQTT client: Connection refused");
            break;
        }
        case MQTTSessionEventProtocolError:
        {
            NSLog(@"MQTT client: Protocol error");
            break;
        }
        default:
        {
            NSLog(@"MQTT client: Unknown event");
            break;
        }
    }
}


@end
