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

namespace ListViews
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ListTree :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event TreeNodeClickEventHandler E_OnNodeClicked;
        public event TreeNodeCheckedEventHandler E_OnNodeChecked;
        public List<TreeTableNode> _items = new List<TreeTableNode>();
        ClickCounter _clickCounter;
        delegate void DelFunc();
        String _checkBoxVisibility = Visibility.Hidden.ToString();

        public String CheckBoxVisibility
        {
            get { return _checkBoxVisibility; }
            set
            {
                _checkBoxVisibility = value;
                NotifyPropertyChanged("CheckBoxVisibility");
            }
        }
        
        public List<TreeTableNode> Items
        {
            get
            { return _items; }
            set
            {
                //VerifyAccess();
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }
        
        public void ClearAllItems()
        {
            Items = new List<TreeTableNode>();
        }

        public List<TreeTableNode> Root { get {  return Items; } }
        

        public ListTree()
        {
            InitializeComponent();

            
            _clickCounter = new ClickCounter(200, this);

            Items = new List<TreeTableNode>();
            
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

        public TreeTableNode getNodeByName(String NamePath,String seperator=".")
        {
            String[] names = NamePath.Split( seperator.ToCharArray());
            int depth=0;
            TreeTableNode node = null;
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

        public TreeTableNode getNodeByText(String TextPath, String seperator = ".")
        {
            String[] texts = TextPath.Split(seperator.ToCharArray());
            int depth = 0;
            TreeTableNode node = null;
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
            if (clickArg.Count != 2) return;



            object sender = clickArg.Sender;
            RoutedEventArgs e = clickArg.MouseBtnArgs;

            ContentPresenter p = null;
            TreeViewItem item = null;
            TreeTableNode treeNode = null;

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
                    treeNode = item.Header as TreeTableNode;
                }
                else
                {
                    treeNode = p.Content as TreeTableNode;
                }

                if (treeNode.Enabled)
                {
                    if (treeNode.Children.Count > 0)
                    {
                        //(p.Parent as Grid).Children[0].Visibility = System.Windows.Visibility.Hidden;
                        RunWhenParentNodeClicked(treeNode);
                    }
                    else
                    {
                        RunWhenEndNodeClicked(treeNode);
                    }



                    if (E_OnNodeClicked != null && treeNode.Enabled)
                    {

                        E_OnNodeClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));
                    }
                }
            }
            //catch { }

        }
        protected virtual void RunWhenParentNodeClicked(TreeTableNode parentNode) {
            //addItemForTest(); //forTest
        }
        protected virtual void RunWhenEndNodeClicked(TreeTableNode endNode)
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
                List<TreeTableNode> newlist = new List<TreeTableNode>(this.Items);
                for (int j = 0; j < 1; j++)
                {
                    TreeTableNode model = new TreeTableNode("Tree" + j,false);
                    for (int i = 0; i < 2; i++)
                    {
                        model.AddChild(new TreeTableNode("TreeNode" + i, false));
                    }

                    newlist[0].AddChild(model);
                    

                }
                this.Items = newlist;
            

        }

  

        void Test()
        {

            VerifyAccess();
            TreeTableNode gst = new TreeTableNode("pGST", false);
            for (int j = 0; j < 4; j++)
            {
                TreeTableNode model = new TreeTableNode("model" + j,false);
                for (int i = 0; i < 4; i++)
                {
                    model.AddChild(new TreeTableNode("var" + i, false));
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
        TreeTableNode BeforeClickedParent = null;
        void CheckBoxClicked(object sender, RoutedEventArgs e)
        {
             VerifyAccess();
            if (_isChecking || TreeTableNode.IsChecking()) return;
            _isChecking = true; //무한루프를 막기위함..
            CheckBox c = sender as CheckBox;
            ContentPresenter p = c.TemplatedParent as ContentPresenter;
            TreeTableNode treeNode;
            TreeViewItem item = null;// = WpfFinder.getParentFromTemplate(c, DependencyObjectType.FromSystemType(typeof(TreeViewItem))) as TreeViewItem;
            //try
            //{
                if (p == null)
                {
                    item = c.TemplatedParent as TreeViewItem;
                    treeNode = item.Header as TreeTableNode;
                }
                else
                {
                    treeNode = p.Content as TreeTableNode;
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
                List<TreeTableNode> Added = new List<TreeTableNode>();
                List<TreeTableNode> Removed = new List<TreeTableNode>();
                List<TreeTableNode> Selected = new List<TreeTableNode>();

                if (treeNode.Children.Count == 0 && treeNode.Parent != null) //endNode
                {
                    if (BeforeClickedParent != treeNode.Parent)
                    {
                        BeforeClickedParent = null;
                        BeforeClickedRow = -1;
                    }

                    int row_index = treeNode.Parent.IndexOfChild(treeNode);
                    if (E_OnNodeClicked != null && treeNode.Enabled == true) E_OnNodeClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));


                    if ((System.Windows.Input.Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) == KeyStates.Down)
                    {
                        if (BeforeClickedRow >= 0)
                        {
                            int min = (BeforeClickedRow < row_index) ? BeforeClickedRow : row_index;
                            int max = (BeforeClickedRow < row_index) ? row_index : BeforeClickedRow;


                            for (int i = min; i <= max; i++)
                            {
                                TreeTableNode child = treeNode.Parent.Children[i];
                                if (child.Enabled)
                                {
                                    if (E_OnNodeChecked != null && child.Checked != true) Added.Add(child);

                                    treeNode.Parent.Children[i].Checked = true;

                                }

                            }
                            if (E_OnNodeChecked != null) Selected = treeNode.SelectedSiblings;
                            if (E_OnNodeChecked != null) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
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
                                TreeTableNode child = treeNode.Parent.Children[i];
                                if (child.Enabled)
                                {
                                    if (E_OnNodeChecked != null && child.Checked != false) Removed.Add(child);

                                    treeNode.Parent.Children[i].Checked = false;

                                }
                            }
                            if (E_OnNodeChecked != null) Selected = treeNode.SelectedSiblings;
                            if (E_OnNodeChecked != null) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
                        }
                    }
                    else
                    {

                        BeforeClickedRow = row_index;
                        BeforeClickedParent = treeNode.Parent;
                        if (treeNode.Checked == true) Added.Add(treeNode);
                        else if (treeNode.Checked == false) Removed.Add(treeNode);
                        if (E_OnNodeChecked != null) Selected = treeNode.SelectedSiblings;
                        if (E_OnNodeChecked != null) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
                    }
                }
                else //부모노드일때
                {
                    treeNode.CheckRecursive(treeNode.Checked, Added, Removed, Selected);
                    if (E_OnNodeChecked != null) E_OnNodeChecked(this, new TreeNodeCheckedEventArg(Selected, Added, Removed));
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
            TreeTableNode tr = item.Header as TreeTableNode;
            tr.Checked = true;
        }

       
    }
}
