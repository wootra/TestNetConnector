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
        public event wTreeNodeDrawingEvent E_TreeNodeDrawing;

        public wTree()
        {
            InitializeComponent();
            UseCustomDrawing = false;
            _children = new wTreeNodeCollection(this);
            _children.E_TreeListChanged += TreeListChanged;
            _children.E_TreeNodeChanged += TreeNodeChanged;
        }

        #region eventHandlers
        void TreeListChanged(wTreeNode node)
        {
        }

        void TreeNodeChanged(wTreeNode node, int index)
        {
        }
        #endregion

        wTreeNodeCollection _children;
        public wTreeNodeCollection Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// User가 직접 그릴 것인지를 지정한다. 만일 직접 그린다면, E_TreeNodeDrawing 이벤트를 구현해야 한다.
        /// </summary>
        public bool UseCustomDrawing { get; set; }
        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RePaint(e.Graphics);
        }

        /// <summary>
        /// 갱신된 트리를 다시 그린다.
        /// </summary>
        public new void Refresh()
        {
            Graphics g = this.CreateGraphics();
            RePaint(g);
        }

        Point _startDrawingPoint = new Point(0, 0);
        /// <summary>
        /// Drawing을 시작해야 할 위치. default는 0,0
        /// </summary>
        public Point StartDrawingPoint
        {
            get { return _startDrawingPoint; }
            set { _startDrawingPoint = value; }
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

        void RePaint(Graphics g)
        {
            Size totalSize = RePaint(g, _children, _startDrawingPoint);
        }
        Size _totalSize = new Size(0,0);

        /// <summary>
        /// 그려져야 할 전체 크기이다. 이 크기를 기초로 스크롤바가 나타난다.
        /// </summary>
        Size TotalSize { get { return _totalSize; } }

        private Size RePaint(Graphics g, wTreeNodeCollection children, Point startPoint)
        {
            Size totalSize = new Size(0, 0);
            foreach (wTreeNode node in children)
            {
                Size drawnSize = DrawNode(g, node, startPoint);
                totalSize.Width += drawnSize.Width;
                totalSize.Height += drawnSize.Height;
                startPoint.Y += drawnSize.Height;
                if (node.IsExpaned)
                {
                    Point drawingPoint = new Point(startPoint.X + _levelSize, startPoint.Y);
                    drawnSize = RePaint(g, node.Children, drawingPoint);
                    totalSize.Width += drawnSize.Width;
                    totalSize.Height += drawnSize.Height;
                    startPoint.Y += drawnSize.Height;
                }
            }
            return totalSize;
        }

        int getMax(int a, int b)
        {
            if (a > b) return a;
            else return b;
        }
        int getCenter(int itemSize, int fromSize)
        {
            return (fromSize - itemSize) / 2;
        }

        int _itemMargin = 3;
        public int ItemMargin { get { return _itemMargin; } set { _itemMargin = value; } }

        private Size DrawNode(Graphics g, wTreeNode node, Point startPoint)
        {
            Size drawSize = new Size(0, 0);
            wTreeNodeDrawingArgs args=null;
            if (UseCustomDrawing)
            {
                args = new wTreeNodeDrawingArgs(node, startPoint);
                if (E_TreeNodeDrawing != null) E_TreeNodeDrawing(g, args);
            }

            if (args == null || args.UserDrawn==false)
            {
                g.FillRectangle(Brushes.White, startPoint.X, startPoint.Y, this.Width - startPoint.X, 20);
                int x=startPoint.X + 2;
                int y = startPoint.Y+2;
                int width = x;
                int height = 0;
                foreach (wTreeNodeItem item in node.Items)
                {
                    int itemHeight = 0;
                    
                    switch (item.ItemType)
                    {
                        case wTreeNodeItemTypes.CheckBox:
                            {
                                Image checkBox;
                                if (item._isChecked == true)
                                {
                                    checkBox = Properties.Resources.check_red;
                                }
                                else if (item._isChecked == false)
                                {
                                    checkBox = Properties.Resources.normal;
                                }
                                else
                                {
                                    checkBox = Properties.Resources.inter;
                                }

                                itemHeight = checkBox.Height;
                                
                                break;
                            }
                        case wTreeNodeItemTypes.Image:
                            {
                                itemHeight = item._image.Height;
                                
                                break;
                            }
                        case wTreeNodeItemTypes.Text:
                            {
                                SizeF txtSize = g.MeasureString(item._text, this.Font);
                                itemHeight = (int)txtSize.Height;
                                
                                break;
                            }
                    }
                    height = getMax(itemHeight, height);
                }
                
                drawSize.Height = height;

                foreach (wTreeNodeItem item in node.Items)
                {
                    int itemHeight=0;
                    int toY=0;
                    switch(item.ItemType){
                        case wTreeNodeItemTypes.CheckBox:
                            {
                                Image checkBox;
                                if (item._isChecked == true)
                                {
                                    checkBox = Properties.Resources.check_red;
                                }
                                else if (item._isChecked == false)
                                {
                                    checkBox = Properties.Resources.normal;
                                }
                                else
                                {
                                    checkBox = Properties.Resources.inter;
                                }

                                itemHeight = checkBox.Height;
                                toY = getCenter(itemHeight, height)+y;
                                g.DrawImage(Properties.Resources.check_red, new Point(width, toY));
                                width += checkBox.Width + _itemMargin;

                                break;
                            }
                        case wTreeNodeItemTypes.Image:
                            {
                                itemHeight = item._image.Height;
                                toY = getCenter(itemHeight, height) + y;
                                
                                g.DrawImage(item._image, new Point(width, toY));
                                width += item._image.Width + _itemMargin;
                                break;
                            }
                        case wTreeNodeItemTypes.Text:
                            {
                                SizeF txtSize = g.MeasureString(item._text, this.Font);
                                itemHeight = (int)txtSize.Height;
                                toY = getCenter(itemHeight, height) + y;

                                g.DrawString(item._text, this.Font, new SolidBrush(this.ForeColor), new Point(width, toY));
                                width += (int)txtSize.Width + _itemMargin;
                                
                                break;
                            }
                    }
                }
                
            }
            drawSize.Width = Width;
            return drawSize;
        }


        public int Depth
        {
            get { return 0; }
        }

        public IwTreeNodeCollectionParent TreeParent
        {
            get { return null; }
        }
    }

 

    
   

   



 
}
