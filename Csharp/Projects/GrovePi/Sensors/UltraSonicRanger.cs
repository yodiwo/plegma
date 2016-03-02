using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class UltraSonicRanger : GrovePiSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal UltraSonicRanger(Pin pin, Transport transport, int period = 0) :
            base(pin, transport, period)
        {
            this.OnGetContinuousDatacb += OnGetUltrasonicSensorData;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void ReadContinuously()
        {
            SharpPy msg = new SharpPy()
            {
                operation = RWCMD.ULTRASONIC,
                payload = "",

            };
            base.ReadContinuously(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnGetUltrasonicSensorData(object data)
        {
#if DEBUG
            Console.WriteLine("Get UltraSonic SensorData");
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
