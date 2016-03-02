using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class LCD : GrovePiSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal LCD(Pin pin, Transport transport) : base(pin, transport) { }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Display(string value)
        {
            SharpPy msg = new SharpPy()
            {
                pin = this.Pin.ToString(),
                operation = RWCMD.LCD,
                payload = value,
                isRequest = true
            };
            DebugEx.TraceLog("LCD: Send to python: " + msg);
            this.transport.Send2python(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
