using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DataHandling;
using RtwEnums.Network;

namespace NetworkModules3
{
    [Serializable]
    public class NetworkPacketWith<T> : ResizableNetworkObject<T>,INetPacket
    {
        // constructor
        Dictionary<int, int> _startByteOffsetInBuffer = new Dictionary<int, int>();
        Dictionary<int, int> _sizeOfItemInBuffer = new Dictionary<int, int>();
        Dictionary<int, Type> _returnType = new Dictionary<int, Type>();
        Dictionary<int, double> _LSB = new Dictionary<int, double>();
        Dictionary<int, int> _bitSize = new Dictionary<int, int>();
        Dictionary<int, int> _bitIndex = new Dictionary<int, int>();
        Dictionary<int, List<int>> _mergedItems = new Dictionary<int, List<int>>(); //몇 개의 unit을 겹쳐 사용할 때, 그들의 이름을 저장.
        Dictionary<int, MswPos> _mswInMergedItem = new Dictionary<int, MswPos>();
        Dictionary<int, int> _parentUnitOfBitItem = new Dictionary<int, int>();
        List<int> _itemListInBuffer = new List<int>();
        List<int> _itemListForView = new List<int>();
        Dictionary<int, String> _description = new Dictionary<int, string>();
        Dictionary<int, String> _category = new Dictionary<int, string>();
        Dictionary<int, Func<Object,Object>> _setFunc = new Dictionary<int, Func<Object,Object>>();
        Dictionary<int, Func<Object,Object>> _getFunc = new Dictionary<int, Func<Object,Object>>();
        Dictionary<int, Func<Int64, Int64>> _setFuncInt = new Dictionary<int, Func<Int64, Int64>>();
        Dictionary<int, Func<Int64, Int64>> _getFuncInt = new Dictionary<int, Func<Int64, Int64>>();
        Dictionary<int, Func<Double, Double>> _setFuncFloat = new Dictionary<int, Func<Double, Double>>();
        Dictionary<int, Func<Double, Double>> _getFuncFloat = new Dictionary<int, Func<Double, Double>>();
        Dictionary<int, Boolean> _isUnsigned = new Dictionary<int, bool>();
        List<String> _names = null; //이 리스트를 초기화하지 말 것. null값이 중요한 값임.
        Dictionary<Type, String> _typeFormat = new Dictionary<Type, string>();
        Dictionary<int, String> _enumFormat = new Dictionary<int, string>();
        String _nameHeader = ""; //출력되는 이름 앞에 일률적으로 붙을 문자열
        Boolean _unsigned = false;
        Type _itemReturnType = typeof(int);
        int _beforeSetint;
        Dictionary<int, String> _unit = new Dictionary<int, string>(); //단위저장
        int _pragma=1;
        int _pragmaStart = 0;
        int _byteCount=0;
        int _bitItemIndex = 0;
        int _bitItemParent = -1;
        double _bitItemLSB = 1;
        Type _bitItemReturnType = null;
        #region 생성자

        /// <summary>
        /// bufferSize로 버퍼를 초기화합니다.
        /// </summary>
        /// <param name="bufferSize"></param>
        public NetworkPacketWith(int bufferSize):base(bufferSize)
        {
            _byteCount = 0;
        }
        
        /// <summary>
        /// 초기크기 없이 인스턴스를 만듭니다. 모든 item을 추가한 후에 반드시 setBuffSize를 호출해 주어야 합니다.
        /// </summary>
        public NetworkPacketWith()
        {
            _byteCount = 0;
        }

        #endregion

        /*
        #region IClonable 구현
        
        /// <summary>
        /// 해당 clonebase의 속성을 가져와서 자신에 복사함. 자기 자신의 레퍼런스가 바뀌지는 않음.
        /// </summary>
        /// <param name="cloneBase">복사할 정보를 가진 동일하거나 하위의 객체(캐스팅이 가능해야 함)</param>
        public virtual void ClonedBy(IClonable cloneBase)
        {

            base.ClonedBy(cloneBase); //부모의 속성을 일단 복사.
            try
            {//캐스팅 가능시 하위개체..
                NetworkPacketWith<T> obj = cloneBase as NetworkPacketWith<T>;

                this._beforeSetint = obj._beforeSetint;
                this._bitIndex = new Dictionary<int, int>(obj._bitIndex);
                this._bitSize = new Dictionary<int, int>(obj._bitSize);
                this._category = new Dictionary<int, string>(obj._category);
                this._description = new Dictionary<int, string>(obj._description);
                this._enumFormat = new Dictionary<int, string>(obj._enumFormat);
                this._getFunc = new Dictionary<int, Func<object, object>>(obj._getFunc);
                this._setFunc = new Dictionary<int, Func<object, object>>(obj._setFunc);
                this._getFuncFloat = new Dictionary<int, Func<double, double>>(obj._getFuncFloat);
                this._setFuncFloat = new Dictionary<int, Func<double, double>>(obj._setFuncFloat);
                this._getFuncInt = new Dictionary<int, Func<long, long>>(obj._getFuncInt);
                this._setFuncInt = new Dictionary<int, Func<long, long>>(obj._setFuncInt);
                this._sizeOfItemInBuffer = new Dictionary<int, int>(obj._sizeOfItemInBuffer);
                this._startByteOffsetInBuffer = new Dictionary<int, int>(obj._startByteOffsetInBuffer);
                this._itemListForView = new List<int>(obj._itemListForView);
                this._itemListInBuffer = new List<int>(obj._itemListInBuffer);
                this._LSB = new Dictionary<int, double>(obj._LSB);
                this._mergedItems = new Dictionary<int, List<int>>(obj._mergedItems);
                this._mswInMergedItem = new Dictionary<int, MswPos>(obj._mswInMergedItem);
                this._nameHeader = obj._nameHeader;
                this._names = obj._names;
                this._parentUnitOfBitItem = new Dictionary<int, int>(obj._parentUnitOfBitItem);
                this._pragma = obj._pragma;
                this._pragmaStart = obj._pragmaStart;
                this._returnType = new Dictionary<int, Type>(obj._returnType);
                this._typeFormat = new Dictionary<Type, string>(obj._typeFormat);
                this._unit = new Dictionary<int, string>(obj._unit);
            }
            catch (Exception e)
            {
                throw new Exception("NetworkObjectBase:: 내부버퍼(ArrayBuffer) copy시 오류발생" + e);
            }
        }

        #endregion
        */
        #region get functions
        
        /// <summary>
        /// 리턴타입으로 지정한 타입을 반환함.
        /// </summary>
        /// <param name="item">리턴타입을 알고자 하는 item</param>
        /// <returns></returns>
        public Type getReturnType(int item)
        {
            return _returnType[item];
        }

        /// <summary>
        /// getValues에서 가져갈 리스트 안에 있는 리스트라면 그 리스트상에서의 순서를 리턴함.
        /// 없으면 -1.
        /// </summary>
        /// <param name="item">보여질 리스트에 있는 item의 이름</param>
        /// <returns>리스트상에서의 순서. 없으면 -1</returns>
        public int getVisibleIndex(int item)
        {
            return (_itemListForView.Contains(item))? _itemListForView.IndexOf(item) : -1;
        }

        /// <summary>
        /// item이 버퍼 내에서 바이트단위로 어디에 속해있는지 알려준다.
        /// 만일 item이 mergedItem이거나 bitItem이라면 -1을 리턴한다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int getStartByteOffset(int item)
        {
            return _startByteOffsetInBuffer.ContainsKey(item) ? _startByteOffsetInBuffer[item] : -1;
        }

        /// <summary>
        /// item에 해당하는 이름을 가져간다. 미리 정해진 unit을 합쳐준다.
        /// setNames로 미리 이름을 지정한 것이 있다면 그 이름에서 가져간다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isWithUnit">단위를 합칠 것인지</param>
        /// <returns>enum의 이름을 가져온다.</returns>
        public String getName(int item, Boolean isWithUnit = true)
        {
            if (_names != null)
            {
                try
                {
                    return _names[_itemListForView.IndexOf(item)];
                }
                catch
                {
                    throw new Exception("setNames함수에서 지정한 이름의 개수가 맞지 않습니다.");
                }
            }
            string name;
            String unit = "";

            if (isWithUnit) unit = (_unit[item].Length > 0) ? " (" + _unit[item] + ")" : "";
            name = (item.ToString() + unit);

            return _nameHeader + name;
        }

