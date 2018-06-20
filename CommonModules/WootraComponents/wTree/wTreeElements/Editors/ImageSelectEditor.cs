using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WootraComs.wTreeElements.Editors;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class ImageSelectEditor:wTreeEditor
    {
        UserControl _imgSelector;
        public ImageSelectEditor(UserControl selector, wTree ownerTree): base(selector, ownerTree)
        {
            _imgSelector = selector;

            _imgSelector.LostFocus += _imgSelector_LostFocus;
                            
        }

        void _imgSelector_LostFocus(object sender, EventArgs e)
        {

            base.HideEditor();
        }

        /// <summary>
        /// item을 위해서 textboxeditor를 보여준다.
        /// </summary>
        /// <param name="item"></param>
        public override void ShowEditorFor(wTreeNodeItem item, Rectangle area)
        {
            base.ShowEditorFor(item, area);
            ImageSelectorDialog dlg = new ImageSelectorDialog();
            DialogResult result = dlg.ShowDialog(item.Image, item._imageWidth, item._imageHeight);
            if (result == DialogResult.Abort || result == DialogResult.Cancel)
            {
                return;
            }
            item.Image = dlg.NewImage;
            item._imageWidth = dlg.ImageWidth;
            item._imageHeight = dlg.ImageHeight;
            item.OwnerNode.DrawBuffer();
            DrawHandler.ReDrawTree(false);

        }

        /// <summary>
        /// Editor의 Value를 셋팅한다. int로 하면 combobox의 순서대로 값이 들어갈 것이고, 아니면 text를 선택하여 들어갈 것이다.
        /// </summary>
        /// <param name="p"></param>
        public override void SetValue(object p)
        {

            
            base.SetValue(p);
        }

    }
}
