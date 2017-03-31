using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public interface ITypeDescriptorContext
    {
    }

    class TypeConverter
    {
        public virtual object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return false; }
        public virtual object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) { throw new NotImplementedException("Not Implemented (Not default implementation in Univeral)"); }
        public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) { return false; }
        public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) { return false; }
    }
}
