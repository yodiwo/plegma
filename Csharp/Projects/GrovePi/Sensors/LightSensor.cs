using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class LightSensor : GrovePiSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal LightSensor(Pin pin, Transport transport, int period = 0) : base(pin, transport, period)
        {
            this.OnGetContinuousDatacb += OnGetLightSensorData;
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
        public void OnGetLightSensorData(object data)
        {
#if DEBUG
            Console.WriteLine("Get Light Sensor");
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
