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


namespace WPF_Handler
{
    public class WpfFinder
    {

        public static DependencyObject getParentFromTemplate(Object source, DependencyObjectType parentType, DependencyObjectType findStopType=null)
        {

            DependencyObject current = source as DependencyObject;
            DependencyObject result = source as DependencyObject;

            while (current != null)
            {
                result = current;

                if (current is Visual)
                {
                    current = VisualTreeHelper.GetParent(current);

                }
                else
                {
                    // If we're in Logical Land then we must walk 
                    // up the logical tree until we find a 
                    // Visual/Visual3D to get us back to Visual Land.
                    current = LogicalTreeHelper.GetParent(current);
                }
                if (current != null)
                {
                    if (current.DependencyObjectType.IsSubclassOf(parentType)) return current;
                    if (findStopType != null && current.DependencyObjectType.IsSubclassOf(findStopType)) return null;
                }

            }
            return current;
        }
        public static DependencyObject getVisualChildFromTemplate(Object source, DependencyObjectType childType, String name=null)
        {
            DependencyObject current = source as DependencyObject;
            
            for(int i=0; i<VisualTreeHelper.GetChildrenCount(current); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(current, i);

                if (child != null)
                {
                    if(name!=null){
                        if(child is FrameworkElement){
                            if((child as FrameworkElement).Name.Equals(name)){
                                if (child.DependencyObjectType.IsSubclassOf(childType)) return child;
                            }
                        }
                    }else{
                        if (child.DependencyObjectType.IsSubclassOf(childType) || child.DependencyObjectType.Equals(childType)) return child;
                        else
                        {
                            child = getVisualChildFromTemplate(child, childType, name);
                            if (child != null) return child;
                        }
                    }
                }
                
            }
            return null;
        }

        public static UIElement getUiElement(Object obj,HorizontalAlignment horizonAlign= HorizontalAlignment.Left, VerticalAlignment verticalAlign = VerticalAlignment.Center)
        {
            UIElement element;

            if (obj is Boolean)
            {
                CheckBox cb = new CheckBox();
                cb.IsChecked = (Boolean)obj;
                element = cb;
            }else if (obj is String)
            {
                TextBlock tb = new TextBlock();
                tb.Text = obj as String;
                tb.HorizontalAlignment = horizonAlign;
                tb.VerticalAlignment = verticalAlign;
                tb.Padding = new Thickness(2, 0, 2, 0);
                element = tb;
                
            }
            else if (obj is List<String>)
            {
                List<String> list = obj as List<String>;
                ComboBox cb = new ComboBox();
                cb.HorizontalAlignment = horizonAlign;
                cb.VerticalAlignment = verticalAlign;
                for (int i = 0; i < list.Count; i++)
                {
                    cb.Items.Add(list[i]);
                }
                element = cb;
            }
            else if (obj is FrameworkElement)
            {
                FrameworkElement cb = obj as FrameworkElement;
                
                cb.HorizontalAlignment = horizonAlign;
                cb.VerticalAlignment = verticalAlign;
                element = cb;
            }
            else
            {
                if (obj is UIElement)
                {
                    element = obj as UIElement;
                }
                else
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = obj.ToString();
                    element = tb;
                }

            }
            return element;
        }
    }

}
