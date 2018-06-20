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
	public partial class RtwPlainButton : UserControl, INotifyPropertyChanged, IWpfControls
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

        public RtwPlainButton()
		{
			this.InitializeComponent();

            U_FontColor = Colors.Black;
            U_Enabled = true;
            _events = new EventHelper(this, this);
        }

        EventHelper _events;
        public EventHelper Events { get { return _events; } }
        
        public event PropertyChangedEventHandler PropertyChanged;


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
                        U_FontBrush = _savedForeColor;
                    }
                    else
                    {
                        BackDisabled = System.Windows.Visibility.Visible;
                        BackEnabled = System.Windows.Visibility.Hidden;
                        _savedForeColor = U_FontBrush;
                        U_FontBrush = Brushes.LightGray;
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);
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