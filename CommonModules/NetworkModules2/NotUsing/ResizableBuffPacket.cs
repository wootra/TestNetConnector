using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkModules;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace NetworkModules.NotUsing
{
    [Serializable]
    public class ResizableBuffPacket:NetworkPacket
    {
        int _initByteSize;

        public ResizableBuffPacket(int initByteSize)
            : base(initByteSize)
        {
            _initByteSize = initByteSize;
        
        }

        public ResizableBuffPacket()
        {
         

        }
        public new void setBuffSize(int buffSize = -1)
        {
            base.setBuffSize(buffSize);
            _initByteSize = Buffer.ByteLength(ArrayBuffer);
        }

        public override int ChildOffset
        {
            get
            {
                return _initByteSize;
            }
        }

        /// <summary>
        /// 추가된 NetworkObject형식의 버퍼는 리스트로 관리됩니다.
        /// index부터 시작하여 그 이후의 추가된 버퍼를 삭제합니다.
        /// </summary>
        /// <param name="index">추가된 순서입니다.</param>
        public void cutChidrenAfter(int index)
        {
            Children.RemoveRange(index, Children.Count - index);
            reSizeBuffer();
        }

        /// <summary>
        /// _initBuffer의 크기를 다시 조정합니다.
        /// 조정된 크기는 buffer 프로퍼티에 바로 반영됩니다.
        /// </summary>
        public void reSizeBuffer()
        {
            int childByteSize = 0;

            foreach (INetPacket child in Children)
            {
                childByteSize += child.bufferByteSize;//.bufferByteSize;
            }

            Byte[] newBuff = new Byte[_initByteSize + childByteSize];
            int minSize = 0;
            if (_initBuffer != null && _initBuffer.Length > 0)
                minSize = _initBuffer.Length < newBuff.Length ? _initBuffer.Length : newBuff.Length;

            _initBuffer.CopyTo(newBuff, 0);
            
            int offset = _initByteSize;
            int childSize = 0;
            int totalSize = 0;
            foreach (INetPacket child in Children)
            {
                childSize = child.bufferByteSize;
                Buffer.BlockCopy(child.ArrayBuffer, 0, newBuff, offset + totalSize, childSize);
                totalSize += childSize;
            }
            _initBuffer = newBuff;
        }

        /// <summary>
        /// NetworkObject 형식의 추가버퍼의 배열이나 리스트를 추가합니다.
        /// </summary>
        /// <param name="children">배열형식이나 List형식의 NetworkObject파생 클래스모음</param>
        public void addChildren(IEnumerable<Array> children)
        {
            Children.AddRange(children);
            reSizeBuffer();
        }

        /// <summary>
        /// NetworkObject 형식의 추가버퍼를 추가합니다.
        /// </summary>
        /// <param name="children">NetworkObject파생 클래스</param>
        public void addChild(Array child)
        {
            Children.Add(child);
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
                return Children[index];
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
            if (index > Children.Count)
            {
                throw new Exception("순차적으로 입력되지 않았습니다.");
            }

            if (Children.Count - 1 < index)
            {
                addChild(child);
            }
            else
            {
                Children[index] = child;
            }
        }
    }
}
