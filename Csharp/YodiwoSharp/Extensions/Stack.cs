using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public static string ToStringEx(this StackFrame f)
        {
            var msg = "at " + f.GetMethod();
            if (!string.IsNullOrEmpty(f.GetFileName()))
            {
                msg += "(file: " + f.GetFileName() + ":" + f.GetFileLineNumber() + ")";
            }
            return msg;
        }
#endif
        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
