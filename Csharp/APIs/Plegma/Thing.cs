using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;

namespace Yodiwo.API.Plegma
{
    /// <summary>
    /// Configuration parameters for the thing in generic name-value pairs
    /// </summary>
    [Serializable]
    public struct ConfigParameter
    {
        /// <summary>Name of configuration parameter</summary>
        public string Name;
        /// <summary>Value of configuration parameter</summary>
        public string Value;
        /// <summary>Description and usage guidelines of configuration parameter</summary>
        public string Description;
    }

    /// <summary>
    /// Collection of instructions ("hints") for how to present this thing in the Cyan UI
    /// </summary>
    [Serializable]
    public struct ThingUIHints
    {
        /// <summary>URI of icon to show in Cyan for this thing</summary>
        public string IconURI;

        /// <summary>Description of Thing to show in Cyan (tooltip, etc)</summary>
        public string Description;
    }

    /// <summary>
    /// Main representation of a Thing that can interact with the Yodiwo cloud service
    /// </summary>
    [Serializable]
    public class Thing
    {
        #region Fields/Properties
        /// <summary>
        /// Globally unique Key string of this Thing
        /// </summary>
        public string ThingKey;

        /// <summary>
        /// friendly name of this Thing
        /// </summary>
        public string Name;

        /// <summary>
        /// list of vendor provided configuration parameters (changeable by the user)
        /// </summary>
        public List<ConfigParameter> Config;

        /// <summary>
        /// list of vendor provided read-only information
        /// </summary>
        public List<ConfigParameter> ReadonlyInfo;

        /// <summary>
        /// list of ports (inputs / outputs) that this Thing implements
        /// </summary>
        public List<Port> Ports;

        /// <summary>
        /// Specifies the Thing's type <see cref="ThingModelType"/>
        /// </summary>
        public string Type;

        /// <summary>
        /// Specifies the Thing's block type if it's to be specially modeled in the Cyan UI
        /// It can be left null if this Thing is to be modeled by the default Cyan UI blocks
        /// In this case Output-type Ports are gathered and represented as a Cyan UI Input Thing (thing->cloud events)
        /// and Input-type Ports are gathered and represented as a Cyan UI Output Thing (cloud->thing events)
        /// Both event directions occur via the <see cref="PortEventMsg"/> messages
        /// </summary>
        public string BlockType;

        /// <summary>
        /// Specifies whether the thing can be removed
        /// </summary>
        public bool Removable;

        /// <summary>
        /// Specifies a uri to which the cloud will post messages in case an RX-incapable node is connected.
        /// Can be used as a return path for REST api. May be left null or empty.
        /// </summary>
        public string RESTUri;

        /// <summary>
        /// Specifies the Thing's hierarchy(ies) within the node's modeled ecosystem.
        /// Each entry specifies a hierarchical view (separated by '/') of the Thing's position in the User's ecosystem of devices
        /// May be left null or empty.
        /// </summary>
        public List<string> Hierarchies;

        /// <summary>
        /// Hints for the UI system
        /// </summary>
        public ThingUIHints UIHints;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [NonSerialized]
        public object Ything;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region Constructors

        public Thing(ThingKey key, List<Port> ports)
        {
            this.ThingKey = key;
            this.Ports = ports;
            this.Config = new List<ConfigParameter>();
            this.ReadonlyInfo = new List<ConfigParameter>();
        }

        public Thing(string uid, List<Port> ports, NodeKey nodeKey)
            : this(new ThingKey(nodeKey, uid), ports)
        {
        }

        public Thing(ThingKey key) : this(key, new List<Port>()) { }

        public Thing() : this(default(ThingKey), new List<Port>()) { }

        #endregion

        #region Functions

