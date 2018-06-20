using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WootraComs.wTreeElements
{
    public class wTreeNode: IwTreeNodeCollectionParent
    {
        internal event wTreeNodeChangedEventHandler E_TreeNodeChanged;
        internal event wTreeListChangedEventHandler E_TreeListChanged;
        internal event wTreeListChangedEventHandler E_TreeExpandChanged;
        /// <summary>
        /// Name이 변했을 때 호출된다.
        /// </summary>
        internal event wTreeNodeNameChanged E_NameChanged;
        //internal event wTreeNodeItemMouseEvent E_TreeNodeItemSelected;

        internal wTreeNodeCollection _children = null;//부모 collection 을 가진다.
        wTree _ownerTree;
        /// <summary>
        /// icollection에 추가되면 셋팅된다.
        /// </summary>
        internal IwTreeNodeCollectionParent _parent = null;
        static int _plusWid=0;
        
        internal wTreeNode(wTree ownerTree, IwTreeNodeCollectionParent parent)
        {
            _parent = parent;
            if (parent == null)
            {
                _parent = null;
            }
            _ownerTree = ownerTree;
            _children = new wTreeNodeCollection(_ownerTree, this);
            _children.E_TreeListChanged += TreeListChanged;
            _children.E_TreeNodeChanged += TreeNodeChanged;
            _children.E_TreeExpandChanged += _children_E_TreeExpandChanged;
            //_children.E_TreeNodeItemSelected += _items_E_ItemSelected;
            _items = new wTreeNodeItemCollection(_ownerTree, this);
            _items.E_ItemChanged += _items_E_ItemChanged;
            _items.E_ItemListChanged += _items_E_ItemListChanged;
            //_items.E_ItemSelected += _items_E_ItemSelected;

            if (_plusWid == 0) _plusWid = Properties.Resources.plus.Width;
        }

        public wTreeMouseEventsHandler MouseEventsHandler { get { return _ownerTree.wMouseEventsHandler; } }

        public wTreeScroll ScrollHandler { get { return _ownerTree.wScrollHandler; } }

        public wTreeSelections SelectionHandler { get { return _ownerTree.wSelectionHandler; } }

        public DrawHandler DrawHandler { get { return _ownerTree.wDrawHandler; } }

        public EditorHandlerClass EditorHandler { get { return _ownerTree.wEditorHandler; } }

        /// <summary>
        /// 사용자가 원하는 object를 할당한다.
        /// </summary>
        public object Tag { get; set; }

        string _name;
        /// <summary>
        /// 사용자가 원하는 이름을 할당한다.
        /// </summary>
        public String Name {
            get
            {
                return _name;
            }
            set
            {
                if (value.Equals(_name) == false)
                {
                    bool isChanged = true;
                    string errMsg = "";
                    if (E_NameChanged != null) isChanged = E_NameChanged(this, value, ref errMsg);
                    if (isChanged) //이름바꾸기가 성공했으면
                    {
                        _name = value; //값 갱신.
                    }
                    else //실패했으면
                    {
                        if (errMsg.Length > 0)
                        {
                            throw new Exception("Name Cannot Changed :" + errMsg);
                        }
                        else
                        {
                            throw new Exception("Name Cannot Changed");
                        }
                    }
                }
            }
        }

        void _children_E_TreeExpandChanged(IwTreeNodeCollectionParent treeParent)
        {
            if (E_TreeExpandChanged != null) E_TreeExpandChanged(treeParent);
        }

        /// <summary>
        /// 부모에서 순서이다.할당되기 전에는 -1이 된다.
        /// </summary>
        public int Index
        {
            get
            {
                if (_parent != null)
                {
                    return _parent.Children.IndexOf(this);
                }
                else return -1;//할당되기 전..
            }
        }

        internal Font Font
        {
            get { return _ownerTree.Font; }
        }

        /// <summary>
        /// 선택된 상태인지 나타낸다.
        /// </summary>
        public bool IsSelected
        {
            get {
                if (SelectionHandler.SelectedNodes.Contains(this) && (DrawHandler.BasicDrawing & BasicDrawings.MultiSelections) == BasicDrawings.MultiSelections) return true;
                else return SelectionHandler.SelectedNode == this; 
            }
        }

        /// <summary>
        /// 이 Node를 Selected로 표시한다. IsSelected함수로 가져온다.
        /// 내부적으로는 _ownerTree.SelectedNodes안에 집어넣는다.
        /// </summary>
        public void SetSelection(bool selected)
        {

            if (selected == true)
            {
                
                SelectionHandler.InsertSelected(this);
                
            }
            else
            {
                SelectionHandler.RemoveSelected(this);
            }

        }

        bool IsRollOver
        {
            get { return MouseEventsHandler.RollOverNode == this; }
        }

        internal wTree OwnerTree
        {
            get { return _ownerTree; }
        }



        internal void SetOwnerTree(wTree ownerTree){
            _ownerTree = ownerTree;
        }
        
        bool _isExpanded = false;
        public bool IsExpanded
        {
            get{
                if (Children.Count > 0) return _isExpanded;
                else return false;
            }
            set {
                bool isChanged = (_isExpanded != value);
                _isExpanded = value;
                if (isChanged)
                {
                    if (E_TreeExpandChanged != null) E_TreeExpandChanged(this);
                }
            }
        }


        /// <summary>
        /// tree에서 보이는 중인지를 나타낸다. false인 경우는, item의 높이가 0이거나 IsVisible이 false로 설정되었을 경우이다.
        /// </summary>
        public bool Visible{
            get{
                if (Area.Height == 0) return false;
                else return IsVisible;
            }
        }

       

        #region eventHandlers

        bool _needRefresh = true;
        void _items_E_ItemListChanged(wTreeNodeItem item)
        {
            // DrawBuffer();
            _needRefresh = true;
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(this, new wTreeNodeChangedEventArgs(-1));
        }

        

        void _items_E_ItemChanged(wTreeNodeItem item)
        {
            //DrawBuffer();
            _needRefresh = true;
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(this,new wTreeNodeChangedEventArgs( _items.IndexOf(item)));
        }


        void TreeListChanged(IwTreeNodeCollectionParent node)
        {
            if (E_TreeListChanged != null) E_TreeListChanged(node);
        }

        void TreeNodeChanged(wTreeNode node, wTreeNodeChangedEventArgs e)
        {
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(node, e);
        }
        #endregion

        public wTreeNodeCollection Children
        {
            get { return _children; }
        }



        bool _isVisible = true;//현재 보이고 있는지
        /// <summary>
        /// wTreeNode를 일시적으로 보이지 않게 할 것인지 정할 수 있다. 
        /// 현재 보이고 있는지도 나타낸다. 단, Size가 0임으로 인해 보이지 않는 것은
        /// Visible속성으로 가지고 와야 한다.
        /// </summary>
        public bool IsVisible { 
            get {
                if (TreeParent is wTreeNode)
                {
                    if ((TreeParent as wTreeNode).IsExpanded == false) return false;
                }
                return _isVisible; 
            } 
            set { _isVisible = value; } }

        wTreeNodeItemCollection _items;

        public wTreeNodeItemCollection Items
        {
            get { return _items; }
        }

        /// <summary>
        /// -1이면 아무곳에도 소속되지 않았다는 것을 나타냄..
        /// </summary>
        public int Depth
        {
            get {
                if (_parent is wTree) return 0;//root
                else return _parent.Depth + 1;
            }
        }

        /// <summary>
        /// treeNode의 부모. wTree가 될 수도 있고, wTreeNode가 될 수도 있다.
        /// </summary>
        public IwTreeNodeCollectionParent TreeParent
        {
            get { return _parent; }
        }
        Point _point;
        /// <summary>
        /// 현재 Node의 위치..보이지 않으면 -1,-1 을 리턴한다.
        /// </summary>
        public Point Location { get { return _point; } }
        
        /// <summary>
        /// Node의 영역. 선택영역 포함
        /// </summary>
        public Rectangle SelectionArea {
            get
            {
                //int rightMargin = _ownerTree.VerticalScrollVisible ? 17 : 0;
                return new Rectangle(
                    Location.X,
                    Location.Y,
                    DrawHandler.ImageTempBufferToDrawForTree.Width - Location.X-1,// - rightMargin,
                    _imgToDrawNode.Height + 2);

            }
        }

        /// <summary>
        /// Node의 영역. 선택영역 제외
        /// </summary>
        public Rectangle Area {
            get
            {
                if (_imgToDrawNode == null)
                {
                    DrawBuffer();
                }
                return new Rectangle(_point, new Size(_imgToDrawNode.Size.Width+moveX, _imgToDrawNode.Height+2));//2는 Selection Border 영역을 위한 위,아래
            }
        }

        /// <summary>
        /// y축 방향의 공백..
        /// </summary>
        int ItemYMargin
        {
            get { return DrawHandler.ItemYMargin; }
        }

        /// <summary>
        /// 내부의 Item들을 그리고 나서 다음에 그릴 y좌표를 가지고 온다.
        /// 실제 내부의 내용은 바꾸지 않으며, 바꾸려면 DrawBuff를 해야 한다.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sizeChanged">size가 변하여 visibleNodes가 변해야 할 상황이면 true</param>
        /// <returns></returns>
        internal int OnImagePaint(Graphics g, int level, int y, bool sizeChanged)
        {
            if (_isVisible)
            {
                int x = level * _ownerTree.LevelSize - ScrollHandler.HScrollOffset;
                _point = new Point(x, y);
                x += 1;//배경위한 margin..
                y += 1;//위 margin...

                //if (sizeChanged) DrawBuffer();
                Size drawnSize = DrawNode(g, x, y);
                
               

                //if(sizeChanged) _ownerTree.VisibleNodes.Add(this);//visible Nodes에 등록된다. 
                //g.DrawRectangle(Pens.Black, x, y, ImageBufferToDraw.Width, ImageBufferToDraw.Height);
                y += drawnSize.Height + 1;// //아래쪽 margin..
                //Area는 위,아래에 border를 위한 2px가 포함되어야 한다.
                //Area = new Rectangle(_point, new Size(drawnSize.Width, drawnSize.Height + 2));
                
                
                if (this.IsExpanded)
                {
                    //x += _ownerTree.LevelSize;

                    foreach (wTreeNode node in this.Children)
                    {
                        y = node.OnImagePaint(g, level+1, y, sizeChanged);
                    }

                }
                else
                {
                    foreach (wTreeNode node in this.Children)
                    {
                        node.HideControl();
                    }
                }
            }
            else
            {
                _point = new Point(-1, -1);
                HideControl();
            }
            return y;
        }

        /// <summary>
        /// plusminus나 line을 그린다. 
        /// VisibleNodes를 기준으로해서 그린다.
        /// </summary>
        public void DrawBasicDrawings(Graphics g)
        {
            if (IsVisible)
            {
                if ((DrawHandler.BasicDrawing & BasicDrawings.Lines) == BasicDrawings.Lines)
                {
                    drawLinkLines(g);
                    // node.drawLinkLines(g);
                }
                if ((DrawHandler.BasicDrawing & BasicDrawings.PlusMinus) == BasicDrawings.PlusMinus)
                {
                    if (Children.Count > 0)
                    {
                        DrawPlusMinus(g);
                        //baseX += Properties.Resources.plus.Width;//plusminus의 크기..
                    }
                }
            }
            
            /*
            if (Children.Count > 0)
            {
                DrawPlusMinus(g, Area.X, Area.Y);
                //baseX += Properties.Resources.plus.Width;//plusminus의 크기..
            }

            for (int i = 0; i < Children.Count; i++)
            {
                wTreeNode node = Children[i];
                
                if ((_ownerTree.BasicDrawing & BasicDrawings.Lines) == BasicDrawings.Lines)
                {
                   // node.drawLinkLines(g);
                }
                if ((_ownerTree.BasicDrawing & BasicDrawings.PlusMinus) == BasicDrawings.PlusMinus)
                {
                    if (Children.Count>0)
                    {
                        node.DrawPlusMinus(g, node.Area.X, node.Area.Y);
                        //baseX += Properties.Resources.plus.Width;//plusminus의 크기..
                    }
                }
            }
            */
        }

        private void drawLinkLines(Graphics g)
        {
            
            int halfX = (int)Math.Ceiling(_ownerTree.LevelSize / 2.0);
            int xMiddle = Area.X + halfX;// -_ownerTree.HScrollOffset;
            int xRight = Area.X + halfX;// -_ownerTree.HScrollOffset;
            int yMiddle = Area.Y + (int)(_lineHeight[0] / 2.0);// -_ownerTree.VScrollOffset;
            Pen linePen = new Pen(Brushes.Gray);
            linePen.DashStyle = DashStyle.Dot;
            //g.DrawLine(linePen, new Point(xMiddle, yMiddle), new Point(xRight, yMiddle));//자기 자신것..중앙부터 우측까지..

            if(Children.Count>0){

                if (IsExpanded)
                {

                    foreach (wTreeNode node in Children)
                    {
                        if (node.Depth > 0)
                        {
                            int childXMiddle = node.Area.X + halfX;// -_ownerTree.HScrollOffset;
                            int childYMiddle = node.Area.Y + (int)Math.Ceiling(node._lineHeight[0] / 2.0);// -_ownerTree.VScrollOffset;
                            if (node.IsVisible)
                            {
                                //if (_ownerTree.VisibleNodes.Contains(node))
                                {

                                    g.DrawLine(linePen, new Point(xMiddle, yMiddle), new Point(xMiddle, childYMiddle));
                                    g.DrawLine(linePen, new Point(xMiddle, childYMiddle), new Point(childXMiddle, childYMiddle));

                                    yMiddle = childYMiddle;
                                }
                            }
                            node.drawLinkLines(g);
                        }
                    }
                }
            }

        }


        private void HideControl()
        {
            foreach (wTreeNodeItem item in Items)
            {
                if (item.ItemType == wTreeNodeItemTypes.Control)
                {
                    _ownerTree.Controls.Remove(item.Control);
                    item.Visible = false;
                }
            }

        }

        Image _imgToDrawNode;
        /// <summary>
        /// Node에 그리기 위한 버퍼. 이 버퍼가 Tree에 그려진다.
        /// </summary>
        internal Image ImageBufferToDraw
        {
            get { return _imgToDrawNode; }
        }
        
        /// <summary>
        /// buffer를 다시 그리고 Node를 갱신한다.
        /// </summary>
        internal void ReDrawNode()
        {
            DrawBuffer();
            Graphics g = Graphics.FromImage(DrawHandler.ImageTempBufferToDrawForTree);

            DrawNode(g, Location.X, Location.Y);
        }

        
        /// <summary>
        /// Line모드나 PlusMinus모드일 때 앞에 공백을 결정하는 요소이다. 모든 좌표가 연결되기 때문에 중요한 요소이다.
        /// </summary>
        int moveX
        {
            get
            {
                if ((DrawHandler.BasicDrawing & BasicDrawings.PlusMinus) == BasicDrawings.PlusMinus ||
                (DrawHandler.BasicDrawing & BasicDrawings.Lines) == BasicDrawings.Lines)
                {
                    return _plusWid;
                }
                else return 0;

            }
        }

        /// <summary>
        /// Node를 그린다. 내부적으로는 이미 그려진 버퍼를 정해진 Graphics에 그리는 작업이다.
        /// 새로 버퍼를 그리려면 DrawBuffer를 호출해야 한다.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="node"></param>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        /// <returns></returns>
        internal Size DrawNode(Graphics g, int baseX, int baseY)
        {
            Rectangle selectionArea = SelectionArea;
            if (IsRollOver && (DrawHandler.BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)
            {
                g.FillRectangle(DrawHandler.HoverBackColor, selectionArea);
                g.DrawRectangle(DrawHandler.HoverBorderColor, selectionArea);
               
            }
            else if (IsSelected && (DrawHandler.BasicDrawing & BasicDrawings.Selection) == BasicDrawings.Selection)
            {
                g.FillRectangle(DrawHandler.SelectedBackColor, selectionArea);
                g.DrawRectangle(DrawHandler.SelectedBorderColor, selectionArea);
               
            }
            else
            {
                //g.FillRectangle(new SolidBrush(Brushes.Blue, new Rectangle(baseX, baseY, _ownerTree.Width - baseX, _imgToDrawNode.Height));
                //g.DrawRectangle(Pens.LightBlue, new Rectangle(baseX, baseY, _ownerTree.Width - baseX, _imgToDrawNode.Height));
            }

            //_point = new Point(baseX, baseY);
            
            g.DrawImage(ImageBufferToDraw, new Point(baseX, baseY));
            if (_needRefresh)
            {
                DrawBuffer();
                _needRefresh = false;
            }
            //DrawBuffer();

            int lineNum = 0;
            if (DrawnByUser == false)
            {
                foreach (wTreeNodeItem item in Items)
                {
                    if (item.ItemType == wTreeNodeItemTypes.WhiteSpace)
                    {
                        if (item.WhiteSpaceType == WhiteSpaceTypes.Return)
                        {
                            lineNum++;
                            continue;//다음으로 넘어감..
                        }
                    }

                    else if (item.ItemType == wTreeNodeItemTypes.Control)
                    {
                        item.DrawControl(item.ItemArea.X, item.ItemArea.Y, _lineHeight[lineNum]);
                    }

                }
            }
            else
            {
                HideControl();
            }
            //DrawBasicDrawings(g);
            return ImageBufferToDraw.Size;
        }

        bool _drawnByUser = false;
        
        /// <summary>
        /// userDrawn 이벤트에 의해 그려졌으면 true..
        /// </summary>
        public bool DrawnByUser { get { return _drawnByUser; } }

        private void DrawPlusMinus(Graphics g)
        {
            if (IsVisible)
            {
                int x = Area.X;
                int y = Area.Y;
                if (Children.Count == 0)
                {
                    //plusminus를 그리지 않음..그래도 그 위치는 띄워야 함..
                }
                else
                {
                    y += getCenter(Properties.Resources.minus.Height, _lineHeight[0]);
                    if (IsExpanded)
                    {

                        g.DrawImage(Properties.Resources.minus, x, y);
                    }
                    else
                    {
                        g.DrawImage(Properties.Resources.plus, x, y);
                    }
                }
                foreach (wTreeNode node in Children)
                {
                    node.DrawPlusMinus(g);
                }
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
        /// 이 node와 node에 종속된 모든 Node의 총 크기를 가지고 온다.
        /// </summary>
        /// <param name="x">Node가 그려질 x좌표</param>
        /// <param name="y">Node가 그려질 y좌표</param>
        /// <param name="maxWid">x좌표 + 최대 Width</param>
        /// <returns>다음 노드가 그려질 y좌표</returns>
        internal Size GetSize()
        {
            int height = 0;
            int maxWid = 0;
            height += Area.Height;//margin for border is 2
            
            if (IsExpanded)
            {
                foreach (wTreeNode child in Children)
                {
                    Size childSize = child.GetSize();
                    height += childSize.Height;
                    maxWid = getMax(childSize.Width, maxWid);
                }
            
            }
            maxWid = getMax(_ownerTree.LevelSize + maxWid, Area.Width);
            return new Size(maxWid, height);
        }


        Dictionary<int, int> _lineHeight = new Dictionary<int, int>();
        /// <summary>
        /// 노드를 그리기 위한 버퍼. 노드 자체적으로 가지는 버퍼를 그린다.
        /// </summary>
        internal void DrawBuffer()
        {
            
            Size drawSize = new Size(0, 0);
            wTreeNodeDrawingArgs args = null;
            bool isUserDrawn = false;
            
            if (DrawHandler.UseCustomDrawing)
            {
                args = new wTreeNodeDrawingArgs(this, 0, 0);
                DrawHandler.CustomDrawing(args);
                if (args.UserDrawn)
                {
                    _drawnByUser = true;
                    _imgToDrawNode = args.DrawnBuffer;
                    drawSize = args.DrawnSize;
                    isUserDrawn = true;
                    _lineHeight.Clear();
                    _lineHeight[0] = _imgToDrawNode.Height;
                }
            }
            if (isUserDrawn)
            {

                //drawItemsInBuffer();
                return;//이미 그렸으므로 지나감.
            }

            _drawnByUser = false;
            Graphics g = Graphics.FromImage(DrawHandler.ImageTempBufferToDrawForTree);
            //단순히 크기 가져오는 것이므로 버퍼는 그냥 전체버퍼를 사용.
            int wid = 0;
            //int wid = 0;
            int moveY=0;
           
            int maxHeight = 0;
            int maxWid = 0;
            Size itemSize;

            _lineHeight = new Dictionary<int, int>();//첫 줄의 높이.
            int lineNum = 0;//줄바꿈이 있을 경우,줄의 번호..0부터 시작.

            foreach (wTreeNodeItem item in this.Items)
            {
                itemSize = item.GetItemSize();

                if (item.ItemType == wTreeNodeItemTypes.WhiteSpace)
                {
                    if((WhiteSpaceTypes)item.Value == WhiteSpaceTypes.Return)
                    //if (itemSize.Width < 0)//다음줄로 넘어감..whiteSpace가 Return일 때..
                    {
                        moveY += maxHeight;
                        maxHeight = 0;
                        wid = 0;
                        lineNum++;
                    }
                    else
                    {
                        wid += itemSize.Width;//Space크기..
                    }
                }else{
                    wid += itemSize.Width;//item왼쪽의 margin
                    maxHeight = getMax(maxHeight, itemSize.Height);//item 위쪽의 margin이다.
                    _lineHeight[lineNum] =maxHeight+ ItemYMargin;//각 줄의 높이를 구한다.//최대에서 ItemYMargin만큼 키운다.
                }
                maxWid = getMax(maxWid, wid);//줄이바뀌어도 maxWid는 가장 큰 wid를 유지해야 함..
            }

            int totalHeight = 0;
            foreach (int lineHeight in _lineHeight.Values)
            {
                totalHeight += lineHeight;
            }
               

            Size buffSize = new Size(maxWid+moveX, totalHeight);// maxHeight + moveY);//버퍼크기를 정하고..(x margin은 2, ymargin은 4)
            if (ImageBufferToDraw == null || ImageBufferToDraw.Width!=buffSize.Width || ImageBufferToDraw.Height != buffSize.Height)
            {//크기가 다르거나버퍼 없을 경우 다시 버퍼를 만들어줌..
                _imgToDrawNode = new Bitmap(buffSize.Width, buffSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

             
            
            
            drawItemsInBuffer();
        }
        /// <summary>
        /// item들을 Node Buffer에 그린다.
        /// </summary>
        private void drawItemsInBuffer()
        {
            Graphics g = Graphics.FromImage(ImageBufferToDraw);

            
            DrawBackSelectionForBuffer(g);//선택영역 그림..

            int startX = moveX;
            int moveY = 0;
            int lineNum = 0;
            Size itemSize;
            foreach (wTreeNodeItem item in this.Items)
            {

                switch (item.ItemType)
                {
                    case wTreeNodeItemTypes.CheckBox:
                        {
                            //itemSize = item.GetCheckBoxSize();

                            //wid += ItemXMargin;//margin
                            itemSize = item.DrawCheckBox(g, startX, moveY, _lineHeight[lineNum]);
                            startX += itemSize.Width;

                            break;
                        }
                    case wTreeNodeItemTypes.Image:
                        {
                            //itemSize = item.GetImageSize();
                            //toY = getCenter(itemSize.Height, _lineHeight[lineNum]) + moveY;
                            //wid += ItemXMargin;
                            itemSize = item.DrawImage(g, startX, moveY, _lineHeight[lineNum]);
                            startX += itemSize.Width;
                            break;
                        }
                    case wTreeNodeItemTypes.Text:
                    case wTreeNodeItemTypes.TextArray:
                        {
                            //itemSize = item.GetStringSize();
                            //toY = getCenter(itemSize.Height, _lineHeight[lineNum]) + moveY;
                            //wid += ItemXMargin;
                            if (IsRollOver && (DrawHandler.BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)
                            {
                                itemSize = item.DrawHoveredString(g, startX, moveY, _lineHeight[lineNum]);
                            }
                            else if (IsSelected && (DrawHandler.BasicDrawing & BasicDrawings.Selection) == BasicDrawings.Selection)
                            {
                                itemSize = item.DrawSelectedString(g, startX, moveY, _lineHeight[lineNum]);
                            }
                            else
                            {
                                itemSize = item.DrawString(g, startX, moveY, _lineHeight[lineNum]);
                            }
                            startX += itemSize.Width;
                            break;
                        }
                    
                    case wTreeNodeItemTypes.Control:
                        {
                            //itemSize = item.GetControlSize();
                            //toY = getCenter(itemSize.Height, _lineHeight[lineNum]) + moveY;
                            //wid += ItemXMargin;
                            itemSize = item.DrawControl(startX, moveY, _lineHeight[lineNum]);
                            startX += itemSize.Width;

                            break;
                        }
                    case wTreeNodeItemTypes.WhiteSpace:
                        {
                            if ((WhiteSpaceTypes)item.Value == WhiteSpaceTypes.Return)
                            {
                                moveY += _lineHeight[lineNum];
                                startX = 0;
                                lineNum++;
                            }
                            else//space
                            {
                                startX += item.GetItemSize().Width;
                            }

                            break;
                        }
                }

            }
        }
        
        private void DrawBackSelectionForBuffer(Graphics g)
        {
            g.FillRectangle(BackColor, new Rectangle(0, 1, _imgToDrawNode.Width, _imgToDrawNode.Height - 2));
            /*
            if (IsRollOver && (DrawHandler.BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)
            {
                g.FillRectangle(DrawHandler.HoverBackColor, new Rectangle(0, 1, _imgToDrawNode.Width, _imgToDrawNode.Height - 2));
                //g.DrawRectangle(_ownerTree.SelectedBorderColor, new Rectangle(0, 0, _imgToDrawNode.Width, _imgToDrawNode.Height));
            }
            else if (IsSelected && (DrawHandler.BasicDrawing & BasicDrawings.Selection) == BasicDrawings.Selection)
            {
                g.FillRectangle(DrawHandler.SelectedBackColor, new Rectangle(0, 1, _imgToDrawNode.Width, _imgToDrawNode.Height - 2));
                //g.DrawRectangle(_ownerTree.SelectedBorderColor, new Rectangle(0, 0, _imgToDrawNode.Width, _imgToDrawNode.Height));
                //_ownerTree.SelectedBackColor
            }
            else
            {
                g.FillRectangle(new SolidBrush(_ownerTree.BackColor), new Rectangle(0, 1, _imgToDrawNode.Width, _imgToDrawNode.Height - 2));
                //g.DrawRectangle(_ownerTree.SelectedBorderColor, new Rectangle(0, 0, _imgToDrawNode.Width, _imgToDrawNode.Height));
                //_ownerTree.SelectedBackColor
                //g.Clear(Color.FromArgb(255, _ownerTree.BackColor));
            }
             */
        }

       

        private int getMax(int num1, int num2)
        {
            if (num1 > num2) return num1;
            else return num2;
        }

        Color _foreColor = Color.Empty;
        public Color ForeColor {
            get
            {
                if (_foreColor == Color.Empty) return _ownerTree.ForeColor;
                else return _foreColor;    
            }
            set
            {
                _foreColor = value;
            }
        }

        Color _selectedForeColor = Color.Empty;
        public Color SelectedForeColor
        {
            get
            {
                if (_selectedForeColor == Color.Empty) return DrawHandler.SelectedForeColor;
                else return _selectedForeColor;
            }
            set
            {
                _selectedForeColor = value;
            }
        }

        Color _hoveredForeColor = Color.Empty;
        public Color HoveredForeColor
        {
            get
            {
                if (_hoveredForeColor == Color.Empty) return DrawHandler.HoverForeColor;
                else return _hoveredForeColor;
            }
            set
            {
                _hoveredForeColor = value;
            }
        }

        

        internal wTreeNodeItem GetItemFromPoint(Point point)
        {

            if (DrawHandler.BasicDrawing == BasicDrawings.None)
            {
                Point relativePt = new Point(point.X - Location.X, point.Y - Location.Y);
                foreach (wTreeNodeItem item in Items)
                {
                    if (item.ItemArea.Contains(relativePt))
                    {
                        return item;
                    }
                }
                return null;
            }
            else
            {
                Point relativePt = new Point(point.X - Location.X, point.Y - Location.Y);
                if (relativePt.X > 0 && relativePt.X <=  moveX)
                {
                    return new wTreeNodeItem(this, wTreeNodeItemTypes.PlusMinus);
                }
                else
                {
                    
                    foreach (wTreeNodeItem item in Items)
                    {
                        if (item.ItemArea.Left < (relativePt.X) && item.ItemArea.Right >= (relativePt.X))
                        {
                            return item;
                        }

                    }
                }
                return null;
            }
            
            
        }


        /// <summary>
        /// wNodeItem들 중, itemIndex에 있는 체크박스를 모두 newValue로 바꾸어줌.
        /// </summary>
        /// <param name="itemIndex">체크박스가 존재하는 itemIndex</param>
        /// <param name="newValue">바꾸어줄 체크박스 상태.</param>
        internal void ControlChildChecks(int itemIndex, bool newValue)
        {
            foreach (wTreeNode node in Children)
            {
                if (node.Items[itemIndex].Value is bool?)
                {
                    node.Items[itemIndex].Value = newValue;
                    if (node.Visible)
                    {
                        node.DrawBuffer();
                    }
                }
                node.ControlChildChecks(itemIndex, newValue);
            }
        }

        /// <summary>
        /// 부모노드가 wTreeNode라면 부모노드가 자식들의 check상태를 검사하여 자기자신을 바꾸도록 한다.
        /// </summary>
        /// <param name="itemIndex">체크박스가 존재하는 itemIndex</param>
        internal void ControlParentChecks(int itemIndex)
        {
            if (this.TreeParent is wTreeNode)
            {
                wTreeNode parent = this.TreeParent as wTreeNode;
                parent.SetCheckStateFromChildren(itemIndex);
                parent.ControlParentChecks(itemIndex);
            }
        }

        /// <summary>
        /// 바로 다음 Depth의 자식들의 상태들로만으로 자기 자신의 checkbox의 상태를 체크한다.
        /// </summary>
        /// <param name="itemIndex">Node상에서 checkbox의 itemIndex.</param>
        public void SetCheckStateFromChildren(int itemIndex)
        {
            int countChecked = 0;
            int empty = 0;
            
            foreach (wTreeNode child in Children)
            {
                if (child.Items[itemIndex].Value is bool?)
                {
                    bool? childState = (bool?)(child.Items[itemIndex].Value);
                    if (childState == (bool?)null)//중간상태가 나오면..
                    {
                        Items[itemIndex].Value = (bool?)null;//자기 자신도 중간상태..
                        return;
                    }
                    else if (childState == true)
                    {
                        countChecked++;
                    }
                }
                else
                {
                    empty++;
                }
            }
            if (Items[itemIndex] != null && Items[itemIndex].ItemType == wTreeNodeItemTypes.CheckBox)
            {
                if (countChecked + empty == Children.Count)
                {
                    Items[itemIndex].Value = true;
                }
                else if (countChecked == 0)
                {
                    Items[itemIndex].Value = false;
                }
                else
                {
                    Items[itemIndex].Value = (bool?)null;// 중간상태..
                }
            }
        }

        /// <summary>
        /// pt가 node의 영역 안에 존재하는지 묻는다. mode로 y영역만 검사하거나 x영역만 검사할 수도 있다.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal bool ContainsPoint(Point pt, ContainsModes mode )
        {
            if (mode == ContainsModes.Y)
            {
                if (pt.Y >= SelectionArea.Top && pt.Y <= SelectionArea.Bottom) return true;
                else return false;
            }
            else if (mode == ContainsModes.X)
            {
                if (pt.X >= SelectionArea.Left && pt.X <= SelectionArea.Right) return true;
                else return false;
            }
            else//XY
            {
                if (pt.X >= SelectionArea.Left && pt.X <= SelectionArea.Right)
                {
                    if (pt.Y >= SelectionArea.Top && pt.Y <= SelectionArea.Bottom) return true;
                    else return false;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Expand되어 있고, IsVisible로 표시된 Node를 가져온다.
        /// 만일 이 노드가 visible이 false라면, 0을 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal int GetVisibleNodesCount()
        {
            if (IsVisible)
            {
                int nodes = 1;//자기자신..
                if (IsExpanded)
                {
                    foreach (wTreeNode node in Children)
                    {
                        if (node.IsVisible)
                        {
                            nodes += node.GetVisibleNodesCount();
                        }
                    }
                }

                return nodes;
            }
            else return 0;
        }

        /// <summary>
        /// 종속된 노드에 있는 모든 Control을 지운다.
        /// </summary>
        internal void HideAllControls()
        {
            HideControl();
            foreach (wTreeNode node in Children)
            {
                node.HideAllControls();
            }
        }

        /// <summary>
        /// 자식 노드들까지 모두 새로 그린다.
        /// </summary>
        internal void RedrawAllBuffers()
        {
            DrawBuffer();
            foreach (wTreeNode node in Children)
            {
                node.RedrawAllBuffers();
            }
        }

        internal void GetVisibleNodes(List<wTreeNode> nodes)
        {
            if (this.Visible)
            {
                nodes.Add(this);
                if (IsExpanded)
                {
                    foreach(wTreeNode node in Children){
                        node.GetVisibleNodes(nodes);
                    }
                }
            }
        }

        public Brush BackColor {
            get
            {
                if (IsRollOver && (DrawHandler.BasicDrawing & BasicDrawings.RollOver) == BasicDrawings.RollOver)
                {
                    return DrawHandler.HoverBackColor;
                }
                else if (IsSelected && (DrawHandler.BasicDrawing & BasicDrawings.Selection) == BasicDrawings.Selection)
                {
                    return DrawHandler.SelectedBackColor;
                }
                else
                {
                    return new SolidBrush(_ownerTree.BackColor);
                }
            }
        
        }
    }
}
