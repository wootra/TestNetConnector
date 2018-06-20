using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DataHandling;
using RtwEnums.Network;

namespace NetworkModules3.NotUsing
{
    [Serializable]
    public class NetworkPacket : ResizableNetworkObject<byte>,INetPacket
    {

        // constructor
        Dictionary<Enum, int> _startOffsetInBuffer = new Dictionary<Enum, int>();
        Dictionary<Enum, int> _sizeOfItemInBuffer = new Dictionary<Enum, int>();
        Dictionary<Enum, Type> _returnType = new Dictionary<Enum, Type>();
        Dictionary<Enum, double> _LSB = new Dictionary<Enum, double>();
        Dictionary<Enum, int> _bitSize = new Dictionary<Enum, int>();
        Dictionary<Enum, int> _bitIndex = new Dictionary<Enum, int>();
        Dictionary<Enum, List<Enum>> _mergedItems = new Dictionary<Enum, List<Enum>>(); //몇 개의 unit을 겹쳐 사용할 때, 그들의 이름을 저장.
        Dictionary<Enum, MswPos> _mswInMergedItem = new Dictionary<Enum, MswPos>();
        Dictionary<Enum, Enum> _parentUnitOfBitItem = new Dictionary<Enum, Enum>();
        List<Enum> _itemIndexInBuffer = new List<Enum>();
        List<Enum> _showList = new List<Enum>();
        /// <summary>
        /// 단위를 저장하는 공간
        /// </summary>
        Dictionary<Enum, String> _unit = new Dictionary<Enum, string>();
        int _pragma=1;
        int _pragmaStart = 0;
        int _count=0;

        public NetworkPacket(int bufferSize):base(bufferSize)
        {
            _count = 0;
        }
        
        public NetworkPacket()
        {
            _count = 0;
            
        }

        public new void setBuffSize(int buffSize = -1)
        {
            if (buffSize > 0) base.setBuffSize(buffSize);
            else
            {
                int totalSize = 0;
                foreach (Enum itemInBuffer in _sizeOfItemInBuffer.Keys)
                {
                    totalSize += _sizeOfItemInBuffer[itemInBuffer];
                }
                setBuffSize(totalSize);
            }
        }

        public void setPragma(int cutSize)
        {
            _pragma = cutSize;
            _pragmaStart = _count;
        }




        /// <summary>
        /// 버퍼상에 이름을 지정한다. 이 명령은 순서대로 버퍼상에 크기를 잡기 때문에 반드시 버퍼의 순서대로 호출해야 한다.
        /// </summary>
        /// <param name="enumItem">미리 지정한 enum의 이름</param>
        /// <param name="byteSize">버퍼상에 잡힐 크기</param>
        /// <param name="isSignedRange">+와 -로 구성된 min/max값으로 정상값의 범위가 정해지면 true이다.\n+180/-180 사이의 값이 정상값이라면 true이고, \n+360/0 사이의 값이 정상값이라면 false이다.</param>
        /// <param name="max">범위의 최대값. +/- 를 가진 값이 정상값이라면 -값은 빼고 +최대값만 적는다. 대신 isSignedRange는 true로 해 주어야 한다. 이 값으로 LSB를 계산하여 getValue로 불러들여올 때 곱해진다.</param>
        /// <param name="isVisibleUnit">실제 목록구성시에 보일 이름인지 나타낸다. 기본값은 true.</param>
        /// <param name="returnType"></param>
        /// <param name="unit"></param>
        public void addItem(Enum enumItem, int byteSize, Boolean isSignedRange, double max, Boolean isVisibleUnit = true, Type returnType = null, String unit = "")
        {
            double lsb;
            if (byteSize == 1) lsb = (isSignedRange)? max/(Byte.MaxValue/2) : max/Byte.MaxValue;
            else if (byteSize == 2) lsb = (isSignedRange)? max/Int16.MaxValue : (max/2)/Int16.MaxValue;
            else if (byteSize == 4) lsb = (isSignedRange) ? max / Int32.MaxValue : (max / 2) / Int32.MaxValue;
            else //if(byteSize==8) 
                lsb = (isSignedRange) ? max / Int16.MaxValue : (max / 2) / Int16.MaxValue;
            addItem(enumItem, byteSize, isVisibleUnit, returnType, lsb, unit);
        }
        
