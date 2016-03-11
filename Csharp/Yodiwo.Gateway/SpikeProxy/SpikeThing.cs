using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Spike;
using Yodiwo.Logic.Blocks.A2MCU;

namespace Yodiwo.Spike
{
    public class SpikePort
    {
        public byte PortId;
        public Yodiwo.API.Plegma.ioPortDirection Direction;
        public eSpikeValueTypes Type;

        public static readonly Dictionary<eSpikeValueTypes, Yodiwo.API.Plegma.ePortType> PortTypesFromSpike;
        public static readonly Dictionary<eSpikeValueTypes, Type> SpikeTypeTypes;

        static SpikePort()
        {
            PortTypesFromSpike = new Dictionary<eSpikeValueTypes, ePortType>
            {
                { eSpikeValueTypes.BOOLEAN, ePortType.Boolean },
                { eSpikeValueTypes.UINT8, ePortType.Integer },
                { eSpikeValueTypes.UINT16, ePortType.Integer },
                { eSpikeValueTypes.UINT32, ePortType.Integer },
                { eSpikeValueTypes.UINT64, ePortType.Integer },
                { eSpikeValueTypes.INT8, ePortType.Integer },
                { eSpikeValueTypes.INT16, ePortType.Integer },
                { eSpikeValueTypes.INT32, ePortType.Integer },
                { eSpikeValueTypes.INT64, ePortType.Integer },
                { eSpikeValueTypes.FLOAT, ePortType.Decimal },
                { eSpikeValueTypes.DOUBLE, ePortType.DecimalHigh },
                { eSpikeValueTypes.BLOB, ePortType.String },//TODO check type
                { eSpikeValueTypes.STRING, ePortType.String },
                { eSpikeValueTypes.NTSTRING, ePortType.String },
                { eSpikeValueTypes.I2C, ePortType.String },
            };

            SpikeTypeTypes = new Dictionary<eSpikeValueTypes, Type>
            {
                { eSpikeValueTypes.BOOLEAN, typeof(bool) },
                { eSpikeValueTypes.UINT8, typeof(byte) },
                { eSpikeValueTypes.UINT16, typeof(UInt16) },
                { eSpikeValueTypes.UINT32, typeof(UInt32) },
                { eSpikeValueTypes.UINT64, typeof(UInt64) },
                { eSpikeValueTypes.INT8,  typeof(sbyte) },
                { eSpikeValueTypes.INT16, typeof(Int16) },
                { eSpikeValueTypes.INT32, typeof(Int32) },
                { eSpikeValueTypes.INT64, typeof(Int64) },
                { eSpikeValueTypes.FLOAT,typeof(float)  },
                { eSpikeValueTypes.DOUBLE, typeof(double)  },
                { eSpikeValueTypes.BLOB, typeof(string)  },//TODO check type
                { eSpikeValueTypes.STRING, typeof(string)  }, //TODO
                { eSpikeValueTypes.NTSTRING, typeof(string)  },//TODO
                { eSpikeValueTypes.I2C, typeof(I2CCommand) },//TODO
            };
        }

        public static object ConvertToNativeBySpikePortType(object value, eSpikeValueTypes spikeType)
        {
            ePortType plegmaType;
            Type type;
            if (PortTypesFromSpike.TryGetValue(spikeType, out plegmaType))
            {
                if (PortConfiguration.PortTypeDict.TryGetValue(plegmaType, out type))
                {
                    return ConvertEx.Convert(value, type);
                }
            }
            return null;
        }

        public static object ConvertToSpikeType(object value, eSpikeValueTypes spikeType)
        {
            Type type;
            if (SpikeTypeTypes.TryGetValue(spikeType, out type))
            {
                return ConvertEx.Convert(value, type);
            }
            return null;
        }

    }

    public class SpikeThing
    {
        public uint SpikeThingId;
        public eSpikeThingType Type;
        public eSpikeThingFlags Flags;
        public SpikeConfig[] Configs;
        public SpikePort[] Ports;

        public DictionaryTS<A2mcuActiveDriver, bool> ConnectedDrivers = new DictionaryTS<A2mcuActiveDriver, bool>();

        public SpikeConfig GetConfig(string name)
        {
            return Configs.FirstOrDefault(c => c.Name == name);
        }

        public bool UpdateConfig(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            var conf = Configs.FirstOrDefault(x => x.Name == name);
            if (conf == null)
                return false;
            //convert to check
            var native = SpikePort.ConvertToNativeBySpikePortType(value, conf.Type);
            if (native == null)
                return false;
            if (conf.Value == value)
                return false;
            conf.Value = value;
            return true;
        }


