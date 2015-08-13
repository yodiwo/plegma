//
//  MqttClientService.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/30/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "MqttClient.h"
#import "MQTTKit.h"
#import "SettingsVault.h"
#import "NodeController.h"

#define kMqttTopic_publish @"/api/in/"
#define kMqttTopic_subscribe @"/api/out/"


@interface MqttClient ()

@property (strong, nonatomic) MQTTClient *mqttClient;
@property (strong, nonatomic) NSString *subscribeTopic;
@property (strong, nonatomic) NSString *publishTopic;

@end


@implementation MqttClient

-(instancetype)init {
    if (self = [super init]) {
        _mqttClient = [[MQTTClient alloc] initWithClientId:[[SettingsVault sharedSettingsVault] getPairingNodeKey] cleanSession:NO];

        _subscribeTopic = [[kMqttTopic_subscribe
                           stringByAppendingString:[[SettingsVault sharedSettingsVault] getPairingNodeKey]]
                                                    stringByAppendingString:@"/#"];

        _publishTopic = [[[[kMqttTopic_publish
                                stringByAppendingString:[[SettingsVault sharedSettingsVault] getUserKey]]
                                    stringByAppendingString:@"/"]
                                        stringByAppendingString:[[SettingsVault sharedSettingsVault] getPairingNodeKey] ]
                                            stringByAppendingString:@"/"];
    }

    return self;
}

// Start MQTT client
- (void)start
{
    self.mqttClient.username = [[SettingsVault sharedSettingsVault] getPairingNodeKey];
    self.mqttClient.password = [[SettingsVault sharedSettingsVault] getPairingSecretKey];
    self.mqttClient.host = [[SettingsVault sharedSettingsVault] getMqttParamsBrokerAddress];

    [self setMessageHandler];
    [self connect];
    [self subscribe];
}

// Disconnect client from MQTT broker
- (void)disconnect{

    [self.mqttClient disconnectWithCompletionHandler:^(NSUInteger code) {
        NSLog(@"MQTT client: disconnected from broker");
    }];
}

// Connect client to MQTT broker
- (void)connect {

    [self.mqttClient connectToHost:[[SettingsVault sharedSettingsVault] getMqttParamsBrokerAddress]
                 completionHandler:^(MQTTConnectionReturnCode code) {
        if (code == ConnectionAccepted) {
            NSLog(@"MQTT client: connected with id: %@", self.mqttClient.clientID);
        }
        else {
            NSLog(@"MQTT client: error connecting to broker: %lu", code);
        }
    }];
}

// Subscribe client to topic
- (void)subscribe {

    [self.mqttClient subscribe:[self subscribeTopic]
                       withQos:ExactlyOnce
             completionHandler:^(NSArray *grantedQos){

                 NSLog(@"MQTT client: subscribed to topic: %@ (QoS ==> %@)",
                       [self subscribeTopic], grantedQos[0]);

                 dispatch_async(dispatch_get_main_queue(), ^{
                     [[NSNotificationCenter defaultCenter]
                        postNotificationName:@"yodiwoConnectedToCloudServiceNotification"
                                      object:self
                                    userInfo:nil];
                 });
    }];
}

- (void)setMessageHandler {

    [self.mqttClient setMessageHandler:^(MQTTMessage *message) {
        NSLog(@"MQTT client: message received: %@", [message payloadString]);
        [[NodeController sharedNodeController] handleMqttApiMsgWithPayload:message.payloadString
                                                                   atTopic:message.topic retained:message.retained];
    }];
}

// Publish message
- (void)publishMessage:(NSString *)msg inTopic:(NSString *)topic {

    [self.mqttClient publishString:msg
                       toTopic:[[self publishTopic] stringByAppendingString:topic]
                       withQos:ExactlyOnce
                        retain:NO
             completionHandler:^(int mid){

                 NSLog(@"Published message id: %d", mid);
    }];
}

@end
