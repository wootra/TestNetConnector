using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DockingActions;
using DataHandling;

namespace DockingActions
{
    public partial class DockingContainer : ScrollableControl,IDisposable
    {
        protected SplitContainer _sContainer = new SplitContainer();
        
        protected DockingRoot _root;
        protected List<DockingContainer> _children = new List<DockingContainer>();
        protected Dictionary<String,int> _childId = new Dictionary<String,int>();
        private TabControl _tabContainer = new TabControl();
        
        protected DockingContainer _parent;
        protected TitleBar _titleBar;
        
        internal Control Content; //초기 구성은 _titleBar 와 _contents이다.
        public TabControl TabContainer { get { return _tabContainer; } }//for test public
        internal Dictionary<String,int> ChildId { get { return _childId; } }
        protected const int TITLE_BAR_HEIGHT = 18;
        protected DraggingPopup _draggingPopup;
        protected internal int _id;
        protected DockStyle _initDock;
        public enum ContainerType { SingleContent = 0, TabContents = 1, SplitContents = 2 };
        public ContainerType _type = ContainerType.SingleContent;
        
        private Boolean _isInPopup = false;
        public DockingContainer(DockingRoot root, DockingContainer parent, String name, Control contents, int id, DockStyle initDock):base()
        {

            if (root != null) _root = root;
            else _root = this as DockingRoot;
            _initDock = initDock;
            _parent = parent;
            Content = contents;
            this.Name = name;
            _draggingPopup = new DraggingPopup(_root, null); //평소에는 숨어있다가 Drag할 때에만 나타난다.
            _titleBar = new TitleBar(this, _draggingPopup, _root);
            InitializeComponent();

            
            this.Width = Content.Width;
            this.Height = Content.Height;
            this.Location = new Point(0, 0);
            this._tabContainer.SetBounds(0, 0, this.Width, this.Height);
            this._tabContainer.Alignment = TabAlignment.Bottom;
            this._tabContainer.SelectedIndexChanged += new EventHandler(_tabContainer_SelectedIndexChanged);
            this._sContainer.SetBounds(0, 0, this.Width, this.Height);
            _id = id;
            this.DoubleBuffered = true;

            // _draggingPopup.MdiParent = _root.getMdiForm();
           
            
            //if(Content!=null) Content.Resize += new EventHandler(_contents_Resize);


            this.SuspendLayout();

            titleBarInitSetting();

            Content.Location = new Point(0, _titleBar.Height);
            contents.Dock = DockStyle.Fill;
            this.Controls.Add(contents);
            
            this.ResumeLayout();

            showTitleBar(true, name);
            
            this.Dock = DockStyle.Fill;
            if(_root!=this) _root.Connect(this.Name, this);
            

            _tabContainer.TabIndexChanged += new EventHandler(_tabContainer_TabIndexChanged);
        }

        protected void setContainerType(ContainerType type)
        {
            if (this.Name == "b")
            {
                 int a = 1;
            }
            _type = type;
        }

        internal ContainerType getContainerType() { return _type; }

        void _tabContainer_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tab = (TabControl)sender;
            _titleBar.Text = tab.SelectedTab.Name;
           // this.Name = _titleBar.Text;
            MessageBox.Show("titleName:" + tab.SelectedTab.Name + "/"+_titleBar.Text);
        }

        internal DockStyle initDock() { return _initDock; }

        void titleBarInitSetting()
        {
            _titleBar.Dock = DockStyle.Top;
            //_titleBar.U_DragBegin += new MouseEventHandler(_titleBar_U_DragBegin);
            //_titleBar.U_DragEnd += new MouseEventHandler(_titleBar_U_DragEnd);
            //_titleBar.U_Moving += new MouseEventHandler(_titleBar_U_Moving);
            
            _titleBar.Width = this.Width;
            this.Controls.Add(_titleBar);
            
        }

        internal void setNowInPopup(Boolean isInPopupForm)
        {
            _isInPopup = isInPopupForm;
        }

        internal Boolean isInPopup()
        {
            return _isInPopup;
        }

        internal int getChildWidth(DockingContainer child)
        {
            if (this.getContainerType() == ContainerType.TabContents)
            {
                
                return this._tabContainer.Width;
            }
            else if (this.getContainerType() == ContainerType.SplitContents)
            {
                if (child == _sContainer.Panel1.Controls[0])
                {
                    return _sContainer.SplitterDistance;
                }
                else
                {
                    if (_sContainer.Orientation == Orientation.Vertical)
                    {
                        return _sContainer.Width - _sContainer.SplitterDistance;
                    }
                    else
                    {
                        return _sContainer.Height - _sContainer.SplitterDistance;
                    }
                }
            }
            else
            {
                //this is not possible
                return this.Width;
            }
        }

