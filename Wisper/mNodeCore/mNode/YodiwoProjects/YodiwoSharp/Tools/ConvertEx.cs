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
            catch { result = null; return false; } //pokemon exception handler
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static bool SingleValueConvert(object value, Type toType, out object result)
        {
            result = null;
            //try convert
            try
            {
                //if nullable extract wrapped type
#if NETFX
                var isNullable = toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>);
#elif UNIVERSAL
                var isNullable = toType.GetTypeInfo().IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>);
#endif
                if (isNullable)
                    toType = toType.GenericTypeArguments[0];

                try
                {
                    //get value type
                    var fromType = value == null ? typeof(object) : value.GetType();
                    var ToTypeIsInteger = toType.IsInteger();
                    var ToTypeIsDecimal = toType.IsDecimal();
                    var ToTypeIsNumber = ToTypeIsInteger || ToTypeIsDecimal;

                    //universal helpers
#if NETFX
                    var ToTypeInfo = toType;
                    var FromTypeInfo = fromType;
#elif UNIVERSAL
                    var ToTypeInfo = toType.GetTypeInfo();
                    var FromTypeInfo = fromType.GetTypeInfo();
#endif

                    //nothing to do?
                    if (fromType == toType)
                    {
                        result = value;
                        return true;
                    }

                    //check for enums
                    if (ToTypeInfo.IsEnum && fromType != typeof(string) && toType.GetEnumUnderlyingType() != fromType)
                    {
                        //try to convert to target type
                        object convValue = null;
                        if (Convert(value, toType.GetEnumUnderlyingType(), out convValue))
                        {
                            value = convValue?.ToStringInvariant();
                            fromType = typeof(string);
#if UNIVERSAL
                            FromTypeInfo = fromType.GetTypeInfo();
#endif
                        }
                    }

                    //check easy stuff
                    if (value != null && ToTypeInfo.IsAssignableFrom(FromTypeInfo))
                    {
                        result = value;
                        return true;
                    }
#if NETFX
                    else if (value is string && typeof(IFillFromString).IsAssignableFrom(toType))
#elif UNIVERSAL
                    else if (value is string && typeof(IFillFromString).GetTypeInfo().IsAssignableFrom(ToTypeInfo))
#endif
                    {
                        var box = Activator.CreateInstance(toType) as IFillFromString;
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
                        if (toType == typeof(Boolean))
                        {
                            bool bv;
                            double fv;
                            Int64 iv;
                            if (str.TryParse(out bv))
                            {
                                result = bv;
                                return true;
                            }
                            else if (str.TryParse(out fv))
                            {
                                result = fv != 0;
                                return true;
                            }
                            else if (str.TryParse(out iv))
                            {
                                result = iv != 0;
                                return true;
                            }
                            else
                            {
                                //could not parse to a bool, fallback to false
                                result = false;
                                //but allow callers to handle the failure on their own if they want to
                                return false;
                            }
                        }

                        //special cases for string
                        if (!ToTypeInfo.IsEnum)
                        {
                            if (string.IsNullOrEmpty(str) && ToTypeInfo.IsPrimitive && toType != typeof(string))
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
                            else if (str == "enabled" || str == "disabled")
                            {
                                fromType = typeof(bool);
                                value = str == "enabled" ? true : false;
                            }
                            else if (str == "enable" || str == "disable")
                            {
                                fromType = typeof(bool);
                                value = str == "enable" ? true : false;
                            }
                            else if (str == "active" || str == "inactive")
                            {
                                fromType = typeof(bool);
                                value = str == "active" ? true : false;
                            }
                            else if (str == "activate" || str == "deactivate")
                            {
                                fromType = typeof(bool);
                                value = str == "activate" ? true : false;
                            }
                            else if (str == "activated" || str == "deactivated")
                            {
                                fromType = typeof(bool);
                                value = str == "activated" ? true : false;
                            }
                        }

                        //nothing to do?
                        if (fromType == toType)
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
                        if (toType == typeof(float))
                        {
                            result = (bool)value ? 1f : 0f;
                            return true;
                        }
                        else if (toType == typeof(byte))
                        {
                            result = (bool)value ? (byte)1 : (byte)0;
                            return true;
                        }
                        else if (toType == typeof(sbyte))
                        {
                            result = (bool)value ? (sbyte)1 : (sbyte)0;
                            return true;
                        }
                        else if (toType == typeof(double))
                        {
                            result = (bool)value ? 1d : 0d;
                            return true;
                        }
                        else if (toType == typeof(decimal))
                        {
                            result = (bool)value ? (decimal)1 : (decimal)0;
                            return true;
                        }
                        else if (toType == typeof(Int16))
                        {
                            result = (bool)value ? (Int16)1 : (Int16)0;
                            return true;
                        }
                        else if (toType == typeof(Int32))
                        {
                            result = (bool)value ? (Int32)1 : (Int32)0;
                            return true;
                        }
                        else if (toType == typeof(Int64))
                        {
                            result = (bool)value ? (Int64)1 : (Int64)0;
                            return true;
                        }
                        else if (toType == typeof(UInt16))
                        {
                            result = (bool)value ? (UInt16)1 : (UInt16)0;
                            return true;
                        }
                        else if (toType == typeof(UInt32))
                        {
                            result = (bool)value ? (UInt32)1 : (UInt32)0;
                            return true;
                        }
                        else if (toType == typeof(UInt64))
                        {
                            result = (bool)value ? (UInt64)1 : (UInt64)0;
                            return true;
                        }
                        else if (toType == typeof(string))
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
                        result = ToTypeInfo.IsClass || toType.IsInterface ? null : Activator.CreateInstance(toType);
#elif UNIVERSAL
                        result = ToTypeInfo.IsClass || ToTypeInfo.IsInterface ? null : Activator.CreateInstance(toType);
#else
#error unkown platform
#endif
                        return true;
                    }

#if NETFX
                    //try forward convert
                    {
                        var convValue = TypeDescriptor.GetConverter(fromType);
                        if (convValue.CanConvertTo(toType))
                        {
                            result = convValue.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, value, toType);

                            //special object->string case (use json if objects ToString is the default typename)
                            if (fromType.IsClass && toType == typeof(string) && (result as string) == fromType.FullName)
                            {
#if NEWTONSOFT
                                result = value.ToJSON();
#endif
                            }

                            return true;
                        }
                    }

                    //try the other way around
                    {
                        var convType = TypeDescriptor.GetConverter(toType);
                        if (convType.CanConvertFrom(fromType))
                        {
                            //try the other way around
                            result = convType.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, value);
                            return true;
                        }
                    }
