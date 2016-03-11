using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Yodiwo.API.Plegma.Private;

namespace Yodiwo.Gateway.GatewayMain
{
    public static class ClientYPChannel
    {
        public static void Initialize()
        {
            //create protocol
            var proto = new YPChannel.Protocol()
            {
                Version = API.Plegma.PlegmaAPI.APIVersion,
                ProtocolDefinitions = new List<YPChannel.Protocol.MessageTypeGroup>()
                    {
                        new YPChannel.Protocol.MessageTypeGroup() { MessageTypes = Yodiwo.API.Plegma.PlegmaAPI.ApiMessages },
                        new YPChannel.Protocol.MessageTypeGroup() { MessageTypes = Yodiwo.API.Plegma.PlegmaAPI.LogicApiMessages },
                        new YPChannel.Protocol.MessageTypeGroup() { MessageTypes = Yodiwo.API.Plegma.Private.PlegmaPlusAPI.ApiMessages }
                    },
            };

            //start the client connection towards the configured Worker instance
            GatewayStatics.client = new Yodiwo.YPChannel.Transport.Sockets.Client(proto);

            GatewayStatics.client.OnMessageReceived += MsgHandler.OnSrvMsgReceived;

            GatewayStatics.client.OnOpenEvent += (sender) =>
                {
                    var cli = sender as Yodiwo.YPChannel.Transport.Sockets.Client;
                    DebugEx.TraceLog("client " + cli + " connected");
                };

            GatewayStatics.client.OnClosedEvent += (sender) =>
                {
                    sender.Close();
                    GatewayStatics.client = null;
                    //NOTES: 
                    //1. remove nulling of static client if more than channels per gateway are allowed
                    //2. check if reconnection attempt(s) should be made
                };

            GatewayStatics.client.Connect(GatewayStatics.active_conf.YPChannelServer, GatewayStatics.active_conf.YPChannelPort, GatewayStatics.active_conf.YPChannelSecure);
        }

    }
}