        Type _itemReturnType = typeof(int);
        /// <summary>
        /// 버퍼상에 이름을 지정한다. 이 명령은 순서대로 버퍼상에 크기를 잡기 때문에 반드시 버퍼의 순서대로 호출해야 한다.
        /// </summary>
        /// <param name="enumItem">미리 지정한 enum의 이름</param>
        /// <param name="byteSize">버퍼상에 잡힐 크기</param>
        /// <param name="isVisibleUnit">실제 목록구성시에 보일 이름인지 나타낸다. 기본값은 true.</param>
        /// <param name="returnType">getValue함수에서 불러들인 후 캐스팅할 타입이다.</param>
        /// <param name="LSB">getValue로 불러들일 때 곱해질 수이다.</param>
        /// <param name="unit">목록을 구성할 때 제목에표시되는 단위를 정해준다. 단위는 m/s 와 같이 쓰면 된다.</param>
        public void addItem(Enum enumItem, int byteSize, Boolean isVisibleUnit=true, Type returnType=null, double LSB = 1, String unit="")
        {
            if (returnType != null) _itemReturnType = returnType;
            _sizeOfItemInBuffer.Add(enumItem, byteSize);
            
            //pragma에 따라서 버퍼를 잘라줌.
            int indexInPragma = (_count - _pragmaStart)%_pragma;
            if (indexInPragma!=0 && indexInPragma<_pragma && (indexInPragma+byteSize)>_pragma)
            {
                _count += (_pragma - indexInPragma);
            }

            _startOffsetInBuffer.Add(enumItem, _count); //버퍼 안에서 위치
            _itemIndexInBuffer.Add(enumItem); // 버퍼 안에서의 순서
            _LSB.Add(enumItem, LSB); //getValue를 할 때 곱해질 값. 기본값은 1
            _returnType.Add(enumItem, _itemReturnType); //리턴되는 값의 타입. lsb가 곱해진후의 타입이다.
            _count += byteSize; //버퍼상에서 다음에 입력될 위치
            _bitItemParent = enumItem; //이 명령 다음에 바로 bitItem이 입력되면 이 Item이 부모가 된다.
            _bitItemIndex = 0;
            _bitItemLSB = LSB;
            _bitItemReturnType = _itemReturnType;
            _unit.Add(enumItem, unit);
            if (isVisibleUnit) _showList.Add(enumItem);
        }

        int _bitItemIndex = 0;
        Enum _bitItemParent = null;
        double _bitItemLSB = 1;
        Type _bitItemReturnType = null;

        /// <summary>
        /// setBitUnit() 함수 전에 bit count를 초기화한다.
        /// </summary>
        public void setBitItemInit()
        {
            _bitItemIndex = 0;
        }

