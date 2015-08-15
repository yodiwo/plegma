//
//  MotionManagerModule.m
//  YodiwoNode
//
//  Created by r00tb00t on 8/13/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "MotionManagerModule.h"
#import "NodeThingsRegistry.h"
#import "NodeController.h"

#import <CoreMotion/CoreMotion.h>


@interface MotionManagerModule ()

@property (strong, nonatomic) CMMotionManager *motionRawManager;

@property (strong, nonatomic) CMMotionActivityManager *motionActivityManager;

@property (strong, nonatomic) CMPedometer *motionPedometer;

@end


@implementation MotionManagerModule

///***** Override synthesized getters, lazy instantiate

-(CMMotionManager *)motionRawManager {
    
    if(_motionRawManager == nil) {

        _motionRawManager = [CMMotionManager new];
    }

    return _motionRawManager;
}

-(CMMotionActivityManager *)motionActivityManager {

    if(_motionActivityManager == nil) {
        _motionActivityManager = [CMMotionActivityManager new];
    }
    return _motionActivityManager;
}

-(CMPedometer *)motionPedometer {

    if(_motionPedometer == nil) {
        _motionPedometer = [CMPedometer new];
    }
    return _motionPedometer;
}

//******************************************************************************




///***** Public api

- (void)start {

    // Raw motion sensors
    if (self.motionRawManager.deviceMotionAvailable){

        self.motionRawManager.deviceMotionUpdateInterval = 10;

        [self.motionRawManager startDeviceMotionUpdatesToQueue:[NSOperationQueue new]
                                                withHandler:^(CMDeviceMotion *motion,
                                                              NSError *error) {

                                                    NSLog(@"New DeviceMotion data: %@", motion);
                                                }];
    }
    else {
        NSLog(@"Device motion monitoring is not available");
    }


    // Activity manager
    if ([CMMotionActivityManager isActivityAvailable]) {

        [self.motionActivityManager startActivityUpdatesToQueue:[NSOperationQueue new]
                                 withHandler:^(CMMotionActivity *activity) {
                                     if (activity.confidence  == CMMotionActivityConfidenceHigh){
                                         NSLog(@"Quite probably a new activity.");
                                         NSDate *started = activity.startDate;

                                         NSString *userActivity = @"";

                                         if (activity.stationary){
                                             NSLog(@"Sitting");
                                             userActivity = @"Sitting";
                                         } else if (activity.running){
                                             NSLog(@"Running");
                                             userActivity = @"Running";
                                         } else if (activity.automotive){
                                             NSLog(@"Driving");
                                             userActivity = @"Driving";
                                         } else if (activity.walking){
                                             NSLog(@"Walking");
                                             userActivity = @"Walking";
                                         }

                                         // Notify cloud service
                                         NSArray *data = [NSArray arrayWithObjects:userActivity, nil];

                                         [[NodeController sharedNodeController]
                                          sendPortEventMsgFromThing:ThingNameActivityTracker withData:data];

                                         // Notify interested UIViewController
                                         NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                                                    ThingNameActivityTracker, @"thingName",
                                                                    userActivity, @"activity",
                                                                    nil];
                                         [[NSNotificationCenter defaultCenter]
                                                postNotificationName:@"yodiwoUIUpdateNotification"
                                                              object:self
                                                            userInfo:notParams];
                                     }
                                 }];
    }
    else {
        NSLog(@"Activity manager is not available on this device");
    }

    
    // TODO::: Check if each sensor available with isGyroAvailable

    // TODO:: stopGyroUpdates, etc (the same for location manager ?)

    /*
     [self.motionRawManager startAccelerometerUpdatesToQueue:[NSOperationQueue new]
     withHandler:^(CMAccelerometerData *accelerometerData,
     NSError *error) {

     NSLog(@"New Accelerometer data: %@", accelerometerData);
     }];

     [self.motionRawManager startGyroUpdatesToQueue:[NSOperationQueue new]
     withHandler:^(CMGyroData *gyroData,
     NSError *error) {

     NSLog(@"New Gyro data: %@", gyroData);
     }];

     [self.motionRawManager startMagnetometerUpdatesToQueue:[NSOperationQueue new]
     withHandler:^(CMMagnetometerData *magnetometerData,
     NSError *error) {

     NSLog(@"New Magnetometer data: %@", magnetometerData);
     }];
     */

}
//******************************************************************************

@end

