using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class wTreeEditor
    {
        Control _editorControl;
        wTree _ownerTree;
        public event EditorValueChanged E_EditorValueChanged;
        public event wTreeNodeItemValueChangeCanceled E_EditorValueChangeCanceled;
        public event EditorValueChanging E_EditorValueChanging;

        internal Control EditorControl
        {
            get { return _editorControl; }
        }

        public wTreeEditor(Control editorControl, wTree ownerTree)
        {
            _editorControl=editorControl;
            _ownerTree = ownerTree;
            //_ownerTree.EditorHandler.E_TreeNodeItemValueChanged += EditorHandler_E_TreeNodeItemValueChanged;
            //_ownerTree.EditorHandler.E_TreeNodeItemValueChangeCanceled += EditorHandler_E_TreeNodeItemValueChangeCanceled;
        }

        void _editorControl_VisibleChanged(object sender, EventArgs e)
        {
            
        }
        public wTreeMouseEventsHandler MouseEventsHandler { get { return _ownerTree.wMouseEventsHandler; } }

        public wTreeScroll ScrollHandler { get { return _ownerTree.wScrollHandler; } }

        public wTreeSelections SelectionHandler { get { return _ownerTree.wSelectionHandler; } }

        public DrawHandler DrawHandler { get { return _ownerTree.wDrawHandler; } }

        public EditorHandlerClass EditorHandler { get { return _ownerTree.wEditorHandler; } }

        public wTree OwnerTree
        {
            get { return _ownerTree; }
        }
        /*
        protected virtual void EditorHandler_E_TreeNodeItemValueChangeCanceled(wTreeNode node, wTreeNodeItem item)
        {
            EditorHandler.OnNodeItemValueChangeCanceled(node, item);
        }

        protected virtual void EditorHandler_E_TreeNodeItemValueChanged(wTreeNode node, wTreeNodeItem item, object oldValue, object newValue)
        {
            EditorHandler.OnNodeItemValueChanged(node, item, oldValue, newValue);
        }
        */
        wTreeNodeItem _itemToEdit;
        /// <summary>
        /// Editing중인 item..
        /// </summary>
        public wTreeNodeItem ItemToEdit { get { return _itemToEdit; } }

        /// <summary>
        /// Editor를 시작할 때 할 작업..
        /// </summary>
        /// <param name="item"></param>
        public virtual void ShowEditorFor(wTreeNodeItem item, Rectangle area)
        {
            _itemToEdit = item;
            
            _ownerTree.wEditorHandler.ShowEditor(this, area);

        }

        internal bool _isValueChanged = false;
        /// <summary>
        /// 상속받은 에디터는 Value 를 수정한 뒤에는 이 함수를 호출해야 한다.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        internal void OnEditorValueChanged(object oldValue, object newValue, bool callOnce=false){
            if (_isValueChanged) return;
            if (callOnce) _isValueChanged = true;
            if (oldValue.Equals(newValue)) return;//editor value is not changed...

            wEditorValueChangingArgs args = new wEditorValueChangingArgs(oldValue,newValue);

            if (E_EditorValueChanging != null) E_EditorValueChanging(_itemToEdit, args);
            if (args.IsCanceled)
            {
                if (E_EditorValueChangeCanceled != null) E_EditorValueChangeCanceled(ItemToEdit.OwnerNode, ItemToEdit);
            }
            else
            {
                newValue = args.NewValue;

                if (oldValue.Equals(newValue) == false)
                {
                    _itemToEdit.SetValue(newValue);
                    _itemToEdit.OwnerNode.DrawBuffer();
                    DrawHandler.ReDrawTree(false);
                    if (E_EditorValueChanged != null) E_EditorValueChanged(ItemToEdit, oldValue, newValue);
                }
                else
                {
                    if (E_EditorValueChangeCanceled != null) E_EditorValueChangeCanceled(ItemToEdit.OwnerNode, ItemToEdit);
                }
            }
        }

        /// <summary>
        /// 상속받은 에디터는 Value가 수정되지 않은 경우 이 함수를 호출해야 한다.
        /// </summary>
        internal void OnValueChangeCanceled()
        {
            if (E_EditorValueChangeCanceled != null) E_EditorValueChangeCanceled(ItemToEdit.OwnerNode, ItemToEdit);
        }

        public void HideEditor()
        {
            EditorHandler.HideEditor();
        }
        /// <summary>
        /// Node의 위치까지 고려하여 절대 좌표를 가져온다.
        /// </summary>
        /// <returns></returns>
        public Point EditorPosition
        {
            get
            {
                if (ItemToEdit != null)
                {
                    int toY = getCenter(EditorControl.Height, ItemToEdit.ItemArea.Height) + ItemToEdit.ItemArea.Y;
                    Point pt = new Point(ItemToEdit.OwnerNode.Location.X + ItemToEdit.ItemArea.X, ItemToEdit.OwnerNode.Location.Y + toY);

                    return pt;
                }
                else return Point.Empty;
            }
        }
        /// <summary>
        /// baseSize 안에서 itemSize를 가진 item이 중앙이 되려면 어느 위치에서 시작해야 하는지..
        /// </summary>
        /// <param name="itemSize"></param>
        /// <param name="baseSize"></param>
        /// <returns></returns>
        private int getCenter(int itemSize, int baseSize)
        {
            return (baseSize - itemSize) / 2;
        }

        /// <summary>
        /// 에디터의 값을 바꾸기 위해 이 함수를 이용한다. 상속받아야 한다.
        /// </summary>
        /// <param name="p"></param>
        public virtual void SetValue(object p)
        {
            
        }
    }

    
}