        /// <summary>
        /// <BR>특정 버퍼를 나타내는 EnumItem에 비트단위로 잘라 새로운 이름을 부여한다. 이 명령은 따로 버퍼를 잡지는 않는다.</BR>
        /// <BR>다만, 이 명령을 호출하기 전에 setBitUnitInit() 명령을 호출하여 BitCounter를 초기화해야 한다.</BR>
        /// <BR>그렇게 하지 않으려면 비트로 자를 EnumItem을 선언하고 나서 바로 이 명령을 사용하여 Bit로 나누어주면 된다.</BR>
        /// ** 이 명령은 bitCount를 bitSize만큼씩 증가시킨다. 
        /// </summary>
        /// <param name="bitItem">Bit로 나눈 후 가지게 되는 이름</param>
        /// <param name="bitSize">Bit수</param>
        /// <param name="parentEnumUnit">부모 Enum. 실제 Buffer상에 할당된 이름을 적어야 한다. 부모Enum을 정의한 다음이라면 적지 않아도 된다.</param>
        /// <param name="isVisibleUnit">목록을 구성하면 나타날지 여부이다.</param>
        /// <param name="returnType">getValue명령으로 값을 가져갈 때 캐스팅할 타입이다.</param>
        /// <param name="LSB">값을 가져갈 때 곱해질 단위값이다.</param>
        /// <param name="unit">목록을 구성할 때 사용될 unit이다. cm, m, m/s 등을 써주면 목록구성시 (m)와 같은 식으로 추가된다.</param>
        public void setBitItem(Enum bitItem, int bitSize, Boolean isVisibleUnit = true, Type returnType = null, double LSB = Double.MinValue, String unit = "", Enum parentEnumUnit = null)
        {
            if (parentEnumUnit != null)
            {
                _bitItemIndex = 0;
                _bitItemParent = parentEnumUnit;
            }
            if (LSB != Double.MinValue) { _bitItemLSB = LSB; } //LSB값을 반복 입력할 때 넣지 않아도 됨.
            if (returnType != null) _bitItemReturnType = returnType;
            _parentUnitOfBitItem.Add(bitItem, _bitItemParent);
            _bitSize.Add(bitItem, bitSize);
            _bitIndex.Add(bitItem, _bitItemIndex);
            _bitItemIndex += bitSize;
            _returnType.Add(bitItem, _bitItemReturnType);
            _LSB.Add(bitItem, _bitItemLSB);
            _unit.Add(bitItem, unit);
            if (isVisibleUnit) _showList.Add(bitItem);
        }

        /// <summary>
        /// 특정 버퍼를 나타내는 EnumItem에 비트단위로 잘라 새로운 이름을 부여한다. 이 명령은 따로 버퍼를 잡지는 않는다.<BR/>
        /// 만일 setUnit명령을 호출한 이후라면 기본으로 그 버퍼를 비트로 나눌 것이다.<BR/>
        /// ** 이 명령은 bitCount를 증가시키지 않는다.
        /// </summary>
        /// <param name="bitItem">Bit로 나눈 후 가지게 되는 이름</param>
        /// <param name="bitIndex">Bit의 index</param>
        /// <param name="bitSize">Bit수</param>
        /// <param name="parentEnumUnit">부모 Enum. 실제 Buffer상에 할당된 이름을 적어야 한다. 부모Enum을 정의한 바로다음이라면 적지 않아도 된다.</param>
        /// <param name="isVisibleUnit">목록을 구성하면 나타날지 여부이다.</param>
        /// <param name="returnType">getValue명령으로 값을 가져갈 때 캐스팅할 타입이다.</param>
        /// <param name="LSB">값을 가져갈 때 곱해질 단위값이다.</param>
        /// <param name="unit">목록을 구성할 때 사용될 unit이다. cm, m, m/s 등을 써주면 목록구성시 (m)와 같은 식으로 추가된다.</param>
        public void setBitItem(Enum bitItem, int bitIndex, int bitSize, Boolean isVisibleUnit = true, Type returnType = null, double LSB = Double.MinValue, String unit = "", Enum parentEnumUnit = null)
        {
            if (parentEnumUnit != null)
            {
                _bitItemIndex = 0;
                _bitItemParent = parentEnumUnit;
            }
            if (LSB != Double.MinValue) { _bitItemLSB = LSB; } //LSB값을 반복 입력할 때 넣지 않아도 됨.
            if (returnType != null) _bitItemReturnType = returnType;
            _parentUnitOfBitItem.Add(bitItem, _bitItemParent);
            _bitSize.Add(bitItem, bitSize);
            _bitIndex.Add(bitItem, bitIndex);
            _returnType.Add(bitItem, _bitItemReturnType);
            _LSB.Add(bitItem, _bitItemLSB);
            _unit.Add(bitItem, unit);
            if (isVisibleUnit) _showList.Add(bitItem);
        }

