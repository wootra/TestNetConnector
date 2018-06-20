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
using System.Timers;
using System.Windows.Threading;

namespace ListViews
{
    [Browsable(true)]
    public class ListRowClass : UserControl
    {
        StackPanel panel = new StackPanel();
        List<UIElement> _items = new List<UIElement>();
        List<double> _wid;
        HorizontalAlignment ItemHorizontalAlignment { get; set; }
        Rectangle _seperator = null;


        public ListRowClass()
            : base()
        {
            init((new double[] { }).ToList(), 30, true);
        }

        public ListRowClass(List<double> width, double height, Boolean wrap = false, Rectangle sep = null, params UIElement[] items)
            : base()
        {
            setSeperator(sep);
            init(width, height, wrap);
            AddItems(wrap, items);
        }

        public ListRowClass(List<double> width, double height, Boolean wrap = false, params UIElement[] items)
            : base()
        {
            init(width, height, wrap);
            AddItems(wrap, items);
        }

        public ListRowClass(List<double> width, double height, Boolean wrap = false, List<UIElement> items = null)
            : base()
        {
            init(width, height, wrap);
            AddItems(wrap, items);
        }

        public ListRowClass(List<double> width, double height, Boolean wrap = false, Rectangle sep = null, List<UIElement> items = null)
            : base()
        {
            setSeperator(sep);
            init(width, height, wrap);
            AddItems(wrap, items);
        }

        public void setSeperator(Rectangle rect)
        {
            _seperator = rect;
        }


        public void init(List<double> width, double height, Boolean wrap = false)
        {
            panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            panel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            panel.Margin = new Thickness(0, 0, 0, 0);
            panel.Orientation = Orientation.Horizontal;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.Height = height;
            this._wid = width;

            //this.Children.Add(panel);
            //this.AddLogicalChild(panel);
            this.Content = panel;
            //this.AddVisualChild(panel);
            ItemHorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        }

        public void AddItems(Boolean wrap = false, List<UIElement> items = null)
        {
            if (items == null) return;

            UIElement element;

            for (int i = 0; i < items.Count; i++)
            {
                element = items[i];

                addAnItem(wrap, element, (_wid.Count > i) ? _wid[i] : (double)(element.GetValue(WidthProperty)));
            }
        }

        public void AddItems(Boolean wrap = false, params UIElement[] items)
        {
            if (items == null) return;

            UIElement element;

            for (int i = 0; i < items.Length; i++)
            {
                element = items[i];
                addAnItem(wrap, element, (_wid.Count > i) ? _wid[i] : (double)(element.GetValue(WidthProperty)));
            }
        }

        public void addAnItem(Boolean wrap, UIElement element, double width)
        {
            Grid col;

            if (wrap == true)
            {
                element.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                element.SetValue(HorizontalAlignmentProperty, ItemHorizontalAlignment);

                col = new Grid();
                col.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                col.Width = width;
                panel.Children.Add(col); //StackPanel에 들어가는 것은 실제로는 Grid이다.
                col.Children.Add(element);

                Rectangle rect;
                if (_seperator == null)
                {
                    rect = new Rectangle();
                    rect.Fill = Brushes.LightGray;
                    rect.Width = 1;
                }
                else
                {
                    rect = new Rectangle();
                    rect.Style = _seperator.Style;
                }

                rect.Height = this.Height - 6;
                rect.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                rect.Margin = new Thickness(col.Width - 3, 0, 0, 0);

                col.Children.Add(rect);
                _items.Add(element);//관리되는 item은 element이다.
            }
            else
            {
                _items.Add(element);
                panel.Children.Add(element);
            }
        }

        public UIElement this[int num]
        {
            set
            {
                _items[num] = value;
                panel.Children[num] = value;
            }
            get
            {
                return _items[num];
                //return Children[num];
            }
        }

        public int Count { get { return _items.Count; } }

        public int IndexOf(UIElement item)
        {
            return _items.IndexOf(item);
        }

        public void setRealItem(int id, UIElement item)
        {
            _items[id] = item;
        }


    }
}
