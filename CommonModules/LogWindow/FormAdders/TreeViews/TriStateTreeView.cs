using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using IOHandling;
using System.Drawing;

namespace FormAdders
{
    [Browsable(false)]
    [System.ComponentModel.Designer(typeof(TriStateTreeViewDesigner))]
    public class CustomStateTreeView : System.Windows.Forms.TreeView
    {
        #region fields
        #endregion

        #region ctor
        public CustomStateTreeView()
            : base()
        {
            if (System.Windows.Forms.VisualStyles.VisualStyleInformation.IsSupportedByOS)
            {
                this.ShowRootLines = false;
                this.ShowLines = false;
            }
        }
        #endregion

        #region public interface
        #endregion

        #region internal interface
        protected virtual void CreateStateImages()
        {
        }
        protected virtual void OnCustomCheck(System.Windows.Forms.TreeNode node, System.Windows.Forms.TreeViewAction action)
        {
            System.Windows.Forms.TreeViewCancelEventArgs e = new System.Windows.Forms.TreeViewCancelEventArgs(node, false, action);
            OnBeforeCheck(e);
            //
            if (node is IStateTreeNode)
            {
                ((IStateTreeNode)node).UpdateState(e);
            }
            //
            if (e.Cancel) return;

            OnAfterCheck(new System.Windows.Forms.TreeViewEventArgs(node, action));

        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //
            StateImageList = new System.Windows.Forms.ImageList();
            CreateStateImages();
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.TreeViewHitTestInfo info = this.HitTest(e.X, e.Y);
            if (info.Node != null && info.Location.ToString() == "StateImage")
            {
                OnCustomCheck(info.Node, System.Windows.Forms.TreeViewAction.ByMouse);
            }
            //
            base.OnMouseDown(e);
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            System.Windows.Forms.TreeViewHitTestInfo info = this.HitTest(e.X, e.Y);
            if (info.Node != null && info.Location.ToString() == "StateImage")
            {
                return;
                //OnCustomCheck(info.Node, System.Windows.Forms.TreeViewAction.ByMouse);
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Space:
                    if (this.StateImageList != null && this.SelectedNode != null)
                    {
                        OnCustomCheck(SelectedNode, System.Windows.Forms.TreeViewAction.ByKeyboard);
                    }
                    e.Handled = true;
                    break;
            }
            base.OnKeyDown(e);
        }
        #endregion
    }
    /// <summary>
    /// Treenode that supports tristate checkboxes
    /// </summary>
    public class TriStateTreeNode : System.Windows.Forms.TreeNode, IStateTreeNode
    {
        #region fields
        private System.Windows.Forms.CheckState _checkState = CheckState.Unchecked;
        Dictionary<String, Object> _relativeObject = new Dictionary<string, object>();
        #endregion

        #region ctor

        public TriStateTreeNode(string text, String relObjName = null, Object relObj = null)
            : base(text)
        {
            _nodes = new TriStateTreeNodeCollection(base.Nodes);
            if (this.Name != null || this.Name.Length == 0) this.Name = text;
            this.Checked = false;
            if (relObjName != null && relObj != null) { _relativeObject[relObjName] = relObj; }
            
        }

        



        public TriStateTreeNode(TreeNode node, String relObjName = null, Object relObj = null)
            : base(node.Text)
        {
            if (node.Name != null && node.Name.Length > 0) this.Name = node.Name;
            else this.Name = node.Text;
            this.Checked = node.Checked;
            if (relObjName != null && relObj != null) { _relativeObject[relObjName] = relObj; }
        }

        public TriStateTreeNode(TriStateTreeNode node, String relObjName = null, Object relObj = null)
            : base(node.Text)
        {
            if (node.Name != null && node.Name.Length > 0) this.Name = node.Name;
            else this.Name = node.Text;
            this.CheckState = node.CheckState;
            if (relObjName != null && relObj != null) { _relativeObject[relObjName] = relObj; }
        }

        #endregion

        public Dictionary<String, Object> RelativeObject
        {
            get
            {
                return _relativeObject;
            }
        }

        public new TriStateTreeNode Parent
        {
            get
            {
                return base.Parent as TriStateTreeNode;
            }
        }

