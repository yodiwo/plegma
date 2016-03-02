using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Yodiwo.Tools;

namespace Yodiwo.API.Plegma
{
    //TODO: User=0, Graph=0, Block=0 are now the invalid (default) values and should be added checks on graphbuilder/dbmanager etc

    #region UserKey
    /// <summary>
    /// Globally unique identifier of a <see cref="User"/>
    /// </summary>
#if NETFX
    [TypeConverter(typeof(UserKeyConverter))]
#endif
    public struct UserKey : IEquatable<UserKey>, IFillFromString
    {
        public /*readonly*/ string UserID;

        public UserKey(string UserID) { this.UserID = UserID; }

        [NonSerialized]
        public const int MinUserIDLength = 24;
        [NonSerialized]
        public const int MaxUserIDLength = MinUserIDLength;

        [NonSerialized]
        public const int MinKeyLength = MinUserIDLength;
        [NonSerialized]
        public const int MaxKeyLength = MaxUserIDLength;

        [JsonIgnore]
        public bool IsValid { get { return !string.IsNullOrEmpty(UserID) && UserID.Length >= MinUserIDLength && UserID.Length <= MaxUserIDLength && Validators.ValidateKey(UserID); } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        #region Conversion to/from string key

        public static implicit operator UserKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(UserKey key)
        {
            return key.ToString();
        }

        public static UserKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(UserKey);
            else
                try
                {
                    var ret = new UserKey(stringValue);
                    return ret.IsValid ? ret : default(UserKey);
                }
                catch
                {
                    return default(UserKey);
                }
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            UserID = key.UserID;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return UserID;
        }
        #endregion

        public string ToStringEx()
        {
            return "UserKey:{" + UserID + "}";
        }

