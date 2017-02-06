using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.mNode.Plugins.TestPlugin.ThingTools
{
    public static class TextThing
    {
        public static ThingKey ThingKey;
        public static PortKey PortKey_Text;

        public static Thing CreateThing(NodeKey NodeKey)
        {
            //create key
            ThingKey = new ThingKey(NodeKey, PluginMain.ThingKeyPrefix + "Test_Text_IO");
            PortKey_Text = new PortKey(ThingKey, "Text");

            //create thing
            var thing = new Thing()
            {
                ThingKey = ThingKey,
                Name = "Text IO",
                ConfFlags = eThingConf.Removable,
            };
            thing.Ports = new List<Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = PortKey_Text,
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.InputOutput,
                        Name = "Text",
                        State = "",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        ConfFlags = ePortConf.None,
                    },
                };
            return thing;
        }
    }
}

