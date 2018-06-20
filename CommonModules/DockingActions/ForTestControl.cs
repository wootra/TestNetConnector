using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingActions
{
    public partial class ForTestControl : UserControl
    {
        public Label TestLabel;
        public ForTestControl()
        {
            InitializeComponent();
        }
        public void setLabel(String text){
            if (TestLabel != null) TestLabel.Text = text;
        }
    }
}