        public new Rectangle Bounds
        {
            get
            {
                return new Rectangle(base.Bounds.X - 40, base.Bounds.Y, base.Bounds.Width + 40, base.Bounds.Height);
            }
        }

        public new bool? Checked
        {
            get
            {

                if (_checkState == System.Windows.Forms.CheckState.Checked) return true;
                //else if (_checkState == System.Windows.Forms.CheckState.Indeterminate) return (bool?)null;
                else return false;
                //return _checkState == System.Windows.Forms.CheckState.Checked;
            }
            set
            {
                _checkState = (value == true) ? System.Windows.Forms.CheckState.Checked : (value == false) ? System.Windows.Forms.CheckState.Unchecked : System.Windows.Forms.CheckState.Indeterminate;
                StateImageIndex = (int)_checkState;
                if (_checkState == System.Windows.Forms.CheckState.Checked) base.Checked = true;
                else if (_checkState == System.Windows.Forms.CheckState.Unchecked) base.Checked = false;
                else base.Checked = false;

                //base.Checked = (_checkState == System.Windows.Forms.CheckState.Checked);
                //System.Windows.Forms.TreeViewCancelEventArgs e = new
                //System.Windows.Forms.TreeViewCancelEventArgs(this, value, TreeViewAction.ByMouse);
                //      ((IStateTreeNode)this).UpdateState(e);


            }
        }

        public virtual System.Windows.Forms.CheckState CheckState
        {
            get
            {
                return _checkState;
            }
            set
            {
                //_checkState = (value == System.Windows.Forms.CheckState.Indeterminate) ? System.Windows.Forms.CheckState.Unchecked : value;
                _checkState = value;
                //
                StateImageIndex = (int)_checkState;
                /*
                if (_checkState == System.Windows.Forms.CheckState.Checked) base.Checked = true;
                else if (_checkState == System.Windows.Forms.CheckState.Unchecked) base.Checked = false;
                else base.Checked = false;
                 */
            }
        }
        public void setTreeView(TreeView treeView)
        {
            _nodes.setTreeView(treeView);
        }

        void IStateTreeNode.UpdateState(System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            switch (CheckState)
            {
                case System.Windows.Forms.CheckState.Checked:
                    CheckState = System.Windows.Forms.CheckState.Checked;
                    break;
                case System.Windows.Forms.CheckState.Indeterminate:
                    this.CheckState = System.Windows.Forms.CheckState.Indeterminate;
                    break;
                case System.Windows.Forms.CheckState.Unchecked:
                    CheckState = System.Windows.Forms.CheckState.Unchecked;
                    break;
            }
        }

        TriStateTreeNodeCollection _nodes;
        public new TriStateTreeNodeCollection Nodes
        {
            get { return _nodes; }
        }
    }

    public interface IStateTreeNode
    {
        void UpdateState(System.Windows.Forms.TreeViewCancelEventArgs e);
    }