        /// <summary>
        /// Directly access port #id of this Thing
        /// </summary>
        /// <param name="id">Port ID (Unique ID string of Thing's Port</param>
        /// <returns>Class Port corresponding to incoming port Id</returns>
        public virtual Port GetPort(PortKey key)
        {
            return this.Ports.FirstOrDefault(c => key == c.PortKey);
        }

        public virtual Port GetPort(int idx)
        {
            try
            {
                return this.Ports.ElementAt(idx);
            }
            catch { return null; }
        }

        public Port GetPortByName(string name)
        {
            return this.Ports.FirstOrDefault(p => p.Name.Equals(name));
        }


        public virtual void Update(Thing incomingThing, bool UpdatePortStates = true)
        {
            DebugEx.Assert(this.ThingKey == incomingThing.ThingKey || ((ThingKey)this.ThingKey).IsInvalid, "Incompatible Things");

            this.ThingKey = incomingThing.ThingKey;
            this.Name = incomingThing.Name;
            this.Config = incomingThing.Config?.ToList();
            this.ReadonlyInfo = incomingThing.ReadonlyInfo?.ToList();
            this.Type = incomingThing.Type;
            this.BlockType = incomingThing.BlockType;
            this.Removable = incomingThing.Removable;
            this.RESTUri = incomingThing.RESTUri;
            this.UIHints = incomingThing.UIHints;
            this.Hierarchies = incomingThing.Hierarchies;

            lock (this)
            {
                //update existing Ports or add the ones that are missing
                foreach (var p in incomingThing.Ports)
                {
                    var port = this.GetPort(p.PortKey);
                    if (port != null)
                    {
                        if (UpdatePortStates)
                            port.Update(p);
                        else
                            port.Update_Except_States(p);
                    }
                    else
                        this.Ports.Add(p.DeepClone());
                }

                //remove the ones that don't exist in incoming Thing
                List<Port> toRemove = new List<Port>();
                foreach (var p in this.Ports)
                {
                    if (incomingThing.GetPort(p.PortKey) == null)
                        toRemove.Add(p);
                }
                foreach (var p in toRemove)
                    this.Ports.Remove(p);
            }
        }

        public Thing DeepClone()
        {
            var newThing = new Thing(ThingKey);
            newThing.Update(this);
            return newThing;
        }

        public string GetConfigValue(string confName)
        {
            var cp = Config.GetConfigParam(confName);
            return cp.Name == confName ? cp.Value : null;
        }

        public bool SetConfigValue(string confName, string value)
        {
            var removed = Config.RemoveAll(n => n.Name == confName);
            if (removed != 0)
            {
                Config.Add(new ConfigParameter() { Value = value, Name = confName });
            }
            return removed != 0;
        }

        public string GetReadOnlyValue(string roName)
        {
            var cp = this.ReadonlyInfo.GetConfigParam(roName);
            return cp.Name == roName ? cp.Value : null;
        }

        public override string ToString()
        {
            string str = "Key=" + this.ThingKey + " (" + Name + ")";
            foreach (var p in this.Ports)
                str += " /port " + p;
            return str;
        }


        public bool IsSameType(Thing thing)
        {
            if (!String.IsNullOrWhiteSpace(Type) && Type != thing.Type)
                return false;

            if (Ports.Count != thing.Ports.Count)
                return false;

            foreach (var pkv in thing.Ports)
            {
                // build the right PortKey
                PortKey thingPortKey = PortKey.BuildFromArbitraryString(thing.ThingKey, ((PortKey)pkv.PortKey).PortUID);
                Port Port = thing.GetPort(thingPortKey);
                if (Port == null)
                    DebugEx.Assert("could not find Port for pkey: " + thingPortKey);
                if (Port.Type != pkv.Type)
                    return false;
            }
            return true;
        }

        #endregion
    }

    public static class ThingExtensions
    {
        public static ConfigParameter GetConfigParam(this IEnumerable<ConfigParameter> source, string confName)
        {
            return source.FirstOrDefault(n => n.Name == confName);
        }
    }
}
