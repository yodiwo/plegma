using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.SkyWriter
{
    public class Waiter
    {
        public string response;
    }

    public class SharpPy
    {
        public bool isRequest;
        public int watcherid;
        public int syncid;
        public CMD operation;
        public string payload;
    }

    public enum CMD
    {
        Gesture = 0,
        Position = 1,
    }
}
