using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;
using System.Runtime.InteropServices;
using DataHandling;
using System.Data;
using System.Collections;
using CSEval;
using System.Reflection;

namespace CustomParser
{
    public enum CStructItemTypes { Normal = 0, Range, Func }
    public class CPacketItem
    {
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
                        

        string _typeString = "uchar";
        CPacketStruct _parent = null;
        /*
        bool _visible = true;
        public bool Visible{
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }
        */
        bool _showOnReport = true;
        public bool ShowOnReport
        {
            get
            {
                return _showOnReport;
            }
            set
            {
                _showOnReport = value;
            }
        }
        BitItemCollection _bitItems;
        public BitItemCollection BitItems{
            get { return _bitItems; }
        }

        public CPacketItem(CPacketStruct parent=null) {
            Init("Undefined", "uchar",1, "", parent);
            _parent = parent;
            _bitItems = new BitItemCollection(this);
            this.Length = 1;
            InitValues = new String[] { "0" };
        }

        internal void SetParent(CPacketStruct parent){
            _parent= parent;
        }

        /// <summary>
        /// 배열타입이지만 size를 우선하여 크기지정. values 안의 내용이 적으면 0으로 채움..
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="values"></param>
        /// <param name="parent"></param>
        /// <param name="comment"></param>
        public CPacketItem(String name, String type, int size, String[] values, CPacketStruct parent = null, string comment = "")
        {

            string[] initValueClone = new string[size];
            values.CopyTo(initValueClone, 0);

            Init(name, type, size, comment, parent);

            this.InitValues = initValueClone; // = values.Clone();

        }

        /// <summary>
        /// 배열타입0
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="values"></param>
        /// <param name="parent"></param>
        /// <param name="comment"></param>
        public CPacketItem(String name, String type, String[] values, CPacketStruct parent=null, string comment = "")
        {
            
            string[] initValueClone = new string[values.Length];
            values.CopyTo(initValueClone, 0);
            
            Init(name, type, values.Length, comment, parent);

            this.InitValues = initValueClone; // = values.Clone();
            
        }

        void Init(String name, String type, int length, string comment, CPacketStruct parent)
        {
            this._parent = parent;
            this._nameStr = name;
            if (length == 0) throw new Exception("CPacketItem:: it's length cannot be 0!!");
            //_size = length;
            
            _bitItems = new BitItemCollection(this);

            SetType(type, true);
            Length = length;
            //this.TypeString = type;
            _comment = comment;
            
        }


        /// <summary>
        /// 단일 type의 Item.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="initString"></param>
        /// <param name="parent"></param>
        /// <param name="comment"></param>
        public CPacketItem(String name, String type, String initString,  CPacketStruct parent,string comment = "")
        {
            
            Init(name, type, 1, comment, parent);
            this.InitString = initString;
        }

        public CPacketItem(CPacketItem cloneBase, bool copyBitItems)
        {
            Init(cloneBase.Name, cloneBase.TypeString, cloneBase.Length, cloneBase.Description, cloneBase.Parent);
            PassCondition = cloneBase.PassCondition;
            if (copyBitItems)
            {
                foreach (BitItem bItem in cloneBase.BitItems)
                {
                    _bitItems.Add(bItem.Clone(this));
                }
            }
        }
        


        /// <summary>
        /// 배열타입. initString으로 모든 배열의내용을 채움.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="initString"></param>
        /// <param name="parent"></param>
        /// <param name="comment"></param>
        public CPacketItem(String name, String type, int size, String initString, CPacketStruct parent, string comment = "")
        {

            Init(name, type, size, comment, parent);
            string[] initValueClone = new string[size];
           
            this.InitValues = initValueClone; // = values.Clone();
            for (int i = 0; i < size; i++)
            {
                this.InitValues[i] = initString;
            }
            
        }