        public void setMergedItem(Enum mergeUnit, MswPos mswPos, Boolean isVisibleUnit = true, String unit = "", params Enum[] mergingUnits)
        {
            List<Enum> units = new List<Enum>();
            units.AddRange(mergingUnits);
            _mswInMergedItem.Add(mergeUnit, mswPos);
            _unit.Add(mergeUnit, unit);
            if (isVisibleUnit) _showList.Add(mergeUnit);
        }

        public int getStartIndex(Enum item)
        {
            return _startOffsetInBuffer[item];
        }
 



        public void swapBuffer(Enum startUnit, int numOfUnits)
        {
            int startIndex = 0;
            int size = 0;
            
            
            foreach (Enum key in _itemIndexInBuffer)
            {
                startIndex = _startOffsetInBuffer[key];
                size = _sizeOfItemInBuffer[key];
                Swaper.swapWithSize(_initBuffer, _initBuffer, size, size, startIndex, startIndex);
            }

        }
        public List<String> getNames(Boolean isWithUnit=true)
        {
            List<String> names = new List<String>();
            String unit = "";
            foreach (Enum item in _showList)
            {
                if(isWithUnit) unit =  (_unit[item].Length>0)? " ("+_unit[item]+")":"";
                names.Add(item.ToString() + unit);
            }
            return names;
        }
        public List<Object> getValues()
        {
            List<Object> names = new List<Object>();
            foreach (Enum item in _showList)
            {
                names.Add(getValue(item));
            }
            return names;
        }
        public List<String> getValuesAsString()
        {
            List<String> names = new List<String>();
            foreach (Enum item in _showList)
            {
                names.Add(getValue(item).ToString());
            }
            return names;
        }


