using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Yodiwo.Gateway.GatewayMain
{
    public class GatewayStatics
    {
        public static GwConfig ActiveCfg;
        public static YConfig<GwConfig> YConfig;

        public static YConfig<GwConfig> Init()
        {
            var cfgFile = "conf_file.json";

            var yconfig = new YConfig<GwConfig>(cfgFile);

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
            GwConfig cfg = new GwConfig();
            cfg.FrontendServer = "https://localhost:3334";
            cfg.ApiServer = "localhost";
            cfg.LocalWebServer = "http://localhost:8081";
            cfg.Uuid = "1337RasPiCamera";
            cfg.YPChannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.YpchannelSecure = false;

            //add new active conf and save to disk
            yconfig.AddActiveConf("LocalGateway", cfg);
            yconfig.Save();

            return yconfig;
        }
    }


    public class GwConfig : IYConfigEntry
    {

        public string Uuid;
        public string NodeKey;
        public string NodeSecret;
        public string FrontendServer;
        public string ApiServer;
        public bool YpchannelSecure;
        public int YPChannelPort;
        public string CertificateServerName;
        public string LocalWebServer;
        public string OpenhabBaseUrl;

        public string SpikeSerialNodePort;
    }
}