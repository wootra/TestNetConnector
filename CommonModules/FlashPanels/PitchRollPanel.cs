using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flash.External;

namespace FlashPanels
{
    public class PitchRollPanel:FlashAdder
    {
        public PitchRollPanel()
            : base()
        {
        }

        public void setPitch(double angle){
            U_CallFlashFunc("setPitch",angle);
        }
        public void setRoll(double angle){
            U_CallFlashFunc("setRoll",angle);
        }

    }
}
