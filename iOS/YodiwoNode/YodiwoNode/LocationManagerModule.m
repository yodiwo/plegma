//
//  LocationManagerModule.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/9/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "LocationManagerModule.h"
#import "NodeThingsRegistry.h"
#import "NodeController.h"
#import "SettingsVault.h"

#import <UIKit/UIKit.h>



@interface LocationManagerModule ()

@property (strong, nonatomic) CLLocationManager *locationManager;

@property (strong, nonatomic) NSMutableSet *beaconsToRange; // of CLBeacon

@property (strong, nonatomic) NSMutableDictionary *rangedBeaconsDict; // of <uuid, CLBeacon>

@property (strong, nonatomic) NSMutableSet *beaconRegionsToMonitor; // of CLBeaconRegion

@property (strong, nonatomic) NSMutableDictionary *regionedBeaconsDict; // of <uuid, CLBeaconRegion>


@end


@implementation LocationManagerModule

///***** Override synthesized getters, lazy instantiate

-(CLLocationManager *)locationManager {
    if(_locationManager == nil) {
        if ([CLLocationManager authorizationStatus] == kCLAuthorizationStatusDenied ||
            [CLLocationManager authorizationStatus] == kCLAuthorizationStatusRestricted )
        {
            NSLog(@"ERROR: Location services are restricted");
            return nil;
        }

        _locationManager = [[CLLocationManager alloc] init];

        if ([_locationManager respondsToSelector:@selector(requestAlwaysAuthorization)]) {
            [_locationManager requestAlwaysAuthorization];
        }

        [_locationManager setDelegate:self];
    }

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(yodiwoThingUpdateNotification:)
                                                 name:@"yodiwoThingUpdateNotification"
                                               object:nil];
    
    return _locationManager;
}

-(NSMutableDictionary *)rangedBeaconsDict {
    if (_rangedBeaconsDict == nil) {
        _rangedBeaconsDict = [[NSMutableDictionary alloc] init];
    }

    return _rangedBeaconsDict;
}

-(NSMutableDictionary *)regionedBeaconsDict {
    if (_regionedBeaconsDict == nil) {
        _regionedBeaconsDict = [[NSMutableDictionary alloc] init];
    }

    return _regionedBeaconsDict;
}

-(NSMutableSet *)beaconsToRange {
    if (_beaconsToRange == nil) {
        _beaconsToRange = [[NSMutableSet alloc] init];
    }

    return _beaconsToRange;
}

-(NSMutableSet *)beaconRegionsToMonitor {
    if (_beaconRegionsToMonitor == nil) {
        _beaconRegionsToMonitor = [[NSMutableSet alloc] init];
    }

    return _beaconRegionsToMonitor;
}


//******************************************************************************


///***** Public api

- (void)start {

    // GPS
    self.locationManager.distanceFilter = 100.0f; // accuracy in meters
    [self.locationManager startUpdatingLocation];

    // Proximity sensor
    // Enabled monitoring of the sensor
    [[UIDevice currentDevice] setProximityMonitoringEnabled:YES];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(notifyProximitySensorStateChange:)
                                                 name:@"UIDeviceProximityStateDidChangeNotification" object:nil];

    // iBeacon ranging
    [self startRangingBeacons];
}

- (void)startRegioningBeacons {
    for (CLBeaconRegion *br in self.beaconRegionsToMonitor) {
        [self.locationManager startMonitoringForRegion:br];
    }
}

- (void)stopRegioningBeacons {
    for (CLBeaconRegion *br in self.beaconRegionsToMonitor) {
        [self.locationManager stopMonitoringForRegion:br];
    }
}

- (void)startRangingBeacons {
    for (NSString *uuid in self.beaconsToRange) {
        CLBeaconRegion *br = [[CLBeaconRegion alloc] initWithProximityUUID:[[NSUUID alloc] initWithUUIDString:uuid]
                                                                identifier:uuid];
        [self.locationManager startRangingBeaconsInRegion:br];
    }
}

-(void)stopRangingBeacons {
    for (NSString *uuid in self.beaconsToRange) {
        CLBeaconRegion *br = [[CLBeaconRegion alloc] initWithProximityUUID:[[NSUUID alloc] initWithUUIDString:uuid]
                                                                identifier:uuid];
        [self.locationManager stopRangingBeaconsInRegion:br];
    }
}

