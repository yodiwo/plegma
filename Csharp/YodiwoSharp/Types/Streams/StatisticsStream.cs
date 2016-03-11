using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class StatisticsStream : Stream
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        object _syncRoot = new object();
        //------------------------------------------------------------------------------------------------------------------------
        public Stream BaseStream;
        public bool LeaveOpen = false;
        //------------------------------------------------------------------------------------------------------------------------
        DateTime _CreatedTimestamp = DateTime.Now;
        public DateTime CreatedTimestamp => _CreatedTimestamp;
        //------------------------------------------------------------------------------------------------------------------------
        public Int64 TotalBytesRead = 0;
        public Int64 TotalBytesWritten = 0;
        //------------------------------------------------------------------------------------------------------------------------
        Int64 currTxSecondID, currRxSecondID;
        Int64 currTxSecondTotalBytes, currRxSecondTotalBytes;
        Int64 _txPerSecond, _rxPerSecond;
        public Int64 ReadBytesPerSecond { get { return updateTxMetrics(0); } }
        public Int64 WriteBytesPerSecond { get { return updateRxMetrics(0); } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public StatisticsStream(Stream Stream, bool LeaveOpen)
        {
            this.BaseStream = Stream;
            this.LeaveOpen = LeaveOpen;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool CanRead { get { return BaseStream.CanRead; } }
        public override bool CanSeek { get { return BaseStream.CanSeek; } }
        public override bool CanWrite { get { return BaseStream.CanWrite; } }
        public override long Length { get { return BaseStream.Length; } }
        public override long Position { get { return BaseStream.Position; } set { BaseStream.Position = value; } }
        public override void Flush() { BaseStream.Flush(); }
        public override long Seek(long offset, SeekOrigin origin) { return BaseStream.Seek(offset, origin); }
        public override void SetLength(long value) { BaseStream.SetLength(value); }
        //------------------------------------------------------------------------------------------------------------------------
        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_syncRoot)
            {
                var b = BaseStream.Read(buffer, offset, count);
                Interlocked.Add(ref TotalBytesRead, b.ClampFloor(0));
                updateRxMetrics(TotalBytesRead);
                return b;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_syncRoot)
            {
                BaseStream.Write(buffer, offset, count);
                Interlocked.Add(ref TotalBytesWritten, count);
                updateTxMetrics(TotalBytesWritten);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private long updateTxMetrics(Int64 txBytes)
        {
            var secID = (DateTime.Now - _CreatedTimestamp).TotalSeconds;
            var secDiff = secID - currTxSecondID;
            //if we are still in same second
            if (secDiff <= 0)
            {
                currTxSecondTotalBytes += txBytes;
                return _txPerSecond;
            }
            else
            {
                if (currTxSecondTotalBytes > _txPerSecond)
                    _txPerSecond = currTxSecondTotalBytes;
                else
                    _txPerSecond = _txPerSecond / 2 + (Int64)((currTxSecondTotalBytes / secDiff) / 2);  //a 2 frame smoothing filter
                //reset
                currTxSecondID = (Int64)secID;
                currTxSecondTotalBytes = txBytes;
                return _txPerSecond;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private long updateRxMetrics(Int64 rxBytes)
        {
            var secID = (DateTime.Now - _CreatedTimestamp).TotalSeconds;
            var secDiff = secID - currRxSecondID;
            //if we are still in same second
            if (secDiff <= 0)
            {
                currRxSecondTotalBytes += rxBytes;
                return _rxPerSecond;
            }
            else
            {
                if (currRxSecondTotalBytes > _rxPerSecond)
                    _rxPerSecond = currRxSecondTotalBytes;
                else
                    _rxPerSecond = _rxPerSecond / 2 + (Int64)((currRxSecondTotalBytes / secDiff) / 2);  //a 2 frame smoothing filter
                //reset
                currRxSecondID = (Int64)secID;
                currRxSecondTotalBytes = rxBytes;
                return _rxPerSecond;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public override void Close()
        {
            base.Close();
            if (!LeaveOpen)
            {
                try
                {
                    BaseStream.Close();
                    BaseStream.Dispose();
                }
                catch (Exception ex) { DebugEx.TraceError(ex, "Exception while closing underlying stream"); }
            }
        }
#elif UNIVERSAL
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && !LeaveOpen)
            {
                GC.SuppressFinalize(this);
                try
                {
                    BaseStream.Dispose();
                }
                catch (Exception ex) { DebugEx.TraceError(ex, "Exception while closing underlying stream"); }
            }
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        ~StatisticsStream()
        {
            if (!LeaveOpen)
            {
                try
                {
                    if (BaseStream != null)
                        BaseStream.Dispose();
                }
                catch (Exception ex) { DebugEx.TraceError(ex, "Exception while closing underlying stream"); }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
