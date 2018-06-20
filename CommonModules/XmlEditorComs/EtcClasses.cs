using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using XmlHandlers;


namespace XmlEditorComs
{
    public class Description
    {
        public int Col { set; get; }
        public int Line { set; get; }
        public int Length { set; get; }
        public Description(int col, int line, int length)
        {
            this.Col = col;
            this.Line = line;
            this.Length = length;
        }
    }
    /*
    public class ColorizeAvalonEdit : DocumentColorizingTransformer
    {
        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            while ((index = text.IndexOf("AvalonEdit", start)) >= 0)
            {
                base.ChangeLinePart(
                    lineStartOffset + index, // startOffset 
                    lineStartOffset + index + 10, // endOffset 
                    (VisualLineElement element) =>
                    {
                        // This lambda gets called once for every VisualLineElement 
                        // between the specified offsets. 
                        Typeface tf = element.TextRunProperties.Typeface;
                        // Replace the typeface with a modified version of 
                        // the same typeface 
                        element.TextRunProperties.SetTypeface(new Typeface(
                            tf.FontFamily,
                            FontStyles.Italic,
                            FontWeights.Bold,
                            tf.Stretch
                        ));
                    });
                start = index + 1; // search for next occurrence 
            }
        }
    } 
    */


    public class Colorizing : DocumentColorizingTransformer
    {
        List<Description> desiredList = new List<Description>();
        public Action<VisualLineElement> ApplyChanges;
        public Colorizing()
        {
            ApplyChanges = new Action<VisualLineElement>(visualLineElementAction);

        }


        void visualLineElementAction(VisualLineElement el)
        {

        }

        public void AddList(Description desc)
        {
            desiredList.Add(desc);
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (desiredList == null) return;

            foreach (Description d in desiredList)
            {
                if (line.LineNumber == d.Line)
                {
                    ChangeLinePart(line.Offset + d.Col, line.Offset + d.Col + d.Length, ApplyChanges);
                }
            }
        }
    }


    public delegate void SyntaxErrorEvent(IEnumerable<SyntaxError> a);

    public class AXmlFinder
    {
        /// <summary>
        /// get xml pair. if it is not pair(attribute or single tag), it returns just the tag or attribute.
        /// </summary>
        /// <param name="xDoc">AXmlDocument which is searched</param>
        /// <param name="xPath">XPath of the tag or attribute. 
        /// if you want search attribute, use '@' character 
        /// before the name of attribute. 
        /// ex> Command/Data or Command/@Name  </param>
        /// <param name="index">0 if you want get the first node. if the nodes are more than 1, use this index.</param>
        /// <returns>Pair of the tag or just single tag or attribute</returns>
        public static List<AXmlObject> GetXmlElementPair(AXmlDocument xDoc, String xPath, int index)
        {

            AXmlObject xmlObj;
            if (index == 0) xmlObj = GetXmlObject(GetRoot(xDoc), xPath);
            else
            {
                List<AXmlObject> list = new List<AXmlObject>();
                GetXmlObject(GetRoot(xDoc), xPath, list); //해당 path의 list를 가져와서
                if (index < list.Count) xmlObj = list[index];//순서를 가져온다.
                else return new List<AXmlObject>();
            }
            return GetXmlElementPair(xDoc, xmlObj);
        }

        /// <summary>
        /// currentOffset을 기준으로 element의 pair를 가져온다.
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="currentOffset"></param>
        /// <returns></returns>
        public static List<AXmlObject> GetXmlElementPair(AXmlDocument xDoc, int currentOffset)
        {
            AXmlObject xmlObj = xDoc.GetChildAtOffset(currentOffset);

            return GetXmlElementPair(xDoc, xmlObj);
            
        }

        public static List<AXmlObject> GetXmlElementPair(AXmlDocument xDoc, AXmlObject xmlObj)
        {
            List<AXmlObject> tsList = new List<AXmlObject>();
            AXmlElement ele = FindOwnerElement(xmlObj, true);
            if (ele == null) return new List<AXmlObject>();

            if (ele.HasStartOrEmptyTag) tsList.Add(ele.Children[0]);
            if (ele.HasEndTag) tsList.Add(ele.Children[ele.Children.Count - 1]);

            return tsList;
        }

