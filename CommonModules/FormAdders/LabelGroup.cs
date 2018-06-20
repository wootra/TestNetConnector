using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataHandling;

namespace FormAdders
{
    public partial class LabelGroup : UserControl
    {
        List<String> _nameList = new List<string>();
        Dictionary<String, Object> _items = new Dictionary<string,object>();
        Dictionary<String, Image> _images = new Dictionary<string,Image>();
        List<Control> _panelList = new List<Control>();
        Control _selectedPanel;
        Boolean _isPanelVisible = true;
        public new event DragLeaveEventHandler DragLeave;
        MouseEventHandler _MouseMove;
        MouseEventHandler _MouseUp;
        int _labelHeight = 15;
        int _comboHeight = 20;

        public LabelGroup()
            : base()
        {
            InitializeComponent();
            this.Dock = DockStyle.Top;
            B_Title.Click += new EventHandler(B_Title_Click);
            _MouseMove = new MouseEventHandler(al_MouseMove);
            _MouseUp = new MouseEventHandler(al_MouseUp);
            C_PanelChanger.SelectedIndexChanged += new EventHandler(C_PanelChanger_SelectedIndexChanged);
            _selectedPanel = P_Back;
            AddPanel("Items", P_Back);
            SelectPanel(0);
            this.Load += new EventHandler(LabelGroup_Load);
        }



        public void AddPanel(String name, Control panel)
        {
            
            C_PanelChanger.Items.Add(name);
            _panelList.Add(panel);
            if (this.Controls.Contains(panel) == false)
            {
                this.Controls.Add(panel);
                panel.Dock = DockStyle.Bottom;
                panel.Hide();
            }
            if (_panelList.Count < 2)
            {
                C_PanelChanger.Hide();
                _comboHeight = 0;
            }
            else
            {
                C_PanelChanger.Show();
                _comboHeight = C_PanelChanger.Height;
            }
            
        }

        public void SelectPanel(int index)
        {
            //if (_selectedPanel == _panelList[index]) return;
            //this.SuspendLayout();
            if(_selectedPanel!=null) _selectedPanel.Hide(); //과거 패널을 숨기고
            _selectedPanel = _panelList[index];
            _selectedPanel.Show();
            C_PanelChanger.SelectedIndex = index;
            //this.ResumeLayout();
            resizeGroup();
        }

        void C_PanelChanger_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = sender as ComboBox;
            int sel = c.SelectedIndex;
            SelectPanel(sel);

        }

        void LabelGroup_Load(object sender, EventArgs e)
        {
            resizeGroup();
        }
        void resizeGroup()
        {

            this.SuspendLayout();
            //_selectedPanel.SuspendLayout();
            resizePanel();
            int height = 0;
            if (_isPanelVisible)
            {
                height = _selectedPanel.Height;
                _selectedPanel.Show();

            }
            else
            {
                height = 0;
                _selectedPanel.Hide();
            }
            
            this.SetBounds(0,0,0, B_Title.Height + _comboHeight+ height, BoundsSpecified.Height);
            //P_Back.ResumeLayout();
            this.ResumeLayout();
            //this.Refresh();
        }
        void resizePanel()
        {
            P_Back.SuspendLayout();
            P_Back.SetBounds(0, 0, 0, _items.Count * (_labelHeight + 1), BoundsSpecified.Height);
            P_Back.ResumeLayout();
            
        }

        public void ShowPanel(Boolean isShow)
        {
            _isPanelVisible = !isShow;
            B_Title_Click(null, null);
        }
        public void PanelEnable(Boolean isEnable)
        {
            _selectedPanel.Enabled = isEnable;
            if (isEnable)
            {
                _selectedPanel.BackColor = SystemColors.ButtonHighlight;
            }
            else
            {
                _selectedPanel.BackColor = SystemColors.Control;
            }
        }

        void B_Title_Click(object sender, EventArgs e)
        {
            //int height = 0;

            if (_isPanelVisible)
            {
                B_Title.Image = Properties.Resources.tree_close;
            }
            else
            {
                B_Title.Image = Properties.Resources.tree_open;
            }
            _isPanelVisible = !_isPanelVisible;
            resizeGroup();
        }

        [Browsable(true)]
        [SettingsBindable(true)]
        ///<summary> 제목에 나타나는 텍스트문구입니다. </summary>
        
