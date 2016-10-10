using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Yodiwo
{
    public static partial class Extensions
    {
        const int MaxDepth = 200;

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string ToJSON(this Object obj, bool HtmlEncode = false)
        {
            return JsonConvert.SerializeObject(obj, HtmlEncode ? createSettings(true, true, null) : createSettings(true, false, null));
        }

        public static byte[] ToJSON2(this Object obj, bool HtmlEncode = false)
        {
            var str = obj.ToJSON(HtmlEncode: HtmlEncode);
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static object FromJSON(this string json, bool HtmlDecode = false, bool AllowExtendedTypes = false, HashSet<Type> AllowedTypes = null)
        {
            if (json == null) return null;
            if (HtmlDecode) json = json.HtmlDecode();
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json, AllowExtendedTypes ? createSettingsUnsafe(AllowedTypes) : createSettingsSafe(AllowedTypes));
        }

        public static T FromJSON<T>(this string json, bool HtmlDecode = false, bool AllowExtendedTypes = false, HashSet<Type> AllowedTypes = null)
        {
            if (json == null) return default(T);
            if (HtmlDecode) json = json.HtmlDecode();
            return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json, AllowExtendedTypes ? createSettingsUnsafe(AllowedTypes) : createSettingsSafe(AllowedTypes));
        }

        public static object FromJSON(this string json, Type type, bool HtmlDecode = false, bool AllowExtendedTypes = false, HashSet<Type> AllowedTypes = null)
        {
            if (json == null) return null;
            if (HtmlDecode) json = json.HtmlDecode();
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json, type, AllowExtendedTypes ? createSettingsUnsafe(AllowedTypes) : createSettingsSafe(AllowedTypes));
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static object FromJSON(this byte[] json, bool HtmlDecode = false, HashSet<Type> AllowedTypes = null)
        {
            if (json == null || json.Length == 0) return null;
#if NETFX
            var str = System.Text.Encoding.UTF8.GetString(json);
#elif UNIVERSAL
            var str = System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
#endif
            if (HtmlDecode) str = str.HtmlDecode();
            return string.IsNullOrEmpty(str) ? null : JsonConvert.DeserializeObject(str, createSettingsSafe(AllowedTypes));
        }

        public static T FromJSON<T>(this byte[] json, bool HtmlDecode = false, HashSet<Type> AllowedTypes = null)
        {
            if (json == null || json.Length == 0) return default(T);
#if NETFX
            var str = System.Text.Encoding.UTF8.GetString(json);
#elif UNIVERSAL
            var str = System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
#endif
            if (HtmlDecode) str = str.HtmlDecode();
            return string.IsNullOrEmpty(str) ? default(T) : JsonConvert.DeserializeObject<T>(str, createSettingsSafe(AllowedTypes));
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        static JsonSerializerSettings createSettingsSafe(HashSet<Type> AllowedTypes) { return createSettings(false, false, AllowedTypes); }
        static JsonSerializerSettings createSettingsUnsafe(HashSet<Type> AllowedTypes) { return createSettings(true, false, AllowedTypes); }

        static JsonSerializerSettings createSettings(bool AllowExtendedTypes, bool HtmlEscape, HashSet<Type> AllowedTypes)
        {
#if UNIVERSAL
            DebugEx.Assert(AllowedTypes == null, "AllowedTypes are not supported yet");
#endif
            return new JsonSerializerSettings()
            {
                TypeNameHandling = AllowExtendedTypes ? TypeNameHandling.Auto : Newtonsoft.Json.TypeNameHandling.None,
#if NETFX
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
#endif
                StringEscapeHandling = HtmlEscape ? StringEscapeHandling.EscapeHtml : StringEscapeHandling.Default,
                Formatting = Formatting.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                FloatParseHandling = FloatParseHandling.Double,
                FloatFormatHandling = FloatFormatHandling.String,
                Culture = System.Globalization.CultureInfo.InvariantCulture,
                MaxDepth = MaxDepth,
                AllowedTypes = AllowedTypes,
            };
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
