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
    public partial class ControlGroup : UserControl
    {
        List<String> _nameList = new List<string>();
        Dictionary<String, Control> _items = new Dictionary<string,Control>();
        Boolean _isPanelVisible = true;
        public new event DragLeaveEventHandler DragLeave;
        MouseEventHandler _MouseMove;
        MouseEventHandler _MouseUp;
        public ControlGroup()
            : base()
        {
            InitializeComponent();
            B_Title.Click += new EventHandler(B_Title_Click);
        }

        void B_Title_Click(object sender, EventArgs e)
        {
            int height = 0;
            if (_isPanelVisible)
            {
                P_Back.Visible = false;
                P_Back.SetBounds(0, 0, P_Back.Width, 0, BoundsSpecified.Height);
                B_Title.Image = Properties.Resources.tree_close;
            }
            else
            {
                P_Back.Visible = true;
                height = _items[_nameList[0]].Height;
                P_Back.SetBounds(0, 0, P_Back.Width, height , BoundsSpecified.Height);
                B_Title.Image = Properties.Resources.tree_open;
            }
            _isPanelVisible = !_isPanelVisible;
            this.Height = B_Title.Height + height;
        }

        [Browsable(true)]
        [SettingsBindable(true)]
        ///<summary> 제목에 나타나는 텍스트문구입니다. </summary>
        public String Title
        {
            get { return B_Title.Text; }
            set { B_Title.Text = value; }
        }

        public void addItem(String name, Control item){
            _nameList.Add(name);
            _items.Add(name, item);
            Image.GetThumbnailImageAbort a = callBackAbort;
            item.Dock = DockStyle.Top;
            P_Back.Dock = DockStyle.Fill;
            P_Back.Controls.Add(item);
            P_Back.Height = item.Height;
            this.Height = B_Title.Height + item.Height+10;
        }
        

        #region removeItem
        public void removeItem(String name)
        {
            _items.Remove(name);
            _nameList.Remove(name);
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