        /// <summary>
        /// visible을 true로 지정하여(기본값) 셋팅한 요소의 이름을 리스트로 가져옵니다.
        /// 이 함수를 호출하기 전에 setNameHeader를 호출해 주면 getNames로 가져온 리스트에는
        /// 모두 그 헤더가 추가되어 나옵니다. 만일 리스트의 이름이 같아서 Dictionary등에 추가
        /// 할 때 에러가 날 가능성이 있으면 setNameHeader함수로 헤더를추가하십시오.
        /// </summary>
        /// <param name="isWithUnit">(단위)를 붙일지</param>
        /// <returns>이름 리스트</returns>
        public List<String> getNames(Boolean isWithUnit = true)
        {
            if (_names != null) return _names;
            List<String> names = new List<String>();
            String unit = "";
            foreach (int item in _itemListForView)
            {
                if (isWithUnit) unit = (_unit[item].Length > 0) ? " (" + _unit[item] + ")" : "";
                names.Add(_nameHeader + item.ToString() + unit);
            }
            return names;
        }


        public List<String> getAllNames(Boolean isWithUnit = true)
        {
            if (_names != null) return _names;
            List<String> names = new List<String>();
            String unit = "";
            foreach (int item in _itemListInBuffer)
            {
                if (isWithUnit) unit = (_unit[item].Length > 0) ? " (" + _unit[item] + ")" : "";
                names.Add(_nameHeader + item.ToString() + unit);
            }
            return names;
        }

        public int ItemsCount
        {
            get
            {
                return _itemListInBuffer.Count;
            }
        }

        public int viewItemsCount
        {
            get
            {
                return _itemListForView.Count;
            }
        }
        #endregion

        #region set or add functions 
        
        public void setNameHeader(String header)
        {
            _nameHeader = header;
        }

        /// <summary>
        /// 모든 item을 입력하고 난 뒤 호출해 주면 미리 nameList를 만들어둔다.
        /// getNameList를 할 때 따로 처리하지 않아서 처리 속도가 올라간다.
        /// </summary>
        /// <param name="isWithUnit"></param>
        public void setNameList(Boolean isWithUnit)
        {
            _names = new List<String>();
            String unit = "";
            foreach (int item in _itemListForView)
            {
                if (isWithUnit) unit = (_unit[item].Length > 0) ? " (" + _unit[item] + ")" : "";
                _names.Add(item.ToString() + unit);
            }
        }

        /// <summary>
        /// 특정 item에 개별적으로 카테고리를 정한다.이미 카테고리가 정해졌다면 갱신한다.
        /// </summary>
        /// <param name="item">카테고리를 정할 item</param>
        /// <param name="category">카테고리. </param>
        public void setCategory(int item, String category)
        {
            if (_category.ContainsKey(item)) _category[item] = category;
            else _category.Add(item, category);
        }




        /// <summary>
        /// setValue와 getValue를 할 때 추가로 처리할 함수를 지정한다.
        /// </summary>
        /// <param name="getFunc">getValue시 처리될 함수.</param>
        /// <param name="setFunc">setValue시 처리될 함수.</param>
        public void addFunctionRunningWhenGetOrSet(Func<Object, Object> getFunc, Func<Object, Object> setFunc = null)
        {
            if (getFunc != null) _getFunc.Add(_beforeSetint, getFunc);
            if (setFunc != null) _setFunc.Add(_beforeSetint, setFunc);
        }

        /// <summary>
        /// setValue와 getValue를 할 때 추가로 처리할 함수를 지정한다. double형
        /// </summary>
        /// <param name="getFunc">getValue시 처리될 함수.</param>
        /// <param name="setFunc">setValue시 처리될 함수.</param>
        public void addFunctionRunningWhenGetOrSetInt(Func<Int64, Int64> getFunc, Func<Int64, Int64> setFunc = null)
        {
            if (getFunc != null) _getFuncInt.Add(_beforeSetint, getFunc);
            if (setFunc != null) _setFuncInt.Add(_beforeSetint, setFunc);
        }

        /// <summary>
        /// setValue와 getValue를 할 때 추가로 처리할 함수를 지정한다. int64형
        /// </summary>
        /// <param name="getFunc">getValue시 처리될 함수.</param>
        /// <param name="setFunc">setValue시 처리될 함수.</param>
        public void addFunctionRunningWhenGetOrSetFloat(Func<Double, Double> getFunc, Func<Double, Double> setFunc = null)
        {
            if (getFunc != null) _getFuncFloat.Add(_beforeSetint, getFunc);
            if (setFunc != null) _setFuncFloat.Add(_beforeSetint, setFunc);
        }

        /// <summary>
        /// 동일한 리턴타입을 가졌다면 동일한 포멧을 적용시킨다. 개별포멧보다는 우선순위가 낮다.
        /// </summary>
        /// <param name="type">포멧을 지정할 리턴타입</param>
        /// <param name="format">포멧은 C#의 기본포멧인 {0:Xn},{0:Dn},{0:Fn}과 비트를 나태내는 타입인 {Bn[,seporator]},{Bn}{Dn}단체 가 있다.
        /// {Bn[,seporator]}은 하나만 적을 시에는 n개의 비트로 나뉜 비트문자열이 된다. 나뉨문자를 지정하고 싶을 때는 ,로 구분한 뒤 문자를 넣는다.
        /// 예) {B4,:} - :로 구분되어 4개씩 짝을 지어 보여줌 - 결과: 0000:0000:0000:0000
        /// {Bn}{Dn}단체 는 {Bn}과 {Dn}을 몇개 열거하여 원하는 포멧으로 표시한다.여기서 {Bn}은 n자리비트의 비트를 열거하며, {Dn}은 n자리의 비트로 구성된 정수를 나타낸다.
        /// 예) {B4},{D4}:{D4}:{B4} - 비트값이 0101010101010101일때 결과: 0101,3:3:0101
        /// .</param>
        public void setFormat(Type type, String format)
        {
            _typeFormat.Add(type, format);
        }



        /// <summary>
        /// 초기에 버퍼크기를 지정하지 않았거나 item들을 다 할당하고 나서 버퍼크기를 딱맞게 조절하고 싶을 때 호출한다.
        /// </summary>
        /// <param name="buffSize">비워두면 현재의 item들의 byteSize의 합으로 설정된다. 채우면 그 크기로 고정된다.</param>
        public override void setBuffSize(int buffSize = -1)
        {
            if (buffSize > 0) base.setBuffSize(buffSize);
            else
            {
                int typeBytes = Marshal.SizeOf(typeof(T));
                int totalSize = 0;
                foreach (int itemInBuffer in _sizeOfItemInBuffer.Keys)
                {
                    totalSize += _sizeOfItemInBuffer[itemInBuffer];
                }
                base.setBuffSize(totalSize/typeBytes);
            }
        }

        /// <summary>
        /// item들이 지정될 때 프라그마에 맞추어서 위치가 정해진다.
        /// 예를 들어 pragma를 4로 지정했다면, 1byte,2byte 크기의 item을 정하고 나서 남는 pragma는 1이 된다.
        /// 따라서 2byte이상의 item이 새로 잡힌다면 pragma에 의해서 1byte는 채워지고 새로 4byte의 시작지점에
        /// 할당된다.
        /// </summary>
        /// <param name="cutSize"></param>
        public void setPragma(int cutSize)
        {
            _pragma = cutSize;
            _pragmaStart = _byteCount;
        }
        #endregion

        #region add or set functions for enum items

