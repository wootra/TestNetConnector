using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public partial class DoubleBufferedLabel : Label
    {
        public DoubleBufferedLabel()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        public DoubleBufferedLabel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
