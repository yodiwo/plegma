//
//  YABPairingServerResponseTokens.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "YABPairingServerResponseTokens.h"

@interface YABPairingServerResponseTokens ()

@end


@implementation YABPairingServerResponseTokens

-(instancetype)initWithToken1:(NSString *)token1
                       Token2:(NSString *)token2 {
    self = [super init];
    if(self)
    {
        _token1 = token1;
        _token2 = token2;
    }
    return self;
}

-(instancetype)init {
    return [self initWithToken1:nil Token2:nil];
}

+(instancetype)pairingServerResponseTokensWithToken1:(NSString *)token1
                                              Token2:(NSString *)token2 {
    return [[self alloc] initWithToken1:token1 Token2:token2];
}

@end
