//
//  NodeThingsViewController.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/31/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "NodeThingsViewController.h"
#import "NodeController.h"
#import "NodeThingsRegistry.h"
#import "SettingsVault.h"
#import "Reachability.h"

#import "TWMessageBarManager.h"
#import <MapKit/MapKit.h>
#import <CoreLocation/CoreLocation.h>
#import <AVFoundation/AVFoundation.h>

@interface NodeThingsViewController ()

@property (weak, nonatomic) IBOutlet UISwitch *thingInVirtualSwitch;
@property (weak, nonatomic) IBOutlet UILabel *virtualTextLabel;

@property (weak, nonatomic) IBOutlet UILabel *userActivityLabel;
@property (weak, nonatomic) IBOutlet UILabel *deviceMotionLabel;
@property (weak, nonatomic) IBOutlet MKMapView *gpsLocationMapView;
@property (weak, nonatomic) IBOutlet UILabel *iBeacon1ProximityLevelLabel;
@property (weak, nonatomic) IBOutlet UILabel *iBeacon2ProximityLevelLabel;
@property (weak, nonatomic) IBOutlet UITextField *thingInVirtualTextInput;
@property (weak, nonatomic) IBOutlet UISlider *thingInVirtualSlider;
@property (weak, nonatomic) IBOutlet UIButton *thingOutVirtualLight1;
@property (weak, nonatomic) IBOutlet UIButton *thingOutVirtualLight2;
@property (strong, nonatomic) IBOutlet UIView *hubThingsSceneView;

@property BOOL initialConnectionToCloudServicePending;
@property (strong, nonatomic) UIView *uiDisabledEmptyView;

@property (strong, nonatomic) Reachability *nodeReachability;

@end



@implementation NodeThingsViewController

///***** UI actions

- (IBAction)thingInVirtualSwitchValueChanged:(UISwitch *)sender
                                    forEvent:(UIEvent *)event {

    NSArray *array = [NSArray arrayWithObjects:(sender.on) ?
                      @"true" : @"false", nil];

    [[NodeController sharedNodeController] sendPortEventMsgFromThing:ThingNameVirtualSwitch
                                                            withData:array];
}

- (IBAction)thingInVirtualTextInputEditingDidEnd:(UITextField *)sender {

    NSArray *array = [NSArray arrayWithObjects:sender.text, nil];

    [[NodeController sharedNodeController] sendPortEventMsgFromThing:ThingNameVirtualTextInput
                                                            withData:array];


}

- (IBAction)thingInVirtualSliderValueChanged:(UISlider *)sender {

    NSArray *array = [NSArray arrayWithObjects:
                      [[NSNumber numberWithFloat:sender.value] stringValue], nil];

    [[NodeController sharedNodeController] sendPortEventMsgFromThing:ThingNameVirtualSlider
                                                            withData:array];
}

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event {
    [self.thingInVirtualTextInput endEditing:YES];
}

//******************************************************************************





///***** Override synthesized getters, lazy instantiate


//******************************************************************************





///***** View related

- (void)viewDidLoad {
    [super viewDidLoad];

    self.uiDisabledEmptyView = [[UIView alloc] initWithFrame:
                                CGRectMake(0,
                                           0,
                                           [[UIScreen mainScreen] bounds].size.width,
                                           [[UIScreen mainScreen] bounds].size.height)];
    [self.uiDisabledEmptyView  setBackgroundColor:[UIColor blackColor]];
    [self.uiDisabledEmptyView setAlpha:0.7];

    // Disable user interaction until connected to cloud service
    self.initialConnectionToCloudServicePending = YES;
    [self.hubThingsSceneView setUserInteractionEnabled:NO];
    [self.view addSubview:self.uiDisabledEmptyView];

    [[NodeController sharedNodeController] populateNodeThingsRegistry];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(yodiwoConnectedToCloudServiceNotification:)
                                                 name:@"yodiwoConnectedToCloudServiceNotification"
                                               object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(yodiwoDisconnectedFromCloudServiceNotification:)
                                                 name:@"yodiwoDisconnectedFromCloudServiceNotification"
                                               object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(yodiwoThingUpdateNotification:)
                                                 name:@"yodiwoThingUpdateNotification"
                                               object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(yodiwoUIUpdateNotification:)
                                                 name:@"yodiwoUIUpdateNotification"
                                               object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(yodiwoNodeUnpairedNotification:)
                                                 name:@"yodiwoNodeUnpairedNotification"
                                               object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(yodiwoNodeIsApiConnectedNotification:)
                                                 name:@"yodiwoNodeIsApiConnectedNotification"
                                               object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector: @selector(reachabilityChanged:)
                                                 name: kReachabilityChangedNotification
                                               object: nil];

    // Initialize UI stuff
    self.gpsLocationMapView.mapType = MKMapTypeStandard;
    self.gpsLocationMapView.zoomEnabled = YES;
    self.gpsLocationMapView.scrollEnabled = YES;

    // Reachability
    self.nodeReachability = [Reachability reachabilityForLocalWiFi];
    [self.nodeReachability startNotifier];

    // Shoulder-tap yodiwo cloud
    [[NodeController sharedNodeController] connectToCloudService];
}