        /// <summary>
        /// 해당되는 unit의 값을 가져옴. 알맞는 타입으로 캐스팅해서 쓰면 된다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        public Object getValue(Enum item, Endians endian= Endians.Little)
        {
            int startOffset = 0;
            int byteSize = 0;
            int bitStart = -1;
            int bitSize = -1;
            int returnSize = Marshal.SizeOf(_returnType[item]);
            
            if (_startOffsetInBuffer.ContainsKey(item))//버퍼상에 실제 할당되는 unit
            {
                byteSize = _sizeOfItemInBuffer[item];
                startOffset = _startOffsetInBuffer[item];
                if (byteSize == 1)
                {
                    Byte[] aByte = new Byte[1];
                    Buffer.BlockCopy(_initBuffer, startOffset, aByte, 0, 1);
                    return getWithLSB(item, aByte[0]);
                }
                else if (byteSize == 2)
                {
                    return getWithLSB(item, BitConverter.ToInt16(_initBuffer, startOffset));
                }
                else if (byteSize == 4)
                {
                    return getWithLSB(item, BitConverter.ToInt32(_initBuffer, startOffset));
                }
                else //if (byteSize == 8)
                {
                    return getWithLSB(item, BitConverter.ToInt64(_initBuffer, startOffset));
                    
                }
            }
            else if (_bitIndex.ContainsKey(item)) //bit 분리된 unit
            {
                Enum parent = _parentUnitOfBitItem[item];
                byteSize = _sizeOfItemInBuffer[parent];
                startOffset = _startOffsetInBuffer[parent];
                bitStart = _bitIndex[item];
                bitSize = _bitSize[item];

                if (byteSize == 1)
                {
                    return getWithLSB(item, trimBit(_initBuffer[startOffset], bitStart, bitSize));
                }
                else if (byteSize == 2)
                {
                    return getWithLSB(item, trimBit(BitConverter.ToInt16(_initBuffer, startOffset), bitStart, bitSize));
                }
                else if (byteSize == 4)
                {
                    return getWithLSB(item, trimBit(BitConverter.ToInt32(_initBuffer, startOffset), bitStart, bitSize));
                }
                else// if (byteSize == 8)
                {
                    if (bitSize > 32) return getWithLSB(item, trimBit(BitConverter.ToInt64(_initBuffer, startOffset), bitStart, bitSize));
                    else return getWithLSB(item, trimBit(BitConverter.ToInt32(_initBuffer, startOffset), bitStart, bitSize));
                }
            }
            else //merged unit
            {
                List<Enum> mergingUnits = _mergedItems[item];
                int unitSize = _sizeOfItemInBuffer[mergingUnits[0]];//unit 하나의 크기
                byteSize = unitSize * mergingUnits.Count; //전체크기
                startOffset = _startOffsetInBuffer[mergingUnits[0]];
                
                
                if (_mswInMergedItem[item] == MswPos.Before && endian== Endians.Little)
                {
                    if(byteSize==2){

                        return getWithLSB(item, UnitsSwapTo<Int16>(startOffset,startOffset+1));
                        
                    }else if(byteSize==4){
                        return getWithLSB(item, UnitsSwapTo<Int32>(startOffset, startOffset + 1));
                        
                    }else if(byteSize==8){
                        return getWithLSB(item, UnitsSwapTo<Int64>(startOffset, startOffset + 1));
                        
                    }else{//지원하지 않음. 1은 에러.
                        throw new Exception("지원하지 않는크기. mergedUnit은 최소한 2이상이고 8이하여야 함");
                    }
                }
                else
                {
                    if (byteSize == 2)
                    {
                        return getWithLSB(item, UnitsTo<Int16>(startOffset, startOffset + 1));
                    }
                    else if (byteSize == 4)
                    {
                        return getWithLSB(item, UnitsTo<Int16>(startOffset, startOffset + 1));
                    }
                    else if (byteSize == 8)
                    {
                        return getWithLSB(item, UnitsTo<Int16>(startOffset, startOffset + 1));
                    }
                    else
                    {//지원하지 않음. 1은 에러.
                        throw new Exception("지원하지 않는크기. mergedUnit은 최소한 2이상이고 8이하여야 함");
                    }

                }
            }

        }
        Object getWithLSB(Enum item, Int64 value)
        {
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
                return (Boolean)((value * (Int64)_LSB[item])>0);
            }
            else
            {
                return ((value * (Double)_LSB[item]) > 0);
            }
        }

        public void setValue(Enum item, Int32 value, Endians endian)
        {
            setValue(item, (Int64)value, endian);
        }
        public void setValue(Enum item, Int16 value, Endians endian)
        {
            setValue(item, (Int64)value, endian);
        }
        public void setValue(Enum item, Byte value, Endians endian)
        {
            setValue(item, (Int64)value, endian);
        }