        void _tabContainer_TabIndexChanged(object sender, EventArgs e)
        {
            _titleBar.Text = ((TabPage)sender).Text;
        }

        internal Boolean isHotArea(int x, int y)
        {
            return CoodinateHandling.isEntered(this, x, y);
        }

        internal void PopThisFromParent()
        {
            if (_parent != null) _parent.PopChildFromThis(this);
        }


        protected delegate void WhenBlankContent();
        protected WhenBlankContent OnBlankContent = null;
        protected virtual void PopChildFromThis(DockingContainer child)
        {
            //this.SuspendLayout();
            if (this.getContainerType() == ContainerType.SplitContents)
            {
                DockingContainer rest;
                if (_sContainer.Panel1.Controls.Count==0 || (_sContainer.Panel1.Controls[0] as DockingContainer) == child)
                {
                    
                    rest = _sContainer.Panel2.Controls[0] as DockingContainer;
                    _sContainer.Panel1.Controls.Clear();
                }
                else
                {
                    
                    
                    rest = _sContainer.Panel1.Controls[0] as DockingContainer;
                    _sContainer.Panel2.Controls.Clear();
                    
                }
                
                this.Controls.Remove(_sContainer);
                //_sContainer.Dispose();

                
                if (rest.getContainerType() == ContainerType.SingleContent)
                {
                    Content = rest.Content;
                    //rest.Content = null;
                    Content.Dock = DockStyle.Fill;
                    this.Controls.Add(Content);
                    this._id = rest._id;
                    _children.Clear();
                    _childId.Clear();
                    this.Name = rest.Name;
                    showTitleBar(true, rest.Name);
                    _root.Disconnect(rest.Name); //기존의 rest는 사라질 것이므로 연결을 끊고
                    _root.Connect(rest.Name, this); //rest의 모든 내용을 계승한 this를 대신 연결한다.

                    this.setContainerType(ContainerType.SingleContent);
                }
                else if (rest.getContainerType() == ContainerType.TabContents)
                {
                    //_tabContainer.Dispose();
                    _tabContainer = rest._tabContainer;
                    _tabContainer.SetBounds(0, 0, this.Width, this.Height-TITLE_BAR_HEIGHT);
                    _tabContainer.Dock = DockStyle.Bottom;

                    //rest._tabContainer = null;
                    _children=rest._children;
                    //rest._children = null;
                    _childId = rest._childId;
                    //rest._childId = null;
                    Content = _tabContainer;
                    this.Controls.Add(_tabContainer);

                    this.Name = rest.Name;
                    showTitleBar(true, rest.Name);

                    _root.Disconnect(rest.Name); //기존의 rest는 사라질 것이므로 연결을 끊고
                    _root.Connect(rest.Name, this); //rest의 모든 내용을 계승한 this를 대신 연결한다.

                    setContainerType(ContainerType.TabContents);
                }
                else //splitContainer - splitContainer는 _root와 연결되어 있지 않으므로 connection을 끊을 필요가 없다.
                {
                    _sContainer = rest._sContainer;
                    _sContainer.Width = this.Width;
                    _sContainer.Height = this.Height;
                    _sContainer.Dock = DockStyle.Fill;
                    //rest._sContainer = null;
                    _children = rest._children;
                    //rest._children = null;
                    _childId = rest._childId;
                    //rest._childId = null;
                    Content = _sContainer;
                    this.Controls.Add(rest._sContainer);
                    //showTitleBar(true, rest.Name);

                    setContainerType(ContainerType.SplitContents);
                }
               
                
            }
            else // ContainerType 이 SingleType일 때이다.
            {
                this.Controls.Remove(child);
                this.Content = null;
                if (OnBlankContent!=null) OnBlankContent();
                setContainerType( ContainerType.SingleContent);
            }
           // this.ResumeLayout();
            //child.Dispose(true);
        }
        internal void hidePopup()
        {
            _draggingPopup.Hide();
        }

