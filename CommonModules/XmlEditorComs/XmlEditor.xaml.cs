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
using System.Windows.Controls.Primitives;

namespace XmlEditorComs
{
    /// <summary>
    /// XmlEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class XmlEditor : UserControl
    {
        public event SyntaxErrorEvent E_SyntaxError;
        public event XmlChangedEventHandler E_XmlChanged;
        public event TextChangedEvent E_TextChanged;
        public event SelectElementEvent E_ElementSelected;
        public Colorizing color;
        //public ColorizeAvalonEdit colorEdit;
        DispatcherTimer _textRefreshTimer;

        FoldingManager foldingManager;
        XmlFoldingStrategy foldingStrategy;
        HighlightCurrentLineBackgroundRenderer backgroundRenderer;
        HighlightCurrentLineBackgroundRenderer errorBackgroundRenderer;
        public XmlEditor()
            : base()
        {
            try
            {
                InitializeComponent();
            }
            catch
            {
                throw;
            }
            B_Num.Click += new RoutedEventHandler(B_Num_Click);
            B_WordWrap.Click += new RoutedEventHandler(B_WordWrap_Click);
            //Editor.TextChanged +=new EventHandler(Editor_TextChanged);
            
            backgroundRenderer = new HighlightCurrentLineBackgroundRenderer(this);
            errorBackgroundRenderer = new HighlightCurrentLineBackgroundRenderer(this);
            Editor.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);
            Editor.TextArea.TextView.BackgroundRenderers.Add(errorBackgroundRenderer);
            Editor.PreviewMouseUp += Editor_MouseUp;
            //Editor.TextArea.Caret.PositionChanged += (sender, e) =>
            //Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
            Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            //Editor.TextArea.ActiveInputHandler = new TextAreaInputHandler(Editor.TextArea);
            color = new Colorizing();

            //colorEdit = new ColorizeAvalonEdit();
            // Editor.TextArea.TextView.LineTransformers.Add(colorEdit);
            #region Folding

            foldingStrategy = new XmlFoldingStrategy();
            Editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(Editor.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();
            #endregion
            this.Loaded += XmlEditor_Loaded;
            
            _textRefreshTimer = new DispatcherTimer(new TimeSpan(2 * 1000 * 1000), DispatcherPriority.Normal,
                delegate
                {
                    try
                    {

                        if (this.IsVisible && this.T_Text.TextArea.IsKeyboardFocused)
                        {
                            textRefreshTimer_Tick();
                        }
                    }
                    catch
                    {
                        throw;
                    }

                }, this.Editor.Dispatcher);//200 ms

            _textRefreshTimer.Start();
        }

        void XmlEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.TextChanged += Editor_TextChanged;
        }

        

        bool _countCommentAsElement = true;

        public AXmlObject ActiveObject = null;
        public AXmlElement ActiveElement = null;
        public AXmlAttribute ActiveAttribute = null;
        public AXmlTag ActiveComment = null;
        
        public int _activeIndex = -1;
        
        public int ActiveIndex
        {
            get { return _activeIndex; }
        }

        public int GetIndex(AXmlObject objToSearch, bool CountCommentAsElement)
        {

            int index = 0;
            bool isInList = false;
            if (objToSearch != null && objToSearch.Parent != null && objToSearch.Parent is AXmlElement)
            {

                AXmlElement parent = objToSearch.Parent as AXmlElement;
                AXmlObject temp = objToSearch.Parent;
                while (parent == null && temp != null)
                {
                    parent = parent.Parent as AXmlElement;
                    temp = temp.Parent;
                }

                if (parent != null)
                {
                    foreach (AXmlObject obj in parent.Children)
                    {

                        if (obj.Equals(objToSearch))
                        {
                            isInList = true;
                            break;//순서를 찾았다.
                        }
                        else if (objToSearch is AXmlElement && obj is AXmlElement)
                        {
                            index++;
                        }
                        else if (objToSearch is AXmlTag && (objToSearch as AXmlTag).OpeningBracket.Equals("<!--") && obj is AXmlElement)
                        {
                            if (CountCommentAsElement) index++;
                        }
                        else if (obj is AXmlTag && (obj as AXmlTag).OpeningBracket.Equals("<!--"))
                        {
                            if (CountCommentAsElement) index++;
                        }

                    }
                }
            }
            if (isInList) return index;
            else return -1;

        }
        
