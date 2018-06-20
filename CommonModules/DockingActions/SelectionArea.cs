using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingActions
{
    internal partial class SelectionArea : Form
    {
        public SelectionArea()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            
        }
        
        
        protected override void  OnPaint(PaintEventArgs e){
            base.OnPaint(e);

            Graphics g = e.Graphics;
            
            using (Pen pen = new Pen(Brushes.Gray, 10.0f))
            {
                //g.FillRectangle(Brushes.Cyan, this.ClientRectangle);
                g.DrawRectangle(pen, this.ClientRectangle);
                //g.DrawLine(pen, 0, 0, this.Width, this.Height);
            }
            Invalidate();
        }
    }
}
