using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{

    public class Accelerometer : GrovePiSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal Accelerometer(Pin pin, Transport transport)
            : base(pin, transport)
        {
            this.OnGetContinuousDatacb += OnGetAccelerometerData;

        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Read()
        {
            SharpPy msg = new SharpPy()
            {
                operation = RWCMD.DRRead,
                payload = "",
            };
            var res = base.ReadSingleValue(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnGetAccelerometerData(object data)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override object DeserializePayload(string payload)
        {
            //deserialize accelerometer data
            var accelerometerData = payload.FromJSON<AccelerometerData>();
            return accelerometerData;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }

    public class AccelerometerData
    {
        public int x;
        public int y;
        public int z;
    }
}
