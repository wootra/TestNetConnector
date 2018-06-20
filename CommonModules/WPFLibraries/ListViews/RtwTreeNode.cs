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

namespace RtwWpfControls
{

    public class RtwTreeNode : NotifyClass
    {
        String _name = String.Empty;
        bool? _isChecked = false;
        Boolean _useCheckBox = true;
        public event TreeNodeCheckChangedHandler E_CheckChanged;
        

        RtwTreeNode _parent = null;
        String _tag = null;
        List<RtwTreeNode> _children = new List<RtwTreeNode>();
        Dictionary<String, Object> _relativeObj = new Dictionary<string, object>();
        bool _enabled = true;
        String _toolTipText=null;

        static bool _isChecking = false; //부모노드의 환경을 체크중일 때는 이벤트가 발생하지 않도록 하기 위한 변수..

        /// <summary>
        /// 트리를 구성하는 하나의 노드를 정의한다.
        /// </summary>
        /// <param name="name">트리이름.같은 부모를 가진 계층 안에서 unique하여야 한다.</param>
        /// <param name="Checked">체크박스가 체크될 지</param>
        /// <param name="parent">부모노드를 지정한다. 부모가 없으면 null.즉, </param>
        /// <param name="_relativeObject"></param>
        public RtwTreeNode(String name, bool? Checked, String objName=null, Object relativeObject = null, RtwTreeNode parent = null)
        {
            init(name, name, Checked, objName, relativeObject, parent);
           
        }

        public RtwTreeNode(String name, String text, bool? Checked,String objName, Object relativeObject, RtwTreeNode parent = null)
        {
            init(name, text, Checked, objName, relativeObject, parent);
        }

        public void init(String name, String text, bool? Checked, String objName, Object relativeObject, RtwTreeNode parent)
        {
            _name = name;
            _parent = parent;
            _isChecked = (Checked == null) ? this._isChecked = false : _isChecked = Checked;
            _relativeObj[objName] = relativeObject;
            Tag = "Child";//기본값은 Child, 자식이 생기면 Parent
            Text = text;
            UserDisign = new UserControl();
            _enabled = true;
            if(parent!=null) ToolTipText = PathNameStr;
        }

        
        public Boolean Enabled {
            get { return _enabled; }
            set { 
                _enabled = value;
                NotifyPropertyChanged("Enabled");
                CheckSelf();
            }
        }


        public String ToolTipText
        {
            get {
                return "[" + Checked + "," + Enabled + "]";
                //return _toolTipText; 
            }
            set
            {
                _toolTipText = value;
                NotifyPropertyChanged("ToolTipText");
            }
        }

        public UserControl UserDisign { get; set; }

        public String Text { get; set; }
        
        public String Spliter = ".";

        public Dictionary<String, Object> RelativeObject
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
        public int IndexOfChild(RtwTreeNode node)
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

