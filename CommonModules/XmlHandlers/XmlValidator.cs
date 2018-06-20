using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace XmlHandler
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class XmlValidator
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        static int nErr = 0;
        //static StringWriter writer;
        static XmlValidateDataList validateList;
        //[STAThread]
        //static void Main(string[] args)
        public static XmlValidateDataList Validate(String xmlFile, string schemaOrDtdFile = null, ValidationType type = ValidationType.Schema)
        {
            StringBuilder builder = new StringBuilder();
            validateList = new XmlValidateDataList();
            //writer = new StringWriter(builder);

            try
            {
                String mainXML, schemaList = "";
                mainXML = xmlFile;
                if (schemaList != null)
                {
                    schemaList = schemaOrDtdFile;
                }

                XmlTextReader tr = new XmlTextReader(mainXML);
                XmlValidatingReader vr = new XmlValidatingReader(tr);
                vr.ValidationType = type;
                vr.ValidationEventHandler += new ValidationEventHandler(validationEventHandler);

                if (schemaList.Length > 0)
                {
                    StreamReader sr = File.OpenText(schemaList);
                    String input;
                    while ((input = sr.ReadLine()) != null)
                    {
                        XmlTextReader xml = new XmlTextReader(input);
                        while (xml.Read())
                        {
                            if (xml.NodeType == XmlNodeType.Element)
                            {
                                if (xml.LocalName == "schema" && xml.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
                                {
                                    String namespaceURI = xml.GetAttribute("targetNamespace");
                                    validateList.List.Add(new XmlValidateData(String.Format("Loading schema {0} for namespace '{1}'", input, namespaceURI)));
                                    vr.Schemas.Add(namespaceURI, input);
                                }
                                else
                                    validateList.List.Add(new XmlValidateData(String.Format("Skipping file {0}, it doesn't look like a valid schema", input)));
                                break;
                            }
                        }
                        xml.Close();
                    }
                    sr.Close();

                }

                nErr = 0;

                //else if(option == "schema")
                //vr.ValidationType = ValidationType.Schema;

                while (vr.Read()) ;
            }
            catch (XmlException e)
            {
                nErr++;
                validateList.List.Add(new XmlValidateData(e.LineNumber, e.LinePosition, e.SourceUri, e.Message));
                //validateList.Add(new XmlValidateData(String.Format("{0}:{1},{2}: {3}", xmlFile, e.LineNumber, e.LinePosition, e.Message ));	
            }
            catch (Exception e)
            {
                nErr++;
                validateList.List.Add(new XmlValidateData(e.ToString()));
            }



            //writer.WriteLine("-----------------------------------");
            //writer.WriteLine("Validation complete with {0} error(s)", nErr);
            validateList.NumOfError = nErr;
            return validateList;
            //return builder.ToString();
        }

        private static void validationEventHandler(object sender, ValidationEventArgs e)
        {
            nErr++;
            XmlValidateData data = new XmlValidateData(e.Exception.LineNumber, e.Exception.LinePosition, e.Exception.SourceUri, e.Exception.Message);

            //data.ErrorText = String.Format("{0}:{1},{2}: {3}", e.Exception.SourceUri, e.Exception.LineNumber, e.Exception.LinePosition, e.Exception.Message);

            //writer.WriteLine("{0}:{1},{2}: {3}", e.Exception.SourceUri, e.Exception.LineNumber, e.Exception.LinePosition, e.Exception.Message );	
        }


    }
    public class XmlValidateDataList
    {
        public List<XmlValidateData> List = new List<XmlValidateData>();
        public int NumOfError = 0;

    }
    public class XmlValidateData
    {
        public string SourceUri;
        public String ErrorText
        {
            get
            {
                if (LineNumber < 0) return ExceptionMsg;
                else return String.Format("{0}:{1},{2}: {3}", SourceUri, LineNumber, LinePosition, ExceptionMsg);
            }
        }
        public int LineNumber;
        public int LinePosition;
        public String ExceptionMsg;
        public XmlValidateData(int lineNumber, int linePosition, string sourceUri, string exceptionMsg)
        {
            this.LineNumber = lineNumber;
            this.LinePosition = linePosition;
            this.SourceUri = sourceUri;
            this.ExceptionMsg = exceptionMsg;
        }
        public XmlValidateData(String msg)
        {
            ExceptionMsg = msg;
            this.LinePosition = -1;
            this.LineNumber = -1;
            this.SourceUri = "";
        }
    }


}
