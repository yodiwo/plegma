using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Node.Pairing;
using Yodiwo.NodeLibrary;
using Nancy.Hosting.Self;
using Yodiwo.Node.Pairing.NancyPairing;

namespace Yodiwo.Gateway.GatewayMain
{
    public class OpenHubHost : IPairingModule
    {
        NancyHost host;
        private NodeConfig conf;
        public delegate void OnOpenHubRx(string thinguid, string channelid, string state);
        public static OnOpenHubRx OnOpenHubRxCb = null;

        public bool StartPair(string frontendUrl, string redirectUrl, NodeConfig conf, string selfUrl, NodePairingBackend.OnPairedDelegate OnPairedcb, NodePairingBackend.OnPairingFailedDelegate OnPairingFailedCb)
        {
            lock (this)
            {
                //pairing already in progress
                if (NancyPairingModule.backend != null)
                    return false;
                this.conf = conf;
                //pairing backend            
                NancyPairingModule.backend = new Yodiwo.Node.Pairing.NodePairingBackend(frontendUrl, conf, OnPairedcb, OnPairingFailedCb);
                //startHttpServer(selfUrl);
                return true;
            }
        }

        public void startHttpServer(string url)
        {
            //check nancy host
            if (host != null)
                return;
            //sanity check
            if (string.IsNullOrWhiteSpace(url))
                return;

            Nancy.StaticConfiguration.DisableErrorTraces = false;
            var uri = new Uri(url);

            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;

            host = new NancyHost(hostConfigs, uri);
            DebugEx.TraceLog("Pairing server runs on " + uri);
            host.Start();
        }

        public void EndPair()
        {
            DebugEx.TraceLog("Nancy host remains alive for getting messages from the openhub bus");
        }
    }
}
