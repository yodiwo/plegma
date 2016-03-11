using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Spike;

namespace Yodiwo.Spike
{
    public class Spike2Cloud
    {
        public int SubNodeId;
        public uint ThingId;
        public uint PortId;
        public string State;
    }

    public class SpikeNode
    {
        //public DictionaryTS<SpikeThing, Thing> Things;
        public HashSet<SpikeThing> SpikeThings = new HashSet<SpikeThing>();

        public DictionaryTS<SpikeThing, bool> ThingActiveStatuses;

        private SpikeMessage _msg2Spike;

        //private List<PortEvent> _msg2Cloud;

        private List<Spike2Cloud> _msg2Cloud;

        public string NodeName;
        public string SpikeNodeName;

        public int SubNodeId;

        public delegate void SpikeMessageSenderDelegate(byte[] msg, int offset, int length);

        public SpikeMessageSenderDelegate SpikeMessageSender = delegate { };

        public delegate void PortEventMsgSenderDelegate(IEnumerable<Spike2Cloud> data);

        public PortEventMsgSenderDelegate Spike2CloudSender = delegate { };

        private readonly ISpikePersistentConfigurator _configurator;

        private readonly Dictionary<uint, List<SpikeConfig>> _spikeConfigs;

        public SpikeNode(IEnumerable<SpikeThing> sthings, string spikeNodeName, int spikeSubNodeId, ISpikePersistentConfigurator configurator)
        {
            SpikeNodeName = spikeNodeName;
            SubNodeId = spikeSubNodeId;
            //Things = sthings.Select(t => new KeyValuePair<SpikeThing, Thing>(t, SpikeThing.ThingFromSpikeThing(t, spikeNodeName, spikeSubNodeId))).ToDictionaryTS();
            SpikeThings.UnionWith(sthings);
            ThingActiveStatuses = SpikeThings.Select(t => new KeyValuePair<SpikeThing, bool>(t, false)).ToDictionaryTS();
            _msg2Cloud = new List<Spike2Cloud>();
            _msg2Spike = new SpikeMessage();
            _configurator = configurator;
            //would we rather identify spikenodes by nodekey + spikenodeid?
            //reasons for no: this function (initialization of spike nodes) does not necessarily happen after pairing
            // we also might want to keep some configs if nodekey changes (unpair / pair gateway)
            _spikeConfigs = _configurator.GetSpikeNodeConfig(spikeNodeName);
            if (_spikeConfigs.Count == 0)
            {
                foreach (var st in SpikeThings)
                {

                    _spikeConfigs[st.SpikeThingId] = st.Configs?.ToList();
                }
                _configurator.SaveConfigs();
            }
            RestoreSpikeConfigs();
        }

        #region generic thing management

        public SpikeThing GetSpikeThing(UInt32 thingId)
        {
            return SpikeThings.FirstOrDefault(t => t.SpikeThingId == thingId);
        }

        public IEnumerable<Thing> GenerateThings()
        {
            return SpikeThings.Select(st => SpikeThing.ThingFromSpikeThing(st, NodeName, SubNodeId));
        }
        #endregion

        public void OnChangedState(uint spikeThingId, uint portId, string state, ePortType portType)
        {
            var sthing = GetSpikeThing(spikeThingId);
            if (sthing == null)
            {
                DebugEx.Assert("thing does not belong to this Spike node, or port does not belong to the thing...");
                return;
            }
            if (_msg2Spike == null)
                _msg2Spike = new SpikeMessage();
            _msg2Spike.Tlvs.Add(SpikeContainerTlv.MakeSetValueAuto(sthing, (byte)portId, state, portType));
        }

        public void DecodeTlvPayloads(SpikeMessage spike)
        {
            if (spike == null)
                return;
            foreach (var tlv in spike.Tlvs)
            {
                switch (tlv.Header.Type)
                {
                    case TlvTypes.GetValue:
                    case TlvTypes.ReadConf:
                    case TlvTypes.WriteConf:
                        throw new NotImplementedException("not yet, please come back later");
                    case TlvTypes.SetValue:
                        var setValHead = (SpikeTlvSetValueHeader)tlv.StdPayload;
                        var spikeThing = GetSpikeThing(setValHead.ThingId);
                        if (spikeThing == null || spikeThing.Ports.Length <= setValHead.PortId)
                        {
                            DebugEx.Assert("Invalid Spike thing or port");
                            continue;
                        }
                        var type = spikeThing.Ports[setValHead.PortId].Type;
                        tlv.VarPayload = SpikeContainerTlv.GetSetValuePayloadFromBytes(tlv.VarPayloadBytes, 0, tlv.VarPayloadBytes.Length, type);
                        tlv.VarPayloadBytes = null;
                        break;
                    case TlvTypes.Reserved:
                    default:
                        DebugEx.Assert("unexpected Tlv type");
                        break;
                }

            }
        }

