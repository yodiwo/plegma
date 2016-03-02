using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Yodiwo.NodeLibrary;

namespace Yodiwo.Node.Pairing
{
    public interface IPairingModule
    {
        bool StartPair(string postUrl, string redirectUri, NodeConfig conf, string selfUrl, NodePairingBackend.OnPairedDelegate onpairedcb, NodePairingBackend.OnPairingFailedDelegate OnPairingFailedCb);
        void EndPair();
    }


    public class CommonDevicePairingPolling : IPairingModule
    {
        private NodeConfig conf;
        public bool StartPair(string postUrl, string redirectUri, NodeConfig conf, string selfUrl, NodePairingBackend.OnPairedDelegate OnPairedcb, NodePairingBackend.OnPairingFailedDelegate OnPairingFailedCB)
        {
            this.conf = conf;

            //pairing backend
            NodePairingBackend backend = new NodePairingBackend(postUrl, conf, OnPairedcb, OnPairingFailedCB);
            string token2 = backend.pairGetTokens(redirectUri);
            var uri = backend.userUrl + "?token2=" + token2;
#if NETFX
            Process.Start(uri);
#else
            Windows.System.Launcher.LaunchUriAsync(new Uri(uri)).AsTask();
#endif
            //wait for finish
            Task.Delay(1000).Wait();
            Task.Run(() =>
            {
                while (true)
                {
                    var keys = backend.pairGetKeys();
                    if (keys != null)
                        break;
                    Task.Delay(1000).Wait();
                }
            });
            return true;
        }

        public void EndPair()
        {
        }
    }
}
