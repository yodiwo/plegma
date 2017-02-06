using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic
{
    public class CommonIOTypes
    {
        public static readonly IReadOnlySet<Type> SupportedTypes = (IReadOnlySet<Type>)new HashSet<Type>()
            {
                typeof(bool),
                typeof(byte) ,  typeof(sbyte),
                typeof(Int16) ,  typeof(Int32) , typeof(Int64),
                typeof(UInt16) , typeof(UInt32) , typeof(UInt64),
                typeof(double) , typeof(float) ,typeof(decimal),
                typeof(string),
            };
    }
}
