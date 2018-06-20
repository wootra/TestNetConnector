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
using System.ComponentModel;
using WPF_Handler;

namespace Buttons
{
	/// <summary>
	/// Interaction logic for RtwLedButton.xaml
	/// </summary>
	public partial class RtwLedButton : UserControl, INotifyPropertyChanged, IWpfControls
	{
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

		public RtwLedButton()
		{
			this.InitializeComponent();
            U_LedSize = 20;
            U_LedPosition = new Thickness(10,10,0,0);
            U_FontColor = Colors.Black;
            U_Enabled = true;
            _events = new EventHelper(this, this);
        }

        EventHelper _events;
        public EventHelper Events { get { return _events; } }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public enum LedState { Green = 0, Red, Off };
        LedState _ledState = LedState.Off;

        public LedState U_LedState
        {
            get
            {
                return _ledState;
            }
            set
            {
                _ledState = value;
                Dispatcher.Invoke(new Action(delegate
                {
                    if (_ledState == LedState.Green)
                    {
                        LedGreen.Visibility = System.Windows.Visibility.Visible;
                        LedRed.Visibility = System.Windows.Visibility.Hidden;
                        LedOff.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else if (_ledState == LedState.Red)
                    {
                        LedGreen.Visibility = System.Windows.Visibility.Hidden;
                        LedRed.Visibility = System.Windows.Visibility.Visible;
                        LedOff.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        LedGreen.Visibility = System.Windows.Visibility.Hidden;
                        LedRed.Visibility = System.Windows.Visibility.Hidden;
                        LedOff.Visibility = System.Windows.Visibility.Visible;
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal); 
            }
        }

        public System.Windows.Visibility U_GreenOn
        {
            //get { return Back.Visibility; }
            set
            {
               // Back.Visibility = value;
                NotifyPropertyChanged("U_GreenOn");
            }
        }
        public System.Windows.Visibility U_RedOn
        {
            //get { return Back.Visibility; }
            set
            {
                //Back.Visibility = value;
                NotifyPropertyChanged("U_RedOn");
            }
        }
        public System.Windows.Visibility U_Off
        {
            //get { return Back.Visibility; }
            set
            {
                //Back.Visibility = value;
                NotifyPropertyChanged("U_Off");
            }
        }

        LedState _savedState = LedState.Off;
        Brush _savedForeColor = Brushes.Black;
        bool _enabled = true;
        public bool U_Enabled
        {
            get
            {
                return _enabled; ;
            }
            set
            {
                _enabled = value;
                Dispatcher.Invoke(new Action(delegate
                {
                    if (value)
                    {
                        BackDisabled = System.Windows.Visibility.Hidden;
                        BackEnabled = System.Windows.Visibility.Visible;
                        U_LedState = _savedState;
                        U_FontBrush = _savedForeColor;
                    }
                    else
                    {
                        BackDisabled = System.Windows.Visibility.Visible;
                        BackEnabled = System.Windows.Visibility.Hidden;
                        _savedState = U_LedState;
                        _savedForeColor = U_FontBrush;
                        U_LedState = LedState.Off;
                        U_FontBrush = Brushes.LightGray;
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        public bool U_LedVisibles
        {
            get
            {
                return (_ledVisible == System.Windows.Visibility.Visible);
            }
            set
            {
                if (value) U_LedVisibility = Visibility.Visible;
                else U_LedVisibility = System.Windows.Visibility.Hidden;
            }
        }

        Visibility _ledVisible = Visibility.Visible;
        public Visibility U_LedVisibility
        {
            get
            {
                return _ledVisible;
            }
            set
            {
                _ledVisible = value;
                NotifyPropertyChanged("U_LedVisibility");
            }
        }
        Brush _fontColor = Brushes.Black;
        public Brush U_FontBrush
        {
            get
            {
                return _fontColor;
            }
            set
            {
                _fontColor = value;
                NotifyPropertyChanged("U_FontBrush");
            }
        }

        public Color U_FontColor
        {
            get
            {
                SolidColorBrush brush = _fontColor as SolidColorBrush;
                return brush.Color;
            }
            set
            {
                U_FontBrush = new SolidColorBrush(value);
            }
        }

        internal System.Windows.Visibility BackEnabled
        {
            get { return Back.Visibility; }
            set {
                Dispatcher.Invoke(new Action(delegate
                {
                    Back.Visibility = value;
                }), System.Windows.Threading.DispatcherPriority.Normal);
                
                NotifyPropertyChanged("BackEnabled");
            }
        }

        internal System.Windows.Visibility BackDisabled
        {
            get { return Back.Visibility; }
            set
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    Back_Disabled.Visibility = value;
                }), System.Windows.Threading.DispatcherPriority.Normal);
                NotifyPropertyChanged("BackDisabled");
            }
        }

        Thickness _pos = new Thickness(5, 5, 0, 0);
        public Thickness U_LedPosition
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                NotifyPropertyChanged("U_LedPosition");
            }
        }

        public double U_LedSize
        {
            get
            {
                return U_LedWidth;
            }
            set
            {
                U_LedWidth = value;
                U_LedHeight = value;
            }
        }

        double _ledWidth = 15;
        public double U_LedWidth
        {
            get
            {
                return _ledWidth;
            }
            set
            {
                _ledWidth = value;
                NotifyPropertyChanged("U_LedWidth");
            }
        }

        double _ledHeight = 15;
        public double U_LedHeight
        {
            get
            {
                return _ledHeight;
            }
            set
            {
                _ledHeight = value;
                NotifyPropertyChanged("U_LedHeight");
            }
        }

        String _text = "label";
        public String Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }
	}
}