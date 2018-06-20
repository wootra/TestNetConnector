using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace TabStripApp {
    
    [ToolboxItem(typeof(TabStripToolboxItem))]
    [System.ComponentModel.DesignerCategory("code")]
    public partial class TabStrip : ToolStrip {
        Font boldFont = new Font(SystemFonts.MenuFont, FontStyle.Bold);
        private const int EXTRA_PADDING = 4;

        public TabStrip() {
            Renderer = new TabStripProfessionalRenderer();
        }
        protected override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) {
            return new Tab(text, image, onClick);
        }

        private TabStripLayoutEngine tabStripLayout;
       
        public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine {
            get {
                if (tabStripLayout == null) {
                    tabStripLayout = new TabStripLayoutEngine(this);
                }
                return tabStripLayout;

            }
        }

        protected override Padding DefaultPadding {
            get {
                Padding padding =  base.DefaultPadding;
                padding.Top += EXTRA_PADDING;
                padding.Bottom += EXTRA_PADDING;

                return padding;
            }
        }

        private Tab currentSelection;

        public Tab SelectedTab {
            get { return currentSelection; }
            set {
                if (currentSelection != value) {
                    currentSelection = value;
                   
                    if (currentSelection != null) {
                        PerformLayout();
                        if (currentSelection.TabStripPage != null) {
                            currentSelection.TabStripPage.Activate();                            
                        }
                    }
                }

            }
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e) {
            for (int i = 0; i < Items.Count; i++) {
                Tab currentTab = Items[i] as Tab;
                SuspendLayout();
                if (currentTab != null) {
                    if (currentTab != e.ClickedItem) {
                        currentTab.Checked = false;
                        currentTab.Font = this.Font;
                    }
                    else {
                        currentTab.Font = boldFont;
                    }
                }
                ResumeLayout();
            }
            SelectedTab = e.ClickedItem as Tab;
            
            base.OnItemClicked(e);
          
        }

        protected override void SetDisplayedItems() {
            base.SetDisplayedItems();
            for (int i = 0; i < DisplayedItems.Count; i++) {
                if (DisplayedItems[i] == SelectedTab) {
                    DisplayedItems.Add(SelectedTab);            
                    break;
                }
            }
        }

        protected override Size DefaultSize {
            get {
                Size size =  base.DefaultSize;
               // size.Height += EXTRA_PADDING*2;
                return size;
            }
        }

        public override Size GetPreferredSize(Size proposedSize) {
            Size preferredSize = Size.Empty;
            proposedSize -= this.Padding.Size;

            foreach (ToolStripItem item in this.Items) {
               preferredSize =  LayoutUtils.UnionSizes(preferredSize, item.GetPreferredSize(proposedSize) + item.Padding.Size);
            }
            return preferredSize + this.Padding.Size;
        }

        private int tabOverlap = 10;
        [DefaultValue(10)]
        public int TabOverlap {
            get { return tabOverlap; }
            set {
                if (tabOverlap != value) {
                    if (value < 0) {
                        throw new ArgumentOutOfRangeException("Tab overlap must be greater than 0");
                    }
                    tabOverlap = value;
                    // call perform layout so we 
                    PerformLayout();
                }                   
            }
        }


        private class TabStripLayoutEngine : LayoutEngine {
            private TabStrip tabStrip;
            public TabStripLayoutEngine(TabStrip tabStrip) {
                this.tabStrip = tabStrip;
            }
#if true

            public override bool Layout(object container, LayoutEventArgs layoutEventArgs) {

                // fetch the display rectangle of the TabStrip.  This is typically
                // the TabStrip.ClientRectangle - TabStrip.Padding
                Rectangle displayRect = tabStrip.DisplayRectangle;

                // the next location to place the item, start off at the upper left hand corner.
                Point nextLocation = displayRect.Location;

                for (int i = 0; i < tabStrip.Items.Count; i++) {
                    ToolStripItem item = tabStrip.Items[i] as ToolStripItem;
                    if (!item.Available) {
                        continue;
                    }
                    Tab currentTab = item as Tab;

                    tabStrip.SetItemLocation(item, new Point(nextLocation.X, nextLocation.Y));

                    if (item.AutoSize) {
                        if (currentTab != null) {
                            //this is a tab, make sure it stretches from top->bottom
                            item.Size = new Size(item.GetPreferredSize(displayRect.Size).Width, displayRect.Height);
                        }
                        else {
                            //this not tab, keep it it's preferred width/height.
                            item.Size = item.GetPreferredSize(displayRect.Size); 
                        }                        
                    }

                    // advance "nextLocation".  
                    // This could simply be 
                    //      nextLocation.X += item.Width - tabStrip.TabOverlap;
                    // ...but we're fancier than that.
                    // if the next thing isnt a tab, we dont want to overlap it.
                    

                    Tab nextTab = (i + 1 < tabStrip.Items.Count) ? tabStrip.Items[i + 1] as Tab : null;
                    if (currentTab != null && nextTab != null) {
                        // we are a Tab, and the next thing is a Tab
                        // overlap 
                        nextLocation.X += item.Width - tabStrip.TabOverlap;
                    }
                    else {
                        nextLocation.X += item.Width;
                    }
                }
                return tabStrip.AutoSize;

            }
        }
#else
            public override bool Layout(object container, LayoutEventArgs layoutEventArgs) {

                // fetch the display rectangle of the TabStrip.  This is typically
                // the TabStrip.ClientRectangle - TabStrip.Padding
                Rectangle displayRect = tabStrip.DisplayRectangle;

                // the next location to place the item, start off at the upper left hand corner.
                Point nextLocation = displayRect.Location;

                for (int i = 0; i < tabStrip.Items.Count; i++) {
                    ToolStripItem item = tabStrip.Items[i] as ToolStripItem;
                
                    // Set the item's location as specified by nextLocation
                    tabStrip.SetItemLocation(item, new Point(nextLocation.X, nextLocation.Y));
                    
                    // if the item is AutoSized, set it to the preferred size
                    if (item.AutoSize) {
                        // use the preferredSize.Width, use the DisplayRectangle.Height for the height
                        Size preferredSize = item.GetPreferredSize(displayRect.Size);
                        preferredSize.Height = displayRect.Height;  
                        item.Size = preferredSize;
                    }                  
                    nextLocation.X += item.Width - tabStrip.TabOverlap;                    
                
                }
                return tabStrip.AutoSize;

            }
        }
#endif
    }

    public class LeftRightMargin {
        private int left;
        private int right;

        public LeftRightMargin(int left, int right) {
            this.left = left;
            this.right = right;
        }

        public int Left {
            get { return left; }
            set { left = value; }
        }


        public int Right {
            get { return right; }
            set { right = value; }
        }
    }
}
