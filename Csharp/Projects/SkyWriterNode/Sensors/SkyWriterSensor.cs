using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.SkyWriter
{
    public class SkyWriterSensor
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //each skywriter sensor has its own watcher
        public SensorWatcher watcher;
        public delegate void OnGetContinuousData(object data);
        public event OnGetContinuousData OnGetContinuousDatacb = delegate { };
        //csharp2python interface
        public Transport transport;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public SkyWriterSensor(Transport trans, Type datatype = null)
        {
            this.transport = trans;
            watcher = new SensorWatcher(this, trans);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void OnGetValue(object data)
        {
            if (OnGetContinuousDatacb != null)
                OnGetContinuousDatacb(data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void ReadContinuously<T>(T data)
        {
            watcher.StartContinuousMonitoring(data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Read() { }
        //------------------------------------------------------------------------------------------------------------------------
        //basic deserialization method
        public virtual object DeserializePayload(string payload)
        {
            return payload;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
