using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WootraComs.wTreeElements
{
    public class wTreeNode: IwTreeNodeCollectionParent
    {
        internal event wTreeNodeChangedEventHandler E_TreeNodeChanged;
        internal event wTreeListChangedEventHandler E_TreeListChanged;

        internal wTreeNodeCollection _children = null;//부모 collection 을 가진다.

        public wTreeNode()
        {
            _children = new wTreeNodeCollection(this);
            _children.E_TreeListChanged += TreeListChanged;
            _children.E_TreeNodeChanged += TreeNodeChanged;
            _items = new wTreeNodeItemCollection(this);
            _items.E_ItemChanged += _items_E_ItemChanged;
            _items.E_ItemListChanged += _items_E_ItemListChanged;
        }
        
        bool _isExpanded = false;
        public bool IsExpaned
        {
            get{ return _isExpanded;}
        }

        int _height=0;
        public int Height{
            get{ return _height;}
        }

        public bool Visible{
            get{ return _height==0 ? false : true;}
        }

        internal void SetExpaned(bool isExpaned, int height){
            _isExpanded = isExpaned;
            _height = height;
        }

        public void SetExpand(bool isExpand)
        {
            _isExpanded = isExpand;
        }

        void _items_E_ItemListChanged(wTreeNodeItem item)
        {
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(this, -1);
        }

        /// <summary>
        /// icollection에 추가되면 셋팅된다.
        /// </summary>
        internal IwTreeNodeCollectionParent _parent=null;

        void _items_E_ItemChanged(wTreeNodeItem item)
        {
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(this, _items.IndexOf(item));
        }

        #region eventHandlers
        void TreeListChanged(wTreeNode node)
        {
            if (E_TreeListChanged != null) E_TreeListChanged(node);
        }

        void TreeNodeChanged(wTreeNode node, int index)
        {
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(node, index);
        }
        #endregion

        public wTreeNodeCollection Children
        {
            get { return _children; }
        }

        bool _isChanged = false;//처음에 한 번 그리기 위해서.. 이후부터는 변경될 때마다 그리는 것을 대기한다.
        /// <summary>
        /// 내부의 내용이 변하면 true가 된다.
        /// </summary>
        public bool IsChanged { get { return _isChanged; } }

        bool _isReadyToDraw = true;
        /// <summary>
        /// 숨겨졌다가 다시 나타나서 보여져야 하는지 나타낸다.
        /// </summary>
        internal bool IsReadyToDraw
        {
            get { return _isReadyToDraw; }
        }

        /// <summary>
        /// 방금 그려졌다면 호출해야 한다. 내부적으로 isReadyToDraw를 false로 만든다.
        /// </summary>
        internal void SetDrawn()
        {
            _isReadyToDraw = false;
        }

        bool _isVisible = false;//현재 보이고 있는지
        public bool IsVisibled { get { return _isVisible; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isVisible"></param>
        internal void SetVisible(bool isVisible)
        {
            if (_isVisible == false && isVisible) _isReadyToDraw = true;
            _isVisible = isVisible;
        }

        wTreeNodeItemCollection _items;

        public wTreeNodeItemCollection Items
        {
            get { return _items; }
        }

        /// <summary>
        /// -1이면 아무곳에도 소속되지 않았다는 것을 나타냄..
        /// </summary>
        public int Depth
        {
            get {
                if (_parent == null) return -1;
                else return _parent.Depth + 1;
            }
        }

        /// <summary>
        /// treeNode의 부모. wTree가 될 수도 있고, wTreeNode가 될 수도 있다.
        /// </summary>
        public IwTreeNodeCollectionParent TreeParent
        {
            get { return _parent; }
        }
    }
}
