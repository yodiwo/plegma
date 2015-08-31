package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:02.
 */

        public class NodePairingConstants
        {

            public static final String Uuid;

            public static final String Token1;

            public static final String Token2;

            public static final String NodeKey;

            public static final String SecretKey;

            public static final String PairingRootURI;

            public static final String UserConfirmPageURI;

            public static final String UserConfirmFullURI;

            public static final String s_GetTokensRequest;

            public static final String s_GetKeysRequest;

            public static final String s_TokensResponse;

            public static final String s_KeysResponse;

            public static final HashMap<Class<?>,String> ApiMsgNames;

            public static final HashMap<String,Class<?>> ApiMsgNamesToTypes;


            static {
                    Uuid = "uuid";
                    Token1 = "token1";
                    Token2 = "token2";
                    NodeKey = "nodekeystr";
                    SecretKey = "secretkey";
                    PairingRootURI = "pairing";
                    UserConfirmPageURI = "userconfirm";
                    UserConfirmFullURI = "pairing/userconfirm";
                    s_GetTokensRequest = "gettokensreq";
                    s_GetKeysRequest = "getkeysreq";
                    s_TokensResponse = "tokensrsp";
                    s_KeysResponse = "keysrsp";
                    ApiMsgNames = new HashMap<Class<?>,String>();
ApiMsgNames.put(PairingNodeGetTokensRequest.class,"gettokensreq");
ApiMsgNames.put(PairingNodeGetKeysRequest.class,"getkeysreq");
ApiMsgNames.put(PairingServerTokensResponse.class,"tokensrsp");
ApiMsgNames.put(PairingServerKeysResponse.class,"keysrsp");
                    ApiMsgNamesToTypes = new HashMap<String,Class<?>>();
ApiMsgNamesToTypes.put("gettokensreq",PairingNodeGetTokensRequest.class);
ApiMsgNamesToTypes.put("getkeysreq",PairingNodeGetKeysRequest.class);
ApiMsgNamesToTypes.put("tokensrsp",PairingServerTokensResponse.class);
ApiMsgNamesToTypes.put("keysrsp",PairingServerKeysResponse.class);
            }
        }
