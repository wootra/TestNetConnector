using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public partial class DoubleBufferedPanel : UserControl
    {
        public DoubleBufferedPanel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }
    }
}
