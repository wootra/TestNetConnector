using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridVariousColumn:DataGridViewColumn
    {
        Dictionary<EasyGridRow, EasyGridVariousTypeCellInfo> Info = new Dictionary<EasyGridRow, EasyGridVariousTypeCellInfo>();

        public EasyGridVariousColumn()
            : base()
        {

        }
    }
}
