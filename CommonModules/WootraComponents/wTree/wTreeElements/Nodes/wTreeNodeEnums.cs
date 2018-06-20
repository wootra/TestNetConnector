using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WootraComs.wTreeElements
{
    public enum wTreeNodeItemTypes { 
        /// <summary>
        /// Image
        /// </summary>
        Image, 
        /// <summary>
        /// Text
        /// </summary>
        Text,
        /// <summary>
        /// Text
        /// </summary>
        TextArray, 
        /// <summary>
        /// Checkbox
        /// </summary>
        CheckBox, 
        /// <summary>
        /// Windows.Forms.Control
        /// </summary>
        Control, 
        /// <summary>
        /// 다음 줄로 넘어가거나 한 칸을 띄움.
        /// </summary>
        WhiteSpace,
        /// <summary>
        /// plus minus영역..
        /// </summary>
        PlusMinus,
    };

    public enum TextEditorTypes
    {
        None=0,
        TextBox=1,
        Custom=3,
    }

    public enum TextArrayEditorTypes
    {
        None = 0,
        ComboBox = 2,
        Custom = 3,
    }

    public enum ImageEditorTypes
    {
        None=0,
        ImageSelector=1,
        Custom=2,
    }
    public enum EditorActivateBasicActions { Programatic = 1, DoubleClick, CtrlDoubleClick, AltDoubleClick, ShiftDoubleClick, CtrlClick, AltClick, ShiftClick, RightClick, RightDoubleClick, ClickOnSelection, Click, KeyPress };
    public enum EditorActivateActions { UseBasicSetting = 0, Programatic, DoubleClick, CtrlDoubleClick, AltDoubleClick, ShiftDoubleClick, CtrlClick, AltClick, RightClick, ShiftClick, RightDoubleClick, ClickOnSelection, Click, KeyPress };
    public enum CheckboxActiveActions { Click, DoubleClick, Programatic };
    public enum WhiteSpaceTypes { Space, Return };

    public enum BasicDrawings { 
        /// <summary>
        /// 추가로 그리는 것이 없슴.
        /// </summary>
        None=0, 
        /// <summary>
        /// PlusMinus를 표시함..
        /// </summary>
        PlusMinus=1, 
        /// <summary>
        /// 앞에 라인을 그림..
        /// </summary>
        Lines=1<<2,
        /// <summary>
        /// Selection을 표시함..
        /// </summary>
        Selection=1<<3,
        /// <summary>
        /// 여러개를 선택할 수있음..
        /// </summary>
        MultiSelections=(1<<4)|(1<<3),//MultiSelection은 항상 Selection이 On이 되어야 함..
        /// <summary>
        /// RollOver를 표시함..
        /// </summary>
        RollOver=1<<5,
    };

    public enum ContainsModes { X, Y, XY };

    /// <summary>
    /// Node의 Action에 적용될 Event목록
    /// </summary>
    public enum EventsForNode
    {
        Click,
        DoubleClick,
        MouseButtonPress,
        MouseButtonUp,
        WheelUp,
        WheelDown,
        KeyDown,
        KeyUp,
        DownKey
    }
}
