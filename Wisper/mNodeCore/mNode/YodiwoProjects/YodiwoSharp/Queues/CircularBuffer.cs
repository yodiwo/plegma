using System;
using System.Collections.Generic;

namespace Yodiwo
{
    public class CircularBuffer<T> : Queue<T>
    {
        public int Size { get; private set; }

        public CircularBuffer(int size) { Size = size; }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);

            while (base.Count > Size)
            {
                base.Dequeue();
            }
        }
    }

    public class CircularBufferTS<T> : QueueTS<T>
    {
        public int Size { get; private set; }

        public CircularBufferTS(int size) { Size = size; }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);

            while (base.Count > Size)
            {
                base.Dequeue();
            }
        }
    }
}
