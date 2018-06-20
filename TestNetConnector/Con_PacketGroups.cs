using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FormAdders;
using CustomParser;
using RtwEnums.Network;
using System.Xml;

namespace TestNetConnector
{
    public partial class Con_PacketGroups : UserControl
    {
        String _groupsDir = "SavedPacketGroups";
        String _peersDir = "Peers";
        TriStateTreeNode _activatedPeerNode;
        //ConMsgList _activatedMsgList;

        //List<Con_Peer> _peers = new List<Con_Peer>();
        Dictionary<Con_Peer, String> _peersGroup = new Dictionary<Con_Peer, string>();
        
        String[] _groupPaths;
        TriStateTreeNode _peersRoot;
        TriStateTreeNode _groupsRoot;
        Encoding _strEncoding = Encoding.UTF8;

        public event GroupSelectedEventHandler E_GroupSelected;
        public event PeerSelectedEventHandler E_PeerSelected;
        public event PacketChangedEventHandler E_PacketChanged;

        public Con_PacketGroups()
        {
            InitializeComponent();
            _groupsDir = Directory.GetCurrentDirectory() +"\\"+ _groupsDir;
            _peersDir = Directory.GetCurrentDirectory() + "\\" + _peersDir;

            if (Directory.Exists(_groupsDir) == false) Directory.CreateDirectory(_groupsDir);
            if (Directory.Exists(_peersDir) == false) Directory.CreateDirectory(_peersDir);

            _groupPaths = Directory.GetDirectories(_groupsDir);

            TR_Groups.ActionOnEndNodeRightClicked = RtwTreeView2.Actions.ContextMenuOpened;
            TR_Groups.ActionOnParentNodeRightClicked = RtwTreeView2.Actions.ContextMenuOpened;
            TR_Groups.ActionOnEndNodeClicked = RtwTreeView2.Actions.None;
            TR_Groups.ActionOnParentNodeClicked = RtwTreeView2.Actions.None;

            TR_Groups.E_ContextMenuEndClicked += new RtwTreeView2.ContextMenuClickHandler(TR_Groups_E_ContextMenuEndClicked);
            TR_Groups.E_ContextMenuParentClicked += new RtwTreeView2.ContextMenuClickHandler(TR_Groups_E_ContextMenuParentClicked);

            TR_Groups.E_OnEndNodeClicked += new TreeNodeClickEventHandler(TR_Groups_E_OnEndNodeClicked);
            TR_Groups.E_OnParentNodeClicked += new TreeNodeClickEventHandler(TR_Groups_E_OnParentNodeClicked);
            TR_Groups.E_OnEndNodeChecked += new RtwTreeNodeCheckedEventHandler(TR_Groups_E_OnEndNodeChecked);

            loadGroups();
            
        }

        public void CloseAllPeers()
        {
            for(int i=0; i<_peersRoot.Nodes.Count; i++){
                Con_Peer peer = _peersRoot.Nodes[i].RelativeObject["peer"] as Con_Peer;
                peer.Close();
            }
        }

        Con_Peer getActivatedPeer()
        {
            if (_activatedPeerNode != null && _activatedPeerNode.RelativeObject.ContainsKey("peer"))
                return _activatedPeerNode.RelativeObject["peer"] as Con_Peer;
            else throw new Exception("Peer를 적어도 하나 선택해 주십시오");
        }

        void TR_Groups_E_OnEndNodeChecked(object sender, RtwTreeNodeCheckedEventArg e)
        {
            //throw new NotImplementedException();
        }

