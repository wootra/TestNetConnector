using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WootraComs.wTreeElements
{
    public class wTreeNodeItemCollection : ICollection<wTreeNodeItem>
    {
        List<wTreeNodeItem> _itemList = new List<wTreeNodeItem>();
        internal event wTreeNodeItemChanged E_ItemChanged;
        internal event wTreeNodeItemChanged E_ItemListChanged;
        //internal event wTreeNodeItemMouseEvent E_ItemSelected;

        wTreeNode _parent;
        wTree _ownerTree;
        internal wTreeNodeItemCollection(wTree ownerTree, wTreeNode parent)
        {
            _ownerTree = ownerTree;
            _parent = parent;
        }

        internal void Add(wTreeNodeItem item)
        {
            _itemList.Add(item);
            item.E_ItemChanged += item_E_ItemChanged;
            //item.E_ItemSelected += item_E_ItemSelected;
            if (E_ItemListChanged != null) E_ItemListChanged(item);

            for (int i = 0; i < _itemList.Count; i++)
            {
                _itemList[i]._itemIndex = i;
            }
        }

        

        public wTreeNodeItem this[int index]
        {
            get
            {
                if (index < 0) return null;
                else if (index >= _itemList.Count) return null;
                else return _itemList[index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isChecked"></param>
        /// <param name="checkBoxActiveAction">체크박스가 반응할 Event</param>
        /// <returns></returns>
        public wTreeNodeItem Add(bool? isChecked, CheckboxActiveActions checkBoxActiveAction = CheckboxActiveActions.Click)
        {
            wTreeNodeItem item = new wTreeNodeItem(_parent, isChecked, checkBoxActiveAction);
            Add(item);
            return item;
        }

        public wTreeNodeItem Add(Image image, ImageEditorTypes imageEditor= ImageEditorTypes.None, EditorActivateActions activationAction= EditorActivateActions.UseBasicSetting)
        {
            wTreeNodeItem item = new wTreeNodeItem(_parent, image) { ImageEditorType = imageEditor, EditorActivateAction = activationAction };
            Add(item);
            return item;
        }

        public wTreeNodeItem Add(Image image, int width, int height, ImageEditorTypes imageEditor = ImageEditorTypes.None, EditorActivateActions activationAction = EditorActivateActions.UseBasicSetting)
        {
            wTreeNodeItem item = new wTreeNodeItem(_parent, image,width,height) { ImageEditorType = imageEditor, EditorActivateAction = activationAction };
            Add(item);
            return item;
        }

        public wTreeNodeItem Add(String text, TextEditorTypes textEditor = TextEditorTypes.None, EditorActivateActions activationAction= EditorActivateActions.UseBasicSetting)
        {
            wTreeNodeItem item = new wTreeNodeItem(_parent, text) { TextEditorType = textEditor, EditorActivateAction = activationAction };
            Add(item);
            return item;
        }

        public wTreeNodeItem Add(ICollection<String> text, int selectedIndex=0, TextArrayEditorTypes textEditor = TextArrayEditorTypes.None, EditorActivateActions activationAction = EditorActivateActions.UseBasicSetting)
        {
            wTreeNodeItem item = new wTreeNodeItem(_parent, text, selectedIndex) { TextArrayEditorType = textEditor, EditorActivateAction = activationAction };
            Add(item);
            return item;
        }

        public wTreeNodeItem Add(Control control)
        {
            wTreeNodeItem item = new wTreeNodeItem(_parent, control);
            Add(item);
            return item;
        }

        public wTreeNodeItem Add(WhiteSpaceTypes space)
        {
            wTreeNodeItem item = new wTreeNodeItem(_parent, space);
            Add(item);
            return item;
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

        internal bool Remove(wTreeNodeItem item)
        {
            bool isRemoved = _itemList.Remove(item);
            if (isRemoved)
            {
                item.E_ItemChanged -= item_E_ItemChanged;//기존의 것을 지운다.
                if (E_ItemListChanged != null) E_ItemListChanged(item);
                
                for (int i = 0; i < _itemList.Count; i++)
                {
                    _itemList[i]._itemIndex = i;
                }
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


        void ICollection<wTreeNodeItem>.Add(wTreeNodeItem item)
        {
            this.Add(item);
        }


        bool ICollection<wTreeNodeItem>.Remove(wTreeNodeItem item)
        {
            return this.Remove(item);
        }
    }

}
