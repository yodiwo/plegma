using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.RasPiCamera
{
    public class SharpPy
    {
        public CMD operation;
        public string payload;
    }
    public enum CMD
    {
        Start,
        Stop,
        Filter
    }
}
