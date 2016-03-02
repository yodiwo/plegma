using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.NodeLibrary.Transports
{
    public interface ITransportMQTT : ITransport
    {
        SimpleActionResult ConnectWithWorker(string mqttbroker, bool UseSsl);
    }
}
