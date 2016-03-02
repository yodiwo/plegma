using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.SkyWriter
{
    public class GestureSensor : SkyWriterSensor
    {
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal GestureSensor(Transport transport) :
            base(transport)
        {
            //register gesture events(tap,touch,doubletap,airwheel,flick)
            //this.OnGetContinuousDatacb += OnGetGestureEventsData;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions

        //------------------------------------------------------------------------------------------------------------------------
        public override void Read()
        {
            SharpPy msg = new SharpPy()
            {
                operation = CMD.Gesture,
                payload = "",
            };
            base.ReadContinuously(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnGetGestureEventsData(object data)
        {
            Console.WriteLine("Get GestureEvents");
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override object DeserializePayload(string payload)
        {
            //deserialize gesture events
            var gesturedata = payload.FromJSON<GestureEvents>();
            return gesturedata;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

    public class GestureEvents
    {
        public string flick;
        public string tap;
        public string touch;
        public string airwheel;
        public string doubletap;
    }


}
