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

namespace RtwWpfControls
{
    #region ListRowClickEvent
    public delegate void ListRowClickEventHandler(Object sender, ListRowClickEventArgs e);
    
    public class ListRowClickEventArgs:EventArgs
    {
        public ListBox ListObj { get; set; }
        public RtwListRow ListRowItem { get; set; }
        public UIElement SelectedItem { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public UIElement Source { get; set; }
        public UIElement OriginalSource { get; set; }
        public object Returns { get; set; }
        public ListRowClickEventArgs(int rowIndex, int colIndex, RtwListRow row,
            UIElement selectedItem=null, UIElement source=null, UIElement originalSource=null,
            ListBox listObj=null)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
            ListRowItem = row;
            SelectedItem = selectedItem;
            ListObj = listObj;
            Source = source;
            OriginalSource = originalSource;
        }
    }
    #endregion

    #region ListRowClickEvent
    public delegate void ListComboBoxEventHandler(Object sender, ListComboBoxEventArgs e);

    public class ListComboBoxEventArgs : EventArgs
    {
        public ListBox ListObj { get; set; }
        public RtwListRow ListRowItem { get; set; }
        public int SelectedIndex { get; set; }
        public object SelectedObject { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public UIElement Source { get; set; }
        public UIElement OriginalSource { get; set; }
    }
    #endregion



    #region ListCheckBoxChecked

    public delegate void ListCheckedEventHandler(Object sender, ListCheckedEventArgs e);
    
    public class ListCheckedEventArgs : EventArgs
    {
        public bool? Checked { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public UIElement Source { get; set; }
        public UIElement OriginalSource { get; set; }
        public ListCheckedEventArgs(bool? isChecked, int row_index, int col_index=0)
        {
            Checked = isChecked;
            RowIndex = row_index;
            ColumnIndex = col_index;
        }
    }
    #endregion

    #region TreeNodeClickEvent
    public delegate void TreeNodeClickEventHandler(Object sender, TreeNodeClickEventArg e);

    public class TreeNodeClickEventArg : EventArgs
    {
        public List<String> TreePath { get; set; }
        public List<int> TreeIndexPath { get; set; }
        public Object Checked { get; set; }
        public RtwTreeNode NodeSource { get; set; }
        public TreeViewItem Item { get; set; }
        public ContentPresenter ContentPresenter { get; set; }
        public TreeNodeClickEventArg(List<String> treePath, List<int> treeIndexPath, Object checkedState, RtwTreeNode node, TreeViewItem item, ContentPresenter p)
        {
            TreePath = treePath;
            TreeIndexPath = treeIndexPath;
            Checked = checkedState;
            NodeSource = node;
            Item = item;
            ContentPresenter = p;
        }
    }
    #endregion

    #region TreeNodeClickEvent
    public delegate void TreeNodeCheckedEventHandler(Object sender, TreeNodeCheckedEventArg e);

    public class TreeNodeCheckedEventArg : EventArgs
    {
        public List<RtwTreeNode> SelectedNodes { get; set; }
        public List<RtwTreeNode> AddedCheckNodes { get; set; }
        public List<RtwTreeNode> RemovedCheckNodes { get; set; }
        public TreeNodeCheckedEventArg(List<RtwTreeNode> selected,List<RtwTreeNode> added, List<RtwTreeNode> removed )
        {
            SelectedNodes = selected;
            AddedCheckNodes = added;
            RemovedCheckNodes = removed;
        }
    }
    #endregion
}
