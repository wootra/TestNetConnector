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
    /// RtwCloseButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RtwCloseButton : UserControl, INotifyPropertyChanged
    {
        #region property changed
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
        #endregion

        public EventHelper Events;
        public int RowIndex;
        public int ColIndex;
        public object Parent;

        public RtwCloseButton(int row, int col, object parent)
        {
            InitializeComponent();
            Events = new EventHelper(this, this);
            RowIndex = row;
            ColIndex = col;
            Parent = parent;
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Size size = sizeInfo.NewSize;

            if (size.Height > size.Width)
            {
                double margin = (size.Height - size.Width) / 2;
                
                this.XMark.Margin = new Thickness(0, margin, 0, margin);
                XSize = size.Width / 10;
            }
            else
            {
                double margin = (size.Width - size.Height) / 2;
                this.XMark.Margin = new Thickness(margin, 0, margin, 0);
                XSize = size.Height / 10;
            }
        }

        double _xSize=50;
        public double XSize
        {
            get
            {
                return _xSize;
            }
            set
            {
                _xSize = value;
                NotifyPropertyChanged("XSize");
            }
        }
    }
}
