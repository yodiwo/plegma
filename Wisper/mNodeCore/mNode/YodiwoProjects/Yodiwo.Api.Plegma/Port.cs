using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.MediaStreaming;
using static Yodiwo.API.Plegma.PortStateSemantics;

namespace Yodiwo.API.Plegma
{
    /// <summary>
    /// type of values that each Port sends / receives
    /// </summary>
    public enum ePortType : int
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
        /// <summary>RGBA triplet in "R,G,B,A" format, where each element is a floating point number</summary>
        Color = 5,
        /// <summary>generic string</summary>
        String = 6,
        /// <summary>video </summary>
        VideoDescriptor = 7,
        /// <summary>audio </summary>
        AudioDescriptor = 8,
        /// <summary>binary resource port</summary>
        BinaryResourceDescriptor = 9,
        /// <summary>i2c thing</summary>
        I2CDescriptor = 10,
        /// <summary>json string</summary>
        JsonString = 11,
        /// <summary>incident descriptor port</summary>
        IncidentDescriptor = 12,
        /// <summary>incident descriptor port</summary>
        Timestamp = 13,
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
            {ePortType.Color, typeof(YColor)},
            {ePortType.String, typeof(string)},
            {ePortType.VideoDescriptor, typeof(VideoDescriptor)},
            {ePortType.AudioDescriptor, typeof(AudioMediaDescriptor)},
            {ePortType.BinaryResourceDescriptor, typeof(BinaryResourceDescriptor)},
            {ePortType.I2CDescriptor, typeof(I2CCommand)},
            {ePortType.JsonString, typeof(string)},
            {ePortType.IncidentDescriptor, typeof(IncidentDescriptor)},
        };

        public static Dictionary<ePortType, object> PortTypeDefaultValueDict = new Dictionary<ePortType, object>
        {
            {ePortType.Undefined, null},
            {ePortType.Integer, default(int)},
            {ePortType.Decimal, default(float)},
            {ePortType.DecimalHigh, default(double)},
            {ePortType.Boolean, default(bool)},
            {ePortType.Color, new YColor(0,0,0,0)},
            {ePortType.String, string.Empty},
            {ePortType.VideoDescriptor, default(VideoDescriptor)},
            {ePortType.AudioDescriptor, default(AudioMediaDescriptor)},
            {ePortType.BinaryResourceDescriptor, default(BinaryResourceDescriptor)},
            {ePortType.I2CDescriptor, default(I2CCommand)},
            {ePortType.JsonString, string.Empty},
            {ePortType.IncidentDescriptor, default(IncidentDescriptor)},
        };

        public static Dictionary<ePortType, string> PortTypeToString = new Dictionary<ePortType, string>
        {
            {ePortType.Undefined, ""},
            {ePortType.Integer, "Integer"},
            {ePortType.Decimal, "Floating Point"},
            {ePortType.DecimalHigh, "Floating Point"},
            {ePortType.Boolean, "Boolean"},
            {ePortType.Color, "Color"},
            {ePortType.String, "String"},
            {ePortType.VideoDescriptor, "Video Descriptor"},
            {ePortType.AudioDescriptor, "Audio Descriptor"},
            {ePortType.BinaryResourceDescriptor, "Binary Descriptor"},
            {ePortType.I2CDescriptor, "I2C Descriptor"},
            {ePortType.JsonString, "JSON-encoded string" },
            {ePortType.IncidentDescriptor, "Incident Descriptor" },
        };

        public static Dictionary<ioPortDirection, string> IoDirectionToString = new Dictionary<ioPortDirection, string>
        {
            {ioPortDirection.Undefined, ""},
            {ioPortDirection.Input, "Cloud to Node"},
            {ioPortDirection.Output, "Node to Cloud"},
            {ioPortDirection.InputOutput, "Bidirectional"}
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

        /// <summary>
        /// OBSOLETE FLAG, as it is now the default behavior even if no flags are set: 
        /// port will propagate all events, not only "dirty" ones (i.e. value not changed but triggered in graph)
        /// <para>To explicitly suppress this behavior set the <see cref="SupressIdenticalEvents"/> flag</para>
        /// </summary>
        PropagateAllEvents = 1 << 0,

        /// <summary>mark the port as a trigger port (this may have an effect on where it's placed on the block model and how events from it are propagated)</summary>
        IsTrigger = 1 << 1,

        /// <summary>Enable this flag to force raw values for the port. Normalization will be applied if <see cref="Decimal_Range"/> or <see cref="Integer_Range"/> semantics is used</summary>
        DoNotNormalize = 1 << 2,

        /// <summary>The opposite of <see cref="PropagateAllEvents"/>. If set port will only propagate "dirty" events, where the value actually changed and was not just triggered</summary>
        SupressIdenticalEvents = 1 << 3,
    }

    /// <summary>
    /// Basic Input/Output entity of a Thing
    /// Creates and sends messages towards the Yodiwo cloud service, 
    /// or receives and handles messages from the cloud.
    /// Both events occur via the <see cref="PortEventMsg"/> message
    /// </summary>
    [Serializable]
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

        /// <summary>semantics of values that Port sends / receives</summary>
        public string Semantics;

        /// <summary>id of Port that can Match to a ThingModelType </summary>
        public string PortModelId;

        /// <summary cref="Port.State">Current (at latest update/sampling/trigger/etc) value of Port as String.
        /// Contains a string representation of the port's state, encoded according to the port's <see cref="ePortType"/>
        /// On receiving events the Cloud Server will attempt to parse the State based on its <see cref="ePortType"/>
        /// When sending events the Cloud Server will encode the new state into a string, again according to the Port's <see cref="ePortType"/>
        /// </summary>
        public volatile string State;

        /// <summary>
        /// Port state sequence number: incremented by the Cloud server at every state update, 
        /// so that Node and servers stay in sync
        /// </summary>
        public volatile uint RevNum;

        /// <summary>Configuration flags for port</summary>
        public ePortConf ConfFlags = ePortConf.PropagateAllEvents;

        /// <summary>
        /// Timestamp of the last action that updated the port value and/or revision number
        /// </summary>
        public ulong LastUpdatedTimestamp;

        /// <summary>
        /// Color of port value
        /// </summary>
        public string Color;

        /// <summary>
        /// Size of port value
        /// </summary>
        public int Size;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [NonSerialized]
        public object Yport;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary> Revnum indicating no tracking of revision numbers </summary>
        public const uint RevNum_NoTracking = 0;
        /// <summary> Revnum indicating initialization of port (as if portstate was followed by thing addditiong) </summary>
        public const uint RevNum_Initialize = 1;
        #endregion

        public Port(PortKey portKey, ePortType type, ioPortDirection ioDirection, string portmodelid = null)
        {
            this.PortKey = portKey;
            this.Type = type;
            this.State = null;
            this.PortModelId = portmodelid;

            DebugEx.Assert(ioDirection != ioPortDirection.Undefined, $"Undefined IO direction for port {portKey}");
            this.ioDirection = ioDirection;
        }

        //should be used only by deserializer
        public Port()
        {
            this.Type = ePortType.Undefined;
            this.ioDirection = ioPortDirection.Undefined;
            this.PortKey = default(PortKey);
            this.PortModelId = default(string);
            this.Color = "#0000FF";
            this.Size = 18;
        }

        public void Update_Except_States(Port otherPort)
        {
            this.Name = otherPort.Name;
            this.Description = otherPort.Description;
            this.Type = otherPort.Type;
            this.Semantics = otherPort.Semantics;

            DebugEx.Assert(otherPort.ioDirection != ioPortDirection.Undefined, $"Undefined IO direction for port {otherPort.PortKey}");
            if (otherPort.ioDirection != ioPortDirection.Undefined)
                this.ioDirection = otherPort.ioDirection;

            this.PortKey = otherPort.PortKey;
            this.ConfFlags = otherPort.ConfFlags;
            this.PortModelId = otherPort.PortModelId;
        }

        public void Update(Port otherPort)
        {
            Update_Except_States(otherPort);

            if (otherPort.RevNum >= RevNum || otherPort.RevNum == RevNum_Initialize)
            {
                State = otherPort.State;
                RevNum = otherPort.RevNum == RevNum_Initialize ? RevNum++ : otherPort.RevNum;
            }
        }

        public Port DeepClone()
        {
            var newPort = new Port(PortKey, Type, ioDirection);
            newPort.Update_Except_States(this);
            newPort.State = State;
            newPort.RevNum = RevNum;
            return newPort;
        }

        public override string ToString()
        {
            return ((PortKey)this.PortKey).PortUID + " " + this.Type;
        }

        public object GetStateObject()
        {
            return State2Value(this.Type, this.State);
        }

        public bool IsNumeric()
        {
            return (this.Type == ePortType.Integer ||
                    this.Type == ePortType.Decimal ||
                    this.Type == ePortType.DecimalHigh);
        }

        public static Type ePortType2Type(ePortType PortType)
        {
            if (PortType == ePortType.Undefined)
                return null;    //treat string as default
            else if (PortType == ePortType.Integer)
                return typeof(Int64);
            else if (PortType == ePortType.Decimal || PortType == ePortType.DecimalHigh)
                return typeof(double);
            else if (PortType == ePortType.Boolean)
                return typeof(bool);
            else if (PortType == ePortType.Color)
                return typeof(YColor);
            else if (PortType == ePortType.String)
                return typeof(string);    //already string
            else if (PortType == ePortType.VideoDescriptor)
                return typeof(VideoMediaDescriptor);
            else if (PortType == ePortType.AudioDescriptor)
                return typeof(AudioMediaDescriptor);
            else if (PortType == ePortType.BinaryResourceDescriptor)
                return typeof(BinaryResourceDescriptor);
            else if (PortType == ePortType.I2CDescriptor)
                return typeof(I2CCommand);//sos discuss with mits and gepa
            else if (PortType == ePortType.JsonString)
                return typeof(string);
            else if (PortType == ePortType.Timestamp)
                return typeof(DateTime);
            else
            {
                DebugEx.Assert($"channel type {PortType} not accounted for");
                return null;
            }
        }

        public static object State2Value(ePortType portType, string state)
        {
            if (portType == ePortType.Undefined)
                return state;    //treat string as default
            else if (portType == ePortType.String)
                return state;
            else
            {
                var type = ePortType2Type(portType);
                if (type != null)
                    return Yodiwo.ConvertEx.Convert(state, type);
                else
                {
                    DebugEx.Assert($"channel type {portType} not accounted for.");
                    return state; //treat string as default
                }
            }
        }

        public static string Value2State(object value)
        {
            if (value == null)
                return null;
            else if (value is string)
                return value as string;
            else if (value is YColor)
                return value.ToJSON();
            else if (value is bool)
                return value.ToStringInvariant();
            else
            {
                var valueType = value.GetType();
                if (valueType.IsNumber()) //known simple types
                    return value.ToStringInvariant();
                else
                    return value.ToJSON();
            }
        }

        public void SetState(string newState, DateTime? timestamp = null)
        {
            var ts = timestamp.HasValue ? timestamp.Value : DateTime.UtcNow;

            this.State = newState;
            this.IncRevNum();
            this.LastUpdatedTimestamp = ts.ToUnixMilli();
        }

        public void IncRevNum()
        {
            lock (this)
            {
                RevNum++;
                if (RevNum == RevNum_NoTracking || RevNum == RevNum_Initialize)
                    RevNum++;
            }
        }
    }
}
