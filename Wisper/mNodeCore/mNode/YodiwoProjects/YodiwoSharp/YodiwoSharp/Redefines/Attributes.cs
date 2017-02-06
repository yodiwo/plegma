using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public class SerializableAttribute : Attribute
    {
    }

    public class NonSerializedAttribute : Attribute
    {
    }

    public class TypeConverterAttribute : Attribute
    {
        Type type;
        string typeName;

        public TypeConverterAttribute() { }
        public TypeConverterAttribute(Type type) { this.type = type; }
        public TypeConverterAttribute(string typeName) { this.typeName = typeName; }

        public string ConverterTypeName => typeName;
    }
}
