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
    public class ListQueue<T>:IList<T>// where T : class //,new()
    {
        List<T> _buffer;
        T _recentOuttedUnit;

        public ListQueue()
        {
            _buffer = new List<T>();
            _recentOuttedUnit = (new T[1])[0];//  null;
            
        }

        public bool Remove(T item){
            return _buffer.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _buffer.RemoveAt(index);
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

        public T RecentOutUnit
        {
            get { return _recentOuttedUnit; }
        }

        public void Push(T arr)
        {
            _buffer.Add(arr);
        }

        public void Enqueue(T arr)
        {
            _buffer.Add(arr);
        }
        
        public T Dequeue()
        {
            if (_buffer.Count == 0)
            {
                throw new InvalidOperationException(ErrorMsg.BufferIsEmpty.ToString());
            }

            T unit = _buffer[0];
            _recentOuttedUnit = unit;
            _buffer.RemoveAt(0);

            return unit;

        }
 
        public T Dequeue(out ErrorMsg errmsg)
        {
            if (_buffer.Count == 0){
                errmsg = ErrorMsg.BufferIsEmpty;
                throw new InvalidOperationException(ErrorMsg.BufferIsEmpty.ToString());
                //return null;
            }

            T unit = _buffer[0];
            _recentOuttedUnit = unit;
            _buffer.RemoveAt(0);

            errmsg = ErrorMsg.NoError;
            return unit;
            
        }

        public T Pop()
        {
            if (_buffer.Count == 0)
            {
                throw new InvalidOperationException(ErrorMsg.BufferIsEmpty.ToString());
            }
            T unit = _buffer[_buffer.Count - 1];
            _recentOuttedUnit = unit;
            _buffer.RemoveAt(_buffer.Count - 1);

            return unit;
        }

        public T Pop(out ErrorMsg errmsg)
        {
            if (_buffer.Count == 0)
            {
                errmsg = ErrorMsg.BufferIsEmpty;
                throw new InvalidOperationException(ErrorMsg.BufferIsEmpty.ToString());
                //return null;
            }
            T unit = _buffer[_buffer.Count-1];
            _recentOuttedUnit = unit;
            _buffer.RemoveAt(_buffer.Count-1);

            errmsg = ErrorMsg.NoError;
            return unit;
        }

        /// <summary>
        /// 내부에 저장된 Array들의 모든 ByteSize를 계산한다.
        /// </summary>
        public int Size
        {
            get
            {
                return _buffer.Count;
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
            _recentOuttedUnit = (new T[1])[0];// null;
        }

        public int IndexOf(T item)
        {
            return _buffer.IndexOf(item);
        }

        /// <summary>
        /// index가 -1이면 가장 뒤에 넣는다.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            if (index < 0 || index >= _buffer.Count)
            {
                _buffer.Add(item);
            }
            else
            {
                _buffer.Insert(index, item);
            }
        }

        public void Add(T item)
        {
            _buffer.Add(item);
        }

        public bool Contains(T item)
        {
            return _buffer.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _buffer.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false ; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _buffer.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _buffer.GetEnumerator();
        }
    }
}
