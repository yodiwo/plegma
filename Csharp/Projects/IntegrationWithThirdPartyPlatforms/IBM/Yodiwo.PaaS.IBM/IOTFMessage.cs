using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.PaaS.IBM
{
    public class Data
    {
        public string name { get; set; }
        public int Temperature { get; set; }
        public bool Switch1 { get; set; }
    }

    public class Payload
    {
        public Data d { get; set; }
    }

    public class MyPayload
    {
        public object d;
    }

    public class IOTFPayload
    {
        public string topic { get; set; }
        public Payload payload { get; set; }
        public string deviceId { get; set; }
        public string deviceType { get; set; }
        public string eventType { get; set; }
        public string format { get; set; }
        public string _msgid { get; set; }
    }
}
