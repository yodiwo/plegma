using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string NamePortable(this Assembly assembly)
        {
            var name = assembly.FullName;
            var sep = name.IndexOf(',');
            if (sep == -1)
                return name;
            else
                return name.Substring(0, sep);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
