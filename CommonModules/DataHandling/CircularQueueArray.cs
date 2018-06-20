using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace DataHandling
{
    public class CircularQueueArray<T>
    {
        T[] _buffer;
        int _bufferByteSize;
        int _filledSize;
        int _startOffset;
        public CircularQueueArray(int size=10000)
        {
            _buffer = new T[size];
            _bufferByteSize = Buffer.ByteLength(_buffer);
            _filledSize = 0;
            _startOffset = 0;
        }

        public bool BlockCopyFrom(Array fromBuffer, int fromOffset, int byteSize)
        {
            int remainBufferSize = (_bufferByteSize - _startOffset);
            int rest = (byteSize>remainBufferSize)? byteSize-remainBufferSize : 0;
            int copySize = (rest == 0) ? byteSize : remainBufferSize;

            Buffer.BlockCopy(fromBuffer, fromOffset, _buffer, _startOffset, copySize);
            _startOffset += copySize;
            if (rest > 0) //
            {
            }
            return false;
        }

        public bool ValueCopyFrom(T[] array, int startIndex, int count)
        {
            return false;
        }

        public bool BlockCopyFromSwap(Array fromBuffer, int fromOffset, int byteSize, int typeSize)
        {
            return false;
        }

        public bool ValueCopyFromSwap(T[] array, int startIndex, int count)
        {
            return false;
        }

        public bool BlockCopyTo(Array fromBuffer, int fromOffset, int byteSize)
        {
            return false;
        }

        public bool ValueCopyTo(T[] array, int startIndex, int count)
        {
            return false;
        }

        public bool BlockCopyToSwap(Array fromBuffer, int fromOffset, int byteSize, int typeSize)
        {
            return false;
        }

        public bool ValueCopyToSwap(T[] array, int startIndex, int count)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>total size in queue after enqueue processed</returns>
        public int Enqueue(T value)
        {
            return -1;
        }

        public T Dequeue()
        {
            T[] a = new T[1];
            return a[0];
        }

        public int ByteSize { get { return _filledSize; } }

        public int Count { get { return _filledSize / (Marshal.SizeOf(typeof(T))); } }

    }
}
