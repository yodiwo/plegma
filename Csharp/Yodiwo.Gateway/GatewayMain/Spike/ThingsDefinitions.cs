using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Spike;
using Yodiwo.Spike;

namespace Yodiwo.Gateway.GatewayMain.Spike
{
    public class SampleSpikeThingDefinitions
    {
        #region Variables

        public const string I2C0MasterName = "I2C0Master";
        //------------------------------------------------------------------------------------------------------------------------
        public static uint GpioThingsNum = 5;
        //public static int GpioThingsNum = 16;
        public static List<SpikeThing> GpiSpikeThings;
        public static List<SpikeThing> GpoSpikeThings;
        public static Thing I2C0MasterThing;
        public static SpikeThing I2C0MasterSpikeThing;
        
        public static readonly Dictionary<Thing, string> Things = GatherThings();
        public static readonly List<SpikeThing> SpikeThings = GatherSpikeThings(); 
        
        #endregion
        
        //------------------------------------------------------------------------------------------------------------------------
        //initialize node's things
        private static Dictionary<Thing, string> GatherThings()
        {
            var things = new Dictionary<Thing, string>();
            //setup I2C 0 Master thing

            #region Setup I2C 0 Master thing

            {
                var thing = I2C0MasterThing = new Thing
                {
                    Name = "I2C0Master",
                    Config = null,
                    UIHints = new ThingUIHints
                    {
                        IconURI = "http://upload.wikimedia.org/wikipedia/en/9/9f/I%C2%B2C_bus_logo.svg",
                    },
                };
                thing.Ports = new List<Port>
                    {
                    new Port
                    {
                        ioDirection = ioPortDirection.InputOutput,
                        Name = "Slave Address",
                        State = "",
                        Type = ePortType.Integer,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    },
                    new Port
                    {
                        ioDirection = ioPortDirection.InputOutput,
                        Name = "Register",
                        State = "",
                        Type = ePortType.Integer,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "1")
                    },
                    new Port
                    {
                        ioDirection = ioPortDirection.InputOutput,
                        Name = "Data",
                        State = "",
                        Type = ePortType.String,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "2")
                    },
                    new Port
                    {
                        ioDirection = ioPortDirection.InputOutput,
                        Name = "Trigger",
                        State = "",
                        Type = ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "3")
                    },
                };
            }

            #endregion

            things[I2C0MasterThing] = I2C0MasterName;

            return things;
        }

        private static List<SpikeThing> GatherSpikeThings()
        {
            GpiSpikeThings = new List<SpikeThing>();
            for (uint i = 0; i < GpioThingsNum; i++)
            {
                GpiSpikeThings.Add(new SpikeThing
                {
                    Type = eSpikeThingType.Gpio,
                    Flags = eSpikeThingFlags.None,
                    SpikeThingId = (uint)i,
                    Configs = new[]
                    {
                        new SpikeConfig
                        {
                            Name = A2mcuConfigConstants.ConfigNameSamplingPeriod,
                            Type = eSpikeValueTypes.INT32,
                            Value = "500",
                        }
                    },
                    Ports = new[]
                    {
                        new SpikePort
                        {
                            PortId = 0,
                            Type = eSpikeValueTypes.BOOLEAN,
                            Direction = ioPortDirection.Output,
                        },
                    }
                });
            }

            GpoSpikeThings = new List<SpikeThing>();
            for (uint i = 0; i < GpioThingsNum; i++)
            {
                GpoSpikeThings.Add(new SpikeThing
                {
                    Type = eSpikeThingType.Gpio,
                    Flags = eSpikeThingFlags.None,
                    SpikeThingId = (uint)GpioThingsNum + i,
                    Configs = null,
                    Ports = new[]
                    {
                        new SpikePort
                        {
                            PortId = 0,
                            Type = eSpikeValueTypes.BOOLEAN,
                            Direction = ioPortDirection.Input,
                        },
                    }
                });
            }


            I2C0MasterSpikeThing = new SpikeThing
            {
                Type = eSpikeThingType.I2C,
                Flags = eSpikeThingFlags.None,
                SpikeThingId = (uint)GpioThingsNum * 2,
                Configs = new[]
                {
                    new SpikeConfig
                    {
                        Name = A2mcuConfigConstants.ConfigNameSamplingPeriod,
                        Type = eSpikeValueTypes.INT32,
                        Value = "500",
                    },
                    new SpikeConfig
                    {
                        Name = A2mcuConfigConstants.ConfigNameSamplingSlave,
                        Type = eSpikeValueTypes.UINT8,
                        Value = "16",
                    },
                    new SpikeConfig
                    {
                        Name = A2mcuConfigConstants.ConfigNameSamplingRegister,
                        Type = eSpikeValueTypes.UINT8,
                        Value = "2",
                    },
                    new SpikeConfig
                    {
                        Name = A2mcuConfigConstants.ConfigNameSamplingBytes,
                        Type = eSpikeValueTypes.UINT8,
                        Value = "6",
                    },
                },
                Ports = new []
                {
                    new SpikePort
                    {
                        PortId = 0,
                        Type = eSpikeValueTypes.I2C,
                        Direction = ioPortDirection.InputOutput,
                    },
                }
            };

            var thelist = new List<SpikeThing>();
            thelist.AddRange(GpiSpikeThings);
            thelist.AddRange(GpoSpikeThings);
            thelist.Add(I2C0MasterSpikeThing);
            return thelist;
        }
    }
}
