using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flash.External;

namespace FlashPanels
{
    public class YawPanel:FlashAdder
    {
        public YawPanel()
            : base()
        {
        }
        public void setValues(double yaw, double roll, double pitch)
        {
            U_setValue(yaw, roll, pitch);
        }
    }
}
