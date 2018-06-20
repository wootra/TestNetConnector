using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;
using XmlHandlers;
using CSEval;
using DataHandling;

namespace CustomParser
{
    public class BitItem
    {
        CPacketItem _parent;

        static TypeParser parser = new TypeParser(Assembly.GetExecutingAssembly(), new List<string>()
                {
                    "System" ,
                    "System.Collections.Generic" ,
                    "System.Linq" ,
                    "System.Text" ,
                    "System.Windows" ,
                    "System.Windows.Shapes" ,
                    "System.Windows.Controls" ,
                    "System.Windows.Media" ,
                    "System.IO" ,
                    "System.Reflection" ,
                    "CSEval"
                }
                        );

        /// <summary>
        /// default로 true가될 조건은 없다. 무조건 true이다.
        /// </summary>
        /// <param name="parent"></param>
        public BitItem(CPacketItem parent)
        {
            _parent = parent;
            PassCondition = "";
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="trueCondition">true가 될 조건..</param>
        public BitItem(CPacketItem parent, int trueCondition)
        {
            _parent = parent;
            PassCondition = trueCondition.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="trueCondition">true가 될 조건..</param>
        public BitItem(CPacketItem parent, string trueCondition)
        {
            _parent = parent;
            PassCondition = trueCondition;

        }

        /// <summary>
        /// BitItem Tag에서 값을 불러온다.
        /// </summary>
        /// <param name="root"></param>
        public void LoadXml(XmlNode root)
        {
            BitName = XmlGetter.Attribute(root, "Name");

            if (int.TryParse(XmlGetter.Attribute(root, "StartOffset"), out _startOffset) == false)
            {
                _startOffset = 0;
            }
            if (int.TryParse(XmlGetter.Attribute(root, "BitSize"), out _bitSize) == false)
            {
                _bitSize = 1;
            }
            string pc = XmlGetter.Attribute(root, "PassCondition").Replace("&lt;","<").Replace("&gt;",">");

            PassCondition = pc;
            Description = XmlGetter.Attribute(root, "Description");
            //Visible = (XmlGetter.Attribute(root, "Visible").Equals("True"));
            ShowOnReport = (XmlGetter.Attribute(root, "ShowOnReport").Equals("False")==false);//true가 기본값.
             
        }

        /// <summary>
        /// BitItems 태그에 값을 가진 BitItem Element를 추가한다.
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="bitItemsNode"></param>
        /// <returns></returns>
        public XmlNode GetXml(XmlDocument xDoc, XmlNode bitItemsNode)
        {
            XmlNode root = XmlAdder.Element(xDoc, "BitItem", bitItemsNode);
            XmlAdder.Attribute(xDoc, "Name", BitName, root);
            XmlAdder.Attribute(xDoc, "StartOffset", StartOffset.ToString(), root);
            XmlAdder.Attribute(xDoc, "BitSize", BitSize.ToString(), root);
            XmlAdder.Attribute(xDoc, "PassCondition", PassCondition.Replace("<","&lt;").Replace(">","&gt;"), root);
            XmlAdder.Attribute(xDoc, "Description", Description, root);
            //XmlAdder.Attribute(xDoc, "Visible", Visible?"True":"False", root);
            XmlAdder.Attribute(xDoc, "ShowOnReport", ShowOnReport ? "True" : "False", root);
            return root;
        }

        int _startOffset = 0;
        public int StartOffset { get { return _startOffset; } set { _startOffset = value; } }
        
        int _bitSize = 1;
        public int BitSize { 
            get { return _bitSize; } 
            set {
                if (value < 0) throw new Exception("bitSize cannot be minus...(" + value + ")");
                _bitSize = value; 
                
            }
        }

        public string NameWithParent
        {
            get
            {
                return _parent.Name + "." + BitName;
            }
        }
        public String BitName { get; set; }
        /*
        bool _visible = false;
        /// <summary>
        /// 테이블에서 보일 지 나타낸다.
        /// </summary>
        public bool Visible { get { return _visible; } set{ _visible = value;} }
        */
        /// <summary>
        /// Report에서 보일지 나타낸다.
        /// </summary>
        public bool ShowOnReport { get; set; }

        public BitItem Clone(CPacketItem parentItem=null)
        {

            BitItem clone = new BitItem(parentItem==null? _parent :parentItem);
            clone.StartOffset = StartOffset;
            clone.BitSize = BitSize;
            clone.BitName = BitName;
            clone.condFunc = condFunc;//.PassCondition = PassCondition;
            clone.PassCondition = _passCondition;
            //clone.Visible = Visible;
            clone.ShowOnReport = ShowOnReport;
            return clone;
        }

        public CPacketItem Parent { get { return _parent; } }
        
        static LexList LexListGet(string s)
        {
            LexListBuilder lb = new LexListBuilder();
            lb.Add(s);
            return lb.ToLexList();
        }
        Func<int, bool> condFunc;
        String _passCondition = "";
        /// <summary>
        /// PassCondition을 넣는다. 실제 Pass/Fail은 AutoTest를 진행하는 중에 들어오는값을 기준으로 정해진다.
        /// </summary>
        public string PassCondition { get { return _passCondition; }
            set
            {
                
                string str;
                int outValue;
                try
                {
                    if (value == null || value.Length == 0)
                    {
                        condFunc = new Func<int, bool>((x) =>
                        {
                            return true;//무조건 pass
                        });//기본함수..
                    }
                    else if (int.TryParse(value, out outValue))
                    {
                        //simple function..
                        condFunc = new Func<int, bool>((x) =>
                        {
                            if (x == outValue) return true;
                            else return false;
                        });//기본함수..
                    }
                    else 
                    {
                        LexToken.ShowError = (msg, theList) =>
                        {
                            throw new Exception(msg + "\n" + theList.CodeFormat);
                        };

                        
                        TypeParser.DefaultParser = parser;
                        //이때는 형식을 x: x==1 과 같은 식으로 썼을 때이다. 아니면 x==1과 같이 쓰면 된다.
                        // x: x==1 에서 x값을 바꾸면 cond: cond==1 이라고 쓸 수 있다.
                        string[] tokens = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        string name;
                        string cond;
                        if (tokens.Length == 0)
                        {
                            name = "x";
                            cond = "x==0";
                        }
                        else if (tokens.Length == 1)
                        {
                            name = "x";
                            int val;
                            if (int.TryParse(tokens[0], out val))
                            {
                                cond = "x==" + tokens[0];
                            }
                            else
                            {
                                cond = tokens[0];
                            }
                        }
                        else
                        {
                            name = tokens[0];
                            cond = tokens[1];
                        }
                        str = @"
                        bool Test (int "+name+@") 
                        { 
                            return "+ cond + @";
                        }";
                        LexList lexList = LexListGet(str);
                        condFunc = MakeMethod<Func<int, bool>>.Compile(parser, lexList);
                        
                    }
                    
                    //if (error) throw new Exception("There was an error", "Test Make with dialog");
                    //else MessageBox.Show("Ran OK", "Test Make with dialog");
                    _passCondition = value;
                }
                catch (Exception ex)
                {
                    throw ex;
                    //MessageBox.Show("There was a compilation or execution error.", "Test Make with dialog");
                }
            }
        }

        /// <summary>
        /// PassCondition이 없으면 0이 true, 1은 false이다. 
        /// </summary>
        public bool IsPassed(int value)
        {
            if(condFunc!=null) return condFunc(value);
            return true;
            /*
            if (PassCondition != null && PassCondition.Length > 0)
            {
                if (condFunc != null) return condFunc(value);
                else return true;
            }
            else
            {
                return true;
            }
             */
        }

        string _description = "";
        /// <summary>
        /// BitItem에 대한 설명..
        /// </summary>
        public string Description {
            get { return _description; }
            set
            {
                if (value == null) _description = "";
                else _description = value;
            }
        }

        
    }
    /// <summary>
    /// BitItemCollection이 변하면 발생한다.
    /// </summary>
    /// <param name="parent">해당 BitItemCollection의 부모</param>
    /// <param name="bitItem">변경된 BitItem</param>
    public delegate void BitItemCollectionChangedEvent(CPacketItem parent, BitItem bitItem);
    public enum BitItemCollectionEvents{Removed, Changed, Inserted};
    public class BitItemCollection: ICollection<BitItem>
    {
        CPacketItem _parent;
        public bool EventEnabled { get; set; }

        public event BitItemCollectionChangedEvent E_BitItemRemoved;
        public event BitItemCollectionChangedEvent E_BitItemCollectionChanged;
        public event BitItemCollectionChangedEvent E_BitItemInserted;

        ListDic<int, BitItem> _items = new ListDic<int, BitItem>();
        int[] _bitArea; //bit의 시작점이 어디인지를 알려준다. bitItem의 영역이 아니라면 -1이다.
        //int _bitSize;
        public BitItemCollection(CPacketItem parent)
        {
            _parent = parent;
            EventEnabled = true;
            //SetBitSize(byteSizes);
            //_bitSize = bitSize;
        }

        public int IndexOf(BitItem item)
        {
            foreach (int index in _items.Keys)
            {
                if (_items[index].Equals(item)) return index;
            }
            return -1;
        }

        /// <summary>
        /// bitOffset이 BitItem의 영역이면 해당 BitItem의 StartOffset을 리턴한다.
        /// 영역이 아니면 -1을 리턴한다.
        /// </summary>
        /// <param name="bitOffset"></param>
        /// <returns></returns>
        public int StartOffsetForBit(int bitOffset)
        {
            if (_bitArea != null)
            {
                return _bitArea[bitOffset];
            }
            else
            {
                return -1;
            }
        }

        public void InvokeEvent(BitItemCollectionEvents evt, BitItem item=null){
            switch (evt)
            {
                case BitItemCollectionEvents.Changed:
                    if (E_BitItemCollectionChanged != null && EventEnabled) E_BitItemCollectionChanged(_parent, item);
                    break;
                case BitItemCollectionEvents.Inserted:
                    if (E_BitItemInserted != null && EventEnabled) E_BitItemInserted(_parent, item);
                    break;
                case BitItemCollectionEvents.Removed:
                    if (E_BitItemRemoved != null && EventEnabled) E_BitItemRemoved(_parent, item);
                    break;
            }
        }

        int _bitSize = 0;
        /// <summary>
        /// 총 bit 크기를 나타낸다.
        /// </summary>
        public int BitSize { get { return _bitSize; } }

        internal void SetBitSize(int byteSize)
        {
            
            _bitSize = byteSize * 8;
            if (_bitSize > 0) MakeBitArea(_bitSize);
        }

        void MakeBitArea(int bitSize)
        {
            _items.Clear();//bitItem을 모두 없앰..
            if (bitSize > 0)
            {
                _bitArea = new int[bitSize];
                for (int i = 0; i < bitSize; i++)
                {
                    _bitArea[i] = -1;
                }
            }
            else
            {
                _bitArea = new int[0];
            }
            
        }
        /// <summary>
        /// 해당 위치에 새 item을 집어넣는다. 만일 영역이 겹치면 겹치는 영역에 해당하는 것들을 모두 없앤다.
        /// </summary>
        /// <param name="startOffset">시작 bit offset</param>
        /// <param name="bitSize">bit size</param>
        /// <param name="name">bit item 의 이름</param>
        public void Insert(int startOffset, int bitSize, String name)
        {
            BitItem newItem = new BitItem(_parent);
            newItem.StartOffset = startOffset;
            newItem.BitSize = bitSize;
            newItem.BitName = name;
            newItem.PassCondition = "";
            Insert(newItem);

        }
        /// <summary>
        /// 해당 위치에 새 item을 집어넣는다. 만일 영역이 겹치면 겹치는 영역에 해당하는 것들을 모두 없앤다.
        /// </summary>
        /// <param name="item"></param>
        public void Insert(BitItem item)
        {
            Insert(item, EventEnabled);
        }

        /// <summary>
        /// 해당 위치에 새 item을 집어넣는다. 만일 영역이 겹치면 겹치는 영역에 해당하는 것들을 모두 없앤다.
        /// </summary>
        /// <param name="item"></param>
        public void Insert(BitItem item, bool eventEnabled)
        {
            //List<BitItem> _items = _items;// new List<BitItem>(_items); //exception이 나오면 
            
            List<BitItem> itemsToRemove = new List<BitItem>();
            for (int i = item.StartOffset; i < item.StartOffset + item.BitSize; i++)//해당영역을 모두 없앰..
            {
                if (_bitArea!=null && _bitArea[i] >= 0)
                {
                    BitItem itemToRemove = _items[_bitArea[i]];
                    RemoveAt(itemToRemove.StartOffset, false);
                    i = itemToRemove.StartOffset + itemToRemove.BitSize - 1;//i++를 위해 하나 적게..
                }
            }

            _items[item.StartOffset] = item;
            
            if (_bitArea != null)
            {
                for (int i = item.StartOffset; i < item.StartOffset + item.BitSize; i++)
                {
                    _bitArea[i] = item.StartOffset;
                }
            }
            if (E_BitItemCollectionChanged != null && eventEnabled) E_BitItemCollectionChanged(_parent, null);

            if (E_BitItemInserted != null && eventEnabled) E_BitItemInserted(_parent, item);
            
            //_items = _items;
        }

        /// <summary>
        /// 다른 item들의 위치를 변경시키지 않고 그냥 지운다.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            RemoveAt(index, EventEnabled);
            
        }

        /// <summary>
        /// 다른 item들의 위치를 변경시키지 않고 그냥 지운다.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index, bool eventEnabled)
        {
            BitItem itemToRemoved = _items[index];
            if (_items.ContainsKey(index))
            {
                _items.Remove(index);
                if (_bitArea != null)
                {
                    for (int i = itemToRemoved.StartOffset; i < itemToRemoved.StartOffset + itemToRemoved.BitSize; i++)
                    {
                        _bitArea[i] = -1;
                    }
                }
            }
            else throw new Exception("BitItemCollection.RemoveAt - no item in index[" + index + "]");
            if (E_BitItemRemoved != null && eventEnabled) E_BitItemRemoved(_parent, itemToRemoved);

        }
        /// <summary>
        /// 다른 item들이 빈 공간만큼 왼쪽으로 이동한다.
        /// </summary>
        /// <param name="index"></param>
        public void PopAt(int index)
        {
            if (_items.ContainsKey(index) == false) return;//해당 자리에 아무것도 없으면 그냥 리턴
                    
            BitItem item = _items[index];//item to remove..
            _lastOffset -= item.BitSize;
            _nextOffset -= item.BitSize;

            ListDic<int, BitItem> newItemList = new ListDic<int, BitItem>();
            int sizeToMove = item.BitSize;
            
            foreach (int key in _items.Keys)//새로운 list를 만듬..
            {
                if (key < item.StartOffset)//없어질 item을 제외하고,  //뒤쪽에 존재하는 item들을 모두 모은다.
                {
                    newItemList.Add(key, _items[key]);
                }
                else if (key > item.StartOffset)
                {
                    newItemList.Add(key-item.BitSize, _items[key]);//빠질 item의 크기만큼 이동한다
                }
                else
                {
                    //pass..pop할 item
                }
            }
            _items = newItemList;

            int i = index;
            if (_bitArea != null)
            {
                while (i < BitSize)//_bitArea를 만듬..
                {
                    if (_items[i] != null)
                    {
                        for (int j = 0; j < _items[i].BitSize; j++)
                        {
                            _bitArea[i + j] = _items[i].StartOffset;
                        }
                        i += _items[i].BitSize;
                        continue;
                    }
                    i++;
                }
            }
            if (E_BitItemRemoved != null && EventEnabled) E_BitItemRemoved(_parent, _items[index]);
            
        }

        /// <summary>
        /// bit영역의 index를 정확하게 기입해야 한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BitItem this[int index]
        {
            get
            {
                if (_items.ContainsKey(index)) return _items[index];
                else return null;
            }
        }

        int _nextOffset = 0;
        int _lastOffset = 0;
        /// <summary>
        /// 가장 뒤에 새로운 item을 추가한다.
        /// 만일 item을 추가할 자리가 모자르면 exeption을 발생시킨다.
        /// </summary>
        /// <param name="item"></param>
        public void Add(BitItem item)
        {
            int nextOffset = _nextOffset;// 0;
            /*
            foreach (int key in _items.Keys)
            {
                nextOffset = key + _items[key].BitSize;
            }
             */
            if (nextOffset + item.BitSize > BitSize) throw new Exception("BitItemCollection.Add(BitItem) - Bit overflow!");

            
            item.StartOffset = nextOffset;
            _items[nextOffset] = item;

            
            _nextOffset += _items[nextOffset].BitSize;
            _lastOffset = nextOffset;

            if (E_BitItemInserted != null && EventEnabled) E_BitItemInserted(_parent, _items[nextOffset]);

            if (E_BitItemCollectionChanged != null && EventEnabled) E_BitItemCollectionChanged(_parent, null);
        }

        /// <summary>
        /// 가장 뒤에 새로운 item을 추가한다.
        /// 만일 item을 추가할 자리가 모자르면 exeption을 발생시킨다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bitSize"></param>
        public void Add(String name, int bitSize, bool visible=true)
        {
            BitItem newItem = new BitItem(_parent);
            newItem.BitSize = bitSize;
            newItem.BitName = name;
            //newItem.Visible = visible;
            Add(newItem);
        }

        public void Clear()
        {
            _items.Clear();
            _nextOffset = 0;
            if (E_BitItemCollectionChanged != null && EventEnabled) E_BitItemCollectionChanged(_parent, null);
        }

        /// <summary>
        /// collection안에 존해하는지 알아냄..
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(BitItem item)
        {
            if (_items.ContainsKey(item.StartOffset))
            {
                if (_items[item.StartOffset].Equals(item)) return true;
            }
            return false;
        }

        /// <summary>
        /// collection안에 존해하는지 알아냄..
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ContainsKey(int index)
        {
            if (_items.ContainsKey(index)) return true;
            return false;
        }

        /// <summary>
        /// item의 StartOffset과 BitSize로 보아 Collection 안에 중복되는공간이 있다면, 그 크기를 리턴한다.
        /// 중복공간이 없다면 0이다.
        /// 만일 공간이 out of range일 때는 -1이다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int SizeOfIntersectionBits(BitItem item)
        {
            int sizeOfIntersect = 0;
            
            for (int i = item.StartOffset; i < item.StartOffset + item.BitSize; i++)
            {
                if (_items.ContainsKey(i)) sizeOfIntersect++;//중복공간의 숫자를 센다.
            }
            if (item.StartOffset + item.BitSize > BitSize) return -1;
            return sizeOfIntersect;
        }

        public new List<BitItem> ToList()
        {
            return _items.Values.ToList();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool Remove(BitItem item)
        {
            try
            {
                RemoveAt(item.StartOffset);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public IEnumerator<BitItem> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

       
        /// <summary>
        /// don't use this.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(BitItem[] array, int arrayIndex)
        {
            
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// bitSize를 다시 셋팅한다.
        /// 만일 bitSize가 작아지는데 작아질 영역 안에 item이 있다면 exeption을 리턴한다.
        /// </summary>
        /// <param name="forceDeleteOverflowItems">만일 bitSize가 작아질 때 true로 셋팅하면 자동으로 해당 영역에 들어있는 item을 삭제한다.</param>
        public bool CheckBitSizesSafe(int bytesToChange, bool forceDeleteOverflowItems=false)
        {
            List<BitItem> itemsToRemove = new List<BitItem>();
            int bitSize = bytesToChange * 8;

            foreach (int key in _items.Keys)
            {
                if (_items[key].StartOffset + _items[key].BitSize >= bitSize)
                {
                    if (forceDeleteOverflowItems)
                    {
                        itemsToRemove.Add(_items[key]);
                    }
                    else
                    {
                        return false;
                        //throw new Exception("There is bitItem in the area to be removed...");
                    }
                }
            }
            foreach (BitItem item in itemsToRemove)
            {
                _items.Remove(item.StartOffset);
            }
            if (itemsToRemove.Count > 0) return false;
            else
            {
                return true;
            }
        }
    }
}