        void Editor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (xDoc != null && Editor.SelectionLength == 0 && _isCaretPosChanged)
            {
                ActiveOnOffset(Editor.CaretOffset);

                if (E_ElementSelected != null && _eventEnabled)
                    E_ElementSelected(ActiveObject, ActiveElement, ActiveIndex);

            }
            _isCaretPosChanged = false;
        }

        public StatusBar GetStatusBar()
        {
            return _statusBar;
        }
        
        public void ActiveOnOffset(int textOffset)
        {
            
            SetCaretOffset(textOffset);
            AXmlObject xmlObj = xDoc.GetChildAtOffset(textOffset);
            backgroundRenderer.HighlightColor = Color.FromArgb(0x40, 0, 0, 0xFF);
            ActiveObject = xmlObj;
            
            _activeIndex = -1; //초기값..

            if (xmlObj is AXmlText)
            {
                if (xmlObj.Parent != null && xmlObj.Parent is AXmlTag)
                {
                    AXmlTag tag = xmlObj.Parent as AXmlTag;
                    if (tag.OpeningBracket.Equals("<!--"))//comment
                    {
                        ActiveComment = tag;
                        ActiveElement = null;
                        _activeIndex = GetIndex(tag,true);
                    }
                    else ActiveComment = null;
                }
                else ActiveComment = null;
            }
            else if (xmlObj is AXmlTag)
            {
                AXmlTag tag = xmlObj as AXmlTag;
                if (tag.OpeningBracket.Equals("<!--"))//comment
                {
                    ActiveComment = tag;
                    ActiveElement = null;
                    _activeIndex = GetIndex(tag, true);
                }
                else ActiveComment = null;
            }
            else
            {
                ActiveComment = null;
            }

            if (ActiveComment == null)
            {
                ActiveElement = AXmlFinder.FindOwnerElement(xmlObj, true);
                if (ActiveElement != null)
                {
                    _activeIndex = GetIndex(ActiveElement, true);
                }
            }

            if (xmlObj is AXmlAttribute)
                ActiveAttribute = xmlObj as AXmlAttribute;
            else if ((xmlObj is AXmlElement) == false && (xmlObj is AXmlTag))
            {
                ActiveAttribute = null;
            }
            else ActiveAttribute = null;

           

            if (ActiveComment != null)
                backgroundRenderer.HighlightObjects = new AXmlObject[]{ActiveComment}.ToList();
            else if (ActiveElement != null)
                backgroundRenderer.HighlightObjects = AXmlFinder.GetXmlElementPair(xDoc, Editor.CaretOffset);//(xDoc, AXmlFinder.GetPath(ActiveElement), ActiveIndex);// AXmlFinder.GetXmlElementPair(xDoc, Editor.CaretOffset);

            
            Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
            
            
        }

        bool _isCaretPosChanged = false;
        void Caret_PositionChanged(object sender, EventArgs e)
        {
            _isCaretPosChanged = true;
            _caretPosition.Content = "(" + Editor.Document.GetLineByOffset(Editor.CaretOffset).LineNumber + "," + Editor.Document.GetLocation(Editor.CaretOffset).Column + ")";

            TextViewPosition? pos1 = Editor.GetPositionFromPoint(new Point(5, 5));
            TextViewPosition? pos2 = Editor.GetPositionFromPoint(new Point(5, Editor.ActualHeight-10));
            
            if (pos1!=null && Editor.CaretOffset < Editor.Document.GetOffset(pos1.Value))
            {
                Editor.ScrollTo(Editor.Document.GetLineByOffset(Editor.CaretOffset).LineNumber, 0);
            }
            else if (pos2!=null && Editor.CaretOffset > Editor.Document.GetOffset(pos2.Value))
            {
                Editor.ScrollTo(Editor.Document.GetLineByOffset(Editor.CaretOffset).LineNumber, 0);
            }
            //Editor.ScrollTo(Editor.Document.GetLineByOffset(Editor.CaretOffset).LineNumber, 0);

        }

        public void SetCaretOffset(int offset)
        {
            try
            {
                Editor.CaretOffset = offset;
            }
            catch { }//실패할 수도 있슴..
        }

        public void SetHighlightedPath(String xPath, int index = 0)
        {
            AbstractAXmlVisitor visitor = new PrettyPrintAXmlVisitor();
            visitor.VisitDocument(xDoc);
            backgroundRenderer.HighlightObjects = AXmlFinder.GetXmlElementPair(xDoc, xPath, index);

            Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        String _lastText = "";

        /// <summary>
        /// TextChanged 이벤트 없이 Text를 바꾼다.
        /// </summary>
        /// <param name="text"></param>
        public void SetTextWithNoEvent(String text)
        {
            _noEventOnceWhenTextChanged = true;
            T_Text.Text = text;
        }

        bool _noEventOnceWhenTextChanged = false;
        void Editor_TextChanged(object sender, EventArgs e)
        {
            if (_noEventOnceWhenTextChanged == false)
            {
                _textChangeDelay = 10;//Text바뀌고 있는 중에는 대기 시간 초기화..
                _textChangeMode = TextChangingModes.TextChanging;
            }
            else
            {
                _noEventOnceWhenTextChanged = false;
            }

        }

        enum TextChangingModes { Wait, TextChanging };

        TextChangingModes _textChangeMode = TextChangingModes.Wait;
        int _textChangeDelay = 10;
        void textRefreshTimer_Tick()
        {
            if (_textChangeMode == TextChangingModes.Wait)
            {
                //do nothing..
                _textChangeDelay = 10;//초기화..
            }else if (_textChangeMode == TextChangingModes.TextChanging)
            {
                _textChangeDelay--;//0이 될때까지..
                if(_textChangeDelay==0){
                    invokeMethod();
                    _textChangeDelay = 10;
                    _textChangeMode = TextChangingModes.Wait;
                    if(E_TextChanged!=null) E_TextChanged(T_Text.Text, T_Text.Text);
                }
            }
        }

        delegate void voidfunc();
        


        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                if (this.IsVisible)
                {
                    foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);
                }
            }
        }
        

       
        public delegate void eventfunc(object sender, MouseButtonEventArgs e);



        delegate void voidFunc();
        public AXmlDocument xDoc;
        string _lastSafeXml = "";
        void invokeMethod()
        {
            
            if (_lastSafeXml.Equals(T_Text.Text)) {

                return; 
            }
            T_Text.IsEnabled = false;
            AXmlParser parser = new AXmlParser();
            List<DocumentChangeEventArgs> args = new List<DocumentChangeEventArgs>();
            //if(e!=null) args.Add(null);
            try
            {
                parser.Lock.EnterWriteLock();

                xDoc = parser.Parse(Editor.Text, args);

                if (xDoc.SyntaxErrors.Count() > 0)
                {
                    errorBackgroundRenderer.UnderlineColor = Color.FromArgb(0x40, 0xFF, 0, 0);
                    errorBackgroundRenderer.UnderlineSegments = new List<ISegment>();
                    
                    foreach (SyntaxError error in xDoc.SyntaxErrors)
                    {
                        DocumentHighlighter highlighter = new DocumentHighlighter(Editor.Document, Editor.SyntaxHighlighting.MainRuleSet);
                        HighlightedInlineBuilder builder = new HighlightedInlineBuilder(Editor.Text);
                        AXmlObject obj = xDoc.GetChildAtOffset((error.StartOffset + error.EndOffset) / 2);
                        errorBackgroundRenderer.UnderlineSegments.Add(obj);
                        showError(error);
                        //addToolTip(error);
                        //ISegment seg = new TextSegment() { StartOffset = error.StartOffset, EndOffset = error.EndOffset };
                        //errorBackgroundRenderer.UnderlineSegments.Add(seg);
                    }

                    if (E_SyntaxError != null && _eventEnabled) E_SyntaxError(xDoc.SyntaxErrors);

                }
                else
                {
                    ClearStatus();
                    
                    errorBackgroundRenderer.UnderlineSegments = null;
                    if (_lastSafeXml.Length>0)
                    {
                        if (_lastSafeXml.Equals(T_Text.Text)==false)
                        {
                            if (_eventEnabled)
                            {
                                if (E_XmlChanged != null) E_XmlChanged(_lastSafeXml, T_Text.Text);//syntax error가 없어야 저장가능..
                            }
                            XmlChanged();
                        }
                    }
                    _lastSafeXml = T_Text.Text;
                    
                }
                parser.Lock.ExitWriteLock();
            }
            catch
            {
                parser.Lock.ExitWriteLock();
            }
            this.Editor.TextArea.TextView.Redraw();
            T_Text.IsEnabled = true;
        }

        public void ClearStatus()
        {
            _textStatus.Content = "";
            _toolTip = null;
            _statusBar.ToolTip = null;
        }
        
        public void WriteStatus(String str, String writeOnlyTooltip="")
        {
            string msg = str.Replace("\n", "/").Replace("\r", "").Replace("\t", " ");
            _textStatus.Content = msg;
            if (writeOnlyTooltip.Length > 0) addToolTip(writeOnlyTooltip);
            else addToolTip(msg);
        }

        private void addToolTip(string msg)
        {
            if (_toolTip == null)
            {
                _toolTip = new ToolTip();
                _statusBar.ToolTip = _toolTip;
                
                ToolTipService.SetShowDuration(_statusBar, 60000);
                _toolTip.StaysOpen = true;
            }
            if(_toolTip.Content==null) _toolTip.Content = "";
            _toolTip.Content = msg;
            /*
                ((_toolTip.Content.ToString().Length != 0) ? _toolTip.Content.ToString() + "\r\n" : "")
                + msg;
             */
        }
        ToolTip _toolTip = null;
        private void showError(SyntaxError error)
        {
            string msg = error.Message.Replace("\n","/").Replace("\r","").Replace("\t"," ") + ": Line(" + Editor.Document.GetLineByOffset(error.StartOffset).LineNumber + "), Col(" + Editor.Document.GetLocation(error.StartOffset).Column + ")";
            _textStatus.Content = msg;
            addToolTip(msg);
        }

        protected virtual void XmlChanged()
        {

        }

        /// <summary>
        /// 해당 offset에 있는 object에 밑줄을 긋는다.
        /// offset이 -1이면 모든 밑줄을 제거한다.
        /// </summary>
        /// <param name="offset">밑줄 그을 object</param>
        public void SetUnderlineOnOffset(int offset)
        {
            if (offset < 0)
            {
                if(errorBackgroundRenderer.UnderlineSegments!=null)errorBackgroundRenderer.UnderlineSegments.Clear();
                //errorBackgroundRenderer.UnderlineSegments = null;
                return;
            }
            errorBackgroundRenderer.UnderlineColor = Color.FromArgb(0x90, 0xFF, 0, 0);
            errorBackgroundRenderer.UnderlineSegments = new List<ISegment>();
                    
            DocumentHighlighter highlighter = new DocumentHighlighter(Editor.Document, Editor.SyntaxHighlighting.MainRuleSet);
            HighlightedInlineBuilder builder = new HighlightedInlineBuilder(Editor.Text);
            
                AXmlObject obj = xDoc.GetChildAtOffset(offset);
                errorBackgroundRenderer.UnderlineSegments.Add(obj);
            
        }

        



        void B_WordWrap_Click(object sender, RoutedEventArgs e)
        {
            Editor.WordWrap = !(Editor.WordWrap);
        }

        void B_Num_Click(object sender, RoutedEventArgs e)
        {
            Editor.ShowLineNumbers = !(Editor.ShowLineNumbers);
        }

        public TextEditor T_Text
        {
            get { return Editor; }
        }

        public void ParseData()
        {
            invokeMethod();
        }
        bool _eventEnabled = true;
        /// <summary>
        /// reload할 때 event를 호출할 것인지 아닐지에 대해 셋팅한다.
        /// </summary>
        public bool EventEnabled { get { return _eventEnabled; } set { _eventEnabled = value; } }
    }
    
    public delegate void XmlChangedEventHandler(String before, String now);
    public delegate void SelectElementEvent(AXmlObject SelectedObject, AXmlObject element, int index);
    public delegate void TextChangedEvent(String old, String newText);
}
