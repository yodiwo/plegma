//
//  YABPairingServerResponseKeys.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "YABPairingServerResponseKeys.h"

@interface YABPairingServerResponseKeys ()

@end


@implementation YABPairingServerResponseKeys

-(instancetype)initWithNodeKey:(NSString *)nodeKey SecretKey:(NSString *)secretKey {
    self = [super init];
    if(self)
    {
        _nodeKey = nodeKey;
        _secretKey = secretKey;
    }
    return self;
}

-(instancetype)init {
    return [self initWithNodeKey:nil SecretKey:nil];
}

+(instancetype)pairingServerResponseKeysWithNodeKey:(NSString *)nodeKey
                                          SecretKey:(NSString *)secretKey {
    return [[self alloc] initWithNodeKey:nodeKey SecretKey:secretKey];
}

@end
