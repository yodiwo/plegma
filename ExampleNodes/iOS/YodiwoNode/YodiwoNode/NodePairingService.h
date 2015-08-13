//
//  NodePairingService.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NodePairingService : NSObject

-(void)initiatePairingWithCompletionHandler:(void(^)(NSString *pairingWebLoginUrl)) completionHandler;

-(void)finalizePairingWithCompletionHandler:(void(^)(BOOL result)) completionHandler;

@end
