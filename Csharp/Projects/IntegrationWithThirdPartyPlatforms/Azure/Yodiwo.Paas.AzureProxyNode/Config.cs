using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.PaaS.AzureProxyNode
{
    public class AzureInfo
    {
        public string AzureDeviceName { get; set; }
        public string AzureIOTApplicationConnectionString { get; set; }
    }

    public class YodiwoInfo
    {
        public string YodiwoNodeKey { get; set; }
        public string YodiwoSecretKey { get; set; }
        public string YodiwoRestUrl { get; set; }
        public string YodiwoApiServer { get; set; }
        public string YPChannelPort { get; set; }
        public bool SecureYPC { get; set; }
    }

    public class ThingDescriptor
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string IO { get; set; }
        public string FriendlyIcon { get; set; }
    }

    public class ThingsDescription
    {
        public List<ThingDescriptor> ThingDescriptor { get; set; }
    }

    public class NodeDescriptor
    {
        public AzureInfo AzureInfo { get; set; }
        public YodiwoInfo YodiwoInfo { get; set; }
        public ThingsDescription ThingsDescription { get; set; }
    }
}
