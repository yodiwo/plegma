using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class Mashalling
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
    }
}