        private string ConvertValue2Cloud(object value, eSpikeValueTypes type)
        {
            string converted = null;
            switch (type)
            {
                case eSpikeValueTypes.BOOLEAN:
                    converted = ((bool)value).ToString();
                    break;
                case eSpikeValueTypes.UINT8:
                    break;
                case eSpikeValueTypes.UINT16:
                    break;
                case eSpikeValueTypes.UINT32:
                    break;
                case eSpikeValueTypes.UINT64:
                    break;
                case eSpikeValueTypes.INT8:
                    break;
                case eSpikeValueTypes.INT16:
                    break;
                case eSpikeValueTypes.INT32:
                    break;
                case eSpikeValueTypes.INT64:
                    break;
                case eSpikeValueTypes.FLOAT:
                    break;
                case eSpikeValueTypes.DOUBLE:
                    break;
                case eSpikeValueTypes.BLOB:
                    break;
                case eSpikeValueTypes.STRING:
                    break;
                case eSpikeValueTypes.NTSTRING:
                    break;
                case eSpikeValueTypes.I2C:
                    var spikeI2c = (SpikeContainerI2c)value;
                    var com = new Logic.Blocks.A2MCU.I2CCommand
                    {
                        devaddress = spikeI2c.Header.DeviceAddress,
                        register = spikeI2c.Header.RegisterAddress,
                        IsWrite = true,
                        value = spikeI2c.Data,
                    };
                    converted = com.ToJSON();
                    break;
                default:
                    break;
            }
            return converted;

        }

        public void SendSpikeMessage()
        {
            if (SpikeMessageSender == null || _msg2Spike == null)
                return;

            var bytes = _msg2Spike.GetBytes();

            SpikeMessageSender(bytes, 0, bytes.Length);
            _msg2Spike = new SpikeMessage();
        }

        #region incoming spike handling

        public void HandleSpikeMessage(byte[] msg)
        {
            var spike = SpikeMessage.FromBytes(msg);
            DecodeTlvPayloads(spike);
            HandleSpikeMessage(spike);
        }

        public void HandleSpikeMessage(SpikeMessage spike)
        {
            DebugEx.TraceLog("Got Spike message");
            if (spike == null)
                return;
            foreach (var tlv in spike.Tlvs)
            {
                HandleTlv(tlv);
            }
            HandleSpikeMessageEnd();
        }

        public void HandleTlv(SpikeContainerTlv tlv)
        {
            switch (tlv.Header.Type)
            {
                case TlvTypes.GetValue:
                case TlvTypes.ReadConf:
                case TlvTypes.WriteConf:
                    throw new NotImplementedException("not yet, please come back later");
                case TlvTypes.SetValue:
                    HandleSetValue(tlv);
                    break;
                case TlvTypes.Reserved:
                default:
                    DebugEx.Assert("unexpected Tlv type");
                    break;

            }
        }

        public bool HandleSetValue(SpikeContainerTlv tlv)
        {
            DebugEx.TraceLog("HandleSetValue");
            var setValHead = (SpikeTlvSetValueHeader)tlv.StdPayload;
            var spikePortId = setValHead.PortId;
            var spikeThing = GetSpikeThing(setValHead.ThingId);
            if (spikeThing == null || spikeThing.Ports.Length <= spikePortId)
            {
                DebugEx.Assert("Invalid Spike thing or port");
                return false;
            }
            var plegmaVal = ConvertValue2Cloud(tlv.VarPayload, spikeThing.Ports[spikePortId].Type);
            if (_msg2Cloud == null)
                _msg2Cloud = new List<Spike2Cloud>();
            //TODO validation? maybe before this function, check it
            _msg2Cloud.Add(new Spike2Cloud
            {
                SubNodeId = SubNodeId,
                ThingId = setValHead.ThingId,
                PortId = spikePortId,
                State = plegmaVal,
            })
            ;
            return true;
        }

        private void HandleSpikeMessageEnd()
        {
            if (_msg2Cloud != null && _msg2Cloud.Count > 0)
            {
                Spike2CloudSender(_msg2Cloud);
                _msg2Cloud = new List<Spike2Cloud>();
            }

        }

        #endregion

        #region thing activation
        public void OnThingActivated(uint spikeThingId)
        {
            OnThingActivationStatusChange(spikeThingId, true);
        }

        public void OnThingDeactivated(uint spikeThingId)
        {
            OnThingActivationStatusChange(spikeThingId, false);
        }

        private void OnThingActivationStatusChange(uint spikeThingId, bool activated)
        {
            var sthing = GetSpikeThing(spikeThingId);
            DebugEx.TraceLog($"SpikeThing with id:{sthing.SpikeThingId} activation status changed, now: {activated}");
            if (sthing != null)
            {
                ThingActiveStatuses[sthing] = activated;
                SendThingActivationState(sthing, activated);
            }
        }

