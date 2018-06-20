using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandling
{
    public class CustomDictionary<T,V> : IDictionary<T,V> where V :class
    {
        V _nullValue;
        public CustomDictionary(V nullValue=null){
            _nullValue = nullValue;
        }
        Dictionary<T, V> _collection = new Dictionary<T, V>();

        public V this[T key]
        {
            get
            {
                if (_collection.ContainsKey(key)) return _collection[key];
                else return null;
            }
            set
            {
                _collection[key] = value;
            }
        }


        public void Add(T key, V value)
        {
            _collection.Add(key, value);
        }

        public bool ContainsKey(T key)
        {
            return _collection.ContainsKey(key);
        }

        public ICollection<T> Keys
        {
            get
            {
                return _collection.Keys;
            }
        }

        public bool Remove(T key)
        {
            return _collection.Remove(key);
        }

        public bool TryGetValue(T key, out V value)
        {
            return _collection.TryGetValue(key, out value);
        }

        public ICollection<V> Values
        {
            get { return _collection.Values; }
        }

        public void Add(KeyValuePair<T, V> item)
        {
            _collection.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(KeyValuePair<T, V> item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
        {
            for (int i = 0; i < _collection.Count; i++)
            {
                array[arrayIndex + i] = _collection.ElementAt(i);
            }
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<T, V> item)
        {
            return _collection.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
