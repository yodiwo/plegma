//
//  YABPairingServerResponseTokens.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JSONModel.h"

@interface YABPairingServerResponseTokens : JSONModel

@property (strong, nonatomic) NSString *token1;

@property (strong, nonatomic) NSString *token2;

/*!
 * @discussion Designated initializer for YABPairingServerResponseTokens class
 * @param token1
 * @param token2
 * @return Initialized YABPairingServerResponseTokens object
 */
-(instancetype)initWithToken1:(NSString *)token1 Token2:(NSString *)token2;

/*!
 * @discussion Initializer for YABPairingServerResponseTokens class
 * @return Initialized YABPairingServerResponseTokens object
 */
-(instancetype)init;

/*!
 * @discussion Convenience initializer for YABPairingServerResponseTokens class
 * @param token1
 * @param token2
 * @return Initialized YABPairingServerResponseTokens object
 */
+(instancetype)pairingServerResponseTokensWithToken1:(NSString *)token1
                                              Token2:(NSString *)token2;


@end
