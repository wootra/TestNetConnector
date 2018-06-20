using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xaml;

namespace WpfHandlers
{
    public class ImageHandlers
    {
        public static ImageSource GetImageSource(Bitmap bitmap)
        {
            MemoryStream img = new MemoryStream();
            Bitmap imgFile = bitmap;
            imgFile.Save(img, System.Drawing.Imaging.ImageFormat.Png);
            img.Seek(0, SeekOrigin.Begin);
            BitmapFrame newImg = BitmapFrame.Create(img);
            return newImg;
        }
    }
}
