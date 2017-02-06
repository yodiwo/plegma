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


        public static bool ValidateIpv4Address(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            var tokens = ip.Split(new[] { '.' });
            if (tokens.Length != 4)
                return false;

            foreach (var t in tokens)
            {
                var num = t.ParseToInt32(-1);
                if (!num.isBetweenValues(0, 255))
                    return false;
            }
            return true;
        }

        public static int Netmask2Prefix(string netmask)
        {
            if (string.IsNullOrEmpty(netmask))
                return -1;
            var tokens = netmask.Split('.');
            if (tokens.Length != 4)
                return -1;
            var prefix = 0;
            var zeros = 0;
            foreach (var t in tokens)
            {
                var num = t.ParseToInt32(-1);
                if (!num.isBetweenValues(0, 255))
                    return -1;
                while (num > 0)
                {
                    num *= 2;
                    if (num >= 256)
                    {
                        if (zeros > 0)
                        {
                            //got ace after zero, netmask invalid
                            return -1;
                        }
                        prefix++;
                    }
                    else
                    {
                        zeros++;
                    }
                    num &= 0xff;
                }
            }
            return prefix;
        }

        public static string Prefix2Netmask(int prefix)
        {
            if (prefix < 0 || prefix > 32)
                return null;
            var rem = prefix;
            var netmask = "";
            for (var i = 0; i < 4; i++)
            {
                var field = 0;
                for (var j = 0; j < 8; j++)
                {
                    field *= 2;
                    if (rem > 0)
                    {
                        field++;
                        rem--;
                    }
                }
                netmask += field;
                if (i < 3)
                    netmask += ".";
            }
            return netmask;
        }
    }
}
