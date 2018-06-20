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
    public partial class PopupLabel : ActiveLabel
    {
        Dictionary<String, Object> _leftItems = new Dictionary<string, object>();
        Dictionary<String, Object> _rightItems = new Dictionary<string, object>();

        ContextMenuStrip M_SelectPopup;
        EventHandler M_SelectPopupEventHandler;
        public event ItemSelectedEventHandler ItemSelected;
        
        
        public PopupLabel(String title, ContextMenuStrip ClickPopupMenu=null):base(title,null)
        {
            this.Text = title;
            this.Dock = DockStyle.Top;
            
           // this.MouseClick += new MouseEventHandler(B_Title_Click);
            this.MouseUp += new MouseEventHandler(B_Title_Click);
            this.M_SelectPopupEventHandler = new EventHandler(M_PopupItemSelected);    
            addRightClickMenu(ClickPopupMenu);
        }


        void addRightClickMenu(ContextMenuStrip RightClickMenu)
        {
            if (RightClickMenu != null)
            {
                M_SelectPopup = RightClickMenu;
            }
            else
            {
                M_SelectPopup = new ContextMenuStrip( new System.ComponentModel.Container());
            }
            
            //M_RightClickEventHandler = new EventHandler(M_RightItemClick);
            this.M_SelectPopup.Name = "M_RightClick";
            this.M_SelectPopup.Size = new System.Drawing.Size(153, 48);
        }

        void B_Title_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                M_SelectPopupItemSetting(_leftItems);
                M_SelectPopup.Show(this, e.X, e.Y);
                M_SelectPopup.BringToFront();
            }
            else if (e.Button == MouseButtons.Right)
            {
                M_SelectPopupItemSetting(_rightItems);
                M_SelectPopup.Show(this, e.X, e.Y);
                M_SelectPopup.BringToFront();
            }
            else if(e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                M_SelectPopupItemSetting(_rightItems);
                M_SelectPopup.Show(this, e.X, e.Y);
                M_SelectPopup.BringToFront();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.XButton1)
            {
                M_SelectPopupItemSetting(_rightItems);
                M_SelectPopup.Show(this, e.X, e.Y);
                M_SelectPopup.BringToFront();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.XButton2)
            {
                M_SelectPopupItemSetting(_rightItems);
                M_SelectPopup.Show(this, e.X, e.Y);
                M_SelectPopup.BringToFront();

            }
            else if (e.Button == System.Windows.Forms.MouseButtons.None)
            {
                M_SelectPopupItemSetting(_rightItems);
                M_SelectPopup.Show(this, e.X, e.Y);
                M_SelectPopup.BringToFront();

            }
            else
            {
                M_SelectPopupItemSetting(_rightItems);
                M_SelectPopup.Show(this, e.X, e.Y);
                M_SelectPopup.BringToFront();

            }
        }

        void M_SelectPopupItemSetting(Dictionary<String, Object> items)
        {
            this.M_SelectPopup.SuspendLayout();
            this.M_SelectPopup.Items.Clear();

            foreach (String key in items.Keys)
            {
                 this.M_SelectPopup.Items.Add(key, null, M_SelectPopupEventHandler);
            }
            
            this.M_SelectPopup.ResumeLayout();

        }

        void M_PopupItemSelected(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (ItemSelected != null) ItemSelected(new ItemSelectedEventArgs(this, _leftItems[item.Text]));
            aLabelPressed(this, _leftItems[item.Text]);
        }
        protected virtual void aLabelPressed(Object sender, Object item) { }

        public void addItemForAll(String name, Object item)
        {
            _leftItems.Add(name, item);
            _rightItems.Add(name, item);
        }
        public void addItemForLeft(String name, Object item)
        {
            _leftItems.Add(name, item);
            //_rightItems.Add(name, item);
        }
        public void addItemForRight(String name, Object item)
        {
            //_leftItems.Add(name, item);
            _rightItems.Add(name, item);
        }

    }
}
