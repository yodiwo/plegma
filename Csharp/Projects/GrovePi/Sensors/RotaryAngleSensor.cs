using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class RotaryAngleSensor : GrovePiSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal RotaryAngleSensor(Pin pin, Transport transport, int period = 0) :
            base(pin, transport, period)
        {
            this.OnGetContinuousDatacb += OnGetRotaryAngleSensorData;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void ReadContinuously()
        {
            SharpPy msg = new SharpPy()
            {
                operation = RWCMD.ARead,
                payload = "",
            };
            base.ReadContinuously(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnGetRotaryAngleSensorData(object data)
        {
#if DEBUG
            Console.WriteLine("Get Rotary Angle Sensor Data");
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
