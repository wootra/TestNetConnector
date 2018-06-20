using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using FormAdders.ImageTabControlEnums;

namespace FormAdders.Designer
{
    /// <summary>
    /// Provides a custom <see cref="ControlDesigner"/> for the
    /// <see cref="ImageTabControl"/>.
    /// </summary>
    public class ImageTabControlDesigner : ControlDesigner
    {
        /// <summary>
        /// Creates an instance of the <see cref="ImageTabControlDesigner"/> class.
        /// </summary>
        public ImageTabControlDesigner() {
            
        }

        /// <summary>
        /// Overridden. Inherited from <see cref="ControlDesigner.PreFilterProperties(IDictionary)"/>.
        /// </summary>
        /// <param name="properties"></param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            Attribute[] attr = new Attribute[3];
            attr[0] = new BrowsableAttribute(true);
            attr[1] = new CategoryAttribute("Custom");
            attr[2] = new EditorBrowsableAttribute();
            

            //properties["ActiveTextColor"] = TypeDescriptor.CreateProperty(typeof(ImageTabPageDesigner), (PropertyDescriptor)properties["ActiveTextColor"], attr);
            //properties["InactiveTextColor"] = TypeDescriptor.CreateProperty(typeof(ImageTabPageDesigner), (PropertyDescriptor)properties["InactiveTextColor"], attr);
            
            base.PreFilterProperties(properties);
            
        }
        /*
        public Color ActiveTextColor
        {
            get
            {
                return _tabControl.ActiveTextColor;
            }
            set
            {

                IComponentChangeService iccs = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                if (iccs != null)
                {
                    _tabControl.ActiveTextColor = value;
                }
            }
        }
        public Color InactiveTextColor
        {
            get
            {
                return _tabControl.InactiveTextColor;
            }
            set
            {

                IComponentChangeService iccs = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                if (iccs != null)
                {
                    _tabControl.InactiveTextColor = value;
                }
            }
        }
       */

        /// <summary>
        /// Overridden. Inherited from <see cref="ControlDesigner"/>.
        /// </summary>
        /// <param name="component">
        /// The <see cref="IComponent"/> to which this designer gets attached.
        /// </param>
        /// <remarks>
        /// This designer exists exclusively for <see cref="ImageTabControl"/>s. If
        /// <i>component</i> does not inherit from <see cref="ImageTabControl"/>,
        /// then this method throws an <see cref="ArgumentException"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown if this designer gets used with a class that does not
        /// inherit from <see cref="ImageTabControl"/>.
        /// </exception>
        public override void Initialize(IComponent component)
        {
            _tabControl = component as ImageTabControl;
            _tabControl.ActiveTextColor = Color.FromArgb(0,0,0);// Color.Black;
            _tabControl.InactiveTextColor = Color.Gray;
            if (_tabControl == null)
            {
                this.DisplayError(new ArgumentException("Tried to use the ImageTabControlDesigner with a class that does not inherit from ImageTabControl.", "component"));
                return;
            }
            base.Initialize(component);
            IComponentChangeService iccs = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
            if (iccs != null)
            {
                iccs.ComponentRemoved += new ComponentEventHandler(ComponentRemoved);
            }
        }