        /// <summary>
        /// 부모노드에서 몇번째에 속한 element인지를 찾는다.
        /// </summary>
        /// <param name="element">index를 찾을 element</param>
        /// <param name="CountCommentAsElement">Comment를 Element로 인식하고 count할 것인지 여부</param>
        /// <returns>element의 index가 부모노드에서 몇번인지. 0 based.</returns>
        public static int GetIndexFromParent(AXmlElement element, bool CountCommentAsElement = true )
        {
            AXmlElement parent = element.Parent as AXmlElement;
            AXmlObject temp = element.Parent;
            int index = 0;
            while (parent == null && temp != null)
            {
                parent = parent.Parent as AXmlElement;
                temp = temp.Parent;
            }
            if (parent != null)
            {
                foreach (AXmlObject obj in parent.Children)
                {

                    if (obj.Equals(element))
                    {
                        return index;
                        break;//순서를 찾았다.
                    }
                    else if (element is AXmlElement && obj is AXmlElement)
                    {
                        index++;
                    }
                    else if (obj is AXmlTag && (obj as AXmlTag).OpeningBracket.Equals("<!--"))
                    {
                        if (CountCommentAsElement) index++;
                    }

                }
            }
            
            return -1;
        }

        public static AXmlAttribute GetAttribute(AXmlElement element, String attrName)
        {
            foreach (AXmlAttribute attr in element.Attributes)
            {
                if (attr.Name.Equals(attrName))
                {
                    return attr;
                    break;
                }
            }
            return null;
        }



        /// <summary>
        /// get the first xmlobject from baseNode by xpath. <br/>
        /// if there are multiple objects in baseNode, <br/>
        /// fill list argument. this function will fill this list with all xml objects<br/>
        /// that have the path.<br/>
        /// </summary>
        /// <param name="baseNode">if you don't know root, call GetRoot(AXmlDocument xDoc) static method.</param>
        /// <param name="xPath">seperator is '/'. ex>Command/Data</param>
        /// <param name="list">if null, this function just return the first Node and end. but if not null, all reference will be inserted in this list</param>
        /// <returns>the first XmlNode which has the path of xPath, formatted of AXmlObject</returns>
        public static AXmlObject GetXmlObject(AXmlElement baseNode, String xPath, List<AXmlObject> list = null)
        {
            String firstNodeName = xPath; // '/'가 없는 문자열일 경우 그 자체가 이름..
            String lastPath = "";
            AXmlObject selectedObj = null;
            AXmlObject firstSelected = null;

            if (baseNode == null)
                return null;

            if (xPath.Contains('/'))
            {
                firstNodeName = xPath.Substring(0, xPath.IndexOf("/"));
                lastPath = xPath.Substring(xPath.IndexOf('/') + 1);

                if (firstNodeName.Equals(baseNode.Name))
                {
                    if (lastPath.Contains('/'))
                    {
                        firstNodeName = lastPath.Substring(0, lastPath.IndexOf("/"));
                        lastPath = lastPath.Substring(lastPath.IndexOf('/') + 1);
                    }
                    else
                    {
                        firstNodeName = lastPath;
                        lastPath = "";
                    }
                }
            }

            if (lastPath.Length == 0 && baseNode.Name.ToLower().Equals(firstNodeName.ToLower()))
            {
                list.Add(baseNode);
                return baseNode;
            }
            else
            {
                foreach (AXmlObject obj in baseNode.Children)
                {
                    if (obj is AXmlElement)
                    {
                        AXmlElement element = (obj as AXmlElement);
                        if (element.Name.Equals(firstNodeName))
                        {//첫 노드를 찾았으니 다음으로 넘어가야 한다.

                            if (lastPath.Length == 0) selectedObj = element;
                            else selectedObj = GetXmlObject(element, lastPath, list);

                            if (selectedObj != null)
                            {
                                if (list == null) return selectedObj;
                                else
                                {
                                    if (firstSelected == null) firstSelected = selectedObj;
                                    list.Add(selectedObj);
                                }
                            }
                        }
                    }
                    else if (obj is AXmlAttribute && firstNodeName[0].Equals('@'))//Attribute일 경우, XPath의 첫글자는 @로 시작해야 한다.
                    {
                        AXmlAttribute attr = (obj as AXmlAttribute);
                        if (attr.Name.Equals(firstNodeName))
                        {//첫 노드를 찾았으니 다음으로 넘어가야 한다.
                            selectedObj = attr;
                            if (selectedObj != null)
                            {
                                if (list == null)
                                    return selectedObj;
                                else
                                {
                                    if (firstSelected == null) firstSelected = selectedObj;
                                    list.Add(selectedObj);
                                }
                            }
                        }
                    }
                }
            }
            return firstSelected;
        }

        public static AXmlElement GetRoot(AXmlDocument xDoc)
        {
            AXmlObjectCollection<AXmlObject> elements = xDoc.Children;
            foreach (AXmlObject obj in elements)
            {
                if ((obj as AXmlElement) != null)
                {//첫번째 만나는 Tag가 Root이다.
                    return obj as AXmlElement;
                }
            }
            return null;
        }


