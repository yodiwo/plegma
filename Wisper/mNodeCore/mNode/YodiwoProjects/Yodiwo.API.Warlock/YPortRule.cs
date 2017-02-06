using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public struct PortRule
    {
        public IComparable Left;
        public bool LeftEqualInclusive;
        public IComparable Right;
        public bool RightEqualInclusive;
        public string Pattern;
        public string Classification;
    }
}
