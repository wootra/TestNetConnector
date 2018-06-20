using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace NetworkModules
{
    public class BufferQueue
    {
        Queue<Byte> _buffer;
        public BufferQueue()
        {
            _buffer = new Queue<byte>();
        }
        public int enqueueFrom(Byte[] buffer, FileStream file)
        {
            return enqueueFrom(buffer, 0, -1, file);
        }

        public int enqueueFrom(Byte[] buffer, int size = -1, FileStream file = null)
        {
            return enqueueFrom(buffer, 0, size, file);
        }
        public int enqueueFrom(Byte[] buffer, int offset, int size, FileStream file)
        {
            if (size < 0) size = buffer.Length;
            for (int i = 0; i < size; i++)
            {
                _buffer.Enqueue(buffer[offset + i]);
                if (file != null)
                {
                    file.WriteByte(buffer[offset + i]);
                }
            }
            return _buffer.Count;
        }
        public int enqueue(Byte num) { _buffer.Enqueue(num); return _buffer.Count; }
        public Byte dequeue() { return _buffer.Dequeue();}

        public void dequeueTo(Array dst, FileStream file)
        {
            dequeueTo(dst, 0, -1, file);
        }
        public void dequeueTo(Array dst, int size = -1, FileStream file = null)
        {
            dequeueTo(dst, 0, size, file);
        }

        public void dequeueTo(Array dst, int offset, int size, FileStream file)
        {
            Byte unit;
            if(size<0){
                size = Marshal.SizeOf(dst.GetValue(0)) * dst.Length;
            }
            for (int i = 0; i < size; i++)
            {
                unit = _buffer.Dequeue();
                Buffer.SetByte(dst, offset + i, unit);
                if (file != null)
                {
                    file.WriteByte(unit);
                }
            }
        }
        public int Length { get { return _buffer.Count; } }
        public int Count { get { return _buffer.Count; } }

    }
}
