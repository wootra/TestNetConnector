using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataHandling
{
    public static class SerializeCloner
    {
        public static Object FileClone(Object src)
        {
            
            String serName = @"temp_serialize_object";
            FileStream fs = File.Create(serName);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, src);
            fs.Close();
            fs = File.OpenRead(serName);
            Object obj = bf.Deserialize(fs);
            //INetPacket obj = (this as IClonable).Clone(typeof(ResizableNetworkObject<T>)) as INetPacket;
            fs.Close();
            return obj;
        }

        public static void SaveObjToFile(Object obj, String file)
        {
            //if (obj == null) return;
            String serName = file;
            String path = file.Substring(0, file.LastIndexOf("\\"));
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            FileStream fs = File.Create(serName);

            BinaryFormatter bf = new BinaryFormatter();
            if(obj!=null) bf.Serialize(fs, obj);
            fs.Close();
            //fs = File.OpenRead(serName);
            //Object obj = bf.Deserialize(fs);
            //INetPacket obj = (this as IClonable).Clone(typeof(ResizableNetworkObject<T>)) as INetPacket;
            //fs.Close();
            //return obj;
        }

        public static Object LoadObjFromFile(String file)
        {
            String serName = file;
            FileStream fs;// = File.Create(serName);

            BinaryFormatter bf = new BinaryFormatter();
            //bf.Serialize(fs, this);
            //fs.Close();
            fs = File.OpenRead(serName);
            Object obj = null;
            try
            {
                obj = bf.Deserialize(fs);
            }
            catch { }
            //INetPacket obj = (this as IClonable).Clone(typeof(ResizableNetworkObject<T>)) as INetPacket;
            fs.Close();
            return obj;
        }

        public static Object MemClone(Object src)
        {
            MemoryStream ms = new MemoryStream();
            
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, src);

            Byte[] mem = ms.GetBuffer();
            ms.Close();


            ms = new MemoryStream(mem);
            Object obj = bf.Deserialize(ms);
            return obj;
        }



    }
}