        /// <summary>
        /// 배열 타입 Item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <param name="func"></param>
        /// <param name="parent"></param>
        /// <param name="comment"></param>
        public CPacketItem(String name, String type, int length, FunctionInfo func,  CPacketStruct parent, string comment = "")
        {
            Init(name, type, length, comment, parent);
            this.Function = func;
        }

        public int Index
        {
            get
            {
                if (_parent == null) return -1;
                else
                {
                    return _parent.Items.IndexOf(this);
                }
            }
        }
        string _nameStr;
        public String Name { get { return _nameStr; } set { _nameStr = value; } }

        

        public string TypeString{
            get { return _typeString; }
        }

        /// <summary>
        /// type을 지정한다. bit크기와 관련하여 bit 방이 있으면 없앨지 정할 수 있다.
        /// 없애지 않기로 하면 exeption을 리턴한다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="removeAllItemsInRemovingArea"></param>
        public void SetType(string type, bool removeAllItemsInRemovingArea)
        {
            Type realTypeTemp = TypeHandling.getTypeFromTypeName(type, TypeHandling.Platforms.C64Bit);

            string typeStrTemp =  TypeHandling.GetTypeString(realTypeTemp);
            int byteSizes = Marshal.SizeOf(realTypeTemp) * Length;

            if (_bitItems != null)
            {

                if (removeAllItemsInRemovingArea) _bitItems.CheckBitSizesSafe(Marshal.SizeOf(realTypeTemp)*Length, true);
                else
                {
                    if (_bitItems.CheckBitSizesSafe(byteSizes) == false)
                    {
                        throw new Exception("Bit Item Overflow!! - size is recovered..");
                    }
                }
                _bitItems.SetBitSize(byteSizes);
            }
            else
            {
                throw new Exception("BitItems is null! check...");
            }

            _realType = realTypeTemp;
            _typeString = typeStrTemp;

            
            //_typeString = type;
        }

        Type _realType;
        public Type RealType
        {
            get
            {
                return _realType;
            }
        }
        int _size = 1;
        //int _totalSize = 1;
        /// <summary>
        /// byte단위의 총 크기
        /// </summary>
        public int TotalSize
        {
            get
            {
                if (_realType.IsValueType)
                {
                    return _size * Marshal.SizeOf(_realType);// _totalSize;
                }
                else if (_realType == typeof(String))
                {
                    if (InitValues.Length > 0) return InitValues[0].Length;
                    else return 0;
                }
                else return 0;

            }
        }

        public int size
        {
            get
            {
                return this.TotalSize;

            }
        }


        /// <summary>
        /// type단위의 item이 몇개 있는지..
        /// </summary>
        public int Length{
            get
            {
                /*
                if (Var != null && Var.Exists &&  Var.ValueExists)
                {
                    return Var.Values.Length;
                }else{
                    return _size;
                }
                 */
                return _size;
            }
            set
            {

                if (value < 1)
                {
                    InitValues = null;
                    _size = 0;
                }
                else
                {
                    _size = value;
                    
                    if (InitValues == null)
                    {
                        string[] initValues = new String[value];
                        for (int i = 0; i < value; i++)
                        {
                            initValues[i] = "0";
                        }
                        InitValues = initValues;
                    }
                    else
                    {
                        if (InitValues.Length < value)
                        {
                            String[] newValues = new string[value];
                            for (int i = 0; i < InitValues.Length; i++)
                            {
                                newValues[i] = InitValues[i];
                            }
                            for (int i = InitValues.Length; i < value; i++)
                            {
                                newValues[i] = "0";
                            }

                            InitValues = newValues;
                        }
                        else if (InitValues.Length > value)
                        {
                            String[] newValues = new string[value];
                            for (int i = 0; i < value; i++)
                            {
                                newValues[i] = InitValues[i];
                            }
                            InitValues = newValues;
                        }
                        else
                        {
                            //do nothing..
                        }
                    }
                }
                if (_realType == typeof(string))
                {
                    _bitItems.SetBitSize(0);
                }
                else
                {
                    int byteSize = _size * Marshal.SizeOf(_realType);
                    _bitItems.SetBitSize(byteSize);//bit방 초기화..
                }
                PassCondition = _passCondition;//Length만 변할 때..
            }
        }


