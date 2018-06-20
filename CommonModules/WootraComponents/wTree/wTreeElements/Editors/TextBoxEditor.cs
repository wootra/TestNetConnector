using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class TextBoxEditor : wTreeEditor
    {
        TextBox _textBox;
        public event TextEditorTextChanged E_TextChanged;

        public TextBoxEditor(TextBox textBox, wTree ownerTree)
            : base(textBox, ownerTree)
        {
            _textBox = textBox;

            _textBox.KeyUp += textEditor_KeyUp;
            _textBox.LostFocus += textEditor_LostFocus;
            _textBox.TextChanged += _textBox_TextChanged;
        }

        void _textBox_TextChanged(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(OwnerTree.wDrawHandler._tempBufferToDraw);
            SizeF size = g.MeasureString(_textBox.Text, OwnerTree.Font);
            _textBox.SetBounds(0, 0, (int)size.Width + 10, 0, BoundsSpecified.Width);
            if (E_TextChanged != null) E_TextChanged(ItemToEdit.OwnerNode, ItemToEdit, _textBox.Text);
        }

        void textEditor_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;
            wTreeNodeItem item = ItemToEdit;
            if (Control.ModifierKeys == Keys.None)
            {
                if ((e.KeyData) == Keys.Enter)
                {
                    object oldValue = item.Value;

                    OnEditorValueChanged(oldValue, box.Text, true);

                    HideEditor();
                    _isValueChanged = false;
                }
                else if ((e.KeyData) == Keys.Escape)
                {
                    OnValueChangeCanceled();
                    HideEditor();

                }
            }
        }

        /// <summary>
        /// item을 위해서 textboxeditor를 보여준다.
        /// </summary>
        /// <param name="item"></param>
        public override void ShowEditorFor(wTreeNodeItem item, Rectangle area)
        {
            if (SelectionHandler.SelectedNode != item.OwnerNode)
            {
                SelectionHandler.SelectedNode = item.OwnerNode;
                DrawHandler.ReDrawTree(false);
            }
            EditorControl.Text = item.Value.ToString();

            base.ShowEditorFor(item, area);

        }

        void textEditor_LostFocus(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            wTreeNodeItem item = ItemToEdit;

            object oldValue = item.Value;
            OnEditorValueChanged(oldValue, box.Text);
            HideEditor();


        }

        /// <summary>
        /// Text를 변경한다.
        /// </summary>
        /// <param name="p"></param>
        public override void SetValue(object p)
        {
            _textBox.Text = p.ToString();
        }

    }
    public delegate void TextEditorTextChanged(wTreeNode node, wTreeNodeItem item, string changedText);
}