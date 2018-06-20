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
	/// Interaction logic for ImageButton.xaml
	/// </summary>
    public partial class ImageButton : UserControl, INotifyPropertyChanged, IWpfControls
	{
        ImageSource _btn1 = null;
        ImageSource _btn2 = null;
        ImageSource _btnDisabled = null;
        EventHelper _btnEvents;

		public ImageButton()
		{
			this.InitializeComponent();
            _btnEvents = new EventHelper(this, this);
		}
        public event PropertyChangedEventHandler PropertyChanged;
        Boolean _isPropertyChecking = false;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                if (_isPropertyChecking) return;
                _isPropertyChecking = true;//무한반복을 막기 위함.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                _isPropertyChecking = false;
            }
        }

        public EventHelper U_Events { get { return _btnEvents; } }
        public EventHelper Events { get { return _btnEvents; } }

        String _text = "label";
        public String Text { 
            get {
                return _text;
            } 
            set {
                _text = value;
                NotifyPropertyChanged("Text");
            } 
        }

        int _imgWidth = 0;
        int _imgHeight = 0;
        public int U_ImageWidth
        {
            get { return _imgWidth; }
            set
            {
                _imgWidth = value;
                if (_image1SourcePath.Length > 0)
                {
                    _btn1 = ImageHandler.getImageSource(_image1SourcePath, _imgWidth, _imgHeight);
                    Image1.Source = _btn1;
                }
                if (_image2SourcePath.Length > 0)
                {
                    _btn2 = ImageHandler.getImageSource(_image2SourcePath, _imgWidth, _imgHeight);
                    Image2.Source = _btn2;
                }
                if (_disabledImageSourcePath.Length > 0)
                {
                    _btnDisabled = ImageHandler.getImageSource(_disabledImageSourcePath, _imgWidth, _imgHeight);
                    
                }
                NotifyPropertyChanged("U_ImageWidth");
            }
        }
        public int U_ImageHeight
        {
            get { return _imgHeight; }
            set
            {
                _imgHeight = value;
                if (_image1SourcePath.Length > 0)
                {
                    _btn1 = ImageHandler.getImageSource(_image1SourcePath, _imgWidth, _imgHeight);
                    Image1.Source = _btn1;
                }
                if (_image2SourcePath.Length > 0)
                {
                    _btn2 = ImageHandler.getImageSource(_image2SourcePath, _imgWidth, _imgHeight);
                    Image2.Source = _btn2;
                }
                if (_disabledImageSourcePath.Length > 0) _btnDisabled = ImageHandler.getImageSource(_disabledImageSourcePath, _imgWidth, _imgHeight);
                NotifyPropertyChanged("U_ImageHeight");
            }
        }

        String _image1SourcePath = "";
        public String U_Image1SourcePath
        {
            set
            {
                _image1SourcePath = value;
                _btn1 = ImageHandler.getImageSource(value, _imgWidth, _imgHeight);
                NotifyPropertyChanged("U_Image1SourcePath");
            }
            get
            {
                return _image1SourcePath;
            }
        }
        String _image2SourcePath = "";
        public String U_Image2SourcePath
        {
            set
            {
                _image2SourcePath = value;
                _btn2 = ImageHandler.getImageSource(value, _imgWidth, _imgHeight);
                NotifyPropertyChanged("U_Image2SourcePath");
            }
            get
            {
                return _image2SourcePath;
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

        int _buttonStyle=0;
        public int U_ButtonStyleIndex
        {
            set
            {
                _buttonStyle = value;
                if (value == 0)
                {
                    Image1.Visibility = System.Windows.Visibility.Visible;
                    Image2.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    Image1.Visibility = System.Windows.Visibility.Hidden;
                    Image2.Visibility = System.Windows.Visibility.Visible;
                }
                NotifyPropertyChanged("U_ButtonStyleIndex");
            }
            get
            {
                return _buttonStyle;
            }
        }

        public Boolean U_Enabled
        {
            set
            {
                this.IsEnabled = value;
                if (_btn1 != null) Image1.Source = (value) ? _btn1 : (_btnDisabled != null) ? _btnDisabled : _btn1;
                if (_btn2 != null) Image2.Source = (value) ? _btn2 : (_btnDisabled != null) ? _btnDisabled : _btn2;
            }
            get
            {
                return this.IsEnabled;
            }
        }
        public void setImage1(String imagePath, int width = 0, int height = 0)
        {
            _btn1 = ImageHandler.getImageSource(imagePath, width, height);
            _image1SourcePath = imagePath;
            _imgWidth = width;
            _imgHeight = height;
            NotifyPropertyChanged("U_Image1SourcePath");
        }
        public void setImage2(String imagePath, int width = 0, int height = 0)
        {
            _btn2 = ImageHandler.getImageSource(imagePath, width, height);
            _image2SourcePath = imagePath;
            _imgWidth = width;
            _imgHeight = height;
            NotifyPropertyChanged("U_Image2SourcePath");
        }

        public void setDisabledImage(String imagePath, int width = 0, int height = 0)
        {
            _btnDisabled = ImageHandler.getImageSource(imagePath, width, height);
            _disabledImageSourcePath = imagePath;
            _imgWidth = width;
            _imgHeight = height;
        }
        public void setImages(String image1Path, String image2Path, String disabledImagePath, int width = 0, int height = 0)
        {
            setImage1(image1Path, width, height);
            setImage2(image2Path, width, height);
            setDisabledImage(disabledImagePath, width, height);
        }
	}
}