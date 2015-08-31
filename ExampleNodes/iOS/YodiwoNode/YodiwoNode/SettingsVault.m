//
//  SettingsVault.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "SettingsVault.h"

@interface SettingsVault ()

@property (strong, nonatomic) NSUserDefaults *settings;

@end


@implementation SettingsVault

///***** Singleton pattern for settings vault

+(id)sharedSettingsVault {
    static SettingsVault *internalSharedSettingsVault = nil;
    static dispatch_once_t once_token;

    dispatch_once(&once_token, ^{
        internalSharedSettingsVault =[[self alloc] init];
    });

    return internalSharedSettingsVault;
}

-(instancetype)init {
    if (self = [super init]) {
        _settings = [NSUserDefaults standardUserDefaults];
    }

    return self;
}
///*****************************************************************************






///***** Public methods
-(NSString *)getConnectionParamsServerAddress {

    return [self.settings stringForKey:@"ConnectionParamsServerAddress"];
}

-(NSString *)getPairingParamsNodeUuid {

    return [self.settings stringForKey:@"PairingParamsNodeUuid"];
}

-(NSString *)getPairingParamsNodeName {

    return [self.settings stringForKey:@"PairingParamsNodeName"];
}

-(BOOL)getConnectionParamsSecureConnection {

    return [self.settings boolForKey:@"ConnectionParamsSecureConnection"];;
}

-(NSString *)getPairingToken1 {

    return [self.settings stringForKey:@"PairingToken1"];
}

-(NSString *)getPairingNodeKey {

    return [self.settings stringForKey:@"NodeKey"];
}

-(NSString *)getPairingSecretKey {

    return [self.settings stringForKey:@"SecretKey"];
}

-(NSString *)getUserKey {
    NSString *nkey = [self getPairingNodeKey];
    if(nkey == nil) {
        NSLog(@"getUserKey(): NodeKey not found");
        return nil;
    }

    NSArray *items = [nkey componentsSeparatedByString:@"-"];
    if ([items count] != 2) {
        NSLog(@"getUserKey(): Invalid NodeKey");
        return nil;
    }

    return [items firstObject];
}

-(void)setPairingKeysWithNodeKey:(NSString *)nodeKey secretKey:(NSString *)secretKey {

    [self.settings setObject:nodeKey forKey:@"NodeKey"];
    [self.settings setObject:secretKey forKey:@"SecretKey"];
}

-(void)setPairingToken1:(NSString *)token1 {

    [self.settings setObject:token1 forKey:@"PairingToken1"];
}

-(void)setNodePaired:(BOOL)status {

    [self.settings setBool:status forKey:@"NodePairStatus"];
}

-(BOOL)isNodePaired {
    
    return [self.settings boolForKey:@"NodePairStatus"];
}

-(void)setIsNodePaired:(BOOL)state {

    return [self.settings setBool:state forKey:@"NodePairStatus"];
}

-(NSString *)getMqttParamsBrokerAddress {
    
    return [self.settings stringForKey:@"MqttParamsBrokerAddress"];
}

-(NSInteger)getMqttParamsPort {

    if ([self.settings objectForKey:@"MqttParamsPort"] == nil) {
        [self.settings setInteger:1883 forKey:@"MqttParamsPort"];
    }

    return [self.settings integerForKey:@"MqttParamsPort"];
}

-(BOOL)getMqttParamsSecureConnection {

    if ([self.settings objectForKey:@"ParamsSecureConnection"] == nil) {
        [self.settings setBool:NO forKey:@"ParamsSecureConnection"];
    }

    return [self.settings boolForKey:@"ParamsSecureConnection"];
}

-(NSString *)getIBeaconParamsMonitoredUUID1 {

    return [self.settings stringForKey:@"IBeaconParamsMonitoredUUID1"];
}

-(NSString *)getIBeaconParamsMonitoredUUID2 {

    return [self.settings stringForKey:@"IBeaconParamsMonitoredUUID2"];
}
///*****************************************************************************

@end
