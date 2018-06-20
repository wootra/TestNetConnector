using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Drawing2D;

namespace TabStripApp {
    class TabStripProfessionalRenderer : ToolStripProfessionalRenderer {
 
        private const int BOTTOM_LEFT = 0;
        private const int TOP_LEFT = 1;
        private const int TOP_RIGHT = 2;
        private const int BOTTOM_RIGHT = 3;

        public TabStripProfessionalRenderer() {
            this.RoundedEdges = false;
        }


        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
            TabStrip tabStrip = e.ToolStrip as TabStrip;
            Tab tab = e.Item as Tab;

            if (tab == null) {
                base.OnRenderButtonBackground(e);
                return;
            }
            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
            Graphics g = e.Graphics;
           

            // we want an even rise in the angle, for every pixel we have to go up, 
            // we need to go over one pixel.  This will give us a straig
            
            if (tabStrip != null) {
                Point[] tabPolygonPoints = new Point[] {
                            new Point(0, bounds.Bottom), // lower left hand corner
                            new Point(tab.Height,bounds.Top), // upper left hand corner
                            new Point(bounds.Right-1, bounds.Top), // upper right hand corner
                            new Point(bounds.Right-1, bounds.Bottom)  // lower right hand corner
                        };

                Point[] fillTabPolygonPoints = new Point[] {
                         tabPolygonPoints[BOTTOM_LEFT],
                         new Point(tabPolygonPoints[TOP_LEFT].X -2, tabPolygonPoints[TOP_LEFT].Y +2),
                         new Point(tabPolygonPoints[TOP_LEFT].X +2, tabPolygonPoints[TOP_LEFT].Y),
                         tabPolygonPoints[TOP_RIGHT],
                         tabPolygonPoints[BOTTOM_RIGHT]
                        };


                Color startColor = ColorTable.MenuStripGradientEnd;
                Color endColor = ColorTable.MenuStripGradientBegin;



                if (tab.Selected && !tab.Checked) {
                    startColor = ColorTable.ButtonSelectedGradientBegin;
                    endColor = ColorTable.ButtonSelectedGradientEnd;
                }
                using (Brush b = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Vertical)) {
                    g.FillPolygon(b, fillTabPolygonPoints);
                }



               
               
