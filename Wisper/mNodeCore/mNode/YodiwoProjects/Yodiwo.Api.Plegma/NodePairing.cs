using System;
using System.Collections.Generic;
using System.Linq;


namespace Yodiwo.API.Plegma.NodePairing
{
    public static class NodePairingConstants
    {
        public const string Uuid = "uuid";
        public const string Token1 = "token1";
        public const string Token2 = "token2";
        public const string NodeKey = "nodekeystr";
        public const string SecretKey = "secretkey";

        public const string PairingRootURI = "pairing";
        public const string UserConfirmPageURI = "userconfirm";

        //Literal API names
        public const string s_GetTokensRequest = "gettokensreq";
        public const string s_GetKeysRequest = "getkeysreq";
        public const string s_TokensResponse = "tokensrsp";
        public const string s_KeysResponse = "keysrsp";

        /// <summary>
        /// Dictionary that maps API classes to names. These names are the ones used for REST routes, MQTT topics, or RabbitMQ queue names
        /// </summary>
        public static Dictionary<Type, String> ApiMsgNames = new Dictionary<Type, string>()
        {
            { typeof(PairingNodeGetTokensRequest),      s_GetTokensRequest      },
            { typeof(PairingNodeGetKeysRequest),        s_GetKeysRequest        },
            { typeof(PairingServerTokensResponse),      s_TokensResponse        },
            { typeof(PairingServerKeysResponse),        s_KeysResponse          },
        };

        /// <summary>
        /// Dictionary that maps API namses to classes. These names are the ones used for REST routes, MQTT topics, or RabbitMQ queue names
        /// </summary>
        public static Dictionary<String, Type> ApiMsgNamesToTypes = ApiMsgNames.Select(e => new KeyValuePair<string, Type>(e.Value, e.Key)).ToDictionary();
    }

    public enum PairingStates //TODO: cleanup
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
        Failed
    }

    public class PairingNodeGetTokensRequest
    {
        public string uuid;
        public string name;
        public string RedirectUri;
        public string image;
        public string description;
        public string pathcss;
        public string PairingCompletionInstructions;
        public byte[] PublicKey;
        public bool NoUUIDAuthentication;
    }

    public class PairingNodeGetKeysRequest
    {
        public string token1;
        public string token2;

        public PairingNodeGetKeysRequest() { }

        public PairingNodeGetKeysRequest(string token1, string token2)
        {
            this.token1 = token1;
            this.token2 = token2;

        }
    }

    public class PairingServerTokensResponse : GenericRsp
    {
        public string token1;
        public string token2;

        public PairingServerTokensResponse() { }

        public PairingServerTokensResponse(string token1, string token2)
        {
            this.token1 = token1;
            this.token2 = token2;
        }
    }

    public class PairingServerKeysResponse : GenericRsp
    {
        public string nodeKey;
        public string secretKey;
        public string email;

        public PairingServerKeysResponse() { }

        public PairingServerKeysResponse(string nodeKey, string secretKey, string email)
        {
            this.nodeKey = nodeKey;
            this.secretKey = secretKey;
            this.email = email;
        }
    }

    public class PairingNodePhase1Response
    {
        public string userNodeRegistrationUrl;
        public string token2;

        public PairingNodePhase1Response() { }

        public PairingNodePhase1Response(string userNodeRegistrationUrl, string token2)
        {
            this.userNodeRegistrationUrl = userNodeRegistrationUrl;
            this.token2 = token2;
        }
    }
}

