//
//  Keys.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/2/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "Keys.h"

@implementation UserKey

-(instancetype)initWithUserId:(NSString *)userID {
    if (self = [super init]) {
        if (userID == nil) {
            return nil;
        }

        _userID = userID;
    }

    return self;
}

-(NSString *)toString {
    return self.userID;
}

@end

//******************************************************************************

@implementation NodeKey

-(instancetype)initWithUserKey:(UserKey *)userKey
                   andNodeId:(NSString *)nodeID {
    self = [super init];
    if(self)
    {
        _userKey = userKey;
        _nodeID = [nodeID integerValue];
    }
    return self;
}

-(instancetype)initWithNodeKeyString:(NSString *)nodeKey {
    self = [super init];
    if(self)
    {
        if (nodeKey == nil || [nodeKey isEqualToString:@""]) {
            return nil;
        }

        NSArray *items = [nodeKey componentsSeparatedByString:@"-"];
        if ([items count] != 2) {
            NSLog(@"NodeKey: Invalid NodeKey string");
            return nil;
        }

        _userKey = [[UserKey alloc] init];
        _userKey.userID = items[0];
        _nodeID = [items[1] integerValue];
    }

    return self;
}

-(NSString *)toString {
    return [[[self.userKey toString] stringByAppendingString:@"-" ]
                stringByAppendingString:[NSString stringWithFormat:@"%ld", (long)self.nodeID]];
}

@end

//******************************************************************************

@implementation ThingKey

-(instancetype)initWithNodeKey:(NodeKey *)nodeKey
                   andThingUid:(NSString *)thingUID {
    self = [super init];
    if(self)
    {
        _nodeKey = nodeKey;
        _thingUID = thingUID;
    }
    return self;
}

-(instancetype)initFromString:(NSString *)thingKey {
    self = [super init];
    if(self)
    {
        if (thingKey == nil || [thingKey isEqualToString:@""]) {
            return nil;
        }

        NSArray *items = [thingKey componentsSeparatedByString:@"-"];
        if ([items count] != 3) {
            NSLog(@"ThingKey: Invalid ThingKey string");
            return nil;
        }



        _nodeKey = [[NodeKey alloc] initWithNodeKeyString:
                                    [[items[0] stringByAppendingString:@"-"]
                                                stringByAppendingString:items[1]]];
        _thingUID = items[2];
    }

    return self;
}

+(NSString *)createKeyFromNodeKey:(NSString *)nodeKey
                        thingName:(NSString *)thingName {
    return [[nodeKey stringByAppendingString:@"-"]
                stringByAppendingString:thingName];
}

-(NSString *)toString {
    return [[[self.nodeKey toString] stringByAppendingString:@"-"]
                stringByAppendingString:self.thingUID];
}

@end

//******************************************************************************

@implementation PortKey

-(instancetype)initWithThingKey:(ThingKey *)thingKey
                   andPortUid:(NSString *)portUID {
    self = [super init];
    if(self)
    {
        _thingKey = thingKey;
        _portUID = portUID;
    }
    return self;
}

-(instancetype)initFromString:(NSString *)portKey {
    self = [super init];
    if(self)
    {
        if (portKey == nil || [portKey isEqualToString:@""]) {
            return nil;
        }

        NSArray *items = [portKey componentsSeparatedByString:@"-"];
        if ([items count] != 4) {
            NSLog(@"portKey: Invalid portKey string");
            return nil;
        }
        _thingKey = [[ThingKey alloc] initFromString:
                        [[[items[0] stringByAppendingString:@"-"]
                          stringByAppendingString:[items[1]
                            stringByAppendingString:@"-"]]
                                stringByAppendingString:items[2]] ];
        _portUID = items[3];
    }

    return self;
}

-(NSString *)toString {
    return [[[self.thingKey toString] stringByAppendingString:@"-"]
            stringByAppendingString:self.portUID];
}


@end