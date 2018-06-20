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
    public partial class 그래프 : Form
    {
        private Timer _graphTimer = new Timer();
        private int _counter = 0;

        public 그래프()
        {
            InitializeComponent();
            _graphTimer.Interval = 200;
            _graphTimer.Tick += new EventHandler(_graphTimer_Tick);
            _graphTimer.Start();
        }
        void _graphTimer_Tick(object sender, EventArgs e)
        {
 

            double rad = 0;


            #region for TEst

                rad = Math.Sin(_counter / 360.0 * Math.PI * 2);
                //for test
                //graph.setData(_counter, Math.Sin(rad), Math.Cos(rad));
                //end Test


            #endregion

            _counter++;
        }
    }

}
