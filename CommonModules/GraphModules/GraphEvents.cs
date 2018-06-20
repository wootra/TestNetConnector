using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Gigasoft.ProEssentials;
using System.IO;

namespace GraphModules
{
    public delegate void SnapShotEventHandler(Object sender, SnapShotEventArgs e);

    public class SnapShotEventArgs : EventArgs
    {
        public Bitmap Image { get; set; }
        public ImageFormat _format = null;
        public SnapShotEventArgs(Pesgo graph, ImageFormat format=null)
        {
            Bitmap bitmap = new Bitmap(graph.Width, graph.Height, graph.CreateGraphics());
            _format = format;
            graph.DrawToBitmap(bitmap, graph.ClientRectangle);
            Image = bitmap;
        }

        public void SaveToFile(String fileName, ImageFormat format)
        {
            MemoryStream _bmpStream = new MemoryStream();
            if (_format != null) Image.Save(_bmpStream, _format);
            else Image.Save(_bmpStream, format);
            //Image.Dispose();
            FileStream file = File.Create(fileName);

            _bmpStream.WriteTo(file);
            file.Close();
            _bmpStream.Close();
        }
    }

}
