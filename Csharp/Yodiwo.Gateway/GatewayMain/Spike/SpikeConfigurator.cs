using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;
using Yodiwo.API.Spike;
using Yodiwo.Spike;

namespace Yodiwo.Gateway.GatewayMain.Spike
{
    class SpikeConfigurator : ISpikePersistentConfigurator
    {
        private YConfig<SpikeThingsConfig> _yConfig;
        public SpikeThingsConfig ActiveConfig;

        private SpikeConfigurator()
        {
        }

        public static SpikeConfigurator InitConfigurator()
        {
            var conf = new SpikeConfigurator
            {
                _yConfig = SpikeThingsConfig.InitConfig()
            };
            conf.ActiveConfig = conf._yConfig.GetActiveConf();
            return conf;
        }


        public void DeleteSpikeNodeConfig(string spikeNodeId)
        {
            ActiveConfig.Configs.Remove(spikeNodeId);
        }

        public Dictionary<uint, List<SpikeConfig>> GetSpikeNodeConfig(string spikeNodeId)
        {
            Dictionary<uint, List<SpikeConfig>> d;
            if (!ActiveConfig.Configs.TryGetValue(spikeNodeId, out d))
            {
                d = new Dictionary<uint, List<SpikeConfig>>();
                ActiveConfig.Configs[spikeNodeId] = d;
            }
            return d;
        }

        public void SaveConfigs()
        {
            _yConfig.Save();
        }
    }
}
