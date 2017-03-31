using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    public static class PortStateSemantics
    {
        #region Boolean
        public enum Boolean_Format : int
        {
            TrueFalse = 0,
            OnOff,
            YesNo,
            EnabledDisabled,
            EnableDisable,
            ActiveInactive,
            ActivateDeactivate,
            ActivatedDeactivated,
        }
        #endregion

        #region String
        public enum String_Case : int
        {
            None = 0,
            Lower,
            Upper
        }
        #endregion

        #region Decimal
        public struct Decimal_Range
        {
            public double Min;
            public double Max;
        }
        #endregion

        #region Integer
        public struct Integer_Range
        {
            public Int64 Min;
            public Int64 Max;
        }
        #endregion

        #region Helpers
        public static string BuildSemantics(string key, object value)
        {
            return BuildSemantics(new Dictionary<string, object>() { { key, value } });
        }

        public static string BuildSemantics(string key1, object value1, string key2, object value2)
        {
            return BuildSemantics(new Dictionary<string, object>() { { key1, value1 }, { key2, value2 } });
        }

        public static string BuildSemantics(IDictionary<string, object> semantics)
        {
            var strings = new Dictionary<string, string>();
            foreach (var entry in semantics)
            {
                if (entry.Value == null)
                    continue;
                var type = entry.Value.GetType();
                if (type == typeof(string))
                    strings.Add(entry.Key, entry.Value as string);
#if NETFX
                else if (type.IsEnum)
#elif UNIVERSAL
                else if (type.GetTypeInfo().IsEnum)
#endif
                    strings.Add(entry.Key, ((int)entry.Value).ToStringInvariant());
                else if (type.IsNumber())
                    strings.Add(entry.Key, entry.ToStringInvariant());
                else
                    strings.Add(entry.Key, entry.Value.ToJSON());
            }
            return strings.ToJSON(HtmlEncode: false);
        }

        public static Dictionary<string, object> GetSemantics(string semantics)
        {
            //checks
            if (string.IsNullOrWhiteSpace(semantics))
                return null;
            //deserialize
            var des = semantics.FromJSON<Dictionary<string, object>>();
            var ret = new Dictionary<string, object>();
            //restore values
            foreach (var __entry in des)
            {
                try
                {
                    var key = __entry.Key;
                    var value = __entry.Value;

                    //fix values (deserialized json from object)
                    if (key == nameof(Boolean_Format))
                        value = Enum.Parse(typeof(Boolean_Format), value.ToStringInvariant());
                    else if (key == nameof(String_Case))
                        value = Enum.Parse(typeof(String_Case), value.ToStringInvariant());
                    else if (key == nameof(Decimal_Range))
                        value = (value as string).FromJSON<Decimal_Range>();
                    else if (key == nameof(Integer_Range))
                        value = (value as string).FromJSON<Integer_Range>();
                    //else if ...
                    else DebugEx.Assert("Could not deserialize semantic entry");

                    //add to final dictionary
                    if (value != null)
                        ret.ForceAdd(key, value);
                }
                catch { }
            }
            //return result
            return ret;
        }
        #endregion
    }
}
