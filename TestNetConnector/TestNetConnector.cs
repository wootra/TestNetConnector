using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomParser;

namespace TestNetConnector
{
    public partial class TestNetConnector : Form
    {

        //Dictionary<Con_PacketGroups,ConMsgList> _packetList
        Con_PacketGroups _groups;
        

        public TestNetConnector()
        {
            InitializeComponent();

            _groups = new Con_PacketGroups();
            _groups.Dock = DockStyle.Fill;
            P_Groups.Controls.Add(_groups);
            _groups.E_GroupSelected += new GroupSelectedEventHandler(groups_E_GroupSelected);
            _groups.E_PeerSelected += new PeerSelectedEventHandler(groups_E_PeerSelected);

            B_HideLeft.Click += new EventHandler(B_HideLeft_Click);
            B_HideRight.Click += new EventHandler(B_HideRight_Click);
            //Groups.Dock = DockStyle.Fill;
            //P_Groups.Controls.Add(Groups);
            
            this.Refresh();
        }

        void B_HideRight_Click(object sender, EventArgs e)
        {
            if (B_HideRight.Text.Equals(">"))
            {
                B_HideRight.Text = "<";
                Table_Main.ColumnStyles[4] = new ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0.0F);
            }
            else
            {
                B_HideRight.Text = ">";
                Table_Main.ColumnStyles[4] = new ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250.0F);
            }
        }

        void B_HideLeft_Click(object sender, EventArgs e)
        {
            if (B_HideLeft.Text.Equals("<"))
            {
                B_HideLeft.Text = ">";
                Table_Main.ColumnStyles[0] = new ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0.0F);
            }
            else
            {
                B_HideLeft.Text = "<";
                Table_Main.ColumnStyles[0] = new ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250.0F);
            }
        }

        void groups_E_PeerSelected(FormAdders.TriStateTreeNode node, PeerSelectedEventArgs args)
        {
            P_Main.Controls.Clear();
            P_Main.Controls.Add(args.Peer);
            
            args.Peer.Dock = DockStyle.Fill;
            
            args.Peer.AddMsgListToPanel(P_Table);
            this.Text = args.Name;
        }

        void groups_E_GroupSelected(FormAdders.TriStateTreeNode node, GroupSelectedEventArgs args)
        {
            P_Table.Controls.Clear();
            
            
            _groups.SetMsgList(args.Name, P_Table);
            
            //throw new NotImplementedException();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _groups.CloseAllPeers();
            base.OnClosing(e);

        }


    }
}
