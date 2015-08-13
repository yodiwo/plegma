//
//  YABPairingNodeResponsePhase1.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "YABPairingNodeResponsePhase1.h"

@interface YABPairingNodeResponsePhase1 ()

@property (strong, nonatomic) NSString *userNodeRegistrationUrl;
@property (strong, nonatomic) NSString *token2;

@end


@implementation YABPairingNodeResponsePhase1

-(instancetype)initWithUserNodeRegistrationUrl:(NSString *)userNodeRegistrationUrl
                                        Token2:(NSString *)token2 {
    self = [super init];
    if(self)
    {
        _userNodeRegistrationUrl = userNodeRegistrationUrl;
        _token2 = token2;
    }
    return self;
}

-(instancetype)init {
    return [self initWithUserNodeRegistrationUrl:nil Token2:nil];
}

+(instancetype)pairingNodeResponsePhase1WithUserNodeRegistrationUrl:(NSString *)userNodeRegistrationUrl
                                                               Token2:(NSString *)token2 {
    return [[self alloc] initWithUserNodeRegistrationUrl:userNodeRegistrationUrl
                                                  Token2:token2];
}

@end