// For UIEvent motion monitoring
- (BOOL)canBecomeFirstResponder {
    return YES;
}

- (void)viewDidAppear:(BOOL)animated {
    [self becomeFirstResponder];
}

- (void)viewDidDisappear:(BOOL)animated {

}

- (void)viewWillAppear:(BOOL)animated {

}

- (void)viewWillDisappear:(BOOL)animated {

}


#pragma mark - Navigation

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {

}

// Shake detection
- (void)motionEnded:(UIEventSubtype)motion withEvent:(UIEvent *)event {
    if (motion == UIEventSubtypeMotionShake)
    {
        // Notify cloud service
        NSArray *data = [NSArray arrayWithObjects:@"true", nil];
        [[NodeController sharedNodeController]
         sendPortEventMsgFromThing:ThingNameShakeDetector withData:data];

        // Notify interested UIViewController
        NSDictionary *notParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                   ThingNameShakeDetector, @"thingName",
                                   [NSNumber numberWithBool:YES], @"hasShaken",
                                   nil];
        [[NSNotificationCenter defaultCenter] postNotificationName:@"yodiwoUIUpdateNotification"
                                                            object:self
                                                          userInfo:notParams];
    }
}

//******************************************************************************





///***** Memory

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}
//******************************************************************************





///***** Observers

- (void)yodiwoConnectedToCloudServiceNotification:(NSNotification *)notification {

    // Enable user interaction and inform user
    [self.hubThingsSceneView setUserInteractionEnabled:YES];
    dispatch_async(dispatch_get_main_queue(), ^{
        [[TWMessageBarManager sharedInstance] showMessageWithTitle:@"Node info:"
                                                       description:@"Connected to cloud service!"
                                                              type:TWMessageBarMessageTypeSuccess
                                                          duration:3.0];
    });

    if (self.initialConnectionToCloudServicePending == YES) {
        // Start location manager module
        [[NodeController sharedNodeController] startLocationManagerModule];

        // Start motion manager module
        [[NodeController sharedNodeController] startMotionManagerModule];

        // Start bluetooth manager module
        [[NodeController sharedNodeController] startBluetoothManagerModule];

        self.initialConnectionToCloudServicePending = NO;
    }

    dispatch_async(dispatch_get_main_queue(), ^{
        [self.uiDisabledEmptyView removeFromSuperview];
    });
}

- (void)yodiwoDisconnectedFromCloudServiceNotification:(NSNotification *)notification {

    // Set API-level connection status
    [[SettingsVault sharedSettingsVault] setIsNodeApiConnected:NO];

    // Disable user interaction and inform user
    [self.hubThingsSceneView setUserInteractionEnabled:NO];
    dispatch_async(dispatch_get_main_queue(), ^{
        [[TWMessageBarManager sharedInstance] showMessageWithTitle:@"Node info:"
                                                       description:@"Disconnected from cloud service!"
                                                              type:TWMessageBarMessageTypeError
                                                          duration:3.0];
    });

    dispatch_async(dispatch_get_main_queue(), ^{
        [self.view addSubview:self.uiDisabledEmptyView];
    });
}

