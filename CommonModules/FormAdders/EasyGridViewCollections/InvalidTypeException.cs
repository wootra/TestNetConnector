using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormAdders.EasyGridViewCollections
{
    public class InvalidTypeException:Exception
    {
        ICollection<Type> ValidTypes;
        object TriedItem;
        
        public InvalidTypeException(object triedItem, ICollection<Type> validTypes, String Msg=null):base()
        {
            this.TriedItem = triedItem;
            this.ValidTypes = validTypes;
            if (Msg == null)
            {
                if (triedItem != null)
                {
                    Msg = "잘못된 타입입니다. 대상object:" + triedItem + "  시도된 type:" + triedItem.GetType().ToString();
                }
                else
                {
                    Msg = "잘못된 타입입니다. 대상object:" + triedItem + "  null임";
                }
            
                Msg += "  올바른 타입:";
                int count = 0;
                foreach (Type type in validTypes)
                {
                    if (count++ != 0) Msg += ",";
                    Msg += type.ToString();
                }
            }
        }

    }
}