        /// <summary>
        /// 버퍼상에 이름을 지정한다. 이 명령은 순서대로 버퍼상에 크기를 잡기 때문에 반드시 버퍼의 순서대로 호출해야 한다.
        /// </summary>
        /// <param name="enumItem">미리 지정한 enum의 이름</param>
        /// <param name="roomSize">버퍼상에 잡힐 크기.int타입이라면 1을 넣으면 4byte가 잡힌다.</param>
        /// <param name="isSignedRange">+와 -로 구성된 min/max값으로 정상값의 범위가 정해지면 true이다.\n+180/-180 사이의 값이 정상값이라면 true이고, \n+360/0 사이의 값이 정상값이라면 false이다.</param>
        /// <param name="max">범위의 최대값. +/- 를 가진 값이 정상값이라면 -값은 빼고 +최대값만 적는다. 대신 isSignedRange는 true로 해 주어야 한다. 이 값으로 LSB를 계산하여 getValue로 불러들여올 때 곱해진다.</param>
        /// <param name="isVisibleUnit">실제 목록구성시에 보일 이름인지 나타낸다. 기본값은 true.</param>
        /// <param name="returnType"></param>
        /// <param name="unit"></param>
        public void addItem(int enumItem, int roomSize, Boolean isSignedRange, double max, Boolean isVisibleUnit = true, Type returnType = null, String unit = "")
        {
            int byteSize = Marshal.SizeOf(typeof(T)) * roomSize;
            double lsb;
            if (byteSize == 1) lsb = (isSignedRange)? max/(Byte.MaxValue/2) : max/Byte.MaxValue;
            else if (byteSize == 2) lsb = (isSignedRange)? max/Int16.MaxValue : (max/2)/Int16.MaxValue;
            else if (byteSize == 4) lsb = (isSignedRange) ? max / Int32.MaxValue : (max / 2) / Int32.MaxValue;
            else //if(byteSize==8) 
                lsb = (isSignedRange) ? max / Int16.MaxValue : (max / 2) / Int16.MaxValue;
            setUnsigned(isSignedRange==false);
            addItem(enumItem, byteSize, isVisibleUnit, returnType, lsb, unit);
        }

        /// <summary>
        /// 버퍼상에 이름을 지정한다. 이 명령은 순서대로 버퍼상에 크기를 잡기 때문에 반드시 버퍼의 순서대로 호출해야 한다.
        /// </summary>
        /// <param name="enumItem">미리 지정한 enum의 이름</param>
        /// <param name="roomSize">버퍼상에 잡힐 크기.int타입이라면 1을 넣으면 4byte가 잡힌다.</param>
        /// <param name="isVisibleUnit">실제 목록구성시에 보일 이름인지 나타낸다. 기본값은 true.</param>
        /// <param name="returnType">getValue함수에서 불러들인 후 캐스팅할 타입이다.</param>
        /// <param name="LSB">getValue로 불러들일 때 곱해질 수이다.</param>
        /// <param name="unit">목록을 구성할 때 제목에표시되는 단위를 정해준다. 단위는 m/s 와 같이 쓰면 된다.</param>
        public void addItem(int enumItem, int roomSize, Boolean isVisibleUnit=true, Type returnType=null, double LSB = 1, String unit="")
        {
            int byteSize = Marshal.SizeOf(typeof(T)) * roomSize;
            if (returnType != null) _itemReturnType = returnType;
            _sizeOfItemInBuffer.Add(enumItem, byteSize);
            
            //pragma에 따라서 버퍼를 잘라줌.
            int indexInPragma = (_byteCount - _pragmaStart)%_pragma;
            if (indexInPragma!=0 && indexInPragma<_pragma && (indexInPragma+byteSize)>_pragma)
            {
                _byteCount += (_pragma - indexInPragma);
            }

            _startByteOffsetInBuffer.Add(enumItem, _byteCount); //버퍼 안에서 위치
            _itemListInBuffer.Add(enumItem); // 구조체목록에서의 순서
            _LSB.Add(enumItem, LSB); //getValue를 할 때 곱해질 값. 기본값은 1
            _returnType.Add(enumItem, _itemReturnType); //리턴되는 값의 타입. lsb가 곱해진후의 타입이다.
            _byteCount += byteSize; //버퍼상에서 다음에 입력될 위치
            _bitItemParent = enumItem; //이 명령 다음에 바로 bitItem이 입력되면 이 Item이 부모가 된다.
            _bitItemIndex = 0;
            _bitItemLSB = LSB;
            _bitItemReturnType = _itemReturnType;
            _unit.Add(enumItem, unit);
            if (isVisibleUnit) _itemListForView.Add(enumItem);
            _beforeSetint = enumItem;
            _isUnsigned[enumItem] = _unsigned;
        }

        /// <summary>
        /// 개별적으로 포멧을 지정한다.
        /// </summary>
        /// <param name="enumItem">포멧을 지정할 아이템</param>
        /// <param name="format">포멧은 C#의 기본포멧인 {0:Xn},{0:Dn},{0:Fn}과 비트를 나태내는 타입인 {Bn[,seporator]},{Bn}{Dn}단체 가 있다.
        /// {Bn[,seporator]}은 하나만 적을 시에는 n개의 비트로 나뉜 비트문자열이 된다. 나뉨문자를 지정하고 싶을 때는 ,로 구분한 뒤 문자를 넣는다.
        /// 예) {B4,:} - :로 구분되어 4개씩 짝을 지어 보여줌 - 결과: 0000:0000:0000:0000
        /// {Bn}{Dn}단체 는 {Bn}과 {Dn}을 몇개 열거하여 원하는 포멧으로 표시한다.여기서 {Bn}은 n자리비트의 비트를 열거하며, {Dn}은 n자리의 비트로 구성된 정수를 나타낸다.
        /// 예) {B4},{D4}:{D4}:{B4} - 비트값이 0101010101010101일때 결과: 0101,3:3:0101
        /// .</param>
        public void setFormat(int enumItem, String format)
        {
            _enumFormat.Add(enumItem, format);
        }

        /// <summary>
        /// 이 명령을 호출한 다음에 추가되는 item들의 타입은 unsigned/signed가 된다.
        /// </summary>
        /// <param name="isUnsigned">unsigned로 표시할 것인지</param>
        public void setUnsigned(Boolean isUnsigned)
        {
            _unsigned = isUnsigned;
        }

        /// <summary>
        /// setBitUnit() 함수 전에 bit count를 초기화한다.
        /// </summary>
        public void setBitItemInit()
        {
            _bitItemIndex = 0;
        }

        /// <summary>
        /// <BR>특정 버퍼를 나타내는 intItem에 비트단위로 잘라 새로운 이름을 부여한다. 이 명령은 따로 버퍼를 잡지는 않는다.</BR>
        /// <BR>다만, 이 명령을 호출하기 전에 setBitUnitInit() 명령을 호출하여 BitCounter를 초기화해야 한다.</BR>
        /// <BR>그렇게 하지 않으려면 비트로 자를 intItem을 선언하고 나서 바로 이 명령을 사용하여 Bit로 나누어주면 된다.</BR>
        /// ** 이 명령은 bitCount를 bitSize만큼씩 증가시킨다. 
        /// </summary>
        /// <param name="bitItem">Bit로 나눈 후 가지게 되는 이름</param>
        /// <param name="bitSize">Bit수</param>
        /// <param name="parentintUnit">부모 int. 실제 Buffer상에 할당된 이름을 적어야 한다. 부모int을 정의한 다음이라면 적지 않아도 된다.</param>
        /// <param name="isVisibleUnit">목록을 구성하면 나타날지 여부이다.</param>
        /// <param name="returnType">getValue명령으로 값을 가져갈 때 캐스팅할 타입이다.</param>
        /// <param name="LSB">값을 가져갈 때 곱해질 단위값이다.</param>
        /// <param name="unit">목록을 구성할 때 사용될 unit이다. cm, m, m/s 등을 써주면 목록구성시 (m)와 같은 식으로 추가된다.</param>
        public void setBitItem(int bitItem, int bitSize, Boolean isVisibleUnit = true, Type returnType = null, double LSB = Double.MinValue, String unit = "", int parentintUnit = -1)
        {
            setBitItem(bitItem, _bitItemIndex, bitSize, isVisibleUnit, returnType, LSB, unit, parentintUnit);
            _bitItemIndex += bitSize;
        }

