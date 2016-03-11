using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Spike;
using Yodiwo.Logic.Blocks.A2MCU;
using Yodiwo.Tools;

namespace Yodiwo.Spike
{

    public abstract class SpikeTlvSubContainer
    {
        public byte[] PayloadBytes;
        public object Payload;
    }


    public class SpikeContainerI2c
    {
        public SpikeI2CTlv Header;
        public byte[] Data;
    }

    public class SpikeContainerTlv
    {
        public SpikeTlvHeader Header;
        public object StdPayload;
        public byte[] VarPayloadBytes;
        public object VarPayload;

        public byte[] Pad(byte[] bytes)
        {
            var pad = (4 - (bytes.Length % 4)) % 4;
            return (pad > 0) ? bytes.Concat(new byte[pad]).ToArray() : bytes;
        }

        public byte[] ToBytes(Endianness endianness)
        {
            var bytes = Marshalling.ToBytes(Header, endianness);
            switch (Header.Type)
            {
                case TlvTypes.GetValue:
                case TlvTypes.ReadConf:
                    throw new NotImplementedException("not yet");
                case TlvTypes.WriteConf:
                    DebugEx.Assert(VarPayloadBytes != null, "writeconf must contain byte payload at this time");
                    bytes = bytes.Concat(Marshalling.ToBytes(StdPayload, endianness)).Concat(VarPayloadBytes).ToArray();
                    break;
                case TlvTypes.SetValue:
                    DebugEx.Assert(VarPayloadBytes != null, "SetValue must contain byte payload at this time");
                    bytes = bytes.Concat(Marshalling.ToBytes(StdPayload, endianness)).Concat(VarPayloadBytes).ToArray();
                    break;
                case TlvTypes.ActivationStatus:
                    bytes = bytes.Concat(Marshalling.ToBytes(StdPayload, endianness)).ToArray();
                    break;
                case TlvTypes.Reserved:
                default:
                    DebugEx.Assert("Unexpected TLV type");
                    break;
            }
            return Pad(bytes);
        }

        public static int FromBytes(byte[] bytes, int offset, out SpikeContainerTlv tlv, Endianness endianness)
        {
            tlv = new SpikeContainerTlv();
            var size = Marshalling.ToObject(bytes, offset, out tlv.Header, endianness);
            if (size < 0)
            {
                tlv = null;
                return -1;
            }
            switch (tlv.Header.Type)
            {
                case TlvTypes.GetValue:
                case TlvTypes.ReadConf:
                case TlvTypes.WriteConf:
                    throw new NotImplementedException("not yet, please come back later");
                case TlvTypes.SetValue:
                    SpikeTlvSetValueHeader setValHeader;
                    var tmp = Marshalling.ToObject(bytes, offset + size, out setValHeader, endianness);
                    if (tmp < 0)
                    {
                        tlv = null;
                        return -1;
                    }
                    size += tmp;
                    var valLength = tlv.Header.Length - Marshal.SizeOf(setValHeader);
                    var payload = new byte[valLength];
                    Buffer.BlockCopy(bytes, offset + size, payload, 0, valLength);
                    size += valLength;
                    tlv.StdPayload = setValHeader;
                    tlv.VarPayload = payload;
                    break;
                case TlvTypes.Reserved:
                default:
                    DebugEx.Assert("unexpected Tlv type");
                    return -1;
            }
            return size;
        }

        public static SpikeContainerTlv MakeWriteConfig(uint thing, SpikeConfig config)
        {
            var val = SpikePort.ConvertToSpikeType(config.Value, config.Type);
            //TODO handle string config
            DebugEx.Assert(val.GetType() != typeof(string), "can't marshal strings");
            var valBytes = Marshalling.ToBytes(val, Endianness.BigEndian);

            var nameBytes = SpikeMessage.Encoding.GetBytes(config.Name);

            return new SpikeContainerTlv
            {
                Header = new SpikeTlvHeader
                {
                    Length = (UInt16)(Marshal.SizeOf(typeof(SpikeTlvWriteConfHeader)) + valBytes.Length + nameBytes.Length),
                    Type = TlvTypes.WriteConf,
                },
                StdPayload = new SpikeTlvWriteConfHeader
                {
                    ThingId = thing,
                    NameLength = (byte)nameBytes.Length,
                    Type = (byte)config.Type,
                    ValueLength = (byte)valBytes.Length,
                    Pad1 = 0,
                },
                VarPayloadBytes = valBytes.Concat(nameBytes).ToArray(),
            };
        }

