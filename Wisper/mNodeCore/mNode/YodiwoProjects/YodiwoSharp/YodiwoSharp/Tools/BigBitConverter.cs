using System;
using System.Linq;

namespace Yodiwo.Tools
{
    public static class BigBitConverter
    {
        public static UInt16 ToUInt16(byte[] bytes, int startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (UInt16) ((bytes[startIndex] << 8) + bytes[startIndex + 1]);
            }
            else
            {
                return BitConverter.ToUInt16(bytes, startIndex);
            }
        }

        public static void PutBytesOfUInt16(byte[] bytes, int startIndex, UInt16 value)
        {
            bytes[startIndex] = (byte)(value >> 8);
            bytes[startIndex + 1] = (byte)(value & 0xff);
        }

        public static byte[] GetBytes(byte value)
        {
            return new[] {value};
        }

        public static byte[] GetBytes(sbyte value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(UInt16 value)
        {
            var v = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? v.Reverse().ToArray() : v;
        }

        public static byte[] GetBytes(Int16 value)
        {
            var v = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? v.Reverse().ToArray() : v;
        }

        public static byte[] GetBytes(UInt32 value)
        {
            var v = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? v.Reverse().ToArray() : v;
        }

        public static byte[] GetBytes(Int32 value)
        {
            var v = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? v.Reverse().ToArray() : v;
        }

        public static byte[] GetBytes(UInt64 value)
        {
            var v = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? v.Reverse().ToArray() : v;
        }

        public static byte[] GetBytes(Int64 value)
        {
            var v = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? v.Reverse().ToArray() : v;
        }

    }
}