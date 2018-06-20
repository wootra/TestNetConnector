using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    /// <summary>
    /// treeNode의 집합.
    /// </summary>
    public class wTreeNodeCollection : ICollection<wTreeNode>
    {
        List<wTreeNode> _list = new List<wTreeNode>();

        internal wTreeNodeChangedEventHandler E_TreeNodeChanged;
        internal wTreeListChangedEventHandler E_TreeListChanged;

        IwTreeNodeCollectionParent _parent;
        internal wTreeNodeCollection(IwTreeNodeCollectionParent parent)
        {
            _parent = parent;
        }

        public void Add(wTreeNode node)
        {
            whenAddNode(node);
            _list.Add(node);
            if (E_TreeListChanged != null) E_TreeListChanged(node);
        }

        public void Add(String text)
        {
            wTreeNode node = new wTreeNode();
            node.Items.Add(new wTreeNodeItem(text));
            Add(node);
        }

        public void Add(bool isChecked, String text)
        {
            wTreeNode node = new wTreeNode();
            node.Items.Add(new wTreeNodeItem(isChecked));
            node.Items.Add(new wTreeNodeItem(text));
            Add(node);
        }

        public void Add(Image image, String text)
        {
            wTreeNode node = new wTreeNode();
            node.Items.Add(new wTreeNodeItem(image));
            node.Items.Add(new wTreeNodeItem(text));
            Add(node);
        }

        public void Add(bool isChecked, Image image, String text)
        {
            wTreeNode node = new wTreeNode();
            node.Items.Add(new wTreeNodeItem(isChecked));
            node.Items.Add(new wTreeNodeItem(image));
            node.Items.Add(new wTreeNodeItem(text));
            Add(node);
        }


        void item_E_TreeNodeChanged(wTreeNode treeNode, int itemIndex)
        {
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(treeNode, itemIndex);
        }

        void item_E_TreeListChanged(wTreeNode treeNode)
        {
            if (E_TreeListChanged != null) E_TreeListChanged(treeNode);
        }

        public wTreeNode this[int index]
        {
            get
            {
                if (index < _list.Count && index >= 0) return _list[index];
                else return null;
            }
            set
            {
                if (index < _list.Count && index >= 0)
                {
                    if (_list.Contains(value) == false || _list[index].Equals(value)==false) //이미 있다면 다시 넣어줄 필요가 없다.
                    {
                        whenAddNode(value);
                        whenReleaseNode(_list[index]);
                        _list[index] = value;
                        if (E_TreeListChanged!=null) E_TreeListChanged(value);
                    }
                }
                else throw new Exception("wTree error: wTree[" + index + "] doesn't exist..");
            }
        }

        /// <summary>
        /// node를 이 컬렉션에 추가할 때 처리해 줄 목록
        /// </summary>
        /// <param name="node"></param>
        void whenAddNode(wTreeNode node)
        {
            node.E_TreeNodeChanged += item_E_TreeNodeChanged;//새 아이템에 연결을 추가한다.
            node.E_TreeListChanged += item_E_TreeListChanged;
            node._parent = _parent;
        }

        /// <summary>
        /// node를 이 컬렉션에서 지울때 처리해 줄 목록
        /// </summary>
        /// <param name="node"></param>
        void whenReleaseNode(wTreeNode node)
        {
            node.E_TreeNodeChanged -= item_E_TreeNodeChanged;//기존에 있었던 이벤트 연결을 끊는다.
            node.E_TreeListChanged -= item_E_TreeListChanged;
            if (node._parent.Equals(_parent)) node._parent = null;
        }


        public void Clear()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                whenReleaseNode(_list[i]);
            }
            _list.Clear();
            if (E_TreeListChanged != null) E_TreeListChanged(null);
        }

        public bool Contains(wTreeNode item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// do not use this..
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(wTreeNode[] array, int arrayIndex)
        {

        }

        public int IndexOf(wTreeNode node)
        {
            return _list.IndexOf(node);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(wTreeNode item)
        {
            bool isRemoved = _list.Remove(item);
            if(isRemoved){
                whenReleaseNode(item);
                if (E_TreeListChanged != null) E_TreeListChanged(item);
            }
            return isRemoved;
        }

        public IEnumerator<wTreeNode> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
