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

@end


@implementation YABPairingNodeGetKeysRequest

-(instancetype)initWithUuid:(NSString *)uuid
                     Token1:(NSString *)token1 {
    self = [super init];
    if(self)
    {
        _uuid = uuid;
        _token1 = token1;
    }
    return self;
}

-(instancetype)init {
    return [self initWithUuid:nil Token1:nil];
}

+(instancetype)pairingNodeGetKeysRequestWithUuid:(NSString *)uuid
                                          Token1:(NSString *)token1 {
    return [[self alloc] initWithUuid:uuid Token1:token1];
}

@end