        internal void removeChildFromTab(TabPage child)
        {
            String selectedTab="";
            for(int i=0; i<_tabContainer.TabPages.Count; i++){
                if (_tabContainer.TabPages[i] == child)
                {
                    selectedTab = _tabContainer.TabPages[i].Name;
                    break;
                }
            }
            if (selectedTab.Length > 0)
            {
                _tabContainer.TabPages.RemoveByKey(selectedTab);
                _childId.Remove(selectedTab);
            }

            if (_tabContainer.TabPages.Count < 2)
            {
                setContainerType(ContainerType.SingleContent);
                _id = _childId[_tabContainer.SelectedTab.Name];
                Name = _tabContainer.SelectedTab.Name;
                _childId.Clear();
                Content = _tabContainer.TabPages[0].Controls[0];
                _tabContainer.TabPages[0].Controls.RemoveAt(0);
                _tabContainer.TabPages.Clear();
                this.Controls.Remove(_tabContainer);
                this.Controls.Add(Content);
            }
        }

        void setChildSize(int width, int height)
        {
            if (this.getContainerType() == ContainerType.TabContents)
            {
                _titleBar.Height = TITLE_BAR_HEIGHT;
                _tabContainer.SetBounds(0, TITLE_BAR_HEIGHT, width, height-TITLE_BAR_HEIGHT);
                
                foreach (TabPage c in _tabContainer.TabPages)
                {
                    c.SetBounds(0, 0, width, height-TITLE_BAR_HEIGHT);
                    c.Controls[0].SetBounds(0, 0, width, height - TITLE_BAR_HEIGHT);
                    //c.setChildSize(width, height);
                }
                
                _tabContainer.Height = height;
            }
            else if (this.getContainerType() == ContainerType.SplitContents)
            {
                _titleBar.Height = 0;
                if (_children[0].getContainerType() == ContainerType.SingleContent)
                {
                    if (_sContainer.Orientation == Orientation.Horizontal)
                    {
                        _sContainer.SplitterDistance = height - (_sContainer.Height - _sContainer.SplitterDistance);
                    }
                    else
                    {
                        _sContainer.SplitterDistance = width - (_sContainer.Width - _sContainer.SplitterDistance);
                    }
                }
                else 
                {
                    if (_sContainer.Orientation == Orientation.Horizontal)
                    {
                        _children[0].setChildSize(width, height - (_sContainer.Height - _sContainer.SplitterDistance));
                    }
                    else
                    {
                        _children[0].setChildSize(width - (_sContainer.Width - _sContainer.SplitterDistance),height);
                    }
                }
               
                this.Controls.Add(_sContainer);
                this.Width = width;
                this.Height = height;
            }
            else
            {
                _titleBar.Height = TITLE_BAR_HEIGHT;
                Content.SetBounds(0, 0, width, height);
                this.Width = width;
                this.Height = height;
            }
        }

        protected void showTitleBar(Boolean isOn, String name)
        {
                if (isOn)
                {
                    _titleBar.SetBounds(0, 0, this.Width, TITLE_BAR_HEIGHT);
                    _titleBar.Show();
                    Content.SetBounds(0, TITLE_BAR_HEIGHT, this.Width, this.Height - TITLE_BAR_HEIGHT);
                }
                else
                {
                    _titleBar.SetBounds(0, 0, this.Width, 0);
                    _titleBar.Hide();
                    Content.SetBounds(0, 0, this.Width, this.Height);
                }
                _titleBar.Text = name;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            //setChildSize(this.Width, this.Height);
            Point pt = this.PointToScreen(new Point(0,0));
            //_draggingPopup.SetBounds(pt.X, pt.Y, this.Width, this.Height);
        }


        public virtual void addChild(DockingContainer child, DockStyle dock, Boolean isRemainSize = true){
            child.setParent(this);

            if (dock == DockStyle.Fill)
            {
                addChildOnCenter(child);
            }
            else 
            {
                addChildOnSide(dock, child, isRemainSize);
            }
        }