#endif

                    //last attempt
                    try
                    {
                        result = System.Convert.ChangeType(value, toType);
                        //special object->string case (use json if objects ToString is the default typename)
                        if (FromTypeInfo.IsClass && toType == typeof(string) && (result as string) == fromType.FullName)
                        {
#if NEWTONSOFT
                            result = value.ToJSON();
#endif
                        }

                        return true;
                    }
                    catch { }

                    //string<->object special case using json
                    if (fromType == typeof(string) && ToTypeInfo.IsClass)
                    {
#if NEWTONSOFT
                        try
                        {
                            var trimmedStr = (value as string).Trim();
                            if (trimmedStr.Length > 1 &&
                                (trimmedStr.StartsWithInvariant("{", true) || trimmedStr.StartsWithInvariant("[", true)) &&
                                (trimmedStr.EndsWithInvariant("}", true) || trimmedStr.EndsWithInvariant("]", true)))
                            {
                                result = trimmedStr.FromJSON(toType);
                                if (result != null && result.GetType() == toType)
                                    return true;
                                else
                                    result = null;
                            }
                        }
                        catch { }
#endif
                    }


                    //failed
                    result = null;
                    return false;
                }
                finally
                {
                    //re-wrap with nullable (not sure if this is needed, because from debugger it will not do it anyway and c# will handle it internally.. but what-the-heck)
                    if (isNullable)
                    {
                        var nullableType = typeof(Nullable<>).MakeGenericType(toType);
                        try { result = Activator.CreateInstance(nullableType, new object[] { result }); } catch { }
                    }
                }
            }
            catch (Exception ex) { DebugEx.TraceLog(ex, "Convert Error"); result = null; return false; } //pokemon exception handler
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
