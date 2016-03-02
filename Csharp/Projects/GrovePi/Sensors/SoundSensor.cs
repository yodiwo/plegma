using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{
    public class SoundSensor : GrovePiSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal SoundSensor(Pin pin, Transport transport, int period = 0)
            : base(pin, transport, period)
        {
            this.OnGetContinuousDatacb += OnGetSoundSensorData;
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
        public void OnGetSoundSensorData(object data)
        {
#if DEBUG
            Console.WriteLine("Get Sound Sensor");
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