        void TR_Groups_E_OnParentNodeClicked(object sender, TreeNodeClickEventArg e)
        {
            TriStateTreeNode selectedNode = TR_Groups.SelectedNode as TriStateTreeNode;
            
            if (selectedNode.Parent == null)
            {
                //RootNodeContextClicked(selectedNode, text, index);
            }
            else
            {
                /*
                if (selectedNode.Parent.Equals(_peersRoot)) //peer
                {
                    int rowIndex = _peersRoot.Nodes.IndexOf(selectedNode);
                    _activatedPeer = _peers[rowIndex];
                    if (E_PeerSelected != null) E_PeerSelected(selectedNode, new PeerSelectedEventArgs(selectedNode.Text, _peers[rowIndex]));


                }
                else
                */
                    if (selectedNode.Parent.Equals(_groupsRoot)) //group
                {

                    //int peerIndex = _peers.IndexOf(_activatedPeer);
                    TriStateTreeNode peerNode = _activatedPeerNode;// _peersRoot.Nodes[peerIndex] as TriStateTreeNode;
                    string groupName = selectedNode.Text;
                    getActivatedPeer().SetMsgList(groupName);
                    peerNode.Text = peerNode.Name + "[" + groupName + "]";


                    if (E_GroupSelected != null) E_GroupSelected(selectedNode, new GroupSelectedEventArgs(groupName, _groupsDir + "\\" + groupName));

                }
                /*
                else if (selectedNode.Parent.Parent != null && selectedNode.Parent.Parent.Equals(_groupsRoot))//packet selected
                {
                    int peerIndex = _peers.IndexOf(_activatedPeer);
                    TriStateTreeNode peerNode = _peersRoot.Nodes[peerIndex] as TriStateTreeNode;

                    string groupName = selectedNode.Parent.Text;
                    String groupPath = _groupsDir + "\\" + groupName;
                    _activatedPeer.SetMsgList(groupName, _groupMsgList[groupName]);
                    peerNode.Text = peerNode.Name + "[" + groupName + "]";

                    if (E_GroupSelected != null) E_GroupSelected(selectedNode, new GroupSelectedEventArgs(selectedNode.Parent.Text, groupPath));
                    String packetPath = groupPath + "\\" + selectedNode.Text;
                    if (E_PacketChanged != null) E_PacketChanged(selectedNode, new PacketChangedEventArgs(selectedNode.Text, packetPath));
                }
                 */
            }
        }

