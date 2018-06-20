using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class ComboBoxEditor:wTreeEditor
    {
        
        public ComboBoxEditor(ComboBox comboBox, wTree ownerTree)
            : base(comboBox, ownerTree)
        {

            comboBox.SelectedIndexChanged += comboEditor_SelectedIndexChanged;
            comboBox.LostFocus += comboEditor_LostFocus;
            EventEnabled = true;
        }

        ComboBox ComboBox { get { return EditorControl as ComboBox; } }

        void comboEditor_LostFocus(object sender, EventArgs e)
        {
            if (EventEnabled == false) return;
            ComboBox box = sender as ComboBox;
            wTreeNodeItem item = ItemToEdit;

            object oldValue = item.Value;
            OnEditorValueChanged(oldValue, box.Text);
            HideEditor();
            
        }


        public bool EventEnabled { get; set; }

        void comboEditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EventEnabled == false) return;
            ComboBox box = sender as ComboBox;
            wTreeNodeItem item = ItemToEdit;
            object oldValue = item.Value;

            item.TextArraySelectedIndex = box.SelectedIndex;

            OnEditorValueChanged(oldValue, item.TextArraySelectedIndex);
            HideEditor();
            
        }


        /// <summary>
        /// item을 위해서 textboxeditor를 보여준다.
        /// </summary>
        /// <param name="item"></param>
        public override void ShowEditorFor(wTreeNodeItem item, Rectangle area)
        {
            ComboBox.Items.Clear();
            Image temp = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(temp);
            int maxWid = 0;

            foreach (string text in item.TextArray)
            {
                SizeF size = g.MeasureString(text, ComboBox.Font);
                if (maxWid < (int)size.Width + 1) maxWid = (int)size.Width + 1;
                ComboBox.Items.Add(text);
            }
            Rectangle rect = new Rectangle(area.X, area.Y, maxWid + 10, area.Height);
            base.ShowEditorFor(item, rect);
            ComboBox.DroppedDown = true;
            EventEnabled = false;
            ComboBox.SelectedIndex = item.TextArraySelectedIndex;
            EventEnabled = true;
            
        }

        /// <summary>
        /// Editor의 Value를 셋팅한다. int로 하면 combobox의 순서대로 값이 들어갈 것이고, 아니면 text를 선택하여 들어갈 것이다.
        /// </summary>
        /// <param name="p"></param>
        public override void SetValue(object p)
        {
            
            if (p is int)
            {
                ComboBox.SelectedIndex = (int)p;
            }
            else if (p is String)
            {
                ComboBox.Text = p.ToString();
            }
            base.SetValue(p);
        }
    }
}
