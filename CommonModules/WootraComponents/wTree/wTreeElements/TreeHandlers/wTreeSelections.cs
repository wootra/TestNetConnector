using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class wTreeSelections
    {
        wTree _ownerTree;
        public event wTreeNodeItemSelectedEvent E_TreeNodeSelected;
        public event wTreeNodeItemSelectingEvent E_TreeNodeSelecting;
        public event wTreeNodeCheckItemValueChanged E_CheckItemChanged;

        /// <summary>
        /// Checkbox 아이템들이 한꺼번에 바뀌었을 때 Invoke된다. 단, 다중 Depth는 허용하지 않으므로, 값이 바뀐 것을 체크하려면, 
        /// Checkbox에 해당하는 ItemValue가 변했는지를 체크해야 한다.
        /// 이 경우, 받을 이벤트는 wTree.E_TreeNodeChanged이다.
        /// </summary>
        public event wTreeNodeCheckItemsValueChanged E_CheckItemsChanged;

        internal wTreeSelections(wTree ownerTree)
        {
            _ownerTree = ownerTree;
        }

        wTreeMouseEventsHandler MouseEventsHandler { get { return _ownerTree.wMouseEventsHandler; } }

        wTreeScroll ScrollHandler { get { return _ownerTree.wScrollHandler; } }

        
        DrawHandler DrawHandler { get { return _ownerTree.wDrawHandler; } }

        EditorHandlerClass EditorHandler { get { return _ownerTree.wEditorHandler; } }

        Keys ModifierKeys
        {
            get { return Control.ModifierKeys; }
        }
        List<wTreeNode> VisibleNodes { get { return _ownerTree.VisibleNodes; } }
       
        wTreeNode _rollOverNode;
        public wTreeNode RollOverNode
        {
            get { return _rollOverNode; }
            set
            {
                if (_rollOverNode == null || _rollOverNode.Equals(value) == false)
                {
                    wTreeNode old = _rollOverNode;
                    _rollOverNode = value;
                    if (old != null && ((BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)) old.DrawBuffer();
                }

            }
        }
        BasicDrawings BasicDrawing { get { return DrawHandler.BasicDrawing; } }
        int _tempSelectingDragging = -1;

        internal void drawSelectingNodes(int tempSelectingStart, int tempSelecting)
        {
            _tempSelectingStart = tempSelectingStart;
            List<wTreeNode> old = new List<wTreeNode>(SelectedNodes);
            _tempSelectingDragging = tempSelecting;
            List<wTreeNode> newSelected = SelectedNodes;
            foreach (wTreeNode node in old)
            {
                node.DrawBuffer();
                newSelected.Remove(node);
            }

            foreach (wTreeNode node in newSelected)
            {
                node.DrawBuffer();
            }

        }

        wTreeNodeItem _selectedItem;
        public wTreeNodeItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
            }
        }

        wTreeNode _selectedNode = null;
        public wTreeNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (_selectedNode != value)
                {
                    wTreeNode before = _selectedNode;

                    _selectedNode = value;
                    if (_selectedNode == null)
                    {
                        _selectedNode = null;
                    }
                    if (before != null)
                    {
                        before.DrawBuffer();
                    }
                    if (value != null)
                    {
                        _selectedNode.DrawBuffer();
                    }
                }
                if((BasicDrawing & BasicDrawings.Selection)== BasicDrawings.Selection) DrawHandler.ReDrawTree(false);
            }
        }
        /// <summary>
        /// node를 선택하고, item을 선택하면 해당 Action을 한다. 이 함수의 결과로 SelectedItem이 바뀐다.
        /// </summary>
        /// <param name="selectedNode"></param>
        /// <param name="pt"></param>
        internal void SetNodeSelected(wTreeNode selectedNode, wTreeNodeItem item)
        {
            wTreeNodeItemSelectingArgs args = new wTreeNodeItemSelectingArgs();
            if (E_TreeNodeSelecting != null) E_TreeNodeSelecting(SelectedNode, item, args);
            if (args.IsCancel) return;
            //_pressedNode = selectedNode;
            if (selectedNode != null)
            {
                //selectedNode.IsExpaned = !selectedNode.IsExpaned;

                if (item != null)
                {
                    
                    //_pressedItem = item;
                    //MessageBox.Show(item.ItemType.ToString());

                    if (item.ItemType == wTreeNodeItemTypes.PlusMinus)
                    {
                        selectedNode.IsExpanded = !selectedNode.IsExpanded;
                        selectedNode.ReDrawNode();
                        //_ownerTree.DrawHandler.ReDrawTree(true);

                    }
                    

                }
                _selectedItem = item;

                if (E_TreeNodeSelected != null) E_TreeNodeSelected(SelectedNode, item);
            }


            //this.Focus();
        }

        /// <summary>
        /// 체크박스의 값을 toggle한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="itemIndex"></param>
        internal void SetCheckboxIfItemIsCheckbox(wTreeNode node, int itemIndex)
        {
            if (node.Items[itemIndex] != null && node.Items[itemIndex].ItemType == wTreeNodeItemTypes.CheckBox)
            {
                wTreeNodeItem item = node.Items[itemIndex];
                bool? value = (bool?)item.Value;
                if (value == true) item.Value = false;
                else if (value == false) item.Value = true;
                else item.Value = true;//중간상태일 때는 true로 바꿈.
                bool newValue = (bool)item.Value;
                node.ControlChildChecks(itemIndex, newValue);
                node.ControlParentChecks(itemIndex);
                node.DrawBuffer();
                _ownerTree.wDrawHandler.ReDrawTree(false);
                if (E_CheckItemChanged != null) E_CheckItemChanged(node, item, newValue);
            }
        }

        /// <summary>
        /// 체크박스의 값을 toggle한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="itemIndex"></param>
        internal void SetCheckboxIfItemIsCheckbox(IwTreeNodeCollectionParent parentNode, int childStart, int childEnd, int itemIndex)
        {

            int min = childStart < childEnd ? childStart : childEnd;
            int max = childStart < childEnd ? childEnd : childStart;
            Dictionary<wTreeNodeItem, bool> itemValues = new Dictionary<wTreeNodeItem, bool>();
            for (int i = min; i <= max; i++)
            {
                if (i == childStart) continue;
                wTreeNode child = parentNode.Children[i];

                if (child.Items[itemIndex] != null && child.Items[itemIndex].ItemType == wTreeNodeItemTypes.CheckBox)
                {
                    wTreeNodeItem item = child.Items[itemIndex];
                    bool? value = (bool?)item.Value;
                    if (value == true) item.Value = false;
                    else if (value == false) item.Value = true;
                    else item.Value = true;//중간상태일 때는 true로 바꿈.
                    bool newValue = (bool)item.Value;
                    child.ControlChildChecks(itemIndex, newValue);
                    child.DrawBuffer();
                    itemValues.Add(item, newValue);//toggle이니까 무조건 바뀐다.
                }
            }
            parentNode.Children[0].ControlParentChecks(itemIndex);
            _ownerTree.wDrawHandler.ReDrawTree(false);
            if (itemValues.Count > 0)
            {
                if (E_CheckItemChanged != null) E_CheckItemsChanged(parentNode as wTreeNode, itemValues);
            }
        }

        /// <summary>
        /// 특정 값으로 체크박스를 셋팅한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="itemIndex"></param>
        /// <param name="checkBoxValue"></param>
        internal void SetCheckboxValueIfItemIsCheckbox(wTreeNode node, int itemIndex, bool checkBoxValue)
        {
            if (node.Items[itemIndex] != null && node.Items[itemIndex].ItemType == wTreeNodeItemTypes.CheckBox)
            {
                wTreeNodeItem item = node.Items[itemIndex];
                item.Value = checkBoxValue;
                bool newValue = (bool)item.Value;
                node.ControlChildChecks(itemIndex, newValue);
                node.ControlParentChecks(itemIndex);
                node.DrawBuffer();
                _ownerTree.wDrawHandler.ReDrawTree(false);
                if (E_CheckItemChanged != null) E_CheckItemChanged(node, item, newValue);
            }
        }

        /// <summary>
        /// 특정 값으로 체크박스를 셋팅한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="itemIndex"></param>
        /// <param name="checkBoxValue"></param>
        internal void SetCheckboxValueIfItemIsCheckbox(IwTreeNodeCollectionParent parentNode, int childStart, int childEnd, int itemIndex, bool checkBoxValue)
        {
            int min = childStart < childEnd ? childStart : childEnd;
            int max = childStart < childEnd ? childEnd : childStart;
            Dictionary<wTreeNodeItem, bool> itemValues = new Dictionary<wTreeNodeItem, bool>();
            
            for (int i = min; i <= max; i++)
            {
                
                wTreeNode child = parentNode.Children[i];


                if (child.Items[itemIndex] != null && child.Items[itemIndex].ItemType == wTreeNodeItemTypes.CheckBox)
                {
                    wTreeNodeItem item = child.Items[itemIndex];
                    bool? oldValue = (bool?)item.Value;
                    if (checkBoxValue != oldValue)
                    {
                        item.Value = checkBoxValue;
                        bool newValue = (bool)item.Value;
                        child.ControlChildChecks(itemIndex, newValue);
                        child.DrawBuffer();
                        if (oldValue != newValue) itemValues.Add(item, newValue);//값이 바뀔 때만 추가한다.
                    }
                }
            }
            parentNode.Children[0].ControlParentChecks(itemIndex);
            _ownerTree.wDrawHandler.ReDrawTree(false);
            if (itemValues.Count > 0)
            {
                if (E_CheckItemChanged != null) E_CheckItemsChanged(parentNode as wTreeNode, itemValues);
            }
        }

