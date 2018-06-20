using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UserDialogs
{
    public partial class MyDialog : Form
    {
        public MyDialog()
        {
            InitializeComponent();
        }

        int _selectedIndex = -1;
        public int U_SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
        }

        public enum ButtonAlign{ Horizon = 0, Vertical};

        int yPos = 0;
        int maxWid = 0;
        String _text;
        String _caption;
        ButtonAlign _btnAlign;
        String[] _buttons;

        void init(String text, String caption, ButtonAlign buttonAlign, params String[] buttons)
        {
            this.HelpButton = false;
            this.Text = caption;
            yPos = 0;
            maxWid = 0;


            AddText(text);
            Panel buttonArea = addButtons(buttonAlign, buttons);

            int textWid = L_TextArea.Width;
            int buttonWid = buttonArea.Width;

            int height = yPos;

            this.SuspendLayout();

            if (textWid > buttonWid)
            {
                buttonArea.SetBounds(0, 0, textWid, 0, BoundsSpecified.Width);
                this.SetBounds(0, 0, textWid + 30, height + 30, BoundsSpecified.Size);

            }
            else
            {
                this.SetBounds(0, 0, buttonWid + 30, height + 30, BoundsSpecified.Size);
            }
            if (buttonAlign == ButtonAlign.Vertical)
            {
                ResetChildrenWidth(buttonArea, this.Width - this.Margin.Left - this.Margin.Right);
            }
            this.ResumeLayout();
        }

        

        public MyDialog(String text, String caption, ButtonAlign buttonAlign, params String[] buttons)
        {
            InitializeComponent();
            _text = text;
            _caption = caption;
            _btnAlign = buttonAlign;
            _buttons = buttons;

            
        }
        public int ShowDialog(String text, String caption, ButtonAlign buttonAlign, params String[] buttons)
        {
            _text = text;
            _caption = caption;
            _btnAlign = buttonAlign;
            _buttons = buttons;
            return ShowDialog();
        }

        public new int ShowDialog(Form parent=null)
        {
            
            init(_text, _caption, _btnAlign, _buttons);

            base.ShowDialog(parent);
            return _selectedIndex;
        }

        public void AddText(String text)
        {
            _text = text;
            L_TextArea.Text = text;

            yPos += L_TextArea.Height + L_TextArea.Location.Y+ 10;
            maxWid = (maxWid < L_TextArea.Width) ? L_TextArea.Width : maxWid;
        }

        public void ResetChildrenWidth(System.Windows.Forms.Control control, int width){
            control.SetBounds(0, 0, width, 0, BoundsSpecified.Width);
            System.Windows.Forms.Control.ControlCollection children = control.Controls;
            for (int i = 0; i < children.Count; i++)
            {
                ResetChildrenWidth(children[i], width - control.Margin.Left - control.Margin.Right);
            }
        }


        bool isNewLine(char c)
        {
            if (c == '\n') return true;
            else return false;
        }

        public Panel addButtons(ButtonAlign buttonAlign, params String[] buttons)
        {
            _btnAlign = buttonAlign;
            _buttons = buttons;

            Panel panel = P_Buttons;
            P_Buttons.Controls.Clear();
            int width = 80;
            int margin = 5;
            int height = 30;
            int panelHeight;
            int panelWidth;
            //P_TopDown.SuspendLayout();
            
            //this.Controls.Add(panel);

            if(buttonAlign == ButtonAlign.Horizon){
                
               // panel.BorderStyle = BorderStyle.FixedSingle;
                panelHeight = height + margin;
                panelWidth = ((width + margin) * buttons.Length);
                panel.SetBounds(0, yPos, panelWidth, panelHeight);

            }else{
                panelHeight = (height + margin) * buttons.Length;
                panelWidth = width + margin;
                panel.SetBounds(0, yPos, panelWidth, panelHeight);
            }
            panel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            panel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            
            
            panel.SuspendLayout();
            for (int i = 0; i < buttons.Length; i++)
            {
                Button btn = new Button();
                btn.Anchor = AnchorStyles.Left;
                
                btn.Text = buttons[i];
                
                if (buttonAlign == ButtonAlign.Horizon)
                {
                    btn.SetBounds(i * (width + margin), 0, width, height);
                }
                else
                {
                    btn.SetBounds(0, i * (height + margin), width, height);
                }
                panel.Controls.Add(btn);
                btn.Tag = i;
                btn.Click += new EventHandler(btn_Click);
            }
            panel.ResumeLayout();
            this.ResumeLayout();
            yPos += panelHeight;
            maxWid = (maxWid < panelWidth) ? panelWidth : maxWid;
            return panel;
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            _selectedIndex = (int)(btn.Tag);
            this.Close();
        }
    }
}
