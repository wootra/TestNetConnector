using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandling
{
    public class TreeDataNode
    {
        String _name = String.Empty;

        TreeDataNode _parent = null;
        List<TreeDataNode> _children = new List<TreeDataNode>();
        Object _relativeObj = null;

        /// <summary>
        /// 트리를 구성하는 하나의 노드를 정의한다.
        /// </summary>
        /// <param name="name">트리이름.같은 부모를 가진 계층 안에서 unique하여야 한다.</param>
        /// <param name="Checked">체크박스가 체크될 지</param>
        /// <param name="parent">부모노드를 지정한다. 부모가 없으면 null.즉, </param>
        /// <param name="_relativeObject"></param>
        public TreeDataNode(String name, Object Checked, Object relativeObject = null, TreeDataNode parent = null)
        {
            init(name, name, relativeObject, parent);
        }

        public TreeDataNode(String name, String text, Object relativeObject, TreeDataNode parent = null)
        {
            init(name, text,  relativeObject, parent);
        }

        public void init(String name, String text, Object relativeObject, TreeDataNode parent)
        {
            _name = name;
            _parent = parent;
            _relativeObj = relativeObject;
            Text = text;
        }

        public String Text { get; set; }

        public String Spliter = ".";

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

  
        public String PathNameStr
        {
            get
            {
                String path;
                if (Parent != null) path = Parent.PathNameStr + Spliter;
                else path = "";
                return path + this._name;
            }
        }

        public String PathTextStr
        {
            get
            {
                String path;
                if (Parent != null) path = Parent.PathTextStr + Spliter;
                else path = "";
                return path + this._name;
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
                }
                else
                {
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
                if (_parent != null)
                {
                    List<int> path = _parent.IndexPath;
                    path.Add(_parent.IndexOfChild(this));
                    return path;
                }
                else
                {
                    List<int> path = new List<int>();
                    path.Add(-1);
                    return path;
                }
            }
        }
        public int IndexOfChild(TreeDataNode node)
        {
            return _children.IndexOf(node);
        }

        public String Name
        {
            get { return _name; }
            set
            {
                this._name = value;
            }
        }

        public TreeDataNode Parent
        {
            get { return _parent; }

            set
            {
                _parent = value;
                //ToolTipText = PathNameStr;
            }
        }

        public List<TreeDataNode> AllEndNodes
        {
            get
            {
                List<TreeDataNode> endNodes = new List<TreeDataNode>();
                if (Children.Count > 0)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        endNodes.AddRange(Children[i].AllEndNodes);

                    }
                }
                else
                {
                    endNodes.Add(this);
                }

                return endNodes;
            }
        }

        public TreeDataNode Root
        {
            get
            {
                TreeDataNode parent = Parent;
                TreeDataNode root = this;
                while (parent != null)
                {
                    root = parent;
                    parent = parent.Parent;
                }
                return root;
            }
        }

 
        public List<TreeDataNode> Children
        {
            get { return _children; }
        }

        public void AddChild(TreeDataNode child)
        {
            _children.Add(child);
            child.Parent = this;
        }
    }
}
