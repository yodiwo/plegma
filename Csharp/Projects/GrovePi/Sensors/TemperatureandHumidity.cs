using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class TempAndHumidity : GrovePiSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal TempAndHumidity(Pin pin, Transport transport, int period = 0) : base(pin, transport, period)
        {
            this.OnGetContinuousDatacb += OnGetHTSensorData;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void ReadContinuously()
        {
            SharpPy msg = new SharpPy()
            {
                operation = RWCMD.DHT,
                payload = "",

            };
            base.ReadContinuously(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnGetHTSensorData(object data)
        {
#if DEBUG
            Console.WriteLine("Get Button SensorData");
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
