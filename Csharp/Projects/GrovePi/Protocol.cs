using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
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
        public string payload;
        public string pin;
        public RWCMD operation;

        public override string ToString()
        {
            return this.ToJSON();
        }
    }


    public enum RWCMD
    {
        DRRead,
        DRWrite,
        ARead,
        Awrite,
        DHT,
        ULTRASONIC,
        LCD,
        DAcceler,
        ADC,
        OLEDCLEAR,
        OLEDSETESTXY,
        OLEDPUTS,
        SUNLIGHTREAD,
        TOUCHSENSORREAD,
    }
}
