using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;
namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridRadioButtonColumn:DataGridViewColumn
    {
        public EasyGridRadioButtonColumn()
            : base()
        {

        }

        public EasyGridRadioButtonColumn(EasyGridRadioButtonCell cellTemplate)
            : base(cellTemplate)
        {

        }

        EasyGridRadioButtonCollection _initItems = new EasyGridRadioButtonCollection(null);

        public EasyGridRadioButtonCollection Items
        {
            get
            {
                return _initItems;
            }
        }
    }
}
