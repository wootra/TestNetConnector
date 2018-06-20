using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using FormAdders.ImageTabControlEnums;
using System.ComponentModel;
using System;
using System.Data;
using System.Collections;

namespace FormAdders.Designer
{
    /// <summary>
    /// Summary description for ImageTabPageDesigner.
    /// </summary>
    public class ImageTabPageDesigner : ScrollableControlDesigner
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ImageTabPageDesigner"/> class.
        /// </summary>
        public ImageTabPageDesigner() {
            
        }
        
        /// <summary>
        /// Shadows the <see cref="ImageTabPage.Text"/> property.
        /// </summary>
        public string Text
        {
            get
            {
                return _tabPage.Text;
            }
            set
            {
                string ot = _tabPage.Text;
                _tabPage.Text = value;
                IComponentChangeService iccs = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                if (iccs != null)
                {
                    ImageTabControl ytc = _tabPage.Parent as ImageTabControl;
                    if (ytc != null)
                    {
                        ytc.SelectedIndex = ytc.SelectedIndex;
                    }
                }
            }
        }
        
        /// <summary>
        /// Overridden. Inherited from
        /// <see cref="ControlDesigner.OnPaintAdornments(PaintEventArgs)"/>.
        /// </summary>
        /// <param name="pea">
        /// Some <see cref="PaintEventArgs"/>.
        /// </param>
        protected override void OnPaintAdornments(PaintEventArgs pea)
        {
            base.OnPaintAdornments(pea);

            // My thanks to bschurter (Bruce), CodeProject member #1255339 for this!
            using (Pen p = new Pen(SystemColors.ControlDark, 1))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                
                pea.Graphics.DrawRectangle(p, 0, 0, _tabPage.Width - 1, _tabPage.Height - 1);
                
                
            }
        }

        /// <summary>
        /// Overridden. Inherited from <see cref="ControlDesigner.Initialize( IComponent )"/>.
        /// </summary>
        /// <param name="component">
        /// The <see cref="IComponent"/> hosted by the designer.
        /// </param>
        public override void Initialize(IComponent component)
        {
            _tabPage = component as ImageTabPage;
            
            if (_tabPage == null)
            {
                DisplayError(new Exception("You attempted to use a ImageTabPageDesigner with a class that does not inherit from ImageTabPage."));
            }
            base.Initialize(component);
        }

        ImageTabControl TabControl
        {
            get { return _tabPage.Parent as ImageTabControl; }
        }

        /// <summary>
        /// Overridden. Inherited from <see cref="ControlDesigner.PreFilterProperties(IDictionary)"/>.
        /// </summary>
        /// <param name="properties"></param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            properties["Text"] = TypeDescriptor.CreateProperty(typeof(ImageTabPageDesigner), (PropertyDescriptor)properties["Text"], new Attribute[0]);
        }

        /// <summary>
        /// The <see cref="ImageTabPage"/> hosted by the designer.
        /// </summary>
        private ImageTabPage _tabPage;
    }
}
