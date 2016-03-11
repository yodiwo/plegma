using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yodiwo;
using System.IO;
using Yodiwo.Tools;

namespace Yodiwo.Gateway.GatewayMain.Spike
{
    class SerialLink : GatewayLink
    {
        public delegate void SerialOpenedDelegate();

        public event OnMessageDelegate OnMessage = delegate { };
        private readonly string _portPart;
        private readonly int _baudRate;
        private SerialPort _serial;
        private string _portS;
        private readonly object _serialReadLock = new object();

        private readonly byte[] _currentPartialBytes = new byte[2048];

        private int _partialBytesLength;

        private Task _watchdog;

        public event SerialOpenedDelegate OnSerialOpened = delegate { };

        private Task _readTask; //for Mono, see init code

        readonly System.Text.UTF8Encoding _encoding = new System.Text.UTF8Encoding();

        public SerialLink(string portPart, int baudRate = 115200)
        {
            _portPart = portPart ?? "";
            _baudRate = baudRate;
        }

        public bool Start()
        {
            try
            {
                _portS = SerialPort.GetPortNames().FirstOrDefault(x => x.Contains(_portPart));
                if (string.IsNullOrWhiteSpace(_portS))
                    return false;
                _serial = new SerialPort
                {
                    PortName = _portS,
                    BaudRate = _baudRate,
                    Parity = Parity.None,
                    RtsEnable = true,
                    DtrEnable = true,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    WriteTimeout = 2000,
                    ReadTimeout = 50,
                    //                    ReadTimeout = SerialPort.InfiniteTimeout, //default actually, here for reference
                    //                ReadTimeout = 2000,
                    NewLine = '\0'.ToString(),
                };

                // if not on mono, set read onMessage before open
                //if (!IsRunningOnMono())
                //{
                //    _serial.DataReceived += DataReceivedHandler;
                //}

                _serial.ErrorReceived += OnSerialError;
                if (!SafeOpen(_serial))
                    return false;

                //mono does not support serial events, we need a read task
                //if on mono, start read task after open
                //if (IsRunningOnMono())
                //{
                if (_readTask == null || _readTask.IsCanceled || _readTask.IsCompleted)
                    _readTask = Task.Run((Action)SerialReadTask);
                //}

                OnSerialOpened();

                if (_watchdog == null || _watchdog.IsCanceled || _watchdog.IsCompleted)
                    _watchdog = Task.Run((Action)Watchdog);
                return true;
            }
            catch (Exception ex)
            {
                DebugEx.TraceWarning("error opening serial port: " + ex);
                return false;
            }

        }

        private void Watchdog()
        {
            if (_serial == null)
                return;
            while (true)
            {
                if (!SafeIsOpen(_serial))
                {
                    _serial.Close();
                    IfNotStartedStartUntilStarted();
                }
                Task.Delay(500).Wait();
            }
        }


        private void SerialReadTask()
        {
            byte[] buffer = new byte[1000];
            int read;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            while (true)
            {
                try
                {
                    read = _serial.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        ProcessSerialDataHeaderStyle(buffer, 0, read);
                        //var str = encoding.GetString(buffer, 0, read);
                        //ProcessSerialDataLineStyle(str);
                    }
                }
                catch (Exception)
                {
                }
                //                var data = _serial.ReadLine();
                //                OnMessage(data);
                Task.Delay(50).Wait();
            }
        }


        public void SendMessage(string msg)
        {
            try
            {
                var header = new byte[4];
                header[0] = 0x13;
                header[1] = 0x37;
                BigBitConverter.PutBytesOfUInt16(header, 2, (UInt16)msg.Length);
                _serial.Write(header, 0, header.Length);
                _serial.Write(msg);
                //                _serial.Write(_delimiterBytes, 0, _delimiterBytes.Length);
            }
            catch (Exception ex)
            {
                DebugEx.TraceWarning("Serial send error: " + ex);
                //                IfNotStartedStartUntilStarted();
            }
            Task.Delay(100).Wait();
        }

        public void SendMessage(byte[] bytes, int offset, int length)
        {
            try
            {
                var header = new byte[4];
                header[0] = 0x13;
                header[1] = 0x37;
                BigBitConverter.PutBytesOfUInt16(header, 2, (UInt16)length);
                _serial.Write(header, 0, header.Length);
                _serial.Write(bytes, offset, length);
            }
            catch (Exception ex)
            {
                DebugEx.TraceWarning("Serial send error: " + ex);
                //                IfNotStartedStartUntilStarted();
            }
            Task.Delay(100).Wait();
        }


