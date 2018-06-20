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

namespace RtwWpfControls
{
    public class ResourceHandler
    {
        ResourceDictionary _rDic;
        Dictionary<Type, Object> _rName = new Dictionary<Type, object>();
        public ResourceHandler(ResourceDictionary rd)
        {
            _rDic = rd;
        }
        public void setResourceDictionary(ResourceDictionary rd)
        {
            _rDic = rd;
        }
        public void setControl(Type uiType, Object ResourceName)
        {
            if (_rName.Keys.Contains(uiType)) _rName[uiType] = ResourceName;
            else _rName.Add(uiType, ResourceName);
        }
        public UIElement SetStyle(UIElement obj)
        {
            
            foreach (Type type in _rName.Keys)
            {
                if (type.Equals(obj.GetType()))
                {
                    if( obj is Shape) (obj as Shape).Style = _rDic[_rName[type]] as Style;
                    else if (obj is Control) (obj as Control).Style = _rDic[_rName[type]] as Style;
                    else
                    {
                        throw new NotSupportedException();
                    }
                    return obj;
                }
            }
            return obj;
        }

    }
}
