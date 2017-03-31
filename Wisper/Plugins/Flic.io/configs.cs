using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.mNode.Plugins.Integration_Flic
{
    public static class Config
    {
        public static YConfig<FlicPluginConfig> Init(string plugindir)
        {
            var cfgFile = Path.Combine(plugindir, "conf_file.json");
            var yconfig = new YConfig<FlicPluginConfig>(cfgFile);
            try
            {
                if (yconfig.Retrieve(PreProccessContent: cnt => cnt.Replace("$Role$", "Local")))
                {
                    DebugEx.TraceLog("Config file found: " + cfgFile);
                    return yconfig;
                }
                DebugEx.TraceLog("Config retrieval failed; falling back to defaults");
            }
            catch (Exception ex) { DebugEx.TraceLog("Loading default configuration: " + ex.Message); }


            //create default conf
            FlicPluginConfig cfg = new FlicPluginConfig();

            cfg.PythonServer = "http://localhost:8080/";
            cfg.restrouteremoveflic = "removeflic";
            //add new active conf and save to disk
            yconfig.AddActiveConf("Default", cfg);
            yconfig.Save();

            return yconfig;
        }
    }

    public class FlicPluginConfig : IYConfigEntry
    {
        public string PythonServer;
        public string restrouteremoveflic;
        public string FlicDaemonHostname;
        public int FlicDaemonPort;
    }

}
