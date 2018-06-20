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

namespace RtwWpfControls
{
    /// <summary>
    /// RtwLabel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RtwLabel : UserControl, INotifyPropertyChanged, IWpfControls
    {
        bool _isChecking = false;

        public event PropertyChangedEventHandler PropertyChanged;
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

        public EventHelper Events { get; set; }

        public RtwLabel()
        {
            InitializeComponent();
            Events = new EventHelper(this, this);
        }

        String _text = "label1";
        public String Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                label1.Content = value;
                NotifyPropertyChanged("Text");
            }
        }

        public Color FontColor
        {
            get
            {
                return (this.Foreground as SolidColorBrush).Color;
            }
            set
            {
                this.Foreground = new SolidColorBrush(value);
            }
        }
    }
}
