using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public static IPAddress GetIPAddress(this EndPoint endpoint)
        {
            if (endpoint is IPEndPoint)
                return ((IPEndPoint)endpoint).Address;
            else
            {
                DebugEx.Assert("Could not parse endpoint");
                return default(IPAddress);
            }
        }
#endif
        //----------------------------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public static int GetPort(this EndPoint endpoint)
        {
            if (endpoint is IPEndPoint)
                return ((IPEndPoint)endpoint).Port;
            else
            {
                DebugEx.Assert("Could not parse endpoint");
                return -1;
            }
        }
#endif
        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