        public static SpikeContainerTlv MakeWriteDriconf(uint thing, SpikeDriconf driconf)
        {
            //TODO wip
            byte[] bytes;
            //switch (driconf.Type)
            //{
            //    case eSpikeDriconfTypes.InputSimple:
            //        break;
            //    case eSpikeDriconfTypes.InputI2c:
            //        var i2c = (SpikeDriconfInputI2c)driconf.Data;
            //        bytes = Marshalling.GetBytes(i2c, Endianness.BigEndian);
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
            bytes = Marshalling.ToBytes(driconf.Data, Endianness.BigEndian);
            return new SpikeContainerTlv
            {
                Header = new SpikeTlvHeader
                {
                    Type = TlvTypes.WriteDriconf,
                    Length = (UInt16)(Marshal.SizeOf(typeof(SpikeTlvWriteConfHeader)) + bytes.Length),
                },
                StdPayload = new SpikeTlvWriteDriconfHeader
                {
                    ThingId = thing,
                    Type = (UInt16)driconf.Type,
                },
                VarPayloadBytes = bytes,
            };
        }

        #region MakeSetValue

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, byte[] value)
        {
            var len = value.Length;

            return new SpikeContainerTlv
            {
                Header = new SpikeTlvHeader
                {
                    Length = (UInt16)(len + Marshal.SizeOf(typeof(SpikeTlvSetValueHeader))),
                    Type = TlvTypes.SetValue,
                },
                StdPayload = new SpikeTlvSetValueHeader
                {
                    ThingId = thing,
                    PortId = port,
                    pad1 = 0,
                    pad2 = 0,
                    pad3 = 0,
                },
                VarPayloadBytes = value,
            };
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, bool value)
        {
            return MakeSetValue(thing, port, new[] { (byte)(value ? 1 : 0) });
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, byte value)
        {
            return MakeSetValue(thing, port, new[] { value });
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, sbyte value)
        {
            return MakeSetValue(thing, port, new[] { (byte)value });
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, UInt16 value)
        {
            return MakeSetValue(thing, port, BigBitConverter.GetBytes(value));
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, Int16 value)
        {
            return MakeSetValue(thing, port, BigBitConverter.GetBytes(value));
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, UInt32 value)
        {
            return MakeSetValue(thing, port, BigBitConverter.GetBytes(value));
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, Int32 value)
        {
            return MakeSetValue(thing, port, BigBitConverter.GetBytes(value));
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, UInt64 value)
        {
            return MakeSetValue(thing, port, BigBitConverter.GetBytes(value));
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, Int64 value)
        {
            return MakeSetValue(thing, port, BigBitConverter.GetBytes(value));
        }

        public static SpikeContainerTlv MakeSetValue(uint thing, byte port, SpikeI2CTlv meta, byte[] data)
        {
            var bytes = Marshalling.ToBytes(meta, Endianness.BigEndian);
            if (data != null)
                bytes = bytes.Concat(data).ToArray();
            return MakeSetValue(thing, port, bytes);
        }

