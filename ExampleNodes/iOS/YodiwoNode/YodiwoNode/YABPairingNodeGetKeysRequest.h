//
//  YABPairingNodeGetKeysRequest.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JSONModel.h"

@interface YABPairingNodeGetKeysRequest : JSONModel

/*!
 * @discussion Designated initializer for YABPairingNodeGetKeysRequest class
 * @param uuid Node UUID
 * @param token1 Pairing token1
 * @return Initialized NodePairingGetTokensRequest object
 */
-(instancetype)initWithUuid:(NSString *)uuid
                    Token1:(NSString *)token1 NS_DESIGNATED_INITIALIZER;

/*!
 * @discussion Initializer for YABPairingNodeGetKeysRequest class
 * @return Initialized NodePairingGetTokensRequest object
 */
-(instancetype)init;

/*!
 * @discussion Convenience initializer for YABPairingNodeGetKeysRequest class
 * @param uuid Node UUID
 * @param name token1 Pairing token1
 * @return Initialized YABPairingNodeGetKeysRequest object
 */
+(instancetype)pairingNodeGetKeysRequestWithUuid:(NSString *)uuid
                                          Token1:(NSString *)name;


@end
