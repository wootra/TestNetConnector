using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WootraComs.wTreeElements;

namespace WootraComs
{
    public partial class wTree : UserControl, IwTreeNodeCollectionParent
    {
        public event wTreeNodeChangedEventHandler E_TreeNodeChanged;
        public event wTreeListChangedEventHandler E_TreeListChanged;
        public event wTreeListChangedEventHandler E_TreeExpandChanged;
        public event wTreeKeyEventHandler E_KeyDown;

        public wTree()
        {
            InitializeComponent();
            
            _children = new wTreeNodeCollection(this, this);
            _children.E_TreeExpandChanged += _children_E_TreeExpandChanged;
            _children.E_TreeListChanged += _children_E_TreeListChanged;
            _children.E_TreeNodeChanged += _children_E_TreeNodeChanged;
            
            //this.BackgroundImage = _imageBufferToDraw;
            //this.BackgroundImageLayout = ImageLayout.None;
            this.SizeChanged += wTree_SizeChanged;

            B_FocusGetter.SetBounds(-1000, 0, 0, 0, BoundsSpecified.Location);
            
            _editorHandler = new EditorHandlerClass(this);
            _editorHandler.InitEditors();
            _drawHandler = new DrawHandler(this);
            _selHandler = new wTreeSelections(this);
            _scrolls = new wTreeScroll(this, Scroll_Vertical, Scroll_Horizontal);
            this.SetScrollState(0, true);
            _mouseHandler = new wTreeMouseEventsHandler(this);
            B_FocusGetter.KeyDown += B_FocusGetter_KeyDown;
        }

        void B_FocusGetter_KeyDown(object sender, KeyEventArgs e)
        {
            if (E_KeyDown != null) E_KeyDown(_selHandler.SelectedNode, e);
        }

        bool _needRefresh=true;
        /// <summary>
        /// Refresh가 필요한 지 나타낸다.
        /// </summary>
        public bool NeedRefresh
        {
            get { return _needRefresh; }
            internal set { _needRefresh = value; }
        }
        void _children_E_TreeNodeChanged(wTreeNode treeNode, wTreeNodeChangedEventArgs arg)// int itemIndex)
        {
            _needRefresh = true;
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(treeNode, arg);// itemIndex);

        }
        protected virtual void OnTreeNodeChanged(wTreeNode treeNode, int itemIndex)
        {
        }

        void _children_E_TreeListChanged(IwTreeNodeCollectionParent treeParent)
        {
            _needRefresh = true;
            if (E_TreeListChanged != null) E_TreeListChanged(treeParent);
        }

        void _children_E_TreeExpandChanged(IwTreeNodeCollectionParent treeParent)
        {
            _needRefresh = true;
            wSelectionHandler.ReleaseTempSelection();

            if (E_TreeExpandChanged != null) E_TreeExpandChanged(treeParent);

            wDrawHandler.ReDrawTree(true);
        }
        wTreeMouseEventsHandler _mouseHandler;
        public wTreeMouseEventsHandler wMouseEventsHandler { get { return _mouseHandler; } }

        wTreeScroll _scrolls;
        public wTreeScroll wScrollHandler { get { return _scrolls; } }

        wTreeSelections _selHandler;
        public wTreeSelections wSelectionHandler { get { return _selHandler; } }

        DrawHandler _drawHandler;

        /// <summary>
        /// draw에 관련된 기능 모음..
        /// </summary>
        public DrawHandler wDrawHandler { get { return _drawHandler; } }

        
        EditorHandlerClass _editorHandler;
        /// <summary>
        /// Editor를 모두 관리한다.
        /// </summary>
        public EditorHandlerClass wEditorHandler
        {
            get { return _editorHandler; }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            
            if ((keyData&Keys.Control)==Keys.Control)
            {
                wSelectionHandler.AddSelectedToSelectionNodes();
                
                
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        public new Padding Margin
        {
            get { return base.Margin; }
            set
            {
                base.Margin = value;
                int bottomMargin = (Scroll_Horizontal.Visible) ? 17 : 0;
                _drawHandler._tempBufferToDraw = new Bitmap(this.Width - Margin.Left - Margin.Right, this.Height - Margin.Top - Margin.Bottom - bottomMargin);
                Graphics g = Graphics.FromImage(_drawHandler._imageBufferToDraw);
                g.Clear(BackColor);
            }
        }

        List<wTreeNode> _visibleNodes = new List<wTreeNode>();

        internal void ResetVisibleNodes(){
            List<wTreeNode> nodes = new List<wTreeNode>();
            foreach (wTreeNode node in Children)
            {
                node.GetVisibleNodes(nodes);
                    
            }
            _visibleNodes = nodes;
        }

        /// <summary>
        /// 현재 보이도록 설정된 Node
        /// </summary>
        internal List<wTreeNode> VisibleNodes{
            get{
                return _visibleNodes;
            }
            
        }

        void wTree_SizeChanged(object sender, EventArgs e)
        {
            if (this.Width > 0 && this.Height > 0)
            {
                wDrawHandler._imageBufferToDraw = new Bitmap(this.Width, this.Height);
                wDrawHandler.ResizeTempDrawBuffer();
                wDrawHandler.ReDrawTree(true);
            }
            else
            {
                wDrawHandler._imageBufferToDraw = new Bitmap(10,10);
            }
        }

        #region eventHandlers
       
        /// <summary>
        /// 모든 노드를 보여주고 내부 레퍼런스를 갱신한다.
        /// </summary>
        public new void Refresh()
        {
            //DrawHandler.RedrawAllBuffers();
            //DrawHandler.RedrawAllBuffers();
            int visibleNodes = GetVisibleNodesCount();
            wDrawHandler.ReDrawTree(visibleNodes != VisibleNodes.Count);

        }

        public int GetVisibleNodesCount()
        {
            int visibleNodes = 0;
            foreach (wTreeNode node in Children)
            {
                 visibleNodes += node.GetVisibleNodesCount();
            }
            return visibleNodes;
        }

        #endregion

        wTreeNodeCollection _children;
        /// <summary>
        /// Root Nodes..
        /// </summary>
        public wTreeNodeCollection Children
        {
            get
            {
                return _children;
            }
        }

        
        int _levelSize = 10;
        /// <summary>
        /// Node가 Depth가 깊어질 수록 들어가야 할 크기.. default 는 10.
        /// </summary>
        public int LevelSize
        {
            get { return _levelSize; }
            set { _levelSize = value; }
        }

        public int Depth
        {
            get { return 0; }
        }

        public IwTreeNodeCollectionParent TreeParent
        {
            get { return null; }
        }
        bool _isRefreshing = false;
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_isRefreshing == false)
            {
                _isRefreshing = true;
                int visibleNodes = GetVisibleNodesCount();
                if (visibleNodes != VisibleNodes.Count)
                {
                    wDrawHandler.ReDrawTree(true);
                }
                e.Graphics.DrawImage(wDrawHandler._imageBufferToDraw, 0, 0);
                _isRefreshing = false;
            }
            //base.OnPaintBackground(e);
        }

        public bool IsExpanded { get { return true; } set { } }

        
    }

}
