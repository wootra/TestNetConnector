using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace WootraComs.wTreeElements
{
    public class wTreeMouseEventsHandler
    {
        wTree _ownerTree;
        wTreeNode _pressedNode;//pressed node
        wTreeNodeItem _pressedItem;//pressed nodeitem
        /// <summary>
        /// shift키가 눌러져있는지...
        /// </summary>
        bool _isShiftDown = false;

        wTreeNode _lastMouseUpNode;
        wTreeNodeItem _lastMouseUpItem;


        public event wTreeNodeItemMouseEvent E_TreeNodeMouseDown;

        public event wTreeNodeItemMouseEvent E_TreeNodeDoubleClicked;
        public event wTreeNodeItemMouseMoveEvent E_TreeNodeMouseMove;
        public event wTreeNodeItemMouseMoveEvent E_TreeNodeMouseDragging;
        public event wTreeNodeItemMouseEvent E_TreeNodeMouseUp;
        public event wTreeNodeItemMouseEvent E_TreeNodeDraggedOut;

        /// <summary>
        /// 노드가 클릭되면 node와 item이 null이 아니다. node가 아닌 영역이 클릭되면 argument가 null이 된다.
        /// </summary>
        public event wTreeNodeItemMouseEvent E_TreeNodeClicked;


        internal wTreeMouseEventsHandler(wTree ownerTree)
        {
            _ownerTree = ownerTree;
            _ownerTree.MouseDown += wTree_MouseDown;
            _ownerTree.MouseUp += wTree_MouseUp;
            _ownerTree.MouseDoubleClick += wTree_MouseDoubleClick;
            _ownerTree.MouseMove += wTree_MouseMove;
            _ownerTree.MouseLeave += wTree_MouseLeave;
            _ownerTree.MouseClick += _ownerTree_MouseClick;
            _ownerTree.KeyUp += _ownerTree_KeyUp;
            IsDragAndDropEnabled = false;//default
        }

        void _ownerTree_KeyUp(object sender, KeyEventArgs e)
        {
            _isShiftDown = false;
        }

        /// <summary>
        /// Drag&Drop을 가능하게 할 것인지..
        /// </summary>
        public bool IsDragAndDropEnabled { get; set; }

        public wTreeScroll ScrollHandler { get { return _ownerTree.wScrollHandler; } }

        public wTreeSelections SelectionHandler { get { return _ownerTree.wSelectionHandler; } }

        public DrawHandler DrawHandler { get { return _ownerTree.wDrawHandler; } }

        public EditorHandlerClass EditorHandler { get { return _ownerTree.wEditorHandler; } }

        Padding Margin
        {
            get { return _ownerTree.Margin; }

        }


        public wTreeNode RollOverNode
        {
            get { return SelectionHandler.RollOverNode; }
            set
            {
                SelectionHandler.RollOverNode = value;
            }
        }

        List<wTreeNode> VisibleNodes { get { return _ownerTree.VisibleNodes; } }


        void wTree_MouseLeave(object sender, EventArgs e)
        {
            RollOverNode = null;
            if ((BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)
            {
                DrawHandler.ReDrawTree(false);
            }
        }



        BasicDrawings BasicDrawing { get { return DrawHandler.BasicDrawing; } }

        /// <summary>
        /// 마우스커서가 움직일 때..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wTree_MouseMove(object sender, MouseEventArgs e)
        {
            
            bool isRollOver = (DrawHandler.BasicDrawing & BasicDrawings.RollOver) != BasicDrawings.None;
            bool isMultipleSelection //drag&drop 가능시 drag로 multiple selection은 활성화되면 안된다.
                = IsDragAndDropEnabled==false && (DrawHandler.BasicDrawing & BasicDrawings.MultiSelections) != BasicDrawings.None;


            if (isMultipleSelection ||
                E_TreeNodeMouseDragging!=null
                )
            {

                if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
                {
                    wTreeNode hoveredNode = null;
                    Point pt;
                    //= new Point(e.X - Margin.Left, e.Y - Margin.Top);
                    hoveredNode = GetNodeFromPoint(e.Location, out pt);// _pressedNode;
                    wTreeNodeItem item = (hoveredNode == null)? null : hoveredNode.GetItemFromPoint(pt);

                    if (_pressedNode != null)
                    {
                        wTreeNodeItemMouseArgs args = new wTreeNodeItemMouseArgs(new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta));
                        bool isNewArea = (RollOverNode == null || RollOverNode.Equals(hoveredNode) == false);
                        if (E_TreeNodeMouseDragging != null) E_TreeNodeMouseDragging(hoveredNode, item, isNewArea, args);
                        if (args.IsCanceled) return;


                        if (hoveredNode != null && isNewArea)
                        {
                            

                            int tempSelecting = VisibleNodes.IndexOf(hoveredNode);
                            int tempSelectingStart = VisibleNodes.IndexOf(_pressedNode);
                            SelectionHandler.drawSelectingNodes(tempSelectingStart, tempSelecting);
                            wTreeNode oldRollOver = RollOverNode;
                            RollOverNode = hoveredNode;
                            if ((BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)
                            {
                                hoveredNode.DrawBuffer();
                            }


                            DrawHandler.ReDrawTree(false);
                            //if(oldRollOver!=null) ReDrawNode(oldRollOver);
                            //if(RollOverNode!=null) ReDrawNode(RollOverNode);
                        }
                    }
                }
            }
            else if (E_TreeNodeMouseMove != null ||
                isRollOver
                )//이벤트가 구현되었을 때만 들어가도록..
            {
                wTreeNode hoveredNode = null;
                Point pt;
                //= new Point(e.X - Margin.Left, e.Y - Margin.Top);
                hoveredNode = GetNodeFromPoint(e.Location, out pt);// _pressedNode;

                //if (hoveredNode == null) return;

                wTreeNodeItem item = (hoveredNode == null) ? null : hoveredNode.GetItemFromPoint(pt);

                wTreeNodeItemMouseArgs args = new wTreeNodeItemMouseArgs(new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta));
                bool isNewArea = (RollOverNode == null || RollOverNode.Equals(hoveredNode) == false);
                if (E_TreeNodeMouseMove != null) E_TreeNodeMouseMove(SelectionHandler.SelectedNode, item, isNewArea, args);
                if (args.IsCanceled) return;

                if (hoveredNode != null && isNewArea)
                {//selectedNode가 null이 아니고, 이전 _rollOverNode가 아닐 때, 혹은 처음 rollOver되었을 때..
                    //selectedNode.IsExpaned = !selectedNode.IsExpaned;
                        
                   

                    wTreeNode oldRollOver = RollOverNode;
                    RollOverNode = hoveredNode;

                    //SelectedNode = selectedNode;
                    if ((BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)
                        hoveredNode.DrawBuffer();

                    //wTreeNodeItem item = hoveredNode.GetItemFromPoint(pt);
                   
                    if ((BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver) DrawHandler.ReDrawTree(false);
                    //if (oldRollOver != null) ReDrawNode(oldRollOver);
                    //if (RollOverNode != null) ReDrawNode(RollOverNode);
                }
            }
            else if (_pressedNode!=null && IsDragAndDropEnabled)
            {
                if(E_TreeNodeDraggedOut!=null){
                   
                    E_TreeNodeDraggedOut(_pressedNode, null, null);
                }
                _pressedNode = null;
                _pressedItem = null;
            }
        }

        Keys ModifierKeys { get { return Control.ModifierKeys; } }
        wTreeNode SelectedNode
        {
            get
            { return SelectionHandler.SelectedNode; }
            set
            {
                SelectionHandler.SelectedNode = value;
            }

        }
        /// <summary>
        /// 마우스키를 뗐을 때..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (_doubleClicked)
            {
                _doubleClicked = false;
                return;
            }
            
            
            Point pt;
            //= new Point(e.X - Margin.Left, e.Y - Margin.Top);
            wTreeNode mouseUpNode = GetNodeFromPoint(e.Location, out pt);// _pressedNode;

            //Point pt;
            //wTreeNode clickedNode = GetNodeFromPoint(e.Location, out pt);// _pressedNode;
            //wTreeNodeItem item = clickedNode.GetItemFromPoint(pt);
            //wTreeNodeItemMouseArgs args = new wTreeNodeItemMouseArgs(e);
            //if (E_TreeNodeDoubleClicked != null) E_TreeNodeDoubleClicked(clickedNode, item, args);

            wTreeNodeItem item = (mouseUpNode==null)? null : mouseUpNode.GetItemFromPoint(pt);
            
            wTreeNodeItemMouseArgs args = new wTreeNodeItemMouseArgs(new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta));
            
            
            if (E_TreeNodeMouseUp != null) E_TreeNodeMouseUp(mouseUpNode, mouseUpNode.GetItemFromPoint(pt), args);
            if (args.IsCanceled) return;

            SelectionHandler.FixSelections();

            if (_pressedNode != null && mouseUpNode != null)
            {
                bool isDragging;
                if (_pressedNode != mouseUpNode) isDragging = true;
                else isDragging = false;
                if (EditorHandler.ActivatedEditor != null && EditorHandler.ActivatedEditor.ItemToEdit != item)
                {
                     EditorHandler.HideEditor();
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        if (isDragging)//selection
                        {

                        }
                        else //제자리 클릭..
                        {

                        }



                    }
                    else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                    {

                    }
                    else
                    {

                    }

                }
                if (item != null && isDragging == false)//item clicked or right clicked
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)//item clicked
                    {

                        if (Control.ModifierKeys == Keys.None)
                        {//click

                        }
                        else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {




                        }
                        else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        {



                        }
                        else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                        {



                        }
                    }
                }
                
            }

            _pressedNode = null;
            SelectionHandler.ReleaseTempSelection();

            _lastMouseUpNode = mouseUpNode;
            _lastMouseUpItem = item;
            DrawHandler.ReDrawTree(false);
            //SetNodeSelected(selectedNode, pt);
            //this.Focus();

        }

        /// <summary>
        /// 콘트롤 상에서의 point(e)를 넣으면 해당 포인트에 존재하는 노드를 리턴하고,
        /// Margin을 제외한 실제 컨트롤에서의 point를 리턴한다.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public wTreeNode GetNodeFromPoint(Point e, out Point pt)
        {
            int index = 0;
            pt = new Point(e.X - Margin.Left, e.Y - Margin.Top);
            wTreeNode selectedNode = null;
            foreach (wTreeNode node in VisibleNodes)
            {
                if (node.ContainsPoint(pt, ContainsModes.Y))
                //if (node.Location.Y < pt.Y && node.Location.Y + node.ImageBufferToDraw.Height >= pt.Y)
                {
                    selectedNode = node;
                    break;
                }
                index++;
            }
            return selectedNode;
        }

        /// <summary>
        /// 마우스 키를 눌렀을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wTree_MouseDown(object sender, MouseEventArgs e)
        {
            Point pt;
            wTreeNode selectedNode = GetNodeFromPoint(e.Location, out pt);
            //int index = 0;
            wTreeNodeItemMouseArgs args = new wTreeNodeItemMouseArgs(new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta));
            wTreeNodeItem item = (selectedNode == null) ? null : selectedNode.GetItemFromPoint(pt);
            if (E_TreeNodeMouseDown != null) E_TreeNodeMouseDown(selectedNode, item, args);
            if (args.IsCanceled) return;

            _pressedNode = selectedNode;
            _pressedItem = item;
            if (_pressedItem != null && _pressedItem.ItemType == wTreeNodeItemTypes.CheckBox)
            {
            }
            else
            {
                if ((ModifierKeys & Keys.Control) != Keys.Control)//Control을 안눌렀다면 이전의 선택부분을 모두 지움..
                {

                    if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        if (_isShiftDown)//이전에 shift를 누른 채 떼지 않았다면..
                        {
                            //_pressedNode = _lastMouseUp;//이전에 클릭했던 지점부터 시작..

                            //SelectedNode = _lastMouseUp;
                            int tempSelecting = VisibleNodes.IndexOf(selectedNode);
                            SelectionHandler.drawSelectingNodes(VisibleNodes.IndexOf(_lastMouseUpNode), tempSelecting);
                            _isShiftDown = true;
                        }
                        else
                        {
                            SelectionHandler.ClearAllSelections();

                            //_pressedNode = _lastMouseUp;//이전에 클릭했던 지점부터 시작..

                            //SelectedNode = _lastMouseUp;
                            int tempSelectingStart = VisibleNodes.IndexOf(_lastMouseUpNode);
                            int tempSelecting = VisibleNodes.IndexOf(selectedNode);
                            SelectionHandler.drawSelectingNodes(tempSelectingStart, tempSelecting);
                            _isShiftDown = true;
                        }
                        DrawHandler.ReDrawTree(false);
                    }
                    else
                    {
                        SelectionHandler.ClearAllSelections();

                        //SetNodeSelected(selectedNode, pt);
                        DrawHandler.ReDrawTree(false);
                    }
                    
                    //this.Focus();
                }
            }
            if (EditorHandler.ActivatedEditor != null || _ownerTree.Focused == false)
            {
                ReleaseFocus();

            }
        }

        internal void ReleaseFocus()
        {
            _ownerTree.B_FocusGetter.Focus();
        }

        wTreeNode _oldClickedNode = null;
        void _ownerTree_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks != 1) return;

            Point pt;
            
            //= new Point(e.X - Margin.Left, e.Y - Margin.Top);
            wTreeNode clickedNode = GetNodeFromPoint(e.Location, out pt);// _pressedNode;
            wTreeNodeItem item = (clickedNode==null)? null : clickedNode.GetItemFromPoint(pt);
            wTreeNodeItemMouseArgs args = new wTreeNodeItemMouseArgs(new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta));
            if (E_TreeNodeClicked != null) E_TreeNodeClicked(clickedNode, item, args);
            if (args.IsCanceled) return;

            if (clickedNode != null)
            {
                //selectedNode.IsExpaned = !selectedNode.IsExpaned;
                
                if (item != null)
                {
                    #region item is not null
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (Control.ModifierKeys == Keys.None)
                        {
                            if (item.ItemType == wTreeNodeItemTypes.CheckBox && (item.CheckBoxActiveAction & CheckboxActiveActions.Click) == CheckboxActiveActions.Click)
                            {
                                SelectionHandler.SetCheckboxIfItemIsCheckbox(clickedNode, item.ItemIndex);
                            }
                            else
                            {
                                if (clickedNode.IsSelected)
                                {
                                    if ((item.EditorActivateAction & EditorActivateActions.ClickOnSelection) == EditorActivateActions.ClickOnSelection)
                                    {
                                        EditorHandler.ShowEditor(item, item.ItemArea);
                                    }
                                    else if ((item.EditorActivateAction & EditorActivateActions.Click) == EditorActivateActions.Click)
                                    {
                                        EditorHandler.ShowEditor(item, item.ItemArea);
                                    }
                                    else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                                    {
                                        if ((EditorHandler.EditorActivateAction & EditorActivateBasicActions.ClickOnSelection) == EditorActivateBasicActions.ClickOnSelection)
                                        {
                                            EditorHandler.ShowEditor(item, item.ItemArea);
                                        }
                                        else if ((EditorHandler.EditorActivateAction & EditorActivateBasicActions.Click) == EditorActivateBasicActions.Click)
                                        {
                                            EditorHandler.ShowEditor(item, item.ItemArea);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((item.EditorActivateAction & EditorActivateActions.Click) == EditorActivateActions.Click)
                                    {
                                        EditorHandler.ShowEditor(item, item.ItemArea);
                                    }
                                }
                                _isShiftDown = false;

                                SelectionHandler.SelectedNode = clickedNode;
                                SelectionHandler.SetNodeSelected(clickedNode, item);


                            }


                        }
                        else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {


                            if (item.ItemType == wTreeNodeItemTypes.CheckBox && (item.CheckBoxActiveAction & CheckboxActiveActions.Click) == CheckboxActiveActions.Click)
                            {
                                if (clickedNode != null && clickedNode.TreeParent == clickedNode.TreeParent)
                                {
                                    int itemIndex = item.ItemIndex;
                                    int lastIndex = clickedNode.Index;


                                    SelectionHandler.SetCheckboxValueIfItemIsCheckbox(clickedNode.TreeParent, lastIndex, clickedNode.Index, itemIndex, true);

                                }
                                else
                                {
                                    if ((item.CheckBoxActiveAction & CheckboxActiveActions.Click) == CheckboxActiveActions.Click)
                                    {
                                        SelectionHandler.SetCheckboxValueIfItemIsCheckbox(clickedNode, item.ItemIndex, true);
                                    }
                                }
                            }
                            else
                            {
                                if (clickedNode.IsSelected)
                                {
                                    clickedNode.SetSelection(false);
                                    SelectedNode = null;
                                    clickedNode.DrawBuffer();
                                }
                                else
                                {
                                    clickedNode.SetSelection(true);
                                    SelectedNode = null;
                                    clickedNode.DrawBuffer();
                                }

                                if ((item.EditorActivateAction & EditorActivateActions.CtrlClick) == EditorActivateActions.CtrlClick)
                                {
                                    EditorHandler.ShowEditor(item, item.ItemArea);

                                }
                                else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                                {
                                    if ((EditorHandler.EditorActivateAction & EditorActivateBasicActions.CtrlClick) == EditorActivateBasicActions.CtrlClick)
                                    {
                                        EditorHandler.ShowEditor(item, item.ItemArea);
                                    }
                                }


                            }
                        }
                        else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        {

                            _isShiftDown = true;

                            if (item.ItemType == wTreeNodeItemTypes.CheckBox && (item.CheckBoxActiveAction & CheckboxActiveActions.Click) == CheckboxActiveActions.Click)
                            {
                                if (clickedNode != null && clickedNode.TreeParent == clickedNode.TreeParent)
                                {
                                    int itemIndex = item.ItemIndex;
                                    int lastIndex = (_oldClickedNode==null)? clickedNode.Index : _oldClickedNode.Index;

                                    SelectionHandler.SetCheckboxIfItemIsCheckbox(clickedNode.TreeParent, lastIndex, clickedNode.Index, itemIndex);

                                }
                                else
                                {
                                    if ((item.CheckBoxActiveAction & CheckboxActiveActions.Click) == CheckboxActiveActions.Click)
                                    {
                                        SelectionHandler.SetCheckboxIfItemIsCheckbox(clickedNode, item.ItemIndex);
                                    }
                                }
                            }
                            else
                            {
                                if ((item.EditorActivateAction & EditorActivateActions.ShiftClick) == EditorActivateActions.ShiftClick)
                                {
                                    EditorHandler.ShowEditor(item, item.ItemArea);

                                }
                                else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                                {
                                    if ((EditorHandler.EditorActivateAction & EditorActivateBasicActions.ShiftClick) == EditorActivateBasicActions.ShiftClick)
                                    {
                                        EditorHandler.ShowEditor(item, item.ItemArea);
                                    }

                                }

                            }

                        }
                        else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                        {


                            if (item.ItemType == wTreeNodeItemTypes.CheckBox && (item.CheckBoxActiveAction & CheckboxActiveActions.Click) == CheckboxActiveActions.Click)
                            {
                                if (clickedNode != null && clickedNode.TreeParent == clickedNode.TreeParent)
                                {
                                    int itemIndex = item.ItemIndex;
                                    int lastIndex = clickedNode.Index;
                                    SelectionHandler.SetCheckboxIfItemIsCheckbox(clickedNode.TreeParent, lastIndex, clickedNode.Index, itemIndex);

                                }
                                else
                                {
                                    if ((item.CheckBoxActiveAction & CheckboxActiveActions.Click) == CheckboxActiveActions.Click)
                                    {
                                        SelectionHandler.SetCheckboxValueIfItemIsCheckbox(clickedNode, item.ItemIndex, false);
                                    }
                                }
                            }
                            else
                            {
                                if ((item.EditorActivateAction & EditorActivateActions.AltClick) == EditorActivateActions.AltClick)
                                {
                                    EditorHandler.ShowEditor(item, item.ItemArea);

                                }
                                else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                                {
                                    if ((EditorHandler.EditorActivateAction & EditorActivateBasicActions.AltClick) == EditorActivateBasicActions.AltClick)
                                    {
                                        EditorHandler.ShowEditor(item, item.ItemArea);
                                    }
                                }

                            }
                        }
                    }
                    #endregion
                }
                else // item is null
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (Control.ModifierKeys == Keys.None)
                        {
                            _isShiftDown = false;

                            SelectionHandler.SelectedNode = clickedNode;
                            SelectionHandler.SetNodeSelected(clickedNode, item);

                        }
                        else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {


                            if (clickedNode.IsSelected)
                            {
                                clickedNode.SetSelection(false);
                                SelectedNode = null;
                                clickedNode.DrawBuffer();
                            }
                            else
                            {
                                clickedNode.SetSelection(true);
                                SelectedNode = null;
                                clickedNode.DrawBuffer();
                            }

                        }
                        else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        {

                            _isShiftDown = true;

                        }
                        else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                        {

                        }
                    }
                }
                
                
                _oldClickedNode = clickedNode;
            }

        }

        bool _doubleClicked = false;
        void wTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            Point pt;
            //= new Point(e.X - Margin.Left, e.Y - Margin.Top);
            wTreeNode clickedNode = GetNodeFromPoint(e.Location, out pt);// _pressedNode;
            wTreeNodeItem item = clickedNode.GetItemFromPoint(pt);
            wTreeNodeItemMouseArgs args = new wTreeNodeItemMouseArgs(e);
            if (E_TreeNodeDoubleClicked != null) E_TreeNodeDoubleClicked(clickedNode, item, args);

            if (args.IsCanceled) return;

            if (_lastMouseUpNode != null)
            {
                //selectedNode.IsExpaned = !selectedNode.IsExpaned;
            
                if (item != null)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (Control.ModifierKeys == Keys.None)
                        {


                            if (item.ItemType == wTreeNodeItemTypes.CheckBox && (item.CheckBoxActiveAction & CheckboxActiveActions.DoubleClick) == CheckboxActiveActions.DoubleClick)
                            {
                                SelectionHandler.SetCheckboxIfItemIsCheckbox(_lastMouseUpNode, item.ItemIndex);
                            }
                            else
                            {
                                if (item.EditorActivateAction == EditorActivateActions.DoubleClick)
                                {
                                    EditorHandler.ShowEditor(item, item.ItemArea);

                                }
                                else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                                {
                                    if (EditorHandler.EditorActivateAction == EditorActivateBasicActions.DoubleClick)
                                    {
                                        EditorHandler.ShowEditor(item, item.ItemArea);
                                    }
                                }
                            }
                        }
                        else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {
                            if (item.EditorActivateAction == EditorActivateActions.CtrlDoubleClick)
                            {
                                EditorHandler.ShowEditor(item, item.ItemArea);

                            }
                            else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                            {
                                if (EditorHandler.EditorActivateAction == EditorActivateBasicActions.CtrlDoubleClick)
                                {
                                    EditorHandler.ShowEditor(item, item.ItemArea);
                                }
                            }


                        }
                        else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        {
                            if (item.EditorActivateAction == EditorActivateActions.ShiftDoubleClick)
                            {
                                EditorHandler.ShowEditor(item, item.ItemArea);

                            }
                            else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                            {
                                if (EditorHandler.EditorActivateAction == EditorActivateBasicActions.ShiftDoubleClick)
                                {
                                    EditorHandler.ShowEditor(item, item.ItemArea);
                                }
                            }
                        }
                        else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                        {
                            if (item.EditorActivateAction == EditorActivateActions.AltDoubleClick)
                            {
                                EditorHandler.ShowEditor(item, item.ItemArea);

                            }
                            else if (item.EditorActivateAction == EditorActivateActions.UseBasicSetting)
                            {
                                if (EditorHandler.EditorActivateAction == EditorActivateBasicActions.AltDoubleClick)
                                {
                                    EditorHandler.ShowEditor(item, item.ItemArea);
                                }
                            }
                        }
                    }
                }



                _doubleClicked = true;
            }

        }




        //public wTreeNode PressedNode { get { return _pressedNode; }  }

        //public wTreeNodeItem PressedItem { get { return _pressedItem; } }

        /// <summary>
        /// the Node which mouse pressed on
        /// </summary>
        public wTreeNode PressedNode { get { return _pressedNode; } }

        /// <summary>
        /// the Node Item which mouse pressed on
        /// </summary>
        public wTreeNodeItem PressedItem { get { return _pressedItem; } }

        /// <summary>
        /// the Node which mouse up last.
        /// </summary>
        public wTreeNode LastMouseUpNode { get { return _lastMouseUpNode; } }



        /// <summary>
        /// the Node which mouse up last.
        /// </summary>
        public wTreeNodeItem LastMouseUpNodeItem { get { return _lastMouseUpItem; } }

    }
}