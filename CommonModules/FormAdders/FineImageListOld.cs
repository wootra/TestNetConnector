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
    public class FineImageListOld:Component
    {
        ImageCollection _images = new ImageCollection();
        public FineImageListOld()
            : base()
        {

        }
        public FineImageListOld(IContainer parent)
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

        public List<Image> ImageList
        {
            get
            {
                return _images.getImageList();
            }
            set
            {
                _images.Clear();
                _images.AddRange(value);
            }
        }

                

        public int Count
        {
            get { return _images.Count; }
        }

        public ImageCollection Images
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
        
        // 요약:
        //     System.Windows.Forms.ImageList에 System.Drawing.Image 개체의 컬렉션을 캡슐화합니다.
        //[Editor("System.Windows.Forms.Design.ImageCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public sealed class ImageCollection : ICollection, IEnumerable
        {
            List<Image> _images = new List<Image>();
            Dictionary<String, Image> _imageDic = new Dictionary<string, Image>();
            // 요약:
            //     목록에 현재 들어 있는 이미지의 개수를 가져옵니다.
            //
            // 반환 값:
            //     목록에 있는 이미지 수입니다. 기본값은 0입니다.
            [Browsable(false)]
            public int Count { get { return _images.Count; } }
            //
            // 요약:
            //     System.Windows.Forms.ImageList에 이미지가 있는지 여부를 나타내는 값을 가져옵니다.
            //
            // 반환 값:
            //     목록에 이미지가 없으면 true이고, 그렇지 않으면 false입니다. 기본값은 false입니다.
            public bool Empty { get { return _images.Count == 0; } }
            //
            // 요약:
            //     목록이 읽기 전용인지 여부를 나타내는 값을 가져옵니다.
            //
            // 반환 값:
            //     항상 false입니다.
            public bool IsReadOnly { get { return false; } }
            //
            // 요약:
            //     System.Windows.Forms.ImageList.ImageCollection의 이미지와 연결된 키 컬렉션을 가져옵니다.
            //
            // 반환 값:
            //     System.Windows.Forms.ImageList.ImageCollection의 이미지 이름이 포함된 System.Collections.Specialized.StringCollection입니다.
            public StringCollection Keys
            {
                get
                {
                    StringCollection coll = new StringCollection();
                    
                    foreach (String key in _imageDic.Keys)
                    {
                        coll.Add(key);
                    }
                    return coll;
                    
                }
            }

            public List<Image> getImageList()
            {
                return _images;
            }

            

            // 요약:
            //     컬렉션의 지정된 인덱스에 있는 System.Drawing.Image를 가져오거나 설정합니다.
            //
            // 매개 변수:
            //   index:
            //     가져오거나 설정할 이미지의 인덱스입니다.
            //
            // 반환 값:
            //     index로 지정된 목록의 이미지입니다.
            //
            // 예외:
            //   System.ArgumentOutOfRangeException:
            //     인덱스가 0보다 작거나 System.Windows.Forms.ImageList.ImageCollection.Count보다 크거나 같은
            //     경우
            //
            //   System.ArgumentException:
            //     image가 System.Drawing.Bitmap이 아닌 경우
            //
            //   System.ArgumentNullException:
            //     할당할 이미지가 null이거나, System.Drawing.Bitmap이 아닌 경우
            //
            //   System.InvalidOperationException:
            //     이미지를 목록에 추가할 수 없는 경우
            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Image this[int index] {
                get{
                    if (_images.Count > index) return _images[index];
                    else return null;
                }
                set{
                    if (_images.Count > index) _images[index] = (Image)value;
                    else throw new IndexOutOfRangeException("인덱스가 리스트의 범위를 벗어났습니다: ("+index+"/"+(_images.Count-1)+")");
                } 
            }
            //
            // 요약:
            //     컬렉션에서 지정된 키를 가진 System.Drawing.Image를 가져옵니다.
            //
            // 매개 변수:
            //   key:
            //     컬렉션에서 검색할 이미지의 이름입니다.
            //
            // 반환 값:
            //     지정된 키를 가진 System.Drawing.Image입니다.
            public Image this[string key] {
                get { return _imageDic[key]; } 
            }

            // 요약:
            //     지정된 아이콘을 System.Windows.Forms.ImageList에 추가합니다.
            //
            // 매개 변수:
            //   value:
            //     목록에 추가할 System.Drawing.Icon입니다.
            //
            // 예외:
            //   System.ArgumentNullException:
            //     value가 null인 경우- 또는 -값이 System.Drawing.Icon이 아닌 경우
            public void Add(Icon value)
            {
                Image img = Image.FromHbitmap(value.Handle);
                _images.Add(img);
            }
            //
            // 요약:
            //     지정된 이미지를 System.Windows.Forms.ImageList에 추가합니다.
            //
            // 매개 변수:
            //   value:
            //     목록에 추가할 이미지의 System.Drawing.Bitmap입니다.
            //
            // 예외:
            //   System.ArgumentNullException:
            //     추가되는 이미지가 null인 경우
            //
            //   System.ArgumentException:
            //     추가되는 이미지가 System.Drawing.Bitmap이 아닌 경우
            public int Add(Object value)
            {
                _images.Add((Image)value);
                return _images.Count;
            }
            public int Add(Image value)
            {
                _images.Add(value);
                return _images.Count;
            }
            //
            // 요약:
            //     지정된 이미지를 System.Windows.Forms.ImageList에 추가하고 지정된 색을 사용하여 마스크를 생성시킵니다.
            //
            // 매개 변수:
            //   value:
            //     목록에 추가할 이미지의 System.Drawing.Bitmap입니다.
            //
            //   transparentColor:
            //     해당 이미지를 마스킹할 System.Drawing.Color입니다.
            //
            // 반환 값:
            //     새로 추가된 이미지의 인덱스이거나, 이미지를 추가할 수 없는 경우 -1입니다.
            //
            // 예외:
            //   System.ArgumentNullException:
            //     추가되는 이미지가 null인 경우
            //
            //   System.ArgumentException:
            //     추가되는 이미지가 System.Drawing.Bitmap이 아닌 경우
            public int Add(Image value, Color transparentColor){
                try
                {
                    if (value == null) throw new ArgumentNullException("Image is not exist...");
                    _images.Add(value);
                    if (value.RawFormat == ImageFormat.Bmp)
                    {
                        Bitmap bitmap = new Bitmap(value);
                        bitmap.MakeTransparent(transparentColor);
                        
                    }
                    else
                    {
                        throw new System.ArgumentException("This Image is not Bitmap format(BMP).");
                    }

                    return _images.Count;
                }
                catch
                {
                    return -1;
                }
            }
            //
            // 요약:
            //     지정된 키를 가진 아이콘을 컬렉션의 끝에 추가합니다.
            //
            // 매개 변수:
            //   key:
            //     아이콘의 이름입니다.
            //
            //   icon:
            //     컬렉션에 추가할 System.Drawing.Icon입니다.
            //
            // 예외:
            //   System.ArgumentNullException:
            //     icon이 null인 경우
            public void Add(string key, Icon icon){
                Image newImg = Image.FromHbitmap(icon.Handle);
                _images.Add(newImg);
                _imageDic.Add(key, newImg);
            }
            //
            // 요약:
            //     지정된 키를 가진 이미지를 컬렉션의 끝에 추가합니다.
            //
            // 매개 변수:
            //   key:
            //     이미지의 이름입니다.
            //
            //   image:
            //     컬렉션에 추가할 System.Drawing.Image입니다.
            //
            // 예외:
            //   System.ArgumentNullException:
            //     image가 null인 경우
            public void Add(string key, Image image)
            {
                _images.Add(image);
                _imageDic.Add(key, image);
            }
            //
            // 요약:
            //     이미지 배열을 컬렉션에 추가합니다.
            //
            // 매개 변수:
            //   images:
            //     컬렉션에 추가할 System.Drawing.Image 개체의 배열입니다.
            //
            // 예외:
            //   System.ArgumentNullException:
            //     images가 null인 경우
            public void AddRange(Image[] images)
            {
                _images.AddRange(images);
            }

            public void AddRange(List<Image> images)
            {
                _images.AddRange(images);
            }

            //
            // 요약:
            //     지정된 이미지에 대한 이미지 스트립을 System.Windows.Forms.ImageList에 추가합니다.
            //
            // 매개 변수:
            //   value:
            //     추가할 이미지가 있는 System.Drawing.Bitmap입니다.
            //
            // 반환 값:
            //     새로 추가된 이미지의 인덱스이거나, 이미지를 추가할 수 없는 경우 -1입니다.
            //
            // 예외:
            //   System.ArgumentException:
            //     추가되는 이미지가 null인 경우- 또는 - 추가되는 이미지가 System.Drawing.Bitmap이 아닌 경우
            //
            //   System.InvalidOperationException:
            //     이미지를 추가할 수 없는 경우- 또는 - 추가되는 이미지 스트립의 너비가 0이거나 기존 이미지 너비와 같지 않은 경우- 또는 - 이미지
            //     스트립 높이가 기존 이미지 높이와 같지 않은 경우
            public int AddStrip(Image value)
            {
                if (value == null) return -1;
                if (_images.Count > 0)
                {
                    if (_images[0].Width != value.Width || _images[0].Height != value.Height) return -1;
                    else
                    {
                        _images.Add(value);
                        return _images.Count;
                    }
                }
                else
                {
                    _images.Add(value);
                    return _images.Count;
                }

            }
            //
            // 요약:
            //     System.Windows.Forms.ImageList에서 이미지와 마스크를 모두 제거합니다.
            public void Clear()
            {
                _images.Clear();
                _imageDic.Clear();
            }
            //
            // 요약:
            //     지원되지 않습니다. System.Collections.IList.Contains(System.Object) 메서드는 지정된 개체가
            //     목록에 있는지 여부를 나타냅니다.
            //
            // 매개 변수:
            //   image:
            //     목록에서 찾을 System.Drawing.Image입니다.
            //
            // 반환 값:
            //     이미지가 목록에 있으면 true이고, 그렇지 않으면 false입니다.
            //
            // 예외:
            //   System.NotSupportedException:
            //     이 메서드가 지원되지 않는 경우
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool Contains(Object image)
            {
                return _images.Contains((Image)image);
            }
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool Contains(Image image)
            {
                return _images.Contains(image);
            }
            //
            // 요약:
            //     지정된 키를 가진 이미지가 컬렉션에 있는지 여부를 확인합니다.
            //
            // 매개 변수:
            //   key:
            //     검색할 이미지의 키입니다.
            //
            // 반환 값:
            //     지정된 키를 가진 이미지가 컬렉션에 있음을 나타내면 true이고, 그렇지 않으면 false입니다.
            public bool ContainsKey(string key)
            {
                return _imageDic.ContainsKey(key);
            }
            //
            // 요약:
            //     항목 컬렉션 전체에서 반복하는 데 사용할 수 있는 열거자를 반환합니다.
            //
            // 반환 값:
            //     항목 컬렉션을 나타내는 System.Collections.IEnumerator입니다.
            public IEnumerator GetEnumerator()
            {
                return _images.GetEnumerator();
            }
            //
            // 요약:
            //     지원되지 않습니다. System.Collections.IList.IndexOf(System.Object) 메서드는 목록에서 지정된
            //     개체의 인덱스를 반환합니다.
            //
            // 매개 변수:
            //   image:
            //     목록에서 찾을 System.Drawing.Image입니다.
            //
            // 반환 값:
            //     목록의 이미지에 대한 인덱스입니다.
            //
            // 예외:
            //   System.NotSupportedException:
            //     이 메서드가 지원되지 않는 경우
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int IndexOf(Object image)
            {
                return _images.IndexOf((Image)image);
            }
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int IndexOf(Image image)
            {
                return _images.IndexOf(image);
            }
            //
            // 요약:
            //     컬렉션에서 지정된 키를 가진 이미지 중 맨 처음 발견된 이미지의 인덱스를 확인합니다.
            //
            // 매개 변수:
            //   key:
            //     인덱스를 검색할 이미지의 키입니다.
            //
            // 반환 값:
            //     컬렉션에서 지정된 키를 가진 이미지가 있으면 그 중 맨 처음 발견된 인덱스(0부터 시작)이거나, 그렇지 않으면 -1입니다.
            public int IndexOfKey(string key)
            {
                if (_imageDic.ContainsKey(key))
                {
                    Image img = _imageDic[key];
                    return _images.IndexOf(img);
                }
                else return -1;
            }
            //
            // 요약:
            //     지원되지 않습니다. System.Collections.IList.Remove(System.Object) 메서드는 지정된 개체를 목록에서
            //     제거합니다.
            //
            // 매개 변수:
            //   image:
            //     목록에서 제거할 System.Drawing.Image입니다.
            //
            // 예외:
            //   System.NotSupportedException:
            //     이 메서드가 지원되지 않는 경우
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Remove(Object image)
            {
                _images.Remove((Image)image);
                foreach (String key in _imageDic.Keys)
                {
                    if (_imageDic[key].Equals((Image)image))
                    {
                        _imageDic.Remove(key);
                        return;
                    }
                }
            }
            //
            // 요약:
            //     목록에서 이미지를 제거합니다.
            //
            // 매개 변수:
            //   index:
            //     제거할 이미지의 인덱스입니다.
            //
            // 예외:
            //   System.InvalidOperationException:
            //     이미지를 제거할 수 없는 경우
            //
            //   System.ArgumentOutOfRangeException:
            //     인덱스 값이 0보다 작은 경우- 또는 - 인덱스 값이 이미지의 System.Windows.Forms.ImageList.ImageCollection.Count보다
            //     크거나 같은 경우
            public void RemoveAt(int index)
            {
                Image img = _images[index];
                foreach (String key in _imageDic.Keys)
                {
                    if (_imageDic[key].Equals(img))
                    {
                        _imageDic.Remove(key);
                        break;
                    }
                }
                _images.RemoveAt(index);
            }
            //
            // 요약:
            //     컬렉션에서 지정된 키를 가진 이미지를 제거합니다.
            //
            // 매개 변수:
            //   key:
            //     컬렉션에서 제거할 이미지의 키입니다.
            public void RemoveByKey(string key)
            {
                Image img = _imageDic[key];
                _images.Remove(img);
                _imageDic.Remove(key);
            }
            //
            // 요약:
            //     컬렉션에 있는 이미지의 키를 설정합니다.
            //
            // 매개 변수:
            //   index:
            //     컬렉션에 있는 이미지의 인덱스(0부터 시작)입니다.
            //
            //   name:
            //     이미지 키로 설정할 이미지의 이름입니다.
            //
            // 예외:
            //   System.IndexOutOfRangeException:
            //     지정된 인덱스가 0보다 작거나 System.Windows.Forms.ImageList.ImageCollection.Count보다 크거나
            //     같은 경우
            public void SetKeyName(int index, string name)
            {
                Image img = _images[index];
                _imageDic.Add(name, img);
            }

            public Boolean IsFixedSize { get { return false; } }

            public void Insert(int index, object img)
            {
                _images.Insert(index, (Image)img);
            }

            public Object SyncRoot
            {
                get { return null; }
            }

            public Boolean IsSynchronized { get { return false; } }

            public void CopyTo(Array arr, int arrayIndex)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr.SetValue(_images[i], arrayIndex + i);
                }
            }


        }
    }
}
