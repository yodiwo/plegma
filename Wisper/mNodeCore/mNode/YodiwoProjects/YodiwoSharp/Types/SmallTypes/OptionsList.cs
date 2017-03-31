using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Yodiwo
{
#if NETFX
    [TypeConverter(typeof(OptionsListConverter))]
#endif
    public class OptionsList
    {
        public List<string> Options = new List<string>();
        private int _selection;
        public int selection
        {
            get { return _selection; }
            set { _selection = (value < 0) ? -1 : (value >= Options.Count) ? Options.Count - 1 : value; }
        }

        public OptionsList()
        {
            selection = -1;
        }
    }

#if NETFX
    public class OptionsListConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                return null;
                //                return NodeKey.ConvertFromString(value as string);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
#endif
}
