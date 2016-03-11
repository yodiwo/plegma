using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.GatewayMain
{
    public static class OpenhabModelExtensions
    {

        /// <summary>
        /// Converts an OpenHAB Thing, as modeled in openhab, to the Yodiwo Public API representation
        /// </summary>
        /// <param name="ohabThing"></param>
        /// <returns>the converted Private Basic API Thing</returns>
        public static Yodiwo.API.Plegma.Thing ToApiThing(this Yodiwo.Gateway.Openhab.Rest.Model.Thing ohabThing)
        {
            var apiThing = new API.Plegma.Thing();

            var thingKeyStr = ThingKey.BuildFromArbitraryString("$NodeKey$", ohabThing.UID);

            apiThing.ThingKey = thingKeyStr;

            apiThing.Name = ohabThing.item.label;

            foreach (var cfg in ohabThing.configuration)
            {
                apiThing.Config.Add(new ConfigParameter()
                {
                    Name = cfg.Key,
                    Value = cfg.Value.ToStringInvariant(), //TODO: Check if Value is always string 
                });
            }

            // Use the first part of the ohabThing's UID as the Thing's default name,
            // if the user has not provided a name. This name will be used for the toolbox 
            // name of the Thing in the Designer UI and will also appear in ThingsManager.
            if (String.IsNullOrEmpty(apiThing.Name))
            {
                try
                {
                    apiThing.Name = ohabThing.UID
                        .Split(new string[] { ":" }, StringSplitOptions.None)[0]
                        .ToUpper();
                }
                catch { }
            }


            foreach (Channel chan in ohabThing.channels)
            {
                if (chan.linkedItems.Count == 0)
                    continue;

                var li = chan.linkedItems.FirstOrDefault();
                if (li == null)
                    continue;

                var member = ohabThing.item.members.Find(m => m.link == li);
                if (member == null)
                {
                    //search name
                    member = ohabThing.item.members.Find(m => m.name == li);
                    if (member == null)
                        continue;
                }

                var dirChannel = member.stateDescription.readOnly ?
                                                 ioPortDirection.Output :
                                                 ioPortDirection.InputOutput;

                var label = member.label;
                var state = member.state;
                var portKey = PortKey.BuildFromArbitraryString(thingKeyStr, chan.id);
                var portType = ePortType.DecimalHigh;
                if (member.type == "StringItem")
                {
                    portType = ePortType.String;
                }
                apiThing.Ports.Add(new Yodiwo.API.Plegma.Port()
                {
                    Name = label,
                    Type = portType,   //high precision number by default
                    ioDirection = dirChannel,
                    State = state,
                    PortKey = portKey,
                });
            }

            try
            {
                var partials = ohabThing.UID.Split(':');
                apiThing.Type = partials[0] + ':' + partials[1];
            }
            catch { }
            apiThing.BlockType = "Openhab." + apiThing.Name;
            //TODO: Improve image handling
            apiThing.UIHints = new ThingUIHints { IconURI = "/Content/ThingsManager/img/" + ohabThing.UID.Split(':')[0] + ".png" };

            return apiThing;
        }

    }
}
