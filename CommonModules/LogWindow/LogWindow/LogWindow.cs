using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IOHandling;
using System.Runtime.InteropServices;

namespace Display
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message wMessage)
        {
            Win32APIs.COPYDATASTRUCT lParam;
                
            switch ((int)(wMessage.Msg))
            {
                
                case Win32APIs.WM_COPYDATA:

                    lParam = (Win32APIs.COPYDATASTRUCT)Marshal.PtrToStructure(wMessage.LParam, typeof(Win32APIs.COPYDATASTRUCT));
                    T_Log.AppendText(lParam.lpData);
                        
                    break;

                default:
                    lParam = new Win32APIs.COPYDATASTRUCT();
                    break;

            }
            
            base.WndProc(ref wMessage);
        }
    }
}