        static LexList LexListGet(string s)
        {
            LexListBuilder lb = new LexListBuilder();
            lb.Add(s);
            return lb.ToLexList();
        }

        List<Func<int, bool>> condFuncs = new List<Func<int,bool>>();
        String _passCondition = "";
        /// <summary>
        /// PassCondition을 넣는다. 실제 Pass/Fail은 AutoTest를 진행하는 중에 들어오는값을 기준으로 정해진다.
        /// </summary>
        public string PassCondition
        {
            get { return _passCondition; }
            set
            {
                condFuncs.Clear();
                string org = (value == null) ? "" : value;
                string[] tkn = org.Split(",".ToCharArray());
                _passCondition = org;
                for (int i = 0; i < Length; i++)
                {
                    string aVal = (tkn.Length > i) ? tkn[i] : "";
                    string str;
                    int outValue;
                    try
                    {
                        if (aVal.Length == 0)
                        {
                            condFuncs.Add(new Func<int, bool>((x) =>
                            {
                                return true;//무조건 pass
                            }));//기본함수..
                        }
                        else if (int.TryParse(aVal, out outValue))
                        {
                            //simple function..
                            condFuncs.Add(new Func<int, bool>((x) =>
                            {
                                if (x == outValue) return true;
                                else return false;
                            }));//기본함수..
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
                            string[] tokens = aVal.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
                        bool Test (int " + name + @") 
                        { 
                            return " + cond + @";
                        }";
                            LexList lexList = LexListGet(str);
                            condFuncs.Add(MakeMethod<Func<int, bool>>.Compile(parser, lexList));

                        }

                        //if (error) throw new Exception("There was an error", "Test Make with dialog");
                        //else MessageBox.Show("Ran OK", "Test Make with dialog");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                        //MessageBox.Show("There was a compilation or execution error.", "Test Make with dialog");
                    }
                }

            }
        }

