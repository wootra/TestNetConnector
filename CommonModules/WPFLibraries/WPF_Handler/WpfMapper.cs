using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms.Integration;

namespace WPF_Handler
{
    public class WpfMapper
    {
        public static void MapTextContentAndFont(ElementHost userControlItem)
        {
            userControlItem.PropertyMap.Add("Text", new PropertyTranslator(OnContentChange));
            try
            {
                userControlItem.PropertyMap.Remove("Font");
            }
            catch { }
            userControlItem.PropertyMap.Add("Font", new PropertyTranslator(OnFontSizeChange));
            userControlItem.PropertyMap.Add("ForeColor", new PropertyTranslator(OnForegroundChange));
        }

        public static void MapTextTagAndFont(ElementHost userControlItem)
        {
            userControlItem.PropertyMap.Add("Text", new PropertyTranslator(OnTagChange));
            try
            {
                userControlItem.PropertyMap.Remove("Font");
            }
            catch { }
            userControlItem.PropertyMap.Add("Font", new PropertyTranslator(OnFontSizeChange));
        }
        
        public static void MapTextAndFont(ElementHost userControlItem)
        {
            userControlItem.PropertyMap.Add("Text", new PropertyTranslator(OnTextChange));
            try
            {
                userControlItem.PropertyMap.Remove("Font");
            }
            catch { }
            userControlItem.PropertyMap.Add("Font", new PropertyTranslator(OnFontSizeChange));
        }

        private static void OnTagChange(object h, String propertyName, object value)
        {
            ElementHost host = h as ElementHost;

            System.Windows.Controls.UserControl wpfButton =
                host.Child as System.Windows.Controls.UserControl;
            wpfButton.Tag = value as String;
        }

        private static void OnContentChange(object h, String propertyName, object value)
        {
            ElementHost host = h as ElementHost;
            String str = value as String;
            System.Windows.Controls.UserControl wpfButton =
                host.Child as System.Windows.Controls.UserControl;
            wpfButton.Content = str;
            
        }
        private static void OnTextChange(object h, String propertyName, object value)
        {
            ElementHost host = h as ElementHost;
            String str = value as String;
            IWpfControls wpfButton =
                host.Child as IWpfControls;
            wpfButton.Text = str;

        }
        
        private static void OnFontSizeChange(object h, String propertyName, object value)
        {
            ElementHost host = h as ElementHost;
            BrushConverter bc = new BrushConverter();
            System.Drawing.Font font = value as System.Drawing.Font;
            
            System.Windows.Controls.UserControl wpfButton =
                host.Child as System.Windows.Controls.UserControl;
            wpfButton.FontFamily = new FontFamily(font.FontFamily.Name);
            wpfButton.FontSize = font.Size;
            wpfButton.FontWeight = (font.Bold) ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Regular;
        }

        private static void OnForegroundChange(object h, String propertyName, object value)
        {
            ElementHost host = h as ElementHost;
            
            System.Drawing.Color color = (System.Drawing.Color)value;
            System.Windows.Controls.UserControl wpfButton =
                host.Child as System.Windows.Controls.UserControl;
            BrushConverter bCon = new BrushConverter();
            Brush br;
            try
            {
                Color col = Color.FromArgb(color.A,color.R, color.G, color.B);
                br = new SolidColorBrush(col);
                wpfButton.Foreground = br;
            }
            catch {
                br = Brushes.Black;
                wpfButton.Foreground = br;
            }
            
        }
    }
}
