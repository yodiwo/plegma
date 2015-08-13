//
//  NodePairingGetTokensRequest.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/24/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "YABPairingNodeGetTokensRequest.h"

@interface YABPairingNodeGetTokensRequest ()

@property (strong, nonatomic) NSString *uuid;
@property (strong, nonatomic) NSString *name;

@end


@implementation YABPairingNodeGetTokensRequest

-(instancetype)initWithUuid:(NSString *)uuid
                       Name:(NSString *)name {
    self = [super init];
    if(self)
    {
        _uuid = uuid;
        _name = name;
    }
    return self;
}

-(instancetype)init {
    return [self initWithUuid:nil Name:nil];
}

+(instancetype)pairingNodeGetTokensRequestWithUuid:(NSString *)uuid
                                              Name:(NSString *)name {
    return [[self alloc] initWithUuid:uuid Name:name];
}

@end
