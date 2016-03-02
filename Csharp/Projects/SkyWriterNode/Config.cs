using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.SkyWriter
{
    public static class Config
    {
        public static YConfig<SkyWriterNodeConfig> Init()
        {
            var cfgFile = "conf_file.json";

            var yconfig = new YConfig<SkyWriterNodeConfig>(cfgFile);

            try
            {
                if (yconfig.Retrieve())
                {
                    Console.WriteLine("Config file found: " + cfgFile);
                    return yconfig;
                }
                Console.WriteLine("Config retrieval failed; falling back to defaults");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loading default configuration");
            }

            //create default conf
            SkyWriterNodeConfig cfg = new SkyWriterNodeConfig();
            cfg.FrontendServer = "https://localhost:3334";
            cfg.ApiServer = "localhost";
            cfg.LocalWebServer = "http://localhost:4050";
            cfg.Uuid = "1337SkyWriter";
            cfg.MqttBrokerHostname = "localhost";
            cfg.YpchannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.MqttUseSsl = false;
            cfg.YpchannelSecure = false;

            //add new active conf and save to disk
            yconfig.AddActiveConf("LocalSkyWriter", cfg);
            yconfig.Save();

            return yconfig;
        }
    }

    public class SkyWriterNodeConfig : IYConfigEntry
    {
        public string webBase;
        public string Uuid;
        public string NodeKey;
        public string NodeSecret;
        public string FrontendServer;
        public string ApiServer;
        public string MqttBrokerHostname;
        public bool MqttUseSsl;
        public int YpchannelPort;
        public bool YpchannelSecure;
        public string CertificateServerName;
        public string LocalWebServer;
        public string MqttApiPasswd;
    }
}