        private void SendThingActivationState(SpikeThing sthing, bool activated)
        {
            SpikeContainerTlv.MakeActivationStatus(sthing.SpikeThingId, activated);
            SendSpikeMessage();
        }
        #endregion

        #region thing configuration
        public void OnSpikeThingUpdated(uint spikeThingId, IEnumerable<ConfigParameter> confs)
        {
            var spikeThing = GetSpikeThing(spikeThingId);
            if (spikeThing == null)
            {
                DebugEx.Assert("spikething not found");
                return;
            }
            foreach (var conf in confs)
            {
                if (!spikeThing.UpdateConfig(conf.Name, conf.Value)) continue;
                var spikeConf = spikeThing.GetConfig(conf.Name);
                if (spikeConf == null)
                    continue;

                if (_msg2Spike == null)
                    _msg2Spike = new SpikeMessage();
                UpdatePersistentSpikeConfig(spikeThing.SpikeThingId, spikeConf);
                _msg2Spike.Tlvs.Add(SpikeContainerTlv.MakeWriteConfig(spikeThing.SpikeThingId, spikeConf));
            }
            SendSpikeMessage();
        }

        private void RestoreSpikeConfigs()
        {
            foreach (var cfgsOfThing in _spikeConfigs)
            {
                var sthing = GetSpikeThing(cfgsOfThing.Key);
                if (sthing == null || cfgsOfThing.Value == null)
                    continue;
                foreach (var conf in cfgsOfThing.Value)
                {
                    var confInThing = sthing.Configs.FirstOrDefault(x => x.Name == conf.Name);

                    if (confInThing != null)
                    {
                        //type should not change
                        confInThing.Type = conf.Type;
                        confInThing.Value = conf.Value;
                    }

                }
            }
        }

        private void UpdatePersistentSpikeConfig(uint spikeThingId, SpikeConfig config)
        {
            List<SpikeConfig> confList;
            if (_spikeConfigs.TryGetValue(spikeThingId, out confList))
            {
                // for new stuff, just add them
                if (confList == null)
                    confList = new List<SpikeConfig>();
                var persistentConf = confList.FirstOrDefault(x => x.Name == config.Name);

                if (persistentConf != null)
                {
                    //type should not change
                    //persistentConf.Type = config.Type;
                    persistentConf.Value = config.Value;
                }
                else
                {
                    confList.Add(config);
                }
                _configurator.SaveConfigs();
            }
        }

        public void SyncWithSerialSpikeNode()
        {
            DebugEx.TraceLog("Connected to SpikeNode, syncing Configs");
            foreach (var sthing in SpikeThings)
            {
                if (sthing.Configs == null)
                    continue;
                foreach (var cfg in sthing.Configs)
                {
                    _msg2Spike.Tlvs.Add(SpikeContainerTlv.MakeWriteConfig(sthing.SpikeThingId, cfg));
                }
            }
            foreach (var kv in ThingActiveStatuses)
            {
                SpikeContainerTlv.MakeActivationStatus(kv.Key.SpikeThingId, kv.Value);
            }
            SendSpikeMessage();
        }
        #endregion

        #region driver handling
        public ApiMsg OnA2McuActiveDriversReq(A2mcuActiveDriversReq request)
        {
            foreach (var driver in request.ActiveDrivers)
            {
                var spikeThing = GetThingFromActiveDriver(driver);
                DebugEx.Assert(spikeThing != null, "");
                spikeThing.ConnectedDrivers.TryAdd(driver, false);
                spikeThing.InitDriver(driver);
            }
            SendSpikeMessage();
            return new GenericRsp
            {
                IsSuccess = true,
                Message = "active drivering success",
            };

        }

        //TODO properly get driven things
        // should a driver drive just one thing?
        // for now iterate through ctrl messages and get the first thingkey, this will be the driven thing
        public SpikeThing GetThingFromActiveDriver(A2mcuActiveDriver driver)
        {
            string thingKey = null;
            foreach (var conc in driver.Init.Seq)
            {
                if (conc is A2mcuCtrl)
                    thingKey = (conc as A2mcuCtrl).ThingKey;
                else if (conc is A2mcuConcurrentCommands)
                    thingKey =
                        (conc as A2mcuConcurrentCommands).CtrlMsgs.Select(ctrl => ctrl.ThingKey)
                            .FirstOrDefault(tk => tk != null);
                if (!string.IsNullOrEmpty(thingKey))
                    break;
            }
            DebugEx.Assert(!string.IsNullOrEmpty(thingKey), "BOOMSHAKALAKA");
            //thingkey here ruins our mpougiampesa a little, whatever
            return GetSpikeThing(((ThingKey)thingKey).SpikeThingId);
        }

        #endregion

    }
}
