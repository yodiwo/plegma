using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using MsgPack;
using Newtonsoft.Json;
using System.Reflection;


namespace Yodiwo.YPChannel
{
    [JsonConverter(typeof(YPCObjectJsonSerializer))]
    public struct YPCObject : IPackable, IUnpackable
    {
        object _Object;
        public object Object { get { return _Object; } }

        public YPCObject(object obj)
        {
            this._Object = obj;
        }

        #region MessagePack packer
        public void PackToMessage(Packer packer, PackingOptions options)
        {
            //declares
            var _PrimitiveType = _Object == null ? MessageIEValueType.Null : Helpers.Type2Enum.TryGetOrDefaultReadOnly(_Object.GetType(), MessageIEValueType.Object);
            string _ExtType = null;
            object _PrimitiveObject = null;
            byte[] _PackedObject = null;

            //pack object
            if (_Object != null && _PrimitiveType == MessageIEValueType.Object)
            {
                var type = _Object.GetType();
                _ExtType = type.AssemblyQualifiedName_Portable();
                var _packer2 = MessagePackSerializer.Get(type);
                _PackedObject = _packer2.PackSingleObject(_Object);
            }
            else
                _PrimitiveObject = _Object;

            //pack them
            packer.PackArrayHeader(3);
            packer.Pack((byte)_PrimitiveType);
            packer.Pack(_ExtType);
            switch (_PrimitiveType)
            {
                case MessageIEValueType.Null:
                    packer.Pack((Byte)0);
                    break;
                case MessageIEValueType.Object:
                    packer.Pack(_PackedObject);
                    break;
                case MessageIEValueType.String:
                    packer.Pack((string)_PrimitiveObject);
                    break;
                case MessageIEValueType.Byte:
                    packer.Pack((Byte)_PrimitiveObject);
                    break;
                case MessageIEValueType.Boolean:
                    packer.Pack((Boolean)_PrimitiveObject);
                    break;
                case MessageIEValueType.Int16:
                    packer.Pack((Int16)_PrimitiveObject);
                    break;
                case MessageIEValueType.Int32:
                    packer.Pack((Int32)_PrimitiveObject);
                    break;
                case MessageIEValueType.Int64:
                    packer.Pack((Int64)_PrimitiveObject);
                    break;
                case MessageIEValueType.UInt16:
                    packer.Pack((UInt16)_PrimitiveObject);
                    break;
                case MessageIEValueType.UInt32:
                    packer.Pack((UInt32)_PrimitiveObject);
                    break;
                case MessageIEValueType.UInt64:
                    packer.Pack((UInt64)_PrimitiveObject);
                    break;
                case MessageIEValueType.Single:
                    packer.Pack((Single)_PrimitiveObject);
                    break;
                case MessageIEValueType.Double:
                    packer.Pack((Double)_PrimitiveObject);
                    break;
            }
        }