        /// <summary>
        /// 특정 버퍼를 나타내는 intItem에 비트단위로 잘라 새로운 이름을 부여한다. 이 명령은 따로 버퍼를 잡지는 않는다.<BR/>
        /// 만일 setUnit명령을 호출한 이후라면 기본으로 그 버퍼를 비트로 나눌 것이다.<BR/>
        /// ** 이 명령은 bitCount를 증가시키지 않는다.
        /// </summary>
        /// <param name="bitItem">Bit로 나눈 후 가지게 되는 이름</param>
        /// <param name="bitIndex">Bit의 index</param>
        /// <param name="bitSize">Bit수</param>
        /// <param name="parentintUnit">부모 int. 실제 Buffer상에 할당된 이름을 적어야 한다. 부모int을 정의한 바로다음이라면 적지 않아도 된다.</param>
        /// <param name="isVisibleUnit">목록을 구성하면 나타날지 여부이다.</param>
        /// <param name="returnType">getValue명령으로 값을 가져갈 때 캐스팅할 타입이다.</param>
        /// <param name="LSB">값을 가져갈 때 곱해질 단위값이다.</param>
        /// <param name="unit">목록을 구성할 때 사용될 unit이다. cm, m, m/s 등을 써주면 목록구성시 (m)와 같은 식으로 추가된다.</param>
        public void setBitItem(int bitItem, int bitIndex, int bitSize, Boolean isVisibleUnit = true, Type returnType = null, double LSB = Double.MinValue, String unit = "", int parentintUnit = -1)
        {
            if (parentintUnit != -1)
            {
                _bitItemIndex = 0;
                _bitItemParent = parentintUnit;
            }
            if (LSB != Double.MinValue) { _bitItemLSB = LSB; } //LSB값을 반복 입력할 때 넣지 않아도 됨.
            if (returnType != null) _bitItemReturnType = returnType;
            _parentUnitOfBitItem.Add(bitItem, _bitItemParent);
            _bitSize.Add(bitItem, bitSize);
            _bitIndex.Add(bitItem, bitIndex);
            _returnType.Add(bitItem, _bitItemReturnType);
            _LSB.Add(bitItem, _bitItemLSB);
            _unit.Add(bitItem, unit);
            if (isVisibleUnit) _itemListForView.Add(bitItem);
            _beforeSetint = bitItem;
            _isUnsigned[bitItem] = _unsigned;
        }

        /// <summary>
        /// 항목을 두 개 이상 합쳐서 하나의 유닛이 되는 항목을 지정함. MSW,LSW를 지정할 때 유용함.
        /// </summary>
        /// <param name="mergeUnit">버퍼상 유닛이 합쳐져서 가지게 될 새로운 이름</param>
        /// <param name="mswPos">상위위치(MSW)의 버퍼의 0부터 시작하는 index. 합쳐지는 유닛이 4개이면 최대값은 3이다.MSW는 가장 높은 자리가 위치하는 버퍼상의 위치이다.</param>
        /// <param name="isVisibleUnit">리스트를 구성할 때 보여지는 항목인지 지정한다.</param>
        /// <param name="unit">단위. m 라고 적으면 리스트를 구성할 때 (m)로 자동으로 만들어진다.</param>
        /// <param name="mergingUnits">합쳐질 unit들</param>
        public void setMergedItem(int mergeUnit, MswPos mswPos, int startMerging, int numOfItems, Boolean isVisibleUnit = true, Type returnType = null, double lsb = 1, String unit = "")// params int[] mergingUnits)
        {
            //List<int> units = new List<int>();
            
            //units.AddRange(mergingUnits);
            int startIndex = _itemListInBuffer.IndexOf(startMerging);
            List<int> units=_itemListInBuffer.GetRange(startIndex, numOfItems);
            _mergedItems.Add(mergeUnit, units);
            _mswInMergedItem.Add(mergeUnit, mswPos);
            _unit.Add(mergeUnit, unit);
            if (isVisibleUnit) _itemListForView.Add(mergeUnit);
            _beforeSetint = mergeUnit;
            _isUnsigned[mergeUnit] = _unsigned;
            _LSB.Add(mergeUnit, lsb);
            _returnType.Add(mergeUnit, returnType);
            //_isUnsigned[mergeUnit] = _unsigned;
        }

        /// <summary>
        /// 이전에 추가한 아이템에 종속된 description을 추가한다. getDescription으로 각 item에 속한 설명을 가져올 수 있다.
        /// </summary>
        /// <param name="description">추가할 설명.</param>
        public void setDescription(String description)
        {
            _description.Add(_beforeSetint, description);
        }

        /// <summary>
        /// 각 item에 속한 description을 가져온다. 없을 경우 빈 문자를 리턴한다.
        /// </summary>
        /// <param name="item">item의 enum</param>
        /// <returns></returns>
        public String getDescription(int item)
        {
            if(_description.ContainsKey(item)) return _description[item];
            else return "";
        }

        /// <summary>
        /// item이 어떤 카테고리에 속해있는지 
        /// </summary>
        /// <param name="item">item이름 </param>
        /// <returns>카테고리</returns>
        public String getCategory(int item)
        {
            if (_category.ContainsKey(item)) return _category[item];
            else return "";
        }

        /// <summary>
        /// item이 속한 카테고리를 지정한다.
        /// 이후에 추가되는 item들은 모두 이 카테고리에 속하게 된다.
        /// </summary>
        /// <param name="category">지정할 카테고리</param>
        public void setCategory(String category)
        {
            _category.Add(_beforeSetint, category);
        }


        #endregion


        #region others

        /// <summary>
        /// 각 아이템들의 크기가 각자 다를 수 있기 때문에 이 함수를 사용하여 swap을 한다.
        /// </summary>
        /// <param name="startUnit">swap을 시작할 unit을 지정한다. 버퍼상에서의 위치는 </param>
        /// <param name="numOfUnits"></param>
        public void swapBuffer(int startUnit, int numOfUnits)
        {
            int startIndex = 0;
            int size = 0;
            
            
            foreach (int key in _itemListInBuffer)
            {
                startIndex = _startByteOffsetInBuffer[key];
                size = _sizeOfItemInBuffer[key];
                Swaper.swapWithSize(_initBuffer, _initBuffer, size, size, startIndex, startIndex);
            }

        }

        /// <summary>
        /// 각 아이템들의 크기가 각자 다를 수 있기 때문에 이 함수를 사용하여 swap을 한다.
        /// </summary>
        /// <param name="startUnit">swap을 시작할 unit을 지정한다. 버퍼상에서의 위치는 </param>
        /// <param name="numOfUnits"></param>
        public V getSwappedItem<V>(int key)
        {
            int startIndex = 0;
            int size = 0;


            startIndex = _startByteOffsetInBuffer[key];
            size = _sizeOfItemInBuffer[key];
            V[] value = new V[1];
            
            Swaper.swapWithSize(_initBuffer, value, size, size, startIndex, 0);

            return value[0];
        }

        public void setSwappedItem<V>(int key, V value)
        {
            int startIndex = 0;
            int size = 0;


            startIndex = _startByteOffsetInBuffer[key];
            size = _sizeOfItemInBuffer[key];
            V[] values = new V[1];
            values[0] = value;

            Swaper.swapWithSize(values, _initBuffer, size, size, 0, startIndex);
        }

        #endregion


        #region private and protected functions
        protected String bitHandling(String format, Int64 value, int byteSize )
        {
            List<String> inner = new List<String>();
           
            String newStr = "";
            Boolean isBracketOpen = false;
            for (int i = 0; i < format.Length; i++)
            {
                if (format[i] == '{'){
                    newStr += "[" + inner.Count + "]";
                    isBracketOpen = true;
                    inner.Add("");
                }
                else if (format[i] == '}')
                {
                    isBracketOpen = false;
                }
                else
                {
                    if (isBracketOpen)
                    {
                        inner[inner.Count - 1] += format[i];
                    }
                    else
                    {
                        newStr += format[i];
                    }
                }
            }
            if (inner.Count == 1) //그냥 B하나의 항목에 B4와 같이 되어있을 때는 그 숫자로 나누어서 표시해 준다.
            {
                String[] formats = inner[0].Split(",".ToCharArray());
                int sepSize = Int32.Parse(formats[0].Substring(1));
                String seperator = (formats.Length > 1) ? formats[1] : ":";
                if (byteSize == 1)
                    inner[0] = TypeHandling.getBinNumber((Byte)value, 8 * byteSize, seperator);
                if (byteSize == 2)
                    inner[0] = TypeHandling.getBinNumber((Int16)value, 8 * byteSize, seperator);
                if (byteSize == 4)
                    inner[0] = TypeHandling.getBinNumber((Int32)value, 8 * byteSize, seperator);
                if (byteSize == 8)
                    inner[0] = TypeHandling.getBinNumber((Int64)value, 8 * byteSize, seperator);

            }
            else
            {
                int bitCount = 0;
                for (int i = 0; i < inner.Count; i++)
                {
                    if (inner[i].Substring(0, 1).ToLower().Equals("b"))
                    {

                        int bitSize = Int32.Parse(inner[i].Substring(1));

                        if (byteSize == 1)
                            inner[i] = TypeHandling.getBinNumber(trimBit((Byte)value, (byteSize*8)-bitCount-bitSize, bitSize), bitSize);
                        if (byteSize == 2)
                            inner[i] = TypeHandling.getBinNumber(trimBit((Int16)value, (byteSize * 8) - bitCount - bitSize, bitSize), bitSize);
                        if (byteSize == 4)
                            inner[i] = TypeHandling.getBinNumber(trimBit((Int32)value, (byteSize * 8) - bitCount - bitSize, bitSize), bitSize);
                        if (byteSize == 8)
                            inner[i] = TypeHandling.getBinNumber(trimBit((Int64)value, (byteSize * 8) - bitCount - bitSize, bitSize), bitSize);
                        bitCount += bitSize;
                    }
                    else//D
                    {
                        int bitSize = Int32.Parse(inner[i].Substring(1));
                        if (byteSize == 1)
                            inner[i] = trimBit((Byte)value, (byteSize * 8)-bitCount-bitSize, bitSize).ToString();
                        if (byteSize == 2)
                            inner[i] = trimBit((Int16)value, (byteSize * 8) - bitCount - bitSize, bitSize).ToString();
                        if (byteSize == 4)
                            inner[i] = trimBit((Int32)value, (byteSize * 8) - bitCount - bitSize, bitSize).ToString();
                        if (byteSize == 8)
                            inner[i] = trimBit((Int64)value, (byteSize * 8) - bitCount - bitSize, bitSize).ToString();
                        bitCount += bitSize;
                    }
                }

            }
            for (int i = 0; i < inner.Count; i++)
            {
                newStr = newStr.Replace("[" + i + "]", inner[i]);
            }
            return newStr;

        }

