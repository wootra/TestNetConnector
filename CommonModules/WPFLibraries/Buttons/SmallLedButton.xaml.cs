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
using System.ComponentModel;
using WPF_Handler;

namespace Buttons
{
	/// <summary>
	/// Interaction logic for MainControl.xaml
	/// </summary>
	public partial class SmallLedButton :INotifyPropertyChanged, IWpfControls
	{
        bool _isChecking = false;

        public SmallLedButton()
		{
			this.InitializeComponent();
            U_Selected = false;
            _events = new EventHelper(this, this);
        }

        EventHelper _events;
        public EventHelper Events { get { return _events; } }
        public String Text { get { return this.Content as String; } set { this.Content = value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        

        Boolean _selected=false;
        public Boolean U_Selected
        {
            get
            {
                if (_selected) return true;
                else return false;
            }
            set
            {
                if (value.Equals(true))
                {
                    _selected = true;
                    AllowDrop = true;
                }
                else
                {
                    _selected = false;
                    AllowDrop = false;
                }
                
                NotifyPropertyChanged("Selected");
            }

        }

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
	}
}