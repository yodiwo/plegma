//
//  LocationManagerModule.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/9/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

@interface LocationManagerModule : NSObject<CLLocationManagerDelegate>

- (void)start;

- (void)startRegioningBeacons;

- (void)startRangingBeacons;

- (void)stopRegioningBeacons;

- (void)stopRangingBeacons;

@end
