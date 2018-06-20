using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Flash.External;
using FormAdders;
namespace FlashPanels
{
    public class AltPanel :FlashAdder
    {
        public AltPanel():base()
        {
            
        }

        private Object[] arg = null;


        object AirSpeedPanel_U_FlashFuncCallBack(object sender, ExternalInterfaceCall e)
        {
            switch (e.FunctionName)
            {

                case "SetMaxValue":
                    arg = new Object[2];
                    Popup popup = new Popup(0, arg, TypeCode.Int32);
                    popup.list.AddRow("MaxValue", "100000");

                    popup.U_PopupClosed += new PopupClosedEventHandler(popup_PopupClosed);
                    popup.ShowDialog();

                    break;

            }
            return null;
        }

        void popup_PopupClosed(object sender, PopupClosedEventArgs args)
        {
            if (arg != null && arg.Length > 0 && (int)(arg[0])>=0) U_setMaxAltitude((int)arg[0], 5);
        }

        protected override void OnMovieLoad()
        {
            try
            {
                this.U_FlashFuncCallBack += new FlashFunctionCallEventHandler(AirSpeedPanel_U_FlashFuncCallBack);
                this.U_addRightMenu("SetMax", "SetMaxValue", false, true);
                U_setMaxAltitude(100000, 5);
            }catch{
            }
        }

        public void U_set1UnitForNumUpPanel(int value)
        {
            U_CallFlashFunc("set1UnitNumPanel", value);
        }
        public void U_setNumLine(int totalUnit, int sizePer1Point, int smallUnitPerBigUnit, int varViewRate, int devideNumWith)
        {
            U_CallFlashFunc("setNumLine",
                totalUnit,
                sizePer1Point,
                smallUnitPerBigUnit,
                varViewRate,
                devideNumWith,
                0,
                360,
                0
            );
        }
        public void U_setMaxAltitude(int max, int smallUnitPerBigUnit)
        {
            int devideNumWith = max / 100;
            int sizePer1Point = devideNumWith / smallUnitPerBigUnit;
            int totalUnit = smallUnitPerBigUnit*10;

            int varViewRate = smallUnitPerBigUnit;
            U_CallFlashFunc("setNumLine",
                totalUnit,
                sizePer1Point,
                smallUnitPerBigUnit,
                varViewRate,
                devideNumWith,
                0,
                360,
                0
            );

            U_CallFlashFunc("set1UnitNumPanel", max / 10000.0);

        }
    }
}
