using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Yodiwo.NodeLibrary.Pairing
{
    public interface IPairingModule
    {
        bool StartPair(string postUrl, string redirectUri, NodeConfig conf, string selfUrl, NodePairingBackend.OnPairedDelegate onpairedcb, NodePairingBackend.OnPairingFailedDelegate OnPairingFailedCb);
        void EndPair();
    }


    public class CommonDevicePairingPolling : IPairingModule
    {
        string pairingURI;
        private NodeConfig conf;
        public Action<string> UriCustomLauncher = null;
        public int SpinDelay = 1000; //in miliseconds

        public bool StartPair(string frontendUrl, string redirectUri, NodeConfig conf, string selfUrl, NodePairingBackend.OnPairedDelegate OnPairedcb, NodePairingBackend.OnPairingFailedDelegate OnPairingFailedCB)
        {
            this.conf = conf;

            //pairing backend
            var backend = new NodePairingBackend(frontendUrl, conf, OnPairedcb, OnPairingFailedCB);
            string token2 = backend.pairGetTokens(redirectUri);
            pairingURI = backend.pairingPostUrl + "/" + API.Plegma.NodePairing.NodePairingConstants.UserConfirmPageURI + "?token2=" + token2;

            if (UriCustomLauncher != null)
                UriCustomLauncher(pairingURI);
            else
            {
#if NETFX
                Process.Start(pairingURI);
#else
                Windows.System.Launcher.LaunchUriAsync(new Uri(pairingURI)).AsTask();
#endif
            }

            //wait for finish
            Task.Delay(1000).Wait();
            Task.Run(() =>
            {
                while (true)
                {
                    var keys = backend.pairGetKeys();
                    if (keys != null)
                        break;
                    Task.Delay(SpinDelay).Wait();
                }
            });
            return true;
        }

        public void EndPair()
        {
        }

        public void RelaunchURI()
        {
#if NETFX
            Process.Start(pairingURI);
#else
            Windows.System.Launcher.LaunchUriAsync(new Uri(pairingURI)).AsTask();
#endif
        }
    }
}
