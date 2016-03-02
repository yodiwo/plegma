using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Yodiwo
{
    public static partial class Extensions
    {
        #region Settings
        static JsonSerializerSettings settingsSafe = new JsonSerializerSettings()
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
#if NETFX
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
#endif
            StringEscapeHandling = StringEscapeHandling.Default,
            Formatting = Formatting.None,
        };

        static JsonSerializerSettings settingsUnsafe = new JsonSerializerSettings()
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
#if NETFX
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
#endif
            StringEscapeHandling = StringEscapeHandling.Default,
            Formatting = Formatting.None,
        };


        static JsonSerializerSettings settingsHtmlEscapeSafe = new JsonSerializerSettings()
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
#if NETFX
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
#endif
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            Formatting = Formatting.None,
        };

        static JsonSerializerSettings settingsHtmlEscapeUnsafe = new JsonSerializerSettings()
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
#if NETFX
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
#endif
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            Formatting = Formatting.None,
        };
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string ToJSON(this Object obj, bool HtmlEncode = false)
        {
            return JsonConvert.SerializeObject(obj, HtmlEncode ? settingsHtmlEscapeUnsafe : settingsUnsafe);
        }

        public static byte[] ToJSON2(this Object obj, bool HtmlEncode = false)
        {
            var str = obj.ToJSON(HtmlEncode: HtmlEncode);
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static object FromJSON(this string json, bool HtmlDecode = false, bool AllowExtendedTypes = false)
        {
            if (HtmlDecode) json = json.HtmlDecode();
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json, AllowExtendedTypes ? settingsUnsafe : settingsSafe);
        }

        public static T FromJSON<T>(this string json, bool HtmlDecode = false, bool AllowExtendedTypes = false)
        {
            if (HtmlDecode) json = json.HtmlDecode();
            return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json, AllowExtendedTypes ? settingsUnsafe : settingsSafe);
        }

        public static object FromJSON(this string json, Type type, bool HtmlDecode = false, bool AllowExtendedTypes = false)
        {
            if (HtmlDecode) json = json.HtmlDecode();
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json, type, AllowExtendedTypes ? settingsUnsafe : settingsSafe);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static object FromJSON(this byte[] json, bool HtmlDecode = false)
        {
            if (json == null || json.Length == 0) return null;
#if NETFX
            var str = System.Text.Encoding.UTF8.GetString(json);
#else
            var str = System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
#endif
            if (HtmlDecode) str = str.HtmlDecode();
            return string.IsNullOrEmpty(str) ? null : JsonConvert.DeserializeObject(str, settingsSafe);
        }

        public static T FromJSON<T>(this byte[] json, bool HtmlDecode = false)
        {
            if (json == null || json.Length == 0) return default(T);
#if NETFX
            var str = System.Text.Encoding.UTF8.GetString(json);
#else
            var str = System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
#endif
            if (HtmlDecode) str = str.HtmlDecode();
            return string.IsNullOrEmpty(str) ? default(T) : JsonConvert.DeserializeObject<T>(str, settingsSafe);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
