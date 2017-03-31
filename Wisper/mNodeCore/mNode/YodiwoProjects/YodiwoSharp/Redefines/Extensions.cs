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

        public static Type GetEnumUnderlyingType(this Type enumType)
        {
            return Enum.GetUnderlyingType(enumType);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
