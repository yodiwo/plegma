using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Type GetMemberType(this MemberInfo mi)
        {
            if (mi is FieldInfo)
                return (mi as FieldInfo).FieldType;
            else if (mi is PropertyInfo)
                return (mi as PropertyInfo).PropertyType;
            else
                return null;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsStatic(this MemberInfo mi)
        {
#if NETFX
            if (mi is Type)
                return (mi as Type).IsAbstract;
            else
#endif
            if (mi is FieldInfo)
                return (mi as FieldInfo).IsStatic;
            else if (mi is PropertyInfo)
            {
#if NETFX
                var set = (mi as PropertyInfo).GetSetMethod(true);
#elif UNIVERSAL
                var set = (mi as PropertyInfo).SetMethod;
#endif
                if (set != null)
                    return set.IsStatic;
                else
                {
#if NETFX
                    var get = (mi as PropertyInfo).GetGetMethod(true);
#elif UNIVERSAL
                    var get = (mi as PropertyInfo).GetMethod;
#endif
                    if (get != null)
                        return get.IsStatic;
                    else
                        return true;
                }
            }
            else
                return false;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsReadable(this MemberInfo mi)
        {
            if (mi is FieldInfo)
                return true;
            else if (mi is PropertyInfo)
            {
#if NETFX
                var get = (mi as PropertyInfo).GetGetMethod(true);
#elif UNIVERSAL
                var get = (mi as PropertyInfo).GetMethod;
#endif
                return get != null;
            }
            else
                return true;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