        /// <summary>
        /// PassCondition이 없으면 0이 true, 1은 false이다. 
        /// </summary>
        public bool IsPassed(int index, int value)
        {
            if (condFuncs[index] != null) return condFuncs[index](value);
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

        String[] _initValues = null;
        Array _initValueArray = null;

        /// <summary>
        /// 버퍼에 있는 값을 initValue와 비교하여 그 차이를 리턴한다.
        /// +값이면 buff의 값이 큰 것을 나타내고, -값이면 buff의 값이 작은 것을 나타낸다.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="startIndex"></param>
        /// <param name="values">받은 값.swap이면 swap된 값이 string으로 들어간다.</param>
        /// <returns>차이값의 배열. initValue의 버퍼크기와 동일하다.</returns>
        public double[] GetMarginFromInitValue(Array buff, int startIndex, object [] values=null)
        {
            double[] margins = new double[_size];
            if(_initValueArray==null) return null;
            else if(buff==null) throw new Exception("입력 buff가 null임");
            else if(Buffer.ByteLength(buff)-startIndex < Buffer.ByteLength(_initValueArray)){
                throw new Exception("입력버퍼의 크기가 item의 buff크기보다 작음");
            }else{
                 if (RealType == typeof(byte))
                    {
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                byte value = value = TypeArrayConverter.CopyBufferToVariable<byte>(buff, startIndex + i, -1, true, false);
                                // (byte)(((byte[])buff)[startIndex+i]);
                                margins[i] = value - (byte)(((byte[])_initValueArray)[i]);
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                                
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(sbyte))
                    {
                         for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                sbyte value = TypeArrayConverter.CopyBufferToVariable<sbyte>(buff, startIndex + i, -1, true, false);
                                // (byte)(((byte[])buff)[startIndex + i]);
                                margins[i] = value - (byte)(((byte[])_initValueArray)[i]);
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(short))
                    {
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                short value;
                                if(IsSwap){

                                    value= TypeArrayConverter.CopyBufferToVariable<short>(buff, startIndex + i, -1, false, true);
                                    // =  Swaper.swap<short>((short)(((short[])buff)[startIndex + i]));
                                    margins[i] = value - ((short[])_initValueArray)[i];
                                        
                                }else{
                                    value = TypeArrayConverter.CopyBufferToVariable<short>(buff, startIndex + i, -1, false, false);
                                    //(short)(((short[])buff)[startIndex + i]);
                                    margins[i] = value - (((short[])_initValueArray)[i]);
                                }
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(ushort))
                    {
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                ushort value;
                                if (IsSwap)
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<ushort>(buff, startIndex + i, -1, true, true); 
                                    //Swaper.swap<ushort>((ushort)(((ushort[])buff)[startIndex + i]));
                                    margins[i] = value - ((ushort[])_initValueArray)[i];

                                }
                                else
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<ushort>(buff, startIndex + i, -1, true, false);
                                    //(ushort)(((ushort[])buff)[startIndex + i]);
                                    margins[i] = value - (((ushort[])_initValueArray)[i]);
                                }
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(int))
                    {
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                int value;
                                if (IsSwap)
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<short>(buff, startIndex + i, -1, false, true);
                                        //Swaper.swap<int>((int)(((int[])buff)[startIndex + i]));
                                    margins[i] = value - ((int[])_initValueArray)[i];

                                }
                                else
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<short>(buff, startIndex + i, -1, false, false);
                                    //(int)(((int[])buff)[startIndex + i]);
                                    margins[i] = value - (((int[])_initValueArray)[i]);
                                }
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(uint))
                    {
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                uint value;
                                if (IsSwap)
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<uint>(buff, startIndex + i, -1, true, true);
                                    //value = Swaper.swap<uint>((uint)(((uint[])buff)[startIndex + i]));
                                    margins[i] = value - ((uint[])_initValueArray)[i];

                                }
                                else
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<uint>(buff, startIndex + i, -1, true, false);
                                    //value = (uint)(((uint[])buff)[startIndex + i]);
                                    margins[i] = value - (((uint[])_initValueArray)[i]);
                                }
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(long))
                    {
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                long value;
                                if (IsSwap)
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<short>(buff, startIndex + i, -1, false, true);
                                        //Swaper.swap<long>((long)(((long[])buff)[startIndex + i]));
                                    margins[i] = value - ((long[])_initValueArray)[i];

                                }
                                else
                                {
                                    value = TypeArrayConverter.CopyBufferToVariable<short>(buff, startIndex + i, -1, false, false);
                                    //(long)(((long[])buff)[startIndex + i]);
                                    margins[i] = value - (((long[])_initValueArray)[i]);
                                }
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(ulong))
                    {
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                ulong value;
                                if (IsSwap)
                                {
                                    value = Swaper.swap<ulong>((ulong)(((ulong[])buff)[startIndex + i]));
                                    margins[i] = value - ((ulong[])_initValueArray)[i];

                                }
                                else
                                {
                                    value = (ulong)(((ulong[])buff)[startIndex + i]);
                                    margins[i] = value - (((ulong[])_initValueArray)[i]);
                                }
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(float))
                    {
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                float value;
                                if (IsSwap)
                                {
                                    value = Swaper.swap<float>((float)(((float[])buff)[startIndex + i]));
                                    margins[i] = value - ((float[])_initValueArray)[i];

                                }
                                else
                                {
                                    value = (float)(((float[])buff)[startIndex + i]);
                                    margins[i] = value - (((float[])_initValueArray)[i]);
                                }
                                if (values != null && values.Length >= _size)
                                {
                                    values[i] = value;
                                }
                                else
                                {
                                    throw new Exception("value size exception..");
                                }
                            }
                            catch (Exception ex)
                            {
                                margins[i] = 0;
                            }
                        }
                    }
                 else if (RealType == typeof(double))
                 {
                     for (int i = 0; i < InitValues.Length; i++)
                     {
                         try
                         {
                             double value;
                             if (IsSwap)
                             {
                                 value = Swaper.swap<double>((double)(((double[])buff)[startIndex + i]));
                                 margins[i] = value - ((double[])_initValueArray)[i];

                             }
                             else
                             {
                                 value = (double)(((double[])buff)[startIndex + i]);
                                 margins[i] = value - (((double[])_initValueArray)[i]);
                             }
                             if (values != null && values.Length >= _size)
                             {
                                 values[i] = value;
                             }
                             else
                             {
                                 throw new Exception("value size exception..");
                             }
                         }
                         catch (Exception ex)
                         {
                             margins[i] = 0;
                         }
                     }
                 }
                 else
                 {
                     throw new Exception("No RealType..." + RealType);
                 }
            }
            return margins;
        }

        public String[] InitValues
        {
            get
            {
                return _initValues;
            }

            set
            {
                _initValues = value;
                if (value == null) _initValueArray = null;
                else
                {
                    
                    if (RealType == typeof(byte))
                    {
                        byte[] dst;
                        _initValueArray = dst = new byte[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                dst[i] = Convert.ToByte(InitValues[i]);
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(sbyte))
                    {
                        sbyte[] dst;
                        _initValueArray = dst = new sbyte[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                dst[i] = Convert.ToSByte(InitValues[i]);
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(short))
                    {
                        short[] dst;
                        _initValueArray = dst = new short[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                dst[i] = Convert.ToInt16(InitValues[i]);
                                
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                        //Swaper.swapWithSize(buffer, dst, Marshal.SizeOf(item.RealType), item.TotalSize, startOffsetInByte, 0);
                    }
                    else if (RealType == typeof(ushort))
                    {
                        ushort[] dst;
                        _initValueArray = dst = new ushort[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                    dst[i] = Convert.ToUInt16(InitValues[i]);
                                
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(int))
                    {
                        int[] dst;
                        _initValueArray = dst = new int[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                    dst[i] = Convert.ToInt32(InitValues[i]);
                                
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(uint))
                    {
                        uint[] dst;
                        _initValueArray = dst = new uint[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                    dst[i] = Convert.ToUInt32(InitValues[i]);
                                
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(long))
                    {
                        long[] dst;
                        _initValueArray = dst = new long[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                    dst[i] = Convert.ToInt64(InitValues[i]);
                               
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(ulong))
                    {
                        ulong[] dst;
                        _initValueArray = dst = new ulong[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                    dst[i] = Convert.ToUInt64(InitValues[i]);
                                
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(float))
                    {
                        float[] dst;
                        _initValueArray = dst = new float[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                    dst[i] = Convert.ToSingle(InitValues[i]);
                               
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                    else if (RealType == typeof(double))
                    {
                        double[] dst;
                        _initValueArray = dst = new double[Length];
                        //Buffer.BlockCopy(buffer, startOffsetInByte, dst, 0, item.TotalSize);
                        for (int i = 0; i < InitValues.Length; i++)
                        {
                            try
                            {
                                
                                    dst[i] = Convert.ToDouble(InitValues[i]);
                                
                            }
                            catch (Exception ex)
                            {
                                dst[i] = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 초기값 배열. 실제 값의 배열이다.
        /// InitValues를 셋팅하면 해당 type에 맞추어 생성된다.
        /// </summary>
        public Array InitValueArray
        {
            get { return _initValueArray; }
        }
        /// <summary>
        /// 허용오차
        /// </summary>
        public double margin = 0;
        public bool IsSwap=true;
        public FunctionInfo Function = null;
        public VariableInfo Var = null;//new VariableInfo();
        public Dictionary<String, PacketData> PacketData = new Dictionary<string, PacketData>();
       
        string _comment = "";
        public String Description
        {
            get { return _comment; }
            set { _comment = value; }
        }
        public String InitString{
            get
            {
                string value="";
                if (InitValues == null) return "";
                for (int i = 0; i < InitValues.Length; i++)
                {
                    if (i > 0) value += ",";
                    value += InitValues[i];
                }
                return value;
                /*
                if (InitValues.Length > 1) return "";
                else if (InitValues.Length == 1) return InitValues[0];
                else return "";
                 */
            }
            set
            {
                if (value.Length > 0)
                {
                    InitValues = value.Split(",/; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);//  new String[1];
                }
                else
                {
                    InitValues = null;
                }
                //InitValues[0] = value;
            }
        }

        List<int> _selectedCells = new List<int>();
        public List<int> SelectedCells {
            get { return _selectedCells; }
        }

        bool _isSelected = false;
        /// <summary>
        /// 줄이 선택되었는지 나타내는 임시변수..
        /// </summary>
        public bool IsSelected { get { return _isSelected; } set { _isSelected=value; } }

        public CPacketStruct Parent { get { return _parent; } }

        /// <summary>
        /// 같은 이름이 존재하는지 알아본다. 없으면 true를 리턴한다.
        /// item을 넣는 이유는 이 아이템을 제외하고 검색해야 하기 때문이다.
        /// </summary>
        /// <param name="item">이름을 셋팅할 item</param>
        /// <param name="name">setting할 이름</param>
        /// <returns>같은 이름 없을 시 true리턴.</returns>
        public bool IsNewName(BitItem bitItem, String name)
        {
            foreach (BitItem bitem in _bitItems)
            {
                if (bitem.Equals(bitItem)) continue;//같은 item 제외
                if (bitem.BitName.Equals(name))
                {
                    return false;//같은 이름이 있으므로 newName이 아니다.
                }
            }
            return true;
        }

        int _byteOffset = -1;
        /// <summary>
        /// 버퍼상에서 그 위치가 어디부터 시작하는지를 나타낸다.
        /// 실제 버퍼는 없으므로 단순 위치값만 가진다.
        /// </summary>
        public int ByteOffset { get { return _byteOffset; } set { _byteOffset = value; } }

        public static int GetBitItemData(int value, int bitOffset, int bitSize)
        {
            //int bitOffset = bitItem.StartOffset - startByteOffset * 8;
            int mask = ((1 << (bitSize))) - 1 << bitOffset;
            return (value & mask)>>bitOffset;
        }

        public CPacketItem Clone(CPacketStruct parent=null)
        {
            CPacketItem clone = new CPacketItem((parent==null)?_parent : parent);
            clone.Init(_nameStr, _typeString, Length, _comment, _parent);
            clone._byteOffset = _byteOffset;
            //clone._initValueArray = _initValueArray;
            //clone._initValues = _initValues;
            clone._isSelected = _isSelected;
            clone._selectedCells = new List<int>();
            //clone.ShowOnReport = ShowOnReport;
            clone._showOnReport = _showOnReport;
            clone.BitItems.EventEnabled = false;
            clone.InitString = InitString;
            clone.PassCondition = PassCondition;
            foreach (BitItem item in _bitItems)
            {
                BitItem bitClone = item.Clone();
                clone._bitItems.Insert(bitClone);
            }
            clone.BitItems.EventEnabled = true;
            return clone;
        }

        public string GetPassCondition(int i)
        {
            string[] tkn = _passCondition.Split(",".ToCharArray());
            if (i >= tkn.Length) return "";
            else return tkn[i];
        }

        public void SetPassCondition(int index, string p)
        {
            string[] tkn = _passCondition.Split(",".ToCharArray());
            string cond = "";
            for (int i = 0; i < Length; i++)
            {
                if (i != 0) cond += ",";
                if (i >= tkn.Length)
                {
                    if (i == index) cond += p;
                    else cond += "";
                }
                else
                {
                    if (i == index) cond += p;
                    else cond += tkn[i];
                }
            }
            PassCondition = cond;
        }
    }

    public class PacketData
    {
        String _value="";
        public String Value { get { return _value; } set { _value=value; } }
    }

    public class RealValue{
        public RealValue(){
            
        }
    }
}
