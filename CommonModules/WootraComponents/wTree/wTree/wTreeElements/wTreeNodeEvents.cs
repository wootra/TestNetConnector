using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace WootraComs.wTreeElements
{
    /// <summary>
    /// index는 몇번째 item이 바뀌었는지를 나타낸다.
    /// </summary>
    /// <param name="treeNode"></param>
    /// <param name="itemIndex"></param>
    internal delegate void wTreeNodeChangedEventHandler(wTreeNode treeNode, int itemIndex);

    /// <summary>
    /// 어떤 리스트가 바뀌었는지를 나타낸다.
    /// </summary>
    /// <param name="treeNode"></param>
    internal delegate void wTreeListChangedEventHandler(wTreeNode treeNode);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    internal delegate void wTreeNodeItemChanged(wTreeNodeItem item);

    /// <summary>
    /// 각 노드들을 user defined으로 그릴 때 이벤트이다.
    /// </summary>
    /// <param name="e">그려야 할 정보와 그렸는지 여부를 가진 argument</param>
    public delegate void wTreeNodeDrawingEvent(Graphics g, wTreeNodeDrawingArgs e);

    /// <summary>
    /// User가 그릴 정보와 그렸는지 여부를 가지고 있는 클래스이다.
    /// </summary>
    public class wTreeNodeDrawingArgs{
        internal wTreeNodeDrawingArgs(wTreeNode nodeToDraw, Point startPoint)
        {
            _items = new List<wTreeNodeItem>();
            foreach(wTreeNodeItem item in nodeToDraw.Items){
                _items.Add(item);
            }
            _nodeToDraw = nodeToDraw;
            StartPoint = startPoint;
            DrawnSize = Size.Empty;
            UserDrawn = false;
        }
        List<wTreeNodeItem> _items;
        /// <summary>
        /// 내부에 있는 item들
        /// </summary>
        public List<wTreeNodeItem> Items { get{ return _items;} }
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
        /// <summary>
        /// 그리고 난뒤 true로 해 주면 자동으로 그려지지 않는다.
        /// </summary>
        public bool UserDrawn { get; set; }
    }
}
