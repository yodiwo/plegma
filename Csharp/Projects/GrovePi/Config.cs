using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public static class Config
    {
        public static YConfig<GrovePiNodeConfig> Init()
        {
            var cfgFile = "conf_file.json";

            var yconfig = new YConfig<GrovePiNodeConfig>(cfgFile);

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
            GrovePiNodeConfig cfg = new GrovePiNodeConfig();
            cfg.FrontendServer = "https://localhost:3334";
            cfg.ApiServer = "localhost";
            cfg.LocalWebServer = "http://localhost:4040";
            cfg.Uuid = "1337GrovePi";
            cfg.MqttBrokerHostname = "localhost";
            cfg.YpchannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.MqttUseSsl = false;
            cfg.YpchannelSecure = false;

            //add new active conf and save to disk
            yconfig.AddActiveConf("LocalGrovePi", cfg);
            yconfig.Save();

            return yconfig;
        }
    }

    public class GrovePiNodeConfig : IYConfigEntry
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
        public bool CanSolveGraphs;
    }

}
