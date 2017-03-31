using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class Marshalling
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static object ToObject(Type type, byte[] buffer, int index = 0)
        {
            int size = Marshal.SizeOf(type);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(buffer, index, ptr, size);

            var obj = Marshal.PtrToStructure(ptr, type);
            Marshal.FreeHGlobal(ptr);

            return obj;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T ToObject<T>(byte[] buffer, int index = 0)
        {
            return (T)ToObject(typeof(T), buffer, index: index);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T[] ToObjects<T>(Type type, byte[] buffer, int count, int index = 0)
        {
            int size = Marshal.SizeOf(type);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            var arr = Array.CreateInstance(type, count);
            for (int n = 0; n < count; n++)
            {
                Marshal.Copy(buffer, index + size * n, ptr, size);
                arr.SetValue((T)Marshal.PtrToStructure(ptr, type), n);
            }

            Marshal.FreeHGlobal(ptr);

            return arr as T[];
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static int ToObject<T>(byte[] buffer, int index, out T typed, Endianness fromEndianness = Endianness.Unkown) where T : new()
        {
            var type = typeof(T);
            var size = Marshal.SizeOf(type);
            if ((fromEndianness == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
                (fromEndianness == Endianness.LittleEndian && !BitConverter.IsLittleEndian))
            {
                To(buffer, index, type, fromEndianness);
            }

            typed = ToObject<T>(buffer, index: index);

            return size;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T[] ToObjects<T>(byte[] buffer, int count, int index = 0)
        {
            return ToObjects<T>(typeof(T), buffer, count, index: index);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static byte[] ToBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            var arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static byte[] ToBytes(object obj, Endianness endianness = Endianness.Unkown)
        {
            var bytes = ToBytes(obj);

            if ((endianness == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
                (endianness == Endianness.LittleEndian && !BitConverter.IsLittleEndian))
            {
                To(bytes, 0, obj.GetType(), endianness);
            }
            return bytes;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static void To(byte[] data, int offset, int count, Endianness endianness)
        {
            if (BitConverter.IsLittleEndian == (endianness == Endianness.LittleEndian))
                return;
            Array.Reverse(data, offset, count);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------

        //exploiting symmetry in endinanness change operations, parameter works as from or to (endianness)
        public static void To(byte[] bytes, int offset, Type originalType, Endianness endianness)
        {
#if NETFX
            if (originalType.IsEnum)
#elif UNIVERSAL
            if (originalType.GetTypeInfo().IsEnum)
#endif
                originalType = Enum.GetUnderlyingType(originalType);

#if NETFX
            if (originalType.IsPrimitive)
#elif UNIVERSAL
            if (originalType.GetTypeInfo().IsPrimitive)
#endif
            {
                if (originalType == typeof(byte) || originalType == typeof(sbyte))
                {
                    return;
                }
                else if (originalType == typeof(Int16) || originalType == typeof(UInt16))
                {
                    To(bytes, offset, sizeof(Int16), endianness);
                }
                else if (originalType == typeof(Int32) || originalType == typeof(UInt32))
                {
                    To(bytes, offset, sizeof(Int32), endianness);
                }
                else if (originalType == typeof(Int64) || originalType == typeof(UInt64))
                {
                    To(bytes, offset, sizeof(Int64), endianness);
                }
                // do not change anything for other types (?? string... what else?)
                return;
            }
            else
            {
                var subfields = originalType.GetFields().Where(sf => !sf.IsStatic);
                foreach (var sf in subfields)
                {
                    var suboff = (int)Marshal.OffsetOf(originalType, sf.Name);
                    To(bytes, offset + suboff, sf.FieldType, endianness);
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static void To(byte[] data, Type originalType, string fieldName, Endianness endianness)
        {
            var fieldInfo = originalType.GetField(fieldName);
            To(data, originalType, fieldInfo, endianness);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static void To(byte[] data, Type originalType, FieldInfo field, Endianness endianness)
        {
            var off = (int)Marshal.OffsetOf(originalType, field.Name);
            var count = Marshal.SizeOf(field.FieldType);
            To(data, off, count, endianness);

        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
