using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataHandling;
using FormAdders;
using CustomParser;

namespace TestNetConnector
{
    public partial class DlgViewStruct : PopupForm
    {
        CPacketStruct _ns;
        IDictionary<String, String[]> _dic;
        List<String[]> _arrs;

        public DlgViewStruct()
        {
            InitializeComponent();
        }

        public DlgViewStruct(CPacketStruct ns, IDictionary<String, String[]> dic, List<String[]> arrs)
        {
            InitializeComponent();
            _ns = ns;
            this._dic = dic;
            this._arrs = arrs;

            makeTree();
        }

        public void makeTree(CPacketStruct ns = null, IDictionary<String, String[]> dic = null, List<String[]> arrs = null)
        {
            if (ns == null) ns = _ns;
            if (dic == null) dic = _dic;
            if (arrs == null) arrs = _arrs;

            Tr_Struct.Nodes.Clear();
            String arrStr;
            for(int i=0; i<ns.Count; i++){
                if (ns.Items[i].size == 1) Tr_Struct.Nodes.Add(ns.Items[i].Name, ns.Items[i].Name +":"+ ns.Items[i].TypeString+ "(" + _dic[ns.Items[i].Name][0] + ")");
                else
                {

                    Tr_Struct.Nodes.Add(ns.Items[i].Name, "");
                    arrStr = "{";
                    for (int j = 0; j < ns.Items[i].size; j++)
                    {
                        if (j != 0) arrStr += ",";
                        arrStr += _dic[ns.Items[i].Name][j];
                        Tr_Struct.Nodes[ns.Items[i].Name].Nodes.Add(ns.Items[i].Name+"["+j+"]", "["+j+"] : "+_dic[ns.Items[i].Name][j]);
                    }
                    arrStr += "}";
                    Tr_Struct.Nodes[ns.Items[i].Name].Text = ns.Items[i].Name + ":" + ns.Items[i].TypeString+"["+ns.Items[i].size+"]" + arrStr;
                }
            }
        }

        
    }
}
