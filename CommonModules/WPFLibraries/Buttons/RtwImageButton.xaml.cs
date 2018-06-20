using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Handler;
using System.ComponentModel;

namespace Buttons
{
    /// <summary>
    /// RtwImageButton.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class RtwImageButton : UserControl, IWpfControls, INotifyPropertyChanged
    {
        ImageSource _btn=null;
        ImageSource _btnDisabled=null;
        
        public RtwImageButton()
        {
            InitializeComponent();
            _events = new EventHelper(this, this);
        }

        EventHelper _events;
        public EventHelper Events { get { return _events; } }
        public String Text { get { return this.Content as String; } set { this.Content = value; } }

        public event PropertyChangedEventHandler PropertyChanged;
        
        bool _isChecking = false;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                if (_isChecking) return;
                _isChecking = true;//무한반복을 막기 위함.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                _isChecking = false;
            }
        }

        public EventHelper U_Events { get { return _events; } }

        int _imgWidth=0;
        int _imgHeight=0;
        public int U_ImageWidth{ 
            get{ return _imgWidth;} 
            set{ 
                _imgWidth=value;
                if (_imageSourcePath.Length > 0) _btn = ImageHandler.getImageSource(_imageSourcePath, _imgWidth, _imgHeight);
                if (_disabledImageSourcePath.Length > 0) _btnDisabled = ImageHandler.getImageSource(_disabledImageSourcePath, _imgWidth, _imgHeight);
            }
        }
        public int U_ImageHeight{ 
            get{ return _imgHeight;} 
            set{
                _imgHeight=value;
                if (_imageSourcePath.Length > 0) _btn = ImageHandler.getImageSource(_imageSourcePath, _imgWidth, _imgHeight);
                if (_disabledImageSourcePath.Length > 0) _btnDisabled = ImageHandler.getImageSource(_disabledImageSourcePath, _imgWidth, _imgHeight);
            }
        }

        String _imageSourcePath="";
        public String U_ImageSourcePath
        {
            set
            {
                _imageSourcePath = value;
                _btn = ImageHandler.getImageSource(value, _imgWidth, _imgHeight);
            }
        }
        String _disabledImageSourcePath = "";
        public String U_DisabledImageSourcePath
        {
            set
            {
                _disabledImageSourcePath = value;
                _btnDisabled = ImageHandler.getImageSource(value, _imgWidth, _imgHeight);
            }
        }


        public Boolean U_Enabled
        {
            set
            {
                this.IsEnabled = value;
                if(_btn!=null) InnerImage.Source = (value)? _btn : (_btnDisabled!=null)? _btnDisabled : _btn;
            }
            get
            {
                return this.IsEnabled;
            }
        }
        public void setImage(String imagePath, int width=0, int height=0)
        {
            _btn = ImageHandler.getImageSource(imagePath, width, height);
            _imageSourcePath = imagePath;
            _imgWidth = width;
            _imgHeight = height;
        }
        public void setDisabledImage(String imagePath, int width = 0, int height = 0)
        {
            _btnDisabled = ImageHandler.getImageSource(imagePath, width, height);
            _disabledImageSourcePath = imagePath;
            _imgWidth = width;
            _imgHeight = height;
        }
        public void setImages(String normalImagePath, String disabledImagePath, int width = 0, int height = 0)
        {
            setImage(normalImagePath, width, height);
            setDisabledImage(disabledImagePath, width, height);
        }
        
    }
}
