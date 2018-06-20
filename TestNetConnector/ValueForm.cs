using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestNetConnector
{
    public enum Titles { name, value };
    public partial class ValueForm : Form
    {
        public ValueForm()
        {
            InitializeComponent();
            addTitles();
        }

        void addTitles()
        {
            V_Data.AddTitleTextBoxColumn(-1, Titles.name.ToString(), "Name", false);
            V_Data.AddTitleTextBoxColumn(200, Titles.value.ToString(), "value", true);
        }

    }
}
