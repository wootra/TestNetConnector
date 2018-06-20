using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flash.External;

namespace FlashPanels
{
    public class SignalNoisePanel:FlashAdder
    {
        public SignalNoisePanel()
            : base()
        {
        }

        public void U_setSvId(int startIndex, params Object[] ids)
        {
            Object[] data = getDataArray(startIndex, ids);
            U_CallFlashFuncWithArray("setSvId", data);
        }

        public void U_setStatus(int startIndex, params Object[] status)
        {
            Object[] data = getDataArray(startIndex, status);
            U_CallFlashFuncWithArray("setStatus", data);

        }

        public void U_setSignalNoise(int startIndex, params Object[] snr)
        {
            Object[] data = getDataArray(startIndex, snr);
            U_CallFlashFuncWithArray("setValue", data); 
        }

        public void U_setInit()
        {
            Object[] data = new Object[39];
            data[0] = 0;
            U_CallFlashFuncWithArray("setSvId", data);
            U_CallFlashFuncWithArray("setStatus", data);
            U_CallFlashFuncWithArray("setValue", data); 
        }

        public void U_setTest()
        {
            U_CallFlashFunc("setTest");
        }
    }
}