    /// <summary>
    /// A simple designer class for the <see cref="TreeViewFolderBrowser"/> control to remove 
    /// unwanted properties at design time.
    /// </summary>
    public class TriStateTreeViewDesigner
    : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Allows a designer to change or remove items from the set of properties that it exposes through a TypeDescriptor. 
        /// </summary>
        /// <param name="properties">The properties for the class of the component.</param>
        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            properties.Remove("CheckBoxes");
            properties.Remove("StateImageList");
        }
    };
    public static class Converters
    {
        public static TreeNode ConvertToTreeNode(TriStateTreeNode node) { return node; }
        public static TriStateTreeNode ConvertToTriStateTreeNode(TreeNode node)
        {
            if (node is TriStateTreeNode) return node as TriStateTreeNode;
            else return new TriStateTreeNode(node);
        }
    }
    public class TriStateTreeNodeCollection : ICollection<TriStateTreeNode>
    {
        //List<TriStateTreeNode> _list = new List<TriStateTreeNode>();
        //Dictionary<String, TriStateTreeNode> _dic = new Dictionary<string, TriStateTreeNode>();
        TreeNodeCollection _tnc;
        TreeView _treeView;
        public TriStateTreeNodeCollection(TreeNodeCollection tnc, TreeView treeView = null)
        {
            _tnc = tnc;
            _treeView = treeView;
        }

        public void setTreeView(TreeView treeView)
        {
            _treeView = treeView;
        }

        public new TriStateTreeNode this[int index]
        {
            get
            {
                return _tnc[index] as TriStateTreeNode;
            }
        }

        public new TriStateTreeNode this[String name]
        {
            get
            {
                return _tnc[name] as TriStateTreeNode;
            }
            set
            {
                if (_tnc.ContainsKey(name))
                {
                    int index = _tnc.IndexOf(_tnc[name]);//함께 갱신해야 하므로..
                    _tnc.RemoveByKey(name);
                    if (value is TriStateTreeNode)
                    {
                        value.Name = name;
                        _tnc.Insert(index, value);
                    }
                    else
                    {
                        TriStateTreeNode node = new TriStateTreeNode(value);
                        value.Name = name;
                        _tnc.Insert(index, value);
                    }
                }
                else
                {
                    value.Name = name;
                    _tnc.Add(value);
                }
            }
        }

        int AddNode(TriStateTreeNode node)
        {
            int num = _tnc.Add(node);
            if (_treeView != null)
            {
                node.setTreeView(_treeView);
                if (_treeView.ImageList.Images.Count > 2 && node.Parent!=null)
                {
                    node.Parent.ImageIndex = 2;
                    node.Parent.SelectedImageIndex = 2;
                }
            }
            return num;
        }

        void AddNodeRange(TriStateTreeNode[] node)
        {
            _tnc.AddRange(node);
            if (_treeView != null)
            {
                if (_treeView.ImageList.Images.Count > 2)
                {
                    for (int i = 0; i < node.Length; i++)
                    {
                        (node[i] as TriStateTreeNode).setTreeView(_treeView);

                    }
                    node[0].Parent.ImageIndex = 2;
                    node[0].Parent.SelectedImageIndex = 2;
                }
            }
        }

        void InsertNode(int index, TriStateTreeNode node)
        {
            _tnc.Insert(index, node);
            if (_treeView != null)
            {
                node.setTreeView(_treeView);
                if (_treeView.ImageList.Images.Count > 2)
                {
                    node.Parent.ImageIndex = 2;
                    node.Parent.SelectedImageIndex = 2;
                }
            }
        }

        public TriStateTreeNode Add(string key, string text, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            AddNode(node);
            return node;
        }
        public TriStateTreeNode Add(string key, string text, int imageIndex, int selectedImageIndex, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageIndex = imageIndex;
            node.SelectedImageIndex = selectedImageIndex;
            AddNode(node);
            return node;
        }
        public TriStateTreeNode Add(string key, string text, int imageIndex, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageIndex = imageIndex;
            node.SelectedImageIndex = imageIndex;
            AddNode(node);
            return node;
        }
        public TriStateTreeNode Add(string key, string text, string imageKey, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageKey = imageKey;
            node.SelectedImageKey = imageKey;
            AddNode(node);
            return node;
        }
        public TriStateTreeNode Add(string key, string text, string imageKey, string selectedImageKey, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageKey = imageKey;
            node.SelectedImageKey = selectedImageKey;
            AddNode(node);
            return node;
        }
        public TriStateTreeNode Add(string text, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            //node.Checked = false;
            node.CheckState = CheckState.Unchecked;

            AddNode(node);
            return node;
        }
        public int Add(TreeNode inNode)
        {
            TriStateTreeNode node = (inNode is TriStateTreeNode) ? inNode as TriStateTreeNode : new TriStateTreeNode(inNode);
            int num = AddNode(node);
            
            return num;
        }
        public void AddRange(TreeNode[] nodes)
        {
            TriStateTreeNode[] arr = new TriStateTreeNode[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] is TriStateTreeNode) arr[i] = nodes[i] as TriStateTreeNode;
                else arr[i] = new TriStateTreeNode(nodes[i]);
            }
            AddNodeRange(arr);

        }

        public void AddRange(List<TreeNode> nodes)
        {
            TriStateTreeNode[] arr = new TriStateTreeNode[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is TriStateTreeNode) arr[i] = nodes[i] as TriStateTreeNode;
                else arr[i] = new TriStateTreeNode(nodes[i]);
            }
            AddNodeRange(arr);
        }

        public void AddRange(TriStateTreeNode[] nodes)
        {
            TriStateTreeNode[] arr = new TriStateTreeNode[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                arr[i] = nodes[i];
            }
            AddNodeRange(arr);
        }

        public void AddRange(List<TriStateTreeNode> nodes)
        {
            TriStateTreeNode[] arr = new TriStateTreeNode[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                arr[i] = nodes[i];
            }
            AddNodeRange(arr);
        }

        public TriStateTreeNode Insert(int index, string key, string text, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            InsertNode(index, node);
            return node;

        }
        public TriStateTreeNode Insert(int index, string key, string text, int imageIndex, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageIndex = imageIndex;
            InsertNode(index, node);
            return node;
        }
        public int IndexOfKey(string key)
        {
            return _tnc.IndexOfKey(key);
        }
        public TriStateTreeNode Insert(int index, string key, string text, int imageIndex, int selectedImageIndex, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageIndex = imageIndex;
            node.SelectedImageIndex = selectedImageIndex;
            InsertNode(index, node);
            return node;
        }
        public TriStateTreeNode Insert(int index, string key, string text, string imageKey, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageKey = imageKey;
            node.SelectedImageKey = imageKey;
            InsertNode(index, node);
            return node;
        }
        public TriStateTreeNode Insert(int index, string key, string text, string imageKey, string selectedImageKey, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            node.Name = key;
            node.ImageKey = imageKey;
            node.SelectedImageKey = selectedImageKey;
            InsertNode(index, node);
            return node;

        }
        public TriStateTreeNode Insert(int index, string text, String relObjName = null, Object relObj = null)
        {
            TriStateTreeNode node = new TriStateTreeNode(text, relObjName, relObj);
            InsertNode(index, node);
            return node;

        }
        public void Insert(int index, TreeNode node)
        {
            if (node is TriStateTreeNode) _tnc.Insert(index, node);
            else
            {
                InsertNode(index, new TriStateTreeNode(node));
            }
        }
        public void RemoveAt(int index)
        {
            _tnc.RemoveAt(index);
        }
        public void RemoveByKey(string key)
        {
            _tnc.RemoveByKey(key);
        }
        public bool ContainsKey(string key)
        {
            return _tnc.ContainsKey(key);
        }

        public void Add(TriStateTreeNode item)
        {
            this[item.Text] = item;
        }

        public void Clear()
        {
            _tnc.Clear();
        }

        public bool Contains(TriStateTreeNode item)
        {
            return _tnc.Contains(item);
        }




        public void CopyTo(TriStateTreeNode[] array, int arrayIndex)
        {
            array[arrayIndex] = this[arrayIndex] as TriStateTreeNode;
        }

        public void CopyTo(TriStateTreeNode[] array)
        {
            for (int i = 0; i < _tnc.Count; i++)
            {
                array[i] = _tnc[i] as TriStateTreeNode;
            }
        }

        public void CopyTo(TreeNode[] array, int arrayIndex)
        {
            array[arrayIndex] = this[arrayIndex];
        }

        public List<T> ToList<T>() where T : TreeNode, IStateTreeNode
        {
            List<T> list = new List<T>();
            for (int i = 0; i < _tnc.Count; i++)
            {
                list.Add(this[i] as T);
            }
            return list;
        }
        public List<TriStateTreeNode> ToList()
        {
            List<TriStateTreeNode> list = new List<TriStateTreeNode>();
            for (int i = 0; i < _tnc.Count; i++)
            {
                list.Add(this[i]);
            }

            return list;
        }

        public int IndexOf(TreeNode node)
        {
            return _tnc.IndexOf(node);
        }

        public int Count
        {
            get
            {
                return _tnc.Count;
            }
        }

        public bool Contains(TreeNode item)
        {
            return _tnc.Contains(item);
        }

        public bool IsReadOnly
        {
            get { return _tnc.IsReadOnly; }
        }

        public bool Remove(TreeNode item)
        {
            if (_tnc.Contains(item))
            {
                _tnc.Remove(item);
                return true;
            }
            else return false;
        }

        public IEnumerator<TriStateTreeNode> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _tnc.GetEnumerator();
        }


        public bool Remove(TriStateTreeNode item)
        {
            if (_tnc.Contains(item))
            {
                _tnc.Remove(item as TreeNode);
                return true;
            }
            else return false;
        }
    }
}
