using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using FormAdders; using DataHandling;
using System.Drawing;

namespace FormAdders
{
    public partial class ImageTabControl
    {

        #region Private Inner Classes

        /// <summary>
        /// Let's the tabs scroll.
        /// </summary>
        private class ScrollerThread
        {
            /// <summary>
            /// Creates a new instance of the
            /// <see cref="ImageTabControl.ScrollerThread"/> class.
            /// </summary>
            /// <param name="amount">The amount to scroll.</param>
            /// <param name="control">The control to scroll.</param>
            public ScrollerThread(int amount, ImageTabControl control)
            {
                this.tabControl = control;
                this.amount = new object[] { amount };
                scroller = new ScrollTabsDelegate(tabControl.ScrollTabs);
            }

            /// <summary>
            /// Scrolls the tabs on the <see cref="ImageTabControl"/>
            /// by the given amount.
            /// </summary>
            public void ScrollIt()
            {
                bool keepScrolling = false;
                lock (tabControl)
                {
                    keepScrolling = tabControl.yaKeepScrolling;
                }
                while (keepScrolling)
                {

                    tabControl.Invoke(scroller, amount);
                    lock (tabControl)
                    {
                        keepScrolling = tabControl.yaKeepScrolling;
                    }
                }
            }

            /// <summary>
            /// The control to scroll.
            /// </summary>
            private ImageTabControl tabControl;

            /// <summary>
            /// The amount to scroll.
            /// </summary>
            private object[] amount;

            /// <summary>
            /// A delegate to scroll the tabs.
            /// </summary>
            private ScrollTabsDelegate scroller;

            /// <summary>
            /// A delegate to use in scrolling the tabs.
            /// </summary>
            private delegate void ScrollTabsDelegate(int amount);
        }

        #endregion

        #region Public Inner Classes

        /// <summary>
        /// A <see cref="ImageTabControl"/>-specific
        /// <see cref="Control.ControlCollection"/>.
        /// </summary>
        public new class ControlCollection : Control.ControlCollection
        {
            public List<ImageTabPage> TabPages;
            /// <summary>
            /// Creates a new instance of the
            /// <see cref="ImageTabControl.ControlCollection"/> class with 
            /// the specified <i>owner</i>.
            /// </summary>
            /// <param name="owner">
            /// The <see cref="ImageTabControl"/> that owns this collection.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <i>owner</i> is <b>null</b>.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown if <i>owner</i> is not a <see cref="ImageTabControl"/>.
            /// </exception>
            public ControlCollection(Control owner)
                : base(owner)
            {
                TabPages = new List<ImageTabPage>();
                if (owner == null)
                {
                    throw new ArgumentNullException("owner", "Tried to create a ImageTabControl.ControlCollection with a null owner.");
                }
                this.owner = owner as ImageTabControl;
                if (this.owner == null)
                {
                    throw new ArgumentException("Tried to create a ImageTabControl.ControlCollection with a non-ImageTabControl owner.", "owner");
                }
                monitor = new EventHandler( this.owner.ChildTabTextChanged );
            }

            /// <summary>
            /// Overridden. Adds a <see cref="Control"/> to the
            /// <see cref="ImageTabControl"/>.
            /// </summary>
            /// <param name="value">
            /// The <see cref="Control"/> to add, which must be a
            /// <see cref="YaTabPage"/>.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <i>value</i> is <b>null</b>.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown if <i>value</i> is not a <see cref="YaTabPage"/>.
            /// </exception>
            public override void Add(Control value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Tried to add a null value to the ImageTabControl.ControlCollection.");
                }
                ImageTabPage p = value as ImageTabPage;
                if (p == null)
                {
                    throw new ArgumentException("Tried to add a non-YaTabPage control to the ImageTabControl.ControlCollection.", "value");
                }
                TabPages.Add(p);
                p.SendToBack();
                p._index = base.Count;
                if (p.Text == null || p.Text.Length == 0) p.Text = "Tab" + p._index;
                //p.Dock = DockStyle.Fill;

                owner.SetTabSize(ref p);
                base.Add(p);
                if (owner.Parent != null)
                {
                    Rectangle bounds = owner.GetTabRect(p);
                    p.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);

                    owner.InU();
                }
                p.TextChanged += monitor;
            }

            /// <summary>
            /// Overridden. Inherited from <see cref="Control.ControlCollection.Remove( Control )"/>.
            /// </summary>
            /// <param name="value"></param>
            public override void Remove(Control value)
            {
                value.TextChanged -= monitor;
                
                ImageTabPage pageToRemove = null;
                foreach (ImageTabPage page in TabPages)
                {

                    if (page.Name.Equals(value.Name)) pageToRemove = page;
                }

                if (pageToRemove != null)
                {
                    base.Remove(pageToRemove);
                    TabPages.Remove(pageToRemove);
                }
            }

            /// <summary>
            /// Overridden. Inherited from <see cref="Control.ControlCollection.Clear()"/>.
            /// </summary>
            public override void Clear()
            {
                foreach (Control c in this)
                {
                    c.TextChanged -= monitor;
                }

                foreach (ImageTabPage page in TabPages)
                {
                    base.Remove(page);
                }

                //base.Clear();
                TabPages.Clear();
            }

            /// <summary>
            /// The owner of this <see cref="ImageTabControl.ControlCollection"/>.
            /// </summary>
            private ImageTabControl owner;
            /*
            public new Control this[int index]
            {
                get
                {
                    if (index < 0) return null;
                    else return this[index];
                }
            }
            
            public override int Count
            {
                get
                {
                    return owner._tabPanel.Controls.Count;// base.Count;
                }
            }
            public new Control this[String name]
            {
                get
                {
                    if (owner._tabPanel.Controls.ContainsKey(name)) return owner._tabPanel.Controls[name];
                    else return null;
                }
            }

            */
            private EventHandler monitor;
        }

        #endregion
    }

}
