//
//  YABPairingNodeResponsePhase1.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JSONModel.h"

@interface YABPairingNodeResponsePhase1 : JSONModel

/*!
 * @discussion Designated initializer for YABPairingNodeResponsePhase1 class
 * @param userNodeRegistrationUrl
 * @param token2
 * @return Initialized YABPairingNodeResponsePhase1 object
 */
-(instancetype)initWithUserNodeRegistrationUrl:(NSString *)userNodeRegistrationUrl
                                        Token2:(NSString *)token2;
/*!
 * @discussion Initializer for YABPairingNodeResponsePhase1 class
 * @return Initialized YABPairingNodeResponsePhase1 object
 */
-(instancetype)init;

/*!
 * @discussion Convenience initializer for YABPairingNodeResponsePhase1 class
 * @param userNodeRegistrationUrl
 * @param token2
 * @return Initialized YABPairingNodeResponsePhase1 object
 */
+(instancetype)pairingNodeResponsePhase1WithUserNodeRegistrationUrl:(NSString *)userNodeRegistrationUrl
                                                             Token2:(NSString *)token2;

@end
