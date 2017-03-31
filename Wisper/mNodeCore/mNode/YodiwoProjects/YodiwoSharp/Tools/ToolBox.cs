using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class ToolBox
    {
        //string equality, ignoring differenced between "",null,whitespace
        public static bool StringEqualityEx(string A, string B, bool IgnoreCase = false)
        {
            //prepare
            if (string.IsNullOrWhiteSpace(A))
                A = null;
            if (string.IsNullOrWhiteSpace(B))
                B = null;

            //check
            if (A == null && B == null)
                return true;
            else if ((A != null && B == null) || (A == null && B != null))
                return false;
            else
                return string.Compare(A, B, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0;
        }
    }
}
