using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridRow : DataGridViewRow
    {
        public Dictionary<String, Object> RelativeObject = new Dictionary<string, object>();
        public EasyGridRow()
            : base()
        {

        }

        bool _isSetting = false;
        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                if (_isSetting == false)
                {
                    _isSetting = true;
                    base.Selected = value;
                    _isSetting = false;
                }
            }
        }


    }



}
