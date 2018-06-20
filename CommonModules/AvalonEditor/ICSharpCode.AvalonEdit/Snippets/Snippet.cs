// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// A code snippet that can be inserted into the text editor.
	/// </summary>
	[Serializable]
	public class Snippet : SnippetContainerElement
	{
        public InsertionContext Context = null;
        public static bool IsSnippetActivated = false;
		/// <summary>
		/// Inserts the snippet into the text area.
		/// </summary>
		public void Insert(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			
			ISegment selection = textArea.Selection.SurroundingSegment;
			int insertionPosition = textArea.Caret.Offset;
			
			if (selection != null) // if something is selected
				// use selection start instead of caret position,
				// because caret could be at end of selection or anywhere inside.
				// Removal of the selected text causes the caret position to be invalid.
				insertionPosition = selection.Offset + TextUtilities.GetWhitespaceAfter(textArea.Document, selection.Offset).Length;
			
			Context = new InsertionContext(textArea, insertionPosition);

			using (Context.Document.RunUpdate()) {
				if (selection != null)
					textArea.Document.Remove(insertionPosition, selection.EndOffset - insertionPosition);
				Insert(Context);
				Context.RaiseInsertionCompleted(EventArgs.Empty);
			}
            IsSnippetActivated = true;
		}
        
        IActiveElement _activeElement = null;

        public ISegment GetNextReplaceableActiveElement(bool stopWhenReachOut = true)
        {
            if (Context != null && IsSnippetActivated)
            {
                bool isCurrent = false;
                foreach(IActiveElement element in Context.ActiveElements){
                    ReplaceableActiveElement re = element as ReplaceableActiveElement;
                    if (re != null)
                    {
                        if (_activeElement == null)
                        {
                            _activeElement = re;
                            IsSnippetActivated = true;
                            return re.Segment;
                        }
                        else
                        {
                            if (_activeElement.Equals(re)) isCurrent = true;
                            else if (isCurrent)
                            {
                                _activeElement = re;
                                
                                IsSnippetActivated = true;
                                return re.Segment;
                            }
                        }
                    }
                }
            }
            if (stopWhenReachOut)
            {
                return _activeElement.Segment;
            }else{
                _activeElement = null;
                IsSnippetActivated = false;
                return null;//제일 마지막까지 갔는데 없다면 모든 Active snippet을 돈 것이다.
            }
        }

        public ISegment GetFirstReplaceableActiveElement()
        {
            if (Context != null && IsSnippetActivated)
            {
                foreach (IActiveElement element in Context.ActiveElements)
                {
                    ReplaceableActiveElement re = element as ReplaceableActiveElement;
                    if (re != null)
                    {
                        return re.Segment;
                    }
                }
            }
            
            return null;//제일 마지막까지 갔는데 없다면 모든 Active snippet을 돈 것이다.
        }

        public ISegment GetLastReplaceableActiveElement()
        {
            if (Context != null && IsSnippetActivated)
            {
                ReplaceableActiveElement last = null;
                foreach (IActiveElement element in Context.ActiveElements)
                {
                    ReplaceableActiveElement re = element as ReplaceableActiveElement;
                    if (re != null)
                    {
                        last = re;
                    }
                }
                return last.Segment;
            }
            return null;
        }


        public ISegment GetReplaceableActiveElement(int index)
        {
            if (Context != null && IsSnippetActivated)
            {
                int count = 0;
                foreach (IActiveElement element in Context.ActiveElements)
                {
                    ReplaceableActiveElement re = element as ReplaceableActiveElement;
                    if (re != null)
                    {
                        if (count == index) return re.Segment;
                        else count++;
                    }
                }
            }
            return null;
        }

        public int GetCountOfReplaceableActiveElement()
        {
            if (Context != null && IsSnippetActivated)
            {
                int count = 0;
                foreach (IActiveElement element in Context.ActiveElements)
                {
                    ReplaceableActiveElement re = element as ReplaceableActiveElement;
                    if (re != null)
                    {
                        count++;
                    }
                }
                return count;
            }
            return 0;
        }

        public int GetIndexOfActivatedReplaceableActiveElement()
        {
            if (Context != null && IsSnippetActivated)
            {
                int count = 0;
                foreach (IActiveElement element in Context.ActiveElements)
                {
                    ReplaceableActiveElement re = element as ReplaceableActiveElement;
                    if (re != null)
                    {
                        if (re.Equals(_activeElement)) return count;
                        count++;
                    }
                }
                return count;
            }
            return 0;
        }


        public ISegment GetBeforeReplaceableActiveElement(bool stopWhenReachOut=true)
        {
            if (Context != null && IsSnippetActivated)
            {
                ReplaceableActiveElement beforeElement = null;
                foreach (IActiveElement element in Context.ActiveElements)
                {
                    ReplaceableActiveElement re = element as ReplaceableActiveElement;

                    if (re != null)
                    {
                        
                        if (_activeElement.Equals(re))
                        {
                            if (beforeElement == null)//이번 것이 첫 snippet이고, 이전에 선택된 것과 같을 경우..
                            {
                                if (stopWhenReachOut) return re.Segment; //더이상 움직이지 않는다.
                                else//끝낸다.
                                {
                                    IsSnippetActivated = false;
                                    _activeElement = null;
                                    return null;
                                }
                            }
                            else
                            {
                                _activeElement = beforeElement;
                                IsSnippetActivated = true;
                                return beforeElement.Segment;
                            }
                        }
                        else
                        {
                            beforeElement = re;//계속 저장하면서 나간다.
                        }
                    }
                }
                if (beforeElement != null)//제일 마지막까지 왔다면 뒤에서부터 처음 시작했다는 의미이다.
                {
                    _activeElement = beforeElement;
                    IsSnippetActivated = true;
                    return beforeElement.Segment;
                }
            }

            return null;//제일 마지막까지 갔는데 없다면 ActiveSnippet이 하나도 없는 것이다.
        }
	}
}