        #region Equality
        public bool Equals(UserKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref UserKey other)
        {
            return ToolBox.StringEqualityEx(UserID, other.UserID);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UserKey)) return false;
            return Equals((UserKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (UserID == null ? 0 : UserID.GetHashCode());
            return hash;
        }

        public static bool operator ==(UserKey left, UserKey right) { return left.Equals(ref right); }
        public static bool operator !=(UserKey left, UserKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class UserKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return UserKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((UserKey)value).ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
#endif
    #endregion

    #endregion

    #region NodeKey
    /// <summary>
    /// Globally unique identifier of a <see cref="Node"/>
    /// </summary>
#if NETFX
    [TypeConverter(typeof(NodeKeyConverter))]
#endif
    public struct NodeKey : IEquatable<NodeKey>, IFillFromString
    {
        public /*readonly*/ UserKey UserKey;
        public /*readonly*/ uint NodeID;

        public NodeKey(string userId, uint nodeId) { this.UserKey = new UserKey(userId); this.NodeID = nodeId; }
        public NodeKey(UserKey userKey, uint nodeId) { this.UserKey = userKey; this.NodeID = nodeId; }
        public NodeKey(string str) { this = str; }

        [NonSerialized]
        public const int MinNodeIDLength = 1;
        [NonSerialized]
        public const int MaxNodeIDLength = 10; //<--- uint.MaxValue.ToStringInvariant().Length;

        [NonSerialized]
        public const int MinKeyLength = UserKey.MinKeyLength + 1 + MinNodeIDLength;
        [NonSerialized]
        public const int MaxKeyLength = UserKey.MaxKeyLength + 1 + MaxNodeIDLength;

        [JsonIgnore]
        public bool IsValid { get { return NodeID != default(uint) && UserKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        [JsonIgnore]
        public const uint CloudId = uint.MaxValue;

        #region Conversion to/from string key

        public static implicit operator NodeKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(NodeKey key)
        {
            return key.ToString();
        }

        public static NodeKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(NodeKey);
            else
                try
                {
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    var ret = new NodeKey(stringValue.Substring(0, key_separator), uint.Parse(stringValue.Substring(key_separator + 1)));
                    return ret.IsValid ? ret : default(NodeKey);
                }
                catch
                {
                    return default(NodeKey);
                }
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            UserKey = key.UserKey;
            NodeID = key.NodeID;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return UserKey.ToString() + PlegmaAPI.KeySeparator + NodeID.ToStringInvariant();
        }

        /// <summary> like ToString() but fills digits with leading zeros</summary>
        public string ToStringFullSize()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return UserKey.ToString() + PlegmaAPI.KeySeparator + NodeID.ToStringInvariant("D" + MaxNodeIDLength);
        }

        /// <summary> write the key to a byte array </summary>
        public void GetBytes(byte[] bytes, int index)
        {
            try
            {
                if (IsInvalid)
                    for (int n = 0; n < MaxKeyLength; n++)
                        bytes[n] = 0;
                else
                {
                    var str = ToStringFullSize();
#if DEBUG
                    DebugEx.Assert(str.Length == MaxKeyLength, "Invalid key length");
#endif
                    for (int n = 0; n < MaxKeyLength; n++)
                        bytes[index + n] = (byte)str[n];
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Could not get nodekey bytes");
                for (int n = 0; n < MaxKeyLength; n++)
                    bytes[n] = 0;
            }
        }

        /// <summary> get key from byte array </summary>
        public static NodeKey FromBytes(byte[] bytes, int index)
        {
            try
            {
                //get to string
                var str = System.Text.Encoding.UTF8.GetString(bytes, index, MaxKeyLength).Trim();
                if (str.Length != MaxKeyLength)
                    return default(NodeKey);
                //convert to node Key
                return (NodeKey)str;
            }
            catch //(Exception ex)
            {
                return default(NodeKey);
            }
        }
        #endregion

        /// <summary> User-Friendly string (invalid as key)</summary>
        public string ToStringEx()
        {
            return "NodeKey:{" + UserKey + NodeID.ToStringInvariant() + "}";
        }

        #region Equality
        public bool Equals(NodeKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref NodeKey other)
        {
            return UserKey == other.UserKey && NodeID == other.NodeID;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NodeKey)) return false;
            return Equals((NodeKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ UserKey.GetHashCode();
            hash = (hash * 397) ^ NodeID.GetHashCode();
            return hash;
        }

        public static bool operator ==(NodeKey left, NodeKey right) { return left.Equals(ref right); }
        public static bool operator !=(NodeKey left, NodeKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class NodeKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                return NodeKey.ConvertFromString(value as string);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
            {
                return null;
            }

            if (destinationType == typeof(string))
            {
                return ((NodeKey)value).ToString();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }
    }
#endif
    #endregion

    #endregion

    #region ThingKey
    /// <summary>
    /// Globally unique identifier of a <see cref="Thing"/>
    /// </summary>
#if NETFX
    [TypeConverter(typeof(ThingKeyConverter))]
#endif
    public struct ThingKey : IEquatable<ThingKey>, IFillFromString
    {
        public /*readonly*/ NodeKey NodeKey;
        public /*readonly*/ string ThingUID;

        [JsonIgnore]
        public bool IsValid { get { return !string.IsNullOrWhiteSpace(ThingUID) && Validators.ValidateKey(ThingUID) && NodeKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }


        #region Constructors
        public ThingKey(NodeKey nodeKey, string thingId)
        {
            DebugEx.Assert(!thingId.Contains(SpikeSeparator), SpikeSeparator + " is only allowed in Spike Thing Keys and this is not the way to create one");
            this.NodeKey = nodeKey;
            this.ThingUID = thingId;
            this._SubnodeId = 0;
            this._SpikeThingId = 0;
        }

        public ThingKey(NodeKey nodeKey, int subNodeId, int spikeId)
        {
            this.NodeKey = nodeKey;
            this.ThingUID = subNodeId.ToStringInvariant() + SpikeSeparator + spikeId.ToStringInvariant();
            this._SubnodeId = subNodeId;
            this._SpikeThingId = spikeId;
        }

        public ThingKey(string str) { this = str; }
        #endregion


        #region Spike sub keys
        [JsonIgnore]
        public bool IsSpikeTKey { get { return SubnodeId != 0 && SpikeThingId != 0; } }
        //--------------------------------------------------------------------------------------------------------
        [NonSerialized]
        private int _SubnodeId;

        [JsonIgnore]
        public int SubnodeId
        {
            get
            {
                if (IsValid)
                {
                    if (_SubnodeId == 0)
                        _GetSpikeSubKeys();

                    return _SubnodeId;
                }
                return 0;
            }
        }

        //--------------------------------------------------------------------------------------------------------
        [NonSerialized]
        private int _SpikeThingId;

        [JsonIgnore]
        public int SpikeThingId
        {
            get
            {
                if (IsValid)
                {
                    if (_SpikeThingId == 0)
                        _GetSpikeSubKeys();

                    return _SpikeThingId;
                }
                return 0;
            }
        }

        //--------------------------------------------------------------------------------------------------------
        public readonly static string SpikeSeparator = "/";

        //--------------------------------------------------------------------------------------------------------
        private void _GetSpikeSubKeys()
        {
            var spikeSubKeys = ThingUID.Split(new[] { SpikeSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (spikeSubKeys.Length == 2)
            {
                int.TryParse(spikeSubKeys[0], out _SubnodeId);
                int.TryParse(spikeSubKeys[1], out _SpikeThingId);
            }
        }

        public string SubnodeKey { get { return IsSpikeTKey ? NodeKey + PlegmaAPI.KeySeparator + _SubnodeId + SpikeSeparator : ""; } }
        public ThingKey SubnodeKeyAsTkey { get { return IsSpikeTKey ? new ThingKey(NodeKey, _SubnodeId + SpikeSeparator) : default(ThingKey); } }
        #endregion


        #region Conversion to/from string key

        public static implicit operator ThingKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(ThingKey key)
        {
            return key.ToString();
        }

        static DictionaryTS<string, ThingKey> cache_String2Key = new DictionaryTS<string, ThingKey>();
        public static ThingKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(ThingKey);
            else
            {
                ThingKey ret;
                if (cache_String2Key.TryGetValue(stringValue, out ret) == false)
                {
                    try
                    {
                        var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                        ret = new ThingKey(stringValue.Substring(0, key_separator), stringValue.Substring(key_separator + 1));
                        //checks
                        ret = ret.IsValid ? ret : default(ThingKey);
                    }
                    catch { ret = default(ThingKey); }
                    cache_String2Key.ForceAdd(stringValue, ret);
                }
                return ret;
            }
        }

        public static string BuildFromArbitraryString(string nodeKey, int subNodeId, int spikeId)
        {
            return nodeKey + PlegmaAPI.KeySeparator + subNodeId.ToStringInvariant() + SpikeSeparator + spikeId.ToStringInvariant();
        }

        public static string BuildFromArbitraryString(string nodeKey, string ThingUID)
        {
            return nodeKey + PlegmaAPI.KeySeparator + ThingUID.ToStringInvariant();
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            NodeKey = key.NodeKey;
            ThingUID = key.ThingUID;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return NodeKey.ToString() + PlegmaAPI.KeySeparator + ThingUID.ToStringInvariant();
        }
        #endregion


        public string ToStringEx()
        {
            return "ThingKey:{" + NodeKey + ", " + ThingUID.ToStringInvariant() + "}";
        }

        #region Equality
        public bool Equals(ThingKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref ThingKey other)
        {
            return NodeKey == other.NodeKey && ToolBox.StringEqualityEx(ThingUID, other.ThingUID);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ThingKey)) return false;
            return Equals((ThingKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ NodeKey.GetHashCode();
            hash = (hash * 397) ^ (ThingUID == null ? 0 : ThingUID.GetHashCode());
            return hash;
        }

        public static bool operator ==(ThingKey left, ThingKey right) { return left.Equals(ref right); }
        public static bool operator !=(ThingKey left, ThingKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class ThingKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return ThingKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((ThingKey)value).ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
#endif
    #endregion

    #endregion

    #region PortKey
    /// <summary>
    /// Globally unique identifier of a <see cref="Thing"/>'s <see cref="Port"/>
    /// </summary>
#if NETFX
    [TypeConverter(typeof(PortKeyConverter))]
#endif
    public struct PortKey : IEquatable<PortKey>, IFillFromString
    {
        public /*readonly*/ ThingKey ThingKey;
        public /*readonly*/ string PortUID;

        public PortKey(Thing thing, string portUid) { ThingKey = thing.ThingKey; PortUID = portUid; }
        public PortKey(ThingKey thingKey, string portUid) { ThingKey = thingKey; PortUID = portUid; }
        public PortKey(string str) { this = str; }

        [JsonIgnore]
        public bool IsValid { get { return !string.IsNullOrWhiteSpace(PortUID) && Validators.ValidateKey(PortUID) && ThingKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        #region Conversion to/from string key
        public static implicit operator PortKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(PortKey key)
        {
            return key.ToString();
        }

        static DictionaryTS<string, PortKey> cache_String2Key = new DictionaryTS<string, PortKey>();
        public static PortKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(PortKey);
            else
            {
                PortKey ret;
                if (cache_String2Key.TryGetValue(stringValue, out ret) == false)
                {
                    try
                    {
                        var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                        ret = new PortKey(stringValue.Substring(0, key_separator), stringValue.Substring(key_separator + 1));
                        //checks
                        ret = ret.IsValid ? ret : default(PortKey);
                    }
                    catch { ret = default(PortKey); }
                    cache_String2Key.ForceAdd(stringValue, ret);
                }
                return ret;
            }
        }
        public static string BuildFromArbitraryString(string thingKey, string PortUID)
        {
            return thingKey + PlegmaAPI.KeySeparator + PortUID.ToStringInvariant();
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            ThingKey = key.ThingKey;
            PortUID = key.PortUID;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return ThingKey.ToString() + PlegmaAPI.KeySeparator + PortUID.ToStringInvariant();
        }
        #endregion

        #region Equality
        public bool Equals(PortKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref PortKey other)
        {
            return ThingKey.Equals(ref other.ThingKey) &&
                   ToolBox.StringEqualityEx(PortUID, other.PortUID);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PortKey)) return false;
            return Equals((PortKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ ThingKey.GetHashCode();
            hash = (hash * 397) ^ (PortUID == null ? 0 : PortUID.GetHashCode());
            return hash;
        }

        public static bool operator ==(PortKey left, PortKey right) { return left.Equals(ref right); }
        public static bool operator !=(PortKey left, PortKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class PortKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                return PortKey.ConvertFromString(value as string);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
            {
                return null;
            }
            if (destinationType == typeof(string))
            {
                return ((PortKey)value).ToString();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }
    }
#endif
    #endregion

    #endregion

    #region GraphDescriptorBaseKey (no versioning)
    public struct GraphDescriptorBaseKey : IEquatable<GraphDescriptorBaseKey>, IFillFromString
    {
        public  /*readonly*/ UserKey UserKey;
        public  /*readonly*/ string Id;

        public GraphDescriptorBaseKey(UserKey key, string id) { this.UserKey = key; this.Id = id; }

        [JsonIgnore]
        public bool IsValid { get { return !string.IsNullOrWhiteSpace(Id) && Validators.ValidateKey(Id) && UserKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }


        #region Implicit conversion to GraphDescriptorBaseKey
        //public static implicit operator GraphDescriptorKey(GraphDescriptorBaseKey baseKey)
        //{
        //    return new GraphDescriptorKey(baseKey.UserKey, baseKey.Id, 0);
        //}
        public static implicit operator GraphDescriptorBaseKey(GraphDescriptorKey key)
        {
            return key.BaseKey;
        }
        #endregion

        #region Implicit conversion to/from string key
        public static implicit operator GraphDescriptorBaseKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(GraphDescriptorBaseKey key)
        {
            return key.ToString();
        }

        public static GraphDescriptorBaseKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(GraphDescriptorBaseKey);
            else
                try
                {
                    var key_separator = stringValue.IndexOf(PlegmaAPI.KeySeparator);

                    //backward compatibility
                    var id = stringValue.Substring(key_separator + 1);
                    if (id.LastIndexOf(PlegmaAPI.KeySeparator) != -1)
                        id = id.Substring(0, id.IndexOf(PlegmaAPI.KeySeparator));

                    var ret = new GraphDescriptorBaseKey(stringValue.Substring(0, key_separator), id);

                    //checks
                    return ret.IsValid ? ret : default(GraphDescriptorBaseKey);
                }
                catch
                {
                    return default(GraphDescriptorBaseKey);
                }
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            UserKey = key.UserKey;
            Id = key.Id;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return UserKey + PlegmaAPI.KeySeparator + Id;
        }
        #endregion

        #region Equality with self
        public bool Equals(GraphDescriptorBaseKey other)
        {
            return this.Equals(ref other);
        }
        public bool Equals(ref GraphDescriptorBaseKey other)
        {
            return this.UserKey == other.UserKey && ToolBox.StringEqualityEx(Id, other.Id);
        }
        public static bool operator ==(GraphDescriptorBaseKey left, GraphDescriptorBaseKey right) { return left.Equals(ref right); }
        public static bool operator !=(GraphDescriptorBaseKey left, GraphDescriptorBaseKey right) { return !left.Equals(ref right); }
        #endregion

        #region Equality with GraphDescriptorKey
        public bool Equals(GraphDescriptorKey other)
        {
            return this.Equals(ref other);
        }
        public bool Equals(ref GraphDescriptorKey other)
        {
            return this == other.BaseKey;
        }
        public static bool operator ==(GraphDescriptorBaseKey left, GraphDescriptorKey right) { return left.Equals(ref right); }
        public static bool operator !=(GraphDescriptorBaseKey left, GraphDescriptorKey right) { return !left.Equals(ref right); }
        #endregion

        #region Equality
        public override bool Equals(object obj)
        {
            if (!(obj is GraphDescriptorBaseKey)) return false;
            return Equals((GraphDescriptorBaseKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ UserKey.GetHashCode();
            hash = (hash * 397) ^ (Id == null ? 0 : Id.GetHashCode());
            return hash;
        }
        #endregion
    }
    #endregion

    #region GraphDescriptorKey

    /// <summary>
    /// Globally unique identifier of a <see cref="GraphDescriptor"/>
    /// </summary>
#if NETFX
    [TypeConverter(typeof(GraphDescriptorKeyConverter))]
#endif
    public struct GraphDescriptorKey : IEquatable<GraphDescriptorKey>
    {
        public  /*readonly*/ GraphDescriptorBaseKey BaseKey;
        public  /*readonly*/ int Revision;

        public GraphDescriptorKey(UserKey key, string name, int rev) { this.BaseKey = new GraphDescriptorBaseKey(key, name); this.Revision = rev; }
        public GraphDescriptorKey(GraphDescriptorBaseKey desc, int rev) { this.BaseKey = desc; this.Revision = rev; }

        [JsonIgnore]
        public bool IsValid { get { return Revision != default(int) && BaseKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        [JsonIgnore]
        public UserKey UserKey { get { return BaseKey.UserKey; } }
        [JsonIgnore]
        public string Id { get { return BaseKey.Id; } }

        #region Implicit conversion to/from string key
        public static implicit operator GraphDescriptorKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(GraphDescriptorKey key)
        {
            return key.ToString();
        }

        public static GraphDescriptorKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(GraphDescriptorKey);
            else
                try
                {
                    int revision = 0;
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    if (!int.TryParse(stringValue.Substring(key_separator + 1), out revision))
                        return default(GraphDescriptorKey);

                    var userkeyAndId = stringValue.Substring(0, key_separator);

                    key_separator = userkeyAndId.LastIndexOf(PlegmaAPI.KeySeparator);
                    var ret = new GraphDescriptorKey(userkeyAndId.Substring(0, key_separator), userkeyAndId.Substring(key_separator + 1), revision);

                    //checks
                    return ret.IsValid ? ret : default(GraphDescriptorKey);
                }
                catch
                {
                    return default(GraphDescriptorKey);
                }
        }
        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return BaseKey.ToString() + PlegmaAPI.KeySeparator + Revision.ToStringInvariant();
        }
        #endregion

        #region Equality
        public bool Equals(GraphDescriptorKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref GraphDescriptorKey other)
        {
            return this.BaseKey == other.BaseKey && this.Revision == other.Revision;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GraphDescriptorKey)) return false;
            return Equals((GraphDescriptorKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (BaseKey.GetHashCode());
            hash = (hash * 397) ^ (Revision.GetHashCode());
            return hash;
        }

        public static bool operator ==(GraphDescriptorKey left, GraphDescriptorKey right) { return left.Equals(ref right); }
        public static bool operator !=(GraphDescriptorKey left, GraphDescriptorKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class GraphDescriptorKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return GraphDescriptorKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((GraphDescriptorKey)value).ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
#endif
    #endregion

    #endregion

    #region GraphKey
    /// <summary>
    /// Globally unique identifier of a <see cref="Graph"/>
    /// </summary>
#if NETFX
    [TypeConverter(typeof(GraphKeyConverter))]
#endif
    public struct GraphKey : IEquatable<GraphKey>, IFillFromString
    {
        public  /*readonly*/ GraphDescriptorKey GraphDescriptorKey;
        public  /*readonly*/ uint NodeId;

        [JsonIgnore]
        public  /*readonly*/ NodeKey NodeKey { get { return new NodeKey(GraphDescriptorKey.BaseKey.UserKey, NodeId); } }

        public GraphKey(GraphDescriptorKey descriptorKey, uint NodeId) { this.GraphDescriptorKey = descriptorKey; this.NodeId = NodeId; }

        [JsonIgnore]
        public bool IsValid { get { return NodeId != default(uint) && GraphDescriptorKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        public string ToStringEx()
        {
            return "GraphKey:{" + GraphDescriptorKey + ", " + NodeId.ToStringInvariant() + "}";
        }

        #region Implicit conversion to/from string key
        public static implicit operator GraphKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(GraphKey key)
        {
            return key.ToString();
        }

        public static GraphKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(GraphKey);
            else
                try
                {
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    var ret = new GraphKey(stringValue.Substring(0, key_separator), stringValue.Substring(key_separator + 1).ParseToUInt32());
                    return ret.IsValid ? ret : default(GraphKey);
                }
                catch
                {
                    return default(GraphKey);
                }
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            GraphDescriptorKey = key.GraphDescriptorKey;
            NodeId = key.NodeId;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return GraphDescriptorKey + PlegmaAPI.KeySeparator + NodeId.ToStringInvariant();
        }
        #endregion

        #region Equality
        public bool Equals(GraphKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref GraphKey other)
        {
            return GraphDescriptorKey == other.GraphDescriptorKey && NodeId == other.NodeId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GraphKey)) return false;
            return Equals((GraphKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ GraphDescriptorKey.GetHashCode();
            hash = (hash * 397) ^ NodeId.GetHashCode();
            return hash;
        }

        public static bool operator ==(GraphKey left, GraphKey right) { return left.Equals(ref right); }
        public static bool operator !=(GraphKey left, GraphKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class GraphKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return GraphKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((GraphKey)value).ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
#endif
    #endregion

    #endregion

    #region BlockKey
    /// <summary>
    /// Globally unique identifier of a <see cref="Graph"/>'s <see cref="Block"/>
    /// </summary>
    public struct BlockKey : IEquatable<BlockKey>, IFillFromString
    {
        public  /*readonly*/ GraphKey GraphKey;
        public  /*readonly*/ int BlockId;

        public BlockKey(GraphKey GraphKey, int BlockID) { this.GraphKey = GraphKey; this.BlockId = BlockID; }

        [JsonIgnore]
        public bool IsValid { get { return BlockId != default(int) && GraphKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        public string ToStringEx()
        {
            return "BlockKey:{" + GraphKey + ", " + BlockId.ToStringInvariant() + "}";
        }

        #region Implicit conversion to/from string key
        public static implicit operator BlockKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(BlockKey key)
        {
            return key.ToString();
        }

        public static BlockKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(BlockKey);
            else
                try
                {
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    var ret = new BlockKey(stringValue.Substring(0, key_separator), int.Parse(stringValue.Substring(key_separator + 1)));
                    return ret.IsValid ? ret : default(BlockKey);
                }
                catch
                {
                    return default(BlockKey);
                }
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            GraphKey = key.GraphKey;
            BlockId = key.BlockId;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return GraphKey + PlegmaAPI.KeySeparator + BlockId.ToStringInvariant();
        }
        #endregion

        #region Equality
        public bool Equals(BlockKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref BlockKey other)
        {
            return GraphKey == other.GraphKey && BlockId == other.BlockId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BlockKey)) return false;
            return Equals((BlockKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ GraphKey.GetHashCode();
            hash = (hash * 397) ^ BlockId.GetHashCode();
            return hash;
        }

        public static bool operator ==(BlockKey left, BlockKey right) { return left.Equals(ref right); }
        public static bool operator !=(BlockKey left, BlockKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class BlockKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return BlockKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((BlockKey)value).ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
#endif
    #endregion

    #endregion

    #region GroupKey
    /// <summary>
    /// Globally unique identifier of a <see cref="GroupInfo"/>
    /// </summary>
#if NETFX
    [TypeConverter(typeof(GroupKeyConverter))]
#endif
    public struct GroupKey : IEquatable<GroupKey>, IFillFromString
    {
        public /*readonly*/ UserKey UserKey;
        public /*readonly*/ int GroupID;

        //public GroupKey(string userId, int groupId) { this.UserKey = new UserKey(userId); this.GroupID = groupId; }
        public GroupKey(UserKey userKey, int groupId) { this.UserKey = userKey; this.GroupID = groupId; }
        //public GroupKey(string str) { this = str; }

        [JsonIgnore]
        public bool IsValid { get { return GroupID != default(int) && UserKey.IsValid; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        #region Conversion to/from string key

        public static implicit operator GroupKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(GroupKey key)
        {
            return key.ToString();
        }

        public static GroupKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(GroupKey);
            else
                try
                {
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    var ret = new GroupKey(stringValue.Substring(0, key_separator), int.Parse(stringValue.Substring(key_separator + 1)));
                    return ret.IsValid ? ret : default(GroupKey);
                }
                catch
                {
                    return default(GroupKey);
                }
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            UserKey = key.UserKey;
            GroupID = key.GroupID;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return UserKey.ToString() + PlegmaAPI.KeySeparator + GroupID.ToStringInvariant();
        }
        #endregion

        public string ToStringEx()
        {
            return "GroupKey:{" + UserKey + GroupID.ToStringInvariant() + "}";
        }

        #region Equality
        public bool Equals(GroupKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref GroupKey other)
        {
            return UserKey == other.UserKey && GroupID == other.GroupID;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GroupKey)) return false;
            return Equals((GroupKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ UserKey.GetHashCode();
            hash = (hash * 397) ^ GroupID.GetHashCode();
            return hash;
        }

        public static bool operator ==(GroupKey left, GroupKey right) { return left.Equals(ref right); }
        public static bool operator !=(GroupKey left, GroupKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class GroupKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                return GroupKey.ConvertFromString(value as string);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
            {
                return null;
            }

            if (destinationType == typeof(string))
            {
                return ((GroupKey)value).ToString();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }
    }
#endif
    #endregion

    #endregion

    #region DriverKey
    public enum eDriverType
    {
        Unknown = 0,
        LCD
    }

    /// <summary>
    /// Globally unique identifier of a <see cref="Graph"/>'s <see cref="Block"/>
    /// </summary>
    public struct DriverKey : IEquatable<DriverKey>, IFillFromString
    {
        public eDriverType Type;
        public int Id;

        public DriverKey(eDriverType type, int Id) { this.Type = type; this.Id = Id; }

        [JsonIgnore]
        public bool IsValid { get { return Id != default(int) && Type != eDriverType.Unknown; } }
        [JsonIgnore]
        public bool IsInvalid { get { return !IsValid; } }

        public string ToStringEx()
        {
            return "DriverKey:{" + Type + ", " + Id.ToStringInvariant() + "}";
        }

        #region Implicit conversion to/from string key
        public static implicit operator DriverKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(DriverKey key)
        {
            return key.ToString();
        }

        public static DriverKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(DriverKey);
            else
                try
                {
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    var ret = new DriverKey((eDriverType)int.Parse(stringValue.Substring(0, key_separator)), int.Parse(stringValue.Substring(key_separator + 1)));
                    return ret.IsValid ? ret : default(DriverKey);
                }
                catch
                {
                    return default(DriverKey);
                }
        }

        public void FillFromString(string input)
        {
            var key = ConvertFromString(input);
            Type = key.Type;
            Id = key.Id;
        }

        public override string ToString()
        {
            if (IsInvalid)
                return Constants.InvalidKeyString;
            else
                return Type + PlegmaAPI.KeySeparator + Id.ToStringInvariant();
        }
        #endregion

        #region Equality
        public bool Equals(DriverKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref DriverKey other)
        {
            return Type == other.Type && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DriverKey)) return false;
            return Equals((DriverKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ Type.GetHashCode();
            hash = (hash * 397) ^ Id.GetHashCode();
            return hash;
        }

        public static bool operator ==(DriverKey left, DriverKey right) { return left.Equals(ref right); }
        public static bool operator !=(DriverKey left, DriverKey right) { return !left.Equals(ref right); }
        #endregion
    }

    #region Converters
#if NETFX
    public class DriverKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return DriverKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((DriverKey)value).ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
#endif
    #endregion

    #endregion

}