        protected Array getArrayBySize(int unitByteSize, int numOfUnits=1, Boolean unsigned = false)
        {
            if (unsigned)
            {
                switch (unitByteSize)
                {
                    case 1:
                        return new byte[numOfUnits];
                    case 2:
                        return new short[numOfUnits];
                    case 4:
                        return new int[numOfUnits];
                    default://8
                        return new long[numOfUnits];
                }
            }
            else
            {
                switch (unitByteSize)
                {
                    case 1:
                        return new byte[numOfUnits];
                    case 2:
                        return new ushort[numOfUnits];
                    case 4:
                        return new uint[numOfUnits];
                    default://8
                        return new ulong[numOfUnits];
                }
            }

        }

        #region getWithLsb...s
        Int64 getWithLSBInt(int item, Int64 value)
        {
            if (_getFunc.ContainsKey(item))
            {
                value = (Int64)_getFunc[item].Invoke(value);
            }
            else if (_getFuncInt.ContainsKey(item))
            {
                value = (Int64)(_getFuncInt[item].Invoke((Int64)value));
            }
            else if (_getFuncFloat.ContainsKey(item))
            {
                value = (Int64)(_getFuncFloat[item].Invoke((Double)value));
            }

            return (Int64)(value * _LSB[item]);
        }
        Double getWithLSBFloat(int item, Int64 value)
        {
            int byteSize = _sizeOfItemInBuffer[item];
            if (_LSB[item] == 1)
            {
                if(byteSize==4){
                    Single[] a = new Single[1];
                    Buffer.BlockCopy(BitConverter.GetBytes(value), 0, a, 0, byteSize);
                    
                    if(_getFuncFloat.ContainsKey(item)) return (Double)(_getFuncFloat[item].Invoke((Double)a[0]));
                    else return a[0];
                }else{//double
                    Double[] a = new Double[1];
                    Buffer.BlockCopy(BitConverter.GetBytes(value), 0, a, 0, byteSize);

                    if (_getFuncFloat.ContainsKey(item)) return(Double)(_getFuncFloat[item].Invoke((Double)a[0]));
                    else return a[0];
                }
            }else{
                if (_getFunc.ContainsKey(item))
                {
                    value = (Int64)_getFunc[item].Invoke(value);
                }
                else if (_getFuncInt.ContainsKey(item))
                {
                    value = (Int64)(_getFuncInt[item].Invoke((Int64)value));
                }
                else if (_getFuncFloat.ContainsKey(item))
                {
                    value = (Int64)(_getFuncFloat[item].Invoke((Double)value));
                }
                return (value * _LSB[item]);
            }
        }
        Boolean getWithLSBBoolean(int item, Int64 value)
        {
            if (_getFunc.ContainsKey(item))
            {
                value = (Int64)_getFunc[item].Invoke(value);
            }
            else if (_getFuncInt.ContainsKey(item))
            {
                value = (Int64)(_getFuncInt[item].Invoke((Int64)value));
            }
            else if (_getFuncFloat.ContainsKey(item))
            {
                value = (Int64)(_getFuncFloat[item].Invoke((Double)value));
            }

            return (Boolean)(value > 0); //boolean은 lsb가 필요없음.
        }


        Object getWithLSB(int item, Int64 value)
        {
            if (_getFunc.ContainsKey(item))
            {
                value = (Int64)_getFunc[item].Invoke(value);
            }
            else if (_getFuncInt.ContainsKey(item))
            {
                value = (Int64)(_getFuncInt[item].Invoke((Int64)value));
            }
            else if (_getFuncFloat.ContainsKey(item))
            {
                value = (Int64)(_getFuncFloat[item].Invoke((Double)value));
            }

            if (_returnType[item] == typeof(Byte))
            {
                return (Byte)(value * (Int64)_LSB[item]);
            }
            else if (_returnType[item] == typeof(Int16))
            {
                return (Int16)(value * (Int64)_LSB[item]);
            }
            else if (_returnType[item] == typeof(Int32))
            {
                return (Int32)(value * (Int64)_LSB[item]);
            }
            else if (_returnType[item] == typeof(Int64))
            {
                return (Int64)(value * (Int64)_LSB[item]);
            }
            else if (_returnType[item] == typeof(Single))
            {
                return (Single)(value * (Double)_LSB[item]);
            }
            else if (_returnType[item] == typeof(Double))
            {
                return (Double)(value * (Double)_LSB[item]);
            }
            else if (_returnType[item] == typeof(Boolean))
            {
                return (Boolean)((value * (Int64)_LSB[item]) > 0);
            }
            else
            {
                return ((value * (Double)_LSB[item]) > 0);
            }
        }
        #endregion

        #region setWithLsb...s
        void setWithLSB(int unit, float value, int typeSize)
        {
            setWithLSB(unit, (double)value, typeSize);
        }
        void setWithLSB(int unit, double value, int typeSize)
        {
            if (_LSB[unit] == 1)
            {
                TypeArrayConverter.FillBufferUnitsFromTrimed<double>(value, typeSize, _initBuffer, _startByteOffsetInBuffer[unit], 1);
            }
            else TypeArrayConverter.FillBufferUnitsFromTrimed<Int64>((Int64)(value * (1 / _LSB[unit])), typeSize, _initBuffer, _startByteOffsetInBuffer[unit], 1);
        }

        void setWithLSB(int unit, Byte value, int typeSize)
        {
            setWithLSB(unit, (Int64)value, typeSize);
        }

        void setWithLSB(int unit, Int16 value, int typeSize)
        {
            setWithLSB(unit, (Int64)value, typeSize);
        }

        void setWithLSB(int unit, Int32 value, int typeSize)
        {
            setWithLSB(unit, (Int64)value, typeSize);
        }
        void setWithLSB(int unit, Int64 value, int typeSize)
        {
            TypeArrayConverter.FillBufferUnitsFromTrimed<Int64>((Int64)(value * (1 / _LSB[unit])), typeSize, _initBuffer, _startByteOffsetInBuffer[unit], 1);
        }
        #endregion

        #endregion

        #region getValue functions

