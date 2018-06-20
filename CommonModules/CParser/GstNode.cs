using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomParser
{
   [Serializable]
    public class GstNode
    {
        public String TypeName="";
        public String Name="";
        public String Key = "";
        public IOTStruct IOT;
        public ContextKind Kind;
        public DataType Type;
        public Direction Direction = Direction.Input;
        public GstNode _parent=null;
        public Object Value = null;
        public List<GstNode> _child = new List<GstNode>();
        public Dictionary<String, GstNode> _gstKeyDic = new Dictionary<String, GstNode>();
        public Dictionary<String, GstNode> _iotKeyDic = new Dictionary<string, GstNode>();
        public String[] DataTypeStr = (new String[]{
                "char", "short", "int","long long", "long",
                "unsigned char", "unsigned short", "unsigned int", "unsigned long long", "unsigned long",
                "char*" , 
                "float", "double"
            });
        public Type[] DataTypeType = (new Type[]{
                typeof(sbyte),  typeof(short),  typeof(int), typeof(int),  typeof(long), 
                typeof(byte), typeof(ushort), typeof(uint), typeof(uint), typeof(ulong),
                typeof(string) , 
                typeof(float), typeof(double)
            });
        public String[] DataTypeStr2 = (new String[]{
                "char", "short", "int","long2", "long",
                "uchar", "ushort", "uint", "ulong2", "ulong",
                "str" , 
                "float", "double"
            });

        /// <summary>
        /// struct 형이 아닌 일반형을 정의할 때 쓰임.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public GstNode(DataType type, String name, GstNode parent = null)
        {
            this.Name = name;
            this.Kind = ContextKind.VARIABLE;
            this.Type = type;
            this.TypeName = DataTypeStr2[(int)type];
            if(parent!=null) _parent = parent;
            Comment = "";
        }

        public GstNode(String type, String name, ContextKind kind, GstNode parent = null)
        {
            this.Name = name;
            this.Kind = kind;
            this.Type = (DataType)(DataTypeStr.ToList().IndexOf(type));
            this.TypeName = DataTypeStr2[(int)this.Type];// type;
            if (parent != null) _parent = parent;
            Comment = "";
        }

        
        /// <summary>
        /// 최종 노드 만들때 쓰임.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="Key"></param>
        /// <param name="parent"></param>
        public GstNode(String type, String name, String Key, GstNode parent = null)
        {
            this.Name = name;
            this.Kind = ContextKind.VARIABLE;
            this.Type = (DataType)(DataTypeStr.ToList().IndexOf(type));
            this.TypeName = DataTypeStr2[(int)this.Type];// type;
            if (parent != null) _parent = parent;
            Comment = "";
        }
       
        /// <summary>
        /// struct X 형을 가진 변수를 정의할 때 쓰임.
        /// </summary>
        /// <param name="structType"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public GstNode(String structType, String name, GstNode parent = null)
        {
            this.Name = name;
            this.Kind = ContextKind.VARIABLE;
            this.Type = DataType.STRUCT;
            this.TypeName = structType;
            if(parent!=null) _parent = parent;
            Comment = "";
        }

        public GstNode(String StructTypeName, GstNode parent = null)
        {
            this.TypeName = StructTypeName;
            String[] typeTokens = StructTypeName.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.Name = (typeTokens.Length==2)? typeTokens[1] : typeTokens[0];
            this.Kind = ContextKind.STRUCT;
            this.Type = DataType.STRUCT;
            if (parent != null) _parent = parent;
            Comment = "";
        }
       
        public GstNode this[int index]
        {
            get { return _child[index]; }
        }

        public GstNode this[String key]
        {
            get { return _gstKeyDic[key]; }
        }

        public new List<GstNode> Children
        {
            get { return _child; }
        }

        public new GstNode Parent
        {
            get { return _parent; }
        }

       public String Comment { get; set; }

        public String FullPath
        {
            get
            {
                //if (fullName.Trim().Length == 0) fullName = TypeName;
                //if (_parent != null) return (_parent.FullPath.Trim().Length!=0)? _parent.FullPath + "." + Name : Name;
                //else return Name;//최상위 노드(GST)는 출력하지않음. fullName;
                return Name;
            }
        }


        
        public DataType TypeFromStr(String type)
        {
            int index = DataTypeStr.ToList().IndexOf(type);
            return (DataType)index;
        }

        public GstNode()
        {
            this.TypeName = "";
            this.Name = "GST";
            this.Kind = ContextKind.GLOBAL_MAIN;
            this.Type = DataType.NONE;
            _parent = null;
        }


        public void addChild(GstNode aChild){
            _child.Add(aChild);
            addGstKey(aChild);
            aChild._parent = this;
        }

        void addGstKey(GstNode aChild)
        {
            if(aChild.Key.Length>0) _gstKeyDic.Add(aChild.Key, aChild);
            if (Parent != null) Parent.addGstKey(aChild);
        }

        public void setIOT(IOTStruct iot)
        {
            this.IOT = iot;
            addIotKey(this);
        }
        void addIotKey(GstNode aChild)
        {
            _iotKeyDic.Add(aChild.IOT.Key, aChild);
            if (Parent != null) addIotKey(aChild);
        }

        /// <summary>
        /// 현재 노드와 상위노드에서 검색하여 해당 이름을 가진 노드를 리턴한다. 없으면 null
        /// 변수를 미리 define했을 때에 그 변수가 해당 범위안에 있는지를 알아내는 함수이다.
        /// </summary>
        /// <param name="name">찾을 Name</param>
        /// <returns>해당 Name을 가진 노드</returns>
        public GstNode getActiveMember(String name)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Name.Equals(name)) return Children[i];
            }
            GstNode m = Parent.getActiveMember(name);

            if (m != null) return m;
            else return null;
        }

        /// <summary>
        /// 현재 노드와 상위노드에서 검색하여 해당 이름을 가진 노드를 리턴한다. 없으면 null
        /// 변수를 미리 define했을 때에 그 변수가 해당 범위안에 있는지를 알아내는 함수이다.
        /// </summary>
        /// <param name="typeName">찾을 Name</param>
        /// <returns>해당 Name을 가진 노드</returns>
        public GstNode getActiveStruct(String typeName)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].TypeName.Equals(typeName) && Children[i].Kind == ContextKind.STRUCT) return Children[i];
            }
            if (Parent != null)
            {
                GstNode m = Parent.getActiveStruct(typeName);

                if (m != null) return m;
                else return null;
            }
            else return null;
        }
        public GstNode getFirstChildNode(String nodeName)
        {
            GstNode aChild = null;
            foreach (GstNode node in Children)
            {
                if (node.Name.Equals(nodeName))
                {
                    return node;
                }
             
                if (node.Children.Count > 0 && (aChild = node.getFirstChildNode(nodeName))!=null) return aChild;
            }
            return null;
        }
        public void getEndNodes(ref List<GstNode> list)
        {

            foreach (GstNode aChild in Children)
            {
                if (aChild.Children.Count == 0) list.Add(aChild);
                else aChild.getEndNodes(ref list);
            }
        }

    }
}
