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
    public partial class CustomListTree :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public List<RtwTreeNode> _items = new List<RtwTreeNode>();
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
        
        public List<RtwTreeNode> Items
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
        public event TreeNodeClickEventHandler OnNodeClicked;



        public CustomListTree()
        {
            InitializeComponent();

            _clickCounter = new ClickCounter(200, this);

            Items = new List<RtwTreeNode>();
            CheckBoxVisibility = System.Windows.Visibility.Hidden.ToString();

            _clickCounter.OnCountedClick += new CountedClickEventHandler(_clickCounter_OnCountedClick);

            //this.MouseUp += new MouseButtonEventHandler(ListTree_MouseUp);
            
            //PropertyChanged += new PropertyChangedEventHandler(ListTree_PropertyChanged);
            Test();
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

                if (treeNode.Children.Count > 0)
                {
                    //(p.Parent as Grid).Children[0].Visibility = System.Windows.Visibility.Hidden;
                    RunWhenParentNodeClicked(treeNode);
                }
                else
                {
                    RunWhenEndNodeClicked(treeNode);
                }
                
                

                if (OnNodeClicked != null) OnNodeClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));

            }
            //catch { }

        }
        protected virtual void RunWhenParentNodeClicked(RtwTreeNode parentNode) { }
        protected virtual void RunWhenEndNodeClicked(RtwTreeNode endNode)
        {
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

                    newlist.Add(model);

                }
                this.Items = newlist;
            

        }

  

        void Test()
        {

            VerifyAccess();
            for (int j = 0; j < 4; j++)
            {
                RtwTreeNode model = new RtwTreeNode("model" + j,false);
                for (int i = 0; i < 4; i++)
                {
                    model.AddChild(new RtwTreeNode("var" + i, false));
                }

                this.Items.Add(model);
            }


        }

        private void MouseUpPreview(object sender, MouseButtonEventArgs e)
        {
            _clickCounter.CountClick(sender, e);
        }

        Boolean _isChecking = false;
        private void List_Check_Checked(object sender, RoutedEventArgs e)
        {
            VerifyAccess();
            if (_isChecking) return;
            _isChecking = true; //무한루프를 막기위함..
            CheckBox c = sender as CheckBox;
            ContentPresenter p = c.TemplatedParent as ContentPresenter;
            RtwTreeNode treeNode;
            TreeViewItem item = null;
            try
            {
                if (p == null)
                {
                    item = c.TemplatedParent as TreeViewItem;
                    treeNode = item.Header as RtwTreeNode;

                    for (int i = 0; i < treeNode.Children.Count; i++)
                    {
                        treeNode.Children[i].Checked = true;
                        
                    }
                    //if (treeNode.Parent != null) treeNode.Parent.CheckSelf();
                }
                else
                {
                    treeNode = p.Content as RtwTreeNode;

                    for (int i = 0; i < treeNode.Children.Count; i++)
                    {
                        treeNode.Children[i].Checked = true;
                    }
                    //if (treeNode.Parent != null) treeNode.Parent.CheckSelf();
                }
            }
            catch
            {
                _isChecking = false;
                return;
            }

            _isChecking = false;
            
            //L_Title.Content = c.Name;
            //MessageBox.Show(c.Name);
            
            if (OnNodeClicked != null) OnNodeClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));
        }
        private void List_Check_UnChecked(object sender, RoutedEventArgs e)
        {
            VerifyAccess();
            if (_isChecking) return;
            _isChecking = true; //무한루프를 막기위함..
            CheckBox c = sender as CheckBox;
            ContentPresenter p = c.TemplatedParent as ContentPresenter;
            TreeViewItem item=null;
            RtwTreeNode treeNode;
            try
            {
                if (p == null)
                {
                    item = c.TemplatedParent as TreeViewItem;

                    treeNode = item.Header as RtwTreeNode;

                    for (int i = 0; i < treeNode.Children.Count; i++)
                    {
                        treeNode.Children[i].Checked = false;
                    }
                    //if (treeNode.Parent != null) treeNode.Parent.CheckSelf();

                }
                else
                {
                    treeNode = p.Content as RtwTreeNode;

                    for (int i = 0; i < treeNode.Children.Count; i++)
                    {
                        treeNode.Children[i].Checked = false;
                    }
                    //if (treeNode.Parent != null) treeNode.Parent.CheckSelf();

                }
            }
            catch {
                _isChecking = false;
                return;
            }
            //L_Title.Content = c.Name;
            //MessageBox.Show(c.Name);
            _isChecking = false;
            if (OnNodeClicked != null) OnNodeClicked(this, new TreeNodeClickEventArg(treeNode.Path, treeNode.IndexPath, treeNode.Checked, treeNode, item, p));
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