        public void CheckRecursive(bool? checkState, List<RtwTreeNode> Added, List<RtwTreeNode> Removed, List<RtwTreeNode> Selected)
        {
            if (Children.Count == 0)
            {
                if (checkState == true)
                {
                    if (Added != null && _isChecked == false) Added.Add(this);
                    if (Selected != null) Selected.Add(this);
                        /*
                    else
                    {
                        Selected = new List<TreeTableNode>();
                        Selected.Add(this);
                    }*/
                }
                else
                {
                    if (Removed != null && _isChecked == true) Removed.Add(this);
                }
            }
            Checked = checkState;
            if (Children.Count > 0)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if(Children[i].Enabled) Children[i].CheckRecursive(checkState,Added, Removed, Selected);
                }
            }
            //if (Children.Count == 0) CheckSelf();
        }

        public bool? Checked
        {
            get { return _isChecked; }
            set
            {
                bool isCheckChanged = (_isChecked != value);
                bool? before = _isChecked;
                _isChecked = value;
                NotifyPropertyChanged("Checked");
                if (Children.Count==0 && E_CheckChanged != null) E_CheckChanged(this, new TreeNodeCheckChangedArgs(this, _isChecked, before)); //EndNode일때만 이벤트가 발생됨.
                //if (E_CheckBoxChanged != null) E_CheckBoxChanged(this, new ListCheckedEventArgs(_isChecked, -1, 0));
                //CheckSelf();
            }
        }
        public Boolean UseCheckBox
        {
            get { return _useCheckBox; }
            set { _useCheckBox = value; }
        }

        public RtwTreeNode Parent
        {
            get{ return _parent;}

            set {
                _parent = value;
                //ToolTipText = PathNameStr;
            }
        }

        /// <summary>
        /// 같은계층에 있는 노드에서 null이나 true인 것만 가져옴. null인 경우 자식의 일부만이 true인 노드이다.
        /// </summary>
        public List<RtwTreeNode> SelectedSiblings
        {
            get
            {
                List<RtwTreeNode> siblings = new List<RtwTreeNode>();
                if (Parent != null)
                {
                    foreach (RtwTreeNode sib in Parent.Children)
                    {
                        if (sib.Enabled && (sib.Checked.Equals(false) == false)) siblings.Add(sib); //null이나 true인 sibling을 가져옴.
                    }
                }
                return siblings;
            }
        }

        public List<RtwTreeNode> AllSelectedEndNodes
        {
            get
            {
                List<RtwTreeNode> endNodes = new List<RtwTreeNode>();
                if (Children.Count > 0)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        endNodes.AddRange(Children[i].AllSelectedEndNodes);
                    }
                    return endNodes;
                }
                else
                {
                    if ((Boolean)Checked) endNodes.Add(this);
                    return endNodes;
                }

            }
        }

        public List<RtwTreeNode> AllDisabledSelectedEndNodes
        {
            get
            {
                List<RtwTreeNode> endNodes = new List<RtwTreeNode>();
                if (Children.Count > 0)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        endNodes.AddRange(Children[i].AllDisabledSelectedEndNodes);
                    }
                    return endNodes;
                }
                else
                {
                    if ((Boolean)Checked && Enabled==false) endNodes.Add(this);
                    return endNodes;
                }

            }
        }

        public List<RtwTreeNode> AllEnabledSelectedEndNodes
        {
            get
            {
                List<RtwTreeNode> endNodes = new List<RtwTreeNode>();
                if (Children.Count > 0)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        endNodes.AddRange(Children[i].AllEnabledSelectedEndNodes);
                    }
                    return endNodes;
                }
                else
                {
                    if((Boolean)Checked && Enabled) endNodes.Add(this);
                    return endNodes;
                }
                
            }
        }

        public List<RtwTreeNode> AllEndNodes
        {
            get
            {
                List<RtwTreeNode> endNodes = new List<RtwTreeNode>();
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

        public RtwTreeNode Root
        {
            get
            {
                RtwTreeNode parent = Parent;
                RtwTreeNode root = this;
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
            try
            {
                if (_isChecking) return;
                _isChecking = true;
                int trueNum = 0;
                Boolean disabled = false;
                Boolean midStates = false;

                if (Children.Count == 0)
                {
                    //맨 마지막 노드에서는 자기 자신의 값을 그대로 쓰면 된다.
                    _isChecking = false;
                    if (Parent != null) Parent.CheckSelf();
                    return;
                }

                for (int i = 0; i < Children.Count; i++)
                {
                    if (disabled == false)
                    {
                        if (Children[i].Enabled == false)
                        {
                            disabled = true;
                            Enabled = false;
                        }
                    }

                    if (midStates == false)
                    {
                        if (Children[i].Checked == null)
                        {
                            midStates = true;
                            Checked = null;
                        }
                    }

                    if (disabled && midStates)
                    {
                        break;
                    }


                    if (Children[i].Checked != null && Children[i].Checked.Equals(true)) trueNum++;
                }
                if (disabled == false) Enabled = true;
                if (!midStates)
                {
                    if (trueNum == 0) Checked = false;
                    else if (trueNum < Children.Count) Checked = null;
                    else Checked = true;
                }
                _isChecking = false;

                if (Parent != null) Parent.CheckSelf();
            }
            catch {
                throw;
            }
        }

        public List<RtwTreeNode> Children
        {
            get { return _children; }
        }

        public void AddChild(RtwTreeNode child)
        {
            _children.Add(child);
            child.Parent = this;
            Tag = "Parent";
            child.E_CheckChanged += new TreeNodeCheckChangedHandler(child_E_CheckChanged);
        }

        void child_E_CheckChanged(object sender, TreeNodeCheckChangedArgs args)
        {
            if (E_CheckChanged != null) E_CheckChanged(sender, args);
        }

        

        public static Boolean IsChecking()
        {
            return _isChecking;
        }
    }
	
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

    public delegate void TreeNodeCheckChangedHandler(object sender, TreeNodeCheckChangedArgs args);

    public class TreeNodeCheckChangedArgs : EventArgs
    {
        public bool? BeforeCheckState;
        public bool? CheckState;
        public RtwTreeNode Node;
        public TreeNodeCheckChangedArgs(RtwTreeNode node, bool? checkState, bool? beforeCheckState)
        {
            CheckState = checkState;
            BeforeCheckState = beforeCheckState;
            Node = node;
        }
    }

}
