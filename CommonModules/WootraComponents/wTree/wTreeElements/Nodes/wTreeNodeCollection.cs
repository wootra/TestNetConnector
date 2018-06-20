using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DataHandling;

namespace WootraComs.wTreeElements
{
    /// <summary>
    /// treeNode의 집합.
    /// </summary>
    public class wTreeNodeCollection : ICollection<wTreeNode>
    {
        ListDic<String, wTreeNode> _list = new ListDic<String,wTreeNode>();

        internal event wTreeNodeChangedEventHandler E_TreeNodeChanged;
        internal event wTreeListChangedEventHandler E_TreeListChanged;
        internal event wTreeListChangedEventHandler E_TreeExpandChanged;
        //internal event wTreeNodeItemMouseEvent E_TreeNodeItemSelected;

        IwTreeNodeCollectionParent _parent;
        wTree _ownerTree;

        internal wTreeNodeCollection(wTree ownerTree, IwTreeNodeCollectionParent parent)
        {
            _parent = parent;
            _ownerTree = ownerTree;
        }
        public wTreeMouseEventsHandler MouseEventsHandler { get { return _ownerTree.wMouseEventsHandler; } }

        public wTreeScroll ScrollHandler { get { return _ownerTree.wScrollHandler; } }

        public wTreeSelections SelectionHandler { get { return _ownerTree.wSelectionHandler; } }

        public DrawHandler DrawHandler { get { return _ownerTree.wDrawHandler; } }

        public EditorHandlerClass EditorHandler { get { return _ownerTree.wEditorHandler; } }

        internal void Add(wTreeNode node, bool occurEvent)
        {
            string name = node.Name;
            if (name == null) name = GetNewName("node");
            else if (_list.ContainsKey(name)) name = GetNewName(name);
            _list.Add(name, node);
            
            whenAddNode(node);
            if (occurEvent)
            {
                if (E_TreeListChanged != null) E_TreeListChanged(_parent);
            }
            
        }

        /// <summary>
        /// 중복되지 않는 이름을 발생시킨다. 기본 이름 baseName에 _숫자를 붙인다.
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        private string GetNewName(string baseName)
        {
            int i = 0;
            Random rand = new Random((int)DateTime.Now.Ticks);//초기값을 random에서 가져와서 반복을 줄인다.
            while (_list.ContainsKey(baseName + "_" + i)) i++;
            return baseName + "_" + i;
        }


        /// <summary>
        /// 비어있는 Node를 추가..wTreeNode.Items.Add()로 Item을 추가해야 함..
        /// </summary>
        /// <returns></returns>
        public wTreeNode Add()
        {
            wTreeNode node = new wTreeNode(_ownerTree, _parent);
            
            //node.Items.Add("");
            Add(node, false);
            return node;
        }

        public wTreeNode Add(String text)
        {
            wTreeNode node = new wTreeNode(_ownerTree, _parent);
            node.Name = text;
            node.Items.Add(new wTreeNodeItem(node, text));
            Add(node, true);
            return node;
        }

        public wTreeNode Add(bool isChecked, String text)
        {
            wTreeNode node = new wTreeNode(_ownerTree, _parent);
            node.Name = text;
            node.Items.Add(new wTreeNodeItem(node, isChecked, CheckboxActiveActions.Click));
            node.Items.Add(new wTreeNodeItem(node, text));
            Add(node, true);
            return node;
        }

        public wTreeNode Add(Image image, String text)
        {
            wTreeNode node = new wTreeNode(_ownerTree, _parent);
            node.Name = text;
            node.Items.Add(new wTreeNodeItem(node, image));
            node.Items.Add(new wTreeNodeItem(node, text));
            Add(node, true);
            return node;
        }

        public wTreeNode Add(bool isChecked, Image image, String text)
        {
            wTreeNode node = new wTreeNode(_ownerTree, _parent);
            node.Name = text;
            node.Items.Add(new wTreeNodeItem(node, isChecked, CheckboxActiveActions.Click));
            node.Items.Add(new wTreeNodeItem(node, image));
            node.Items.Add(new wTreeNodeItem(node, text));
            Add(node, true);
            return node;
        }


        void item_E_TreeNodeChanged(wTreeNode treeNode, wTreeNodeChangedEventArgs arg)// int itemIndex)
        {
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(treeNode, arg);//itemIndex);
        }

        void item_E_TreeListChanged(IwTreeNodeCollectionParent treeParent)
        {
            if (E_TreeListChanged != null) E_TreeListChanged(treeParent);
        }

