using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.SkyWriter
{
    public class PositionSensor : SkyWriterSensor
    {
        internal PositionSensor(Transport transport) :
            base(transport)
        {
            //register hook event for motion event
            //each motion event is characterized by x,y,z coordinates
            //this.OnGetContinuousDatacb += OnGetPositionData;
        }

        public override void Read()
        {
            SharpPy msg = new SharpPy()
            {
                operation = CMD.Position,
                payload = "",
            };
            base.ReadContinuously(msg);
        }

        public void OnGetPositionData(object data)
        {
            Console.WriteLine("Get PositionEvents");
        }

        public override object DeserializePayload(string payload)
        {
            //deserialize position data
            var positiondata = payload.FromJSON<PositionData>();
            return positiondata;
        }
    }

    public class PositionData
    {
        public string x;
        public string y;
        public string z;
    }
}

