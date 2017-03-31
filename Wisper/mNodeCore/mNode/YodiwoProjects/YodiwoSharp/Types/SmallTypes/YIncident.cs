using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public class YIncident
    {
        public string FilePath;
        public string FileName;
        public int LineNumber;
        public string Method;
        public StackTrace StackTrace;
        public List<string> Messages = new List<string>();
        public bool IsAssert;
    }
}