        /// <summary>
        /// 보여지는 것으로 셋팅한 값을 리스트로 리턴합니다.
        /// 어떤 타입이더라도 Object형식의 리스트에 넣어서 줍니다. 
        /// .ToString()함수를 붙여서 String으로 이용하거나 알맞은 타입으로 캐스팅해 주어야 합니다.
        /// 기본적으로 정수형은 Int64형이고, 실수형은 Double형으로 들어갑니다. 따로 Boolean형도 
        /// 지정이 됩니다.
        /// 예를 들어, 만일 정수형으로 지정하였다면 (Int64)getValues()[0] 와 같이 캐스팅 하여 사용하십시오.
        /// </summary>
        /// <returns>Object형으로 Boxing된 값들의목록</returns>
        public List<Object> getValues()
        {
            List<Object> values = new List<Object>();
            foreach (int item in _itemListForView)
            {
                if (_LSB[item] == 1)
                {
                    if (TypeHandling.isIntType(_returnType[item])) values.Add(getValueInt(item));
                    else if (TypeHandling.isFloatType(_returnType[item])) values.Add(getValueFloat(item));
                    else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean))) values.Add((getValueInt(item) > 0));
                    //names.Add(getValueInt(item));
                }
                else values.Add((getValueInt(item)*_LSB[item]));
                /*
                if (TypeHandling.isIntType(_returnType[item])) names.Add(getValueInt(item));
                else if (TypeHandling.isFloatType(_returnType[item])) names.Add(getValueFloat(item));
                else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean))) names.Add((getValueInt(item) > 0));
            */
            }
            return values;
        }
        
        public List<Object> getAllRawValues()
        {
            List<Object> values = new List<Object>();
            foreach (int item in _itemListInBuffer)
            {
                if (_LSB[item] == 1)
                {
                    if (TypeHandling.isIntType(_returnType[item])) values.Add(getValueInt(item));
                    else if (TypeHandling.isFloatType(_returnType[item])) values.Add(getValueFloat(item));
                    else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean))) values.Add((getValueInt(item) > 0));
                    else values.Add(getValueInt(item));
                }
                else values.Add((getValueInt(item)));
                /*
                if (TypeHandling.isIntType(_returnType[item])) names.Add(getValueInt(item));
                else if (TypeHandling.isFloatType(_returnType[item])) names.Add(getValueFloat(item));
                else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean))) names.Add((getValueInt(item) > 0));
            */
            }
            return values;
        }

        public List<Object> getAllValues()
        {
            List<Object> values = new List<Object>();
            foreach (int item in _itemListInBuffer)
            {
                if (_LSB[item] == 1)
                {
                    if (TypeHandling.isIntType(_returnType[item])) values.Add(getValueInt(item));
                    else if (TypeHandling.isFloatType(_returnType[item])) values.Add(getValueFloat(item));
                    else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean))) values.Add((getValueInt(item) > 0));
                    else values.Add(getValueInt(item));
                }
                else values.Add((getValueInt(item) * _LSB[item]));
                /*
                if (TypeHandling.isIntType(_returnType[item])) names.Add(getValueInt(item));
                else if (TypeHandling.isFloatType(_returnType[item])) names.Add(getValueFloat(item));
                else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean))) names.Add((getValueInt(item) > 0));
            */
            }
            return values;
        }


        /// <summary>
        /// 원하는 목록을 모두 Int64형의 리스트로 가져옵니다.
        /// 이 목록들은 사용자가 int형이라고 알고 있는 값이어야 합니다.
        /// 만일 타입이 다르다면 예상하지 못한 결과가 나올 수 있습니다.
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public List<Int64> getValuesInt(IEnumerable<int> itemList)
        {
            List<Int64> items = new List<long>();
            foreach (int item in itemList)
            {
                if (_LSB[item] == 1) items.Add(getValueInt(item));
                else items.Add((Int64)(getValueInt(item)*_LSB[item]));
            }
            return items;
        }

        public List<Double> getValuesFloat(IEnumerable<int> itemList)
        {
            List<Double> items = new List<Double>();
            foreach (int item in itemList)
            {
                items.Add(getValueInt(item)*_LSB[item]);
            }
            return items;
        }

        /// <summary>
        /// 값의 배열을 String리스트로 받아옴.
        /// </summary>
        /// <param name="changedValue">정규화된 값이 아닌 사용자가 원하는 값을 넣고 싶을 때, 항목의 이름과 값의 쌍으로 넣어주면 리스트에서 해당위치에 들어간다.</param>
        /// <returns>Visible로 선택한 모든 값이 리스트로 리턴됨.</returns>
        public List<String> getValuesAsString(Dictionary<int, String> changedValue = null)
        {
            List<String> values = new List<String>();
            foreach (int item in _itemListForView)
            {
                if (changedValue != null) //바뀔 값이 있다면 각각 지정해 줄 수도 있다.
                {
                    if (changedValue.ContainsKey(item))
                    {
                        values.Add(changedValue[item]);
                        continue;
                    }
                }
                values.Add(getValueAsFormat(item));

            }
            return values;
        }
        public String getValueAsFormat(int item)
        {
            String format;

            if (_enumFormat.ContainsKey(item)) format = _enumFormat[item];
            else if (_typeFormat.ContainsKey(_returnType[item])) format = _typeFormat[_returnType[item]];
            else format = "";

            if (format.Length == 0)
            {
                if (_LSB[item] == 1)
                {
                    if (TypeHandling.isIntType(_returnType[item])) return ((int)(getValueInt(item) * _LSB[item])).ToString();
                    else if (TypeHandling.isFloatType(_returnType[item])) return getValueFloat(item).ToString();
                    else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean))) return (getValueInt(item) > 0).ToString();
                    else return String.Format("{0:X}", getValueInt(item));
                    //names.Add(getValueInt(item));
                }
                else return (getValueInt(item) * _LSB[item]).ToString();
                /*
                if (TypeHandling.isIntType(_returnType[item]))
                {
                    return getValueInt(item).ToString();
                }
                else if (TypeHandling.isFloatType(_returnType[item]))
                {
                    return getValueFloat(item).ToString();
                }
                else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean)))
                {
                    return (getValueInt(item) > 0).ToString();
                }
                else
                {
                    return String.Format("{0:X}", getValueInt(item));
                }
                 */
            }
            else if (format.IndexOf("B") >= 0)
            {
                return bitHandling(format, (Int64)getValueInt(item), _sizeOfItemInBuffer[item]);

            }
            else
            {
                if (_returnType[item] == typeof(Byte))
                {
                    return String.Format(format, (Byte)getValueInt(item));
                }
                else if (_returnType[item] == typeof(short))
                {
                    return String.Format(format, (short)getValueInt(item));
                }
                else if (_returnType[item] == typeof(int))
                {
                    return String.Format(format, (int)getValueInt(item));
                }
                else if (_returnType[item] == typeof(Int64))
                {
                    return String.Format(format, (Int64)getValueInt(item));
                }
                else if (_returnType[item] == typeof(ushort))
                {
                    return String.Format(format, (ushort)getValueInt(item));
                }
                else if (_returnType[item] == typeof(uint))
                {
                    return String.Format(format, (uint)getValueInt(item));
                }
                else if (_returnType[item] == typeof(UInt64))
                {
                    return String.Format(format, (UInt64)getValueInt(item));
                }
                else if (TypeHandling.isFloatType(_returnType[item]))
                {
                    if (_LSB[item] == 1) return String.Format(format, getValueFloat(item));
                    else return String.Format(format, getValueInt(item)*_LSB[item]);
                }
                else if (TypeHandling.isSameTypeInList(_returnType[item], typeof(Boolean)))
                {
                    return String.Format(format, (getValueInt(item) > 0).ToString());
                }
                else
                {
                    throw new Exception("타입이정해지지 않았음");
                    return null;
                    //return String.Format("0x{0:X}", getValueInt(item));
                }
            }
        }
        
        /// <summary>
        /// 해당되는 unit의 값을 가져옴. 알맞는 타입으로 캐스팅해서 쓰면 된다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        public Int64 getValueInt(int item, Endians endian = Endians.Little)
        {
            int startOffset = 0;
            int byteSize = 0;
            int bitStart = -1;
            int bitSize = -1;
            int returnSize = Marshal.SizeOf(_returnType[item]);
            bool unsigned = _isUnsigned[item];
            Int64 value=0;

            //Int64[] buff = new Int64[1];
            //buff[0] = 0;

            if (item.ToString().IndexOf("PR") >= 0)
            {
            }
            if (_startByteOffsetInBuffer.ContainsKey(item))//버퍼상에 실제 할당되는 unit
            {
                byteSize = _sizeOfItemInBuffer[item];
                startOffset = _startByteOffsetInBuffer[item];
                switch (byteSize)
                {
                    case 1:
                        value = TypeArrayConverter.CopyBufferToVariable<byte>(_initBuffer, startOffset, byteSize, unsigned);

                        break;
                    case 2:
                        if (unsigned) value = TypeArrayConverter.CopyBufferToVariable<ushort>(_initBuffer, startOffset, byteSize, unsigned);
                        else value = TypeArrayConverter.CopyBufferToVariable<short>(_initBuffer, startOffset, byteSize, unsigned);
                        break;
                    case 4:

                        if (unsigned) value = TypeArrayConverter.CopyBufferToVariable<uint>(_initBuffer, startOffset, byteSize, unsigned);
                        else value = TypeArrayConverter.CopyBufferToVariable<int>(_initBuffer, startOffset, byteSize, unsigned);
                        break;
                    case 8:
                        if (unsigned) value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, unsigned);
                        else value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, unsigned);
                        break;
                }
                


                //return value;// (item, value);
            }
            else if (_bitIndex.ContainsKey(item)) //bit 분리된 unit
            {
                int parent = _parentUnitOfBitItem[item];
                byteSize = _sizeOfItemInBuffer[parent];
                startOffset = _startByteOffsetInBuffer[parent];
                bitStart = _bitIndex[item];
                bitSize = _bitSize[item];
                switch (byteSize)
                {
                    case 1:
                        value = TypeArrayConverter.CopyBufferToVariable<byte>(_initBuffer, startOffset, byteSize, true);
                        break;
                    case 2:
                        value = TypeArrayConverter.CopyBufferToVariable<short>(_initBuffer, startOffset, byteSize, true);
                        break;
                    case 4:
                        value = TypeArrayConverter.CopyBufferToVariable<int>(_initBuffer, startOffset, byteSize, true);
                        break;
                    case 8:
                        value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, true);
                        break;
                }

                //value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, true); //비트로 나뉠 것이므로 unsigned타입이 되어야 한다.
                value = trimBit(value, byteSize * 8 - bitStart - bitSize, bitSize);// getWithLSBInt(item, trimBit(value, byteSize * 8 - bitStart - bitSize, bitSize));
            }
            else //merged unit
            {
                List<int> mergingUnits = _mergedItems[item];
                int unitSize = _sizeOfItemInBuffer[mergingUnits[0]];//unit 하나의 크기
                byteSize = unitSize * mergingUnits.Count; //전체크기
                startOffset = _startByteOffsetInBuffer[mergingUnits[0]];
                int startIndex = startOffset / Marshal.SizeOf(typeof(T));

                if (_mswInMergedItem[item] == MswPos.After && endian == Endians.Little)
                {
                    value = UnitsSwapToWithSize(startIndex, mergingUnits.Count * unitSize, unitSize);// getWithLSBInt(item, UnitsSwapToWithSize(startIndex, mergingUnits.Count, byteSize));
                }
                else if (_mswInMergedItem[item] == MswPos.Before && endian == Endians.Big)
                {
                    value = UnitsSwapToWithSize(startIndex, mergingUnits.Count * unitSize, unitSize);
                }
                else
                {
                    value = UnitsToWithSize(startIndex, mergingUnits.Count * unitSize, unitSize);// getWithLSBInt(item, UnitsToWithSize(startIndex, mergingUnits.Count, byteSize));
                }
            }

            if (_getFunc.ContainsKey(item))
            {
                value = (Int64)_getFunc[item].Invoke(value);
            }
            else if (_getFuncInt.ContainsKey(item))
            {
                value = (Int64)(_getFuncInt[item].Invoke((Int64)value));
            }
            else if (_getFuncFloat.ContainsKey(item))
            {
                value = (Int64)(_getFuncFloat[item].Invoke((Double)value));
            }

            return value;

        }
        

        public void getValue(int item, ref int value,Endians endian = Endians.Little)
        {
            value = (int)getValueInt(item, endian);
        }
        public void getValue(int item, ref Int64 value, Endians endian = Endians.Little)
        {
            value = getValueInt(item,  endian);
        }
        public void getValue(int item, ref Single value, Endians endian = Endians.Little)
        {
            value = (Single)getValueFloat(item, endian);
        }
        public void getValue(int item, ref Double value, Endians endian = Endians.Little)
        {
            value = getValueFloat(item, endian);
        }
        public void getValue(int item, ref Boolean value)
        {
            value = getValueInt(item) > 0;
        }


        /// <summary>
        /// 해당되는 unit의 값을 가져옴. 알맞는 타입으로 캐스팅해서 쓰면 된다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        public Double getValueFloat(int item, Endians endian= Endians.Little)
        {
            int startOffset = 0;
            int byteSize = 0;
            int bitStart = -1;
            int bitSize = -1;
            int returnSize = Marshal.SizeOf(_returnType[item]);

            bool unsigned = _isUnsigned[item];
            
           // byteSize = _sizeOfItemInBuffer[item];
           // startOffset = _startByteOffsetInBuffer[item];
            //Int64[] buff = new Int64[1];
            Int64 value=0;

            if (_startByteOffsetInBuffer.ContainsKey(item))//버퍼상에 실제 할당되는 unit
            {
                byteSize = _sizeOfItemInBuffer[item];
                startOffset = _startByteOffsetInBuffer[item];
                

                if (_LSB[item] == 1)
                {
                    if (byteSize == 4) return TypeArrayConverter.CopyBufferToVariable<Single>(_initBuffer, startOffset, byteSize);
                    else return TypeArrayConverter.CopyBufferToVariable<Double>(_initBuffer, startOffset, byteSize);
                }
                else
                {
                    if (unsigned==false)
                    {
                        switch (byteSize)
                        {
                            case 1:
                                value = TypeArrayConverter.CopyBufferToVariable<byte>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                            case 2:
                                value = TypeArrayConverter.CopyBufferToVariable<short>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                            case 4:
                                value = TypeArrayConverter.CopyBufferToVariable<int>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                            case 8:
                                value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                        }
                    }
                    else
                    {
                        switch (byteSize)
                        {
                            case 1:
                                value = TypeArrayConverter.CopyBufferToVariable<byte>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                            case 2:
                                value = TypeArrayConverter.CopyBufferToVariable<ushort>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                            case 4:
                                value = TypeArrayConverter.CopyBufferToVariable<uint>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                            case 8:
                                value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, unsigned);
                                break;
                        }
                    }
                   //
                   // value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize);
                    return value * _LSB[item];// getWithLSBFloat(item, value);
                }
            }
            else if (_bitIndex.ContainsKey(item)) //bit 분리된 unit //비트 아이템은 무조건 uint다.
            {
                int parent = _parentUnitOfBitItem[item];
                byteSize = _sizeOfItemInBuffer[parent];
                startOffset = _startByteOffsetInBuffer[parent];
                bitStart = _bitIndex[item];
                bitSize = _bitSize[item];
                if (unsigned==false)
                {
                    switch (byteSize)
                    {
                        case 1:
                            value = TypeArrayConverter.CopyBufferToVariable<byte>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                        case 2:
                            value = TypeArrayConverter.CopyBufferToVariable<short>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                        case 4:
                            value = TypeArrayConverter.CopyBufferToVariable<int>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                        case 8:
                            value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                    }
                }
                else
                {
                    switch (byteSize)
                    {
                        case 1:
                            value = TypeArrayConverter.CopyBufferToVariable<byte>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                        case 2:
                            value = TypeArrayConverter.CopyBufferToVariable<ushort>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                        case 4:
                            value = TypeArrayConverter.CopyBufferToVariable<uint>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                        case 8:
                            value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize, unsigned);
                            break;
                    }
                }

                if (_LSB[item] == 1)
                {

                    //value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize);
                    Int64[] temp = new Int64[1];
                    temp[0] = trimBit(value, byteSize * 8 - bitStart - bitSize, bitSize);
                    return TypeArrayConverter.CopyBufferToVariable<Double>(temp, 0, sizeof(Int64));
                    //return trimBit(value, byteSize * 8 - bitStart - bitSize, bitSize);

                    //return  TypeArrayConverter.CopyBufferToVariable<Double>(_initBuffer, startOffset, byteSize);
                }
                else
                {

                    //value = TypeArrayConverter.CopyBufferToVariable<Int64>(_initBuffer, startOffset, byteSize);
                    return trimBit(value, byteSize * 8 - bitStart - bitSize, bitSize) * _LSB[item] ;//getWithLSBFloat(item, trimBit(value, byteSize * 8 - bitStart - bitSize, bitSize));
                }
            }
            else //merged unit
            {
                List<int> mergingUnits = _mergedItems[item];
                int unitSize = _sizeOfItemInBuffer[mergingUnits[0]];//unit 하나의 크기
                byteSize = unitSize * mergingUnits.Count; //전체크기
                startOffset = _startByteOffsetInBuffer[mergingUnits[0]];
                int startIndex = startOffset / Marshal.SizeOf(typeof(T));
                if (_LSB[item] == 1)
                {

                    //value = TypeArrayConverter.copyBuffToVariable(_initBuffer, startOffset, byteSize);

                    if (_mswInMergedItem[item] == MswPos.Before && endian == Endians.Little)
                    {
                        return UnitsSwapToWithSize(startIndex, mergingUnits.Count, byteSize);// getWithLSBFloat(item, UnitsSwapToWithSize(startIndex, mergingUnits.Count, byteSize));
                    }
                    else
                    {
                        return UnitsToWithSize(startIndex, mergingUnits.Count, byteSize) ;// getWithLSBFloat(item, UnitsToWithSize(startIndex, mergingUnits.Count, byteSize));
                    }
                }
                else
                {
                    //value = TypeArrayConverter.copyBuffToVariable(_initBuffer, startOffset, byteSize);

                    if (_mswInMergedItem[item] == MswPos.Before && endian == Endians.Little)
                    {
                        return UnitsSwapToWithSize(startIndex, mergingUnits.Count, byteSize) * _LSB[item];// getWithLSBFloat(item, UnitsSwapToWithSize(startIndex, mergingUnits.Count, byteSize));
                    }
                    else
                    {
                        return UnitsToWithSize(startIndex, mergingUnits.Count, byteSize) * _LSB[item];// getWithLSBFloat(item, UnitsToWithSize(startIndex, mergingUnits.Count, byteSize));
                    }
                }
            }

        }
        #endregion

        #region setValue functions

        public void setValue(int item, Int32 value, Endians endian)
        {
            setValue(item, (Int64)value, endian);
        }
        public void setValue(int item, Int16 value, Endians endian)
        {
            setValue(item, (Int64)value, endian);
        }
        public void setValue(int item, Byte value, Endians endian)
        {
            setValue(item, (Int64)value, endian);
        }

        public void setValue(int item, Single value, Endians endian)
        {
            setValue(item, (Double)value, endian);
        }
        public void setValue(int item, Double value, Endians endian)
        {
            if (_setFunc.ContainsKey(item))
            {
                value = (Double)(_setFunc[item].Invoke(value));
            }
            else if (_setFuncInt.ContainsKey(item))
            {
                value = (Double)(_setFuncInt[item].Invoke((Int64)value));
            }
            else if (_setFuncFloat.ContainsKey(item))
            {
                value = (Double)(_setFuncFloat[item].Invoke((Double)value));
            }

            int startByteOffset = 0;
            int byteSize = 0;

            if (_startByteOffsetInBuffer.ContainsKey(item))//버퍼상에 실제 할당되는 unit
            {
                byteSize = _sizeOfItemInBuffer[item];
                setWithLSB(item, value, byteSize);
            }
            else if (_bitIndex.ContainsKey(item)) //bit 분리된 unit
            {
                int parent = _parentUnitOfBitItem[item];
                byteSize = _sizeOfItemInBuffer[parent];
                startByteOffset = _startByteOffsetInBuffer[parent];
                int bitStart = _bitIndex[item];
                int bitSize = _bitSize[item];
                Int64 ivalue = (Int64)(value * (1 / _LSB[item]));

                addBitToBuffer(_initBuffer, startByteOffset, bitStart, ivalue, bitSize); //이 부분만 바뀌었음.
            }
            else //merged unit
            {
                List<int> mergingUnits = _mergedItems[item];
                int unitSize = _sizeOfItemInBuffer[mergingUnits[0]];//unit 하나의 크기
                byteSize = unitSize * mergingUnits.Count; //전체크기
                startByteOffset = _startByteOffsetInBuffer[mergingUnits[0]];
                value = (Int64)(value * (1 / _LSB[item]));
                Byte[] buff = BitConverter.GetBytes(value);


                if (_mswInMergedItem[item] == MswPos.Before && endian == Endians.Little)
                {//원래 little endian에서는 msw가 뒤에 가야 하지만 before에 있으므로 각요소들의 위치를 바꾸어야한다.
                    copyBufferUnSwapBasedSwapFromArray(buff, startByteOffset, byteSize, byteSize, unitSize);
                }
                else
                {
                    Buffer.BlockCopy(buff, 0, _initBuffer, startByteOffset, byteSize);
                }
            }
        }


        /// <summary>
        /// 값을 입력합니다. 비트아이템이면 비트를 입력하고 mergedItem이면 여러개의 버퍼item에
        /// 동시에 값이 입력됩니다.
        /// </summary>
        /// <param name="item">등록된 item의 이름</param>
        /// <param name="value">입력할 값</param>
        /// <param name="endian">endian을 정함. 이 값은 mergedItem일때만 유효함. little endian인데 msw가 버퍼의 뒤에 위치한다면 위치를 조정해 줍니다.</param>
        public void setValue(int item, Int64 value, Endians endian = Endians.Little)
        {
            if (_setFunc.ContainsKey(item))
            {
                value = (Int64)(_setFunc[item].Invoke(value));
            }
            else if (_setFuncInt.ContainsKey(item))
            {
                value = (Int64)(_setFuncInt[item].Invoke((Int64)value));
            }
            else if (_setFuncFloat.ContainsKey(item))
            {
                value = (Int64)(_setFuncFloat[item].Invoke((Double)value));
            }
            int startIndex = 0;
            int byteSize = 0;

            if (_startByteOffsetInBuffer.ContainsKey(item))//버퍼상에 실제 할당되는 unit
            {
                byteSize = _sizeOfItemInBuffer[item];
                setWithLSB(item, value, byteSize); 
            }
            else if (_bitIndex.ContainsKey(item)) //bit 분리된 unit
            {
                int parent = _parentUnitOfBitItem[item];
                byteSize = _sizeOfItemInBuffer[parent] ;
                startIndex = _startByteOffsetInBuffer[parent];
                int bitStart = _bitIndex[item];
                int bitSize = _bitSize[item];
                value = (Int64)( value * (1 / _LSB[item]));

                addBitToBuffer(_initBuffer, startIndex, bitStart, value, bitSize);
            }
            else //merged unit
            {
                List<int> mergingUnits = _mergedItems[item];
                int unitSize = _sizeOfItemInBuffer[mergingUnits[0]];//unit 하나의 크기
                byteSize = unitSize * mergingUnits.Count; //전체크기
                startIndex = _startByteOffsetInBuffer[mergingUnits[0]];
                value = (Int64)(value * (1 / _LSB[item]));
                Byte[] buff = BitConverter.GetBytes(value);
                

                if (_mswInMergedItem[item] == MswPos.Before && endian == Endians.Little) 
                {//원래 little endian에서는 msw가 뒤에 가야 하지만 before에 있으므로 각요소들의 위치를 바꾸어야한다.
                    copyBufferUnSwapBasedSwapFromArray(buff, startIndex, byteSize, byteSize, unitSize);
                }
                else
                {
                    Buffer.BlockCopy(buff, 0, _initBuffer, startIndex, byteSize);
                }
            }
        }
        #endregion
        
          #region ////////////// Overrides /////////////////

        public override Byte[] getByteBufferSwapCopied(int offset=0)
        {
            byte[] dst = new byte[this.bufferByteSize];

            foreach (int item in _itemListInBuffer)
            {
                Swaper.swapWithSize(_initBuffer, dst, _sizeOfItemInBuffer[item], _sizeOfItemInBuffer[item], _startByteOffsetInBuffer[item]+offset, _startByteOffsetInBuffer[item]);
            }
            return dst;
        }

        public override void copyBufferSwapFromArray(Array srcBuffer, int srcOffset = 0, int size = -1, int swapSize = -1, int dstOffset=0)
        {
            if (size < 0)
            {
                if (Buffer.ByteLength(srcBuffer) > Buffer.ByteLength(this.buffer)) size = Buffer.ByteLength(this.buffer);
                else size = Buffer.ByteLength(srcBuffer);
            }
            int srcSize = Buffer.ByteLength(srcBuffer);
                foreach (int item in _itemListInBuffer)
                {
                    if (_startByteOffsetInBuffer[item] + srcOffset + _sizeOfItemInBuffer[item] <= srcSize)  //버퍼가 넘어버렸을 때
                        Swaper.swapWithSize(srcBuffer, _initBuffer, _sizeOfItemInBuffer[item], _sizeOfItemInBuffer[item], _startByteOffsetInBuffer[item] + srcOffset, _startByteOffsetInBuffer[item] + dstOffset);
                    else break;
                }
            
        }

        #endregion ///////////////////////////////////////
    }
}
