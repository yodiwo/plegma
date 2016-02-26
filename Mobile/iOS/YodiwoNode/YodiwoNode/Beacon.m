//
//  Beacon.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/9/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "Beacon.h"


@interface Beacon ()

@end


@implementation Beacon

- (instancetype)initWithFriendlyName:(NSString *)friendlyName
            shortDescription:(NSString *)shortDescription
                   iBeaconIdentifier:(NSString *)identifier
                     iBeaconUUID:(NSString *)uuid
                iBeaconMajor:(CLBeaconMajorValue)major
                iBeaconMinor:(CLBeaconMinorValue)minor {

    if (self = [super init]) {
        _friendlyName = friendlyName;
        _shortDescription = shortDescription;

        NSUUID *nsuuid = [[NSUUID alloc] initWithUUIDString:uuid];
        _ibeacon = [[CLBeaconRegion alloc] initWithProximityUUID:nsuuid
                                                           major:major
                                                           minor:minor
                                                      identifier:identifier];

        _lastProximity = CLProximityUnknown;
    }

    return self;
}

- (BOOL)isEqualToCLBeacon:(CLBeacon *)beacon {
    if ([beacon.proximityUUID.UUIDString isEqualToString:self.ibeacon.proximityUUID.UUIDString] &&
        [beacon.major isEqual: self.ibeacon.major] &&
        [beacon.minor isEqual: self.ibeacon.minor])
    {
        return YES;
    } else {
        return NO;
    }
}

@end
