//
//  YABPairingServerResponseKeys.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JSONModel.h"

@interface YABPairingServerResponseKeys : JSONModel

@property (strong, nonatomic) NSString *nodeKey;

@property (strong, nonatomic) NSString *secretKey;

/*!
 * @discussion Designated initializer for YABPairingServerResponseKeys class
 * @param nodeKey
 * @param secretKey
 * @return Initialized YABPairingServerResponseKeys object
 */
-(instancetype)initWithNodeKey:(NSString *)nodeKey SecretKey:(NSString *)secretKey;

/*!
 * @discussion Initializer for YABPairingServerResponseKeys class
 * @return Initialized YABPairingServerResponseKeys object
 */
-(instancetype)init;

/*!
 * @discussion Convenience initializer for YABPairingServerResponseKeys class
 * @param nodeKey
 * @param secretKey
 * @return Initialized YABPairingServerResponseKeys object
 */
+(instancetype)pairingServerResponseKeysWithNodeKey:(NSString *)nodeKey
                                          SecretKey:(NSString *)secretKey;

@end
