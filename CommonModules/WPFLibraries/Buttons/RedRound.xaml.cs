﻿using System;
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
	public partial class RedRound
	{
        public RedRound()
		{
			this.InitializeComponent();
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
	}
}