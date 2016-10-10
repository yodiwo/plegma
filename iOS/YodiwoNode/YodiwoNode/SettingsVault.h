//
//  SettingsVault.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SettingsVault : NSObject

+(id)sharedSettingsVault;

-(NSString *)getConnectionParamsServerAddress;

-(NSString *)getPairingParamsNodeUuid;

-(NSString *)getPairingParamsNodeName;

-(BOOL)getConnectionParamsSecureConnection;

-(NSString *)getPairingToken1;

-(NSString *)getPairingToken2;

-(NSString *)getPairingNodeKey;

-(NSString *)getPairingSecretKey;

-(BOOL)isNodePaired;

-(void)setIsNodePaired:(BOOL)state;

-(NSString *)getMqttParamsBrokerAddress;

-(NSInteger)getMqttParamsPort;

-(BOOL)getMqttParamsSecureConnection;

-(NSString *)getUserKey;

-(BOOL)isNodeApiConnected;

-(void)setIsNodeApiConnected:(BOOL)state;

-(void)setPairingKeysWithNodeKey:(NSString *)nodeKey secretKey:(NSString *)secretKey;

-(void)setPairingToken1:(NSString *)token1;

-(void)setPairingToken2:(NSString *)token2;

-(void)setNodePaired:(BOOL)status;

-(NSInteger)getNodeThingsRevNum;

-(void)setNodeThingsRevNum:(NSInteger)nodeThingsRevNum;

@end
