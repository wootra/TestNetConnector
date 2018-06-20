using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Timers;
using System.Windows.Threading;
using WPF_Handler;

namespace RtwWpfControls
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RtwTree :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event TreeNodeClickEventHandler E_OnNodeDoubleClicked;
        public event TreeNodeClickEventHandler E_OnNodeClicked;
        public event TreeNodeClickEventHandler E_OnNodeRightClicked;

        public event TreeNodeCheckedEventHandler E_OnNodeChecked;
        public List<RtwTreeNode> _items = new List<RtwTreeNode>();
        public List<RtwTreeNode> _checked = new List<RtwTreeNode>();
        ClickCounter _clickCounter;
        RtwTreeNode _selectedTreeNode = null;

        delegate void DelFunc();
        String _checkBoxVisibility = Visibility.Hidden.ToString();
        #region contextMenu

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
        public delegate void ContextMenuClickHandler(Object sender, String text, int index, RtwTreeNode selectedRow, object MenuItem);
        public event ContextMenuClickHandler E_ContextMenuParentClicked = null;
        public event ContextMenuClickHandler E_ContextMenuEndClicked = null;

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
            item.Header = text;
            item.Click += new RoutedEventHandler(itemClickedParent);
            if (tooltip != null && tooltip.Length > 0) item.ToolTip = tooltip;
            if (inputGesture != null && inputGesture.Length > 0) item.InputGestureText = inputGesture;
            if (eventHandler != null) _contextMenuParentClickHandlers.Add(item, eventHandler);
            
            baseMenu.Items.Add(item);
        }

        void itemClickedParent(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            int index = ContextMenuItems.IndexOf(item);
            if (_contextMenuParentClickHandlers.ContainsKey(item)) _contextMenuParentClickHandlers[item].Invoke(this, item.Header.ToString(), index, _selectedTreeNode, item);
            else if(E_ContextMenuEndClicked!=null) E_ContextMenuParentClicked(this, item.Header.ToString(), index, _selectedTreeNode, item);
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
            item.Header = text;
            item.Click += new RoutedEventHandler(itemClickedEnd);
            if (tooltip != null && tooltip.Length > 0) item.ToolTip = tooltip;
            if (inputGesture != null && inputGesture.Length > 0) item.InputGestureText = inputGesture;
            if (eventHandler != null) _contextMenuParentClickHandlers.Add(item, eventHandler);

            baseMenu.Items.Add(item);
        }

        void itemClickedEnd(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            int index = ContextMenuItems.IndexOf(item);
            if (_contextMenuEndClickHandlers.ContainsKey(item)) _contextMenuEndClickHandlers[item].Invoke(this, item.Header.ToString(), index, _selectedTreeNode, item);
            else if(E_ContextMenuEndClicked!=null) E_ContextMenuEndClicked(this, item.Header.ToString(), index, _selectedTreeNode, item);
        }

        #endregion

        #region Actions
        public enum Actions { None=0 ,CopyNameToClipBaord=1, OpenChildren=2, CheckBoxClick=4, ContextMenuOpened=8 };

        public Actions ActionOnParentNodeClicked = Actions.CopyNameToClipBaord;
        public Actions ActionOnParentNodeDoubleClicked = Actions.OpenChildren;
        public Actions ActionOnEndNodeClicked = Actions.CopyNameToClipBaord;
        public Actions ActionOnEndNodeDoubleClicked = Actions.CheckBoxClick;
        public Actions ActionOnParentNodeRightClicked = Actions.None;
        public Actions ActionOnEndNodeRightClicked = Actions.None;
        #endregion

        public String CheckBoxVisibility
        {
            get { return _checkBoxVisibility; }
            set
            {
                _checkBoxVisibility = value;
                NotifyPropertyChanged("CheckBoxVisibility");
            }
        }

        
        public List<RtwTreeNode> Items
        {
            get
            { return _items; }
            set
            {
                //VerifyAccess();
                _items = value;
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].E_CheckChanged += new TreeNodeCheckChangedHandler(RtwTree_E_CheckChanged);
                }
                NotifyPropertyChanged("Items");
            }
        }

        public void CheckNodeStates()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[0].CheckSelf();
            }
        }

        public void UnCheckAll()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].CheckRecursive(false, null, null, null);
            }
            _checked.Clear();
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

        void RtwTree_E_CheckChanged(object sender, TreeNodeCheckChangedArgs args)
        {
            if (args.CheckState == true)
            {
                try
                {
                    if(_checked.Contains(args.Node)==false) _checked.Add(args.Node);
                }
                catch { }
            }
            else
            {
                try
                {
                    if(_checked.Contains(args.Node)) _checked.Remove(args.Node);
                }
                catch { }
            }
        }

        public List<RtwTreeNode> CheckedNodes
        {
            get { return _checked; }
        }
        
        public void ClearAllItems()
        {
            Items = new List<RtwTreeNode>();
        }

        public List<RtwTreeNode> Root { get {  return Items; } }
        

        public RtwTree()
        {
            InitializeComponent();

            
            _clickCounter = new ClickCounter(200, this);

            Items = new List<RtwTreeNode>();
            
            CheckBoxVisibility = System.Windows.Visibility.Visible.ToString();

            _clickCounter.OnCountedClick += new CountedClickEventHandler(_clickCounter_OnCountedClick);

            //this.MouseUp += new MouseButtonEventHandler(ListTree_MouseUp);
            
            //PropertyChanged += new PropertyChangedEventHandler(ListTree_PropertyChanged);
            //Test();
        }

        CountedClickEventArgs clickArg;
        void _clickCounter_OnCountedClick(CountedClickEventArgs e)
        {
            DelFunc func = OnClick;
            clickArg = e;
            
            
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(func));
            
        }

        void ListTree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //this.RaiseEvent(new RoutedEventArgs());
        }

        Boolean _isPropertyChecking = false;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                if (_isPropertyChecking) return;
                _isPropertyChecking = true;//무한반복을 막기 위함.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                _isPropertyChecking = false;
            }
        }

        public RtwTreeNode getNodeByName(String NamePath,String seperator=".")
        {
            String[] names = NamePath.Split( seperator.ToCharArray());
            int depth=0;
            RtwTreeNode node = null;
            if(Items.Count>0){
                for(int i=0; i<Items.Count; i++){
                    if(Items[i].Name.Equals(names[depth])){
                        depth++;
                        node = Items[i];
                        break;
                    }
                }
                if(node==null) return null;
            }else return null;
            
            while (node.Children.Count > 0 && names.Length>depth)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    if(node.Children[i].Name.Equals(names[depth])){
                        depth++;
                        node = node.Children[i];
                        break;
                    }
                }
            }
            if (names.Length == depth) return node;
            else return null;
        }

        public RtwTreeNode getNodeByText(String TextPath, String seperator = ".")
        {
            String[] texts = TextPath.Split(seperator.ToCharArray());
            int depth = 0;
            RtwTreeNode node = null;
            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Text.Equals(texts[depth]))
                    {
                        depth++;
                        node = Items[i];
                        break;
                    }
                }
                if (node == null) return null;
            }
            else return null;

            while (node.Children.Count > 0 && texts.Length > depth)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    if (node.Children[i].Text.Equals(texts[depth]))
                    {
                        depth++;
                        node = node.Children[i];
                        break;
                    }
                }
            }
            if (texts.Length == depth) return node;
            else return null;
        }

        void ListTree_MouseUp(object sender, RoutedEventArgs e)
        {
            _clickCounter.CountClick(sender, e);
            //e.Handled = true;
        }
        
        void OnClick() //this function is called by Timer..
        {
            //VerifyAccess();
            


            object sender = clickArg.Sender;
            RoutedEventArgs e = clickArg.MouseBtnArgs;

            ContentPresenter p = null;
            TreeViewItem item = null;
            RtwTreeNode treeNode = null;

            FrameworkElement fe = e.OriginalSource as FrameworkElement;

            if (fe.TemplatedParent is ContentPresenter)
            {
                p = fe.TemplatedParent as ContentPresenter;
            }
            else if (fe.TemplatedParent is TreeViewItem)
            {
                item = fe.TemplatedParent as TreeViewItem;
            }
            else return;
            /*
            if (e.OriginalSource is TextBlock)
            {
                //p = (e.OriginalSource as TextBlock).TemplatedParent as ContentPresenter;
                p = (e.OriginalSource as TextBlock).TemplatedParent as ContentPresenter;

            }
            else if (e.OriginalSource is Rectangle)
            {
                if ((e.OriginalSource as Rectangle).TemplatedParent is ContentPresenter)
                {
                    p = (e.OriginalSource as Rectangle).TemplatedParent as ContentPresenter;
                }
                else
                {
                    item = (e.OriginalSource as Rectangle).TemplatedParent as TreeViewItem;
                }
            }
            else
            {
                return;
            }
            */


            //try
           {

               
                if (item != null)
                {
                    if (item.Items.Count > 0) item.Tag = "Parent";
                    treeNode = item.Header as RtwTreeNode;
                }
                else
                {
                    treeNode = p.Content as RtwTreeNode;
                }

                

                if (treeNode.Enabled)
                {
                    if (clickArg.PressedButton == MouseButton.Left)
                    {
                        if (clickArg.Count == 2) //doubleclicked
                        {

                            if (treeNode.Children.Count > 0)
                            {
                                //(p.Parent as Grid).Children[0].Visibility = System.Windows.Visibility.Hidden;
                                RunWhenParentNodeClicked(treeNode, ActionOnParentNodeDoubleClicked);
                            }
                            else
                            {
                                RunWhenEndNodeClicked(treeNode, ActionOnEndNodeDoubleClicked);
                            }



                            if (E_OnNodeDoubleClicked != null && _isEventEnabled)
                            {

                                E_OnNodeDoubleClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));
                            }
                        }
                        else if (clickArg.Count == 1)
                        {
                            
                            if (treeNode.Children.Count > 0)
                            {
                                //(p.Parent as Grid).Children[0].Visibility = System.Windows.Visibility.Hidden;
                                RunWhenParentNodeClicked(treeNode, ActionOnParentNodeClicked);
                            }
                            else
                            {
                                RunWhenEndNodeClicked(treeNode, ActionOnEndNodeClicked);
                            }

                            if (E_OnNodeClicked != null && _isEventEnabled)
                            {

                                E_OnNodeClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));
                            }
                        }
                        else
                        {
                        }
                    }
                    else if (clickArg.PressedButton == MouseButton.Right)
                    {
                        if (clickArg.Count == 1)
                        {
                            
                            if (treeNode.Children.Count > 0)
                            {
                                //(p.Parent as Grid).Children[0].Visibility = System.Windows.Visibility.Hidden;
                                RunWhenParentNodeClicked(treeNode, ActionOnParentNodeDoubleClicked);
                            }
                            else
                            {
                                RunWhenEndNodeClicked(treeNode, ActionOnEndNodeDoubleClicked);
                            }

                            if (E_OnNodeRightClicked != null && _isEventEnabled)
                            {

                                E_OnNodeRightClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));
                            }
                        }
                        else
                        {
                        }
                    }

                }
            }
            //catch { }

        }
        protected virtual void RunWhenParentNodeClicked(RtwTreeNode parentNode, Actions action) {
            //addItemForTest(); //forTest
        
            if ((action & Actions.CheckBoxClick) > 0)
            {
                //endNode.Checked = endNode.Checked;
                if (parentNode.Checked == true)
                {
                    parentNode.Checked = false;
                }
                else if (parentNode.Checked == false)
                {
                    parentNode.Checked = true;
                }
                else //null
                {
                    parentNode.Checked = true;
                }
            }
            if ((action & Actions.ContextMenuOpened) > 0)
            {
                _selectedTreeNode = parentNode;
                U_ContextMenuParentNode.HorizontalOffset = 10;
                U_ContextMenuParentNode.VerticalOffset = 10;

                U_ContextMenuParentNode.IsOpen = true;
            }
            if ((action & Actions.CopyNameToClipBaord) > 0)
            {
                System.Windows.Clipboard.SetDataObject(parentNode.Text, false); //클릭시 저장함.
                            
            }
            if ((action & Actions.OpenChildren) > 0)
            {
                
            }
        
        }
        protected virtual void RunWhenEndNodeClicked(RtwTreeNode endNode, Actions action)
        {
            
            if ((action & Actions.CheckBoxClick)>0)
            {
                //endNode.Checked = endNode.Checked;
                if (endNode.Checked == true)
                {
                    endNode.Checked = false;
                }
                else if (endNode.Checked == false)
                {
                    endNode.Checked = true;
                }
                else //null
                {
                    endNode.Checked = true;
                }
            }
            if ((action & Actions.ContextMenuOpened)>0)
            {
                _selectedTreeNode = endNode;
                U_ContextMenuParentNode.HorizontalOffset = 10;
                U_ContextMenuParentNode.VerticalOffset = 10;

                U_ContextMenuParentNode.IsOpen = true;
            }
            if ((action & Actions.CopyNameToClipBaord)>0)
            {
                System.Windows.Clipboard.SetDataObject(endNode.Text, false); //클릭시 저장함.
                            
            }
            
            
            //if (endNode.Parent != null)
             //   endNode.Parent.CheckSelf();
        }
        /*
        Boolean _mouseOverDispatched = false;
        void MouseEnter(Object sender, RoutedEventArgs e)
        {
            _clickSender = sender;
            _clickEventArg = e;
            DelFunc func = OnMouseOver;
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(func));
            if (_mouseOverDispatched == false) OnMouseOver();
            _mouseOverDispatched = false;
        }

        void OnMouseOver()
        {
            object sender = _clickSender;
            RoutedEventArgs e = _clickEventArg;

            ContentPresenter p = null;
            TreeViewItem item = null;
            TreeNode up = null;
            try
            {
                if (e.OriginalSource is TextBlock)
                {
                    //p = (e.OriginalSource as TextBlock).TemplatedParent as ContentPresenter;
                    p = (e.OriginalSource as TextBlock).TemplatedParent as ContentPresenter;
                }
                else if (e.OriginalSource is Rectangle)
                {
                    if ((e.OriginalSource as Rectangle).TemplatedParent is ContentPresenter)
                    {
                        p = (e.OriginalSource as Rectangle).TemplatedParent as ContentPresenter;
                    }
                    else
                    {
                        item = (e.OriginalSource as Rectangle).TemplatedParent as TreeViewItem;
                    }
                }
                else if(e.OriginalSource is TreeViewItem)
                {
                    item = e.OriginalSource as TreeViewItem;
                }else if(e.OriginalSource is TreeView){
                    TreeView view = (e.OriginalSource as TreeView);
                }else{
                    return;
                }
                if (item != null)
                {
                    up = item.Header as TreeNode;
                    (item.Parent as Grid).Children[0].Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    up = p.Content as TreeNode;
                    (p.Parent as Grid).Children[0].Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch
            {

            }
            finally
            {
                _mouseOverDispatched = true;
            }
        }
        */



       

        void addItemForTest()
        {
                //VerifyAccess();
                List<RtwTreeNode> newlist = new List<RtwTreeNode>(this.Items);
                for (int j = 0; j < 1; j++)
                {
                    RtwTreeNode model = new RtwTreeNode("Tree" + j,false);
                    for (int i = 0; i < 2; i++)
                    {
                        model.AddChild(new RtwTreeNode("TreeNode" + i, false));
                    }

                    newlist[0].AddChild(model);
                    

                }
                this.Items = newlist;
            

        }

  

        void Test()
        {

            VerifyAccess();
            RtwTreeNode gst = new RtwTreeNode("pGST", false);
            for (int j = 0; j < 4; j++)
            {
                RtwTreeNode model = new RtwTreeNode("model" + j,false);
                for (int i = 0; i < 4; i++)
                {
                    model.AddChild(new RtwTreeNode("var" + i, false));
                }
                gst.AddChild(model);
                
            }
            this.Items.Add(gst);

        }

        private void MouseUpPreview(object sender, RoutedEventArgs e)
        {
            _clickCounter.CountClick(sender, e);
        }

        Boolean _isChecking = false;
        int BeforeClickedRow = -1;
        RtwTreeNode BeforeClickedParent = null;
        void CheckBoxClicked(object sender, RoutedEventArgs e)
        {
             VerifyAccess();
            if (_isChecking || RtwTreeNode.IsChecking()) return;
            _isChecking = true; //무한루프를 막기위함..
            CheckBox c = sender as CheckBox;
            ContentPresenter p = c.TemplatedParent as ContentPresenter;
            RtwTreeNode treeNode;
            TreeViewItem item = null;// = WpfFinder.getParentFromTemplate(c, DependencyObjectType.FromSystemType(typeof(TreeViewItem))) as TreeViewItem;
            //try
            //{
                if (p == null)
                {
                    item = c.TemplatedParent as TreeViewItem;
                    treeNode = item.Header as RtwTreeNode;
                }
                else
                {
                    treeNode = p.Content as RtwTreeNode;
                }
                /*
                if (treeNode.Enabled && treeNode.Children.Count > 0)
                {
                    if (treeNode.Checked != null)
                    {
                        treeNode.CheckRecursive(c.IsChecked);
                    }
                    else
                    {
                        treeNode.CheckRecursive(true);
                    }
                    //if (treeNode.Parent != null) treeNode.Parent.CheckSelf();
                }
                */



                //L_Title.Content = c.Name;
                //MessageBox.Show(c.Name);
                List<RtwTreeNode> Added = new List<RtwTreeNode>();
                List<RtwTreeNode> Removed = new List<RtwTreeNode>();
                List<RtwTreeNode> Selected = new List<RtwTreeNode>();

                if (treeNode.Children.Count == 0 && treeNode.Parent != null) //endNode
                {
                    if (BeforeClickedParent != treeNode.Parent)
                    {
                        BeforeClickedParent = null;
                        BeforeClickedRow = -1;
                    }

                    int row_index = treeNode.Parent.IndexOfChild(treeNode);
                    if (E_OnNodeDoubleClicked != null && treeNode.Enabled == true && _isEventEnabled) E_OnNodeDoubleClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));


                    if ((System.Windows.Input.Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) == KeyStates.Down)
                    {
                        if (BeforeClickedRow >= 0)
                        {
                            int min = (BeforeClickedRow < row_index) ? BeforeClickedRow : row_index;
                            int max = (BeforeClickedRow < row_index) ? row_index : BeforeClickedRow;


                            for (int i = min; i <= max; i++)
                            {
                                RtwTreeNode child = treeNode.Parent.Children[i];
                                if (child.Enabled)
                                {
                                    if (E_OnNodeChecked != null && _isEventEnabled && child.Checked != true) Added.Add(child);

                                    treeNode.Parent.Children[i].Checked = true;

                                }

                            }
                            if (E_OnNodeChecked != null && _isEventEnabled) Selected = treeNode.SelectedSiblings;
                            if (E_OnNodeChecked != null && _isEventEnabled) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
                        }
                    }
                    else if ((System.Windows.Input.Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) == KeyStates.Down)
                    {
                        if (BeforeClickedRow >= 0)
                        {
                            int min = (BeforeClickedRow < row_index) ? BeforeClickedRow : row_index;
                            int max = (BeforeClickedRow < row_index) ? row_index : BeforeClickedRow;

                            for (int i = min; i <= max; i++)
                            {
                                RtwTreeNode child = treeNode.Parent.Children[i];
                                if (child.Enabled)
                                {
                                    if (E_OnNodeChecked != null && _isEventEnabled && child.Checked != false) Removed.Add(child);

                                    treeNode.Parent.Children[i].Checked = false;

                                }
                            }
                            if (E_OnNodeChecked != null && _isEventEnabled) Selected = treeNode.SelectedSiblings;
                            if (E_OnNodeChecked != null && _isEventEnabled) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
                        }
                    }
                    else
                    {

                        BeforeClickedRow = row_index;
                        BeforeClickedParent = treeNode.Parent;
                        if (treeNode.Checked == true) Added.Add(treeNode);
                        else if (treeNode.Checked == false) Removed.Add(treeNode);
                        if (E_OnNodeChecked != null && _isEventEnabled) Selected = treeNode.SelectedSiblings;
                        if (E_OnNodeChecked != null && _isEventEnabled) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
                    }
                }
                else //부모노드일때
                {
                    treeNode.CheckRecursive(treeNode.Checked, Added, Removed, Selected);
                    if (E_OnNodeChecked != null && _isEventEnabled) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
                }
                treeNode.CheckSelf();
                //RunWhenEndNodeClicked(treeNode);
            //}
            //catch
            //{
            //    _isChecking = false;
            //    return;
            //}

            _isChecking = false;
        }
        
        private void EventTrigger_Selected(object sender, RoutedEventArgs e)
        {
            VerifyAccess();
            TreeViewItem item = sender as TreeViewItem;
            RtwTreeNode tr = item.Header as RtwTreeNode;
            tr.Checked = true;
        }

       
    }
}
