using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Projects.GrovePi
{

    public enum PinMode
    {
        Input = 0,
        Ouput = 1,
    }

    public enum Pin : byte
    {
        AnalogPin0 = 0,
        AnalogPin1 = 1,
        AnalogPin2 = 2,
        DigitalPin2 = 2,
        DigitalPin3 = 3,
        DigitalPin4 = 4,
        DigitalPin5 = 5,
        DigitalPin6 = 6,
        DigitalPin7 = 7,
        DigitalPin8 = 8,
        I2C = 9,
        Unknown = 255
    }

    public class GrovePiSensor
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        private object _locker = new object();

        private Pin _pin;
        public Pin Pin { get { lock (_locker) return _pin; } set { if (value != Pin.Unknown) { lock (_locker) _pin = value; } } }
        //------------------------------------------------------------------------------------------------------------------------
        SensorWatcher _watcher;
        public SensorWatcher Watcher { get { return _watcher; } }
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnGetContinuousData(object data);
        public event OnGetContinuousData OnGetContinuousDatacb = delegate { };
        public Transport transport;
        //------------------------------------------------------------------------------------------------------------------------
        public static readonly Dictionary<string, Pin> PinNameToPin = new Dictionary<string, Pin>()
        {
            { "A0", Pin.AnalogPin0 },
            { "A1", Pin.AnalogPin1 },
            { "A2", Pin.AnalogPin2 },
            { "D2", Pin.DigitalPin2},
            { "D3", Pin.DigitalPin3},
            { "D4", Pin.DigitalPin4},
            { "D5", Pin.DigitalPin5},
            { "D6", Pin.DigitalPin6},
            { "D7", Pin.DigitalPin7},
            { "D8", Pin.DigitalPin8},
            { "I2C", Pin.I2C       }
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public GrovePiSensor(Pin p, Transport trans, int period = 0, Type datatype = null)
        {
            Pin = p;
            this.transport = trans;
            _watcher = new SensorWatcher(this, period, trans);
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
        public object ReadSingleValue<T>(T data)
        {
            var res = _watcher.GetSingleValue<T>(data);
            return res;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void ReadContinuously<T>(T data)
        {
            _watcher.StartContinuousMonitoring(data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void ReadContinuously()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void StopMonitoring()
        {
            _watcher.StopContinuousMonitoring();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void SetValue(string data)
        {
            _watcher.SendMessage(data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void GetFirmwareVersion()
        {
            //unsupported
        }
        //------------------------------------------------------------------------------------------------------------------------

        public string DigitalRead(Pin pin)
        {
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------

        public void DigitalWrite(string value)
        {
            SharpPy msg = new SharpPy()
            {
                pin = this.Pin.ToString(),
                operation = RWCMD.DRWrite,
                payload = value,
                isRequest = true
            };
            DebugEx.TraceLog("DigitalWrite: Send to python: " + msg);
            this.transport.Send2python(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public string AnalogRead(Pin pin)
        {
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AnalogWrite(Pin pin, byte value)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void PinMode(Pin pin, PinMode mode)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual object DeserializePayload(string payload)
        {
            return payload;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
