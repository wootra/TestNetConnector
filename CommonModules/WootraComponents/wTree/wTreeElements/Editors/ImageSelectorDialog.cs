using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WootraComs.wTreeElements.Editors
{
    public partial class ImageSelectorDialog : Form
    {
        public ImageSelectorDialog()
        {
            InitializeComponent();
            T_Width.TextChanged += T_Width_TextChanged;
            T_Height.TextChanged += T_Height_TextChanged;
        }

        void T_Height_TextChanged(object sender, EventArgs e)
        {
            int outInt;
            if (int.TryParse(T_Height.Text.Trim(), out outInt))
            {
                int wid2 = (ImageWidth < 0) ? _img1.Width : ImageWidth;
                int hig2 = (ImageHeight < 0) ? _img1.Height : ImageHeight;

                _img2 = new Bitmap(wid2, hig2);
                DrawImageResized(_img1, _img2, wid2, hig2);
                Img2.BackgroundImage = _img2;
                T_Height.BackColor = Color.White;
            }
            else
            {
                T_Height.BackColor = Color.LightPink;
            }
        }

        void T_Width_TextChanged(object sender, EventArgs e)
        {
            int outInt;
            if (int.TryParse(T_Width.Text.Trim(), out outInt))
            {
                int wid2 = (ImageWidth < 0) ? _img1.Width : ImageWidth;
                int hig2 = (ImageHeight < 0) ? _img1.Height : ImageHeight;

                _img2 = new Bitmap(wid2, hig2);
                DrawImageResized(_img1, _img2, wid2, hig2);
                Img2.BackgroundImage = _img2;

                T_Width.BackColor = Color.White;
            }
            else
            {
                T_Width.BackColor = Color.LightPink;
            }
        }
        Image _img1;
        Image _img2;
        public DialogResult ShowDialog(Image img, int wid=-1, int height=-1)
        {
            
            _img1 = img;
            Img1.BackgroundImage = img;
            int wid2 = (wid<0) ? img.Width : wid;
            int hig2 = (height<0) ? img.Height : height;

            _img2 = new Bitmap(wid2, hig2);

            DrawImageResized(_img1, _img2, wid2, hig2);
            Img2.BackgroundImage = _img2;

            T_Width.Text = wid2.ToString();
            T_Height.Text = hig2.ToString();

            return base.ShowDialog();
        }

        void DrawImageResized(Image src, Image dst, int wid, int height)
        {
            Graphics ig = Graphics.FromImage(dst);
            ig.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            ig.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            ig.DrawImage(src, 0, 0, wid, height);
        }

        public int ImageWidth
        {
            get
            {
                if (T_Width.Text.Trim().Equals(_img1.Width.ToString())) return -1;
                else
                {
                    int outInt;
                    if (int.TryParse(T_Width.Text.Trim(), out outInt))
                    {
                        return outInt;
                    }
                    else return -1;
                }
            }
        }

        public int ImageHeight
        {
            get
            {
                if (T_Height.Text.Trim().Equals(_img1.Height.ToString())) return -1;
                else
                {
                    int outInt;
                    if (int.TryParse(T_Height.Text.Trim(), out outInt))
                    {
                        return outInt;
                    }
                    else return -1;
                }
            }
        }

        public Image NewImage
        {
            get { return _img1; }
        }

        private void B_Apply_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Img1_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select Image...";
            dlg.Filter = "Image Files (*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.png;*.gif";
            dlg.Multiselect = false;
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel || result== System.Windows.Forms.DialogResult.Abort) return;
            try
            {
                Image img = Image.FromFile(dlg.FileName);
                _img1 = img;
                Img1.BackgroundImage = img;
            
                
                int wid2 = (ImageWidth < 0) ? _img1.Width : ImageWidth;
                int hig2 = (ImageHeight < 0) ? _img1.Height : ImageHeight;

                _img2 = new Bitmap(wid2, hig2);
                DrawImageResized(_img1, _img2, wid2, hig2);
                Img2.BackgroundImage = _img2;

            }
            catch
            {
                MessageBox.Show(dlg.FileName + " cannot be opened!");
            }

        }


    }
}