//******************************************************************************

///***** Delegates

- (void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray *)locations {

    CLLocationCoordinate2D coord = [[locations objectAtIndex:0] coordinate];
    CLLocation *location = [[CLLocation alloc] initWithLatitude:coord.latitude
                                                      longitude:coord.longitude];

    CLGeocoder *geocoder = [[CLGeocoder alloc] init];
    [geocoder reverseGeocodeLocation:location
                   completionHandler:^(NSArray *placemarks, NSError *error) {
                       if (error){
                           NSLog(@"ERROR: Reverse geocoding failed with error: %@", error);
                           return;
                       }

                       CLPlacemark *currentPlacemark = (CLPlacemark *)[placemarks objectAtIndex:0];

                       NSString *newLocationFriendly = [NSString stringWithFormat: @"%@, %@, %@",
                                                        currentPlacemark.thoroughfare,
                                                        currentPlacemark.locality,
                                                        currentPlacemark.ISOcountryCode]; // Port 0 update

                       NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
                       [numberFormatter setUsesSignificantDigits: YES];
                       numberFormatter.maximumSignificantDigits = 100;
                       [numberFormatter setGroupingSeparator:@""];
                       [numberFormatter setNumberStyle:NSNumberFormatterDecimalStyle];

                       NSString *newLatitude =
                       [numberFormatter stringFromNumber:@(coord.latitude)]; // Port 1 update
                       NSString *newLongitude =
                       [numberFormatter stringFromNumber:@(coord.longitude)]; // Port 2 update

                       // Notify cloud service
                       NSArray *data = [NSArray arrayWithObjects:newLocationFriendly,
                                        newLatitude,
                                        newLongitude,
                                        nil];
                       [[NodeController sharedNodeController]
                        sendPortEventMsgFromThing:ThingNameLocationGPS withData:data];

                       // Notify interested UIViewController
                       NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                                  ThingNameLocationGPS, @"thingName",
                                                  location, @"location",
                                                  newLocationFriendly, @"locationFriendly",
                                                  nil];
                       [[NSNotificationCenter defaultCenter] postNotificationName:@"yodiwoUIUpdateNotification"
                                                                           object:self
                                                                         userInfo:notParams];
                   }];
}

- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error {
    NSLog(@"Location manager failed: %@", error);
}


-(void)locationManager:(CLLocationManager *)manager
       didRangeBeacons:(NSArray *)beacons
              inRegion:(CLBeaconRegion *)region {

    NSString *message = @"";

    if(beacons.count > 0) {

        // Clear ranged beacons dictionary
        [self.rangedBeaconsDict removeAllObjects];

        // Loop through all ranged beacons
        for (CLBeacon *rangedB in beacons) {

            // Check if beacon exists in our monitored beacons
            if ([self.beaconsToRange containsObject:rangedB.proximityUUID.UUIDString] == NO) {
                return;
            }

            if (rangedB.proximity == CLProximityUnknown) {
                return;
            }

            // Set beacon in ranged beacons dictionary
            [self.rangedBeaconsDict setObject:rangedB forKey:rangedB.proximityUUID.UUIDString];

            switch(rangedB.proximity) {
                case CLProximityFar:
                    message = @"Far";
                    break;
                case CLProximityNear:
                    message = @"Near";
                    break;
                case CLProximityImmediate:
                    message = @"Immediate";
                    break;
                case CLProximityUnknown:
                    message = @"Unknown";
                    break;
            }

            // Notify cloud service
            NSString *beaconUUID = rangedB.proximityUUID.UUIDString;
            NSString *beaconMajor = [rangedB.major stringValue];
            NSString *beaconMinor = [rangedB.minor stringValue];
            NSString *proximity = message;
            NSString *rssi = [NSString stringWithFormat:@"%ld",
                              (long)rangedB.rssi];

            NSArray *data = [NSArray arrayWithObjects:
                             beaconUUID,  // Port 0 update
                             beaconMajor, // Port 1 update
                             beaconMinor, // Port 2 update
                             proximity,   // Port 3 update
                             rssi,        // Port 4 update
                             nil];
            [[NodeController sharedNodeController] sendPortEventMsgFromThing:ThingNameLocationBeacon
                                                                    withData:data];
            // Send local notification
            NSString *localNotificationMsg = [[[@"Beacon: " stringByAppendingString:beaconUUID]
                                               stringByAppendingString:@", Proximity: "]
                                              stringByAppendingString:message];
            [self sendLocalNotificationWithMessage:localNotificationMsg];

            // Debug log
            NSLog(@"%@", localNotificationMsg);

            // Notify interested UIViewController
            NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                       ThingNameLocationBeacon, @"thingName",
                                       beaconUUID, @"uuid",
                                       beaconMajor, @"major",
                                       beaconMinor, @"minor",
                                       rssi, @"rssi",
                                       proximity, @"proximity", nil];
            [[NSNotificationCenter defaultCenter] postNotificationName:@"yodiwoUIUpdateNotification"
                                                                object:self
                                                              userInfo:notParams];
        }
    }
}

