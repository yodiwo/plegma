//
//  NodePairingGetTokensRequest.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/24/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JSONModel.h"

@interface YABPairingNodeGetTokensRequest : JSONModel

/*!
 * @discussion Designated initializer for YABPairingNodeGetTokensRequest class
 * @param uuid Node UUID
 * @param name Node Name
 * @return Initialized YABPairingNodeGetTokensRequest object
 */
-(instancetype)initWithUuid:(NSString *)uuid
                       Name:(NSString *)name
                RedirectUri:(NSString *)redirectUri
    UseNoUUIDAuthentication:(BOOL)NoUUIDAuthentication NS_DESIGNATED_INITIALIZER;

/*!
 * @discussion Initializer for YABPairingNodeGetTokensRequest class
 * @return Initialized YABPairingNodeGetTokensRequest object
 */
-(instancetype)init;

/*!
 * @discussion Convenience initializer for YABPairingNodeGetTokensRequest class
 * @param nodeUuid Node UUID
 * @param nodeName Node Name
 * @return Initialized YABPairingNodeGetTokensRequest object
 */
+(instancetype)pairingNodeGetTokensRequestWithUuid:(NSString *)uuid
                                              Name:(NSString *)name
                                       RedirectUri:(NSString *)redirectUri
                              UseNoUUIDAuthentication:(BOOL)NoUUIDAuthentication
;

@end
