using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool StartsWith(this string source, string toCheck, bool CaseSensitive)
        {
            return source.StartsWith(toCheck, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public static bool StartsWithInvariant(this string source, string toCheck, bool CaseSensitive)
        {
            return source.StartsWith(toCheck, CaseSensitive, System.Globalization.CultureInfo.InvariantCulture);
        }
#endif
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool EndsWith(this string source, string toCheck, bool CaseSensitive)
        {
            return source.EndsWith(toCheck, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public static bool EndsWithInvariant(this string source, string toCheck, bool CaseSensitive)
        {
            return source.EndsWith(toCheck, CaseSensitive, System.Globalization.CultureInfo.InvariantCulture);
        }
#endif
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Contains(this string source, string toCheck, bool CaseSensitive)
        {
            return source.IndexOf(toCheck, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public static bool ContainsInvariant(this string source, string toCheck, bool CaseSensitive)
        {
            return source.IndexOf(toCheck, CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
#endif
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static string RemoveLast(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            else if (value.Length <= 1)
                return "";
            else
                return value.Remove(value.Length - 1);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static string RemoveLast(this string value, int count)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            else if (count == 0)
                return value;
            else if (count >= value.Length)
                return "";
            else
                return value.Remove(value.Length - count);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static float ParseToByte(this string value, byte Default = 0)
        {
            byte res;
            if (byte.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Char ParseToChar(this string value, Char Default = Char.MinValue)
        {
            Char res;
            if (Char.TryParse(value, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Int16 ParseToInt16(this string value, Int16 Default = 0)
        {
            Int16 res;
            if (Int16.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Int32 ParseToInt32(this string value, Int32 Default = 0)
        {
            Int32 res;
            if (Int32.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Int64 ParseToInt64(this string value, Int64 Default = 0)
        {
            Int64 res;
            if (Int64.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static UInt16 ParseToUInt16(this string value, UInt16 Default = 0)
        {
            UInt16 res;
            if (UInt16.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static UInt32 ParseToUInt32(this string value, UInt32 Default = 0)
        {
            UInt32 res;
            if (UInt32.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static UInt64 ParseToUInt64(this string value, UInt64 Default = 0)
        {
            UInt64 res;
            if (UInt64.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static float ParseToFloat(this string value, float Default = 0)
        {
            float res;
            if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static double ParseToDouble(this string value, double Default = 0)
        {
            double res;
            if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Decimal ParseToDecimal(this string value, Decimal Default = 0)
        {
            Decimal res;
            if (Decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static DateTime ParseToDateTime(this string value, DateTime? Default = null)
        {
            DateTime res;
            if (DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowInnerWhite | System.Globalization.DateTimeStyles.AllowLeadingWhite | System.Globalization.DateTimeStyles.AllowTrailingWhite | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out res))
                return res;
            else
                return Default == null ? default(DateTime) : Default.Value;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool ParseToBool(this string value, bool Default = false)
        {
            bool res;
            if (bool.TryParse(value, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool TryParse(this string value, out Char result)
        {
            return Char.TryParse(value, out result);
        }
        public static bool TryParse(this string value, out Int16 result)
        {
            return Int16.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }
        public static bool TryParse(this string value, out Int32 result)
        {
            return Int32.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }
        public static bool TryParse(this string value, out Int64 result)
        {
            return Int64.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }
        public static bool TryParse(this string value, out float result)
        {
            return float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }
        public static bool TryParse(this string value, out double result)
        {
            return double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }
        public static bool TryParse(this string value, out decimal result)
        {
            return decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }
        public static bool TryParse(this string value, out bool result)
        {
            return bool.TryParse(value, out result);
        }
        public static bool TryParse(this string value, out DateTime result)
        {
            return DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowInnerWhite | System.Globalization.DateTimeStyles.AllowLeadingWhite | System.Globalization.DateTimeStyles.AllowTrailingWhite | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static float? TryParseToNullableFloatOrDefault2(this string value, float? Default = null)
        {
            float res;
            if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                return res;
            else
                return Default;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool? TryParseToNullableBoolOrDefault(this string value, bool? Default = null)
        {
            bool res;
            if (bool.TryParse(value, out res))
                return res;
            else
                return null;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Matches(this string source, IEnumerable<string> toCheck, bool CaseSensitive = true)
        {
            StringComparison comp = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            foreach (var entry in toCheck)
                if (source.IndexOf(entry, comp) >= 0)
                    return true;
            //nothing..
            return false;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Left of the n'th occurance of c
        /// </summary>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string LeftOf(this string source, char c, int n = 1, int StartIndex = 0)
        {
            int idx = StartIndex - 1;
            while (n != 0)
            {
                idx = source.IndexOf(c, idx + 1);
                if (idx == -1)
                {
                    return source;
                }
                --n;
            }
            return source.Substring(StartIndex, idx);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Left of the n'th occurance of c
        /// </summary>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string LeftOf(this string source, string c, int n = 1, int StartIndex = 0)
        {
            int idx = StartIndex - c.Length;
            while (n != 0)
            {
                idx = source.IndexOf(c, idx + c.Length);
                if (idx == -1)
                {
                    return source;
                }
                --n;
            }
            return source.Substring(StartIndex, idx);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Right of the n'th occurance of c
        /// </summary>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RightOf(this string source, string c, int n = 1)
        {
            int idx = -c.Length;
            while (n != 0)
            {
                idx = source.IndexOf(c, idx + c.Length);
                if (idx == -1)
                {
                    return "";
                }
                --n;
            }

            return source.Substring(idx + c.Length);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Right of the n'th occurance of c
        /// </summary>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RightOf(this string source, char c, int n = 1)
        {
            int idx = -1;
            while (n != 0)
            {
                idx = source.IndexOf(c, idx + 1);
                if (idx == -1)
                {
                    return "";
                }
                --n;
            }

            return source.Substring(idx + 1);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// human-friendly printing of arrays
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToStringEx<T>(this T[] val)
        {
            string s = "{ ";
            foreach (var m in val)
                s += m + " ";
            return s + "}";
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static string ToStringInvariant(this string value) { return value; }
#if NETFX
        public static string ToStringInvariant(this char value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this bool value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
#else
        public static string ToStringInvariant(this char value) { return value.ToString(); }
        public static string ToStringInvariant(this bool value) { return value.ToString(); }
#endif
        public static string ToStringInvariant(this byte value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this byte value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this sbyte value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this sbyte value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }

        public static string ToStringInvariant(this Int16 value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this Int16 value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this Int32 value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this Int32 value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this Int64 value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this Int64 value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }

        public static string ToStringInvariant(this UInt16 value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this UInt16 value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this UInt32 value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this UInt32 value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this UInt64 value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this UInt64 value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }

        public static string ToStringInvariant(this float value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this float value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this double value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this double value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this decimal value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this decimal value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }

        public static string ToStringInvariant(this DateTime value) { return value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this DateTime value, string format) { return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this TimeSpan value) { return value.ToString(@"dd\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture); }
        public static string ToStringInvariant(this TimeSpan value, string format) { return value.ToString(@format, System.Globalization.CultureInfo.InvariantCulture); }

        public static string ToStringInvariant(this object value)
        {
            if (value == null)
                return null;
            else if (value is string)
                return value as string;
            else if (value is TimeSpan)
                return ((TimeSpan)value).ToStringInvariant(); //need to use formating
            else
                return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public static System.Security.SecureString ToSecureString(this string str)
        {
            var sec = new System.Security.SecureString();
            foreach (var c in str)
                sec.AppendChar(c);
            sec.MakeReadOnly();
            return sec;
        }
#endif
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static string HtmlEncode(this string s)
        {
            //sanity check
            if (string.IsNullOrEmpty(s))
                return String.Empty;

            //check if needs encoding
            bool needEncode = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '&' || c == '"' || c == '<' || c == '>' || c > 159 || c == '\'')
                {
                    needEncode = true;
                    break;
                }
            }
            if (!needEncode)
                return s;

            //encode
            int len = s.Length;
            var output = new StringBuilder(len);

            for (int i = 0; i < len; i++)
            {
                char ch = s[i];
                switch (ch)
                {
                    case '&':
                        output.Append("&amp;");
                        break;
                    case '>':
                        output.Append("&gt;");
                        break;
                    case '<':
                        output.Append("&lt;");
                        break;
                    case '"':
                        output.Append("&quot;");
                        break;
                    case '\'':
                        output.Append("&#39;");
                        break;
                    case '\uff1c':
                        output.Append("&#65308;");
                        break;

                    case '\uff1e':
                        output.Append("&#65310;");
                        break;

                    default:
                        if (ch > 159 && ch < 256)
                        {
                            output.Append("&#");
                            output.Append(((int)ch).ToStringInvariant());
                            output.Append(";");
                        }
                        else
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static string HtmlDecode(this string s)
        {
            //sanity check
            if (string.IsNullOrEmpty(s))
                return String.Empty;

            //find any html encode
            if (s.IndexOf('&') == -1)
                return s;

            StringBuilder rawEntity = new StringBuilder();
            StringBuilder entity = new StringBuilder();
            StringBuilder output = new StringBuilder(s.Length);
            int len = s.Length;
            // 0 -> nothing,
            // 1 -> right after '&'
            // 2 -> between '&' and ';' but no '#'
            // 3 -> '#' found after '&' and getting numbers
            int state = 0;
            int number = 0;
            bool is_hex_value = false;
            bool have_trailing_digits = false;

            for (int i = 0; i < len; i++)
            {
                char c = s[i];
                if (state == 0)
                {
                    if (c == '&')
                    {
                        entity.Append(c);
                        rawEntity.Append(c);
                        state = 1;
                    }
                    else
                    {
                        output.Append(c);
                    }
                    continue;
                }

                if (c == '&')
                {
                    state = 1;
                    if (have_trailing_digits)
                    {
                        entity.Append(number.ToStringInvariant());
                        have_trailing_digits = false;
                    }

                    output.Append(entity.ToString());
                    entity.Length = 0;
                    entity.Append('&');
                    continue;
                }

                if (state == 1)
                {
                    if (c == ';')
                    {
                        state = 0;
                        output.Append(entity.ToString());
                        output.Append(c);
                        entity.Length = 0;
                    }
                    else
                    {
                        number = 0;
                        is_hex_value = false;
                        if (c != '#')
                        {
                            state = 2;
                        }
                        else
                        {
                            state = 3;
                        }
                        entity.Append(c);
                        rawEntity.Append(c);
                    }
                }
                else if (state == 2)
                {
                    entity.Append(c);
                    if (c == ';')
                    {
                        string key = entity.ToString();
                        if (key.Length > 1 && Tools.Html.Entities.ContainsKey(key.Substring(1, key.Length - 2)))
                            key = Tools.Html.Entities[key.Substring(1, key.Length - 2)].ToString();

                        output.Append(key);
                        state = 0;
                        entity.Length = 0;
                        rawEntity.Length = 0;
                    }
                }
                else if (state == 3)
                {
                    if (c == ';')
                    {
                        if (number == 0)
                            output.Append(rawEntity.ToString() + ";");
                        else
                            if (number > 65535)
                        {
                            output.Append("&#");
                            output.Append(number.ToStringInvariant());
                            output.Append(";");
                        }
                        else
                        {
                            output.Append((char)number);
                        }
                        state = 0;
                        entity.Length = 0;
                        rawEntity.Length = 0;
                        have_trailing_digits = false;
                    }
                    else if (is_hex_value && IsHexDigit(c))
                    {
                        number = number * 16 + FromHex(c);
                        have_trailing_digits = true;
                        rawEntity.Append(c);
                    }
                    else if (Char.IsDigit(c))
                    {
                        number = number * 10 + ((int)c - '0');
                        have_trailing_digits = true;
                        rawEntity.Append(c);
                    }
                    else if (number == 0 && (c == 'x' || c == 'X'))
                    {
                        is_hex_value = true;
                        rawEntity.Append(c);
                    }
                    else
                    {
                        state = 2;
                        if (have_trailing_digits)
                        {
                            entity.Append(number.ToStringInvariant());
                            have_trailing_digits = false;
                        }
                        entity.Append(c);
                    }
                }
            }

            if (entity.Length > 0)
                output.Append(entity.ToString());
            else if (have_trailing_digits)
                output.Append(number.ToStringInvariant());

            return output.ToString();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string FormatFromDictionary(this string format, IDictionary<string, string> keyvals)
        {
            foreach (var kv in keyvals)
            {
                format = format.Replace("{" + kv.Key + "}", kv.Value);
            }
            return format;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool IsHexDigit(this char character)
        {
            return (('0' <= character && character <= '9') ||
                    ('a' <= character && character <= 'f') ||
                    ('A' <= character && character <= 'F'));
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static int FromHex(this char digit)
        {
            if ('0' <= digit && digit <= '9')
            {
                return (int)(digit - '0');
            }

            if ('a' <= digit && digit <= 'f')
                return (int)(digit - 'a' + 10);

            if ('A' <= digit && digit <= 'F')
                return (int)(digit - 'A' + 10);

            throw new ArgumentException("digit");
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