- (void)yodiwoThingUpdateNotification:(NSNotification *)notification {

    // Get notification parameters
    NSDictionary *notParams = [notification userInfo];

    NSString *thingName = [notParams objectForKey:@"thingName"];
    NSNumber *portIndex = [notParams objectForKey:@"portIndex"];
    NSString *newState = [notParams objectForKey:@"newState"];

    if ([thingName isEqualToString:ThingNameVirtualText]) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.virtualTextLabel.text = newState;
        });
    }
    else if ([thingName isEqualToString:ThingNameVirtualLight1]) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if ([newState boolValue]) {
                self.thingOutVirtualLight1.backgroundColor = [UIColor whiteColor];
                [self.thingOutVirtualLight1 setTitle:@"On" forState:UIControlStateNormal];
            }
            else {
                self.thingOutVirtualLight1.backgroundColor = [UIColor blackColor];
                [self.thingOutVirtualLight1 setTitle:@"Off" forState:UIControlStateNormal];
            }
        });
    }
    else if ([thingName isEqualToString:ThingNameVirtualLight2]) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if ([newState boolValue]) {
                self.thingOutVirtualLight2.backgroundColor = [UIColor whiteColor];
                [self.thingOutVirtualLight2 setTitle:@"On" forState:UIControlStateNormal];
            }
            else {
                self.thingOutVirtualLight2.backgroundColor = [UIColor blackColor];
                [self.thingOutVirtualLight2 setTitle:@"Off" forState:UIControlStateNormal];
            }
        });
    }
    else if ([thingName isEqualToString:ThingNameVirtualSwitch]) {
        dispatch_async(dispatch_get_main_queue(), ^{
                self.thingInVirtualSwitch.on = [newState boolValue];
        });
    }
    else if ([thingName isEqualToString:ThingNameAVTorch]) {
        [self turnTorch:[newState boolValue]];
    }
    else {
        NSLog(@"****** Received yodiwoThingUpdateNotification for unknown thing *******");
    }
}

- (void)yodiwoUIUpdateNotification:(NSNotification *)notification {

    // Get notification parameters
    NSDictionary *notParams = [notification userInfo];
    NSString *thingName = [notParams objectForKey:@"thingName"];

    if ([thingName isEqualToString:ThingNameLocationGPS]) {
        // Region
        float spanX = 0.05;
        float spanY = 0.05;
        MKCoordinateRegion region;
        region.center.latitude = self.gpsLocationMapView.userLocation.coordinate.latitude;
        region.center.longitude = self.gpsLocationMapView.userLocation.coordinate.longitude;
        region.span.latitudeDelta = spanX;
        region.span.longitudeDelta = spanY;

        // Annotation
        MKPointAnnotation *ant = [[MKPointAnnotation alloc] init];
        ant.coordinate = [[notParams objectForKey:@"location"] coordinate];
        ant.title = @"Node location";
        ant.subtitle = [notParams objectForKey:@"locationFriendly"];

        dispatch_async(dispatch_get_main_queue(), ^{
            ant.title = @"Node location";
            [self.gpsLocationMapView setRegion:region animated:YES];
            [self.gpsLocationMapView setCenterCoordinate:ant.coordinate animated:YES];
            [self.gpsLocationMapView addAnnotation:ant];
        });
    }
    else if ([thingName isEqualToString:ThingNameLocationBeacon]) {
        if ( [((NSString *)[notParams objectForKey:@"uuid"])
              isEqualToString:[[SettingsVault sharedSettingsVault]
                                        getIBeaconParamsMonitoredUUID1]]) {

            dispatch_async(dispatch_get_main_queue(), ^{
                self.iBeacon1ProximityLevelLabel.text = [notParams objectForKey:@"proximity"];
            });
        }
        else if ( [((NSString *)[notParams objectForKey:@"uuid"])
                   isEqualToString:[[SettingsVault sharedSettingsVault]
                                    getIBeaconParamsMonitoredUUID2]]) {

                       dispatch_async(dispatch_get_main_queue(), ^{
                           self.iBeacon2ProximityLevelLabel.text = [notParams objectForKey:@"proximity"];
                       });
        }
    }
    else if ([thingName isEqualToString:ThingNameShakeDetector]) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.deviceMotionLabel.text =
                [[notParams objectForKey:@"hasShaken"] boolValue] ? @"Device has shaken" : @"";
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, 3 * NSEC_PER_SEC),
                           dispatch_get_main_queue(), ^{
                self.deviceMotionLabel.text = @"";
                           });
        });
    }
    else if ([thingName isEqualToString:ThingNameActivityTracker]) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.userActivityLabel.text = [notParams objectForKey:@"activity"];
        });
    }
    else {
        NSLog(@"****** Received yodiwoUIUpdateNotification for unknown thing *******");
    }
}

