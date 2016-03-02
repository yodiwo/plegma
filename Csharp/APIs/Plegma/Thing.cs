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
    public struct ConfigParameter
    {
        public string Name;
        public string Value;
    }

    /// <summary>
    /// Collection of instructions ("hints") for how to present this thing in the Cyan UI
    /// </summary>
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
        /// list of vendor provided configuration parameters
        /// </summary>
        public List<ConfigParameter> Config;

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
        /// Both event directions occur via the <see cref="PortEventMsg"/> and <see cref="PortEventBatchMsg"/> messages
        /// </summary>
        public string BlockType;

        /// <summary>
        /// Specifies whether the thing can be removed
        /// </summary>
        public bool Removable;

        /// <summary>
        /// Hints for the UI system
        /// </summary>
        public ThingUIHints UIHints;


        /// <summary> Helper (not part of thing description) </summary>
        [NonSerialized]
        [Newtonsoft.Json.JsonIgnore]
        public object Tag;
        #endregion

        #region Constructors

        public Thing(ThingKey key, List<Port> ports)
        {
            this.ThingKey = key;
            this.Ports = ports;
            this.Config = new List<ConfigParameter>();
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


        public virtual void Update(Thing incomingThing)
        {
            DebugEx.Assert(this.ThingKey == incomingThing.ThingKey || ((ThingKey)this.ThingKey).IsInvalid, "Incompatible Things");

            this.ThingKey = incomingThing.ThingKey;
            this.Name = incomingThing.Name;
            this.Config = incomingThing.Config.ToList();
            this.Type = incomingThing.Type;
            this.BlockType = incomingThing.BlockType;
            this.UIHints = incomingThing.UIHints;

            lock (this)
            {
                //update existing Ports or add the ones that are missing
                foreach (var p in incomingThing.Ports)
                {
                    var port = this.GetPort(p.PortKey);
                    if (port != null)
                        port.Update(p);
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

        public override string ToString()
        {
            string str = "Key=" + this.ThingKey;
            foreach (var p in this.Ports)
                str += " /port " + p;
            return str;
        }

        #endregion
    }
}
