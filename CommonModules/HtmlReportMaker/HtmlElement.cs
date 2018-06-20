using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Xml;
using XmlHandlers;

namespace HtmlReportMaker
{
    public class HtmlElement
    {

        protected HtmlElement()
        {
            _moreStyle = "";
        }

        public virtual int Height { get; set; }

        public String Style
        {
            get
            {
                string style = "";
                
                if (ForeColor != Color.Empty) style += "color:#" + BackColor.R.ToString("X2") + BackColor.G.ToString("X2") + BackColor.B.ToString("X2") + ";";

                //string borderWid="";
                

                string borderColor="";
                               
                    
                if (FontSize >= 0) style += "font-size:" + FontSize + "px;";
                if (BorderWidths != null)
                {
                    //string a = "border-bottom:solid; border-left:none; border-right:none; border-top:none;border-bottom-width:1px;";
                    if (BorderColors != null) borderColor = getColorStr(BorderColors[0]);
                    if (BorderWidths.Left > 0) style += "border-left:solid "+borderColor+ BorderWidths.Left + "pt;";
                    else style += "border-left:none;";
                    if (BorderColors != null) borderColor = getColorStr(BorderColors[1]);
                    if (BorderWidths.Right > 0) style += "border-right:solid " + borderColor + +BorderWidths.Right + "pt;";
                    else style += "border-right:none;";
                    if (BorderColors != null) borderColor = getColorStr(BorderColors[2]);
                    if (BorderWidths.Top > 0) style += "border-top:solid " + borderColor + BorderWidths.Top + "pt;";
                    else style += "border-top:none;";
                    if (BorderColors != null) borderColor = getColorStr(BorderColors[3]);
                    if (BorderWidths.Bottom > 0) style += "border-bottom:solid " + borderColor + BorderWidths.Bottom + "pt;";
                    else style += "border-bottom:none;";

                }
                if(MoreStyle!=null) style += MoreStyle;
                return style;
            }
        }

        string getColorStr(Color color)
        {
            if (color != Color.Empty)
                return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + " ";
            else return "";
        }

        string _moreStyle = "";
        /// <summary>
        /// 기본적인 Style이외에 Style을 추가하려면 MoreStyle을 override한다.
        /// MoreStyle의 뒤에는 ;가 붙지 않아야 한다.(자동으로 붙임)
        /// </summary>
        public virtual String MoreStyle { get { return _moreStyle; } }

        Margins _borderWidths = null;
        public virtual Margins BorderWidths
        {
            get { return _borderWidths; }
            set
            {
                _borderWidths = value;
            }
        }
        
        public virtual Color ForeColor { get; set; }
        public virtual Color BackColor { get; set; }
        
        int _fontSize = -1;
        public virtual int FontSize { get { return _fontSize; } set { _fontSize = value; } }

        
        //public virtual int BorderWidth { get { return _borderWidth; } set { _borderWidth = value; } }
        Color _borderColor = Color.Empty;
        public virtual Color BorderColor { 
            get { return _borderColor; } 
            set { 
                _borderColor = value;
                BorderColors = new Color[4]{
                   value,
                   value,
                   value,
                   value
                };
            } 
        }
        public virtual Color[] BorderColors { get; set; }

        protected void GetXmlForThis(XmlDocument xDoc, XmlNode myNode)
        {
            if (BackColor != Color.Empty)
            {
                XmlAdder.Attribute(xDoc, "bgcolor", "#" + BackColor.R.ToString("X2") + BackColor.G.ToString("X2") + BackColor.B.ToString("X2"), myNode);
            }
            //XmlAdder.Attribute(xDoc, "valign", "middle", myNode);


        }
    }
}