-(void)locationManager:(CLLocationManager *)manager
        didEnterRegion:(CLRegion *)region {
    [manager startRangingBeaconsInRegion:(CLBeaconRegion*)region];
    [self.locationManager startUpdatingLocation];

    NSLog(@"You entered the region.");
    [self sendLocalNotificationWithMessage:@"You entered the region."];
}

-(void)locationManager:(CLLocationManager *)manager
         didExitRegion:(CLRegion *)region {
    [manager stopRangingBeaconsInRegion:(CLBeaconRegion*)region];
    [self.locationManager stopUpdatingLocation];

    NSLog(@"You exited the region.");
    [self sendLocalNotificationWithMessage:@"You exited the region."];
}

- (void)locationManager:(CLLocationManager *)manager monitoringDidFailForRegion:(CLRegion *)region withError:(NSError *)error {
    NSLog(@"Failed monitoring region: %@", error);
}
//******************************************************************************





///***** Observers

- (void)notifyProximitySensorStateChange:(NSNotificationCenter *)notification {
    
    NSString *proximityState = ([[UIDevice currentDevice] proximityState] == YES) ? @"true" : @"false";

    // Notify cloud service
    NSArray *data = [NSArray arrayWithObjects:proximityState,nil];
    [[NodeController sharedNodeController] sendPortEventMsgFromThing:ThingNameLocationProximity withData:data];
}

- (void)yodiwoThingUpdateNotification:(NSNotification *)notification {

    // Get notification parameters
    NSDictionary *notParams = [notification userInfo];

    Thing *thing = [notParams objectForKey:@"thing"];
    if (thing == nil) {
        return;
    }

    if ([[[ThingKey alloc] initFromString:thing.ThingKey].thingUID isEqualToString:ThingNameLocationBeacon]) {
        for (ConfigParameter *cp in thing.Config) {
            if ([cp.Name isEqualToString:@"RangedUUIDList"]) {

                // Clear set
                [self.beaconsToRange removeAllObjects];

                NSArray *uuidsToRange = [cp.Value componentsSeparatedByString:@","];

                // If empty stop ranging
                if ([uuidsToRange count] == 0) {
                    [self stopRangingBeacons];
                    return;
                }

                for (NSString *uuid in uuidsToRange) {

                    // Add in set
                    if ([self isBeaconUuidValid:uuid]) {
                        [self.beaconsToRange addObject:uuid];
                    }
                }

                [self startRangingBeacons];
            }
        }
    }
}
//******************************************************************************





///***** Helpers


- (BOOL)isBeaconNameValid:(NSString *)name {
    return (name.length > 0) ? YES : NO;
}

- (BOOL)isBeaconUuidValid:(NSString *)uuid {
    NSString *uuidPatternString = @"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$";
    NSRegularExpression *uuidRegex = [NSRegularExpression regularExpressionWithPattern:uuidPatternString
                                                                               options:NSRegularExpressionCaseInsensitive
                                                                                 error:nil];
    NSInteger numberOfMatches = [uuidRegex numberOfMatchesInString:uuid
                                                           options:kNilOptions
                                                             range:NSMakeRange(0, uuid.length)];
    return (numberOfMatches > 0) ? YES : NO;
}

-(void)sendLocalNotificationWithMessage:(NSString*)message {
    UILocalNotification *notification = [[UILocalNotification alloc] init];
    notification.alertBody = message;
    [[UIApplication sharedApplication] scheduleLocalNotification:notification];
}
//******************************************************************************

@end
