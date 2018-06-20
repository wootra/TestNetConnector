using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;

namespace FormAdders.EasyGridViewCollections
{
    #region enums
    public enum ItemTypes { TextBox = 0, CheckBox, ComboBox, CheckBoxGroup, Button, Image, ImageButton, ImageCheckBox, CloseButton, Header, RadioButton, KeyValue, Various, BlankBack, FileOpenBox };
    public enum RtwListChildControl { CloseButton = 0, CheckBox };

    /// <summary>
    /// 컨트롤에서 자동으로 동작할 액션 목록입니다.
    /// 주의: Modify는 TextBox에서만 동작할 것입니다.
    /// </summary>
    public enum Actions { Modify = 0, CheckBoxChecked, ContextMenu, Nothing, Auto, CommonAction, CopyToClipBoard };

    public enum SpanPosition { SpanBase = 0, Spanned, NoSpan };
       

    /// <summary>
    /// Action이 수행되는 조건 목록입니다. setColumnAction()함수에서 사용됩니다.
    /// </summary>
    public enum ActionConditions { Clicked, DoubleClicked, RightClicked };

    public enum ActionsOnCtrl_MoveKey { None = 0, SelectRowsMove, CheckedRowsMove };

    public enum EasyGridRadioBoxOrientation { Horizontal = 0, Vertical, VerticalFirstInHeight };

    public enum RadioButtonColors { Green = 1, Blue, Red, Orange };

    public enum CheckBoxColors { Red = 1, Blue = 2 };

    public enum EnterActions { EditOnThePosition = 0, EditNextRow };

    public enum TextViewModes { SystemFontSize = 0, ResizeForCellWid, MultiLines, Default };

    public enum TextAlignModes { Right, Left, NumberOnlyRight, Center, NumberRightTextCenter, None };

    public enum RowHeightSettingModes { UpdateMaxNow, UpdateMinNow, UpdateWithThisNow, SetWithThis, SetForMin, SetForMax };

    public enum RowBackModes { None = 0, Red, Blue, Gray, CustomColor, CellColor };

    public enum ImageSizeModes { Fixed, Stretch };
    #endregion
}
