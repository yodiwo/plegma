//
//  Keys.h
//  YodiwoNode
//
//  Created by r00tb00t on 8/2/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "JSONModel.h"

/*!
 * @discussion Unavailable
 */
@interface UserKey : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *userID;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(NSString *)toString;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface NodeKey : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) UserKey *userKey;

/*!
 * @discussion Unavailable
 */
@property  NSInteger nodeID;


/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(instancetype)initWithUserKey:(UserKey *)userKey
                    andNodeId:(NSString *)nodeID;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(instancetype)initWithNodeKeyString:(NSString *)nodeKey;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(NSString *)toString;

@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface ThingKey : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NodeKey *nodeKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *thingUID;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(instancetype)initWithNodeKey:(NodeKey *)nodeKey
                   andThingUid:(NSString *)thingUID;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(instancetype)initFromString:(NSString *)thingKey;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(NSString *)toString;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
+(NSString *)createKeyFromNodeKey:(NSString *)nodeKey
                        thingName:(NSString *)thingName;
@end

//******************************************************************************

/*!
 * @discussion Unavailable
 */
@interface PortKey : JSONModel

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) ThingKey *thingKey;

/*!
 * @discussion Unavailable
 */
@property (strong, nonatomic) NSString *portUID;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(instancetype)initWithThingKey:(ThingKey *)thingKey
                     andPortUid:(NSString *)portUID;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(instancetype)initFromString:(NSString *)portKey;

/*!
 * @discussion <#description#>
 * @param <#param description#>
 * @return <#return description#>
 */
-(NSString *)toString;

@end
