using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    #region TreeNodeClickEvent
    public delegate void TreeNodeClickEventHandler(Object sender, TreeNodeClickEventArg e);

    public class TreeNodeClickEventArg : EventArgs
    {
        public bool? Checked { get; set; }
        public TriStateTreeNode Node { get; set; }
        public TreeView TreeView { get; set; }
        public TreeNodeClickEventArg(bool? checkedState, TreeNode node, TreeView treeView)
        {
            Checked = checkedState;
            Node = node as TriStateTreeNode;
            TreeView = treeView;
        }
    }
    #endregion
}
