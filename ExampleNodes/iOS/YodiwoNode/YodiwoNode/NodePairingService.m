//
//  NodePairingService.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "NodePairingService.h"
#import "JSONModel.h"
#import "SettingsVault.h"
#import "YodiwoApi.h"

@interface NodePairingService ()

@property (strong, nonatomic) void(^notifyViewControllerToTriggerWebLogin)(NSString *pairingWebLoginUrl);

@end


@implementation NodePairingService

///***** Override getters, lazy instantiate

-(NSDictionary *)pairingServerRoutesDict {
    if (!_pairingServerRoutesDict) {
        NSString *prefix = @"/pairing/";
        prefix = [prefix stringByAppendingString:
                    [NSString stringWithFormat:@"%ld", (long)[PlegmaApi apiVersion]]];

        _pairingServerRoutesDict = [NSDictionary dictionaryWithObjectsAndKeys:
                                        [prefix stringByAppendingString:@"/gettokensreq"], @"getTokens",
                                        [prefix stringByAppendingString:@"/getkeysreq"], @"getKeys",
                                        [prefix stringByAppendingString:@"/userconfirm"], @"userconfirm",
                                        [prefix stringByAppendingString:@"/success"], @"noderedirect",
                                        nil];
    }

    return _pairingServerRoutesDict;
}
///*****************************************************************************





///***** Public methods

-(NSString *)getPairingWebUrl {

    NSString *server = [[SettingsVault sharedSettingsVault] getConnectionParamsServerAddress];
    BOOL useSSL = [[SettingsVault sharedSettingsVault] getConnectionParamsSecureConnection];
    NSString *protocol =  useSSL ? @"https://" : @"http://";
    NSString *url = [[NSString stringWithString:protocol] stringByAppendingString:server];

    return url;
}

-(void)initiatePairingWithCompletionHandler:(void(^)(NSString *pairingWebLoginUrl)) completionHandler {
    NSString *pairingUrl = [self getPairingWebUrl];

    // Send get tokens request
    YABPairingNodeGetTokensRequest *getTokensReq = [YABPairingNodeGetTokensRequest
                                            pairingNodeGetTokensRequestWithUuid:[[SettingsVault sharedSettingsVault] getPairingParamsNodeUuid]
                                                                           Name:[[SettingsVault sharedSettingsVault] getPairingParamsNodeName]];

    NSLog(@"NodePairingService: YABPairingNodeGetTokensRequest body -> %@", [getTokensReq toJSONString]);

    [self restHelperSendHttpRequestWithMethod:@"POST"
                                 withHttpBody:[getTokensReq toJSONData]
                              withContentType:@"application/json"
                                      withUrl:[pairingUrl
                                               stringByAppendingString:[self.pairingServerRoutesDict valueForKey:@"getTokens"]]
                         andCompletionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {

                             if (error != nil) {
                                 NSLog(@"NodePairingService: YABPairingNodeGetTokensRequest error: %ld", (long)error.code);
                                 return;
                             }

                             JSONModelError *err;
                             YABPairingServerResponseTokens *tokens = [[YABPairingServerResponseTokens alloc]
                                                                       initWithString:[[NSString alloc] initWithData:data
                                                                                                            encoding:NSUTF8StringEncoding]
                                                                       error:&err];

                             if (tokens == nil || [tokens.token1 isEqualToString:@""] || [tokens.token2 isEqualToString:@""]) {
                                 NSLog(@"NodePairingService: Error getting tokens");
                                 return;
                             }

                             // Keep Token1 for phase 2
                             [[SettingsVault sharedSettingsVault] setPairingToken1:tokens.token1];

                             // Construct pairing web login url and notify ViewController
                             NSString *url = [NSString stringWithString:[self getPairingWebUrl]];
                             url = [url stringByAppendingString:[self.pairingServerRoutesDict valueForKey:@"userconfirm"]];
                             url = [[url stringByAppendingString:@"?token2="]
                                    stringByAppendingString:tokens.token2];
                             url = [[url stringByAppendingString:@"&noderedirect="]
                                    stringByAppendingString:[[self getPairingWebUrl]
                                                             stringByAppendingString:[self.pairingServerRoutesDict valueForKey:@"noderedirect"]]];
                             url = [[url stringByAppendingString:@"&uuid="]
                                    stringByAppendingString:[[SettingsVault sharedSettingsVault] getPairingParamsNodeUuid]];
                             
                             completionHandler(url);
                         }];
    }

-(void)finalizePairingWithCompletionHandler:(void(^)(BOOL result)) completionHandler {
    YABPairingNodeGetKeysRequest *getKeysRequest =
        [YABPairingNodeGetKeysRequest
         pairingNodeGetKeysRequestWithUuid:[[SettingsVault sharedSettingsVault] getPairingParamsNodeUuid]
                                    Token1:[[SettingsVault sharedSettingsVault] getPairingToken1]];


    [self restHelperSendHttpRequestWithMethod:@"POST"
                                 withHttpBody:[getKeysRequest toJSONData]
                              withContentType:@"application/json"
                                      withUrl:[[self getPairingWebUrl] stringByAppendingString:
                                                        [self.pairingServerRoutesDict
                                                         valueForKey:@"getKeys"]]
                         andCompletionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
                             if (error != nil) {
                                 NSLog(@"NodePairingService: YABPairingNodeGetKeysRequest error: %ld", (long)error.code);
                                 completionHandler(NO);
                                 return;
                             }

                             JSONModelError *err;
                             YABPairingServerResponseKeys *keys = [[YABPairingServerResponseKeys alloc]
                                                                       initWithString:[[NSString alloc]
                                                                                       initWithData:data
                                                                                           encoding:NSUTF8StringEncoding]
                                                                       error:&err];

                             if (keys == nil || [keys.nodeKey isEqualToString:@""] || [keys.secretKey isEqualToString:@""]) {
                                 NSLog(@"NodePairingService: Error getting keys");
                                 completionHandler(NO);
                                 return;
                             }

                             // Keep nodeKey, secretKey
                             [[SettingsVault sharedSettingsVault]
                                    setPairingKeysWithNodeKey:keys.nodeKey
                                                    secretKey:keys.secretKey];
                             [[SettingsVault sharedSettingsVault] setNodePaired:YES];
                             completionHandler(YES);

                             NSLog(@"NodePairingService: Pairing complete with NodeKey = %@, SecretKey = %@",
                                   keys.nodeKey, keys.secretKey);
                         }];
}
///*****************************************************************************





///***** Helpers

// Wrapper method for sending REST requests
-(void)restHelperSendHttpRequestWithMethod:(NSString *)method
                              withHttpBody:(NSData *)body
                           withContentType:(NSString *)contentType
                                   withUrl:(NSString *)url
                      andCompletionHandler:(void(^)(NSData *data,
                                                    NSURLResponse *response,
                                                    NSError *error)) completionHandler {

    // Build-up request
    NSMutableURLRequest *request = [NSMutableURLRequest
                                    requestWithURL:[NSURL
                                                    URLWithString:[NSString stringWithString:url]]];
    [request setHTTPBody:body];
    [request setHTTPMethod:method];
    [request setHTTPShouldHandleCookies:YES];
    [request addValue:contentType forHTTPHeaderField:@"Content-type"];

    // Initiate data-task
    NSURLSession *session = [NSURLSession sharedSession];
    NSURLSessionDataTask *dataTask =
    [session dataTaskWithRequest:request completionHandler:completionHandler];
    [dataTask resume];
}
///*****************************************************************************
@end
