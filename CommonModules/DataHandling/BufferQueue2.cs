using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace DataHandling
{
    /// <summary>
    /// 기존의 queue와는 달리 List를 이용하여 만들어졌다.
    /// 자유롭게 내부의 내용에 access할 수 있고, Push와 Pop 기능도 있으며,
    /// Enqueue, Dequeue기능이 있다.
    /// </summary>
    public class BufferQueue2
    {
        List<Array> _buffer;
        
        public BufferQueue2()
        {
            _buffer = new List<Array>();
        }
        public ErrorMsg enqueueFrom(Array src)
        {
            return enqueueFrom(src, 0, -1);
        }

        public ErrorMsg pushFrom(Array src)
        {
            return enqueueFrom(src, 0, -1);
        }

        public ErrorMsg enqueueFrom(Array src, int byteSize = -1)
        {
            return enqueueFrom(src, 0, byteSize);
        }

        public ErrorMsg pushFrom(Array src, int byteSize = -1)
        {
            return enqueueFrom(src, 0, byteSize);
        }

        /// <summary>
        /// 기본적으로 pushFrom와 enqueueFrom은 같은 일을 한다.
        /// 버퍼의 내용을 size만큼 잘라서 stack에 집어넣는다.
        /// </summary>
        /// <param name="src">데이터소스배열</param>
        /// <param name="offset">데이터소스배열 내에서 복사를시작할 byte위치</param>
        /// <param name="byteSize">복사할 byte size</param>
        /// <returns>내부버퍼에 존재하는 데이터의 총 byteSize</returns>
        public ErrorMsg enqueueFrom(Array src, int offset, int byteSize)
        {
            if (byteSize < 0) byteSize = Buffer.ByteLength(src);
            if (byteSize == 0) return ErrorMsg.ZeroSizeRequested;
            byte[] dst = new byte[byteSize];
            Buffer.BlockCopy(src, offset, dst, 0, byteSize);
            Enqueue(dst);
            return ErrorMsg.NoError;
        }

        /// <summary>
        /// 기본적으로 pushFrom와 enqueueFrom은 같은 일을 한다.
        /// 버퍼의 내용을 size만큼 잘라서 stack에 집어넣는다.
        /// </summary>
        /// <param name="src">데이터소스배열</param>
        /// <param name="offset">데이터소스배열 내에서 복사를시작할 byte위치</param>
        /// <param name="byteSize">복사할 byte size</param>
        /// <returns>내부버퍼에 존재하는 데이터의 총 byteSize</returns>
        public ErrorMsg pushFrom(Array src, int offset, int byteSize)
        {
            if (byteSize < 0) byteSize = Buffer.ByteLength(src);
            byte[] dst = new byte[byteSize];
            Buffer.BlockCopy(src, offset, dst, 0, byteSize);
            Enqueue(dst);
            return ErrorMsg.NoError;
        }

        /// <summary>
        /// 버퍼의 내부내용을 access할 수 있다. get or set
        /// </summary>
        /// <param name="index">버퍼에 저장된 순서대로 index되어있다.</param>
        /// <returns>버퍼의 내용을 돌려준다. 버퍼의 내용이 사라지지는 않는다.</returns>
        public Array this[int index]{
            get{
                return _buffer[index];
            }
            set{
                _buffer[index] = value;
            }
        }

        public void Push(Array arr)
        {
            _buffer.Add(arr);
        }

        public void Enqueue(Array arr)
        {
            _buffer.Add(arr);
        }
        
        public Array Dequeue(out ErrorMsg errmsg, int byteSize=-1)
        {
            if (byteSize == 0){
                errmsg = ErrorMsg.ZeroSizeRequested;
                return null;
            }
            else if (_buffer.Count == 0){
                errmsg = ErrorMsg.BufferIsEmpty;
                return null;
            }
            else if(byteSize<0) { //기본은 버퍼에서 하나의 array를 꺼내어 주는 것이다. udp통신 시 유리..
                Array arr = _buffer[0];
                _buffer.RemoveAt(0);
                errmsg = ErrorMsg.NoError;
                return arr;
            }

            int lastIndex= 0;
            int totalSize = 0;
            
            for(int i=0; (totalSize<byteSize) && i<_buffer.Count; i++){
                totalSize+= Buffer.ByteLength(_buffer[i]);
                lastIndex = i;
            }
            if(totalSize<byteSize){ //충분한 크기를 아직 확보하지 못했다.
                errmsg = ErrorMsg.LessThanRequested;
                return null; //아직 없는 것이나마찬가지임..
            }else{
                Array arr = new Byte[byteSize];
                int offset = 0;
                int buffSize;
                for(int i=0; i<lastIndex-1; i++){ //맨 마지막 것은 따로 처리할 것임.
                    buffSize = Buffer.ByteLength(_buffer[0]);
                    Buffer.BlockCopy(_buffer[0], 0, arr, offset, buffSize);
                    offset+=buffSize;
                    _buffer.RemoveAt(0);//앞에있는 것 하나를 삭제함.
                }
                //맨 마지막 것은 따로 처리함. 남는 경우에는 남은 크기를 따로 처리해 주어야 함.
                buffSize = Buffer.ByteLength(_buffer[0]);
                int copySize = (byteSize - offset); //copySize가 buffSize보다 작을 경우 남은 것을 0에 넣어주어야 함.
                Buffer.BlockCopy(_buffer[0], 0, arr, offset, copySize);

                int remainedSize = buffSize-copySize;
                
                if(remainedSize>0){
                    Byte[] remained = new Byte[remainedSize];
                    Buffer.BlockCopy(_buffer[0], copySize, remained, 0, remainedSize);
                    _buffer.RemoveAt(0);//앞에있는 것 하나를 삭제함.
                    _buffer.Insert(0, remained);//남은 것을 앞에 추가함.
                }else{
                    _buffer.RemoveAt(0);//앞에있는 것 하나를 삭제함.
                }
                errmsg = ErrorMsg.NoError;
                return arr;
            }
        }

        public Array Pop(out ErrorMsg errmsg, int byteSize = -1)
        {
            if (byteSize == 0)
            {
                errmsg = ErrorMsg.ZeroSizeRequested;
                return null;
            }
            else if (_buffer.Count == 0)
            {
                errmsg = ErrorMsg.BufferIsEmpty;
                return null;
            }
            else if (byteSize < 0)
            { //기본은 버퍼에서 하나의 array를 꺼내어 주는 것이다. udp통신 시 유리..
                Array arr = _buffer[_buffer.Count-1];
                _buffer.RemoveAt(_buffer.Count - 1);
                errmsg = ErrorMsg.NoError;
                return arr;
            }

            int lastIndex = 0;
            int totalSize = 0;

            for (int i = _buffer.Count - 1; (totalSize < byteSize) && i >=0; i--)
            {
                totalSize += Buffer.ByteLength(_buffer[i]);
                lastIndex = i;
            }
            if (totalSize < byteSize)
            { //충분한 크기를 아직 확보하지 못했다.
                errmsg = ErrorMsg.LessThanRequested;
                return null; //아직 없는 것이나마찬가지임..
            }
            else
            {
                Array arr = new Byte[byteSize];
                int offset = 0;
                int buffSize;
                for (int i = _buffer.Count - 1; i >= lastIndex; i--)
                { //맨 마지막 것은 따로 처리할 것임.
                    buffSize = Buffer.ByteLength(_buffer[_buffer.Count - 1]);
                    Buffer.BlockCopy(_buffer[_buffer.Count - 1], 0, arr, offset, buffSize);
                    offset += buffSize;
                    _buffer.RemoveAt(_buffer.Count - 1);//앞에있는 것 하나를 삭제함.
                }
                //맨 마지막 것은 따로 처리함. 남는 경우에는 남은 크기를 따로 처리해 주어야 함.
                buffSize = Buffer.ByteLength(_buffer[_buffer.Count - 1]);
                int copySize = (byteSize - offset); //copySize가 buffSize보다 작을 경우 남은 것을 0에 넣어주어야 함.
                Buffer.BlockCopy(_buffer[_buffer.Count - 1], 0, arr, offset, copySize);

                int remainedSize = buffSize - copySize;

                if (remainedSize > 0)
                {
                    Byte[] remained = new Byte[remainedSize];
                    Buffer.BlockCopy(_buffer[_buffer.Count - 1], copySize, remained, 0, remainedSize);
                    _buffer.RemoveAt(_buffer.Count - 1);//앞에있는 것 하나를 삭제함.
                    _buffer.Add(remained);//남은 것을 뒤에 추가함.
                }
                else
                {
                    _buffer.RemoveAt(_buffer.Count - 1);//앞에있는 것 하나를 삭제함.
                }
                errmsg = ErrorMsg.NoError;
                return arr;
            }
        }


        public void DequeueTo(out ErrorMsg errmsg, Array dst)
        {
            DequeueTo(out errmsg, dst, 0, -1);
        }
        public void DequeueTo(out ErrorMsg errmsg, Array dst, int size = -1)
        {
            if (size < 0) size = Buffer.ByteLength(dst);
            DequeueTo(out errmsg, dst, 0, size);
        }

        public void PopTo(out ErrorMsg errmsg, Array dst)
        {
            PopTo(out errmsg, dst, 0, -1);
        }

        /// <summary>
        /// stack에서 데이터를 가져간다.
        /// </summary>
        /// <param name="errmsg"></param>
        /// <param name="dst"></param>
        /// <param name="size"></param>
        public void PopTo(out ErrorMsg errmsg, Array dst, int size = -1)
        {
            if (size < 0) size = Buffer.ByteLength(dst);
            PopTo(out errmsg, dst, 0, size);
        }


        /// <summary>
        /// queue에서 데이터를 가져간다.
        /// </summary>
        /// <param name="errmsg">실패나 성공시 해당 메시지를 리턴한다.</param>
        /// <param name="dst">대상이 되는 버퍼</param>
        /// <param name="offset">대상이 되는 버퍼의 시작점</param>
        /// <param name="byteSize">복사할 크기. -1을 넣으면 버퍼상에 저장된 Array구조 하나만 가져온다.</param>
        /// <returns>버퍼에 복사 된 크기를 리턴한다.</returns>
        public int DequeueTo(out ErrorMsg errmsg, Array dst, int offset, int byteSize)
        {
            if (byteSize == 0)
            {
                errmsg = ErrorMsg.ZeroSizeRequested;
                return 0;
            }
            else if (_buffer.Count == 0)
            {
                errmsg = ErrorMsg.BufferIsEmpty;
                return 0;
            }
            else if ((Buffer.ByteLength(dst)+offset) < byteSize) //대상 배열의 크기가 byteSize보다 작다.
            {
                errmsg = ErrorMsg.SmallerDestSize;
                return 0;
            }
            else if (byteSize < 0) //byteSize가 0보다 작으면 기본값이 온 것이다.
            { //기본은 버퍼에서 하나의 array를 꺼내어 주는 것이다. udp통신 시 유리..
                int buffSize = Buffer.ByteLength(_buffer[0]);
                Buffer.BlockCopy(_buffer[0], 0, dst, offset, buffSize);
                _buffer.RemoveAt(0);
                errmsg = ErrorMsg.NoError;
                return buffSize;
            }

            int lastIndex = 0;
            int totalSize = 0;

            for (int i = 0; (totalSize < byteSize) && i < _buffer.Count; i++) //먼저 크기를 정한다.
            {
                totalSize += Buffer.ByteLength(_buffer[i]);
                lastIndex = i;
            }

            if (totalSize < byteSize)
            { //충분한 크기를 아직 확보하지 못했다.
                errmsg = ErrorMsg.LessThanRequested;
                return 0; //아직 없는 것이나마찬가지임..
            }
            else //충분한 크기가 버퍼에 들어있다.
            {
                int buffSize;
                for (int i = 0; i < lastIndex - 1; i++)
                { //맨 마지막 것은 따로 처리할 것임.
                    buffSize = Buffer.ByteLength(_buffer[0]);
                    Buffer.BlockCopy(_buffer[0], 0, dst, offset, buffSize);
                    offset += buffSize;
                    _buffer.RemoveAt(0);//앞에있는 것 하나를 삭제함.
                }
                //맨 마지막 것은 따로 처리함. 남는 경우에는 남은 크기를 따로 처리해 주어야 함.
                buffSize = Buffer.ByteLength(_buffer[0]);
                int copySize = (byteSize - offset); //copySize가 buffSize보다 작을 경우 남은 것을 0에 넣어주어야 함.
                Buffer.BlockCopy(_buffer[0], 0, dst, offset, copySize);

                int remainedSize = buffSize - copySize;

                if (remainedSize > 0)
                {
                    Byte[] remained = new Byte[remainedSize];
                    Buffer.BlockCopy(_buffer[0], copySize, remained, 0, remainedSize);
                    _buffer.RemoveAt(0);//앞에있는 것 하나를 삭제함.
                    _buffer.Insert(0, remained);//남은 것을 앞에 추가함.
                }
                else
                {
                    _buffer.RemoveAt(0);//앞에있는 것 하나를 삭제함.
                }
                errmsg = ErrorMsg.NoError;
                return byteSize;
            }
        }

        /// <summary>
        /// stack에서 데이터를 가져간다.
        /// </summary>
        /// <param name="errmsg">실패나 성공시 해당 메시지를 리턴한다.</param>
        /// <param name="dst">대상이 되는 버퍼</param>
        /// <param name="offset">대상이 되는 버퍼의 시작점</param>
        /// <param name="byteSize">복사할 크기. -1을 넣으면 버퍼상에 저장된 Array구조 하나만 가져온다.</param>
        /// <returns>버퍼에 복사 된 크기를 리턴한다.</returns>
        public int PopTo(out ErrorMsg errmsg, Array dst, int offset, int byteSize)
        {
            if (byteSize == 0)
            {
                errmsg = ErrorMsg.ZeroSizeRequested;
                return 0;
            }
            else if (_buffer.Count == 0)
            {
                errmsg = ErrorMsg.BufferIsEmpty;
                return 0;
            }
            else if ((Buffer.ByteLength(dst) + offset) < byteSize) //대상 배열의 크기가 byteSize보다 작다.
            {
                errmsg = ErrorMsg.SmallerDestSize;
                return 0;
            }
            else if (byteSize < 0) //byteSize가 0보다 작으면 기본값이 온 것이다.
            { //기본은 버퍼에서 하나의 array를 꺼내어 주는 것이다. udp통신 시 유리..
                int buffSize = Buffer.ByteLength(_buffer[_buffer.Count - 1]);
                Buffer.BlockCopy(_buffer[_buffer.Count - 1], 0, dst, offset, buffSize);
                _buffer.RemoveAt(_buffer.Count - 1);
                errmsg = ErrorMsg.NoError;
                return buffSize;
            }

            int lastIndex = 0;
            int totalSize = 0;

            for (int i = _buffer.Count - 1; (totalSize < byteSize) && i >=0; i--) //먼저 크기를 정한다.
            {
                totalSize += Buffer.ByteLength(_buffer[i]);
                lastIndex = i;
            }

            if (totalSize < byteSize)
            { //충분한 크기를 아직 확보하지 못했다.
                errmsg = ErrorMsg.LessThanRequested;
                return 0; //아직 없는 것이나마찬가지임..
            }
            else //충분한 크기가 버퍼에 들어있다.
            {
                int buffSize;
                for (int i = _buffer.Count - 1; i >= lastIndex; i--)
                { //맨 마지막 것은 따로 처리할 것임.
                    buffSize = Buffer.ByteLength(_buffer[_buffer.Count - 1]);
                    Buffer.BlockCopy(_buffer[_buffer.Count - 1], 0, dst, offset, buffSize);
                    offset += buffSize;
                    _buffer.RemoveAt(_buffer.Count - 1);//앞에있는 것 하나를 삭제함.
                }
                //맨 마지막 것은 따로 처리함. 남는 경우에는 남은 크기를 따로 처리해 주어야 함.
                buffSize = Buffer.ByteLength(_buffer[_buffer.Count - 1]);
                int copySize = (byteSize - offset); //copySize가 buffSize보다 작을 경우 남은 것을 0에 넣어주어야 함.
                Buffer.BlockCopy(_buffer[_buffer.Count - 1], 0, dst, offset, copySize);

                int remainedSize = buffSize - copySize;

                if (remainedSize > 0)
                {
                    Byte[] remained = new Byte[remainedSize];
                    Buffer.BlockCopy(_buffer[_buffer.Count - 1], copySize, remained, 0, remainedSize);
                    _buffer.RemoveAt(_buffer.Count - 1);//앞에있는 것 하나를 삭제함.
                    _buffer.Add(remained);//남은 것을 뒤에 추가함.
                }
                else
                {
                    _buffer.RemoveAt(_buffer.Count - 1);//앞에있는 것 하나를 삭제함.
                }
                errmsg = ErrorMsg.NoError;
                return byteSize;
            }
        }
        
        /// <summary>
        /// 내부에 저장된 Array들의 모든 ByteSize를 계산한다.
        /// </summary>
        public int Size
        {
            get
            {
                int size = 0;
                for (int i = 0; i < _buffer.Count; i++)
                {
                    size += Buffer.ByteLength(_buffer.ElementAt(i) as Array);
                }
                return size;
            }
        }

        /// <summary>
        /// Array들의 개수를 가져온다. udp통신시 패킷이 몇 개인지 알아보는데에만 사용되어야한다.
        /// </summary>
        public int Count { get { return _buffer.Count;  } }

        /// <summary>
        /// 큐에 저장된 모든 데이터를 삭제한다.
        /// </summary>
        public void Clear()
        {
            _buffer.Clear();
        }
    }
}
