using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ListViews
{

    public class NotifyClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Boolean _isChecking = false;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                if (_isChecking) return;
                _isChecking = true;//무한반복을 막기 위함.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                _isChecking = false;
            }
        }
    }

    public class TreeNode : NotifyClass
    {
        String _name = String.Empty;
        Object _isChecked = false;
        Boolean _useCheckBox = true;

        TreeNode _parent = null;
        String _tag = null;
        List<TreeNode> _children = new List<TreeNode>();
        Object _relativeObj=null;
        
        /// <summary>
        /// 트리를 구성하는 하나의 노드를 정의한다.
        /// </summary>
        /// <param name="name">트리이름.같은 부모를 가진 계층 안에서 unique하여야 한다.</param>
        /// <param name="Checked">체크박스가 체크될 지</param>
        /// <param name="parent">부모노드를 지정한다. 부모가 없으면 null.즉, </param>
        /// <param name="_relativeObject"></param>
        public TreeNode(String name, Object Checked, Object relativeObject=null, TreeNode parent = null)
        {
            init(name, name, Checked, relativeObject, parent);
            _name = name;
            _parent = parent;
            _isChecked = (Checked == null) ? this._isChecked = false : _isChecked = Checked;
            _relativeObj = relativeObject;
            Tag = "Child";
            Text = name;
        }

        public TreeNode(String name, String text, Object Checked, Object relativeObject, TreeNode parent=null)
        {
            _name = name;
            _parent = null;
            _isChecked = (Checked == null) ? this._isChecked = false : _isChecked = Checked;
            _relativeObj = relativeObject;
            Tag = "Child";
            Text = text;
        }

        public void init(String name, String text, Object Checked, Object relativeObject, TreeNode parent)
        {
            _name = name;
            _parent = null;
            _isChecked = (Checked == null) ? this._isChecked = false : _isChecked = Checked;
            _relativeObj = relativeObject;
            Tag = "Child";//기본값은 Child, 자식이 생기면 Parent
            Text = text;
            UserDisign = new UserControl();

        }
        public UserControl UserDisign { get; set; }

        public String Text { get; set; }

        public Object RelativeObject
        {
            get
            {
                return _relativeObj;
            }
            set
            {
                _relativeObj = value;
            }
        }

        public String Tag {
            get{ return _tag;}
            set
            {
                _tag = value;
                NotifyPropertyChanged("Tag");
            }
        }

        public List<String> Path
        {
            get
            {
                List<String> path;
                if (Parent != null)
                {
                    path = Parent.Path;
                }else{
                   path = new List<string>();
                }
                path.Add(this.Name);
                return path;
            }
        }

        public List<int> IndexPath
        {
            get
            {
                if(_parent!=null){
                    List<int> path = _parent.IndexPath;
                    path.Add(_parent.IndexOfChild(this));
                    return path;
                }else{
                    List<int> path = new List<int>();
                    path.Add(-1);
                    return path;
                }
            }
        }
        public int IndexOfChild(TreeNode node)
        {
            return _children.IndexOf(node);
        }

        public String Name
        {
            get { return _name; }
            set
            {
                this._name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public void CheckRecursive(Object checkState)
        {
            Checked = checkState;
            if (Children.Count > 0)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].CheckRecursive(checkState);
                }
            }
        }

        public Object Checked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                NotifyPropertyChanged("Checked");
            }
        }
        public Boolean UseCheckBox
        {
            get { return _useCheckBox; }
            set { _useCheckBox = value; }
        }

        public TreeNode Parent
        {
            set { _parent = value; }
            get { return _parent; }
        }

        public List<TreeNode> SelectedSiblings
        {
            get
            {
                List<TreeNode> siblings = new List<TreeNode>();
                if (Parent != null)
                {
                    foreach (TreeNode sib in Parent.Children)
                    {
                        if(sib.Children.Count==0 && sib.Checked.Equals(true)) siblings.Add(sib);
                    }
                }
                return siblings;
            }
        }

        public List<TreeNode> AllSelectedEndNodes
        {
            get
            {
                List<TreeNode> endNodes = new List<TreeNode>();
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].Children.Count == 0)
                    {
                        if(Children[i].Checked.Equals(true)) endNodes.Add(Children[i]);
                    }
                    else endNodes.AddRange(Children[i].AllSelectedEndNodes);
                }
                return endNodes;
            }
        }

        public List<TreeNode> AllEndNodes
        {
            get
            {
                List<TreeNode> endNodes = new List<TreeNode>();
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].Children.Count == 0) endNodes.Add(Children[i]);
                    else endNodes.AddRange(Children[i].AllEndNodes);
                }
                return endNodes;
            }
        }

        public TreeNode Root
        {
            get
            {
                TreeNode parent = Parent;
                TreeNode root = this;
                while (parent != null)
                {
                    root = parent;
                    parent = parent.Parent;
                }
                return root;
            }
        }

        public void CheckSelf()
        {
            int trueNum = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Checked.ToString().ToLower().Equals("true")) trueNum++;
            }
            if (trueNum == 0) Checked = false;
            else if (trueNum == Children.Count) Checked = true;
            else Checked = null;

            if (Parent != null) Parent.CheckSelf();
        }

        public List<TreeNode> Children
        {
            get { return _children; }
        }

        public void AddChild(TreeNode child)
        {
            _children.Add(child);
            child.Parent = this;
            Tag = "Parent";
        }
    }
}
