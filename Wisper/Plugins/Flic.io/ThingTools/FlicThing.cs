using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.mNode.Plugins.Integration_Flic.ThingTools
{
    public static class FlicThing
    {
        public static readonly string SingleClick = "SingleClick";
        public static readonly string DoubleClick = "DoubleClick";
        public static readonly string LongClick = "LongClick";

        public static string CreateThingKey(string btnID)
        {
            return PluginMain.ThingKeyPrefix + "FLIC:" + btnID.Replace(API.Plegma.PlegmaAPI.KeySeparator, '^');
        }

        public static Thing CreateThing(string btnID, string flicBtnName)
        {
            var thing = new Thing()
            {
                ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", CreateThingKey(btnID)),
                Name = "Flic-" + flicBtnName,
                ConfFlags = eThingConf.Removable,
                UIHints = new ThingUIHints()
                {
                    Description = "FLIC Button " + flicBtnName + ".",
                    IconURI = "/Content/img/icons/Generic/icon-thing-flic.png"
                },
            };
            thing.Ports = new List<Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = PortKey.BuildFromArbitraryString(thing.ThingKey, SingleClick),
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Single Click",
                        State = "",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        ConfFlags = ePortConf.None,
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = PortKey.BuildFromArbitraryString(thing.ThingKey, DoubleClick),
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Double Click",
                        State = "",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        ConfFlags = ePortConf.None,
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = PortKey.BuildFromArbitraryString(thing.ThingKey, LongClick),
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Long Click",
                        State = "",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        ConfFlags = ePortConf.None,
                    },
                };
            return thing;
        }
    }
}