        public static ThingKey MakeThingKey(SpikeThing sthing, NodeKey nodeKey, string spikeNodeName)
        {
            return new ThingKey(nodeKey, $"{spikeNodeName}-{sthing.SpikeThingId}");
        }

        public static string GetIconPathForType(eSpikeThingType type)
        {
            switch (type)
            {
                case eSpikeThingType.Invalid:
                    break;
                case eSpikeThingType.Gpio:
                    return "/Content/Designer/img/BlockImages/icon-gpio.png";
                case eSpikeThingType.Spi:
                    break;
                case eSpikeThingType.Sdio:
                    break;
                case eSpikeThingType.I2C:
                    return "/Content/Designer/img/BlockImages/icon-i2c.svg";
                case eSpikeThingType.I2S:
                    break;
                case eSpikeThingType.Usb:
                    break;
                case eSpikeThingType.Adc:
                    break;
                case eSpikeThingType.Dac:
                    break;
                case eSpikeThingType.Conf:
                    break;
                default:
                    break;
            }
            return null;
        }

        public static Thing ThingFromSpikeThing(SpikeThing sthing, string spikeNodeName, int spikeSubNodeId)
        {

            if (sthing == null)
                return null;
            //            var thingKey = MakeThingKey(sthing, nodeKey, spikeNodeName);
            var ports = sthing.Ports.Select(p =>
                new Port
                {
                    //TODO rest of fields
                    Description = null,
                    Name = p.PortId.ToString(),
                    Type = SpikePort.PortTypesFromSpike[p.Type],
                    ConfFlags = ePortConf.PropagateAllEvents,
                    PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", p.PortId.ToString()),
                    RevNum = 0, //TODO
                    State = "",
                    ioDirection = p.Direction,
                }).ToList();

            var t = new Thing
            {
                Name = $"{sthing.Type}-{sthing.SpikeThingId}", //TODO
                Type = null,
                Config = sthing.Configs?.Select(c => new ConfigParameter
                {
                    Name = c.Name,
                    Value = c.Value.ToString(), //TODO not exactly
                }).ToList(),
                Ports = ports,
                ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", spikeSubNodeId, sthing.SpikeThingId),
                BlockType = null,
                Removable = (sthing.Flags & eSpikeThingFlags.Removable) > 0,
                UIHints = new ThingUIHints
                {
                    Description = $"{spikeNodeName} {sthing.Type.ToString()}, ID {sthing.SpikeThingId}",
                    IconURI = GetIconPathForType(sthing.Type),
                },
            };
            return t;
        }

        public void InitDrivers()
        {
            foreach (var driver in ConnectedDrivers.Keys)
            {
                InitDriver(driver);
            }
        }

        public void InitDriver(A2mcuActiveDriver driver)
        {
            if (!ConnectedDrivers.ContainsKey(driver))
            {
                DebugEx.Assert("This driver is not added to this spikething");
                return;
            }
            foreach (var conc in driver.Init.Seq)
            {
                var tlvs = new List<SpikeContainerTlv>();
                if (conc is A2mcuConcurrentCommands)
                {
                    foreach (var ctrl in (conc as A2mcuConcurrentCommands).CtrlMsgs)
                    {
                        tlvs.AddRange(HandleCtrlMsg(ctrl));
                    }
                }
                else if (conc is A2mcuCtrl)
                    tlvs.AddRange(HandleCtrlMsg((conc as A2mcuCtrl)));

            }
            ConnectedDrivers[driver] = true;
        }

        public IEnumerable<SpikeContainerTlv> HandleCtrlMsg(A2mcuCtrl ctrl)
        {
            var tlvs = new List<SpikeContainerTlv>();
            switch (ctrl.Type)
            {
                case eA2mcuCtrlType.Reset:
                    break;
                case eA2mcuCtrlType.SetValue:
                    break;
                case eA2mcuCtrlType.WriteDriconf:
                    tlvs.AddRange(HandleWriteDriconf(ctrl.Data as SpikeDriconf));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return tlvs;
        }

        public IEnumerable<SpikeContainerTlv> HandleWriteDriconf(SpikeDriconf driconf)
        {
            //TODO driconf validation, internal processing, etc...
            var tlv = SpikeContainerTlv.MakeWriteDriconf(SpikeThingId, driconf);
            return new List<SpikeContainerTlv>
            {
                tlv,
            };
        }

    }
}