        void addChildOnCenter(DockingContainer child)
        {
            child.SetBounds(0, 0, this.Width, this.Height);

            if (this.getContainerType()== ContainerType.SingleContent)//(Content is TabControl) == false)
            {
                _tabContainer.TabPages.Add(this.Name, this.Name); //insert current content to tab
                this.Content.Dock = DockStyle.Fill;
                this.Content.SetBounds(0, 0, this.Width, this.Height);
                _tabContainer.TabPages[this.Name].Controls.Add(this.Content);
                _tabContainer.TabPages[this.Name].Tag = this._id;
                _childId.Add(this.Name,this._id);
                //_childId.Add(child.Name, child._id);
                if (child.getContainerType() == ContainerType.SingleContent)
                {
                    addAChildWithSingleContentToTabContainer(_tabContainer, child);
                    
                }
                else //else TabContents
                {
                    addAChildWithTabContentToTabContainer(_tabContainer, child);
                   
                }                

                //this.Controls.Remove(Content);

                _tabContainer.Height = this.Height - TITLE_BAR_HEIGHT;
                _tabContainer.Width = this.Width;
                _tabContainer.Dock = DockStyle.Fill;

                Content = _tabContainer;
                
                this.Controls.Add(_tabContainer);
                
                showTitleBar(true, _tabContainer.TabPages[0].Name);
            }
            else //else TabContents
            {
                if (child.getContainerType() == ContainerType.SingleContent)
                {
                    addAChildWithSingleContentToTabContainer(_tabContainer, child);
                }
                else //else TabContents
                {
                    addAChildWithTabContentToTabContainer(_tabContainer, child);
                }
                
            }
            //_root.Disconnect(child.Name);
            this.setContainerType(ContainerType.TabContents);
            //this.Name = child.Name; //가장 최근에 선택된 tab의 이름을 가져온다.
            //this._id = _childId[child.Name];
            showTitleBar(true, child.Name);
            _tabContainer.SelectTab(child.Name);
        }
        
        internal void addAChildWithSingleContentToTabContainer(TabControl container, DockingContainer child){
            container.TabPages.Add(child.Name, child.Name);
            child.SetBounds(0, 0, container.Width, container.Height - TITLE_BAR_HEIGHT);
            container.TabPages[child.Name].Controls.Add(child.Content);
            container.TabPages[child.Name].Tag = child._id;
            container.TabPages[child.Name].Width = container.Width;
            container.TabPages[child.Name].Height = container.Height - 20;
            
            _childId.Add(child.Name, child._id);
            _root.Disconnect(child.Name);
        }

        internal void addAChildWithTabContentToTabContainer(TabControl container, DockingContainer child)
        {
            String name="";
            foreach (TabPage c in child.TabContainer.TabPages)
            {
                container.TabPages.Add(c);
                _childId.Add(c.Name, Int32.Parse(c.Tag.ToString()));
                c.Width = container.Width;
                c.Height = container.Height - 20;
                name = c.Name;
            }
            _root.Disconnect(child.Name);
        }

        void addChildOnSide(DockStyle newChildDock, DockingContainer newChild, Boolean isRemainSize)
        {

            showTitleBar(false,"");
            _root.Disconnect(this.Name); //새로운 것을 만들고 자기 자신은 루트와의 연결을 끊는다.
            // SplitContainer가 되면 titleBar가 사라진다.
            DockingContainer oldChild = new DockingContainer(_root, this, this.Name, Content, _id, _initDock);
            oldChild._type = this._type;
            oldChild._childId = this._childId;
            _id = -1; //컨테이너만 가지고 있을 것이므로 선택된 id가 없다.
            _titleBar.Text = "";

            this._childId = new Dictionary<String,int>(); //연결을 끊고 새로운 id List를 만든다.
            _tabContainer = new TabControl(); //연결을 끊고 새로운 TabContainer를 만든다.

            //orientation을 선택한다.
            int newSize, oldSize, parentSize;
            if (newChildDock == DockStyle.Left || newChildDock == DockStyle.Right)
            {
                newSize = newChild.Width;
                oldSize = this.Width;
                if (_parent == null) parentSize = this.Width; //parent가 null인 경우는 root일 경우.
                else parentSize = _parent.Width;
                _sContainer.Orientation = Orientation.Vertical;
            }
            else
            {
                newSize = newChild.Height;
                oldSize = this.Height;
                if (_parent == null) parentSize = this.Height;
                else parentSize = _parent.Height;
                _sContainer.Orientation = Orientation.Horizontal;
            }
            this.SuspendLayout();

            if (newChildDock == DockStyle.Left || newChildDock == DockStyle.Top)
            {
                _sContainer.Panel1.Controls.Add(newChild);
                _sContainer.Panel2.Controls.Add(oldChild);
                this._children.Add(newChild);
                this._children.Add(oldChild);
                this._sContainer.SplitterDistance = newSize;
            }
            else
            {
                _sContainer.Panel1.Controls.Add(oldChild);
                _sContainer.Panel2.Controls.Add(newChild);
                this._children.Add(oldChild);
                this._children.Add(newChild);
                this._sContainer.SplitterDistance = oldSize;
            }

            //크기 조정을 한다.
            if (isRemainSize)
            {
                if (parentSize < newSize + oldSize)
                {
                    newSize = oldSize = oldSize / 2; //부모에게 크기 조정 요청할 필요 없음.
                }
                else
                {
                    if (_root!=this) _parent.reSizeSplitDistanceForChild(this, newSize + oldSize);
                    else throw new Exception("예상치 못한 상황. 루트이면서 부모의 크기보다 작다?"); //이런 상황 없음
                }
            }
            else
            {
                newSize = oldSize = oldSize / 2; //부모에게 크기 조정 요청할 필요 없음.
            }
                
            if (newChildDock == DockStyle.Left || newChildDock == DockStyle.Top)
            {

                this._sContainer.SplitterDistance = newSize;
            }
            else
            {

                this._sContainer.SplitterDistance = oldSize;
            }

            this.Controls.Remove(Content);
            Content = _sContainer;
            this.Controls.Add(_sContainer);
            this.setContainerType(ContainerType.SplitContents);
            
            this.Name = "";
            this.ResumeLayout();
        }

