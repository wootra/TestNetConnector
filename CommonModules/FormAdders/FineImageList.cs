using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.Common;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Forms.Design;

namespace FormAdders
{
    public class FineImageList:Component, ICollection<Image>
    {
        List<Image> _images = new List<Image>();
        
        public event EventHandler E_ImageChanged;

        public FineImageList()
            : base()
        {
            
        }
        public FineImageList(IContainer parent)
        {
            parent.Add(this);
        }
        protected override void Dispose(bool disposing)
        {

        }
        public Image this[int index]{
            get{ return (Image)_images[index];}
            set{ _images[index] = value;}
        }
        
        //[Editor("System.Windows.Forms.Design.ImageCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        
        public Image[] Images
        {
            get
            {
                Image[] images = new Image[_images.Count];
                for (int i = 0; i < _images.Count; i++)
                {
                    images[i] = _images[i];
                }
                return images;
            }
            set
            {
                //_images.Clear();
                if (_images == null) _images = new List<Image>();
                if (_images.Count < value.Length)
                {
                    _images.Clear();


                    for (int i = 0; i < value.Length; i++)
                    {
                        _images.Add(value[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        _images[i] = value[i];
                    }
                }
                if (E_ImageChanged != null) E_ImageChanged(this, null);
            }
        }
        /*
        public List<Image> ImageList
        {
            get
            {
                return _images;
            }

            set
            {
                _images = value;
            }
        }
        */
        public int Add(Image img)
        {
            _images.Add(img);
            
            return _images.Count;
        }

        public int Count
        {
            get {
                return _images.Count;
            }
            set
            {
                List<Image> images = new List<Image>();
                int min = (_images.Count < value) ? _images.Count : value;
                
                for (int i = 0; i < value; i++)
                {
                    if (i < min) images.Add(_images[i]); //기존 그림을 옮겨넣음..    
                    else images.Add(null);
                }
                _images = images;
            }
        }




        void ICollection<Image>.Add(Image item)
        {
            Add(item);
        }

        public void Clear()
        {
            _images.Clear();

        }

        public bool Contains(Image item)
        {
            return _images.Contains(item);

        }

        public void CopyTo(Image[] array, int arrayIndex)
        {
            _images.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Image item)
        {
            return _images.Remove(item);
        }

        public IEnumerator<Image> GetEnumerator()
        {
            return _images.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _images.GetEnumerator();
        }
    }
}
