using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.GatewayMain
{

    public delegate void OnMessageDelegate(byte[] data);

    public interface GatewayLink
    {
        event OnMessageDelegate OnMessage;

        void SendMessage(byte[] bytes, int offset, int length);
    }
}