        void TR_Groups_E_OnEndNodeClicked(object sender, TreeNodeClickEventArg e)
        {
            TriStateTreeNode selectedNode = TR_Groups.SelectedNode as TriStateTreeNode;
            if (selectedNode.Parent == null)
            {
                //RootNodeContextClicked(selectedNode, text, index);
            }
            else
            {
                if (selectedNode.Parent.Equals(_peersRoot)) //peer
                {
                    //int rowIndex = _peersRoot.Nodes.IndexOf(selectedNode);
                    _activatedPeerNode = selectedNode;
                    
                    if(E_PeerSelected!=null) E_PeerSelected(selectedNode, new PeerSelectedEventArgs(selectedNode.Text, getActivatedPeer()));
                    
                    
                }
                else if (selectedNode.Parent.Equals(_groupsRoot)) //group
                {

                    TriStateTreeNode peerNode = _activatedPeerNode;
                    string groupName = selectedNode.Text;
                    try
                    {
                        getActivatedPeer().SetMsgList(groupName);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    peerNode.Text = peerNode.Name + "[" + groupName + "]";
                    
                    
                    if (E_GroupSelected != null) E_GroupSelected(selectedNode, new GroupSelectedEventArgs(groupName, _groupsDir+"\\"+groupName));

                }
                else if (selectedNode.Parent.Parent != null && selectedNode.Parent.Parent.Equals(_groupsRoot))//packet selected
                {

                    TriStateTreeNode peerNode = _activatedPeerNode;// _peersRoot.Nodes[peerIndex] as TriStateTreeNode;
                    
                    string groupName = selectedNode.Parent.Text;
                    String groupPath = _groupsDir + "\\" + groupName;
                    getActivatedPeer().SetMsgList(groupName);
                    peerNode.Text = peerNode.Name + "[" + groupName + "]";
                    
                    if (E_GroupSelected != null) E_GroupSelected(selectedNode, new GroupSelectedEventArgs(selectedNode.Parent.Text, groupPath));
                    String packetPath = groupPath + "\\" + selectedNode.Text;
                    if (E_PacketChanged != null) E_PacketChanged(selectedNode, new PacketChangedEventArgs(selectedNode.Text, packetPath));
                }
            }
        }

        /// <summary>
        /// 이 Peer가 선택되면 해당되는 리스트가 활성화 되어야한다.
        /// </summary>
        public void SetMsgList(String group, Panel targetPanel)
        {
            targetPanel.Controls.Clear();

            try
            {
                _peersGroup[getActivatedPeer()] = group;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
                
            
            //_activatedMsgList = savedList;

            getActivatedPeer().SetMsgList(group);//peer에 종속된 msgList를 정한다.

            getActivatedPeer().AddMsgListToPanel(targetPanel);
        }


        public void loadGroups()
        {
            TR_Groups.ClearAllItems();
            
            
            
            _peersRoot = TR_Groups.AddRoot("Peers");
            //TR_Groups.AddContextMenuItemEndNode(_peersRoot, "새 Peer 생성");
            TR_Groups.AddContextMenuItemParentNode(_peersRoot, "새 Peer 생성");

            String[] peerPaths = Directory.GetFiles(_peersDir);

            for (int i = 0; i < peerPaths.Length; i++)
            {
                Con_Peer peer = new Con_Peer(peerPaths[i]);
                String name = peerPaths[i].Substring(peerPaths[i].LastIndexOf("\\") + 1);
                name = name.Substring(0, name.LastIndexOf("."));// .xml을 떼어줌..
                TriStateTreeNode node = TR_Groups.AddChild(_peersRoot, name);
                node.RelativeObject["peer"] = peer;
                peer.SetName(name);
                _activatedPeerNode = node;
                

                //_peersGroup[peer] = name;//마지막 읽은 group을 _activatedPeer에 배정한다.
                //peer.SetMsgList(name, new ConMsgList(name));
            }

            String[] grpPaths = Directory.GetDirectories(_groupsDir);

            _groupsRoot = TR_Groups.AddRoot("Groups");
            //TR_Groups.AddContextMenuItemEndNode(_groupsRoot, "새 그룹 생성");
            TR_Groups.AddContextMenuItemParentNode(_groupsRoot, "새 그룹 생성");

            for (int i = 0; i < grpPaths.Length; i++)
            {
                String name = grpPaths[i].Substring(grpPaths[i].LastIndexOf("\\") + 1);
                TriStateTreeNode grp = TR_Groups.AddChild(_groupsRoot, name);
                TR_Groups.AddContextMenuItemParentNode(grp, "새 패킷 생성");

                grp.RelativeObject["dir"] = grpPaths[i];

                String[] packets = Directory.GetDirectories(grpPaths[i]);
                
                for (int p = 0; p < packets.Length; p++)
                {
                    String childName = packets[p].Substring(packets[p].LastIndexOf("\\") + 1);
                    TriStateTreeNode childNode = TR_Groups.AddChild(grp, childName);
                    childNode.RelativeObject["dir"] = packets[p];
                    TR_Groups.AddContextMenuItemEndNode(childNode, "편집");
                    TR_Groups.AddContextMenuItemEndNode(childNode, "삭제");
                    TR_Groups.AddContextMenuItemEndNode(childNode, "복사본만들기");
                }
            }

            for (int i = 0; i < _peersRoot.Nodes.Count; i++)
            {
                if (_groupsRoot.Nodes.Count > 0)
                {
                    TriStateTreeNode node = _peersRoot.Nodes[i];
                    Con_Peer peer = node.RelativeObject["peer"] as Con_Peer;
                    XmlDocument xDoc;
                    XmlNode root = XmlHandlers.XmlGetter.RootNode(out xDoc, peerPaths[i]);
                    XmlNode Envs = XmlHandlers.XmlGetter.Child(root, "Envs");
                    
                    String groupName = _groupsRoot.Nodes[0].Text;
                    bool isFound = false;
                    foreach(XmlNode anEnv in Envs.ChildNodes){
                        string name = XmlHandlers.XmlGetter.Attribute(anEnv, "Name");
                        if(name.ToLower().Equals("groupname")){
                            
                            for(int g=0; g<_groupsRoot.Nodes.Count; g++){
                                if(_groupsRoot.Nodes[g].Text.Equals(anEnv.InnerText)){
                                    groupName = anEnv.InnerText;
                                    isFound = true;
                                    break;
                                }
                            }
                            if(isFound) break;
                        }

                    }
                    
                    _peersGroup[peer] = groupName;
                    node.Text = peer.Name + "[" + groupName + "]";
                }
            }

           TR_Groups.Refresh();
           _groupsRoot.Expand();
           _peersRoot.Expand();
           TR_Groups.Refresh();
        }
       

        void TR_Groups_E_ContextMenuParentClicked(object sender, string text, int index, TriStateTreeNode selectedNode, object MenuItem)
        {
            if (selectedNode.Parent == null) RootNodeContextClicked(selectedNode, text, index);
            else if (selectedNode.Parent.Equals(_groupsRoot))
            {
                int rowIndex = _groupsRoot.Nodes.IndexOf(selectedNode);
                switch (text)
                {
                    case "새 패킷 생성":
                        MakeNewPacket(selectedNode.Text);
                        break;
                }
                //if (E_GroupSelected != null) E_GroupSelected(selectedNode, new GroupSelectedEventArgs(selectedNode.Text, _groupsDir));

            }
        }
        void MakeNewPacket(String groupName)
        {
            CPacketStruct cp = new CPacketStruct();
            InputForm input = new InputForm();
            DialogResult result = input.ShowDialog("New Packet Name");
            if(result == DialogResult.Abort || result== DialogResult.Cancel || input.InputText.Trim().Length == 0) return;
            String newPacketPath = _groupsDir+"\\"+groupName+"\\"+input.InputText;
            if(Directory.Exists(newPacketPath)){
                MessageBox.Show("중복되는 이름이 있습니다.");
                return;
            }
            
            String msgFile = newPacketPath + "\\Msg.text";
            DlgMsgMaker msgMaker = new DlgMsgMaker(cp, msgFile, getActivatedPeer().Endian == Endians.Big);
            msgMaker.Location = this.Location;
            msgMaker.ShowDialog();
            
            TR_Groups.AddChild(_groupsRoot.Nodes[groupName], input.InputText);
            getActivatedPeer().InitList();
            //if (_activatedPeerNode != null) _activatedPeerNode.InitList();
            //_activatedMsgList.initList();
        }

        void RootNodeContextClicked(TriStateTreeNode node, String text, int index)
        {
            if (node.Equals(_peersRoot))
            {
                switch (text)
                {
                    case "새 Peer 생성":
                        InputForm input = new InputForm();
                        DialogResult result = input.ShowDialog("New Packet Name");
                        if(result == DialogResult.Abort || result== DialogResult.Cancel || input.InputText.Trim().Length == 0) return;
                        String newPacketPath = _peersDir+"\\"+input.InputText+".xml";
                        if(File.Exists(newPacketPath)){
                            MessageBox.Show("중복되는 이름이 있습니다.");
                            return;
                        }

                        Con_Peer peer = new Con_Peer(newPacketPath);
                        
                        TriStateTreeNode peerNode = TR_Groups.AddChild(_peersRoot, input.InputText);
                        peerNode.RelativeObject["peer"] = peer;
                        if (_groupsRoot.Nodes.Count > 0)
                        {
                            String groupName = _groupsRoot.Nodes[0].Text;
                            peer.SetMsgList(groupName);
                        }
                        break;
                }

            }
            else if (node.Equals(_groupsRoot))
            {
                switch (text)
                {
                    case "새 그룹 생성":
                        FormAdders.InputForm input = new InputForm();
                        DialogResult result = input.ShowDialog("Report 사용자이름 입력");
                        
                        if (result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.Cancel) return;
                        if(input.InputText.Trim().Length==0) return;
                        string newDir = _groupsDir + "\\" + input.InputText;
                        if (Directory.Exists(newDir))
                        {
                            MessageBox.Show("이름이 중복됩니다.");
                            return;
                        }
                        TriStateTreeNode child = TR_Groups.AddChild(_groupsRoot, input.InputText);
                        TR_Groups.AddContextMenuItemParentNode(child, "새 패킷 생성");
                        
                        Directory.CreateDirectory(newDir);

                        _groupPaths = Directory.GetDirectories(_groupsDir);
                        break;
                }
            }
        }

        void TR_Groups_E_ContextMenuEndClicked(object sender, string text, int index, TriStateTreeNode selectedNode, object MenuItem)
        {
            if (selectedNode.Parent == null)
            {
                RootNodeContextClicked(selectedNode, text, index);
            }
            else
            {
                if (selectedNode.Parent.Equals(_peersRoot))
                {
                    int rowIndex = _peersRoot.Nodes.IndexOf(selectedNode);
                    //if(E_PeerSelected!=null) E_PeerSelected(selectedNode, new PeerSelectedEventArgs(selectedNode.Text, _peers[rowIndex]));

                }
                else if (selectedNode.Parent.Equals(_groupsRoot))
                {
                    int rowIndex = _groupsRoot.Nodes.IndexOf(selectedNode);

                    //if (E_GroupSelected != null) E_GroupSelected(selectedNode, new GroupSelectedEventArgs(selectedNode.Text, _groupsDir));

                }
                else if(selectedNode.Parent.Parent!=null && selectedNode.Parent.Parent.Equals(_groupsRoot))//packet selected
                {
                    
                }
            }
        }
    }
    public class GroupSelectedEventArgs
    {
        public String Name;
        public String Path;
        public GroupSelectedEventArgs(string groupName, string path)
        {
            Name = groupName;
            Path = path;
        }
    }
    public delegate void GroupSelectedEventHandler(TriStateTreeNode node, GroupSelectedEventArgs args);

    public class PacketChangedEventArgs
    {
        public String Name;
        public String Path;
        public PacketChangedEventArgs(string packetName, string path)
        {
            Name = packetName;
            Path = path;
        }
    }
    public delegate void PacketChangedEventHandler(TriStateTreeNode node, PacketChangedEventArgs args);

    public class PeerSelectedEventArgs
    {
        public String Name;
        public Con_Peer Peer;
        public PeerSelectedEventArgs(string groupName, Con_Peer peer)
        {
            Name = groupName;
            Peer = peer;
        }
    }
    public delegate void PeerSelectedEventHandler(TriStateTreeNode node, PeerSelectedEventArgs args);
}
