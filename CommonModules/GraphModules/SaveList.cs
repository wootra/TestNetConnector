using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace GraphModules
{
    public partial class SaveList : Form
    {
        public SaveList()
        {
            InitializeComponent();
            
        }

        public SaveList(String[] varList)
        {
            InitializeComponent();
            setInitList(varList);
        }

        public void setInitList(String[] varList)
        {
            foreach (String var in varList)
            {
                CL_Vars.Items.Add(var);
            }
            for (int i = 0; i < CL_Vars.Items.Count; i++)
            {
                CL_Vars.SetItemChecked(i, true);
            }
        }

        public List<String> U_SelectedVars
        {
            get
            {
                List<String> items = new List<string>();
                foreach (String item in CL_Vars.CheckedItems)
                {
                    items.Add(item);
                }
                return items;
            }
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
