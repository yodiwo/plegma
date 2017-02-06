using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public class ThingDescriptor
    {
        //-------------------------------------------------------------------------------------------------------------------------
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string ThingKey;
        public string Name;
        public string Img;
        public string Type;
        public string BlockType;
        public string Email; // user e-mail

        public List<ConfigParameter> Config;
        public List<ConfigParameter> ReadOnlyInfo;
        public string Hierarchy;
        public List<PortDescriptor> Ports;
        public HashSet<string> Tags;
        public ThingUIHints UIHints;
        public bool Shared;
        public bool Owned;
        public bool Hidden;
        public bool Removable;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
        public ThingDescriptor()
        {
        }

        public PortDescriptor GetPort(PortKey key)
        {
            return this.Ports.Where(p => p.PortKey == key)?.First();
        }

        public bool IsSameType(ThingDescriptor thing)
        {
            if (!String.IsNullOrWhiteSpace(Type) && Type != thing.Type)
                return false;

            if (Ports.Count != thing.Ports.Count)
                return false;

            foreach (var pkv in thing.Ports)
            {
                // build the right PortKey
                PortKey thingPortKey = PortKey.BuildFromArbitraryString(thing.ThingKey, ((PortKey)pkv.PortKey).PortUID);
                PortDescriptor Port = thing.GetPort(thingPortKey);
                if (Port == null)
                    DebugEx.Assert("could not find Port for pkey: " + thingPortKey);
                if (Port.PortType != pkv.PortType)
                    return false;
            }

            return true;
        }

    }
}
