using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WootraComs.wTreeElements
{
    public class wTreeNodeItemCollection : ICollection<wTreeNodeItem>
    {
        List<wTreeNodeItem> _itemList = new List<wTreeNodeItem>();
        internal event wTreeNodeItemChanged E_ItemChanged;
        internal event wTreeNodeItemChanged E_ItemListChanged;

        wTreeNode _parent;
        internal wTreeNodeItemCollection(wTreeNode parent)
        {
            _parent = parent;
        }

        public void Add(wTreeNodeItem item)
        {
            _itemList.Add(item);
            item.E_ItemChanged += item_E_ItemChanged;
            if (E_ItemListChanged != null) E_ItemListChanged(item);
        }

        void item_E_ItemChanged(wTreeNodeItem item)
        {
            if (E_ItemChanged != null) E_ItemChanged(item);
        }

        public void Clear()
        {
            foreach (wTreeNodeItem item in _itemList)
            {
                item.E_ItemChanged -= item_E_ItemChanged;//기존의 것을 지운다.
            }
            _itemList.Clear();
            if (E_ItemListChanged != null) E_ItemListChanged(null);
        }

        public bool Contains(wTreeNodeItem item)
        {
            return _itemList.Contains(item);
        }

        /// <summary>
        /// do not use this...
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(wTreeNodeItem[] array, int arrayIndex)
        {

        }

        public int IndexOf(wTreeNodeItem item)
        {
            return _itemList.IndexOf(item);
        }

        public int Count
        {
            get { return _itemList.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(wTreeNodeItem item)
        {
            bool isRemoved = _itemList.Remove(item);
            if (isRemoved)
            {
                item.E_ItemChanged -= item_E_ItemChanged;//기존의 것을 지운다.
                if (E_ItemListChanged != null) E_ItemListChanged(item);
            }
            return isRemoved;
        }

        public IEnumerator<wTreeNodeItem> GetEnumerator()
        {
            return _itemList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _itemList.GetEnumerator();
        }
    }

}
