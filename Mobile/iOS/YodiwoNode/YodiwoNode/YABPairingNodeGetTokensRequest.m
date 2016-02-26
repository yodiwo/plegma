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
@property (strong, nonatomic) NSString *redirectUri;

@end


@implementation YABPairingNodeGetTokensRequest

-(instancetype)initWithUuid:(NSString *)uuid
                       Name:(NSString *)name
                RedirectUri:(NSString *)redirectUri {
    self = [super init];
    if(self)
    {
        _uuid = uuid;
        _name = name;
        _redirectUri = redirectUri;
    }
    return self;
}

-(instancetype)init {
    return [self initWithUuid:nil Name:nil RedirectUri:nil];
}

+(instancetype)pairingNodeGetTokensRequestWithUuid:(NSString *)uuid
                                              Name:(NSString *)name
                                       RedirectUri:(NSString *)redirectUri {
    return [[self alloc] initWithUuid:uuid Name:name RedirectUri:redirectUri];
}

@end
