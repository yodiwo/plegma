//
//  YABPairingNodeGetKeysRequest.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "YABPairingNodeGetKeysRequest.h"

@interface YABPairingNodeGetKeysRequest ()

@property (strong, nonatomic) NSString *uuid;
@property (strong, nonatomic) NSString *token1;
@property (strong, nonatomic) NSString *token2;

@end


@implementation YABPairingNodeGetKeysRequest

-(instancetype)initWithUuid:(NSString *)uuid
                     Token1:(NSString *)token1
                     Token2:(NSString *)token2 {
    self = [super init];
    if(self)
    {
        _uuid = uuid;
        _token1 = token1;
        _token2 = token2;
    }
    return self;
}

-(instancetype)init {
    return [self initWithUuid:nil Token1:nil Token2:nil];
}

+(instancetype)pairingNodeGetKeysRequestWithUuid:(NSString *)uuid
                                          Token1:(NSString *)token1
                                          Token2:(NSString *)token2 {
    return [[self alloc] initWithUuid:uuid Token1:token1 Token2:token2];
}

@end
