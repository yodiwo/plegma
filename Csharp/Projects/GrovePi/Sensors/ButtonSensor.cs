using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class Button : GrovePiSensor
    {
        #region Constructor
        internal Button(Pin pin, Transport transport, int period = 0) : base(pin, transport, period)
        {
            this.OnGetContinuousDatacb += OnGetButtonSensorData;
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
        public void OnGetButtonSensorData(object data)
        {
#if DEBUG
            Console.WriteLine("Get Button SensorData");
#endif

        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
