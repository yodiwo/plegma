using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Delegate CreateDelegate(this MethodInfo mi, object Target)
        {
            DebugEx.Assert(mi != null, "Could not create delegate");
            try
            {
                if (mi == null)
                    return null;

                Type delegateType;

                var typeArgs = mi.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToList();

                //create delegate type
                delegateType = System.Linq.Expressions.Expression.GetDelegateType
                                    (
                                        mi.GetParameters()
                                        .Select(p => p.ParameterType)
                                        .Concat(new Type[] { mi.ReturnType })
                                        .ToArray()
                                    );
#if NETFX
                // creates a binded delegate if target is supplied
                var result = (Target == null | mi.IsStatic)
                    ? Delegate.CreateDelegate(delegateType, mi)
                    : Delegate.CreateDelegate(delegateType, Target, mi);
#else
                var result = (Target == null | mi.IsStatic)
                    ? mi.CreateDelegate(delegateType, null)
                    : mi.CreateDelegate(delegateType, Target);
#endif
                return result;
            }
            catch// (Exception ex)
            {
                return null;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
