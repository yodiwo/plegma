//
//  YABPairingStates.h
//  YodiwoNode
//
//  Created by r00tb00t on 7/27/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#ifndef YodiwoNode_YABPairingStates_h
#define YodiwoNode_YABPairingStates_h

typedef NS_ENUM(NSInteger, YABEnumPairingStates)
{
    Initial,
    StartRequested,
    TokensRequested,
    TokensSentToNode,
    Token2SentToUser,
    Token2PostedToServer,
    UUIDEntryRedirect,
    Phase1Complete,
    NextRequested,
    Token1PostedToServer,
    KeysSentToNode,
    Paired,
    Failed,
};

#endif
