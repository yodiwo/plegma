using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;
using Yodiwo.API.Spike;

namespace Yodiwo.Spike
{

    public class SpikeThingsConfig : IYConfigEntry
    {
        public Dictionary<string, Dictionary<uint, List<SpikeConfig>>> Configs;

        public Dictionary<DeviceKey, int> AssignedIds;

        internal static YConfig<SpikeThingsConfig> InitConfig()
        {
            var cfgFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/spikethings_config.json";
            var yConfig = new YConfig<SpikeThingsConfig>(cfgFile);

            try
            {
                if (yConfig.Retrieve())
                {
                    return yConfig;
                }
                Console.WriteLine("config retrieve failed; falling back to defaults");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not deserialize configurations ({0}), falling back to defaults", ex.Message);
            }

            //create default conf
            SpikeThingsConfig cfg = new SpikeThingsConfig
            {
                Configs = new Dictionary<string, Dictionary<uint, List<SpikeConfig>>>(),
                AssignedIds = new Dictionary<DeviceKey, int>(),
            };

            //add new active conf and save to disk
            yConfig.AddActiveConf("default", cfg);
            yConfig.Save();

            return yConfig;
        }

    }

}
