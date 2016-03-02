using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static Object HtmlEncodeObject(this object obj)
        {
            Tools.Html.HtmlEncodeObject(ref obj);
            return obj;
        }

        public static Object HtmlDecodeObject(this object obj)
        {
            Tools.Html.HtmlDecodeObject(ref obj);
            return obj;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
