using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    /// <summary>
    /// Simple Blocking FIFO queue
    /// </summary>
    /// <typeparam name="T">types of objects that the queue holds</typeparam>
    public class BlockingQueue<T> : BlockingCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of a Blocking Queue
        /// Add and TryAdd for Enqueue and TryEnqueue
        /// Take and TryTake for Dequeue and TryDequeue
        /// </summary>
        public BlockingQueue() : base(new ConcurrentQueue<T>()) { }

        /// <summary>
        /// Initializes a new instance of an upper-bound Blocking Queue
        /// Add and TryAdd for Enqueue and TryEnqueue
        /// Take and TryTake for Dequeue and TryDequeue
        /// </summary>
        /// <param name="maxSize">upper bound of queue, after which Adds will block</param>
        public BlockingQueue(int maxSize) : base(new ConcurrentQueue<T>(), maxSize) { }
    }
}