                using (Pen outerBlueBorderPen = new Pen(ColorTable.ButtonSelectedBorder)) {
                    using (Pen innerWhiteBorderPen = new Pen(ColorTable.GripLight)) {
                        
                        //
                        // draw left edge 
                        //
                        //   Bw
                        //  Bw 
                        // Bw
                        Point edgeStart = tabPolygonPoints[BOTTOM_LEFT];
                        Point edgeStop = tabPolygonPoints[TOP_LEFT];
                        edgeStop.Offset(-2,2); // stop one pixels early.

                        g.DrawLine(outerBlueBorderPen, edgeStart, edgeStop);
                      
                        // draw white shadow line along left edge
                        edgeStart.Offset(1,0);
                        edgeStop.Offset(1,0);
                        g.DrawLine(innerWhiteBorderPen, edgeStart, edgeStop);

                        //
                        // draw rounded corner
                        //       TOP_LEFT
                        //          v
                        //          [BBBBBBBBBBBBBBBBBBBBBB] <- top edge
                        //        BB[wwwwwwwwwwwwwwwwwwwwww] <- top edge shadow  (row 2)
                        //      BBww                                             (row 3)
                        //     Bww                                               (row 4)


                        // draw row 2
                        edgeStart = tabPolygonPoints[TOP_LEFT];
                        edgeStart.Offset(0,1);
                        edgeStop = edgeStart;
                        edgeStop.X += 1;

                        g.DrawLine(outerBlueBorderPen, edgeStart, edgeStop);
                        
                        // draw row 3
                        edgeStart.Offset(-2, 1);
                        edgeStop.Offset(-2,1);
                        g.DrawLine(outerBlueBorderPen, edgeStart, edgeStop);
                        edgeStart.Offset(2, 0);
                        edgeStop.Offset(2,0);
                        g.DrawLine(innerWhiteBorderPen, edgeStart, edgeStop);

                        // draw row 4
                        edgeStart.Offset(-2, 1);
                        edgeStop.Offset(-2, 1);
                        g.DrawLine(innerWhiteBorderPen, edgeStart, edgeStop);
                       
                        //
                        // draw top edge
                        //
                        //       TOP_LEFT
                        //          v
                        //          [BBBBBBBBBBBBBBBBBBBBBB] <- top edge
                        //        BB[wwwwwwwwwwwwwwwwwwwwww] <- top edge shadow  (row 2)


                        edgeStart = tabPolygonPoints[TOP_LEFT];
                        edgeStop = tabPolygonPoints[TOP_RIGHT];

                        // scoot over two pixels
                        edgeStart.Offset(2, 0);
                        edgeStop.Offset(-2, 0);

                        g.DrawLine(outerBlueBorderPen, edgeStart, edgeStop);

                        // scoot down to paint white shadow line
                        edgeStart.Offset(0, 1);
                        edgeStop.Offset(0, 1);
                        g.DrawLine(innerWhiteBorderPen, edgeStart, edgeStop);


                        //
                        // draw right edge
                        //          TOP_RIGHT
                        //              v
                        //            bB
                        //              bB
                        //              bB
                        //              bB
                        
                        edgeStart = new Point (tabPolygonPoints[TOP_RIGHT].X, tabPolygonPoints[TOP_RIGHT].Y+2);
                        edgeStop = tabPolygonPoints[BOTTOM_RIGHT];
                        
                        // draw dark blue lines
                        g.DrawLine(outerBlueBorderPen, edgeStart, edgeStop);
                        using (Brush b = new SolidBrush(outerBlueBorderPen.Color)) {
                            g.FillRectangle(b,new Rectangle(edgeStart.X - 1, edgeStart.Y - 1, 1, 1));
                        }

                        // draw light blue lines
                        using (Pen innerBlueBorderPen = new Pen(ColorTable.ButtonPressedHighlight)) {
                            edgeStart.Offset(-1, 0);
                            edgeStop.Offset(-1, 0);
                            g.DrawLine(innerBlueBorderPen, edgeStart, edgeStop);
                            using (Brush b = new SolidBrush(innerBlueBorderPen.Color)) {
                                g.FillRectangle(b, new Rectangle(edgeStart.X - 1, edgeStart.Y - 1, 1, 1));
                            }

                        }
                    }
                }

                
                tabPolygonPoints[0].Offset(1, 0);
                tabPolygonPoints[1].Offset(1, 0);

            }
            else {
                base.OnRenderButtonBackground(e);
            }

        }

      
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
           TabStrip tabStrip = e.ToolStrip as TabStrip;
           
            using (Pen outerBlueBorderPen = new Pen(ColorTable.ButtonSelectedBorder)) {
                using (Pen innerWhiteBorderPen = new Pen(ColorTable.GripLight)) {
                    if (tabStrip != null) {
                        if (tabStrip.SelectedTab != null) {
                            // left border coords
                            Point borderStart1 = new Point(0, tabStrip.SelectedTab.Bounds.Bottom);
                            Point borderStop1 = new Point(tabStrip.SelectedTab.Bounds.Left, tabStrip.SelectedTab.Bounds.Bottom);

                            // right border coords
                            Point borderStart2 = new Point(tabStrip.SelectedTab.Bounds.Right - 1, tabStrip.SelectedTab.Bounds.Bottom);
                            Point borderStop2 = new Point(tabStrip.ClientRectangle.Right, tabStrip.SelectedTab.Bounds.Bottom);
                           
                            e.Graphics.DrawLine(outerBlueBorderPen,borderStart1, borderStop1);
                            e.Graphics.DrawLine(outerBlueBorderPen,borderStart2, borderStop2);
                            
                            // shift all points down one to draw the white line
                            borderStop1.Offset(0,1);
                            borderStart1.Offset(0,1);
                            borderStart2.Offset(0,1);
                            borderStop2.Offset(0,1);
                            e.Graphics.DrawLine(innerWhiteBorderPen, borderStart1, borderStop1);
                            e.Graphics.DrawLine(innerWhiteBorderPen, borderStart2, borderStop2);
                      
                        }

                    }
                }
            }
            
        }
        
    }
}
