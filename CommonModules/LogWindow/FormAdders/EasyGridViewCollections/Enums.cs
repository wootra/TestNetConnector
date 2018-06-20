using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;

namespace FormAdders.EasyGridViewCollections
{
    #region enums
    public enum ItemTypes { TextBox = 0, CheckBox, ComboBox, CheckBoxGroup, Button, Image, ImageCheckBox, CloseButton, Header, RadioButton, KeyValue, KeyColor, Various };
    public enum RtwListChildControl { CloseButton = 0, CheckBox };

    /// <summary>
    /// 컨트롤에서 자동으로 동작할 액션 목록입니다.
    /// 주의: Modify는 TextBox에서만 동작할 것입니다.
    /// </summary>
    public enum Actions { Modify = 0, CheckBoxChecked, ContextMenu, Nothing, Auto, CommonAction, CopyToClipBoard };



    /// <summary>
    /// Action이 수행되는 조건 목록입니다. setColumnAction()함수에서 사용됩니다.
    /// </summary>
    public enum ActionConditions { Clicked, DoubleClicked, RightClicked };

    public enum ActionsOnCtrl_MoveKey { None = 0, SelectRowsMove, CheckedRowsMove };

    public enum EasyGridRadioBoxOrientation { Horizontal = 0, Vertical, VerticalFirstInHeight };

    public enum RadioButtonColors { Green = 1, Blue, Red, Orange };

    public enum CheckBoxColors { Red = 1, Blue = 2 };

    public enum EnterActions { EditOnThePosition = 0, EditNextRow };
    #endregion
}
