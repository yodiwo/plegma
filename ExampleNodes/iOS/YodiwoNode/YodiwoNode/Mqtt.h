//
//  Mqtt.h
//  YodiwoNode
//
//  Created by r00tb00t on 9/1/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "JSONModel.h"

@interface MqttAPIMessage : JSONModel

/*!
 * @discussion Unavailable
 */
@property (nonatomic) NSInteger ResponseToSeqNo;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Payload;

@end