        /// <summary>
        /// Overridden. Inherited from <see cref="ControlDesigner"/>.
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (verbs == null)
                {
                    verbs = new DesignerVerbCollection();
                    verbs.Add(new DesignerVerb("Add Tab", new EventHandler(AddTab)));
                    verbs.Add(new DesignerVerb("Remove Tab", new EventHandler(RemoveTab)));
                }
                return verbs;
            }
        }
        
        /// <summary>
        /// Overridden. Inherited from <see cref="ControlDesigner"/>.
        /// </summary>
        /// <param name="m">
        /// The message.
        /// </param>
        /// <remarks>
        /// Checks for WM_LBUTTONDOWN events and uses that to
        /// select the appropriate tab in the <see cref="ImageTabControl"/>.
        /// </remarks>
        protected override void WndProc(ref Message m)
        {
            try
            {
                int x = 0;
                int y = 0;
                if (_tabControl.Created && m.HWnd == _tabControl.Handle)
                {
                    switch (m.Msg)
                    {
                        case WM_LBUTTONDOWN:
                            x = (m.LParam.ToInt32() << 16) >> 16;
                            y = m.LParam.ToInt32() >> 16;
                            int oi = _tabControl.SelectedIndex;
                            ImageTabPage ot = _tabControl.SelectedTab;
                            if (_tabControl.ScrollButtonStyle == YaScrollButtonStyle.Always && _tabControl.GetLeftScrollButtonRect().Contains(x, y))
                            {
                                _tabControl.ScrollTabs(-10);
                            }
                            else if (_tabControl.ScrollButtonStyle == YaScrollButtonStyle.Always && _tabControl.GetRightScrollButtonRect().Contains(x, y))
                            {
                                _tabControl.ScrollTabs(10);
                            }
                            else
                            {
                                
                                for (int i = 0; i < (_tabControl.Controls as ImageTabControl.ControlCollection).Count; i++)
                                {
                                    Rectangle r = _tabControl.GetTabRect(_tabControl.TabPages[i] as ImageTabPage);
                                    if (r.Contains(x, y))
                                    {
                                        _tabControl.SelectedIndex = i;
                                        RaiseComponentChanging(TypeDescriptor.GetProperties(Control)["SelectedIndex"]);
                                        RaiseComponentChanged(TypeDescriptor.GetProperties(Control)["SelectedIndex"], oi, i);
                                        break;
                                    }
                                    
                                }
                                
                            }
                            break;
                        case WM_LBUTTONDBLCLK:
                            x = (m.LParam.ToInt32() << 16) >> 16;
                            y = m.LParam.ToInt32() >> 16;
                            if (_tabControl.ScrollButtonStyle == YaScrollButtonStyle.Always && _tabControl.GetLeftScrollButtonRect().Contains(x, y))
                            {
                                _tabControl.ScrollTabs(-10);
                            }
                            else if (_tabControl.ScrollButtonStyle == YaScrollButtonStyle.Always && _tabControl.GetRightScrollButtonRect().Contains(x, y))
                            {
                                _tabControl.ScrollTabs(10);
                            }
                            return;
                    }
                }
            }
            finally
            {
                base.WndProc(ref m);
            }
        }
        
        /// <summary>
        /// Overridden. Inherited from <see cref="IDesigner.DoDefaultAction()"/>.
        /// </summary>
        public override void DoDefaultAction() { }

        /// <summary>
        /// Id for the WM_LBUTTONDOWN message.
        /// </summary>
        private const int WM_LBUTTONDOWN = 0x0201;

        /// <summary>
        /// Id for the WM_LBUTTONDBLCLICK message.
        /// </summary>
        private const int WM_LBUTTONDBLCLK = 0x0203;

        /// <summary>
        /// Watches for the removal of <see cref="YaTabDrawer"/>s and, should
        /// one get removed that is assigned to the <see cref="ImageTabControl"/>,
        /// then set the <see cref="ImageTabControl.TabDrawer"/> property to <b>null</b>.
        /// </summary>
        /// <param name="sender">
        /// The object that send this event.
        /// </param>
        /// <param name="cea">
        /// Some <see cref="ComponentEventArgs"/>.
        /// </param>
        private void ComponentRemoved(object sender, ComponentEventArgs cea)
        {
            /*
            if (cea.Component == _tabControl.TabDrawer)
            {
                _tabControl.TabDrawer = null;
                RaiseComponentChanging(TypeDescriptor.GetProperties(Control)["TabDrawer"]);
                RaiseComponentChanged(TypeDescriptor.GetProperties(Control)["TabDrawer"], cea.Component, null);
            }
             */
        }

        /// <summary>
        /// Event handler for the "Add Tab" verb.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="ea">
        /// Some <see cref="EventArgs"/>.
        /// </param>
        private void AddTab(object sender, EventArgs ea)
        {
            IDesignerHost dh = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (dh != null)
            {
                int selTab = _tabControl.SelectedIndex;
                string name = GetNewTabName();
                ImageTabPage itp = dh.CreateComponent(typeof(ImageTabPage), name) as ImageTabPage;
                
                itp._index = _tabControl.TabPages.Count;// (_tabControl.Controls as ImageTabControl.ControlCollection).Count;
                itp.Text = "Tab" + itp._index;
                
                _tabControl.TabPages.Add(itp);
                
                //_tabControl.Controls.Add(itp);
                //ytp.Name = _tabControl.Name + "_Tab" + _tabControl.Controls.Count;
                //ytp.Text = "Tab"+_tabControl.TabPages.Count;
                //(_tabControl.Controls as ImageTabControl.ControlCollection).Add(itp);
                _tabControl.CalculateTabLengths();
                if (_tabControl.SelectedIndex < 0)
                {
                    _tabControl.SelectedIndex = itp.Index;
                   
                    //MessageBox.Show("index:" + ytp.Index);
                    RaiseComponentChanging(TypeDescriptor.GetProperties(Control)["SelectedIndex"]);
                    RaiseComponentChanged(TypeDescriptor.GetProperties(Control)["SelectedIndex"], selTab, _tabControl.SelectedIndex);
                }
                dh.Activate();
            }
            
        }

        /// <summary>
        /// Event handler for the "Remove Tab" verb.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="ea">
        /// Some <see cref="EventArgs"/>.
        /// </param>
        private void RemoveTab(object sender, EventArgs ea)
        {
            IDesignerHost dh = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (dh != null)
            {
                int i = _tabControl.SelectedIndex;
                if (i > -1)
                {
                    ImageTabPage ytp = _tabControl.SelectedTab;
                    (_tabControl.Controls as ImageTabControl.ControlCollection).Remove(ytp);
                    dh.DestroyComponent(ytp);
                    RaiseComponentChanging(TypeDescriptor.GetProperties(Control)["SelectedIndex"]);
                    RaiseComponentChanged(TypeDescriptor.GetProperties(Control)["SelectedIndex"], i, 0);
                }
            }
        }

        /// <summary>
        /// Gets a new tab name for the a tab.
        /// </summary>
        /// <returns></returns>
        private string GetNewTabName()
        {
            int i = 1;

            Hashtable h = new Hashtable(_tabControl.TabPages.Count);
            foreach (ImageTabPage c in _tabControl.TabPages)
            {
                h[c.Name] = null;
            }
            while (h.ContainsKey(_tabControl.Name + "_tabPage" + i))
            {
                i++;
            }
            return _tabControl.Name + "_tabPage" + i;
        }

        /// <summary>
        /// Contains the verbs used to modify the <see cref="ImageTabControl"/>.
        /// </summary>
        private DesignerVerbCollection verbs;

        /// <summary>
        /// Contains a cast reference to the <see cref="ImageTabControl"/> that
        /// this designer handles.
        /// </summary>
        private ImageTabControl _tabControl;
    }
}
