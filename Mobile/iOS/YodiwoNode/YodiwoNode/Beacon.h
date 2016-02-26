//
//  Beacon.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/9/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

@interface Beacon : NSObject

@property (strong, nonatomic) NSString *friendlyName;
@property (strong, nonatomic) NSString *shortDescription;
@property (nonatomic) CLProximity lastProximity;
@property (strong, nonatomic) CLBeaconRegion *ibeacon;

- (instancetype)initWithFriendlyName:(NSString *)friendlyName
                    shortDescription:(NSString *)shortDescription
                   iBeaconIdentifier:(NSString *)identifier
                         iBeaconUUID:(NSString *)uuid
                        iBeaconMajor:(CLBeaconMajorValue)major
                        iBeaconMinor:(CLBeaconMinorValue)minor;

- (BOOL)isEqualToCLBeacon:(CLBeacon *)beacon;

@end