        /// <summary>
        /// find the element which has xmlObj as a child or grand x child.
        /// </summary>
        /// <param name="xmlObj">start point</param>
        /// <param name="useItSelfIfElement">if start point is element, return this one</param>
        /// <param name="elementName">find parent </param>
        /// <returns>parent element which has xmlObj as a child or grand x child</returns>
        public static AXmlElement FindOwnerElement(AXmlObject xmlObj, bool useItSelfIfElement = false, String elementName=null, bool ignoreCases=true)
        {
            if (useItSelfIfElement)
            {
                if (xmlObj is AXmlElement)
                {
                    if(elementName==null) return xmlObj as AXmlElement;
                    else if (ignoreCases)
                    {
                        if ((xmlObj as AXmlElement).Name.ToLower().Equals(elementName.ToLower())) return xmlObj as AXmlElement;
                    }
                    else
                    {
                        if ((xmlObj as AXmlElement).Name.Equals(elementName)) return xmlObj as AXmlElement;
                    }
                }
            }
            AXmlObject temp = xmlObj.Parent;
            if (temp == null) return null; //the end of node
            if (elementName == null)
            {
                while (temp != null && (temp is AXmlElement) == false)
                {
                    temp = temp.Parent;
                }
            }
            else //there is parent name which will be found.
            {
                AXmlElement parent=null;
                while (temp != null)
                {
                    if (temp is AXmlElement)
                    {
                        if (ignoreCases)
                        {
                            if ((temp as AXmlElement).Name.ToLower().Equals(elementName.ToLower()))
                            {
                                parent = temp as AXmlElement;
                                break;
                            }
                        }
                        else
                        {
                            if ((temp as AXmlElement).Name.Equals(elementName))
                            {
                                parent = temp as AXmlElement;
                                break;
                            }
                        }
                    
                    }
                    temp = temp.Parent;
                }
                return parent;
            }
            if ((temp is AXmlElement) == false) return null;
            else return temp as AXmlElement;
        }

        public static String GetPath(AXmlElement activeElement)
        {
            String path = activeElement.Name;
            AXmlElement parent = activeElement;
            while ((parent = AXmlFinder.FindOwnerElement(parent, false)) != null)
            {
                path = parent.Name + "\\" + path;
                
            }
            return path;
        }
    }

    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private XmlEditor _xmlEditor;
        private TextEditor _editor;

        public HighlightCurrentLineBackgroundRenderer(XmlEditor editor, KnownLayer layer = KnownLayer.Background)
        {
            _xmlEditor = editor;
            _editor = editor.Editor;
            _layer = layer;
        }

        KnownLayer _layer = KnownLayer.Background;
        public KnownLayer Layer
        {
            get { return _layer; }
        }

        public List<ISegment> HighlightSegments
        {
            get;
            set;
        }

        public List<ISegment> UnderlineSegments
        {
            get;
            set;
        }

        public List<AXmlObject> HighlightObjects
        {
            set
            {
                HighlightSegments = new List<ISegment>();
                foreach (AXmlObject obj in value)
                {
                    HighlightSegments.Add(obj);
                }
            }
        }

        public List<AXmlObject> UnderlineObjects
        {
            set
            {
                UnderlineSegments = new List<ISegment>();
                foreach (AXmlObject obj in value)
                {
                    UnderlineSegments.Add(obj);
                }
            }
        }

        Color _highlightColor = Color.FromArgb(0x40, 0, 0, 0xFF);
        public Color HighlightColor
        {
            get { return _highlightColor; }
            set { _highlightColor = value; }
        }
        Color _underlineColor = Color.FromArgb(0x40, 0xff, 0, 0);
        public Color UnderlineColor
        {
            get { return _underlineColor; }
            set { _underlineColor = value; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();

            //var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);

            if (_xmlEditor.xDoc == null) return;


            if (textView.ActualWidth > 32)
            {
                if (HighlightSegments != null)
                {

                    foreach (ISegment obj in HighlightSegments)
                    {
                        foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, obj))
                        {
                            drawingContext.DrawRectangle(
                                new SolidColorBrush(HighlightColor), null,
                                new Rect(rect.Location, rect.Size));
                        }
                    }
                }
                if (UnderlineSegments != null)
                {

                    foreach (ISegment obj in UnderlineSegments)
                    {
                        foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, obj))
                        {
                            drawingContext.DrawLine(
                                new Pen(new SolidColorBrush(_underlineColor), 1.0),
                                new Point(rect.BottomLeft.X, rect.BottomLeft.Y),
                                new Point(rect.BottomRight.X, rect.BottomRight.Y));
                        }
                    }
                }
            }
        }
    }
}
