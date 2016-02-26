package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:03.
 */

public enum PairingStates {

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
}
