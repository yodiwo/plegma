using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class Network
    {
        public static string FindLocalIpAddress()
        {
#if NETFX
            try
            {
                //check connection
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    return null;

                //find ethernet
                foreach (var iface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    if (iface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                        if (iface.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet)
                            if (iface.GetIPProperties().GatewayAddresses.Count > 0)
                            {
                                var addrs = iface.GetIPProperties().UnicastAddresses;
                                if (addrs != null)
                                    foreach (var ip in addrs)
                                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                            return ip.Address.ToStringInvariant();
                            }

                //find wifi
                foreach (var iface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    if (iface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                        if (iface.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211)
                            if (iface.GetIPProperties().GatewayAddresses.Count > 0)
                            {
                                var addrs = iface.GetIPProperties().UnicastAddresses;
                                if (addrs != null)
                                    foreach (var ip in addrs)
                                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                            return ip.Address.ToStringInvariant();
                            }

                //get a valid address
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip.ToString();
            }
            catch (Exception ex) { DebugEx.TraceWarning("Could not find internal ip (" + ex.Message + ")"); }
#endif
            //not found
            return "127.0.0.1";
        }

    }
}
