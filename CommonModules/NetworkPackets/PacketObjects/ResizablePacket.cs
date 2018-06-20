using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace NetworkPacket
{
    /// <summary>
    /// 이 클래스는 동적으로 버퍼의 크기가 바뀌는 클래스이다.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ResizablePacket<T>:NetworkPacketWith<T>,INetPacket
    {
        //List<Array> _children = new List<Array>();

        int _initLength;

        public ResizablePacket(int initSize)
            : base(initSize)
        {
            _initLength = initSize;
        
        }

        [Browsable(false)]
        public int InitSize { get { return _initLength*Marshal.SizeOf(typeof(T)); } }
        /*
        public override void ClonedBy(Object cloneBase)
        {
            base.ClonedBy(cloneBase);
            try
            {//캐스팅 가능시 하위개체..
                ResizablePacket<T> obj = cloneBase as ResizablePacket<T>;
                this._children = new List<Array>(obj._children);
            }
            catch (Exception e)
            {
                throw new Exception("NetworkObjectBase:: 내부버퍼(ArrayBuffer) copy시 오류발생" + e);
            }
        }
        */
        public override List<Array> Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children.Clear();
                foreach (Array item in value)
                {
                    _children.Add(item.Clone() as Array);
                }
            }
        }

        public override int ChildOffset
        {
            get
            {
                return _initLength;
            }
        }

        /// <summary>
        /// T type을 리턴합니다.
        /// </summary>
        /// <returns></returns>
        [Category("Core Type")]
        [Browsable(false)]
        public Type ItemType { get { return typeof(T); } }


        /// <summary>
        /// 현재 버퍼의 index에 값을 집어넣습니다.
        /// </summary>
        /// <param name="index">버퍼의 인덱스입니다.</param>
        /// <param name="value">버퍼에 들어갈 값</param>
        public void setInitBuffer(int index, T value)
        {
            _initBuffer[index] = value;
        }

        /// <summary>
        ///  버퍼에서 값을 가져옵니다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T getInitBuffer(int index)
        {
            return _initBuffer[index];
        }
        /// <summary>
        /// 추가된 NetworkObject형식의 버퍼는 리스트로 관리됩니다.
        /// index부터 시작하여 그 이후의 추가된 버퍼를 삭제합니다.
        /// </summary>
        /// <param name="index">추가된 순서입니다.</param>
        public void cutChidrenAfter(int index)
        {
            _children.RemoveRange(index, _children.Count - index);
            reSizeBuffer();
        }

        /// <summary>
        /// _initBuffer의 크기를 다시 조정합니다.
        /// 조정된 크기는 buffer 프로퍼티에 바로 반영됩니다.
        /// </summary>
        public void reSizeBuffer()
        {
            int childBytes = 0;
            int typeSize = Marshal.SizeOf(typeof(T));
            foreach (Array child in _children)
            {
                childBytes += Buffer.ByteLength(child);//.bufferByteSize;
            }

            T[] newBuff = new T[_initLength + childBytes/typeSize];

            if (_initBuffer != null && _initBuffer.Length > 0) Buffer.BlockCopy(_initBuffer, 0, newBuff, 0, Buffer.ByteLength(_initBuffer));

            int offset = _initLength * typeSize;
            int childSize = 0;
            int totalSize = 0;
            foreach (Array child in _children)
            {
                childSize = Buffer.ByteLength(child);
                Buffer.BlockCopy(child, 0, newBuff, offset + totalSize, childSize);
                totalSize += childSize;
            }
            _initBuffer = newBuff;
        }

        /// <summary>
        /// 각각의 Child의 값들은 이 함수를 호출해주기 전까지는 독립적으로 존재한다.
        /// </summary>
        public void refreshBuffer()
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            int offset = _initLength * typeSize;
            int childSize = 0;
            int totalSize = 0;
            foreach (Array child in _children)
            {
                childSize = Buffer.ByteLength(child);
                Buffer.BlockCopy(child, 0, _initBuffer, offset + totalSize, childSize);
                totalSize += childSize;
            }
        }

        /// <summary>
        /// NetworkObject 형식의 추가버퍼의 배열이나 리스트를 추가합니다.
        /// </summary>
        /// <param name="children">배열형식이나 List형식의 NetworkObject파생 클래스모음</param>
        public void addChildren(IEnumerable<Array> children)
        {
            _children.AddRange(children);
            reSizeBuffer();
        }

        /// <summary>
        /// NetworkObject 형식의 추가버퍼를 추가합니다.
        /// </summary>
        /// <param name="children">NetworkObject파생 클래스</param>
        public void addChild(Array child)
        {
            _children.Add(child);
            reSizeBuffer();
        }

        /// <summary>
        /// 추가된 버퍼를 index로 검색하여 불러옵니다.
        /// </summary>
        /// <param name="index">추가된 버퍼가 추가된 순서.0부터 시작되는 index입니다.</param>
        /// <returns>NetworkObject파생형식의 추가버퍼를 리턴합니다.</returns>
        public Array getChild(int index)
        {
            try
            {
                return _children[index];
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// 이미 정의된 추가버퍼의 내용을 바꾸거나 차례대로 입력할 때 쓰입니다.
        /// 차례대로 입력한다면 없는 내용은 추가가 될 것입니다. 하지만 순서를
        /// 건너뛰어서 입력한다면 예외가 발생할 것입니다.
        /// </summary>
        /// <param name="index">입력할 index</param>
        /// <param name="child">수정하거나 추가할 NetworkObject형식의 추가버퍼</param>
        public void setChild(int index, Array child)
        {
            if (index > _children.Count)
            {
                throw new Exception("순차적으로 입력되지 않았습니다.");
            }

            if (_children.Count - 1 < index)
            {
                addChild(child);
            }
            else
            {
                _children[index] = child;
            }
        }
    }
}
