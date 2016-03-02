using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    /// <summary>
    /// type of values that each Port sends / receives
    /// </summary>
    public enum ePortType : byte
    {
        /// <summary>undefined, should not be used!</summary>
        Undefined = 0,
        /// <summary>integer values</summary>
        Integer = 1,
        /// <summary>single precision floating point values</summary>
        Decimal = 2,
        /// <summary>double precision floating point values</summary>
        DecimalHigh = 3,
        /// <summary>boolean values (can be true/false, on/off, 1/0, etc)</summary>
        Boolean = 4,
        /// <summary>RGB triplet in "R,G,B" format</summary>
        Color = 5,
        /// <summary>generic string</summary>
        String = 6
    }

    /// <summary>
    /// Direction of Port
    /// </summary>
    public enum ioPortDirection : byte
    {
        /// <summary>undefined, should not be used!</summary>
        Undefined = 0,
        /// <summary>both Input and Output, Port will be used in both Graph Input and Output Things</summary>
        InputOutput = 1,
        /// <summary>Output only; Port will be used only in Graph Input Things (node->cloud)</summary>
        Output = 2,
        /// <summary>Input only; Port will be used only in Graph Output Things (cloud->node)</summary>
        Input = 3
    }

    public static class PortConfiguration
    {
        public static Dictionary<ePortType, Type> PortTypeDict = new Dictionary<ePortType, Type>
        {
            {ePortType.Undefined, typeof(object)},
            {ePortType.Integer, typeof(int)},
            {ePortType.Decimal, typeof(float)},
            {ePortType.DecimalHigh, typeof(double)},
            {ePortType.Boolean, typeof(bool)},
            {ePortType.Color, typeof(Tuple<int,int,int>)},
            {ePortType.String, typeof(string)}
        };

        public static Dictionary<ePortType, object> PortTypeDefaultValueDict = new Dictionary<ePortType, object>
        {
            {ePortType.Undefined, null},
            {ePortType.Integer, default(int)},
            {ePortType.Decimal, default(float)},
            {ePortType.DecimalHigh, default(double)},
            {ePortType.Boolean, default(bool)},
            {ePortType.Color, new Tuple<int,int,int>(0,0,0)},
            {ePortType.String, string.Empty}
        };
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ePortConf : uint
    {
        /// <summary>no configuration set</summary>
        None = 0,

        /// <summary>port should receive all events, not only "dirty" ones (i.e. value not changed but triggered in graph)</summary>
        ReceiveAllEvents = 1,

        /// <summary>mark the port as a trigger port (this may have an effect on where it's placed on the block model and how events from it are propagated)</summary>
        IsTrigger = 2
    }

    /// <summary>
    /// Basic Input/Output entity of a Thing
    /// Creates and sends messages towards the Yodiwo cloud service, 
    /// or receives and handles messages from the cloud.
    /// Both events occur via the <see cref="PortEventMsg"/> message
    /// </summary>
    public class Port
    {
        #region Fields

        /// <summary>Globally unique string identifying this port; Construct it using the <see cref="PortKey"/> constructor</summary>
        public string PortKey;

        /// <summary>Friendly name of this Port (as it will appear in the Cyan UI and blocks)</summary>
        public string Name;

        /// <summary>Description of Port to show in Cyan (tooltip, etc)</summary>
        public string Description;

        /// <summary>Direction (<see cref="ioPortDirection"/>) of Port</summary>
        public ioPortDirection ioDirection;

        /// <summary>type (<see cref="ePortType"/>) of values that each Port sends / receives</summary>
        public ePortType Type;

        /// <summary cref="Port.State">Current (at latest update/sampling/trigger/etc) value of Port as String.
        /// Contains a string representation of the port's state, encoded according to the port's <see cref="ePortType"/>
        /// On receiving events the Cloud Server will attempt to parse the State based on its <see cref="ePortType"/>
        /// When sending events the Cloud Server will encode the new state into a string, again according to the Port's <see cref="ePortType"/>
        /// </summary>
        public string State;

        [Newtonsoft.Json.JsonIgnore]
        public object TypedState { get { return State2Value(this.Type, this.State); } }

        /// <summary>
        /// Port state sequence number: incremented by the Cloud server at every state update, 
        /// so that Node and servers stay in sync
        /// </summary>
        public int RevNum;

        /// <summary>Configuration flags for port</summary>
        public ePortConf ConfFlags;

        #endregion

        public Port(PortKey portKey, ePortType type, ioPortDirection ioDirection)
        {
            this.PortKey = portKey;
            this.Type = type;
            this.ioDirection = ioDirection;
            this.State = null;
        }

        //should be used only by deserializer
        public Port()
        {
            this.Type = ePortType.Undefined;
            this.ioDirection = ioPortDirection.Undefined;
            this.PortKey = default(PortKey);
        }

        public void Update(Port port)
        {
            this.Name = port.Name;
            this.Description = port.Description;
            this.Type = port.Type;
            this.ioDirection = port.ioDirection;

            this.PortKey = port.PortKey;
            this.ConfFlags = port.ConfFlags;
        }

        public Port DeepClone()
        {
            var newPort = new Port(PortKey, Type, ioDirection);
            newPort.Update(this);
            newPort.State = State;
            return newPort;
        }

        public override string ToString()
        {
            return this.PortKey + " (type " + this.Type + ") = " + this.State;
        }

        public static object State2Value(ePortType PortType, string State)
        {
            if (PortType == ePortType.Undefined)
            {
                return State;    //treat string as default
            }
            else if (PortType == ePortType.Integer)
            {
                return Yodiwo.ConvertEx.Convert(State, typeof(int));
            }
            else if (PortType == ePortType.Decimal)
            {
                return Yodiwo.ConvertEx.Convert(State, typeof(double));
            }
            else if (PortType == ePortType.DecimalHigh)
            {
                return Yodiwo.ConvertEx.Convert(State, typeof(double));
            }
            else if (PortType == ePortType.Boolean)
            {
                return Yodiwo.ConvertEx.Convert(State, typeof(bool));
            }
            else if (PortType == ePortType.Color)
            {
                //TODO: change to vector
                var rgbTriplet = State.Split(',');
                return new Tuple<int, int, int>(int.Parse(rgbTriplet[0]), int.Parse(rgbTriplet[1]), int.Parse(rgbTriplet[2]));
            }
            else if (PortType == ePortType.String)
            {
                return State;    //already string
            }
            else
            {
                throw new Exception("There's a channel type you haven't accounted for.");
            }
        }
    }
}
