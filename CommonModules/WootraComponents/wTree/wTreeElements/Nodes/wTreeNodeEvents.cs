using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace WootraComs.wTreeElements
{
    /// <summary>
    /// index는 몇번째 item이 바뀌었는지를 나타낸다. index는 treeNode의 parent에서 몇번째 item인지를 나타낸다.
    /// </summary>
    /// <param name="treeNode"></param>
    /// <param name="itemIndex"></param>
    public delegate void wTreeNodeChangedEventHandler(wTreeNode treeNode, wTreeNodeChangedEventArgs e);


    public class wTreeNodeChangedEventArgs
    {
        internal wTreeNodeChangedEventArgs(int itemIndex)
        {
            _itemIndex = itemIndex;
            IsHandled = false;
        }
        int _itemIndex;
        public int ItemIndex { get { return _itemIndex; } }
        
        /// <summary>
        /// 처리가 되었는지..setting함.
        /// </summary>
        public bool IsHandled { get; set; }
    }

    /// <summary>
    /// 바뀐 이름이 정상이면 true를, 아니면 false를 리턴한다.
    /// </summary>
    /// <param name="node">이름이 바뀔 노드.</param>
    /// <param name="changedName">바뀔 이름.</param>
    /// <returns></returns>
    internal delegate bool wTreeNodeNameChanged(wTreeNode node, string changedName, ref String errMsg);

    /// <summary>
    /// 어떤 리스트가 바뀌었는지를 나타낸다.
    /// </summary>
    /// <param name="treeNode"></param>
    public delegate void wTreeListChangedEventHandler(IwTreeNodeCollectionParent treeParent);


    public delegate void wTreeKeyEventHandler(wTreeNode node, KeyEventArgs e);


    /// <summary>
    /// Node 내부에 존재하는 Item이 변경되었을 때 발생함.
    /// </summary>
    /// <param name="item"></param>
    public delegate void wTreeNodeItemChanged(wTreeNodeItem item);

    /// <summary>
    /// Node 내부에 존재하는 Item이 변경되었을 때 발생함.
    /// </summary>
    /// <param name="item"></param>
    public delegate void wTreeNodeItemMouseEvent(wTreeNode node, wTreeNodeItem item, wTreeNodeItemMouseArgs e);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="item"></param>
    /// <param name="AreaChanged">Node영역이 다른 영역으로 진입했을 때 true임..</param>
    /// <param name="e"></param>
    public delegate void wTreeNodeItemMouseMoveEvent(wTreeNode node, wTreeNodeItem item, bool AreaChanged, wTreeNodeItemMouseArgs e);

    public class wTreeNodeItemMouseArgs : MouseEventArgs
    {
        public wTreeNodeItemMouseArgs(MouseEventArgs e):base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            IsCanceled = false;
        }

        public bool IsCanceled{get;set;}
    }

    public delegate void wTreeNodeItemSelectedEvent(wTreeNode node, wTreeNodeItem item);

    public delegate void wTreeNodeItemSelectingEvent(wTreeNode node, wTreeNodeItem item, wTreeNodeItemSelectingArgs args);
    public class wTreeNodeItemSelectingArgs
    {
        public wTreeNodeItemSelectingArgs()
        {
            IsCancel = false;
        }
        /// <summary>
        /// Selection을 취소하고싶으면 true..
        /// </summary>
        public bool IsCancel { get; set; }
    }

    public delegate void wTreeNodeItemValueChanged(wTreeNode node, wTreeNodeItem item, object oldValue, object newValue);

    public delegate void wTreeNodeItemValueChangeCanceled(wTreeNode node, wTreeNodeItem item);
   
    public delegate void wTreeNodeCheckItemValueChanged(wTreeNode node, wTreeNodeItem item, bool newValue);

    public delegate void wTreeNodeCheckItemsValueChanged(wTreeNode node, Dictionary<wTreeNodeItem, bool> itemValues);

    public delegate void EditorValueChanged(wTreeNodeItem item, object oldValue, object newValue);


    public delegate void EditorValueChanging(wTreeNodeItem item, wEditorValueChangingArgs args);
    
    /// <summary>
    /// 값을 바꾸는 것을 취소시키려면 IsCanceled를 true로 셋팅한다.
    /// 값을 인위적으로 바꾸려면 NewValue의 값을 바꾸어준다.
    /// </summary>
    public class wEditorValueChangingArgs
    {
        public wEditorValueChangingArgs(object oldValue, object newValue)
        {
            IsCanceled = false;
            OldValue = oldValue;
            NewValue = newValue;
        }
        public bool IsCanceled { get; set; }
        public object OldValue { get; private set; }
        public object NewValue { get; set; }
    }


    /// <summary>
    /// 에디트가 시작된 이후 호출된다.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="editor"></param>
    public delegate void wTreenodeEditorVisibleChanged(wTreeNodeItem item, wTreeEditor editor);

    /// <summary>
    /// 각 노드들을 user defined으로 그릴 때 이벤트이다.
    /// </summary>
    /// <param name="e">그려야 할 정보와 그렸는지 여부를 가진 argument</param>
    public delegate void wTreeNodeDrawingEvent(wTreeNodeDrawingArgs e);

    /// <summary>
    /// User가 그릴 정보와 그렸는지 여부를 가지고 있는 클래스이다.
    /// </summary>
    public class wTreeNodeDrawingArgs{
        internal wTreeNodeDrawingArgs(wTreeNode nodeToDraw, int x, int y)
        {
            _nodeToDraw = nodeToDraw;
            StartPoint = new Point(x,y);
            DrawnSize = Size.Empty;
            _userDrawn = false;
            DrawnBuffer = nodeToDraw.ImageBufferToDraw;
        }
        Graphics _g = null;
        /// <summary>
        /// 사용하려면 SetBufferSize를 먼저 셋팅해야 한다.
        /// </summary>
        public Graphics g
        {
            get { return _g; }
        }

        Image _buffToDraw;
        public Graphics SetBufferSize(int width, int height)
        {
            _buffToDraw = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            _userDrawn = true;
            _g = Graphics.FromImage(_buffToDraw);
            return _g;
        }
        
        public Graphics SetBuffer(Image bufferToDraw)
        {
            _buffToDraw = bufferToDraw;
            _userDrawn = true;
            _g = Graphics.FromImage(_buffToDraw);
            return _g;
        }

        internal wTreeNodeDrawingArgs(wTreeNode nodeToDraw, Point startPoint)
        {
            _nodeToDraw = nodeToDraw;
            StartPoint = startPoint;
            DrawnSize = Size.Empty;
            _userDrawn = false;
        }
        
        
        wTreeNode _nodeToDraw;
        /// <summary>
        /// 그려야 할 Node
        /// </summary>
        public wTreeNode NodeToDraw { get{ return _nodeToDraw;} }

        /// <summary>
        /// 그릴 시작포인트를 나타낸다. 만일 바꾸고 싶으면 바꾸어준다.
        /// </summary>
        public Point StartPoint{get;set;}

        /// <summary>
        /// 사용자가 그렸으면 그린 영역의 크기를 나타낸다.
        /// </summary>
        public Size DrawnSize { get; set; }

        bool _userDrawn = false;
        /// <summary>
        /// SetBuffer를 하면, true가 되어 Node버퍼가 갱신된다.
        /// 또는 g를 이용하여 그린 뒤 UserDrawn을 true로 셋팅해준다
        /// </summary>
        public bool UserDrawn { get { return _userDrawn; } set { _userDrawn = value; } }

        /// <summary>
        /// 노드를 그릴 버퍼.
        /// </summary>
        public Image DrawnBuffer {
            get { return _buffToDraw; }
            set {
                _buffToDraw = value;
                if (_buffToDraw != null)
                {
                    _g = Graphics.FromImage(_buffToDraw);
                }
            }
        }
    }
    
}
