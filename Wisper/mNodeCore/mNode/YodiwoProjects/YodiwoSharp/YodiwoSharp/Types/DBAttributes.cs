using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public class DoNotStoreInDBAttribute : Attribute { }

    public class DB_IgnoreIfDefaultAttribute : Attribute { }

    public class DB_DefaultValueAttribute : Attribute
    {
        public object Default;
        public DB_DefaultValueAttribute(object value) { Default = value; }

        public virtual object GetDefaultValue() { return Default; }
    }

    public class DB_ExtraElements : Attribute { }

    public class DB_IgnoreIfEmptyAttribute : Attribute { }

    public class DB_UseDefaultDictionarySerializerAttribute : Attribute { }
}
