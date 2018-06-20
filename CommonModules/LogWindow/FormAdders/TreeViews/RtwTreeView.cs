using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace FormAdders
{
    [System.ComponentModel.Designer(typeof(TriStateTreeViewDesigner))]
    /// <summary>
    /// 체크박스를 이미지로 나타내는 트리뷰..
    /// 여러가지 기능이 추가되었다.
    /// </summary>
    public class RtwTreeView : TreeView
    {
        #region Events
        public event TreeNodeClickEventHandler E_OnEndNodeClicked;
        public event TreeNodeClickEventHandler E_OnParentNodeClicked;
        
        public event TreeNodeClickEventHandler E_OnEndNodeDoubleClicked;
        public event TreeNodeClickEventHandler E_OnParentNodeDoubleClicked;
        
        public event TreeNodeClickEventHandler E_OnEndNodeRightClicked;
        public event TreeNodeClickEventHandler E_OnParentNodeRightClicked;
        
        public event TreeNodeClickEventHandler E_OnEndNodeMiddleClicked;
        public event TreeNodeClickEventHandler E_OnParentNodeMiddleClicked;

        public event RtwTreeNodeCheckedEventHandler E_OnEndNodeChecked;
        public event RtwTreeNodeCheckedEventHandler E_OnParentNodeChecked;
        #endregion

        delegate void DelFunc();

        #region Actions
        public enum Actions { None=0 ,CopyNameToClipBaord=1, OpenNodes=2, a3=3, CheckBoxClick=4, a4, a5, a6, a7, ContextMenuOpened=8, a9, a10, a11, a12, a13, a14, a15, CloseSiblings=16, a17,a18, a19,a20,a21, a22,a23, ToggleExpand=32 };

        public Actions ActionOnParentNodeClicked = Actions.CopyNameToClipBaord|Actions.CloseSiblings|Actions.ToggleExpand;
        public Actions ActionOnParentNodeDoubleClicked = Actions.OpenNodes;
        public Actions ActionOnEndNodeClicked = Actions.CopyNameToClipBaord|Actions.CloseSiblings;
        public Actions ActionOnEndNodeDoubleClicked = Actions.CheckBoxClick;
        public Actions ActionOnParentNodeRightClicked = Actions.ContextMenuOpened;
        public Actions ActionOnEndNodeRightClicked = Actions.ContextMenuOpened;
        public Actions ActionOnEndNodeMiddleClicked = Actions.CheckBoxClick;
        public Actions ActionOnParentNodeMiddleClicked = Actions.CheckBoxClick;

        #endregion

        #region contextMenu

        TriStateTreeNode _selectedTreeNode; //ContextMenu를 Open할때 기준이 되는 Node
        ContextMenu _contextMenuParentNode = new ContextMenu();
        public ContextMenu U_ContextMenuParentNode
        {
            get { return _contextMenuParentNode; }
        }
        ContextMenu _contextMenuEndNode = new ContextMenu();
        public ContextMenu U_ContextMenuEndNode
        {
            get { return _contextMenuEndNode; }
        }

        public List<MenuItem> ContextMenuItems = new List<MenuItem>();
        Dictionary<MenuItem, ContextMenuClickHandler> _contextMenuParentClickHandlers = new Dictionary<MenuItem, ContextMenuClickHandler>();
        Dictionary<MenuItem, ContextMenuClickHandler> _contextMenuEndClickHandlers = new Dictionary<MenuItem, ContextMenuClickHandler>();
        public delegate void ContextMenuClickHandler(Object sender, String text, int index, TriStateTreeNode selectedRow, object MenuItem);
        public event ContextMenuClickHandler E_ContextMenuParentClicked = null;
        public event ContextMenuClickHandler E_ContextMenuEndClicked = null;
        #endregion


        bool _isCheckBoxClicked = false;

        public RtwTreeView()
            : base()
        {
            
            //this.NodeMouseClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseClick);
            //this.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(RtwTreeView_NodeMouseDoubleClick);
            //this.BeforeCheck += new TreeViewCancelEventHandler(RtwTreeView_BeforeCheck);
            //this.AfterCheck += new TreeViewEventHandler(RtwTreeView_AfterCheck);
            //this.BeforeExpand += new TreeViewCancelEventHandler(RtwTreeView_BeforeExpand);
            //this.BeforeCollapse += new TreeViewCancelEventHandler(RtwTreeView_BeforeCollapse);
            
            //this.CheckBoxes = true;
            
            this.ShowPlusMinus = false;
            this.ShowLines = false;
            this.ShowRootLines = false;

            ImageList list = new ImageList();
            list.Images.AddRange(new Image[]{
                Properties.Resources.rtw_tree_normal,
                Properties.Resources.rtw_open_tree,
                Properties.Resources.rtw_tree_selected});
            this.ImageList = list;
            
            list = new ImageList();
            list.Images.AddRange( new Image[]{
                Properties.Resources.check_normal,
                Properties.Resources.check_red,
                Properties.Resources.check_inter
                });
            this.StateImageList = list;
            
            //this.ImageIndex = 0;
            //this.SelectedImageIndex = 0;

            //this.SelectedImageIndex = 2;
            _nodes = new TriStateTreeNodeCollection(base.Nodes,this);
            
        }
        protected override void OnDoubleClick(EventArgs e)
        {
            //base.OnDoubleClick(e);
        }
        
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            
            System.Windows.Forms.TreeViewHitTestInfo info = this.HitTest(e.X, e.Y);
            if (info.Node != null && info.Location.ToString() == "StateImage")
            {
                return;
                //OnCustomCheck(info.Node, System.Windows.Forms.TreeViewAction.ByMouse);
            }
            base.OnMouseDoubleClick(e);
        }

        
        protected new void CreateStateImages()
        {
            //base.CreateStateImages();
            ImageList list = new ImageList();
            list.Images.AddRange(new Image[]{
                Properties.Resources.check_normal,
                Properties.Resources.check_red,
                Properties.Resources.check_inter
                });
            this.StateImageList = list;

            /*
            System.Windows.Forms.VisualStyles.VisualStyleRenderer vsr = null;
            if (System.Windows.Forms.VisualStyles.VisualStyleRenderer.IsSupported) vsr = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Button.CheckBox.UncheckedNormal);
            System.Drawing.Rectangle smallIconSize = new System.Drawing.Rectangle(0, 0, StateImageList.ImageSize.Width, StateImageList.ImageSize.Height);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(StateImageList.ImageSize.Width, StateImageList.ImageSize.Height);
            //
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                if (vsr == null)
                {
                    System.Windows.Forms.ControlPaint.DrawMixedCheckBox(graphics, smallIconSize, System.Windows.Forms.ButtonState.Normal);
                }
                else
                {
                    vsr.SetParameters(System.Windows.Forms.VisualStyles.VisualStyleElement.Button.CheckBox.UncheckedNormal);
                    vsr.DrawBackground(graphics, smallIconSize);
                }
                StateImageList.Images.Add(bitmap, System.Drawing.Color.Transparent);
                //
                if (vsr == null)
                {
                    System.Windows.Forms.ControlPaint.DrawMixedCheckBox(graphics, smallIconSize, System.Windows.Forms.ButtonState.Checked);
                }
                else
                {
                    vsr.SetParameters(System.Windows.Forms.VisualStyles.VisualStyleElement.Button.CheckBox.CheckedNormal);
                    vsr.DrawBackground(graphics, smallIconSize);
                }
                StateImageList.Images.Add(bitmap, System.Drawing.Color.Transparent);
                //
                if (vsr == null)
                {
                    System.Windows.Forms.ControlPaint.DrawMixedCheckBox(graphics, smallIconSize, System.Windows.Forms.ButtonState.Pushed);
                }
                else
                {
                    vsr.SetParameters(System.Windows.Forms.VisualStyles.VisualStyleElement.Button.CheckBox.MixedNormal);
                    vsr.DrawBackground(graphics, smallIconSize);
                }
                StateImageList.Images.Add(bitmap, System.Drawing.Color.Transparent);
            }
             */
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            System.Windows.Forms.TreeViewHitTestInfo info = this.HitTest(e.X, e.Y);
            if (info.Node != null && info.Location.ToString() == "StateImage")
            {
                _isCheckBoxClicked = true;
                
            }
            
            //
            //base.OnMouseDown(e);
        }

        public void UpdateTreeNodes()
        {
            FindCheckedEndNodes();
        }

        public override void Refresh()
        {
            UpdateTreeNodes();
            base.Refresh();
        }
        
        public new void Update()
        {
            UpdateTreeNodes();
            base.Update();
        }

        

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            if (_isCheckBoxClicked)
            {
                e.Cancel = true;
                _isCheckBoxClicked = false;
            }
            else
            {
                e.Node.ImageIndex = 2; //collapse되었다는 것은 자식이 있는 것이므로..
                e.Node.SelectedImageIndex = 2;
                base.OnBeforeCollapse(e);
            }
            
        }
        

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            if (_isCheckBoxClicked) e.Cancel = true;
            else
            {
                e.Node.SelectedImageIndex = 1;
                e.Node.ImageIndex = 1;
                base.OnBeforeExpand(e);
            }
            
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            //base.OnAfterCheck(e);
            
        }

        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            
            _isCheckBoxClicked = true;

            //CheckBoxClicked(e.Node as TriStateTreeNode);
            
            //e.Cancel = true;
            //e.Cancel = true;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_isCheckBoxClicked)
            {
                System.Windows.Forms.TreeViewHitTestInfo info = this.HitTest(e.X, e.Y);
                if (info.Node != null)
                {
                    if (info.Node != null && info.Location.ToString() == "StateImage")
                    {
                        _isCheckBoxClicked = true;
                        CheckBoxClicked(info.Node as TriStateTreeNode);
                        

                    }else _isCheckBoxClicked = false;
                }
                _isCheckBoxClicked = false;
            }
            //base.OnMouseClick(e);
        }
        /*
        void setCheckBox(TriStateTreeNode node, bool isChecked)
        {
            System.Windows.Forms.TreeViewCancelEventArgs e = new
            System.Windows.Forms.TreeViewCancelEventArgs(node, isChecked, TreeViewAction.ByMouse);
            OnBeforeCheck(e);

            if (e.Cancel) return;
            
            
            if (node is IStateTreeNode)
            {
                ((IStateTreeNode)node).UpdateState(e);
            }
            //
            OnAfterCheck(new System.Windows.Forms.TreeViewEventArgs(node, e.Action));
        }
        */

        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            if (e.Clicks == 1) return;

            if (_isCheckBoxClicked)
            {
                _isCheckBoxClicked = false;
                return;
            }

            this.HideSelection = true;
            _selectedTreeNode = e.Node as TriStateTreeNode;
            bool? state = (_selectedTreeNode.CheckState == CheckState.Checked) ? true : (_selectedTreeNode.CheckState == CheckState.Unchecked) ? false : (bool?)null;
            TreeNodeClickEventArg arg = new TreeNodeClickEventArg(state, e.Node, this);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                }
                else if (Control.ModifierKeys == Keys.Shift)
                {
                }
                else if (Control.ModifierKeys == Keys.Alt)
                {
                }
                else
                {
                    //노드를 클릭하면,(LeftClick)
                    if (e.Node.Nodes.Count > 0)
                    {
                        DoAction(ActionOnParentNodeDoubleClicked, e.Node);
                        if (E_OnParentNodeDoubleClicked != null) E_OnParentNodeDoubleClicked(this, arg);
                    }
                    else
                    { //자식 노드가 없다면 마지막 노드이므로 이벤트를 호출한다.
                        DoAction(ActionOnEndNodeDoubleClicked, e.Node);
                        if (E_OnEndNodeDoubleClicked != null) E_OnEndNodeDoubleClicked(this, arg);
                    }
                }
                //base.OnDoubleClick(e);
                return;
            }/*
            else if (e.Button != System.Windows.Forms.MouseButtons.Right)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    DoAction(ActionOnParentNodeRightDoubleClicked, e.Node);
                    if (E_OnParentNodeRightClicked != null) E_OnParentNodeRightDoubleClicked(tv, arg);
                }
                else
                {
                    DoAction(ActionOnEndNodeRightDoubleClicked, e.Node);
                    if (E_OnEndNodeRightClicked != null) E_OnEndNodeRightDoubleClicked(tv, arg);
                }

                return;
            }
            else if (e.Button != System.Windows.Forms.MouseButtons.Middle)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    DoAction(ActionOnParentNodeMiddleDoubleClicked, e.Node);
                    if (E_OnParentNodeMiddleClicked != null) E_OnParentNodeMiddleDoubleClicked(tv, arg);
                }
                else
                {
                    DoAction(ActionOnEndNodeMiddleDoubleClicked, e.Node);
                    if (E_OnEndNodeMiddleClicked != null) E_OnEndNodeMiddleDoubleClicked(tv, arg);
                }

                return;
            }
              */

        }
        

        /*
        [Browsable(true)]
        [EditorBrowsable]
        public List<TriStateTreeNode> RootNodes
        {
            get
            {
                return (Nodes as TriStateTreeNodeCollection).ToList<TriStateTreeNode>();
            }
            set
            {
                Nodes.Clear();
                (Nodes as TriStateTreeNodeCollection).AddRange(value);
            }
        }
        */

        public TreeNode AddRoot(String text)
        {
            TriStateTreeNode aRoot = new TriStateTreeNode(text);
            this.Nodes.Add(aRoot);
            return aRoot;
        }

        /// <summary>
        /// 해당 root에 자식노드를 할당하고 새로 생성된 자식노드를 리턴한다.
        /// </summary>
        /// <param name="rootIndex">자식노들 추가할 루트의 인덱스</param>
        /// <param name="name">자식노드를 호출할 때 쓰일 이름</param>
        /// <param name="text">트리에 나타날 이름</param>
        /// <returns></returns>
        public TriStateTreeNode AddChild(int rootIndex, String text){
            TriStateTreeNode root; 
            TriStateTreeNode node = new TriStateTreeNode(text);
            if (rootIndex >= 0 && this.Nodes.Count > rootIndex)
            {
                root = this.Nodes[rootIndex] as TriStateTreeNode;
                root.Nodes.Add(node);
            }
            else
            {
                root = this.Nodes[this.Nodes.Count - 1] as TriStateTreeNode;
                root.Nodes.Add(node);
            }
            return node;
        }

        public TriStateTreeNode AddChild(TriStateTreeNode parent, String text){
            TriStateTreeNode node = new TriStateTreeNode(text);
            parent.Nodes.Add(node);
            return node;
        }

        public TriStateTreeNode getNode(String fullPath, String spliter="."){
            String[] tokens = fullPath.Split(spliter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int tokenIndex = 0;
            if(fullPath.Length==0) return null;
            try{
                TreeNode node = this.Nodes[tokens[tokenIndex++]];
                while(tokenIndex<tokens.Length){
                    node = node.Nodes[tokens[tokenIndex++]];
                }
                return node as TriStateTreeNode;
            }catch{//해당 node가 없을 때..
                return null;
            }
        }

        public new ImageList ImageList
        {
            get
            {
                return base.ImageList;
            }
            set
            {
                base.ImageList = value;
            }
        }

        List<String> findTreePath(TreeNode node){
            
            List<String> treePath = new List<string>();
            
            TreeNode parent=node;
            while(parent!=null){
                treePath.Insert(0, parent.Text);
                parent = parent.Parent;
            }

            return treePath;
        }

        List<int> findTreeIndexPath(TreeNode node){
            List<int> treePath = new List<int>();
            
            TreeNode before=node;
            TreeNode parent=before.Parent;
            
            while(parent!=null){
                treePath.Insert(0, parent.Nodes.IndexOf(node));
                before = parent;
                parent = before.Parent;
            }
            treePath.Insert(0, this.Nodes.IndexOf(before));

            return treePath;
        }
        void DoAction(Actions action, TreeNode passedNode){
            if (_isCheckBoxClicked) return;

            TriStateTreeNode node = passedNode as TriStateTreeNode;

            if((action & Actions.CloseSiblings)>0){
                //자신과 같은 depth의 node가 열려있으면 모두 닫고,
                TriStateTreeNode parent = node.Parent as TriStateTreeNode;
                if (parent != null)
                {
                    if (node.Nodes.Count > 0)
                    {
                        foreach (TriStateTreeNode siblings in parent.Nodes)
                        {
                            if (siblings != node)
                            {
                                siblings.Collapse(true);

                            }
                        }
                    }
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        foreach (TriStateTreeNode siblings in Nodes)
                        {
                            if (siblings != node)
                            {
                                siblings.Collapse(true);

                            }
                        }
                    }
                }
            }

            if((action & Actions.ToggleExpand)>0 ){
                //자식노드가 존재한다면 자식노드를 열거나 닫는다.
                
                if (node.Nodes.Count > 0)
                {
                    if (node.IsExpanded)
                    {
                        node.Collapse(false);
                    }
                    else
                    {
                        node.Expand();
                    }
                }
                
            }

            if((action & Actions.ContextMenuOpened)>0){ //ContextMenu가 있으면 연다.
                if (node.Nodes.Count == 0)
                {
                    if (U_ContextMenuEndNode != null) U_ContextMenuEndNode.Show(this, this.PointToClient(Control.MousePosition));
                }
                else
                {
                    if (U_ContextMenuParentNode != null) U_ContextMenuParentNode.Show(this, this.PointToClient(Control.MousePosition));
                }
            }

            if ((action & Actions.CheckBoxClick) > 0)
            {
                List<TriStateTreeNode> added = new List<TriStateTreeNode>();
                List<TriStateTreeNode> removed = new List<TriStateTreeNode>();

                if (node.Checked == false) //기존에 상태가 false 이면..
                {
                    if(_isCheckBoxClicked==false) setEndNodesChecked(node as TriStateTreeNode, true, added, removed);
                }
                else //중간단계도 상태를 false로 만듬.
                {
                    if (_isCheckBoxClicked == false) setEndNodesChecked(node as TriStateTreeNode, false, added, removed);
                }
                
                if (node.Nodes.Count == 0)
                {
                    if (E_OnEndNodeChecked != null) E_OnEndNodeChecked(this,
                        new RtwTreeNodeCheckedEventArg(_checkedNodes, added, removed));
                }
                else
                {
                    if (E_OnParentNodeChecked != null) E_OnParentNodeChecked(this,
                        new RtwTreeNodeCheckedEventArg(_checkedNodes, added, removed));
                }
            }

            if ((action & Actions.CopyNameToClipBaord) > 0)
            {
                Clipboard.SetText(getFullPathByText(node));
            }
            
        }

        String getFullPathByText(TreeNode node, String seperator=".")
        {
            String path = node.Text;

            while ((node = node.Parent) != null)
            {
                path = node.Text + seperator + path;
                //path += seperator + node.Text;
            }
            return path;
        }

        /// <summary>
        /// 노드의 자식에 있는 모든 EndNode를 isChecked의 상태로 바꾼다.
        /// 만일 parent가 EndNode라면 그 자신의 상태만 갱신한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isChecked"></param>
        /// <param name="added"></param>
        /// <param name="removed"></param>
        public void setEndNodesChecked(TriStateTreeNode node, bool isChecked, List<TriStateTreeNode> added, List<TriStateTreeNode> removed)
        {
            
            if (node.Nodes.Count == 0) //자식 노드가 더이상 없으면
            {
                if (isChecked)
                {
                    if (node.Checked == false)
                    {
                        if(added!=null) added.Add(node);
                        
                        _checkedNodes.Add(node);
                        //setCheckBox(node, true);
                        node.Checked = true;
                        
                        //setCheckBox(node, true);
                    }
                }
                else
                {
                    if (node.Checked == true)
                    {
                        if(removed!=null) removed.Add(node);
                        _checkedNodes.Remove(node);
                        //setCheckBox(node, false);
                        node.Checked = false;
                    }
                }
                if (node is IStateTreeNode)
                {
                    System.Windows.Forms.TreeViewCancelEventArgs e = new System.Windows.Forms.TreeViewCancelEventArgs(node, false, TreeViewAction.ByMouse);
            
                    ((IStateTreeNode)node).UpdateState(e);
                }
            }
            else //자식 노드가 존재하면..
            {
                
                foreach (TriStateTreeNode aNode in node.Nodes)
                {
                    setEndNodesChecked(aNode, isChecked, added, removed);
                }
                //node.Checked = isChecked;
                //setCheckBox(node, isChecked);
            }
            //node.Checked = isChecked;
           // node.Checked = (isChecked);

            if(node.Parent!=null) CheckParent(node.Parent);
        }

        void CheckParent(TreeNode node)
        {
            
            bool isAllChecked = true;
            int unChecked =0;
            
                foreach (TriStateTreeNode child in node.Nodes)
                {
                    if (child.Checked == false)
                    {
                        isAllChecked = false;
                        unChecked++;
                    }
                }
                if (isAllChecked == false)
                {
                    if (unChecked == node.Nodes.Count)
                    {//all unchecked
                       // node.Parent.Checked = false;
                        (node as TriStateTreeNode).CheckState = CheckState.Unchecked;
                    }
                    else //partial unchecked
                    {
                        //node.Parent.Checked = true;
                        (node as TriStateTreeNode).CheckState = CheckState.Indeterminate;
                    }
                }
                else
                {
                    //node.Parent.Checked = true;
                    (node as TriStateTreeNode).CheckState = CheckState.Checked;
                }
                if (node.Parent != null) CheckParent(node.Parent);
            
        }

        /// <summary>
        /// 체크된 노드를 관리하는 리스트.
        /// </summary>
        List<TriStateTreeNode> _checkedNodes = new List<TriStateTreeNode>();
        /// <summary>
        /// IsChecked==true 인 TriStateTreeNode의 리스트를 가져온다. 복사본이므로 수정되지 않는다.
        /// </summary>
        public List<TriStateTreeNode> CheckedNodes
        {
            get
            {
                return new List<TriStateTreeNode>(_checkedNodes);
            }
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (_isCheckBoxClicked)
            {
                //_isCheckBoxClicked = false;
                
                return;
            }

            this.HideSelection = true;
            _selectedTreeNode = e.Node as TriStateTreeNode;
            bool? state = (_selectedTreeNode.CheckState == CheckState.Checked) ? true : (_selectedTreeNode.CheckState == CheckState.Unchecked) ? false : (bool?)null;

            TreeNodeClickEventArg arg = new TreeNodeClickEventArg(state, e.Node, this);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                }
                else if (Control.ModifierKeys == Keys.Shift)
                {
                }
                else if (Control.ModifierKeys == Keys.Alt)
                {
                }
                else
                {
                    //노드를 클릭하면,(LeftClick)
                    if (e.Node.Nodes.Count > 0)
                    {
                        DoAction(ActionOnParentNodeClicked, e.Node);
                        if (E_OnParentNodeClicked != null) E_OnParentNodeClicked(this, arg);
                    }
                    else
                    { //자식 노드가 없다면 마지막 노드이므로 이벤트를 호출한다.
                        DoAction(ActionOnEndNodeClicked, e.Node);
                        if (E_OnEndNodeClicked != null) E_OnEndNodeClicked(this, arg);
                    }
                }
                return;
            }
            else if (e.Button != System.Windows.Forms.MouseButtons.Right)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    DoAction(ActionOnParentNodeRightClicked, e.Node);
                    if (E_OnParentNodeRightClicked != null) E_OnParentNodeRightClicked(this, arg);
                }
                else
                {
                    DoAction(ActionOnEndNodeRightClicked, e.Node);
                    if (E_OnEndNodeRightClicked != null) E_OnEndNodeRightClicked(this, arg);
                }

                return;
            }
            else if (e.Button != System.Windows.Forms.MouseButtons.Middle)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    DoAction(ActionOnParentNodeMiddleClicked, e.Node);
                    if (E_OnParentNodeMiddleClicked != null) E_OnParentNodeMiddleClicked(this, arg);
                }
                else
                {
                    DoAction(ActionOnEndNodeMiddleClicked, e.Node);
                    if (E_OnEndNodeMiddleClicked != null) E_OnEndNodeMiddleClicked(this, arg);
                }

                return;
            }

            //base.OnNodeMouseClick(e);
        }

        

        /// <summary>
        /// ContextMenu에 Item을 추가한다. 필요에 따라 각 Item별로 함수를 지정할 수도 있다.
        /// </summary>
        /// <param name="text">보여질 text</param>
        /// <param name="eventHandler">필요하다면 따로 실행될 함수</param>
        /// <param name="baseMenu">필요하다면 미리 만들어 둔 Context 메뉴. 여기에 항목이 추가된다.</param>
        /// <param name="tooltip">메뉴에서 보여질 툴팁</param>
        /// <param name="inputGesture">바로가기 버튼을 지정할 수 있다. Ctrl+C 와 같이 적는다.</param>
        public void AddContextMenuItemParentNode(String text, ContextMenuClickHandler eventHandler = null, ContextMenu baseMenu = null, String tooltip = null, String inputGesture = null)
        {
            if (baseMenu == null) baseMenu = U_ContextMenuParentNode;
            else _contextMenuParentNode = baseMenu; //직접 지정해 줄 경우, 레퍼런스를 바꾸어준다.
            MenuItem item = new MenuItem();
            ContextMenuItems.Add(item);
            item.Text = text;
            item.Click += new EventHandler(ParentNodeMenuClicked);
            if (eventHandler != null) _contextMenuParentClickHandlers.Add(item, eventHandler);
            
            baseMenu.MenuItems.Add(item);
        }

        void  ParentNodeMenuClicked(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            int index = ContextMenuItems.IndexOf(item);
            if (_contextMenuParentClickHandlers.ContainsKey(item)) _contextMenuParentClickHandlers[item].Invoke(this, item.Text.ToString(), index, _selectedTreeNode, item);
            else if (E_ContextMenuParentClicked != null) E_ContextMenuParentClicked(this, item.Text.ToString(), index, _selectedTreeNode, item);
        }


        /// <summary>
        /// ContextMenu에 Item을 추가한다. 필요에 따라 각 Item별로 함수를 지정할 수도 있다.
        /// </summary>
        /// <param name="text">보여질 text</param>
        /// <param name="eventHandler">필요하다면 따로 실행될 함수</param>
        /// <param name="baseMenu">필요하다면 미리 만들어 둔 Context 메뉴. 여기에 항목이 추가된다.</param>
        /// <param name="tooltip">메뉴에서 보여질 툴팁</param>
        /// <param name="inputGesture">바로가기 버튼을 지정할 수 있다. Ctrl+C 와 같이 적는다.</param>
        public void AddContextMenuItemEndNode(String text, ContextMenuClickHandler eventHandler = null, ContextMenu baseMenu = null, String tooltip = null, String inputGesture = null)
        {
            if (baseMenu == null) baseMenu = U_ContextMenuEndNode;
            MenuItem item = new MenuItem();
            ContextMenuItems.Add(item);
            item.Text = text;
            item.Click +=new EventHandler(EndNodeMenuClicked);
            if (eventHandler != null) _contextMenuParentClickHandlers.Add(item, eventHandler);

            baseMenu.MenuItems.Add(item);
        }

        void EndNodeMenuClicked(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            int index = ContextMenuItems.IndexOf(item);
            if (_contextMenuEndClickHandlers.ContainsKey(item)) _contextMenuEndClickHandlers[item].Invoke(this, item.Text.ToString(), index, _selectedTreeNode, item);
            else if(E_ContextMenuEndClicked!=null) E_ContextMenuEndClicked(this, item.Text.ToString(), index, _selectedTreeNode, item);
        }

       /* 
        public bool CheckBoxVisibility
        {
            get { return this.CheckBoxes; }
            set
            {
                this.CheckBoxes = value;
                //TOTO: 체크박스를 보이거나 안보이게 한다.
            }
        }
        */
        /*
        [Browsable(true)]
        [EditorBrowsable]
        public new List<TriStateTreeNode> Items
        {
            get
            {
                return Nodes.ToList();// (Nodes as TriStateTreeNodeCollection).ToList();
            }
            set
            {
                Converter<TreeNode, TriStateTreeNode> converter = Converters.ConvertToTriStateTreeNode;
                Nodes.Clear();
                (Nodes as TriStateTreeNodeCollection).AddRange(value);
            }
        }
        */

        TriStateTreeNodeCollection _nodes;
        public new TriStateTreeNodeCollection Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                List<TriStateTreeNode> list = value.ToList();
                _nodes.Clear();
                _nodes.AddRange(list);
            }
        }

        public void UnCheckAll()
        {
            List<TriStateTreeNode> list = FindCheckedEndNodes();
            
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Checked == true) list[i].Checked = false;
                //TODO: 모든 CHECK를 해제한다.
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Checked = false;
            }
            
        }

        bool _isEventEnabled = true;
        public void SuspendEventActivations()
        {
            _isEventEnabled = false;
        }
        public void ResumeEventActivations()
        {
            _isEventEnabled = true;
        }

        public List<TriStateTreeNode> FindCheckedEndNodes()
        {
            List<TriStateTreeNode> chList = new List<TriStateTreeNode>();
                bool allChecked = true;
                
                for(int i=0; i<Nodes.Count; i++){
                    List<TriStateTreeNode> nodes = getCheckedEndNodes(Nodes[i], out allChecked);
                    if (allChecked) Nodes[i].Checked = true; //모두 체크되었으면
                    else if (nodes.Count == 0) Nodes[i].Checked = false; //모두 체크안되었으면
                    else Nodes[i].Checked = null;// (Nodes[i] as TriStateTreeNode).CheckState = CheckState.Indeterminate; //일부만 체크되었으면
                    chList.AddRange(nodes);
                }
                return chList;
        }

        /// <summary>
        /// 모든 하위 노드를 검색하여 체크가 된 endNode를 가져온다.
        /// </summary>
        /// <param name="passed"></param>
        /// <param name="allChecked"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        List<TriStateTreeNode> getCheckedEndNodes(TriStateTreeNode passed, out bool allChecked, List<TriStateTreeNode> nodes = null)
        {
            if (nodes == null) nodes = new List<TriStateTreeNode>();
            if(passed.Nodes.Count>0){
                if (passed.IsExpanded == false)
                {
                    passed.ImageIndex = 2;
                    passed.SelectedImageIndex = 2;
                }
                allChecked = true;
                bool tempCheck = true;
                foreach (TriStateTreeNode node in passed.Nodes)
                {
                    getCheckedEndNodes(node, out tempCheck, nodes);
                    if (tempCheck == false) allChecked = false;
                }
                if (allChecked)
                {
                    passed.Checked = true;
                }
                else
                {
                    if (nodes.Count == 0) passed.Checked = false;
                    else passed.Checked = null;
                }
            }
            else if (passed.Checked == true)
            {
                allChecked = true;
                nodes.Add(passed);
                if (_checkedNodes.Contains(passed)==false) _checkedNodes.Add(passed);
                ///_checkedNodes는 자동으로 추가/삭제하므로 사용자가 node.Checked를 사용하여 access했을 경우
                ///잡아내지 못한다. 그러므로 이런 식으로 갱신한다.
            }
            else
            {
                allChecked = false;
                if (_checkedNodes.Contains(passed)) _checkedNodes.Remove(passed);
            }
            return nodes;
        }
        
        public void ClearAllItems()
        {
            this.Nodes.Clear();
        }

        public TriStateTreeNode getNodeByName(String NamePath,String seperator=".")
        {
            String[] names = NamePath.Split( seperator.ToCharArray());
            int depth=0;
            TreeNode node = null;
            if(Nodes.Count>0){
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i].Name.Equals(names[depth]))
                    {
                        depth++;
                        node = Nodes[i];
                        break;
                    }
                }
                if(node==null) return null;
            }else return null;
            
            while (node.Nodes.Count > 0 && names.Length>depth)
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    if(node.Nodes[i].Name.Equals(names[depth])){
                        depth++;
                        node = node.Nodes[i];
                        break;
                    }
                }
            }
            if (names.Length == depth) return node as TriStateTreeNode;
            else return null;
        }

        public TriStateTreeNode getNodeByText(String TextPath, String seperator = ".")
        {
            String[] texts = TextPath.Split(seperator.ToCharArray());
            int depth = 0;
            TreeNode node = null;
            if (Nodes.Count > 0)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i].Text.Equals(texts[depth]))
                    {
                        depth++;
                        node = Nodes[i];
                        break;
                    }
                }
                if (node == null) return null;
            }
            else return null;

            while (node.Nodes.Count > 0 && texts.Length > depth)
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    if (node.Nodes[i].Text.Equals(texts[depth]))
                    {
                        depth++;
                        node = node.Nodes[i];
                        break;
                    }
                }
            }
            if (texts.Length == depth) return node as TriStateTreeNode;
            else return null;
        }

       
        int BeforeClickedRow = -1;
        int BeforeClickedEnd = -1;
        TriStateTreeNode BeforeClickedParent = null;
        List<TriStateTreeNode> Added = new List<TriStateTreeNode>();
        List<TriStateTreeNode> Removed = new List<TriStateTreeNode>();

        void CheckBoxClicked(TriStateTreeNode treeNode)
        {

            //L_Title.Content = c.Name;
            //MessageBox.Show(c.Name);
            
            if (treeNode.Nodes.Count == 0 && treeNode.Parent != null) //endNode
            {
                if (BeforeClickedParent != treeNode.Parent)
                {
                    BeforeClickedParent = null;
                    BeforeClickedRow = -1;
                }

                int row_index = treeNode.Parent.Nodes.IndexOf(treeNode);

                if ((Control.ModifierKeys == Keys.Shift))
                {
                    if (BeforeClickedRow >= 0)
                    {
                        for (int i = 0; i < Added.Count; i++)
                        {
                            setEndNodesChecked(Added[i], false, null, null); //이전 것을 모두 undo한다.
                        }
                        Added.Clear();
                        int min = (BeforeClickedRow < row_index) ? BeforeClickedRow : row_index;
                        int max = (BeforeClickedRow < row_index) ? row_index : BeforeClickedRow;

                        for (int i = min; i <= max; i++)
                        {
                            TriStateTreeNode sibling = treeNode.Parent.Nodes[i] as TriStateTreeNode;
                            setEndNodesChecked(sibling, true, Added, Removed);

                        }

                        if (E_OnEndNodeChecked != null && _isEventEnabled) E_OnEndNodeChecked(this, new RtwTreeNodeCheckedEventArg(_checkedNodes, Added, Removed));
                        Added.Clear();
                        Removed.Clear();
                        BeforeClickedRow = -1;
                    }
                    else
                    {
                        BeforeClickedRow = row_index;
                        BeforeClickedParent = treeNode.Parent as TriStateTreeNode;
                        Added.Add(treeNode.Parent.Nodes[row_index]);
                    }
                }
                else if (ModifierKeys == Keys.Alt)
                {
                    if (BeforeClickedRow >= 0)
                    {
                        for (int i = 0; i < Removed.Count; i++)
                        {
                            setEndNodesChecked(Removed[i], true, null, null); //이전 것을 모두 undo한다.
                        }
                        Removed.Clear();
                        int min = (BeforeClickedRow < row_index) ? BeforeClickedRow : row_index;
                        int max = (BeforeClickedRow < row_index) ? row_index : BeforeClickedRow;
                        
                        for (int i = min; i <= max; i++)
                        {
                            TriStateTreeNode child = treeNode.Parent.Nodes[i] as TriStateTreeNode;
                            setEndNodesChecked(child, false, Added, Removed);
                        }
                        if (E_OnEndNodeChecked != null && _isEventEnabled) E_OnEndNodeChecked(this, new RtwTreeNodeCheckedEventArg(_checkedNodes, Added, Removed));
                        Added.Clear();
                        Removed.Clear();
                        BeforeClickedRow = -1;
                    }
                    else
                    {
                        BeforeClickedRow = row_index;
                        BeforeClickedParent = treeNode.Parent as TriStateTreeNode;
                    }
                }
                else if (ModifierKeys == Keys.Control)
                {
                    BeforeClickedRow = row_index;
                    BeforeClickedParent = treeNode.Parent as TriStateTreeNode;
                    bool? state = treeNode.Checked;
                    setEndNodesChecked(treeNode, (state!=true), Added, Removed);

                    if (E_OnEndNodeChecked != null && _isEventEnabled) E_OnEndNodeChecked(this, new RtwTreeNodeCheckedEventArg(_checkedNodes, Added, Removed));
                    Added.Clear();
                    Removed.Clear();
                }
                else
                {
                    bool? state = treeNode.Checked;
                    setEndNodesChecked(treeNode, (state!=true), Added, Removed);
                    if (E_OnEndNodeChecked != null && _isEventEnabled) E_OnEndNodeChecked(this, new RtwTreeNodeCheckedEventArg(_checkedNodes, Added, Removed));
                    Added.Clear();
                    Removed.Clear();
                }
                CheckParent(treeNode.Parent);
            }
            else if (treeNode.Nodes.Count == 0) //end node with no parent
            {
                bool? state = treeNode.Checked;
                treeNode.Checked = (state != true);
                if (E_OnEndNodeChecked != null && _isEventEnabled) E_OnEndNodeChecked(this, new RtwTreeNodeCheckedEventArg(_checkedNodes, Added, Removed));
                Added.Clear();
                Removed.Clear();
            }
            else //부모노드일때
            {
                bool? state = treeNode.Checked;
                setEndNodesChecked(treeNode, (state!=true), Added, Removed);
                if (E_OnEndNodeChecked != null && _isEventEnabled) E_OnEndNodeChecked(this, new RtwTreeNodeCheckedEventArg(_checkedNodes, Added, Removed));
                CheckParent(treeNode);
                Added.Clear();
                Removed.Clear();
            }
            
            //RunWhenEndNodeClicked(treeNode);
            //}
            //catch
            //{
            //    _isChecking = false;
            //    return;
            //}
        }

    }


}
