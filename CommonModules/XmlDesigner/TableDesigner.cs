using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XmlDesigner
{
    public partial class TableDesigner : UserControl
    {
        public TableDesigner()
        {
            InitializeComponent();
        }
        public void LoadXml(String xmlFile, String xsdFile)
        {
            XmlSchema xSchema = new XmlSchema();
            xSchema.SourceUri = "";
            DataSet ds = new DataSet("Test");
            ds.ReadXmlSchema(xsdFile);
            ds.InferXmlSchema(xsdFile, null);
        }
    }
}