        public void setValue(Enum item, Single value, Endians endian)
        {
            setValue(item, (Double)value, endian);
        }
        public void setValue(Enum item, Double value, Endians endian)
        {
            int startIndex = 0;
            int byteSize = 0;

            if (_startOffsetInBuffer.ContainsKey(item))//버퍼상에 실제 할당되는 unit
            {
                byteSize = _sizeOfItemInBuffer[item];
                setWithLSB(item, value, byteSize);
            }
            else if (_bitIndex.ContainsKey(item)) //bit 분리된 unit
            {
                Enum parent = _parentUnitOfBitItem[item];
                byteSize = _sizeOfItemInBuffer[parent];
                startIndex = _startOffsetInBuffer[parent];
                int bitStart = _bitIndex[item];
                int bitSize = _bitSize[item];
                Int64 ivalue = (Int64)(value * (1 / _LSB[item]));

                addBitToBuffer(_initBuffer, startIndex, bitStart, ivalue, bitSize); //이 부분만 바뀌었음.
            }
            else //merged unit
            {
                List<Enum> mergingUnits = _mergedItems[item];
                int unitSize = _sizeOfItemInBuffer[mergingUnits[0]];//unit 하나의 크기
                byteSize = unitSize * mergingUnits.Count; //전체크기
                startIndex = _startOffsetInBuffer[mergingUnits[0]];
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

        public void setValue(Enum item, Int64 value, Endians endian = Endians.Little)
        {

            int startIndex = 0;
            int byteSize = 0;

            if (_startOffsetInBuffer.ContainsKey(item))//버퍼상에 실제 할당되는 unit
            {
                byteSize = _sizeOfItemInBuffer[item];
                setWithLSB(item, value, byteSize); 
            }
            else if (_bitIndex.ContainsKey(item)) //bit 분리된 unit
            {
                Enum parent = _parentUnitOfBitItem[item];
                byteSize = _sizeOfItemInBuffer[parent];
                startIndex = _startOffsetInBuffer[parent];
                int bitStart = _bitIndex[item];
                int bitSize = _bitSize[item];
                value = (Int64)( value * (1 / _LSB[item]));

                addBitToBuffer(_initBuffer, startIndex, bitStart, value, bitSize);
            }
            else //merged unit
            {
                List<Enum> mergingUnits = _mergedItems[item];
                int unitSize = _sizeOfItemInBuffer[mergingUnits[0]];//unit 하나의 크기
                byteSize = unitSize * mergingUnits.Count; //전체크기
                startIndex = _startOffsetInBuffer[mergingUnits[0]];
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

         void setWithLSB(Enum unit, float value, int typeSize)
        {
            setWithLSB(unit, (double)value, typeSize);
        }
         void setWithLSB(Enum unit, double value, int typeSize)
        {
            TypeArrayConverter.FillBufferUnitsFromTrimed<Int64>((Int64)(value * (1 / _LSB[unit])), typeSize, _initBuffer, _startOffsetInBuffer[unit], 1);
        }

         void setWithLSB(Enum unit, Byte value, int typeSize)
        {
            setWithLSB(unit, (Int64)value, typeSize);
        }

         void setWithLSB(Enum unit, Int16 value, int typeSize)
        {
            setWithLSB(unit, (Int64)value, typeSize);
        }

         void setWithLSB(Enum unit, Int32 value, int typeSize)
        {
            setWithLSB(unit, (Int64)value, typeSize);
        }
         void setWithLSB(Enum unit, Int64 value, int typeSize)
        {

            TypeArrayConverter.FillBufferUnitsFromTrimed<Int64>((Int64)(value *(1/_LSB[unit])), typeSize, _initBuffer, _startOffsetInBuffer[unit], 1);
            
        }

        public Type getType(Enum unit)
        {
            return _returnType[unit];
        }
        #region ////////////// Overrides /////////////////
        public override Byte[] getByteBufferSwapCopied(int offset)
        {
            byte[] dst = new byte[this.bufferByteSize];

            foreach (Enum item in _itemIndexInBuffer)
            {
                Swaper.swapWithSize(_initBuffer, dst, _sizeOfItemInBuffer[item], _sizeOfItemInBuffer[item], _startOffsetInBuffer[item]+offset, _startOffsetInBuffer[item]);
            }
            return dst;
        }

        public void copyBufferSwapFromArray(Array srcBuffer, int srcOffset = 0, int size = -1, int dstOffset = 0)
        {
            if (size < 0)
            {
                if (Buffer.ByteLength(srcBuffer) > Buffer.ByteLength(this.buffer)) size = Buffer.ByteLength(this.buffer);
                else size = Buffer.ByteLength(srcBuffer);
            }

            foreach (Enum item in _itemIndexInBuffer)
            {
                Swaper.swapWithSize(srcBuffer, _initBuffer, _sizeOfItemInBuffer[item], _sizeOfItemInBuffer[item], _startOffsetInBuffer[item]+srcOffset, _startOffsetInBuffer[item]+dstOffset);
            }

        }

        #endregion ///////////////////////////////////////
    }
}