/*
        wTreeNode _pressedNode {
            get { return _ownerTree.MouseEventsHandler.PressedNode; }
           
        }
        wTreeNodeItem _pressedItem { get { return _ownerTree.MouseEventsHandler.PressedItem; } }
 */
        
        List<wTreeNode> _beforeSelectedNodes = new List<wTreeNode>();
        /// <summary>
        /// ClearAllSelections가 호출될 때 이전에 저장되었던 Selection들을 저장한다.
        /// </summary>
        public List<wTreeNode> BeforeSelectedNodes
        {
            get { return _beforeSelectedNodes; }
        }

        wTreeNode _beforeSelectedNode = null;
        /// <summary>
        /// ClearAllSelections가 호출될 때 이전에 저장되었던 SelectedNode를 저장한다.
        /// </summary>
        public wTreeNode BeforeSelectedNode
        {
            get { return _beforeSelectedNode; }
        }

        /// <summary>
        /// MultiSelection모드에서 Selected된 모든 Nodes의 선택표시를 없앰..
        /// </summary>
        internal void ClearAllSelections()
        {
            List<wTreeNode> temp = new List<wTreeNode>(SelectedNodes);
            _beforeSelectedNodes = temp;
            _selectedNodes.Clear();
            foreach (wTreeNode node in temp)
            {
                node.DrawBuffer();
            }
            if (SelectedNode != null)
            {
                _beforeSelectedNodes.Add(SelectedNode);
                _beforeSelectedNode = SelectedNode;
            }
            SelectedNode = null;
        }
        internal List<wTreeNode> _selectedNodes = new List<wTreeNode>();
        internal int _tempSelectingStart = -1;
        /// <summary>
        /// 일시적으로 보일 Node - Read-Only.
        /// 갱신하기 위해서는 InsertSelected(node)함수를 이용하라.
        /// </summary>
        internal List<wTreeNode> SelectedNodes
        {
            get
            {
                List<wTreeNode> temp = new List<wTreeNode>(_selectedNodes);//기존 선택부분..

                if (_tempSelectingStart>=0 && _tempSelectingDragging >= 0)//동적 선택부분..
                {
                    int draggingStart = _tempSelectingStart;// VisibleNodes.IndexOf(_pressedNode);
                    int draggingEnd = _tempSelectingDragging;
                    if (draggingStart >= 0 && draggingEnd >= 0)
                    {
                        if (draggingStart < draggingEnd)
                        {
                            for (int i = draggingStart; i <= draggingEnd; i++)
                            {
                                if (temp.Contains(VisibleNodes[i]) == false) temp.Add(VisibleNodes[i]);
                                else if ((ModifierKeys & Keys.Control) == Keys.Control) temp.Remove(VisibleNodes[i]);
                            }
                        }
                        else
                        {
                            for (int i = draggingEnd; i <= draggingStart; i++)
                            {
                                if (temp.Contains(VisibleNodes[i]) == false) temp.Add(VisibleNodes[i]);
                                else if ((ModifierKeys & Keys.Control) == Keys.Control) temp.Remove(VisibleNodes[i]);
                            }
                        }
                    }
                }
                return temp;
            }
        }

        internal void InsertSelected(wTreeNode node)
        {
            if (_selectedNodes.Contains(node) == false) _selectedNodes.Add(node);
        }

        internal void RemoveSelected(wTreeNode node)
        {
            if (_selectedNodes.Contains(node))
            {
                _selectedNodes.Remove(node);
            }
        }


        internal void FixSelections()
        {
            List<wTreeNode> selected = new List<wTreeNode>(SelectedNodes);

            if (selected.Count > 0)
            {
                _selectedNodes = selected;
                _tempSelectingDragging = -1;
                _tempSelectingStart = -1;
            }
        }

        internal void ReleaseTempSelection()
        {
            _tempSelectingStart = -1;
            _tempSelectingDragging = -1;

        }

        /// <summary>
        /// 현재 단일 선택된 Node를 복수선택 리스트로 옮김..
        /// </summary>
        internal void AddSelectedToSelectionNodes()
        {
            if (_selectedNodes.Count == 0)
            {
                if (_selectedNode != null)
                {
                    _selectedNodes.Add(_selectedNode);
                }
            }
        }
    }


}
