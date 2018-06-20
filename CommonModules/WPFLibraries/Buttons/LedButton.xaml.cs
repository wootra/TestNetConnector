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
using System.Windows.Media.Animation;
using WPF_Handler;
using System.ComponentModel;

namespace Buttons
{
	/// <summary>
	/// Interaction logic for MainControl.xaml
	/// </summary>
	public partial class LedButton : IWpfControls, INotifyPropertyChanged
	{
		public LedButton()
		{
			this.InitializeComponent();
            _events = new EventHelper(this, this);
        }

        EventHelper _events;
        public EventHelper Events { get { return _events; } }
        public String Text { get { return this.Content as String; } set { this.Content = value; } }

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


        Boolean _selected=false;
        public Boolean Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                if (_selected)
                {
                    this.ContentTemplate = this.FindResource("LabelInBtn1") as DataTemplate;
                    
                }
                else
                {
                    this.ContentTemplate = this.FindResource("LabelInBtn") as DataTemplate;
                }
            }

        }
	}
}