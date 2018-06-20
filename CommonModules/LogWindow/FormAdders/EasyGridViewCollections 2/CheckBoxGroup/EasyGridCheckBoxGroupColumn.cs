using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;
namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCheckBoxGroupColumn:DataGridViewColumn
    {
        public EasyGridCheckBoxGroupColumn()
            : base()
        {

        }

        public EasyGridCheckBoxGroupColumn(EasyGridCheckBoxGroupCell cellTemplate)
            : base(cellTemplate)
        {

        }

        EasyGridCheckBoxGroupCollection _initItems = new EasyGridCheckBoxGroupCollection(null);

        public EasyGridCheckBoxGroupCollection Items
        {
            get
            {
                return _initItems;
            }
        }
    }
}
