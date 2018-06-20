using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    #region TreeNodeClickEvent
    public delegate void RtwTreeNodeCheckedEventHandler(Object sender, RtwTreeNodeCheckedEventArg e);

    public class RtwTreeNodeCheckedEventArg : EventArgs
    {
        public List<TriStateTreeNode> SelectedNodes { get; set; }
        public List<TriStateTreeNode> AddedCheckNodes { get; set; }
        public List<TriStateTreeNode> RemovedCheckNodes { get; set; }
        public int MinIndex=-1;
        public int MaxIndex=-1;

        /// <summary>
        /// 이전에 minIndex를 가리킨다. 기존의 min보다 컸다면 -1을 리턴한다.
        /// </summary>
        public int BeforeMinIndex=-1;

        /// <summary>
        /// 이전에 maxIndex를 가리킨다. 기존의 max보다 작았다면 -1을 리턴한다.
        /// </summary>
        public int BeforeMaxIndex = -1;
        public TriStateTreeNode Parent=null;
        public bool CheckState;
        public RtwTreeNodeCheckedEventArg(List<TriStateTreeNode> selected, List<TriStateTreeNode> added, List<TriStateTreeNode> removed)
        {
            SelectedNodes = selected;
            AddedCheckNodes = added;
            RemovedCheckNodes = removed;
        }
        public RtwTreeNodeCheckedEventArg(TriStateTreeNode parent, int minIndex, int maxIndex, int beforeMinIndex, int beforeMaxIndex, bool state)
        {
            MinIndex = minIndex;
            MaxIndex = maxIndex;
            BeforeMinIndex = beforeMinIndex;
            BeforeMaxIndex = beforeMaxIndex;
            Parent = parent;
            CheckState = state;
        }
    }
    #endregion
}
