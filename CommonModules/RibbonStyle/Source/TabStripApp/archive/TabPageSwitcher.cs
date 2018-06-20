using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;


namespace TabStripApp {
    [Designer(typeof(TabPageSwitcherDesigner))]
    [System.ComponentModel.DesignerCategory("code")]
    [Docking(DockingBehavior.AutoDock)]
    [ToolboxItem(false)]
    public class TabPageSwitcher : ContainerControl {
        private Control selectedControl;
        private TabStrip tabStrip;

        public TabPageSwitcher() {
            ResetBackColor();
        }

        protected override Size DefaultSize {
            get {
                return new Size(150, 150);
            }
        }
 
        protected override Padding DefaultPadding {
            get {
                return new Padding(2);
            }
        }
        public event EventHandler Load;

        public TabStrip TabStrip {
            get {
               return tabStrip;
            }
            set {
                tabStrip = value;
            }
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
        protected override Control.ControlCollection CreateControlsInstance() {
            return new ZOrderControlCollection(this);
        }
        public Control SelectedControl {
            get { return selectedControl; }
            set {
                selectedControl = value;
                if (selectedControl != null) {
                    if (!Controls.Contains(value)) {
                        Controls.Add(selectedControl);
                    }
                    else {
                        selectedControl.BringToFront();
                    }
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            if (!RecreatingHandle) {
                OnLoad(EventArgs.Empty);
            }
        }
        protected virtual void OnLoad(EventArgs e) {
            if (this.Load != null) {
                Load(this, e);
            }

            if (!DesignMode) {
                // select a Tab 
                if (this.TabStrip != null) {
                    if (this.TabStrip.SelectedTab == null) {
                        foreach (ToolStripItem item in this.TabStrip.Items) {
                            this.TabStrip.SelectedTab = item as Tab;
                            if (this.TabStrip.SelectedTab != null) {
                                break;
                            }
                        }
                    }
                    if (this.TabStrip.SelectedTab != null) {
                        SelectedControl = this.TabStrip.SelectedTab.TabStripPage;
                    }
                }
            }
        }


        protected override void OnControlAdded(ControlEventArgs e) {
            e.Control.Dock = DockStyle.Fill;
            base.OnControlAdded(e);
        }

        #region DesignerSerialization Friendliness
        private bool ShouldSerializeBackColor() {
            return this.BackColor != ProfessionalColors.ToolStripGradientEnd;
        }
        public override void ResetBackColor() {
            this.BackColor = ProfessionalColors.ToolStripGradientEnd;
        }
        #endregion
        private class ZOrderControlCollection : Control.ControlCollection {
            public ZOrderControlCollection(Control parent) : base(parent) {
            }

            public override void SetChildIndex(Control child, int newIndex) {
                if (newIndex == Count-1) {
                    TabPageSwitcher parent = Owner as TabPageSwitcher;
                    if (parent != null && parent.SelectedControl != null && parent.SelectedControl != child) {
                        base.SetChildIndex(child, Math.Max(0,newIndex-1));
                    }
                }

                base.SetChildIndex(child, newIndex);
            }
        }

       
    }
}
