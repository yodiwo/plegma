using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace Yodiwo
{
    public static class ConvertEx
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public const char ArraySeparator = ',';
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public static object Convert(object value, Type toType)
        {
            object result;
            if (Convert(value, toType, out result))
                return result;
            else
                return null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static T Convert<T>(object value)
        {
            object result;
            if (Convert(value, typeof(T), out result))
                return (T)result;
            else
                return default(T);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static bool Convert<T>(object value, out T result, T Default = default(T))
        {
            object tmp;
            if (Convert(value, typeof(T), out tmp))
            {
                result = (T)tmp;
                return true;
            }
            else
            {
                result = Default;
                return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static bool Convert(object value, Type ToType, out object result)
        {
            result = null;
            //try convert
            try
            {
                //get value type
                var fromType = value == null ? typeof(object) : value.GetType();

                //nothing to do?
                if (fromType == ToType)
                {
                    result = value;
                    return true;
                }

                //are both single values?
                if ((!fromType.IsArray && !ToType.IsArray) || (value == null))
                {
                    if (SingleValueConvert(value, ToType, out result))
                        return true;
                    else if (value is string && value != null && (value as string).Contains(ArraySeparator))
                    {
                        //we may be able to convert this string if we split it by the separator and pick the first element
                        var elems = (value as string).Split(ArraySeparator);
                        return SingleValueConvert(elems[0], ToType, out result);
                    }
                    else
                        return false;
                }

                //get parameters
                var src_arr = (value as Array);
                var src_length = src_arr == null ? 0 : src_arr.Length;
                var ToTypeElement = ToType.IsArray ? ToType.GetElementType() : ToType;

                //zero length array?
                if (fromType.IsArray && src_length == 0)
                {
                    //create a zero-length result array
                    result = Array.CreateInstance(ToTypeElement, 0);
                    return true;
                }

                //is source only an array?
                if (fromType.IsArray && !ToType.IsArray)
                {
                    //if target is string then we can join them up into a nice string, else just pick the first value
                    if (ToType == typeof(string))
                    {
                        //build result string by joining up elements 
                        string res = "";
                        for (int n = 0; n < src_length; n++)
                        {
                            object v;
                            if (SingleValueConvert(src_arr.GetValue(n), ToTypeElement, out v) == false)
                                return false;
                            res += v.ToStringInvariant() + ArraySeparator;
                        }
                        result = res;
                        return true;
                    }
                    else
                        return SingleValueConvert(src_arr.GetValue(0), ToType, out result);
                }
                //is only destination an array?
                else if (!fromType.IsArray && ToType.IsArray)
                {
                    //if source is string try to split it into elements
                    if (fromType == typeof(string))
                    {
                        //split using separator
                        var elems = (value as string).Split(ArraySeparator);
                        //create result array
                        var dst_arr = Array.CreateInstance(ToTypeElement, elems.Length);
                        for (int n = 0; n < elems.Length; n++)
                        {
                            object v;
                            if (SingleValueConvert(elems[n], ToTypeElement, out v) == false)
                                return false;
                            dst_arr.SetValue(v, n);
                        }
                        result = dst_arr;
                        return true;
                    }
                    else
                    {
                        //create a zero-length result array
                        var dst_arr = Array.CreateInstance(ToTypeElement, 1);
                        object v;
                        if (SingleValueConvert(value, ToTypeElement, out v) == false)
                            return false;
                        dst_arr.SetValue(v, 0);
                        result = dst_arr;
                        return true;
                    }
                }
                //are both arrays?
                else if (fromType.IsArray && ToType.IsArray)
                {
                    //create result array
                    var dst_arr = Array.CreateInstance(ToTypeElement, src_length);
                    //fill with a per-element convert
                    for (int n = 0; n < src_length; n++)
                    {
                        object v;
                        if (SingleValueConvert(src_arr.GetValue(n), ToTypeElement, out v) == false)
                            return false;
                        dst_arr.SetValue(v, n);
                    }
                    result = dst_arr;
                    return true;
                }
                else
                {
                    DebugEx.Assert("Should not be here (Convert from=" + fromType + ",  to=" + ToType + ")");
                    return false;
                }
            }
            catch (Exception ex) { result = null; return false; } //pokemon exception handler
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static bool SingleValueConvert(object value, Type ToType, out object result)
        {
            result = null;
            //try convert
            try
            {
                //get value type
                var fromType = value == null ? typeof(object) : value.GetType();
                var ToTypeIsInteger = ToType == typeof(Int16) || ToType == typeof(Int32) || ToType == typeof(Int64) ||
                                      ToType == typeof(UInt16) || ToType == typeof(UInt32) || ToType == typeof(UInt64);
                var ToTypeIsDecimal = ToType == typeof(float) || ToType == typeof(double) || ToType == typeof(decimal);
                var ToTypeIsNumber = ToTypeIsInteger || ToTypeIsDecimal;

                //nothing to do?
                if (fromType == ToType)
                {
                    result = value;
                    return true;
                }

                //check easy stuff
#if NETFX
                if (value != null && ToType.IsAssignableFrom(fromType))
#elif UNIVERSAL
                if (value != null && ToType.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo()))
#endif
                {
                    result = value;
                    return true;
                }
#if NETFX
                else if (value is string && typeof(IFillFromString).IsAssignableFrom(ToType))
#elif UNIVERSAL
                else if (value is string && typeof(IFillFromString).GetTypeInfo().IsAssignableFrom(ToType.GetTypeInfo()))
#endif
                {
                    var box = Activator.CreateInstance(ToType) as IFillFromString;
                    box.FillFromString((string)value);
                    result = box;
                    return true;
                }

                //special string cases
                if (value is string)
                {
                    var str = value as string;
                    if (str != null) str = str.ToLowerInvariant().Trim();

                    //string->boolean cases
                    if (ToType == typeof(Boolean))
                    {
                        float fv;
                        int iv;
                        if (str.TryParse(out fv))
                        {
                            result = fv != 0;
                            return true;
                        }
                        else if (str.TryParse(out iv))
                        {
                            result = iv != 0;
                            return true;
                        }
                    }

                    //special cases for string
#if NETFX
                    if (string.IsNullOrEmpty(str) && ToType.IsPrimitive && !ToType.IsEnum && ToType != typeof(string))
#elif UNIVERSAL
                    if (string.IsNullOrEmpty(str) && ToType.GetTypeInfo().IsPrimitive && !ToType.GetTypeInfo().IsEnum && ToType != typeof(string))
#endif
                    {
                        fromType = typeof(bool);
                        value = false;
                    }
                    else if (str == "true" || str == "false")
                    {
                        fromType = typeof(bool);
                        value = str == "true" ? true : false;
                    }
                    else if (str == "yes" || str == "no")
                    {
                        fromType = typeof(bool);
                        value = str == "yes" ? true : false;
                    }
                    else if (str == "on" || str == "off")
                    {
                        fromType = typeof(bool);
                        value = str == "on" ? true : false;
                    }

                    //nothing to do?
                    if (fromType == ToType)
                    {
                        result = value;
                        return true;
                    }

                    //number converter
                    if (ToTypeIsNumber)
                    {
                        double dv;
                        if (str.TryParse(out dv))
                        {
                            value = dv;
                            fromType = typeof(double);
                        }
                    }
                }

                //custom converters
                if (fromType == typeof(Boolean))
                {
                    if (ToType == typeof(float))
                    {
                        result = (bool)value ? 1f : 0f;
                        return true;
                    }
                    else if (ToType == typeof(byte))
                    {
                        result = (bool)value ? (byte)1 : (byte)0;
                        return true;
                    }
                    else if (ToType == typeof(double))
                    {
                        result = (bool)value ? 1d : 0d;
                        return true;
                    }
                    else if (ToType == typeof(int))
                    {
                        result = (bool)value ? (int)1 : (int)0;
                        return true;
                    }
                    else if (ToType == typeof(uint))
                    {
                        result = (bool)value ? (uint)1 : (uint)0;
                        return true;
                    }
                    else if (ToType == typeof(long))
                    {
                        result = (bool)value ? (long)1 : (long)0;
                        return true;
                    }
                    else if (ToType == typeof(ulong))
                    {
                        result = (bool)value ? (ulong)1 : (ulong)0;
                        return true;
                    }
                    else if (ToType == typeof(string))
                    {
                        result = (bool)value ? "True" : "False";
                        return true;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }
                }

                //if input is null then give null back
                if (value == null)
                {
#if NETFX
                    result = ToType.IsClass ? null : Activator.CreateInstance(ToType);
#elif UNIVERSAL
                    result = ToType.GetTypeInfo().IsClass ? null : Activator.CreateInstance(ToType);
#endif
                    return true;
                }

#if NETFX
                //try forward convert
                var convValue = TypeDescriptor.GetConverter(fromType);
                if (convValue.CanConvertTo(ToType))
                {
                    result = convValue.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, value, ToType);
                    return true;
                }

                //try the other way around
                var convType = TypeDescriptor.GetConverter(ToType);
                if (convType.CanConvertFrom(fromType))
                {
                    //try the other way around
                    result = convType.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, value);
                    return true;
                }
#endif

                //last attempt
                try
                {
                    result = System.Convert.ChangeType(value, ToType);
                    return true;
                }
                catch { }

                //failed
                result = null;
                return false;
            }
            catch (Exception ex) { DebugEx.TraceLog(ex, "Convert Error"); result = null; return false; } //pokemon exception handler
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