- (void)yodiwoNodeIsApiConnectedNotification:(NSNotification *)notification {

    [[SettingsVault sharedSettingsVault] setIsNodeApiConnected:YES];

    // Send PortStateReq with node things
    [[NodeController sharedNodeController] sendApiMsgOfType:[[PlegmaApi apiMsgNames] objectForKey:[PortStateReq class]]
                                                 withSyncId:0
                                             withParameters:nil
                                                    andData:nil];

}

- (void)yodiwoNodeUnpairedNotification:(NSNotification *)notification {

    // Get notification parameters
    NSDictionary *notParams = [notification userInfo];
    NSString *rcStr = ([notParams objectForKey:@"reasoncode"] == nil) ? @"" : [notParams objectForKey:@"reasoncode"];
    NSString *message = ([notParams objectForKey:@"message"] == nil) ? @"" : [notParams objectForKey:@"message"];

    [self.hubThingsSceneView setUserInteractionEnabled:NO];

    NSString *msg = @"Node has been unpaired from your account: Reason: ";
    msg = [msg stringByAppendingString:rcStr];
    msg = [msg stringByAppendingString:@" Message: "];
    msg = [msg stringByAppendingString:message];

    dispatch_async(dispatch_get_main_queue(), ^{
        [[TWMessageBarManager sharedInstance] showMessageWithTitle:@"Node info:"
                                                       description:msg
                                                              type:TWMessageBarMessageTypeInfo
                                                          duration:3.0];
    });

    // Clean-up URL cache, cookies
    [[NSURLCache sharedURLCache] removeAllCachedResponses];
    for(NSHTTPCookie *cookie in [[NSHTTPCookieStorage sharedHTTPCookieStorage] cookies]) {
        [[NSHTTPCookieStorage sharedHTTPCookieStorage] deleteCookie:cookie];
    }

    // Clear NodeKey, SecretKey
    [[SettingsVault sharedSettingsVault] setPairingKeysWithNodeKey:nil secretKey:nil];

    // Set Node status to Unpaired and API-disconnected
    [[SettingsVault sharedSettingsVault] setIsNodePaired:NO];
    [[SettingsVault sharedSettingsVault] setIsNodeApiConnected:NO];

    // Re-populate things registry
    [[NodeController sharedNodeController] clearThings];

    // Go back to app start-up screen
    dispatch_async(dispatch_get_main_queue(), ^{
        [self performSegueWithIdentifier:@"nodeUnpairedFromCloudGoBackToStartScreen" sender:self];
    });
}

- (void)reachabilityChanged:(NSNotification *)notification
{
    NSString *wifiStatus = @"";

    Reachability* currentReachability = [notification object];
    NSParameterAssert([currentReachability isKindOfClass: [Reachability class]]);

    NetworkStatus netStatus = [currentReachability currentReachabilityStatus];
    if (netStatus == ReachableViaWiFi) {

        NSLog(@"Reachability status: Reachable via WiFi");
        wifiStatus = @"CONNECTED";

        /** This was deprecated in iOS 9 and re-enabled later on - in any case it
         *  is considered a hacky workaround since no public API for acquiring network
         *  info is provided by Apple
         **
        NSString *wifiName = nil;
        NSArray *interFaceNames = (__bridge_transfer id)CNCopySupportedInterfaces();

        for (NSString *name in interFaceNames) {
            NSDictionary *info = (__bridge_transfer id)CNCopyCurrentNetworkInfo((__bridge CFStringRef)name);

            if (info[@"SSID"]) {
                wifiName = info[@"SSID"]; 
            } 
        }
        */
    }
    else {

        NSLog(@"Reachability status: Not reachable via WiFi");
        wifiStatus = @"DISCONNECTED";
    }

    // Notify cloud service
    NSArray *data = [NSArray arrayWithObjects:wifiStatus, nil];
    [[NodeController sharedNodeController] sendPortEventMsgFromThing:ThingNameWiFiStatus withData:data];
}

//******************************************************************************



///***** Helpers

// TODO: Move this and all FlashLight related handling to dedicated AVDeviceManager
- (void)turnTorch:(BOOL)state
{
    AVCaptureDevice *flashLight = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if ([flashLight isTorchAvailable] && [flashLight isTorchModeSupported:AVCaptureTorchModeOn])
    {
        BOOL success = [flashLight lockForConfiguration:nil];
        if (success)
        {
            if (state)
            {
                [flashLight setTorchMode:AVCaptureTorchModeOn];
            }
            else
            {
                [flashLight setTorchMode:AVCaptureTorchModeOff];
            }
            [flashLight unlockForConfiguration];
        }
    }
}

//******************************************************************************


@end

