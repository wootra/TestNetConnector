using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DataHandling;
using CustomParser;

namespace TestNetConnector
{
    public partial class DlgDefineStruct : Form
    {
        CPacketStruct _ns;
        String _saveFile = "./msg.txt";
        bool _swapWhenMakePacket = true;
        public DlgDefineStruct()
        {//don't use this constructor
            this._ns = new CPacketStruct();
            InitializeComponent();
        }

        public DlgDefineStruct(CPacketStruct ns, String saveFile, bool swapWhenMakePacket)
        {
            InitializeComponent();
            if (saveFile != null) _saveFile = saveFile;
            this._ns = ns;
            _swapWhenMakePacket = swapWhenMakePacket;
            if (ns.NativeText != null && ns.NativeText.Length > 0)
            {
                T_Struct.Text = ns.NativeText;
            }
            else
            {
                try
                {
                    if (File.Exists(_saveFile))
                    {
                        T_Struct.Text = File.ReadAllText(_saveFile);
                    }
                }
                catch { }
            }

        }


        private void B_Save_Click(object sender, EventArgs e)
        {
            
            try
            {
                _ns.MakePacket(T_Struct.Text, _swapWhenMakePacket);
            }
            catch (Exception ex)
            {
                MessageBox.Show("문법에 오류가 있습니다 : " + ex.Message);
                return;
            }

            StreamWriter file = File.CreateText(_saveFile);
            file.Write(T_Struct.Text);
            file.Close();

            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
