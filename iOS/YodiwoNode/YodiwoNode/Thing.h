//
//  Thing.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/2/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "JSONModel.h"


@protocol Port;

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ThingUIHints : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *iconUri;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ConfigParameter : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *name;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *value;

@end

@protocol ConfigParameter

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface Thing : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *thingKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *name;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<ConfigParameter> *config; // of ConfigParameter

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<Port> *ports; // of Port

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *type;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *blockType;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) ThingUIHints *uiHints;

-(instancetype)initWithThingKey:(NSString *)thingKey
                           name:(NSString *)name
                         config:(NSMutableArray *)config
                          ports:(NSMutableArray *)ports
                           type:(NSString *)type
                      blockType:(NSString *)blockType
                        uiHints:(ThingUIHints *)uiHints;

@end

@protocol Thing

@end

//******************************************************************************