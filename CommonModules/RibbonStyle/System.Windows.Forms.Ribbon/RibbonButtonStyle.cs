// *********************************
// Message from Original Author:
//
// 2008 Jose Menendez Poo
// Please give me credit if you use this code. It's all I ask.
// Contact me for more info: menendezpoo@gmail.com
// *********************************
//
// Original project from http://ribbon.codeplex.com/
// Continue to support and maintain by http://officeribbon.codeplex.com/


using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public enum RibbonButtonStyle
    {
        /// <summary>
        /// Simple clickable button
        /// </summary>
        Normal,
        /// <summary>
        /// Button with a right side drop down
        /// </summary>
        DropDown,
        /// <summary>
        /// Button with an optional dropdown attachment on the right
        /// </summary>
        SplitDropDown,
        /// <summary>
        /// Mimics a standard drop down list item with no image
        /// </summary>
        DropDownListItem,
    }
}
