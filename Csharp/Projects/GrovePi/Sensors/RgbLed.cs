using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class RgbLed : GrovePiSensor
    {
        internal RgbLed(Pin pin, Transport transport) : base(pin,transport) { }
    }
}