        public static SpikeContainerTlv MakeSetValueAuto(SpikeThing sthing, byte port, string value, ePortType portType)
        {
            DebugEx.Assert(sthing != null && sthing.Ports.Length > port, "Invalid SpikeThing or port");
            if (sthing == null || sthing.Ports.Length <= port)
                return null;
            switch (sthing.Ports[port].Type)
            {
                case eSpikeValueTypes.BOOLEAN:
                    var bl = ConvertEx.Convert<bool>(value);
                    return MakeSetValue(sthing.SpikeThingId, port, bl);
                case eSpikeValueTypes.UINT8:
                    byte b;
                    if (Byte.TryParse(value, out b))
                        return MakeSetValue(sthing.SpikeThingId, port, b);
                    break;
                case eSpikeValueTypes.UINT16:
                    UInt16 u16;
                    if (UInt16.TryParse(value, out u16))
                        return MakeSetValue(sthing.SpikeThingId, port, u16);
                    break;
                case eSpikeValueTypes.UINT32:
                    UInt32 u32;
                    if (UInt32.TryParse(value, out u32))
                        return MakeSetValue(sthing.SpikeThingId, port, u32);
                    break;
                case eSpikeValueTypes.UINT64:
                    UInt64 u64;
                    if (UInt64.TryParse(value, out u64))
                        return MakeSetValue(sthing.SpikeThingId, port, u64);
                    break;
                case eSpikeValueTypes.INT8:
                    sbyte i8;
                    if (SByte.TryParse(value, out i8))
                        return MakeSetValue(sthing.SpikeThingId, port, i8);
                    break;
                case eSpikeValueTypes.INT16:
                    Int16 i16;
                    if (Int16.TryParse(value, out i16))
                        return MakeSetValue(sthing.SpikeThingId, port, i16);
                    break;
                case eSpikeValueTypes.INT32:
                    Int32 i32;
                    if (Int32.TryParse(value, out i32))
                        return MakeSetValue(sthing.SpikeThingId, port, i32);
                    break;
                case eSpikeValueTypes.INT64:
                    Int64 i64;
                    if (Int64.TryParse(value, out i64))
                        return MakeSetValue(sthing.SpikeThingId, port, i64);
                    break;
                case eSpikeValueTypes.FLOAT:
                case eSpikeValueTypes.DOUBLE:
                    throw new NotImplementedException();
                case eSpikeValueTypes.BLOB:
                    var bytes = SpikeMessage.Encoding.GetBytes(value);
                    return MakeSetValue(sthing.SpikeThingId, port, bytes);
                case eSpikeValueTypes.STRING:
                case eSpikeValueTypes.NTSTRING:
                    throw new NotImplementedException();
                case eSpikeValueTypes.I2C:
                    var command = value.FromJSON<I2CCommand>();
                    var len = (UInt16)((command.IsWrite) ? command.value.Length : command.ReadLength);
                    var toSpike = new SpikeI2CTlv
                    {
                        DeviceAddress = command.devaddress,
                        RegisterAddress = command.register,
                        isWrite = (byte)(command.IsWrite ? 1 : 0),
                        Length = len,
                        //                        Pad = new byte[3] {0,0,0},
                    };
                    return MakeSetValue(sthing.SpikeThingId, port, toSpike, command.value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }

        #endregion

        public static SpikeContainerTlv MakeActivationStatus(uint thing, bool status)
        {
            return new SpikeContainerTlv
            {
                Header = new SpikeTlvHeader
                {
                    Length = (UInt16)(Marshal.SizeOf(typeof(SpikeTlvActivationStatus))),
                    Type = TlvTypes.ActivationStatus,
                },
                StdPayload = new SpikeTlvActivationStatus
                {
                    ThingId = thing,
                    Status = (byte)(status ? 1 : 0),
                },
            };
        }

        public static object GetSetValuePayloadFromBytes(byte[] bytes, int offset, int size, eSpikeValueTypes type)
        {
            object retval = null;
            int headerSize;
            switch (type)
            {
                case eSpikeValueTypes.BOOLEAN:
                    bool b = (bytes[offset] > 0) ? true : false;
                    //headerSize = Marshalling.FromBytes(bytes, offset, out b, Endianness.BigEndian);
                    retval = b;
                    break;
                case eSpikeValueTypes.I2C:

                    SpikeI2CTlv i2c;
                    headerSize = Marshalling.ToObject(bytes, offset, out i2c, Endianness.BigEndian);
                    if (headerSize < 0)
                        break; //error, return null
                    var dataSize = size - headerSize;
                    byte[] data = new byte[dataSize];
                    Buffer.BlockCopy(bytes, offset + headerSize, data, 0, dataSize);
                    SpikeContainerI2c container = new SpikeContainerI2c
                    {
                        Header = i2c,
                        Data = data,
                    };
                    retval = container;
                    break;
            }
            return retval;
        }
    }
}
