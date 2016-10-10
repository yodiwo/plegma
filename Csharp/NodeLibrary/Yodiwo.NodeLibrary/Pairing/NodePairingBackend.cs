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
            var uri = this.pairingPostUrl + "/" + NodePairingConstants.s_GetTokensRequest;
            PairingServerTokensResponse resp = (PairingServerTokensResponse)jsonPost(uri, req, typeof(PairingServerTokensResponse));
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
            return null;
        }
        //--------------------------------------------------------------------------------------------------------------------
        public Tuple<NodeKey, SecureString> pairGetKeys()
        {
            if (pairingState != PairingStates.TokensSentToNode)
            {
                return null;
            }
            var req = new PairingNodeGetKeysRequest(this.token1, this.token2);
            var resp = (PairingServerKeysResponse)jsonPost(this.pairingPostUrl + "/" + NodePairingConstants.s_GetKeysRequest, req, typeof(PairingServerKeysResponse));
            if (resp != null)
            {
                this.nodeKey = resp.nodeKey;
                if (this.nodeKey != null && !string.IsNullOrWhiteSpace(resp.secretKey))
                {
                    this.secretKey = resp.secretKey.ToSecureString();
                    onPaired(this.nodeKey.Value, this.secretKey);
                    return new Tuple<NodeKey, SecureString>(this.nodeKey.Value, this.secretKey);
                }
                else
                    onPairingFailed("NodeKey or SecretKey are invalid");
            }
            else
                onPairingFailed("Could not get keys");
            return null;
        }
        //--------------------------------------------------------------------------------------------------------------------
        public static object jsonPost(string url, object data, Type responseType)
        {
            var response = default(Tools.Http.RequestResult);
            try
            {
                response = Yodiwo.Tools.Http.RequestPost(url, data.ToJSON(), HttpRequestDataFormat.Json, null);
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(response.ResponseBodyText))
                {
                    return response.ResponseBodyText.FromJSON(responseType);
                }
                else
                {
                    DebugEx.TraceWarning("post response: " + response.StatusCode.ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (response.ResponseBodyText == null)
                    DebugEx.Assert(ex, "Pairing jsonPost failed (no body)");
                else
                    DebugEx.Assert(ex, "Pairing jsonPost failed" + Environment.NewLine + "Json Body : " + response.ResponseBodyText);
                return null;
            }
        }
        //--------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
