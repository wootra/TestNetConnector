using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TabStripApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tabStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void lB_Style_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStyle(lB_Style.Text);
        }

        public void SetStyle(string Name)
        {
            Color HaloColor = Color.White;
            switch (Name)
            {
                case "Dark":
                    this.BackColor = Color.FromArgb(88,77,69);
                    HaloColor = Color.FromArgb(200,200,200);
                    SetBase(87,61,53,HaloColor);
                    break;
                case "Nature":
                    this.BackColor = Color.FromArgb(78, 127, 52);
                    HaloColor = Color.FromArgb(254, 209, 94);
                    SetBase(73, 118, 46,HaloColor );
                    break;
                case "Dawn":
                    this.BackColor = Color.FromArgb(177, 108, 45);
                    SetBase(172, 99, 39, Color.FromArgb(254, 209, 94));
                    break;
                case "Corn":
                    this.BackColor = Color.FromArgb(230, 193, 106);
                    SetBase(225, 184, 100, Color.FromArgb(191, 219, 255));
                    break;
                case "Chocolate":
                    this.BackColor = Color.FromArgb(87, 54, 34);
                    SetBase(82, 45, 28, Color.FromArgb(232,80,90));
                    break;
                case "Navy":
                    this.BackColor = Color.FromArgb(88,121,169);
                    SetBase(84,112,163, Color.FromArgb(254, 209, 94));
                    break;
                case "Ice":
                    this.BackColor = Color.FromArgb(235, 243, 236);
                    SetBase(228, 234, 230, Color.FromArgb(254, 209, 94));
                    break;
                case "Vanilla":
                    this.BackColor = Color.FromArgb(233, 243, 213);
                    SetBase(228, 234, 207, Color.FromArgb(254, 209, 94));
                    break;
                case "Canela":
                    this.BackColor = Color.FromArgb(235, 226, 197);
                    SetBase(228, 217, 191, Color.FromArgb(254, 209, 94));
                    break;
                case "Cake":
                    this.BackColor = Color.FromArgb(235, 213, 197);
                    SetBase(228, 204, 198, Color.FromArgb(254, 209, 94));
                    break;
                default:
                    this.BackColor = Color.FromArgb(191, 219, 255);
                    SetBase(215, 227, 242, Color.FromArgb(254, 209, 94));
                    break;
            }


        }

        public void SetBase(int R, int G, int B,Color HaloColor)
        {
            this.SuspendLayout();
            foreach (Control control in this.panel1.Controls)
            {
                if (typeof(RibbonStyle.TabStrip) == control.GetType())
                {
                    ((RibbonStyle.TabStripProfessionalRenderer)((RibbonStyle.TabStrip)control).Renderer).HaloColor = HaloColor;
                    ((RibbonStyle.TabStripProfessionalRenderer)((RibbonStyle.TabStrip)control).Renderer).BaseColor = Color.FromArgb(R + 4, G + 3, B + 3);
                    for (int i=0;i < ((RibbonStyle.TabStrip)control).Items.Count;i++)
                    {
                        RibbonStyle.Tab _tab = (RibbonStyle.Tab)((RibbonStyle.TabStrip)control).Items[i];
                        
                        #region Set Tab Colors
                        if (Color.FromArgb(R, G, B).GetBrightness() < 0.5)
                        {
                            try
                            {
                                _tab.ForeColor = Color.FromArgb(R + 76, G + 71, B + 66);
                            }
                            catch 
                            {
                                _tab.ForeColor = Color.FromArgb(250, 250, 250);
                            }
                        }
                        else
                        {
                            try
                            {
                                _tab.ForeColor = Color.FromArgb(R - 96, G - 91, B - 86);
                            }
                            catch
                            {
                                _tab.ForeColor = Color.FromArgb(10, 10, 10);
                            }
                        }
                        #endregion
                    }
                    
                    control.BackColor = Color.FromArgb(R-24, G-8, B+12);
                    
                }
                if (typeof(RibbonStyle.TabPageSwitcher) == control.GetType())
                {
                    control.BackColor = Color.FromArgb(R - 24, G - 8, B + 12);
                    
                    foreach (Control _control in control.Controls)
                    {
                        if (typeof(RibbonStyle.TabStripPage) == _control.GetType())
                        {
                            ((RibbonStyle.TabStripPage)_control).BaseColor = Color.FromArgb(R, G, B);
                            ((RibbonStyle.TabStripPage)_control).BaseColorOn = Color.FromArgb(R, G, B);

                            foreach (Control __control in _control.Controls)
                            {
                                if (typeof(RibbonStyle.TabPanel) == __control.GetType())
                                {
                                    #region Set TabPanel Colors
                                    if (Color.FromArgb(R, G, B).GetBrightness() < 0.5)
                                    {
                                        try
                                        {
                                            __control.ForeColor = Color.FromArgb(R + 76, G + 71, B + 66);
                                        }
                                        catch
                                        {
                                            __control.ForeColor = Color.FromArgb(250, 250, 250);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            __control.ForeColor = Color.FromArgb(R - 96, G - 91, B - 86);
                                        }
                                        catch
                                        {
                                            __control.ForeColor = Color.FromArgb(10, 10, 10);
                                        }
                                    }
                                    #endregion
                                    
                                    ((RibbonStyle.TabPanel)__control).BaseColor = Color.FromArgb(R, G, B);
                                    ((RibbonStyle.TabPanel)__control).BaseColorOn = Color.FromArgb(R + 16, G + 11, B + 6);

                                    foreach (Control ___control in __control.Controls)
                                    {
                                        if (typeof(RibbonStyle.RibbonButton) == ___control.GetType())
                                        {
                                            ((RibbonStyle.RibbonButton)___control).InfoColor = Color.FromArgb(R, G, B);

                                            RibbonStyle.RibbonButton _but = (RibbonStyle.RibbonButton) ___control;

                                            #region Set Button Colors
                                            if (Color.FromArgb(R, G, B).GetBrightness() < 0.5)
                                            {
                                                try
                                                {
                                                    _but.ForeColor = Color.FromArgb(R + 76, G + 71, B + 66);
                                                }
                                                catch
                                                {
                                                    _but.ForeColor = Color.FromArgb(250, 250, 250);
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    _but.ForeColor = Color.FromArgb(R - 96, G - 91, B - 86);
                                                }
                                                catch
                                                {
                                                    _but.ForeColor = Color.FromArgb(10, 10, 10);
                                                }
                                            }
                                            #endregion

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.ResumeLayout(false);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            SetStyle("Azure");
        }

        
    }
}