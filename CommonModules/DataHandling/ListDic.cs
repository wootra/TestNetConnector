using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandling
{
    /// <summary>
    /// 순서가 변하지 않는 Dictionary이다.Key와 Value가 각각 같은 index 순서를 가진 리스트로 구성되어 있다.
    /// </summary>
    /// <typeparam name="U">Key</typeparam>
    /// <typeparam name="T">Value</typeparam>
    public class ListDic<U, T> : IDictionary<U, T>
    {
        Dictionary<U, int> _hashKey = new Dictionary<U, int>();
        ListDicListCollection<U> _keys = new ListDicListCollection<U>();
        ListDicListCollection<T> _values = new ListDicListCollection<T>();
        
        public delegate void ItemAddEvent(U key, T value);
        public delegate void ItemRemoveEvent(U key, T value);

        public event ItemAddEvent E_ItemAdded;
        public event ItemRemoveEvent E_ItemRemoved;

        public ListDic()
        {
        }

        #region IDictionary<U,T> 멤버

        public void Add(U key, T value)
        {
            if (_hashKey.ContainsKey(key) == false)
            {
                _hashKey[key] = _keys.Count;
                _keys.AddInter(key);
                _values.AddInter(value);
                if (E_ItemAdded != null) E_ItemAdded(key, value);
            }
            else
            {
                throw new InvalidOperationException("이미 지정한 키가 존재합니다.");
            }
        }

        public bool ContainsKey(U key)
        {
            return _hashKey.ContainsKey(key);
            //return _keys.Contains(key);
        }

        public ICollection<U> Keys
        {
            get { return _keys; }
        }

        public ListDicListCollection<U> KeyList
        {
            get { return _keys; }
        }


        public bool Remove(U key)
        {
            
            if (_hashKey.ContainsKey(key))// index >= 0)
            {
                int index = _hashKey[key];// _keys.IndexOf(key);
                T itemToBeRemoved = _values[index];
                _keys.RemoveAt(index);
                _values.RemoveAt(index);
                reloadHashKey();
                if (E_ItemRemoved != null) E_ItemRemoved(key, itemToBeRemoved);
                return true;
            }
            else return false;
        }

        public bool TryGetValue(U key, out T value)
        {
            
            if (_hashKey.ContainsKey(key)==false)
            {
                value = (new T[1])[0];
                return false;
            }
            else
            {
                int index = _hashKey[key];// _keys.IndexOf(key);
                value = _values[index];
                return true;
            }
        }

        public ICollection<T> Values
        {
            get { return _values; }
        }


        public ListDicListCollection<T> ValueList
        {
            get { return _values; }
        }

        public T this[U key]
        {
            get
            {
                
                if (_hashKey.ContainsKey(key)==false) throw new ArgumentOutOfRangeException("해당 key" + key.ToString() + "이 없습니다.");
                else
                {
                    int index = _hashKey[key];// _keys.IndexOf(key);
                    return _values[index];
                }
            }
            set
            {
                
                if (_hashKey.ContainsKey(key)==false)
                {
                    _hashKey.Add(key, _keys.Count);
                    _keys.AddInter(key);
                    _values.AddInter(value);
                    if (E_ItemAdded != null) E_ItemAdded(key, value);
                }
                else
                {
                    int index = _hashKey[key];// _keys.IndexOf(key);
                    _keys[index] = key;
                    _values[index] = value;
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<U,T>> 멤버

        public void Add(KeyValuePair<U, T> item)
        {
            Add(item.Key, item.Value);

        }

        public void Clear()
        {
            _keys.ClearInter();
            _values.ClearInter();
            _hashKey.Clear();
        }

        public bool Contains(KeyValuePair<U, T> item)
        {
            if (_hashKey.ContainsKey(item.Key))
            {
                int index = _hashKey[item.Key];
                if (_values[index].Equals(item.Value)) return true;
            }
            return false;

        }

        /// <summary>
        /// array에 arrayIndex부터 채워넣는다.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<U, T>[] array, int arrayIndex)
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                array[i + arrayIndex] = new KeyValuePair<U, T>(_keys[i], _values[i]);
                
            }
        }

        public int Count
        {
            get { return _keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<U, T> item)
        {
            
            if (_hashKey.ContainsKey(item.Key))
            {
                int index = _hashKey[item.Key];// _keys.IndexOf(item.Key);
                _keys.RemoveAt(index);
                _values.RemoveAt(index);
                reloadHashKey();
                return true;
            }
            else return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<U,T>> 멤버

        public IEnumerator<KeyValuePair<U, T>> GetEnumerator()
        {
            List<KeyValuePair<U, T>> list = new List<KeyValuePair<U, T>>();
            for (int i = 0; i < _keys.Count; i++)
            {
                list.Add(new KeyValuePair<U, T>(_keys[i], _values[i]));
            }
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable 멤버

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            List<KeyValuePair<U, T>> list = new List<KeyValuePair<U, T>>();
            for (int i = 0; i < _keys.Count; i++)
            {
                list.Add(new KeyValuePair<U, T>(_keys[i], _values[i]));
            }
            return list.GetEnumerator();
        }

        #endregion

        public void RemoveAt(int p)
        {
            U key = _keys[p];
            T value = _values[p];
            _keys.RemoveAt(p);
            _values.RemoveAt(p);
            reloadHashKey();
            if (E_ItemRemoved != null) E_ItemRemoved(key,value);
        }

        public void Insert(U key, T value, int index)
        {
            if (index == -1)
            {
                Add(key, value);
            }
            else
            {
                _keys.Insert(key, index);
                _values.Insert(value, index);
                reloadHashKey();
                if (E_ItemAdded != null) E_ItemAdded(key,value);
            }

        }

        private void reloadHashKey()
        {
            _hashKey.Clear();
            for (int i = 0; i < _keys.Count; i++)
            {
                _hashKey[_keys[i]] = i;
            }
        }
    }

    public class ListDicListCollection<T> : ICollection<T>
    {
        #region ICollection<T> 멤버
        List<T> _list = new List<T>();
        internal void AddInter(T item)
        {
            _list.Add(item);
        }
        public void Add(T item)
        {
            throw new Exception("ReadOnly이므로 추가할 수 없습니다.");
            //_list.Add(item);
        }

        public T this[int index]
        {
             get
            {
                try
                {
                    return _list[index];
                }
                catch
                {
                    throw;
                }
            }
            internal set
            {
                _list[index] = value;
            }
        }

        

        /// <summary>
        /// 이 함수는 원래 바꿀 수 없는 값을 바꾸기위해 존재한다.
        /// 위험하다.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void ChangeValue(int index, T value)
        {
            _list[index] = value;
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        internal void ClearInter()
        {
            _list.Clear();
        }
        public void Clear()
        {
            throw new Exception("ReadOnly이므로 Clear할 수 없습니다.");
            
            //_list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex) ;
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        internal bool RemoveInter(T item)
        {
            return _list.Remove(item);
        }
        public bool Remove(T item)
        {
            throw new Exception("ReadOnly이므로 삭제할 수 없습니다.");
            
            //return false;// _list.Remove(item);
        }

        internal void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
        #endregion

        #region IEnumerable<T> 멤버

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable 멤버

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        internal void Insert(T item, int index)
        {
            if (index < 0) _list.Add(item);
            else _list.Insert(index, item);
        }

        internal void InsertRange(ICollection<T> items, int index)
        {
            if (index < 0) _list.AddRange(items);
            else _list.InsertRange(index, items);
        }
    }
}
