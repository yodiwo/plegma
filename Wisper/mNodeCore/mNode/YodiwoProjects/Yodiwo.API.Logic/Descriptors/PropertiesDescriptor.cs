using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yodiwo.API.Plegma;
using System.ComponentModel;

namespace Yodiwo.Logic.Descriptors
{
    //-------------------------------------------------------------------------------------------------------------------------

    [TypeConverter(typeof(MarkdownConverter))]
    public class Markdown // not saved in DB
    {
        public string Value;

        public override string ToString()
        {
            return Value;
        }
    }
    #region Converters
#if NETFX
    public class MarkdownConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return new Markdown() { Value = value as string };
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType == null)
                return null;

            if (destinationType == typeof(string))
                return ((Markdown)value).ToString();
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
#endif
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------
    public enum PropertyEditType
    {
        Number,
        Text,
        TextArea,
        Range,
        Color,
        Toggle,
        StringList,
        NumberList,
        Enum,
        OptionsList,
        MarkDown,
        Tags,
        // used from ThingDesriptor ?
        Port,
        Select,
        Object,
        Config,
        EnumList
    }
    //-------------------------------------------------------------------------------------------------------------------------
    public class PropertyCategoryDescriptor : IEquatable<PropertyCategoryDescriptor>
    {
        [DB_IgnoreIfDefault]
        public int Index;

        [DB_IgnoreIfDefault]
        public String Name;

        [DB_IgnoreIfDefault]
        public bool IsDynamic; // true if this category contains dynamic fields

        public bool Equals(PropertyCategoryDescriptor other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            return other.Index == Index && other.Name == Name && other.IsDynamic == IsDynamic;
        }

        public override int GetHashCode()
        {
            int hashName = Name == null ? 0 : Name.GetHashCode();
            int hashIndex = Index.GetHashCode();
            int hashIsDynamic = IsDynamic.GetHashCode();

            //Calculate the hash code for the product.
            return hashName ^ hashIndex ^ hashIsDynamic;
        }

    }
    //-------------------------------------------------------------------------------------------------------------------------
    public class PropertyFieldDescriptor : IEquatable<PropertyFieldDescriptor>
    {
        [DB_IgnoreIfDefault]
        [DB_DefaultValue(PropertyEditType.Text)]
        public PropertyEditType Type;

        [DB_IgnoreIfDefault]
        public String MemberName;

        [DB_IgnoreIfDefault]
        public String Name;

        [DB_IgnoreIfDefault]
        public String Description;

        [DB_IgnoreIfDefault]
        public Object Value; // Data

        [DB_IgnoreIfDefault]
        public Object Options; // Ports

        [DB_IgnoreIfDefault]
        public int CategoryIndex;

        [DB_IgnoreIfDefault]
        public bool Readonly;

        [DB_IgnoreIfDefault]
        public bool IsAdvanced;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public string[] Tags;

        [DB_IgnoreIfDefault]
        public bool ChangeInputNames;

        [DB_IgnoreIfDefault]
        public bool ChangeOutputNames;

        [DB_IgnoreIfDefault]
        public int StartPortIndex;

        [DB_IgnoreIfDefault]
        public int EndPortIndex;

        public bool Equals(PropertyFieldDescriptor other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            return other.Type == this.Type &&
                other.MemberName == this.MemberName &&
                other.Name == this.Name &&
                other.Description == this.Description &&
                other.Value == this.Value &&
                other.Options == this.Options &&
                other.CategoryIndex == this.CategoryIndex &&
                other.Readonly == this.Readonly &&
                other.IsAdvanced == this.IsAdvanced &&
                other.Tags == this.Tags;
        }

        public override int GetHashCode()
        {
            int hashType = Type.GetHashCode();
            int hashMemberName = MemberName == null ? 0 : MemberName.GetHashCode();
            int hashName = Name == null ? 0 : Name.GetHashCode();
            int hashDescription = Description == null ? 0 : Description.GetHashCode();
            int hashValue = Value == null ? 0 : Value.GetHashCode();
            int hashOptions = Options == null ? 0 : Options.GetHashCode();
            int hashCategoryIndex = CategoryIndex.GetHashCode();
            int hashReadonly = Readonly.GetHashCode();
            int hashIsAdvanced = IsAdvanced.GetHashCode();
            int hashTags = Tags == null ? 0 : Tags.GetHashCode();

            //Calculate the hash code for the field.
            return hashType ^ hashMemberName ^
                hashName ^ hashDescription ^
                hashValue ^ hashOptions ^
                hashCategoryIndex ^ hashReadonly ^
                hashIsAdvanced ^ hashTags;
        }

        // needed only for rappid.js, for ignoring dynamic properties populated through ajax
        // don't save in DB
        [DoNotStoreInDB]
        public bool IsDynamic;

        public override string ToString()
        {
            return "Field : " + MemberName + " (" + Value + ")";
        }

        /// <summary>
        /// Return true or false, whether it seems that the two fields describe the same field or not.
        /// A superset of Equals.
        /// We can have 2 fields with different 'Value', 'Tags', 'IsAdvanced', 'Readonly', 'Options', 'Description', 'Name' and get true.
        /// 
        /// This approach gives the opportunity to the developer to change the previous fields without the need of migration. 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SeemsEqual(PropertyFieldDescriptor other)
        {
            return this.CategoryIndex == other.CategoryIndex &&
                this.Type == other.Type &&
                this.MemberName == other.MemberName &&
                this.IsDynamic == other.IsDynamic;
        }

        public void UpdateIfSeemsEqual(PropertyFieldDescriptor other)
        {
            if (SeemsEqual(other))
            {
                this.Description = other.Description;
                // this.Value = other.Value;
                this.Tags = other.Tags;
                this.IsAdvanced = other.IsAdvanced;
                this.Readonly = other.Readonly;
                this.Options = other.Options;
                this.Name = other.Name;
            }
        }

    }

    public class PropertyFieldDescriptorWeakComparator : IEqualityComparer<PropertyFieldDescriptor>
    {
        public bool Equals(PropertyFieldDescriptor x, PropertyFieldDescriptor y)
        {
            return x.SeemsEqual(y);
        }
        public int GetHashCode(PropertyFieldDescriptor pfd)
        {
            int hashType = pfd.Type.GetHashCode();
            int hashMemberName = pfd.MemberName == null ? 0 : pfd.MemberName.GetHashCode();
            int hashCategoryIndex = pfd.CategoryIndex.GetHashCode();
            int hashIsDynamic = pfd.IsDynamic.GetHashCode();

            //Calculate the hash code for the field.
            return hashType ^ hashMemberName ^ hashCategoryIndex ^ hashIsDynamic;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public class PropertyDescriptor
    {
        #region Variables
        //---------------------------------------------------------------------------------------------------------------------
        [JsonIgnore]
        public static Dictionary<Type, PropertyEditType> EditTypeMapping = new Dictionary<Type, PropertyEditType>();

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<PropertyFieldDescriptor> Fields;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<PropertyCategoryDescriptor> Categories;
        //---------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //---------------------------------------------------------------------------------------------------------------------
        static PropertyDescriptor()
        {
            // At first, fill the mapping dictionary with the support types
            // TODO: Find better solution for the mapping 
            EditTypeMapping.Add(typeof(String), PropertyEditType.Text);
            EditTypeMapping.Add(typeof(int), PropertyEditType.Number);
            EditTypeMapping.Add(typeof(float), PropertyEditType.Text);
            EditTypeMapping.Add(typeof(double), PropertyEditType.Number);
            EditTypeMapping.Add(typeof(Int16), PropertyEditType.Number);
            EditTypeMapping.Add(typeof(YColor), PropertyEditType.Color);
            EditTypeMapping.Add(typeof(bool), PropertyEditType.Toggle);
            EditTypeMapping.Add(typeof(string[]), PropertyEditType.StringList);
            EditTypeMapping.Add(typeof(int[]), PropertyEditType.NumberList);
            EditTypeMapping.Add(typeof(List<string>), PropertyEditType.StringList);
            EditTypeMapping.Add(typeof(List<int>), PropertyEditType.NumberList);
            EditTypeMapping.Add(typeof(List<double>), PropertyEditType.NumberList);
            EditTypeMapping.Add(typeof(OptionsList), PropertyEditType.OptionsList);
            EditTypeMapping.Add(typeof(Markdown), PropertyEditType.MarkDown);
            EditTypeMapping.Add(typeof(HashSet<string>), PropertyEditType.Tags);
            EditTypeMapping.Add(typeof(Enum[]), PropertyEditType.EnumList);
            //TODO: support for more list types / generic lists?
        }
        //---------------------------------------------------------------------------------------------------------------------
        public PropertyDescriptor()
        {
            Fields = new List<PropertyFieldDescriptor>();
            Categories = new List<PropertyCategoryDescriptor>();
        }
        //---------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
