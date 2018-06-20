using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace WootraComs.wTreeElements
{
    public class EditorHandlerClass
    {

        public event wTreeNodeItemValueChanged E_TreeNodeItemValueChanged;
        public event wTreeNodeItemValueChangeCanceled E_TreeNodeItemValueChangeCanceled;
        public event wTreenodeEditorVisibleChanged E_TreeNodeEditorStarted;
        public event wTreenodeEditorVisibleChanged E_TreeNodeEditorFinished;
        public event EditorValueChanging E_TreeNodeEditorValueChanging;

        wTree _ownerTree;
        internal EditorHandlerClass(wTree ownerTree)
        {
            _ownerTree = ownerTree;
            EditorActivateAction = EditorActivateBasicActions.CtrlDoubleClick;
            BasicTextEditor = TextEditorTypes.TextBox;
            BasicTextArrayEditor = TextArrayEditorTypes.ComboBox;
            BasicImageEditor = ImageEditorTypes.ImageSelector;
           
        }

        internal void InitEditors()
        {
            _textBoxEditor = new TextBoxEditor(new TextBox(), _ownerTree);
            InitAEditor(_textBoxEditor);
            _comboBoxEditor = new ComboBoxEditor(new ComboBox(), _ownerTree);
            InitAEditor(_comboBoxEditor);
            _imageSelector = new ImageSelectEditor(new UserControl(), _ownerTree);
            InitAEditor(_imageSelector);
        }

        /// <summary>
        /// Editor가 이 트리와 연동되게 한다.
        /// </summary>
        /// <param name="editor"></param>
        public void InitAEditor(wTreeEditor editor)
        {
            editor.E_EditorValueChangeCanceled += editor_E_EditorValueChangeCanceled;
            editor.E_EditorValueChanged += editor_E_EditorValueChanged;
            editor.E_EditorValueChanging += editor_E_EditorValueChanging;
        }

        void editor_E_EditorValueChanging(wTreeNodeItem item, wEditorValueChangingArgs args)
        {
            if(E_TreeNodeEditorValueChanging!=null) E_TreeNodeEditorValueChanging(item, args);
        }
       

        void editor_E_EditorValueChanged(wTreeNodeItem item, object oldValue, object newValue)
        {
            if (E_TreeNodeItemValueChanged != null) E_TreeNodeItemValueChanged(item.OwnerNode, item, oldValue, newValue);
        }

        void editor_E_EditorValueChangeCanceled(wTreeNode node, wTreeNodeItem item)
        {
            if (E_TreeNodeItemValueChangeCanceled != null) E_TreeNodeItemValueChangeCanceled(node, item);
        }
       
        wTreeEditor _activatedEditor = null;
        /// <summary>
        /// 현재 활성화 된 Editor..
        /// </summary>
        public wTreeEditor ActivatedEditor { get { return _activatedEditor; } }


        /// <summary>
        /// 현재 활성화된 editor를 숨긴다.
        /// </summary>
        public void HideEditor()
        {
            if (_activatedEditor != null)
            {
                
                //_activatedEditor.EditorControl.Hide();
                if (E_TreeNodeEditorFinished != null) E_TreeNodeEditorFinished(_activatedEditor.ItemToEdit, _activatedEditor);
                _ownerTree.Controls.Remove(_activatedEditor.EditorControl);
                _activatedEditor = null;
            }
        }


        TextBoxEditor _textBoxEditor;
        public TextBoxEditor TextBoxEditor { get { return _textBoxEditor; } }

        ComboBoxEditor _comboBoxEditor;
        public ComboBoxEditor ComboBoxEditor { get { return _comboBoxEditor; } }

        ImageSelectEditor _imageSelector;
        public ImageSelectEditor ImageSelector { get { return _imageSelector; } }

        public EditorActivateBasicActions EditorActivateAction { get; set; }

        /// <summary>
        /// wTreeNodeItem의 TextItem에 따로 Editor를 지정하지 않았으면 기본적으로 제공될 TextEditor..
        /// </summary>
        public TextEditorTypes BasicTextEditor { get; set; }

        /// <summary>
        /// wTreeNodeItem의 TextItem에 따로 Editor를 지정하지 않았으면 기본적으로 제공될 TextEditor..
        /// </summary>
        public TextArrayEditorTypes BasicTextArrayEditor { get; set; }

        /// <summary>
        /// wTreeNodeItem의 ImageItem에 따로 Editor를 지정하지 않았으면기본적으로 제공될 ImageEditor...
        /// </summary>
        public ImageEditorTypes BasicImageEditor { get; set; }

        /// <summary>
        /// ActiveEditor로 등록하고 보여준다.
        /// </summary>
        /// <param name="editor"></param>
        internal void ShowEditor(wTreeEditor editor, Rectangle area)
        {
            if (_activatedEditor != null && _activatedEditor!=editor) HideEditor();
            _activatedEditor = editor;
            if (editor != null)
            {
                editor.EditorControl.Width = area.Width;
                editor.EditorControl.Height = area.Height;
                _ownerTree.wDrawHandler.SetControlPositionInMain(editor.EditorControl, editor.EditorPosition);
                _activatedEditor.EditorControl.Focus();

                E_TreeNodeEditorStarted(editor.ItemToEdit, editor);
            }
            
        }

        public void ShowEditor(wTreeNodeItem item, Rectangle area)
        {
            if (item.ItemType == wTreeNodeItemTypes.Image)
            {
                if (item.ImageEditorType == ImageEditorTypes.Custom)
                {
                    item.CustomEditor.SetValue(item.Value);
                    if(item.CustomEditor!=null) ShowEditor(item.CustomEditor, area);
                }
                else if (item.ImageEditorType == ImageEditorTypes.ImageSelector)
                {
                    _imageSelector.ShowEditorFor(item, area);
                }
                else if (item.ImageEditorType == ImageEditorTypes.None)
                {
                    if (BasicImageEditor == ImageEditorTypes.ImageSelector)
                    {
                        _imageSelector.ShowEditorFor(item, area);
                    }
                    
                }
            }
            else if (item.ItemType == wTreeNodeItemTypes.Text)
            {
                if (item.TextEditorType == TextEditorTypes.None)
                {
                    if (BasicTextEditor == TextEditorTypes.TextBox)
                    {
                        _textBoxEditor.ShowEditorFor(item, area);
                    }
                    
                    
                }
                else if (item.TextEditorType == TextEditorTypes.TextBox)
                {
                    _textBoxEditor.ShowEditorFor(item, area);
                }
                
                else if (item.TextEditorType == TextEditorTypes.Custom)
                {
                    if (item.CustomEditor != null)
                    {
                        item.CustomEditor.ShowEditorFor(item, area);
                    }
                }
            }
            else if (item.ItemType == wTreeNodeItemTypes.TextArray)
            {
                if (item.TextArrayEditorType == TextArrayEditorTypes.None)
                {
                    if (BasicTextArrayEditor == TextArrayEditorTypes.ComboBox)
                    {
                        Rectangle rect = new Rectangle(area.X, area.Y, area.Width + 10, area.Height);
                        _comboBoxEditor.ShowEditorFor(item, rect);
                    }
                }
                else if (item.TextArrayEditorType == TextArrayEditorTypes.ComboBox)
                {
                    Rectangle rect = new Rectangle(area.X, area.Y, area.Width + 10, area.Height);
                    _comboBoxEditor.ShowEditorFor(item, rect);
                }
                else if (item.TextArrayEditorType == TextArrayEditorTypes.Custom)
                {
                    if (item.CustomEditor != null)
                    {
                        item.CustomEditor.ShowEditorFor(item, area);
                    }
                }
            }
        }

        internal void OnNodeItemValueChangeCanceled(wTreeNode node, wTreeNodeItem item)
        {
            E_TreeNodeItemValueChangeCanceled(node, item);
        }

        internal void OnNodeItemValueChanged(wTreeNode node, wTreeNodeItem item, object oldValue, object newValue)
        {
            E_TreeNodeItemValueChanged(node, item, oldValue, newValue);
        }
    }
}
