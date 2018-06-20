using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

namespace TabStripApp {
    class TabPageSwitcherDesigner : ParentControlDesigner {
        private DesignerVerbCollection verbs;
        private ISelectionService selectionService;

        public TabPageSwitcher ControlSwitcher  {
            get { return Component as TabPageSwitcher; }
        }

        internal ISelectionService SelectionService {
            get {
                if (selectionService == null) {
                    selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                    Debug.Assert(selectionService != null, "Failed to get Selection Service!");
                }
                return selectionService;
            }
        }
        public override System.ComponentModel.Design.DesignerVerbCollection Verbs {
            get {
                if (verbs == null) {

                    verbs = new DesignerVerbCollection();
                    verbs.Add(new DesignerVerb(Properties.Resources.AddTab, new EventHandler(this.OnAdd)));         
                }

                return verbs;
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                SelectionService.SelectionChanged -= new EventHandler(SelectionService_SelectionChanged);
            }
        }
        public override void Initialize(IComponent component) {
            base.Initialize(component);
            SelectionService.SelectionChanged += new EventHandler(SelectionService_SelectionChanged);
        }

        private void OnAdd(object sender, EventArgs eevent) {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null) {

                DesignerTransaction t = null;
                try {
                    try {
                        t = host.CreateTransaction(Properties.Resources.AddTab + Component.Site.Name);
                    }
                    catch (CheckoutException ex) {
                        if (ex == CheckoutException.Canceled) {
                            return;
                        }
                        throw ex;
                    }
                    MemberDescriptor member = TypeDescriptor.GetProperties(ControlSwitcher)["Controls"];
                    TabStripPage page = host.CreateComponent(typeof(TabStripPage)) as TabStripPage;
                    RaiseComponentChanging(member);
                   
                    
                    ControlSwitcher.Controls.Add(page);
                    SetProperty("SelectedControl", page);                    
                    RaiseComponentChanged(member, null, null);

                    if (ControlSwitcher.TabStrip != null) {

                        // add a tab to the toolstrip designer
                        MemberDescriptor itemsProp = TypeDescriptor.GetProperties(ControlSwitcher.TabStrip)["Items"];

                        Tab tab = host.CreateComponent(typeof(Tab)) as Tab;
                        RaiseComponentChanging(itemsProp);

                        ControlSwitcher.TabStrip.Items.Add(tab);
                        RaiseComponentChanged(itemsProp, null, null);
                        
                        SetProperty(tab, "DisplayStyle", ToolStripItemDisplayStyle.ImageAndText);
                        SetProperty(tab, "Text", tab.Name);
                        SetProperty(tab, "TabStripPage", page);
                        SetProperty(ControlSwitcher.TabStrip, "SelectedTab", tab);
                    }
                    
                }
                finally {
                    if (t != null)
                        t.Commit();
                }
            }
         
        }

      
      
        void SelectionService_SelectionChanged(object sender, EventArgs e) {
            IList selectedComponents = (IList)SelectionService.GetSelectedComponents();
            if (selectedComponents.Count == 1) {
                Tab tab = selectedComponents[0] as Tab;
                if (tab != null) {
                    SetProperty("SelectedControl", tab.TabStripPage);
                    SetProperty(tab, "Checked", true);
                }
            }
        }
        private void SetProperty(object target, string propname, object value) {
            PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(target)[propname];
            if (propDescriptor != null) {
                propDescriptor.SetValue(target, value);
            }
        }
 
        private void SetProperty(string propname, object value) {
            SetProperty(ControlSwitcher, propname, value);
        }
    }
}
