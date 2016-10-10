using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Spike
{
    #region enums
    public enum TlvTypes : ushort
    {
        Reserved = 0,
        GetValue = 1,
        SetValue = 2,
        ReadConf = 3,
        WriteConf = 4,
        ActivationStatus = 5,
        WriteDriconf = 6,
    }

    public enum eSpikeThingType
    {
        Invalid = 0,
        Gpio = 1,
        Spi = 2,
        Sdio = 3,
        I2C = 4,
        I2S = 5,
        Usb = 6,
        Adc = 7,
        Dac = 8,
        Conf = 255
    }

    [Flags]
    public enum eSpikeThingFlags
    {
        None = 0,
        Removable = 1 << 0,
    }

    public enum eSpikeValueTypes
    {
        BOOLEAN = 0,
        UINT8 = 1,
        UINT16 = 2,
        UINT32 = 3,
        UINT64 = 4,
        INT8 = 5,
        INT16 = 6,
        INT32 = 7,
        INT64 = 8,
        FLOAT = 9,
        DOUBLE = 10,
        BLOB = 11,
        STRING = 12,
        NTSTRING = 13,
        I2C = 14,
    }

    public enum eSpikeDriconfTypes
    {
        InputSimple = 0,
        InputI2c = 1,
    }

    #endregion

    #region tlv types
    [StructLayout(LayoutKind.Sequential)]
    public class SpikeTlvHeader
    {
        public TlvTypes Type;
        public UInt16 Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SpikeTlvSetValueHeader
    {
        public UInt32 ThingId;
        public byte PortId;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        //public byte[] Padding;
        public byte pad1;
        public byte pad2;
        public byte pad3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SpikeTlvWriteConfHeader
    {
        public UInt32 ThingId;
        public byte NameLength;
        public byte Type;
        public byte ValueLength;
        public byte Pad1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SpikeTlvActivationStatus
    {
        public UInt32 ThingId;
        public byte Status;
    }

    public class SpikeTlvWriteDriconfHeader
    {
        public UInt32 ThingId;
        public UInt16 Type;
        public UInt16 Lenght;
    }

    #endregion

    #region spike message header
    [StructLayout(LayoutKind.Sequential)]
    public class SpikeMessageHeader
    {
        public UInt16 Length;
        public DeviceKey DeviceKey = new DeviceKey();
        public UInt16 TlvsLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceKey
    {
        public PlatformId PlatformId;
        public UInt32 UniqueId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PlatformId
    {
        public byte ComId;
        public byte FamilyId;
        public UInt16 McuId;
    }
    #endregion

    #region tlv payload structures
    [StructLayout(LayoutKind.Sequential)]
    public class SpikeI2CTlv
    {

        public byte DeviceAddress;
        public byte RegisterAddress;
        public UInt16 Length;
        public byte isWrite;
        public byte pad1;
        public byte pad2;
        public byte pad3;
        //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        //        public byte[] Pad;
    }

    public class SpikeTlvDriconfInput
    {
        public uint SamplingPeriodMs;
        public int IrqId;
    }

    public class SpikeTlvDriconfInputI2c
    {
        public byte SlaveAddress;
        public byte RegisterAddress;
        public byte Length;
    }
    #endregion

}
