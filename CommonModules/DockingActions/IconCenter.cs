using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UCoreComponents;
using DataHandling;

namespace DockingActions
{
    internal partial class IconCenter : UserControl
    {
        public enum ButtonPosition { B_Center = 0, B_TopHalf, B_TopRemain, B_BottomHalf, B_BottomRemain, B_LeftHalf, B_LeftRemain, B_RightHalf, B_RightRemain, NotOnTheButton };
        public ButtonPosition[] _buttonPosList = new ButtonPosition[]{
            ButtonPosition.B_Center ,
            ButtonPosition.B_TopHalf,ButtonPosition.B_TopRemain,
            ButtonPosition.B_BottomHalf, ButtonPosition.B_BottomRemain,
            ButtonPosition.B_LeftHalf, ButtonPosition.B_LeftRemain,
            ButtonPosition.B_RightHalf, ButtonPosition.B_RightRemain 
        };
        public Control[] _buttonList;
        DockingRoot _root;
        public IconCenter(DockingRoot root)
        {
            InitializeComponent();
            _buttonList = new Control[] { B_Center, B_TopHalf, B_TopRemain, B_BottomHalf, B_BottomRemain, B_LeftHalf, B_LeftRemain, B_RightHalf, B_RightRemain };

            _root = root;
        }
        ButtonPosition GetButtonMouseOn(Control sender,int screenX = 0, int screenY = 0)
        {
            for (int i = 0; i < _buttonList.Length; i++)
            {
                if (CoodinateHandling.isEntered(_buttonList[i], screenX, screenY))
                {
                    return _buttonPosList[i];
                }
            }
            return ButtonPosition.NotOnTheButton;
        }
    }
}
