using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using Yodiwo;

namespace Yodiwo.PaaS.Azure
{
    public class SimpleEventProcessor : IEventProcessor
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        Stopwatch checkpointStopWatch;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            DebugEx.TraceLog("Processor Shutting Down. Partition :" + context.Lease.PartitionId + ", Reason: " + reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            DebugEx.TraceLog("SimpleEventProcessor initialized.  Partition: " + context.Lease.PartitionId + ", Offset: " + context.Lease.Offset);
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }
        //------------------------------------------------------------------------------------------------------------------------
        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                DebugEx.TraceLog("Message received.  Partition: " + context.Lease.PartitionId + " Data:" + data);
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
