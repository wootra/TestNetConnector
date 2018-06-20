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

namespace Buttons
{
	/// <summary>
	/// Interaction logic for BlueRect.xaml
	/// </summary>
	public partial class BlueRect : UserControl, IWpfControls
	{
		public BlueRect()
		{
			this.InitializeComponent();
            _events = new EventHelper(this, this);
		}

        EventHelper _events;
        public EventHelper Events { get { return _events; } }
        public String Text { get { return this.Content as String; } set { this.Content = value; } }
	}
}