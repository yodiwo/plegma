using System.Collections.Generic;
using Yodiwo.API.Spike;

namespace Yodiwo.Spike
{
    public interface ISpikePersistentConfigurator
    {
        Dictionary<uint, List<SpikeConfig>> GetSpikeNodeConfig(string spikeNodeId);

        void DeleteSpikeNodeConfig(string spikeNodeId);

        void SaveConfigs();
    }
}