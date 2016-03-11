using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Node.Pairing;
using Yodiwo.Node.Pairing.NancyPairing;
using System.Net;
using Yodiwo.API.Plegma;

namespace Yodiwo.Gateway.GatewayMain
{
    public class OpenHubBindingNancyModule : Nancy.NancyModule
    {
        public OpenHubBindingNancyModule()
            : base("/clrest")
        {
            Get["/cmd/{id}/{cmd}"] = x =>
            {
                DebugEx.TraceLog("item " + x.id + " received cmd " + x.cmd);
                return HttpStatusCode.OK;
            };
            Get["/event/{id}/{ev}"] = x =>
            {
                string msgId = x.id;
                string msgContent = x.ev;

                if (msgId != null)
                {
                    string[] idParts = msgId.Split(new string[] { "_" }, StringSplitOptions.None);
                    DebugEx.Assert(idParts.Length >= 4, "unexpected ID: " + msgId);

                    string tmp = null;
                    for (int i = 0; i < idParts.Length - 1; i++)
                    {
                        tmp += idParts[i] + ":";
                    }
                    //cut off the final ':'
                    tmp = tmp.Substring(0, tmp.Length - 1);

                    //string.Join<string>(":", idParts);

                    var channelId = idParts[idParts.Length - 1];
                    var thingKey = new ThingKey(GatewayStatics.ActiveCfg.NodeKey, tmp);

                    var portKey = new PortKey(thingKey, channelId);
                    if (OpenHubHost.OnOpenHubRxCb != null)
                        OpenHubHost.OnOpenHubRxCb(tmp, channelId, msgContent);

                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.MethodNotAllowed;
            };
        }
    }
}
