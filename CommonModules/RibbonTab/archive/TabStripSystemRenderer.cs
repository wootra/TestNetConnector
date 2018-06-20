using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace TabStripApp {
    class TabStripSystemRenderer : ToolStripSystemRenderer {

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
            TabStrip tabStrip = e.ToolStrip as TabStrip;
            ToolStripButton button = e.Item as ToolStripButton;
            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);

            if (tabStrip != null) {
                System.Windows.Forms.VisualStyles.TabItemState tabState = System.Windows.Forms.VisualStyles.TabItemState.Normal;
                if (button.Checked) {
                    tabState |= System.Windows.Forms.VisualStyles.TabItemState.Selected;
                }
                if (button.Selected) {
                    tabState |= System.Windows.Forms.VisualStyles.TabItemState.Hot;
                }

                
                TabRenderer.DrawTabItem(e.Graphics, bounds, tabState);

                if (button.Checked) {
                    VisualStyleRenderer vsr = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Hot);
                    Padding borderPadding = button.Padding;
                    borderPadding.Top += 4;
                    borderPadding.Bottom += 2;
                    borderPadding.Left -= 2;
                    borderPadding.Right -= 2;
                    Rectangle rect = LayoutUtils.DeflateRect(bounds, borderPadding);
             

                    ControlPaint.DrawFocusRectangle(e.Graphics, rect);
                }
            }
            else {
                base.OnRenderButtonBackground(e);
            }
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) {
            
            base.OnRenderItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, e.ImageRectangle));
        }

        
    }
}
