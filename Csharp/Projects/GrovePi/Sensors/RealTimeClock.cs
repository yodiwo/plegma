using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class RealTimeClock : GrovePiSensor
    {
        internal RealTimeClock(Pin pin, Transport transport) : base(pin,transport) { }
    }
}
