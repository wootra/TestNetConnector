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

   /// <summary>
   /// Represents possible flow directions of items on the panels
   /// </summary>
   public enum RibbonPanelFlowDirection
   {
      /// <summary>
      /// Layout of items flows to the left, then down
      /// </summary>
      Left = 2,
      /// <summary>
      /// Layout of items flows to the Right, then down
      /// </summary>
      Right = 1,
      /// <summary>
      /// Layout of items flows to the bottom, then to the right
      /// </summary>
      Bottom = 0
   }
}