        public void UnpackFromMessage(Unpacker unpacker)
        {
            //get primitive type
            byte pb; unpacker.ReadByte(out pb);
            var _PrimitiveType = (MessageIEValueType)pb;

            //get Extended Type
            string _ExtType;
            unpacker.ReadString(out _ExtType);

            //unpack object like a monkey
            _Object = null;
            switch (_PrimitiveType)
            {
                case MessageIEValueType.Null:
                    {
                        Byte data;
                        unpacker.ReadByte(out data);
                        _Object = null;
                        break;
                    }
                case MessageIEValueType.Object:
                    {
                        byte[] data;
                        unpacker.ReadBinary(out data);
                        //unpack packed object
                        var extType = Yodiwo.TypeCache.GetType(_ExtType);
                        if (extType == null)
                            DebugEx.Assert("Could not determine Extented Type for YPCObject (type='" + _ExtType + "')");
                        else
                        {
                            var packer = MessagePackSerializer.Get(extType);
                            try { _Object = packer.UnpackSingleObject(data); }
                            catch (Exception ex) { DebugEx.Assert(ex, "Could not unpack YPCObject type"); }
                        }
                        break;
                    }
                case MessageIEValueType.String:
                    {
                        string data;
                        unpacker.ReadString(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.Byte:
                    {
                        Byte data;
                        unpacker.ReadByte(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.Boolean:
                    {
                        Boolean data;
                        unpacker.ReadBoolean(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.Int16:
                    {
                        Int16 data;
                        unpacker.ReadInt16(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.Int32:
                    {
                        Int32 data;
                        unpacker.ReadInt32(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.Int64:
                    {
                        Int64 data;
                        unpacker.ReadInt64(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.UInt16:
                    {
                        UInt16 data;
                        unpacker.ReadUInt16(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.UInt32:
                    {
                        UInt32 data;
                        unpacker.ReadUInt32(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.UInt64:
                    {
                        UInt64 data;
                        unpacker.ReadUInt64(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.Single:
                    {
                        Single data;
                        unpacker.ReadSingle(out data);
                        _Object = data;
                        break;
                    }
                case MessageIEValueType.Double:
                    {
                        Double data;
                        unpacker.ReadDouble(out data);
                        _Object = data;
                        break;
                    }
            }
        }
        #endregion

        #region JsonConverter
        public class YPCObjectJsonSerializer : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                //declares
                var yobj = (YPCObject)value;
                var _Object = yobj.Object;
                var _PrimitiveType = _Object == null ? MessageIEValueType.Null : Helpers.Type2Enum.TryGetOrDefaultReadOnly(_Object.GetType(), MessageIEValueType.Object);
                string _ExtType = null;
                object _PrimitiveObject = null;

                //pack object
                if (_Object != null && _PrimitiveType == MessageIEValueType.Object)
                {
                    var type = _Object.GetType();
                    _ExtType = type.AssemblyQualifiedName_Portable();
                }
                else
                    _PrimitiveObject = _Object;

                //pack
                writer.WriteStartObject();
                writer.WritePropertyName("Type");
                writer.WriteValue(_PrimitiveType.ToStringInvariant());
                writer.WritePropertyName("ExtType");
                writer.WriteValue(_ExtType);
                writer.WritePropertyName("Value");
                serializer.Serialize(writer, _Object);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                //Load JObject from reader
                var jObject = Newtonsoft.Json.Linq.JObject.Load(reader);

                //get primitive type
                MessageIEValueType _PrimitiveType = (MessageIEValueType)Enum.Parse(typeof(MessageIEValueType), jObject["Type"].ToStringInvariant());

                //get Extended Type
                string _ExtType = jObject["ExtType"].ToStringInvariant();

                //unpack object like a monkey
                const string jvalueKey = "Value";
                object _Object = null;
                switch (_PrimitiveType)
                {
                    case MessageIEValueType.Null:
                        {
                            _Object = null;
                            break;
                        }
                    case MessageIEValueType.Object:
                        {
                            //TODO
                            DebugEx.Assert("TODO");
                            _Object = null;
                            break;
                        }
                    case MessageIEValueType.String:
                        {
                            _Object = jObject.Value<string>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.Byte:
                        {
                            _Object = jObject.Value<byte>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.Boolean:
                        {
                            _Object = jObject.Value<bool>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.Int16:
                        {
                            _Object = jObject.Value<Int16>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.Int32:
                        {
                            _Object = jObject.Value<Int32>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.Int64:
                        {
                            _Object = jObject.Value<Int64>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.UInt16:
                        {
                            _Object = jObject.Value<UInt16>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.UInt32:
                        {
                            _Object = jObject.Value<UInt32>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.UInt64:
                        {
                            _Object = jObject.Value<UInt64>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.Single:
                        {
                            _Object = jObject.Value<float>(jvalueKey);
                            break;
                        }
                    case MessageIEValueType.Double:
                        {
                            _Object = jObject.Value<double>(jvalueKey);
                            break;
                        }
                }

                return new YPCObject(_Object);
            }

            public override bool CanConvert(Type objectType)
            {
#if NETFX
                return typeof(JsonSerializer).IsAssignableFrom(objectType);
#else
                return typeof(JsonSerializer).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
#endif
            }
        }
        #endregion
    }
}