        public wTreeNode this[int index]
        {
            get
            {
                if (index < _list.Count && index >= 0) return _list.ValueList[index];
                else return null;
            }
            set
            {
                if (index <= _list.Count && index >= 0)
                {
                    if (_list.ContainsKey(value.Name))//같은이름이 존재하면.
                    {
                        if (_list[value.Name].Index != index)//그런데 다른 인덱스에 있는 Node라면..
                        {
                            if (_list[value.Name].Equals(value)) //같은 인스턴스이면..
                            {
                                throw new Exception("Remove the node[" + value.Name + "] from this collection before assign in the other position...");
                            }
                            else //인스턴스가 다르다면 이름만 같은 것.
                            {
                                throw new Exception("the name to insert already exists in this collection - index:" + _list[value.Name].Index);
                            }
                        }
                        else //같은 인덱스에 같은 이름으로 넣으려고 하면..
                        {
                            if (_list.ValueList[index].Equals(value)) //같은 인스턴스라면 넣을 필요 없다.
                            {
                                //do nothing..
                            }
                            else //다른 인스턴스라면..이름만 같고 다른 인스턴스이므로 기존의 것을 지워준다.
                            {
                                whenReleaseNode(_list[value.Name]);//기존것의 연결을 끊음.
                                _list.Remove(value.Name);//리스트에서 삭제..
                                _list.Insert(value.Name, value, index);
                                whenAddNode(value);
                                if (E_TreeNodeChanged != null) E_TreeNodeChanged(value, null);
                            }
                        }
                    }
                    else //같은 이름이 존재하지 않으면..해당 index에 완전히 새로운 것을 넣어주는 것이다.
                    {
                        if (index < _list.Count) //index가 count와 같으면 지울 것이 없다.
                        {
                            wTreeNode nodeToRemove = _list.ValueList[index];
                            whenReleaseNode(nodeToRemove);//기존것의 연결을 끊음.
                            _list.Remove(nodeToRemove.Name);//리스트에서 삭제..
                            _list.Insert(value.Name, value, index);
                            whenAddNode(value);
                            if (E_TreeNodeChanged != null) E_TreeNodeChanged(value, null);
                        }else{
                            _list.Add(value.Name, value);
                            whenAddNode(value);
                            if (E_TreeListChanged != null) E_TreeListChanged(_parent);//새로 넣은 것이므로 리스트가 바뀌었슴.
                        }
                    }

                }
                
                else throw new Exception("wTree error: wTree[" + index + "] doesn't exist..");
            }
        }

        public wTreeNode this[string name]
        {
            get
            {
                if (_list.ContainsKey(name) == false) return null;
                else return _list[name];
            }
            set
            {
                if (_list.ContainsKey(name)) //같은 이름이 존재하면..
                {
                    if (_list[name].Equals(value))//같은 인스턴스일 때.
                    {
                        //아무것도 하지 않는다.
                    }
                    else //같은 인스턴스가 아니면.
                    {
                        wTreeNode nodeToRemove = _list[name];//기존것을 
                        int index = nodeToRemove.Index;
                        whenReleaseNode(nodeToRemove);//기존것의 연결을 끊음.
                        _list.Remove(nodeToRemove.Name);//리스트에서 삭제..
                        _list.Insert(value.Name, value, index);
                        whenAddNode(value);
                        if (E_TreeNodeChanged != null) E_TreeNodeChanged(value, null);
                    }
                }
                else //같은 이름이 존재하지 않으면..
                {
                    _list.Add(name, value);//새로이 넣은 것이므로 리스트가 바뀌었다.
                    if (E_TreeListChanged != null) E_TreeListChanged(_parent);
                }
            }
        }

        /// <summary>
        /// node를 이 컬렉션에 추가할 때 처리해 줄 목록
        /// </summary>
        /// <param name="node"></param>
        void whenAddNode(wTreeNode node)
        {
            node.E_TreeNodeChanged += item_E_TreeNodeChanged;//새 아이템에 연결을 추가한다.
            node.E_TreeListChanged += item_E_TreeListChanged;
            node.E_TreeExpandChanged += node_E_TreeExpandChanged;
            //node.E_TreeNodeItemSelected += node_E_TreeNodeItemSelected;
            node.E_NameChanged += node_E_NameChanged;
            node._parent = _parent;
        }



        bool node_E_NameChanged(wTreeNode node, string changedName, ref string errMsg)
        {
            if (_list.ContainsKey(changedName))
            {
                errMsg = "Same Node Name Exist in the Collection. NodeName:" + changedName;
                return false;
            }
            else
            {
                int index = _list.ValueList.IndexOf(node);// _list[node.Name].Index;
                _list.RemoveAt(index);
                _list.Insert(changedName, node, index);
            }
            if (E_TreeNodeChanged != null) E_TreeNodeChanged(node, null);
            return true;
        }

        void node_E_TreeExpandChanged(IwTreeNodeCollectionParent treeParent)
        {
            if (E_TreeExpandChanged != null) E_TreeExpandChanged(treeParent);
            (treeParent as wTreeNode).DrawBuffer();
        }


        /// <summary>
        /// node를 이 컬렉션에서 지울때 처리해 줄 목록
        /// </summary>
        /// <param name="node"></param>
        void whenReleaseNode(wTreeNode node)
        {
            node.E_TreeNodeChanged -= item_E_TreeNodeChanged;//기존에 있었던 이벤트 연결을 끊는다.
            node.E_TreeListChanged -= item_E_TreeListChanged;
            if (node._parent.Equals(_parent)) node._parent = null;
        }


        public void Clear()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                whenReleaseNode(_list.ValueList[i]);
            }
            _list.Clear();
            if (E_TreeListChanged != null) E_TreeListChanged(_parent);
        }

        public bool Contains(wTreeNode item)
        {
            return _list.ValueList.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _list.ContainsKey(key);
        }

        /// <summary>
        /// do not use this..
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(wTreeNode[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < _list.Count; i++)
            {
                array[i] = _list.ValueList[i];
            }
        }

        public int IndexOf(wTreeNode node)
        {
            return _list.ValueList.IndexOf(node);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(wTreeNode item)
        {
            wTreeNode itemToRemove = _list[item.Name];
            if (itemToRemove != null)
            {
                _list.RemoveAt(itemToRemove.Index);
                
                whenReleaseNode(itemToRemove);
                if (E_TreeListChanged != null) E_TreeListChanged(_parent);

                return true;
            }
            else return false;
        }

        public IEnumerator<wTreeNode> GetEnumerator()
        {
            return _list.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }



        void ICollection<wTreeNode>.Add(wTreeNode item)
        {
            Add(item, false);
        }

    }
}