        internal void reSizeSplitDistanceForChild(DockingContainer child, int size) //child 에서 크기조정을 요청할 때 호출된다.
        {

            if (_sContainer.Panel1.Controls[0] == child)
            {
                _sContainer.SplitterDistance = size;
            }
            else
            {
                if (_sContainer.Orientation == Orientation.Horizontal)
                {
                    _sContainer.SplitterDistance = this.Width - size;
                }
                else
                {
                    _sContainer.SplitterDistance = this.Height - size;
                }
            }
        }

        internal void setParent(DockingContainer parent)
        {
            _parent = parent;
        }

        public Boolean ClearAll()
        {
            //all grandchildren clear first.
            for (int i = _children.Count; i >= 0; i--)
            {
                _children[i].ClearAll();
            }
            _children.Clear();
            //all children clear
            _sContainer.Panel1.Controls.Clear();
            _sContainer.Panel2.Controls.Clear();
            for (int i = 0; i < _tabContainer.TabPages.Count; i++)
            {
                _tabContainer.TabPages[i].Controls.Clear();
            }
            _tabContainer.TabPages.Clear();
            
            _draggingPopup.Dispose();

            return true;
        }

        internal Rectangle getScreenRect()
        {
            Point pt = PointToScreen(new Point(0, 0));
            return new Rectangle(pt, new Size(this.Width, this.Height));
        }

        internal int getDepth()
        {
            if (_parent != null) return _parent.getDepth() + 1;
            else return 0;
        }


        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                ClearAll();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            /*
            //this._tabContainer = new System.Windows.Forms.TabControl();
            //this._sContainer = new System.Windows.Forms.SplitContainer();
            this._sContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tabContainer
            // 
            this._tabContainer.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this._tabContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabContainer.Location = new System.Drawing.Point(0, 0);
            this._tabContainer.Name = "_tabContainer";
            this._tabContainer.SelectedIndex = 0;
            this._tabContainer.Size = new System.Drawing.Size(200, 100);
            this._tabContainer.TabIndex = 0;
            // 
            // _sContainer
            // 
            this._sContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._sContainer.Location = new System.Drawing.Point(0, 0);
            this._sContainer.Name = "_sContainer";
            this._sContainer.Size = new System.Drawing.Size(533, 400);
            this._sContainer.SplitterDistance = 177;
            this._sContainer.TabIndex = 0;
            // 
            // DockingContainer
            // 
            this.Name = "DockingContainer";
            //this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.AutoSize = true;
            //this.Size = new System.Drawing.Size(533, 400);
            this._sContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            */
            this._sContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tabContainer
            // 
            this._tabContainer.Alignment = System.Windows.Forms.TabAlignment.Bottom;
//            this._tabContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabContainer.SetBounds(0, 0, 100, 100);
            this._tabContainer.Name = "_tabContainer";
            this._tabContainer.SelectedIndex = 0;
            this._tabContainer.TabIndex = 0;
            // 
            // _sContainer
            // 
            this._sContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._sContainer.Location = new System.Drawing.Point(0, 0);
            this._sContainer.Name = "_sContainer";
            this._sContainer.SplitterDistance = 177;
            this._sContainer.TabIndex = 0;
            // 
            // DockingContainer
            // 
            
            //this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.AutoSize = true;
            //this.Size = new System.Drawing.Size(533, 400);
            this._sContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


    }
}
