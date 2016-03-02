using System;
using System.Collections.Generic;
using System.Linq;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static TimeSpan ToUnix(this DateTime value)
        {
            return value.Subtract(UnixEpoch);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static ulong ToUnixMilli(this DateTime value)
        {
            return (ulong)value.Subtract(UnixEpoch).TotalMilliseconds;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static DateTime FromUnix(int value)
        {
            return UnixEpoch + TimeSpan.FromSeconds(value);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static DateTime FromUnixMilli(ulong value)
        {
            return UnixEpoch + TimeSpan.FromMilliseconds(value);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
