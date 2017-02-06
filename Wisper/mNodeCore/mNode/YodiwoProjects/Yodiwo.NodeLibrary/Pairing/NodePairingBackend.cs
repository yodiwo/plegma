using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Plegma.NodePairing;
using System.Net;
using System.Security;

namespace Yodiwo.NodeLibrary.Pairing
{
    public class NodePairingBackend
    {
        #region Variables
        //--------------------------------------------------------------------------------------------------------------------
        public string frontendUrl;
        public string pairingPostUrl;
        private NodeConfig conf;
        string token1;
        string token2;
        public NodeKey? nodeKey;
        public SecureString secretKey;
        //--------------------------------------------------------------------------------------------------------------------
        public delegate void OnPairedDelegate(NodeKey nodeKey, SecureString nodeSecret);
        public event OnPairedDelegate onPaired = delegate { };
        //--------------------------------------------------------------------------------------------------------------------
        public delegate void OnPairingFailedDelegate(string Message);
        public event OnPairingFailedDelegate onPairingFailed = delegate { };
        //--------------------------------------------------------------------------------------------------------------------
        PairingStates pairingState;
        //--------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //--------------------------------------------------------------------------------------------------------------------
        public NodePairingBackend(string frontendUrl, NodeConfig conf, OnPairedDelegate callback, OnPairingFailedDelegate onPairingFailedCB)
        {
            this.frontendUrl = frontendUrl;
            this.pairingPostUrl = frontendUrl + "/" + NodePairingConstants.PairingRootURI + "/" + PlegmaAPI.APIVersion;
            this.conf = conf;
            pairingState = PairingStates.Initial;
            this.onPaired += callback;
            this.onPairingFailed += onPairingFailedCB;
        }
        //--------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //--------------------------------------------------------------------------------------------------------------------
        public string pairGetTokens(string redirectUri)
        {
            var req = new PairingNodeGetTokensRequest()
            {
                uuid = this.conf.uuid,
                name = this.conf.Name,
                image = this.conf.Image,
                description = this.conf.Description,
                pathcss = this.conf.Pathcss,
                PairingCompletionInstructions = this.conf.Pairing_CompletionInstructions,
                NoUUIDAuthentication = this.conf.Pairing_NoUUIDAuthentication,
                RedirectUri = redirectUri,
            };

            try
            {
                var response = Yodiwo.Tools.Http.RequestPost(this.pairingPostUrl + "/" + NodePairingConstants.s_GetTokensRequest,
                                                             req.ToJSON(),
                                                             HttpRequestDataFormat.Json,
                                                             null);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    onPairingFailed("HTTP Request for tokens failed with " + response.StatusCode);
                    return null;
                }
                if (string.IsNullOrWhiteSpace(response.ResponseBodyText))
                {
                    onPairingFailed("HTTP Request for tokens returned empty body");
                    return null;
                }
                var resp = response.ResponseBodyText.FromJSON<PairingServerTokensResponse>();
                if (resp != null)
                {
                    this.token1 = resp.token1;
                    this.token2 = resp.token2;
                    if (this.token1 != null && this.token2 != null)
                    {
                        pairingState = PairingStates.TokensSentToNode;
                        return this.token2;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
            return null;
        }
        //--------------------------------------------------------------------------------------------------------------------
        public Tuple<NodeKey, SecureString, HttpStatusCode> pairGetKeys()
        {
            if (pairingState != PairingStates.TokensSentToNode)
            {
                return null;
            }

            try
            {
                var req = new PairingNodeGetKeysRequest(this.token1, this.token2);
                var response = Yodiwo.Tools.Http.RequestPost(this.pairingPostUrl + "/" + NodePairingConstants.s_GetKeysRequest,
                                                             req.ToJSON(),
                                                             HttpRequestDataFormat.Json,
                                                             null);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    onPairingFailed("HTTP Request for keys failed with " + response.StatusCode);
                    return new Tuple<NodeKey, SecureString, HttpStatusCode>(null, null, response.StatusCode);
                }
                if (string.IsNullOrWhiteSpace(response.ResponseBodyText))
                {
                    onPairingFailed("HTTP Request keys returned empty body");
                    return new Tuple<NodeKey, SecureString, HttpStatusCode>(null, null, response.StatusCode);
                }
                var resp = response.ResponseBodyText.FromJSON<PairingServerKeysResponse>();

                this.nodeKey = resp.nodeKey;
                if (this.nodeKey != null && !string.IsNullOrWhiteSpace(resp.secretKey))
                {
                    this.secretKey = resp.secretKey.ToSecureString();
                    onPaired(this.nodeKey.Value, this.secretKey);
                    return new Tuple<NodeKey, SecureString, HttpStatusCode>(this.nodeKey.Value, this.secretKey, response.StatusCode);
                }
                else
                    onPairingFailed("NodeKey or SecretKey are invalid");
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
            return null;
        }
        //--------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