        public String Title
        {
            get { return B_Title.Text; }
            set { B_Title.Text = value; }
        }

        public void addItem(String name, Object item, Image icon=null){
            
            _items.Add(name, item);
            Image.GetThumbnailImageAbort a = callBackAbort;
            if (icon != null) _images.Add(name,icon);
            else _images.Add(name, Properties.Resources.tree_close.GetThumbnailImage(17,17,a,IntPtr.Zero));
            makeNewLabel(name, item, icon);
            _nameList.Add(name);

            resizeGroup();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Control c in P_Back.Controls)
            {
                c.SetBounds(0, 0, this.Width, 0, BoundsSpecified.Width);
            }
        }
        
        ActiveLabel makeNewLabel(String name, Object item, Image icon)
        {
            ActiveLabel aLabel = new ActiveLabel("     "+name,item);
                    // 
            // activeLabel1
            // 
            //aLabel.BackColor = System.Drawing.SystemColors.Control;
            
            if(icon!=null) aLabel.Image = icon;
            aLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;

            aLabel.Location = new System.Drawing.Point(0, _labelHeight * _nameList.Count + 1000);
            aLabel.Name = name;
            aLabel.Size = new System.Drawing.Size(100, _labelHeight);
            aLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            aLabel.MouseDown += new MouseEventHandler(aLabel_MouseDown);
            aLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            //aLabel.DragLeave +=new EventHandler(aLabel_DragLeave);
            aLabel.Parent = P_Back;
            P_Back.SuspendLayout();
            P_Back.Controls.Add(aLabel);
            
            P_Back.ResumeLayout();
            
            return aLabel;
        }
        public void setBackColor(String name, Color color)
        {
            foreach (Control c in P_Back.Controls)
            {
                if (c.Name == name)
                {
                    (c as ActiveLabel).setBackColor(color);
                    return;
                }
            }
        }

        Boolean _isDragging = false;
        void aLabel_MouseDown(object sender, MouseEventArgs e)
        {
            ActiveLabel al = sender as ActiveLabel;
            String name = al.Name;
            Object item = _items[name];

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.None)
                {
                    if (DragLeave != null) DragLeave(new DragLeaveEventArgs(item));
                    DoDragDrop(item, DragDropEffects.Copy);
                }

            }
            aLabelPressed(name, item, e);
            /*
            al.MouseMove += _MouseMove;
            al.MouseUp += _MouseUp;
            if (_isDragging) _isDragging = false;
             */
        }
        protected virtual void aLabelPressed(String sender, Object item, MouseEventArgs e)
        {
            //for override
        }

        void al_MouseUp(object sender, MouseEventArgs e)
        {
            ActiveLabel al = sender as ActiveLabel;
            al.MouseMove -= _MouseMove;
            al.MouseUp -= _MouseUp;
            _isDragging = false;
            aLabelReleased(al.Name, _items[al.Name], e);
        }

        protected virtual void aLabelReleased(String sender, Object item, MouseEventArgs e)
        {
            //for override
        }

        void al_MouseMove(object sender, MouseEventArgs e)
        {
            ActiveLabel al = sender as ActiveLabel;
            if (_isDragging == false)
            {
                al.DoDragDrop(al.getItem(), DragDropEffects.Copy);
                _isDragging = true;
            }
            if (DragLeave != null && _isDragging) DragLeave(new DragLeaveEventArgs(al.getItem()));
        }
        /*
        void aLabel_DragLeave(object sender, EventArgs e)
        {
            ActiveLabel al = (ActiveLabel)sender;
            al.DoDragDrop(al.getItem(), DragDropEffects.Copy);
            if (DragLeave != null) DragLeave(new DragLeaveEventArgs(al.getItem()));
        }
        */
        #region removeItem
        public void removeItem(String name)
        {
            _items.Remove(name);
            _nameList.Remove(name);
            _images.Remove(name);
            P_Back.Controls.RemoveByKey(name);
        }
        public void removeItem(int index)
        {
            try
            {
                String key = _nameList[index];
                removeItem(key);
            }
            catch {
                throw;
            }
        }
        public void removeAllItems()
        {
            while (_nameList.Count > 0)
            {
                removeItem(0);
            }
        }
        #endregion

        Boolean callBackAbort()
        {
            return false;
        }


    }
   
}
