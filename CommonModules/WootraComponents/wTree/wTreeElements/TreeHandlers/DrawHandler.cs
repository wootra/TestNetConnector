using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class DrawHandler
    {
        wTree _ownerTree;
        /// <summary>
        /// UseCustomDrawing이 true일 경우, 이 이벤트를 구현해야 한다.
        /// </summary>
        public event wTreeNodeDrawingEvent E_TreeNodeDrawing;

        internal DrawHandler(wTree ownerTree)
        {
            _ownerTree = ownerTree;
            _imageBufferToDraw = new Bitmap(10, 10);
            _tempBufferToDraw = new Bitmap(10, 10);

            SelectedBackColor = Brushes.LightCyan;
            SelectedBorderColor = Pens.LightBlue;
            SelectedForeColor = Color.Blue;
            HoverBackColor = Brushes.LightPink;
            HoverBorderColor = Pens.LightSalmon;
            HoverForeColor = Color.Red;

            BasicDrawing = BasicDrawings.PlusMinus | BasicDrawings.Lines | BasicDrawings.Selection | BasicDrawings.MultiSelections | BasicDrawings.RollOver;
            ////BasicDrawing = BasicDrawings.PlusMinus | BasicDrawings.Selection | BasicDrawings.MultiSelections;
            ItemXMargin = 0;
            ItemYMargin = 1;
            CheckBoxImages = new Image[] { Properties.Resources.normal, Properties.Resources.check_red, Properties.Resources.inter };
            UseCustomDrawing = true;
        }

        wTreeMouseEventsHandler MouseEventsHandler { get { return _ownerTree.wMouseEventsHandler; } }

        wTreeScroll ScrollHandler { get { return _ownerTree.wScrollHandler; } }

        wTreeSelections SelectionHandler { get { return _ownerTree.wSelectionHandler; } }

        
        EditorHandlerClass EditorHandler { get { return _ownerTree.wEditorHandler; } }

        /// <summary>
        /// CheckBox 이미지는 기본적으로 제공되지만, 바꾸고자 할 때 바꿀 수 있다. 
        /// 0번은 unchecked,1번은 checked, 2번은 일부체크상태이다.
        /// </summary>
        public Image[] CheckBoxImages { get; set; }

        public int ItemXMargin { get; set; }

        public int ItemYMargin { get; set; }

        /// <summary>
        /// User가 직접 그릴 것인지를 지정한다. 만일 직접 그린다면, E_TreeNodeDrawing 이벤트를 구현해야 한다.
        /// </summary>
        public bool UseCustomDrawing { get; set; }

        public Brush SelectedBackColor
        {
            get;
            set;
        }

        public Pen SelectedBorderColor
        {
            get;
            set;
        }


        public Color SelectedForeColor
        {
            get;
            set;
        }

        public Brush HoverBackColor
        {
            get;
            set;
        }

        public Pen HoverBorderColor
        {
            get;
            set;
        }


        public Color HoverForeColor
        {
            get;
            set;
        }

        BasicDrawings _basicDrawings = BasicDrawings.PlusMinus | BasicDrawings.Lines | BasicDrawings.Selection;
        public BasicDrawings BasicDrawing
        {
            get { return _basicDrawings; }
            set { _basicDrawings = value; }
        }

        internal Image _imageBufferToDraw;//실제 그려지는 버퍼.
        internal Image _tempBufferToDraw;//더블버퍼링 위한 버퍼

        /// <summary>
        /// wTree에 그리기 위해 임시로 그리는 버퍼.
        /// </summary>
        internal Image ImageTempBufferToDrawForTree
        {
            get { return _tempBufferToDraw; }
        }
        /// <summary>
        /// tree를 다시 그린다. 모든 자식들을 다시 모두 그린다., 
        /// </summary>
        public void ReDrawTree(bool treeSizeChanged)
        {
            Graphics g = Graphics.FromImage(_tempBufferToDraw);
            ReDrawAllChildren(g, treeSizeChanged);
            g = Graphics.FromImage(_imageBufferToDraw);
            //g.Clear(BackColor);
            g.DrawImage(_tempBufferToDraw, _ownerTree.Margin.Left, _ownerTree.Margin.Top);//더블버퍼링..
            //this.BackgroundImage = _imageBufferToDraw;
            MoveEditor();
            _ownerTree.Invalidate();
            _ownerTree.Update();


        }
        /// <summary>
        /// 모든 자식 노드들을 다시 그린다. 이 때 VisibleNodes가 갱신된다.
        /// </summary>
        /// <param name="g"></param>
        internal void ScrollChanged(Graphics g)
        {
            int y = -ScrollHandler.VScrollOffset;
            //int x = 1 - HScrollOffset;

            g.Clear(_ownerTree.BackColor);
            bool isVisible = true;
            foreach (wTreeNode node in _ownerTree.Children)
            {
                if (isVisible)
                {
                    node.IsVisible = true;
                    y = node.OnImagePaint(g, 0, y, false);
                }
                else
                {
                    //y = node.OnImagePaint(g, 0, y, treeSizeChanged);//좌표계산을 해야 하므로..
                    node.IsVisible = false;
                    node.HideAllControls();
                }
                
                if (y > _ownerTree.Height) isVisible = false;// break;//보이는 영역에서 벗어나면 나감..
            }

            foreach (wTreeNode node in _ownerTree.Children)
            {
                node.DrawBasicDrawings(g);
            }

        }

        /// <summary>
        /// ShowControl에서 Control을 보여줄 때, 스크롤과 Margin으로 인한 보정오류가 발생하므로, 그 부분을 메인에서 처리해 준다.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void SetControlPositionInMain(Control control, Point positionInBuffer)
        {
            int newLeft = positionInBuffer.X + _ownerTree.Margin.Left;
            int newTop = positionInBuffer.Y + _ownerTree.Margin.Top;

            if (_ownerTree.Controls.Contains(control))
            {
                if (newLeft != control.Left || newTop != control.Top)//위치가 바뀌었을 때만..
                {
                    control.SetBounds(newLeft, newTop, 0, 0, BoundsSpecified.Location);
                    //control.SetBounds(x + Margin.Left, y, 0, 0, BoundsSpecified.Location);
                }
            }
            else
            {

                control.Left = newLeft;
                control.Top = newTop;
                _ownerTree.Controls.Add(control);
                control.Show();
            }
        }

        internal void MoveEditor()
        {
            wTreeEditor editor = _ownerTree.wEditorHandler.ActivatedEditor;
            if (editor != null)
            {
                SetControlPositionInMain(editor.EditorControl, editor.EditorPosition);
            }
        }

        bool _redrawingChildren = false;
        /// <summary>
        /// 모든 자식 노드들을 다시 그린다. 이 때 VisibleNodes가 갱신된다.
        /// </summary>
        /// <param name="g"></param>
        internal void ReDrawAllChildren(Graphics g, bool treeSizeChanged)
        {
            if (_redrawingChildren == false && _ownerTree.NeedRefresh)
            {
                _redrawingChildren = true;
                int y = -ScrollHandler.VScrollOffset;
                //int x = 1 - HScrollOffset;
                _ownerTree.ResetVisibleNodes();
                //if (treeSizeChanged) _ownerTree.VisibleNodes.Clear();
                g.Clear(_ownerTree.BackColor);
                bool isVisible = true;
                _ownerTree.SuspendLayout();
                foreach (wTreeNode node in _ownerTree.Children)
                {
                    if (isVisible)
                    {
                        node.IsVisible = true;
                        y = node.OnImagePaint(g, 0, y, treeSizeChanged);
                    }
                    else
                    {
                        //y = node.OnImagePaint(g, 0, y, treeSizeChanged);//좌표계산을 해야 하므로..
                        node.IsVisible = false;
                        node.HideAllControls();
                    }
                    if (y > _ownerTree.Height) isVisible = false;// break;//보이는 영역에서 벗어나면 나감..
                }
                
                foreach (wTreeNode node in _ownerTree.Children)
                {
                    node.DrawBasicDrawings(g);
                }
                if (treeSizeChanged)
                {
                    _totalSize = GetTotalSize();
                    _ownerTree.wScrollHandler.SetScrollBars(_totalSize, _tempBufferToDraw.Size);
                }
                _ownerTree.ResumeLayout();
                _redrawingChildren = false;
            }
        }

        /// <summary>
        /// tree를 다시 그린다. 모든 자식들을 다시 모두 그리며, 
        /// </summary>
        public void ReDrawTreeForScroll()
        {
            if (_redrawingChildren == false)
            {
                _redrawingChildren = true;
                Graphics g = Graphics.FromImage(_tempBufferToDraw);
                //g.Clear(BackColor);
                _ownerTree.Invalidate();
                _ownerTree.Update();
                ScrollChanged(g);
                g = Graphics.FromImage(_imageBufferToDraw);
                g.DrawImage(_tempBufferToDraw, _ownerTree.Margin.Left, _ownerTree.Margin.Top);//더블버퍼링..
                //this.BackgroundImage = _imageBufferToDraw;
                MoveEditor();
                _ownerTree.Invalidate();
                _ownerTree.Update();
                //base.Refresh();
                _redrawingChildren = false;
            }
        }




        private void ClearBackground()
        {
            Graphics g = Graphics.FromImage(_imageBufferToDraw);
            g.Clear(_ownerTree.BackColor);
        }
        



        Size _totalSize = new Size(0, 0);

        /// <summary>
        /// 그려져야 할 전체 크기이다. 이 크기를 기초로 스크롤바가 나타난다.
        /// </summary>
        public Size TotalSize { get { return _totalSize; } }



        int getMax(int a, int b)
        {
            if (a > b) return a;
            else return b;
        }
        int getCenter(int itemSize, int fromSize)
        {
            return (fromSize - itemSize) / 2;
        }

        public Size GetTotalSize()
        {
            int height = 0;
            int maxWid = 0;
            foreach (wTreeNode node in _ownerTree.Children)
            {
                Size childSize = node.GetSize();
                maxWid = getMax(childSize.Width, maxWid);
                height += childSize.Height;
            }
            return new Size(maxWid, height);
        }

        internal void ResizeTempDrawBuffer()
        {
            int bottomMargin = (ScrollHandler.ScrollHorizontalVisible) ? 17 : 0;
            int rightMargin = (ScrollHandler.ScrollVerticalVisible) ? 17 : 0;
            int wid = _ownerTree.Width - _ownerTree.Left - _ownerTree.Margin.Right - rightMargin;
            int hig = _ownerTree.Height - _ownerTree.Margin.Top - _ownerTree.Margin.Bottom - bottomMargin;
            if (wid < 0) wid = 20;
            if (hig < 0) hig = 20;
            _tempBufferToDraw = new Bitmap(wid, hig);
            ClearBackground();
        }

        internal void CustomDrawing(wTreeNodeDrawingArgs args)
        {
            if (E_TreeNodeDrawing != null)
            {
                E_TreeNodeDrawing(args);
            }
            
        }

        /// <summary>
        /// 현재 보이고 있는 Node의 내부 버퍼를 모두 새로 그림..
        /// </summary>
        public void RedrawAllBuffers()
        {
            foreach (wTreeNode node in _ownerTree.Children)
            {
                node.RedrawAllBuffers();
                
            }
        }

        public List<wTreeNode> VisibleNodes { get { return _ownerTree.VisibleNodes; } }

        public void RedrawNode(wTreeNode node)
        {
            node.DrawBuffer();
            Graphics g = Graphics.FromImage(ImageTempBufferToDrawForTree);
            node.DrawNode(g, node.Area.X, node.Area.Y);
        }
    }
}
