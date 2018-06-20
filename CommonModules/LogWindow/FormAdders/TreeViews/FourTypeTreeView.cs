using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public class FourTypeTreeView : TreeView
    {
        private TreeNode _oldTreeNode;
        public event TreeNodeMouseClickEventHandler FinalNodeClick;
        public event TreeNodeMouseClickEventHandler LeftClick;
        public event TreeNodeMouseClickEventHandler RightClick;
        public event TreeNodeMouseClickEventHandler MiddleClick;
        public event TreeNodeMouseClickEventHandler WheelDown;
        public event TreeNodeMouseClickEventHandler WheelUp;

        public FourTypeTreeView()
            : base()
        {
            this.NodeMouseClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseClick);
            ImageList list = new ImageList();
            list.Images.Add(Properties.Resources.tree_close);
            list.Images.Add(Properties.Resources.tree_open);
            this.ImageList = list;
        }

        public void setInitNode(TreeNode tn=null){
            
            if (tn == null)
            {
                if (this.Nodes.Count > 0)
                {

                    _oldTreeNode = this.Nodes[0];
                    if (_oldTreeNode.Nodes.Count > 0)
                    {
                        _oldTreeNode.Expand();
                        _oldTreeNode.ImageIndex = 1;
                        _oldTreeNode.SelectedImageIndex = 1;
                    }
                }
            }
            else
            {
                _oldTreeNode = tn;
                if (_oldTreeNode.Nodes.Count > 0)
                {
                    _oldTreeNode.Expand();
                    _oldTreeNode.ImageIndex = 1;
                    _oldTreeNode.SelectedImageIndex = 1;
                }
            }
            setTreeNode(this.Nodes);
        }
        public new ImageList ImageList
        {
            get
            {
                return base.ImageList;
            }
            set
            {
                base.ImageList = value;
                setTreeNode(this.Nodes);
            }
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeView tv = (TreeView)sender;
            this.HideSelection = true;
            
            if (_oldTreeNode != null)
            {
                if (_oldTreeNode.Equals(e.Node)) //자기 자신을 클릭했을 때
                {
                    if (e.Node.Nodes.Count == 0)
                    { //최종 노드를 다시 클릭했을 때.
                        //검사 실행
                        if(FinalNodeClick!=null) FinalNodeClick(this, e);
                    }
                    else
                    {//자식노드가 있는 노드를 다시 클릭했을 때
                        if (e.Node.IsExpanded)
                        { //열려있으면,
                            e.Node.Collapse(); //닫고
                            e.Node.SelectedImageIndex = 0; //비활성상태
                        }
                        else
                        { //닫혀있으면
                            e.Node.Expand(); //열고
                            e.Node.SelectedImageIndex = 1;
                            e.Node.ImageIndex = 1;
                        }
                    }
                }
                else
                { //자기 자신이 아니면,
                    if (_oldTreeNode.Nodes.Count > 0) //과거의 노드가 자식노드가 있는 것이라면
                    {
                        if (_oldTreeNode.Nodes.IndexOf(e.Node) >= 0) //과거의 노드의 자식노드를 클릭한 케이스
                        { //열려있는 노드의 자식노드를 클릭했을 때,
                            _oldTreeNode.ImageIndex = 1;
                        }
                        else //자식노드와 상관없는 노드를 클릭했을 때
                        {
                            _oldTreeNode.ImageIndex = 0;
                            _oldTreeNode.Collapse();
                        }
                    }
                    else //과거 노드가 자식노드가 없는 것이라면 선택해제
                    {
                        _oldTreeNode.ImageIndex = 2;
                    }

                    if (e.Node.Nodes.Count == 0)
                    { //최종 노드를 새로 클릭했을 때.
                        //검사 실행
                        if (FinalNodeClick != null) FinalNodeClick(this, e);
                    }
                    else
                    {//자식노드가 있는 노드를 새로 클릭했을 때
                        if (e.Node.IsExpanded)
                        { //열려있으면,
                            e.Node.Collapse(); //닫고
                            e.Node.SelectedImageIndex = 0; //비활성상태
                        }
                        else
                        { //닫혀있으면
                            e.Node.Expand(); //열고
                            e.Node.SelectedImageIndex = 1;
                            e.Node.ImageIndex = 1;
                        }
                    }
                }
            }
            _oldTreeNode = e.Node;


            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                if (LeftClick != null) LeftClick(sender, e);
                return;
            }
            else if (e.Button != System.Windows.Forms.MouseButtons.Right)
            {
                if (RightClick != null) RightClick(sender, e);
                return;
            }
            else if (e.Button != System.Windows.Forms.MouseButtons.Middle)
            {
                if (MiddleClick != null) MiddleClick(sender, e);
                return;
            }
            else if (e.Delta < 0)
            {
                if (WheelUp != null) WheelUp(sender, e);
                return;
            }
            else if (e.Delta > 0)
            {
                if (WheelDown != null) WheelDown(sender, e);
                return;
            }

        }
        void setTreeNode(TreeNodeCollection tnc)
        {
            foreach (TreeNode tn in tnc)
            {
                if (tn.Nodes.Count > 0)
                {
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 1;
                    setTreeNode(tn.Nodes);
                }
                else
                {
                    tn.ImageIndex = 2;
                    tn.SelectedImageIndex = 3;
                }
            }
        }
    }
}
