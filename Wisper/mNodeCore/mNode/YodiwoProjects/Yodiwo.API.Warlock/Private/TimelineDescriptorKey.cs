using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Newtonsoft.Json;

namespace Yodiwo.API.Warlock.Private
{
    #region TimelineDescriptorKey

    /// <summary>
    /// Globally unique identifier of a <see cref="TimelineDescriptor"/>
    /// </summary>
    [TypeConverter(typeof(TimelineDescriptorKeyConverter))]
    public struct TimelineDescriptorKey : IEquatable<TimelineDescriptorKey>
    {
        public  /*readonly*/ UserKey UserKey;
        public  /*readonly*/ string Id;

        public TimelineDescriptorKey(UserKey key, string Id) { this.UserKey = key; this.Id = Id; }

        [JsonIgnore]
        public bool IsValid { get { return this != default(TimelineDescriptorKey); } }
        [JsonIgnore]
        public bool IsInvalid { get { return this == default(TimelineDescriptorKey); } }

        #region Implicit conversion to/from string key
        public static implicit operator TimelineDescriptorKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(TimelineDescriptorKey key)
        {
            return key.ToString();
        }

        public static TimelineDescriptorKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(TimelineDescriptorKey);
            else
                try
                {
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    return new TimelineDescriptorKey(stringValue.Substring(0, key_separator), stringValue.Substring(key_separator + 1));
                }
                catch
                {
                    return default(TimelineDescriptorKey);
                }
        }
        public override string ToString()
        {
            return UserKey.ToString() + PlegmaAPI.KeySeparator + Id;
        }
        #endregion

        #region Equality
        public bool Equals(TimelineDescriptorKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref TimelineDescriptorKey other)
        {
            return this.UserKey == other.UserKey && this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TimelineDescriptorKey)) return false;
            return Equals((TimelineDescriptorKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (UserKey.GetHashCode());
            hash = (hash * 397) ^ (Id == null ? 0 : Id.GetHashCode());
            return hash;
        }

        public static bool operator ==(TimelineDescriptorKey left, TimelineDescriptorKey right) { return left.Equals(ref right); }
        public static bool operator !=(TimelineDescriptorKey left, TimelineDescriptorKey right) { return !left.Equals(ref right); }
        #endregion

    }

    #region Converters
    public class TimelineDescriptorKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return TimelineDescriptorKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((TimelineDescriptorKey)value).ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
    #endregion

    #endregion
}
