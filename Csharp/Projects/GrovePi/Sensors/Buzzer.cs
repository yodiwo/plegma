using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class Buzzer : GrovePiSensor
    {
        internal Buzzer(Pin pin, Transport transport) : base(pin,transport) { }
    }
}
