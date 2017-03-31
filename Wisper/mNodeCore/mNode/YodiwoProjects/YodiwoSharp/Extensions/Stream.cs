using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static byte[] ReadToEnd<T>(this T stream) where T : Stream
        {
            //find remaining length
            var len = (stream.Length - stream.Position);
            if (len <= 0)
            {
                DebugEx.Assert(len >= 0, "Negative length detected");
                return new byte[0];
            }
            //read
            var buffer = new byte[len];
            var offset = 0;
            while (offset < len)
                offset += stream.Read(buffer, offset, (int)(len - offset));
            return buffer;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
