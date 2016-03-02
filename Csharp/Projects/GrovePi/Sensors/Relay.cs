using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class Relay : GrovePiSensor
    {
        #region Constructor
        internal Relay(Pin pin, Transport transport, int period = 0) : base(pin, transport, period)
        {
            this.OnGetContinuousDatacb += OnGetRelaySensorData;
        }
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void ReadContinuously()
        {
            SharpPy msg = new SharpPy()
            {
                operation = RWCMD.DRRead,
                payload = "",

            };
            base.ReadContinuously(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnGetRelaySensorData(object data)
        {
#if DEBUG
            Console.WriteLine("Get Relay SensorData");
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
