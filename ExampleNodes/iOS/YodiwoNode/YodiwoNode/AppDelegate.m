//
//  AppDelegate.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/24/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "AppDelegate.h"

@interface AppDelegate ()

@end

@implementation AppDelegate


- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {

#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 80000
    if ([application respondsToSelector:@selector(registerUserNotificationSettings:)]) {
        [application registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert|UIUserNotificationTypeBadge|UIUserNotificationTypeSound categories:nil]];
    }
#endif

    // Register defaults to application domain settings
    [self populateRegistrationDomain];

    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application {
    // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
    // Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
}

- (void)applicationDidEnterBackground:(UIApplication *)application {
    // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
    // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
}

- (void)applicationWillEnterForeground:(UIApplication *)application {
    // Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
}

- (void)applicationDidBecomeActive:(UIApplication *)application {
    // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
}

- (void)applicationWillTerminate:(UIApplication *)application {
    // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
}

- (void)populateRegistrationDomain
{
    NSURL *settingsBundleURL = [[NSBundle mainBundle] URLForResource:@"Settings" withExtension:@"bundle"];
    NSDictionary *appDefaults = [self loadDefaultsFromSettingsPage:@"Root.plist" inSettingsBundleAtURL:settingsBundleURL];

    // appDefaults is now populated with the preferences and their default values.
    // Add these to the registration domain.
    [[NSUserDefaults standardUserDefaults] registerDefaults:appDefaults];
    [[NSUserDefaults standardUserDefaults] synchronize];
}

- (NSDictionary*)loadDefaultsFromSettingsPage:(NSString*)plistName inSettingsBundleAtURL:(NSURL*)settingsBundleURL
{
    NSDictionary *settingsDict = [NSDictionary dictionaryWithContentsOfURL:[settingsBundleURL URLByAppendingPathComponent:plistName]];

    // The elements defined in a settings page are contained within an array
    // that is associated with the root-level PreferenceSpecifiers key.
    NSArray *prefSpecifierArray = settingsDict[@"PreferenceSpecifiers"];
    if (prefSpecifierArray == nil)
        return nil;

    // Create a dictionary to hold the parsed results.
    NSMutableDictionary *keyValuePairs = [NSMutableDictionary dictionary];

    for (NSDictionary *prefItem in prefSpecifierArray) {
        NSString *prefItemType = prefItem[@"Type"];
        NSString *prefItemKey = prefItem[@"Key"];
        NSString *prefItemDefaultValue = prefItem[@"DefaultValue"];

        if ([prefItemType isEqualToString:@"PSChildPaneSpecifier"]) {
            NSString *prefItemFile = prefItem[@"File"];
            NSDictionary *childPageKeyValuePairs = [self loadDefaultsFromSettingsPage:prefItemFile inSettingsBundleAtURL:settingsBundleURL];
            [keyValuePairs addEntriesFromDictionary:childPageKeyValuePairs];
        }
        else if (prefItemKey != nil && prefItemDefaultValue != nil) {
            keyValuePairs[prefItemKey] = prefItemDefaultValue;
        }
    }

    return keyValuePairs;
}

@end
