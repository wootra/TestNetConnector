using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace WPF_Handler
{
    public class ImageHandler
    {

        public static ImageSource getImageSource(String imgPath, int width = 0, int height = 0)
        {
            byte[] imageBytes = LoadImageData(imgPath);

            return CreateImage(imageBytes, width, height);
        }
        
        public static void ResizeImage(String srcPath, String dstPath, String decodeTo="gif", int width=0, int height=0)
        {

            byte[] imageBytes = LoadImageData(srcPath);



            // decode the image such that its width is 120 and its 

            // height is scaled proportionally

            ImageSource imageSource = CreateImage(imageBytes, width, height);



            // the following will decode the image to its natural size

            // imageSource = CreateImage(imageBytes, 0, 0);



            // the following will decode the image such that its height

            // is 160 and its width is scaled proportionally

            // imageSource = CreateImage(imageBytes, 0, 160);



            // the following will decode the image to exactly 120 x 160 

            // imageSource = CreateImage(imageBytes, 120, 160);

            imageBytes = GetEncodedImageData(imageSource, "."+decodeTo.ToLower());

            SaveImageData(imageBytes, dstPath);

        }





        public static byte[] LoadImageData(string filePath)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                BinaryReader br = new BinaryReader(fs);

                byte[] imageBytes = br.ReadBytes((int)fs.Length);

                br.Close();

                fs.Close();
                return imageBytes;
            }
            catch(Exception e) {
                System.Windows.Forms.MessageBox.Show(e.Message + "\r\n"+e.ToString());
                return new byte[256];
            }
            

        }



        public static void SaveImageData(byte[] imageData, string filePath)
        {

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(imageData);

            bw.Close();

            fs.Close();

        }



        public static ImageSource CreateImage(byte[] imageData, int decodePixelWidth, int decodePixelHeight)
        {

            if (imageData == null) return null;



            BitmapImage result = new BitmapImage();

            result.BeginInit();

            if (decodePixelWidth > 0)
            {

                result.DecodePixelWidth = decodePixelWidth;

            }

            if (decodePixelHeight > 0)
            {

                result.DecodePixelHeight = decodePixelHeight;

            }

            result.StreamSource = new MemoryStream(imageData);

            result.CreateOptions = BitmapCreateOptions.None;

            result.CacheOption = BitmapCacheOption.Default;

            result.EndInit();

            return result;

        }



        public static byte[] GetEncodedImageData(ImageSource image, string preferredFormat)
        {

            byte[] result = null;

            BitmapEncoder encoder = null;

            switch (preferredFormat.ToLower())
            {

                case ".jpg":

                case ".jpeg":

                    encoder = new JpegBitmapEncoder();

                    break;



                case ".bmp":

                    encoder = new BmpBitmapEncoder();

                    break;



                case ".png":

                    encoder = new PngBitmapEncoder();

                    break;



                case ".tif":

                case ".tiff":

                    encoder = new TiffBitmapEncoder();

                    break;



                case ".gif":

                    encoder = new GifBitmapEncoder();

                    break;



                case ".wmp":

                    encoder = new WmpBitmapEncoder();

                    break;

            }



            if (image is BitmapSource)
            {

                MemoryStream stream = new MemoryStream();

                encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));

                encoder.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);

                result = new byte[stream.Length];

                BinaryReader br = new BinaryReader(stream);

                br.Read(result, 0, (int)stream.Length);

                br.Close();

                stream.Close();

            }

            return result;

        }




    }
}
