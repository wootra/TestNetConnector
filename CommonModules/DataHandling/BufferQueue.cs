using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace DataHandling
{
    public class BufferQueue
    {
        Queue<Array> _buffer;
        int _totalSize = 0;
        Byte[] _remained = null;
        
        public BufferQueue(int size=-1)
        {
            if (size <= 0) _buffer = new Queue<Array>();
            else _buffer = new Queue<Array>(size);
        }
        public int enqueueFrom(Array buffer)
        {
            return enqueueFrom(buffer, 0, -1);
        }

        public int enqueueFrom(Array buffer, int size = -1)
        {
            return enqueueFrom(buffer, 0, size);
        }
        public int enqueueFrom(Array buffer, int offset, int size)
        {
            if (size < 0) size = Buffer.ByteLength(buffer);
            byte[] dst = new byte[size];
            Buffer.BlockCopy(buffer, offset, dst, 0, size);
            Enqueue(dst);

            return _totalSize;
        }
        
        public int Enqueue(Array arr) {
            _buffer.Enqueue(arr);
            _totalSize += Buffer.ByteLength(arr);
            return _totalSize; 
        }
        public Array dequeue() {
            if (_remained != null)
            {//이전에 빼가고 남은 버퍼가 있으면
                Byte[] returnBuff = _remained;
                _totalSize -= Buffer.ByteLength(_remained);
                _remained = null;
                return returnBuff;//그 버퍼를 리턴한다.
            }
            if (_buffer.Count == 0)
            {
                return null;
            }
            _totalSize -= Buffer.ByteLength(_buffer.Last());
            return _buffer.Dequeue();
        }
        public Byte[] dequeue(int byteSize)
        {
            if (byteSize == 0) return null;
            Byte[] dst = new Byte[byteSize];
            dequeueTo(dst);
            return dst;
        }


        public void dequeueTo(Array dst)
        {
            dequeueTo(dst, 0, -1);
        }
        public void dequeueTo(Array dst, int size = -1)
        {
            if (size < 0) size = Buffer.ByteLength(dst);
            dequeueTo(dst, 0, size);
        }

        public void dequeueTo(Array dst, int offset, int byteSize)
        {
            if (byteSize > _totalSize) throw new IndexOutOfRangeException("큐에 저장된 데이터보다 큰 데이터를 요구했습니다. 요구한 크기:" + byteSize + " / 큐 데이터크기:" + _totalSize);
            if (byteSize < 0) byteSize = Buffer.ByteLength(dst) - offset;
            int size = offset;
            int rest = byteSize;
            _totalSize -= byteSize;
            if (_remained != null) //이전에 가져가고 남은 버퍼가 있을 때..
            {
                int remainSize = Buffer.ByteLength(_remained);
                if (remainSize <= byteSize) //남은 버퍼보다 큰 양을 가져갈때
                {
                    Buffer.BlockCopy(_remained, 0, dst, size, remainSize);
                    rest -= remainSize;
                    _remained = null;//더이상 남은 것이 없다.
                    size += remainSize;
                    
                }
                else //남은 버퍼보다 적은 양을 가져갈 땐 _reamined가 남는다.
                {
                    Byte[] newRemained = new Byte[remainSize - byteSize];
                    Buffer.BlockCopy(_remained, 0, dst, size, byteSize);
                    Buffer.BlockCopy(_remained, byteSize, newRemained, 0, newRemained.Length);
                    _remained = newRemained;
                    return; //바로 리턴한다.
                }
            }
            while (rest > 0)
            {
                Array first = _buffer.Dequeue(); //첫 버퍼를 꺼낸다.
                int firstBuffSize = Buffer.ByteLength(first);
                if (firstBuffSize > rest)//첫 버퍼가 더 클때
                {
                    _remained = new Byte[firstBuffSize - rest];
                    Buffer.BlockCopy(first, 0, dst, size, rest);
                    Buffer.BlockCopy(first, rest, _remained, 0, Buffer.ByteLength(_remained));
                    return;
                }
                else //첫 버퍼가 더 작을 때..
                {
                    Buffer.BlockCopy(first, 0, dst, size, firstBuffSize);
                    size += firstBuffSize;
                    rest -= size;
                    //다음 항목으로 간다.
                }
            }
        }
        public int Length { get { return _totalSize; } }
        public int Size
        {
            get
            {
                int size = 0;
                if (_remained != null) size += Buffer.ByteLength(_remained);
                if (_buffer.Count > 0)
                {
                    for (int i = 0; i < _buffer.Count; i++)
                    {
                        size += Buffer.ByteLength(_buffer.ElementAt(i) as Array);
                    }
                }
                _totalSize = size;
                return size;
            }
        }
        public int Count { get { return _buffer.Count;  } }
        public void Clear()
        {
            _buffer.Clear();
            _remained = null;
        }
    }
}
