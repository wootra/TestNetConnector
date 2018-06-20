using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public partial class ContextListForm : Form
    {
        public ContextListFormCollections Items;
        public ContextListForm()
        {
            InitializeComponent();
            Items = new ContextListFormCollections(this);
            this.Visible = false;
        }

        int _width=10;
        int _height=10;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            float maxWid = _width;
            float th = TextSize("Text",g).Height;
            float margin = 3;
            _height = (int)((th+3)*(Items.Count));
            for (int i = 0; i < Items.Count; i++)
            {
                SizeF size = TextSize(Items.Keys.ElementAt(i),g);
                if(maxWid<size.Width) maxWid = size.Width;
                g.DrawString(Items.Keys.ElementAt(i),_font, Items.Values.ElementAt(i), 0, (th+margin)*i);
            }
            _width = (int)maxWid;
            if (this.Width != _width || this.Height != _height)
            {
                this.SetBounds(0, 0, _width, _height, BoundsSpecified.Size);
            }

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(this.BackColor), this.Bounds);
        }

        internal void Repaint()
        {
            this.SetBounds(0, 0, _width, _height, BoundsSpecified.Size);
            this.Invalidate();
        }

        internal void SizeReset()
        {
            _width = 10;
            _height = 10;
            Repaint();
        }

        Font _font = new Font(SystemFonts.DefaultFont, FontStyle.Regular);
        public new Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                Repaint();
            }
        }

        private SizeF TextSize(String text, Graphics g)
        {
            // Set the return value to the normal node bounds.

            if (text != null)
            {
                return g.MeasureString(text, _font);
            }
            return new SizeF(0, 0);

        }
    }

    public class ContextListFormCollections : IDictionary<String, Brush>
    {
        ContextListForm _parent;
        Dictionary<String, Brush> _dic = new Dictionary<string, Brush>();
        internal ContextListFormCollections(ContextListForm parent)
        {
            _parent = parent;
        }

        public Brush this[String key]
        {
            get
            {
                return _dic[key];
            }
            set
            {
                _dic[key] = value;
                _parent.Repaint();
            }
        }

        public ICollection<string> Keys
        {
            get { return _dic.Keys; }
        }


        public ICollection<Brush> Values
        {
            get
            {
                return _dic.Values;
            }
        }

        public void Add(string key, Brush value)
        {
            _dic.Add(key, value);
            _parent.Repaint();
        }

        public bool ContainsKey(string key)
        {
            return _dic.ContainsKey(key);
        }


        public bool Remove(string key)
        {
            return _dic.Remove(key);
        }

        public bool TryGetValue(string key, out Brush value)
        {
            return _dic.TryGetValue(key, out value);
        }

        
        public void Add(KeyValuePair<string, Brush> item)
        {
            _dic[item.Key] = item.Value;
            _parent.Repaint();
        }

        public void Add(Dictionary<String, Brush> dic)
        {
            foreach (String key in dic.Keys)
            {
                _dic[key] = dic[key];

            }
        }

        public void Clear()
        {
            _dic.Clear();
            _parent.SizeReset();
        }

        public bool Contains(KeyValuePair<string, Brush> item)
        {
            return _dic.Contains(item);
        }

        
        public void CopyTo(KeyValuePair<string, Brush>[] array, int arrayIndex)
        {
            _dic.ToArray().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _dic.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<string, Brush> item)
        {
            bool isRemoved = _dic.Remove(item.Key);
            if (isRemoved) _parent.Repaint();
            return isRemoved;

        }

        public IEnumerator<KeyValuePair<string, Brush>> GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dic.GetEnumerator();
        }


    }
}
