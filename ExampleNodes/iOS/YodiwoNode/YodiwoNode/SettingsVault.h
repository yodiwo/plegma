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

-(NSString *)getPairingNodeKey;

-(NSString *)getPairingSecretKey;

-(BOOL)isNodePaired;

-(NSString *)getMqttParamsBrokerAddress;

-(NSInteger)getMqttParamsPort;

-(BOOL)getMqttParamsSecureConnection;

-(NSString *)getUserKey;


-(void)setPairingKeysWithNodeKey:(NSString *)nodeKey secretKey:(NSString *)secretKey;

-(void)setPairingToken1:(NSString *)token1;

-(void)setNodePaired:(BOOL)status;

@end
