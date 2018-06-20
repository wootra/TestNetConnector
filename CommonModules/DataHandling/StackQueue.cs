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
    public class StackQueue<T>
    {
        List<T> _buffer;
        
        public StackQueue()
        {
            _buffer = new List<T>();
        }
        

        /// <summary>
        /// 버퍼의 내부내용을 access할 수 있다. get or set
        /// </summary>
        /// <param name="index">버퍼에 저장된 순서대로 index되어있다.</param>
        /// <returns>버퍼의 내용을 돌려준다. 버퍼의 내용이 사라지지는 않는다.</returns>
        public T this[int index]{
            get{
                return _buffer[index];
            }
            set{
                _buffer[index] = value;
            }
        }

        public void Push(T arr)
        {
            _buffer.Add(arr);
        }

        public void Enqueue(T arr) {
            _buffer.Add(arr);
        }

        
        public T Dequeue(out ErrorMsg errmsg)
        {
            if (_buffer.Count == 0){
                errmsg = ErrorMsg.BufferIsEmpty;
                return default(T);
            }
            else { //기본은 버퍼에서 하나의 array를 꺼내어 주는 것이다. udp통신 시 유리..
                T arr = _buffer[0];
                _buffer.RemoveAt(0);
                errmsg = ErrorMsg.NoError;
                return arr;
            }
        }
        
        public T Dequeue()
        {
            ErrorMsg msg;
            return Dequeue(out msg);
        }

        public T Pop()
        {

            ErrorMsg msg;
            return Pop(out msg);
        }

        public T Pop(out ErrorMsg errmsg)
        {
            if (_buffer.Count == 0)
            {
                errmsg = ErrorMsg.BufferIsEmpty;
                return default(T);
            }
            else
            { //기본은 버퍼에서 하나의 array를 꺼내어 주는 것이다. udp통신 시 유리..
                T arr = _buffer[_buffer.Count-1];
                _buffer.RemoveAt(_buffer.Count - 1);
                errmsg = ErrorMsg.NoError;
                return arr;
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
