using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TabStripApp {
    [ToolboxItem(false)]
    [Docking(DockingBehavior.Never)]
    public class TabStripPage : Panel {
        public TabStripPage() {
        }

        public void Activate() {
            TabPageSwitcher tabPageSwitcher = this.Parent as TabPageSwitcher;
            if (tabPageSwitcher != null) {
                tabPageSwitcher.SelectedControl = this;
            }
            
        }
    }
}
