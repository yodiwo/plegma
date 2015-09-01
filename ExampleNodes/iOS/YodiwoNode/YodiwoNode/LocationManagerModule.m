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
#import "Beacon.h"

#import <UIKit/UIKit.h>



@interface LocationManagerModule ()

@property (strong, nonatomic) CLLocationManager *locationManager;

@property (strong, nonatomic) NSMutableDictionary *beaconsDict; // of <uuid, Beacon>

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

        [_locationManager setDelegate:self];
    }

    return _locationManager;
}

-(NSMutableDictionary *)beaconsDict {
    if (_beaconsDict == nil) {
        _beaconsDict = [[NSMutableDictionary alloc] init];
    }

    return _beaconsDict;
}
//******************************************************************************




///***** Public api

- (void)start {

    if ([self.locationManager respondsToSelector:@selector(requestWhenInUseAuthorization)]) {
        [self.locationManager requestWhenInUseAuthorization];
    }

    // GPS
    self.locationManager.distanceFilter = 100.0f; // 100m accuracy
    [self.locationManager startUpdatingLocation];
    [self.locationManager startMonitoringSignificantLocationChanges];

    // iBeacon
    Beacon *b1 = [[Beacon alloc] initWithFriendlyName:@"MonitoredUUID1"
                                    shortDescription:@"Monitored beacon 1"
                                   iBeaconIdentifier:@"MonitoredUUID1"
                                         iBeaconUUID:[[SettingsVault sharedSettingsVault] getIBeaconParamsMonitoredUUID1]
                                        iBeaconMajor:0
                                        iBeaconMinor:0];

    if([self isBeaconNameValid:b1.ibeacon.identifier] &&
       [self isBeaconUuidValid:b1.ibeacon.proximityUUID.UUIDString]) {
        [self.beaconsDict setObject:b1 forKey:b1.ibeacon.proximityUUID.UUIDString];
    }

    Beacon *b2 = [[Beacon alloc] initWithFriendlyName:@"MonitoredUUID2"
                                     shortDescription:@"Monitored beacon 2"
                                    iBeaconIdentifier:@"MonitoredUUID2"
                                          iBeaconUUID:[[SettingsVault sharedSettingsVault] getIBeaconParamsMonitoredUUID2]
                                         iBeaconMajor:0
                                         iBeaconMinor:0];

    if([self isBeaconNameValid:b2.ibeacon.identifier] &&
       [self isBeaconUuidValid:b2.ibeacon.proximityUUID.UUIDString]) {
        [self.beaconsDict setObject:b2 forKey:b2.ibeacon.proximityUUID.UUIDString];
    }

    for (Beacon *b in [self.beaconsDict allValues]) {
        [self.locationManager startMonitoringForRegion:b.ibeacon];
        [self.locationManager startRangingBeaconsInRegion:b.ibeacon];
    }

    // Proximity sensor
    // Enabled monitoring of the sensor
    [[UIDevice currentDevice] setProximityMonitoringEnabled:YES];

    // Set up an observer for proximity changes
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(notifyProximitySensorStateChange:)
                                                 name:@"UIDeviceProximityStateDidChangeNotification" object:nil];
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

                       [self.locationManager stopUpdatingLocation]; // Only to get initial location
                   }];
}

- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error {
    NSLog(@"Location manager failed: %@", error);
}

-(void)locationManager:(CLLocationManager *)manager
       didRangeBeacons:(NSArray *)beacons
              inRegion:(CLBeaconRegion *)region {

    NSString *message = @"";

    // TODO: Add this as thing's ConfigParameter and factor out common code
    NSString *monitoringMode = @"all";

    if(beacons.count > 0) {
        if ([monitoringMode  isEqual: @"nearest"]) {
            CLBeacon *nearestBeacon = beacons.firstObject;

            // Check if beacon exists in our monitored beacons
            Beacon *b = [self.beaconsDict objectForKey:nearestBeacon.proximityUUID.UUIDString];
            if (b == nil) {
                return;
            }

            // Check if proximity to this beacon has changed since last notification
            if(nearestBeacon.proximity == b.lastProximity) {
                return;
            }

            // Update proximity level of this beacon
            b.lastProximity = nearestBeacon.proximity;
            [self.beaconsDict setObject:b forKey:b.ibeacon.proximityUUID.UUIDString];

            switch(nearestBeacon.proximity) {
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
            NSString *beaconUUID = nearestBeacon.proximityUUID.UUIDString;
            NSString *beaconMajor = [nearestBeacon.major stringValue];
            NSString *beaconMinor = [nearestBeacon.minor stringValue];
            NSString *proximity = message;
            NSString *rssi = [NSString stringWithFormat:@"%ld",
                              (long)nearestBeacon.rssi];

            NSArray *data = [NSArray arrayWithObjects:
                             beaconUUID,  // Port 0 update
                             beaconMajor, // Port 1 update
                             beaconMinor, // Port 2 update
                             proximity,   // Port 3 update
                             rssi,        // Port 4 update
                             nil];
            [[NodeController sharedNodeController] sendPortEventMsgFromThing:ThingNameLocationBeacon
                                                                    withData:data];

            [self sendLocalNotificationWithMessage:message];
            NSLog(@"%@", message);

            // Notify interested UIViewController
            NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                       ThingNameLocationBeacon, @"thingName",
                                       proximity, @"proximity", nil];
            [[NSNotificationCenter defaultCenter] postNotificationName:@"yodiwoUIUpdateNotification"
                                                                object:self
                                                              userInfo:notParams];
        }
        else if([monitoringMode  isEqual: @"all"]) {

            // Loop through all ranged beacons
            for (CLBeacon *rangedB in beacons) {

                // Check if beacon exists in our monitored beacons
                Beacon *b = [self.beaconsDict objectForKey:rangedB.proximityUUID.UUIDString];
                if (b == nil) {
                    return;
                }

                // Check if proximity to this beacon has changed since last notification
                if(rangedB.proximity == b.lastProximity) {
                    return;
                }

                // Update proximity level of this beacon
                b.lastProximity = rangedB.proximity;
                [self.beaconsDict setObject:b forKey:b.ibeacon.proximityUUID.UUIDString];

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