        public void OnSerialError(object sender, SerialErrorReceivedEventArgs e)
        {
            DebugEx.TraceWarning("Serial error: " + e);
            //            IfNotStartedStartUntilStarted();
        }

        public void IfNotStartedStartUntilStarted()
        {
            while (_serial == null || !SafeIsOpen(_serial))
            {
                if (Start())
                    continue;
                Task.Delay(500).Wait();
            }
        }

        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //lock here?
            var str = _serial.ReadExisting();
            ProcessSerialDataHeaderStyle(str);
        }

#if false
    public void ProcessSerialDataLineStyle(byte[] bytes, int offset, int length)
        {
            lock (_serialReadLock)
            {
                try
                {
                    Array.Copy(bytes, offset, _currentPartialBytes, _partialBytesLength, length);
                    _partialBytesLength += length;

                    var pkts = _currentPartialPacket.Split(_delimiterArray, StringSplitOptions.None);
                    for (var i = 0; i < pkts.Length - 1; i++)
                    {
                        throw new Exception("reimplement this");
                        //OnMessage(pkts[i]);
                    }
                    _currentPartialPacket = pkts.Last();

                }
                catch (InvalidOperationException)
                {
                    _currentPartialPacket = "";
                }
            }
        }
#endif


        public void ProcessSerialDataHeaderStyle(string data)
        {
            byte[] bytes = _encoding.GetBytes(data);
            ProcessSerialDataHeaderStyle(bytes, 0, bytes.Length);
        }

        public void ProcessSerialDataHeaderStyle(byte[] bytes, int offset, int length)
        {
            lock (_serialReadLock)
            {
                //                try
                //                {
                Array.Copy(bytes, offset, _currentPartialBytes, _partialBytesLength, length);
                _partialBytesLength += length;
                while (_partialBytesLength >= 4)
                {
                    int magic = BigBitConverter.ToUInt16(_currentPartialBytes, 0);
                    if (magic != 0x1337)
                    {
                        var found = 0;
                        int i;
                        for (i = 1; i < _partialBytesLength; i++)
                        {
                            if (found == 0 && _currentPartialBytes[i] == 0x13)
                                found = 1;
                            else if (found == 1 && _currentPartialBytes[i] == 0x37)
                            {
                                found = 2;
                                break;
                            }
                            else
                                found = 0;
                        }
                        if (found == 0)
                        {
                            _partialBytesLength = 0;
                        }
                        else if (found == 1)
                        {
                            _currentPartialBytes[0] = 0x13;
                            _partialBytesLength = 1;
                        }
                        else if (found == 2)
                        {
                            Array.Copy(_currentPartialBytes, i - 1, _currentPartialBytes, 0, _partialBytesLength - i);
                            _partialBytesLength -= i;
                        }

                    }
                    // check if there is still a header, after garbage removal
                    if (_partialBytesLength < 4) continue;
                    int len = BigBitConverter.ToUInt16(_currentPartialBytes, 2);
                    //if we don't have a complete packet, return
                    if (_partialBytesLength < len + 4)
                        break;
                    //if we have a complete packet, send it to the onMessage, remove it from the buffer and run the processing for the remaining buffer
                    var packet = new byte[len];
                    Array.Copy(_currentPartialBytes, 4, packet, 0, len);
                    //for now
                    OnMessage(packet);

                    Array.Copy(_currentPartialBytes, len + 4, _currentPartialBytes, 0, _partialBytesLength - len - 4);
                    _partialBytesLength = _partialBytesLength - len - 4;
                }
                //                }
                //                catch (InvalidOperationException)
                //                {
                //                    _currentPartialPacket = "";
                //                }
            }
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        private static bool SafeOpen(SerialPort s)
        {
            var tryToOpen = true;

            if (IsRunningOnMono())
                tryToOpen = File.Exists(s.PortName);

            if (tryToOpen)
            {
                try
                {
                    s.Open();
                }
                catch (IOException)
                {
                    return false;
                }
                return true;
            }
            else
                return false;
            //                throw new IOException("Serial port is not ready");
        }

        private static bool SafeIsOpen(SerialPort serialPort)
        {
            if (IsRunningOnMono())
                return File.Exists(serialPort.PortName) && serialPort.IsOpen;

            return serialPort.IsOpen;
        }

        //        private void ClosePort()
        //        {
        //            if (_serial == null)
        //                return;
        //            _readTask.
        //        }
        //
    }

}
