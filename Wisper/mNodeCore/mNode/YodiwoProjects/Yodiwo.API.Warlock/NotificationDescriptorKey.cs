using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    #region NotificationDescriptorKey

    /// <summary>
    /// Globally unique identifier of a <see cref="NotificationDescriptor"/>
    /// </summary>
    [TypeConverter(typeof(NotificationDescriptorKeyConverter))]
    public struct NotificationDescriptorKey : IEquatable<NotificationDescriptorKey>
    {
        public  /*readonly*/ UserKey UserKey;
        public  /*readonly*/ string Id;

        public NotificationDescriptorKey(UserKey key, string Id) { this.UserKey = key; this.Id = Id; }

        [JsonIgnore]
        public bool IsValid { get { return this != default(NotificationDescriptorKey); } }
        [JsonIgnore]
        public bool IsInvalid { get { return this == default(NotificationDescriptorKey); } }

        #region Implicit conversion to/from string key
        public static implicit operator NotificationDescriptorKey(string str)
        {
            return ConvertFromString(str);
        }
        public static implicit operator string(NotificationDescriptorKey key)
        {
            return key.ToString();
        }

        public static NotificationDescriptorKey ConvertFromString(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue) || !Validators.ValidateKey(stringValue))
                return default(NotificationDescriptorKey);
            else
                try
                {
                    var key_separator = stringValue.LastIndexOf(PlegmaAPI.KeySeparator);
                    return new NotificationDescriptorKey(stringValue.Substring(0, key_separator), stringValue.Substring(key_separator + 1));
                }
                catch
                {
                    return default(NotificationDescriptorKey);
                }
        }
        public override string ToString()
        {
            return UserKey.ToString() + PlegmaAPI.KeySeparator + Id;
        }
        #endregion

        #region Equality
        public bool Equals(NotificationDescriptorKey other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref NotificationDescriptorKey other)
        {
            return this.UserKey == other.UserKey && this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NotificationDescriptorKey)) return false;
            return Equals((NotificationDescriptorKey)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (UserKey.GetHashCode());
            hash = (hash * 397) ^ (Id == null ? 0 : Id.GetHashCode());
            return hash;
        }

        public static bool operator ==(NotificationDescriptorKey left, NotificationDescriptorKey right) { return left.Equals(ref right); }
        public static bool operator !=(NotificationDescriptorKey left, NotificationDescriptorKey right) { return !left.Equals(ref right); }
        #endregion

    }

    #region Converters
    public class NotificationDescriptorKeyConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return NotificationDescriptorKey.ConvertFromString(value as string);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((NotificationDescriptorKey)value).ToString();
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
