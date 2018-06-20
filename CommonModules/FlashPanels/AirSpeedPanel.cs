using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flash.External;
using FormAdders;
namespace FlashPanels
{
    public class AirSpeedPanel:FlashAdder
    {
       
        public AirSpeedPanel()
            : base()
        {
            
        }

        private Object[] arg = null;
       

        object AirSpeedPanel_U_FlashFuncCallBack(object sender, ExternalInterfaceCall e)
        {
            switch (e.FunctionName)
            {

                case "SetMinMaxValue":
                    arg = new Object[1];
                    Popup popup = new Popup(0, arg, TypeCode.Int32);
                    //popup.list.AddRow("MinValue", "1000");
                    popup.list.AddRow("MaxValue", "10000");

                    popup.U_PopupClosed += new PopupClosedEventHandler(popup_PopupClosed);
                    popup.ShowDialog();
                    
                    break;
                    
            }
            return null;
        }

        void popup_PopupClosed(object sender, PopupClosedEventArgs args)
        {
            if(arg!=null && arg.Length>0 && (int)(arg[0])>=0) U_setMaxSpeed((int)arg[0]/10, (int)arg[0], 5);
        }

        protected override void OnMovieLoad()
        {
            try
            {
                this.U_addRightMenu("SetMinMax", "SetMinMaxValue", false, true);
                this.U_FlashFuncCallBack += new FlashFunctionCallEventHandler(AirSpeedPanel_U_FlashFuncCallBack);
                
            }
            catch { }
        }

        public void U_drawArcs(int thick, int radius, int startNum, params int[] colorEndValuePairs)
        { //rest is pairs of a color and an endNumber.
            Object[] arr = new Object[3 + colorEndValuePairs.Length];
            arr[0] = thick;
            arr[1] = radius;
            arr[2] = startNum;
            for (int i = 0; i < colorEndValuePairs.Length; i++)
            {
                arr[3 + i] = colorEndValuePairs[i];
            }
            U_CallFlashFuncWithArray("drawArcs", arr);
        }

        public void U_setNumLine(int totalUnit, int sizePer1Point, int smallUnitPerBigUnit, int varViewRate, int devideNumWith, int startAngle, int endAngle, int minValue)
        {
            U_CallFlashFunc("setNumLine",
                totalUnit,
                sizePer1Point,
                smallUnitPerBigUnit,
                varViewRate,
                devideNumWith,
                startAngle,
                endAngle,
                minValue
            );
        }

        public void U_setMaxSpeed(int min, int max, int smallUnitPerBigUnit)
        {
            int devideNumWith = max / 10;
            min = max / 10;
            int sizePer1Point = devideNumWith / smallUnitPerBigUnit;
            int totalUnit = smallUnitPerBigUnit * ((max-min)/min);

            int varViewRate = smallUnitPerBigUnit;

            U_CallFlashFunc("setNumLine",
                totalUnit,
                sizePer1Point,
                smallUnitPerBigUnit,
                varViewRate,
                devideNumWith,
                30,
                330,
                min
            );

        }

        public void U_setArcs(int startAng, int endAng, int sizePer1Point, int totalUnit, int minValue)
        {
            U_CallFlashFunc("setArcs", startAng, endAng, sizePer1Point, totalUnit, minValue);
        }

        public void U_setSpeedVector(double ve, double vn, double vu)
        {

            U_setValue(Math.Sqrt(ve * ve + vn * vn + vu * vu));
        }
    }
}
