using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Yodiwo.NodeLibrary;

namespace Yodiwo.NodeLibrary.Pairing.NancyPairing
{
    public class NancyPairing : IPairingModule
    {
        NancyHost host;
        private NodeConfig conf;

        public bool StartPair(string frontendUrl, string redirectUrl, NodeConfig conf, string selfUrl, NodePairingBackend.OnPairedDelegate OnPairedcb, NodePairingBackend.OnPairingFailedDelegate OnPairingFailedCb)
        {
            lock (this)
            {
                //pairing already in progress
                if (host != null || NancyPairingModule.backend != null)
                    return false;

                //sanity checks
                if (string.IsNullOrWhiteSpace(selfUrl))
                    return false;

                this.conf = conf;

                //pairing backend            
                NancyPairingModule.backend = new Yodiwo.NodeLibrary.Pairing.NodePairingBackend(frontendUrl, conf, OnPairedcb, OnPairingFailedCb);
                startHttpServer(selfUrl);
                return true;
            }
        }

        public void startHttpServer(string url)
        {
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
            Task.Run(() =>
            {
                //wait for a while to end service any requests (like the success page)
                Task.Delay(3000).Wait();
                //close it
                lock (this)
                {
                    if (host != null)
                        host.Stop();

                    host = null;
                    NancyPairingModule.backend = null;
                }
            });
        }
    }
}
