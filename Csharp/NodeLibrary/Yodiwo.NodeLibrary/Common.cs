using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.NodeLibrary
{
    public class NodeConfig
    {
        public string uuid;
        public string Name;
        public string FrontendServer;
        public string MqttBrokerHostname;
        public bool MqttUseSsl;
        public int YpchannelPort;
        public string YpServer;
        public bool SecureYpc;
        public string MqttApiPasswd;
        public bool CanSolveGraphs;
        public string Description;
        public string Image;
        public string Pathcss;

        public bool EnableNodeDiscovery;
        public int NodeDiscovery_YPCPort_Start; //Port range start
        public int NodeDiscovery_YPCPort_End; //port range end


        public NodeConfig Clone() { return MemberwiseClone() as NodeConfig; }
    }

    [Flags]
    public enum Transport : int
    {
        None = 0,
        YPCHANNEL = 1 << 0,
        MQTT = 1 << 1,
        REST = 1 << 2,
        WEBSOCKET = 1 << 3,
    };


    public enum TransportErrors
    {
        None = 0,
        ConnectionEstablishFailed,
        RxFail,
        TxFail,
        Other,
    }
}
