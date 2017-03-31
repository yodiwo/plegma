using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.mNode.Plugins.Bridge_ZWave
{

    public static class Config
    {
        #region Init function
        //------------------------------------------------------------------------------------------------------------------------
        public static YConfig<ZwavePluginConfig> Init(string plugindir)
        {
            var cfgFile = Path.Combine(plugindir, "conf_file.json");
            var yconfig = new YConfig<ZwavePluginConfig>(cfgFile);
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
            ZwavePluginConfig cfg = new ZwavePluginConfig();

            cfg.SerialPort = "/dev/ttyAMA0";
            //add new active conf and save to disk
            yconfig.AddActiveConf("Default", cfg);
            yconfig.Save();

            return yconfig;
        }
        #endregion

    }
    //------------------------------------------------------------------------------------------------------------------------

    #region ZWaveConfig Class

    //------------------------------------------------------------------------------------------------------------------------
    public class ZwavePluginConfig : IYConfigEntry
    {
        public string SerialPort;
    }
    //------------------------------------------------------------------------------------------------------------------------
    #endregion
}

