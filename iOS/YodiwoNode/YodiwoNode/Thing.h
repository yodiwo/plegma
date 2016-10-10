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
@property (strong, nonatomic) NSString<Optional> *IconURI;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *Description;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ConfigParameter : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Name;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Value;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString<Optional> *Description;

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
@property (strong, nonatomic) NSString *ThingKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Name;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<ConfigParameter,Optional> *Config; // of ConfigParameter

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSMutableArray<Port> *Ports; // of Port

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *Type;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *BlockType;

/*!
 * @discussion Unavailable
 */
@property (nonatomic) BOOL Removable;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) ThingUIHints *UIHints;

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