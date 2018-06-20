using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IOHandling;
using FormAdders;

namespace FormAdders.EasyGridViewCollections
{

    public class EasyGridViewParent : DataGridView
    {
        //int Now.Row = -1;
        //int Now.Col = -1;
        PositionOnList Now;
        PositionOnList Before;
        List<bool?> _editables;
        List<ItemTypes> _itemTypes;
        //bool _isEditing = false;
        bool _goBack = false;
        //enum Direction { Left = 0, Right, Up, Down, OnPosition, NoPositioning };
        //Direction _posAfterEdit = Direction.OnPosition;

        public event CellTextChangedEventHandler E_TextChanged;
        public event CellTextChangedEventHandler E_TextEditFinished;
        public event CellClickEventHandler E_CellSelected;

        PreviewKeyDownEventHandler _editingControlEvent;

        /// <summary>
        /// 편집모드에서 엔터를 쳤을 때의 액션목록입니다.
        /// </summary>

        public EasyGridViewParent()
            : base()
        {
            _editingControlEvent = new PreviewKeyDownEventHandler(EditingControl_PreviewKeyDown);
            Now = new PositionOnList(-1, -1);
            Before = new PositionOnList(-1, -1);
            _editables = new List<bool?>();
            _itemTypes = new List<ItemTypes>();
        }


        EnterActions _actionOnEnterOnEditMode = EnterActions.EditNextRow;

        public EnterActions ActionOnEnterInEditMode
        {
            get
            {
                return _actionOnEnterOnEditMode;
            }
            set
            {
                _actionOnEnterOnEditMode = value;
            }
        }

        public void setEditables(List<bool?> editables, List<ItemTypes> itemType)
        {
            _editables = editables;
            _itemTypes = itemType;
        }

        public DataGridViewCell Cell(PositionOnList pos)
        {
            return this.Rows[pos.Row].Cells[pos.Col];
        }

        public DataGridViewCell Cell(int row, int col)
        {
            return this.Rows[row].Cells[col];
        }

        bool _willSelectionChange = true;
        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseDown(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _willSelectionChange = false;
                _goBack = false;
                if (e.RowIndex > 0 && e.ColumnIndex > 0)
                {
                    try
                    {
                        if (Control.ModifierKeys != Keys.Control && Control.ModifierKeys != Keys.Shift)
                        {
                            Cell(Now).Selected = false;
                            Cell(Before).Selected = false;
                        }
                    }
                    catch { }
                    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    {
                        DataGridViewTextBoxCell textCell = Cell(e.RowIndex, e.ColumnIndex) as DataGridViewTextBoxCell;
                        if (textCell != null) BeforeText = textCell.Value as String;
                    }
                    Before.Set(e.RowIndex, e.ColumnIndex);
                    Now.Set(e.RowIndex, e.ColumnIndex);
                    CurrentCell = Cell(Now);
                    if (this.SelectedRows.Count == 0 || this.SelectedCells.Count == 0)
                    {
                        CurrentCell.Selected = true;
                    }
                    //CurrentCell.Selected = !(CurrentCell.Selected);
                }
            }
            else
            {
                _willSelectionChange = false;
            }
        }

        bool _isCellSelectedEventActivated = true;
        public void suspendCellSelectedEvent() { _isCellSelectedEventActivated = false; }
        public void resumeCellSelectedEvent() { _isCellSelectedEventActivated = true; }

        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            //cell Enter의 경우에는 EditingControl은 항상 null이다.
            //Go Back을 할 것인지, 그대로있을 것인지가 중요하다. GoBack을 할 경우에는 과거 있었던 곳이 중요하다.
            //이때 Before를 사용하고, 다른 경우에는 모두 다음 길을 찾은 이후가 될 것이므로 Before가 중요하지 않다.

            if (_goBack) //이 경우는 Enter를 쳤을 때 밖에는 없다.
            {
                //_goBack = false;//한번 goBack을 하고 나면 다시 되돌아가지 않도록 하기위해..
                Cell(Before).Selected = true;
                Cell(e.RowIndex, e.ColumnIndex).Selected = false;//현재 들어온 곳의 선택을 없앰..
                Now.Set(Before); //현재 선택을 Now로 바꿈..
                base.OnCellEnter(new DataGridViewCellEventArgs(Now.Col, Now.Row));
            }
            else //이외의 경우, Now를 현재 위치로 갱신해 준다.
            {
                Now.Set(e.RowIndex, e.ColumnIndex);
            }
            if (_itemTypes[Now.Col] == ItemTypes.Various)
            {
                IEasyGridCell cell = Cell(Now) as IEasyGridCell;
                if (E_CellSelected != null && _isCellSelectedEventActivated) E_CellSelected(this, new CellClickEventArgs(Now.Row, Now.Col, Rows[Now.Row], cell.ItemType, Cell(Now), this, e));
            }
            else
            {
                if (E_CellSelected != null && _isCellSelectedEventActivated) E_CellSelected(this, new CellClickEventArgs(Now.Row, Now.Col, Rows[Now.Row], _itemTypes[Now.Col], Cell(Now), this, e));
            }
            /*
            if (this.EditingControl == null && _isEditing)
            {
                if (Before.Col >= 0 && Before.Row >= 0)
                {
                    if (_posAfterEdit == Direction.NoPositioning)
                    {
                        //Before.Set(Now);
                        Now.Set(e.RowIndex, e.ColumnIndex);
                    
                    }else if (_posAfterEdit == Direction.OnPosition)
                    {

                        //_isEditing = false;

                        Now.Set(Before);//이전으로 복귀
                        this.CurrentCell.Selected = false;
                        cell(Before).Selected = true;

                        //this.NotifyCurrentCellDirty(true);

                        return;

                    }
                    else
                    {
                        
                        if (_posAfterEdit == Direction.Down)
                        {
                            cell(Before).Selected = false;
                            if (Now.Row < this.Rows.Count - 1)
                            {
                                Now.Set(++Now.Row, Now.Col);
                                cell(Now).Selected = true;

                                base.OnCellEnter(new DataGridViewCellEventArgs(Now.Col, Now.Row));
                            }
                            _isEditing = false;
                            return;
                        }
                        else if (_posAfterEdit == Direction.Left)
                        {
                            _isEditing = false;

                        }
                        else if (_posAfterEdit == Direction.Right)
                        {
                            _isEditing = false;
                            //return;
                        }
                        else if (_posAfterEdit == Direction.Up)
                        {
                            cell(Before).Selected = false;
                            if (Now.Row > 0)
                            {
                                Now.Set(--Now.Row, Now.Col);

                                //Before.Set(Now);
                                this.Rows[Now.Row].Cells[Now.Col].Selected = true;

                                base.OnCellEnter(new DataGridViewCellEventArgs(Now.Col, Now.Row));
                            }
                            _isEditing = false;
                            return;
                        }
                    }
             
                }
            }
             * */


            base.OnCellEnter(e);
        }
        /*
        protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    Now.Set(e.RowIndex, e.ColumnIndex);
                    if (Before.Col >= 0 && Before.Row >= 0)
                    {

                        this.Rows[Before.Row].Cells[Before.Col].Selected = false;
                    }
                    Before.Set(Now);
                    this.CurrentCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    this.CurrentCell.Selected = true;
                }
            }
            catch { }
            base.OnCellMouseClick(e);
        }
 */
        protected override void OnCellLeave(DataGridViewCellEventArgs e)
        {//cell을 떠나는 경우는 CurrentCell이 있는 경우이다.
            //있을 수 있는경우의 수는, 세 가지이다.
            //마우스로 인하여 이전에서 옮겨지는 경우, Before가 의미가 없어진다.
            //키보드의 방향키로 인해 위치가 옮겨지는 경우, 역시 Before의 의미가 없다. 
            //Enter로 그 자신의 위치로 다시 옮겨지는 경우에만 Before가 의미가 있다.

            if (_goBack)
            {
                Before.Set(e.RowIndex, e.ColumnIndex);
            }
            if (this.EditingControl != null && _isEditActivated) //편집모드였다가 나갈 때는 값이 갱신되었다면 이벤트를 호출시킨다.
            {
                String nowText = this.EditingControl.Text.ToString();// this.CurrentCell.Value.ToString();
                this.CurrentCell.Value = nowText;
                if (_isEditActivated)
                {
                    CellTextChangedEventArgs args = new CellTextChangedEventArgs(BeforeText, nowText, Before.Col, Before.Row, this.Rows[Before.Row], this.Rows[Before.Row].Cells[Before.Col] as DataGridViewTextBoxCell);
                    if (BeforeText.Equals(nowText) == false)
                    {
                        if (E_TextChanged != null) E_TextChanged(this, args);
                        if (args.IsCancel)
                        {
                            this.EditingControl.Text = BeforeText;
                        }
                    }
                    if (E_TextEditFinished != null) E_TextEditFinished(this, args);
                    _isEditActivated = false;
                }
            }

            /*
            if (this.EditingControl == null && _isEditing)
            {
                if (ActionOnEnterInEditMode == EnterActions.EditNextColumn)
                {

                }
                else if (ActionOnEnterInEditMode == EnterActions.EditNextRow)
                {
                }
                else //ActionOnEnterInEditMode == EnterActions.EditOnThePosition
                {
            
                        return;
                }
            }
            */
            /*
            if (Now.Row >= 0 && Now.Col >= 0)
            {
                
                if (this.CurrentCell!=null && this.CurrentCell is DataGridViewTextBoxCell)
                {
                    if (_isEditing && this.EditingControl!=null)
                    {
                        String nowText = this.EditingControl.Text.ToString();// this.CurrentCell.Value.ToString();
                        this.CurrentCell.Value = nowText;
                        if (_isEditActivated)
                        {
                            CellTextChangedEventArgs args = new CellTextChangedEventArgs(BeforeText, nowText, Before.Col, Before.Row, this.Rows[Before.Row], this.Rows[Before.Row].Cells[Before.Col] as DataGridViewTextBoxCell);
                            if (BeforeText.Equals(nowText) == false)
                            {
                                if (E_TextChanged != null) E_TextChanged(this, args);

                            }
                            if (E_TextEditFinished != null) E_TextEditFinished(this, args);
                            _isEditActivated = false;
                        }
                    }
                }
                if(_posAfterEdit!=_posAfterEdit) Before.Set(Now);
                
            }
            else
            {
                Before.Set(e.RowIndex, e.ColumnIndex);
            }
            */
            base.OnCellLeave(e);
        }


        protected override void OnCellParsing(DataGridViewCellParsingEventArgs e)
        {
            base.OnCellParsing(e);

            String nowText = e.Value as String;// this.CurrentCell.Value.ToString();
            CellTextChangedEventArgs args = new CellTextChangedEventArgs(BeforeText, nowText, Now.Col, Now.Row, this.Rows[Now.Row], this.Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
            if (_isEditActivated)
            {
                if (BeforeText.Equals(nowText) == false)
                {
                    if (E_TextChanged != null) E_TextChanged(this, args);
                    if (args.IsCancel)
                    {
                        if (EditingControl != null) this.EditingControl.Text = BeforeText;
                        Cell(Now).Value = BeforeText;
                    }

                }
                if (E_TextEditFinished != null) E_TextEditFinished(this, args);
                _isEditActivated = false;
            }

        }

        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            base.OnCellBeginEdit(e);
            Before.Set(e.RowIndex, e.ColumnIndex);
            _isEditActivated = true;
        }


        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewTextBoxCell textCell = Cell(e.RowIndex, e.ColumnIndex) as DataGridViewTextBoxCell;
                if (textCell != null) BeforeText = textCell.Value as String;
            }
            base.OnCellDoubleClick(e);
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            String nowText = this.CurrentCell.Value as String;

            CellTextChangedEventArgs args = new CellTextChangedEventArgs(BeforeText, nowText, Now.Col, Now.Row, this.Rows[Now.Row], this.Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
            if (_isEditActivated)
            {
                if (BeforeText.Equals(nowText) == false)
                {
                    if (E_TextChanged != null) E_TextChanged(this, args);
                    if (args.IsCancel)
                    {
                        if (EditingControl != null) this.EditingControl.Text = BeforeText;
                        Cell(Now).Value = BeforeText;
                    }
                }
                if (E_TextEditFinished != null) E_TextEditFinished(this, args);
                _isEditActivated = false;
            }
            base.OnCellEndEdit(e);
        }


        protected override void SetSelectedCellCore(int columnIndex, int rowIndex, bool selected)
        {
            //Rows[rowIndex].Selected = selected;
            //Rows[rowIndex].Cells[columnIndex].Selected = selected;

            base.SetSelectedCellCore(columnIndex, rowIndex, selected);

        }

        
        protected override void SetSelectedRowCore(int rowIndex, bool selected)
        {
            (Rows[rowIndex] as EasyGridRow).Selected = selected;
            base.SetSelectedRowCore(rowIndex, selected);
        }

        bool _isEditActivated = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (CurrentCell == null || CurrentCell.ColumnIndex < 0 || CurrentCell.RowIndex < 0) //current cell이 없다는 것은 key의 효력이 없다.
            {
                base.OnKeyDown(e);
                return;
            }
            if (EditingControl != null)
            {
                base.OnKeyDown(e);
                return;
            }

            if (_goBack)//이전에 _goBack이 있었다면,
            {
                _goBack = false;
                CurrentCell = Cell(Before);//이전셀로 돌아간다. 이 때,LeaveCell과 EnterCell을 지나친다.
                Now.Set(Before);
            }
            else
            {
                Now.Set(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
            }

            //방향키
            #region 방향키..
            if (Control.ModifierKeys == Keys.None)
            {

                if (e.KeyData == Keys.Right) //next col then next row
                {
                    CurrentCell.Selected = false;
                    if (GoRight() == false)
                    {
                        CurrentCell = Cell(Now);
                        CurrentCell.Selected = true;
                        /*
                    if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                    {
                        this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                        this.CurrentCell.Selected = true;
                    }
                     */
                        return;
                    }
                    CurrentCell.Selected = true;
                }
                else if (e.KeyData == Keys.Left) //go left
                {
                    CurrentCell.Selected = false;
                    if (GoLeft() == false)
                    {
                        CurrentCell = Cell(Now);
                        CurrentCell.Selected = true;
                        /*
                       if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                       {
                           this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                           this.CurrentCell.Selected = true;
                       }
                        */
                        return;
                    }
                    CurrentCell.Selected = true;

                }

                else if (e.KeyData == Keys.Down) //next row
                {

                }
                else if (e.KeyData == Keys.Up) //next row
                {

                }
                else if (e.KeyData == Keys.Enter) //next row
                {
                    //Now.Row++;
                    if (ActionOnEnterInEditMode == EnterActions.EditNextRow)
                    {

                    }
                    else if (ActionOnEnterInEditMode == EnterActions.EditOnThePosition)
                    {
                        //BeginEdit(true);//Edit Start and Go Nowhere
                        //return;
                    }
                    else //go next col
                    {
                        CurrentCell.Selected = false;
                        if (GoRight() == false)
                        {
                            CurrentCell = Cell(Now);
                            CurrentCell.Selected = true;
                            /*
                            if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                            {
                                this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                                this.CurrentCell.Selected = true;
                            }
                             */
                            return;
                        }
                        CurrentCell.Selected = true;
                    }

                }

                else if (e.KeyData == Keys.Space)
                {

                }
                else if (e.KeyData == Keys.Delete)
                {
                    this.CurrentCell.Value = "";//.GetData("{0:S}");
                    this.BeginEdit(false);
                    this.CurrentCell.Value = BeforeText;
                }
                else //다른 키일때
                {

                    if (_editables[Now.Col]==true)
                    {
                        DataGridViewTextBoxCell cell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
                        if (cell == null) //이런 경우는 있으면 안되지만 사용자의 실수를 고려하여..
                        {
                            base.OnKeyDown(e);
                            return;
                        }
                        if (this.EditingControl == null)
                        {
                            //this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
                            BeforeText = this.CurrentCell.Value as String;
                        }

                        if ((int)e.KeyData >= (int)Keys.A && (int)e.KeyData <= (int)Keys.Z)
                        {

                            if (Win32APIs.GetKeyState(Keys.CapsLock) == false)
                            {
                                this.CurrentCell.Value = ((char)('a' + ((int)e.KeyData - (int)'A'))).ToString();
                            }
                            else
                            {
                                this.CurrentCell.Value = e.KeyCode.ToString();
                            }
                        }
                        else if ((int)e.KeyData >= (int)Keys.D0 && (int)e.KeyData <= (int)Keys.D9)
                        {
                            this.CurrentCell.Value = ((int)e.KeyData - (int)Keys.D0).ToString();

                        }

                        // V_Data.CurrentCell.Selected = false;
                        this.BeginEdit(false);
                        _isEditActivated = true;
                        if (this.EditingControl != null)
                        {
                            this.EditingControl.PreviewKeyDown += _editingControlEvent;
                            this.CurrentCell.Value = BeforeText;
                        }
                    }


                }
            }
            else if (e.KeyData == (Keys.V | Keys.Control) && e.Control)
            {
                this.CurrentCell.Value = Clipboard.GetText();//.GetData("{0:S}");
                this.BeginEdit(false);
                this.CurrentCell.Value = BeforeText;
            }
            else if (e.KeyData == (Keys.C | Keys.Control) && e.Control)
            {
                Clipboard.SetText(this.CurrentCell.Value as String);
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                if (e.KeyData == (Keys.Right | Keys.Control)) //next col then next row
                {

                }
                else if (e.KeyData == (Keys.Left | Keys.Control)) //go left
                {


                }

                else if (e.KeyData == (Keys.Down | Keys.Control)) //next row
                {

                }
                else if (e.KeyData == (Keys.Up | Keys.Control)) //next row
                {

                }
                return;//selection을 움직이지 않는다.
            }
            else if (Control.ModifierKeys == Keys.Alt)
            {
                return;
            }
            else if (e.KeyData == Keys.Tab && e.Shift == false) //next col then next row
            {
                CurrentCell.Selected = false;
                if (GoRight(true) == false)
                {
                    CurrentCell = Cell(Now);
                    CurrentCell.Selected = true;
                    /*
                    if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                    {
                        this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                        this.CurrentCell.Selected = true;
                    }
                     */
                    return;
                }
                CurrentCell.Selected = true;

            }
            else if (e.KeyData == Keys.Tab && e.Shift == true) //go left in textfield
            {
                CurrentCell.Selected = false;

                if (GoLeft(true) == false)
                {
                    CurrentCell = Cell(Now);
                    CurrentCell.Selected = true;
                    /*
                    if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                    {
                        this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                        this.CurrentCell.Selected = true;
                    }
                     */
                    return;
                }
                CurrentCell.Selected = true;

            }
            else if (e.KeyData == (Keys.Shift | Keys.ShiftKey) || e.KeyData == Keys.ShiftKey || e.KeyData == Keys.LShiftKey)
            {

            }

            else if (e.KeyData == (Keys.LButton | Keys.ShiftKey | Keys.Control))
            {

            }
            else if (e.KeyData == (Keys.LButton | Keys.Shift | Keys.Back)) //Shift+Tab
            {
                CurrentCell.Selected = false;
                if (GoLeft(true) == false)
                {
                    CurrentCell = Cell(Now);
                    CurrentCell.Selected = true;
                    /*
                    if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                    {
                        this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                        this.CurrentCell.Selected = true;
                    }
                     */
                    return;
                }
                CurrentCell.Selected = true;
            }
            #endregion


            base.OnKeyDown(e);

            /*

            //if (this.CurrentCell != null)
            if (Now.Row >= 0 && Now.Col >= 0)
            {
                this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                //if(Now.Row>=0 && Now.Col>=0) this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);

                this.CurrentCell.Selected = true;

                if (e.KeyData == Keys.Tab && e.Shift==false) //next col then next row
                {
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    if (GoRight(true) == false)
                    {
                        if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                        {
                            this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                            this.CurrentCell.Selected = true;
                        }
                        return;
                    }
                    
                }
                if (e.KeyData == Keys.Right) //next col then next row
                {
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    if (GoRight() == false)
                    {
                        if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                        {
                            this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                            this.CurrentCell.Selected = true;
                        }
                        return;
                    }
                }
                else if (e.KeyData == Keys.Left) //go left
                {
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    if (GoLeft() == false)
                    {
                        if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                        {
                            this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                            this.CurrentCell.Selected = true;
                        }
                        return;
                    }
                }
                else if (e.KeyData == Keys.Tab && e.Shift==true) //go left in textfield
                {
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    if (GoLeft(true) == false)
                    {
                        if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                        {
                            this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                            this.CurrentCell.Selected = true;
                        }
                        return;
                    }
                }
                else if (e.KeyData == Keys.Down) //next row
                {
                    //Now.Row++;
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    //GoDown();
                }
                else if (e.KeyData == Keys.Up) //next row
                {
                    //Now.Row++;
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    //GoUp();
                }
                else if (e.KeyData == Keys.Enter) //next row
                {
                    //Now.Row++;
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    if (ActionOnEnterInEditMode == EnterActions.EditNextRow)
                    {
                        GoDown();
                    }
                    else if (ActionOnEnterInEditMode == EnterActions.EditOnThePosition)
                    {
                        //go nowhere
                        this.EndEdit();
                        return;
                    }
                    else //go next col
                    {
                        if (GoRight() == false)
                        {
                            if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                            {
                                this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                                this.CurrentCell.Selected = true;
                            }
                            return;
                        }
                    }

                }

                else if (e.KeyData == Keys.Space)
                {
                    _posAfterEdit = Direction.NoPositioning;
                }
                else if (e.KeyData == Keys.Delete)
                {
                    _posAfterEdit = Direction.NoPositioning;
                    if (this.CurrentCell != null)
                    {
                        _isEditing = true;
                        if (this.EditingControl == null)
                        {
                            BeforeText = this.CurrentCell.Value as String;
                        }
                        this.CurrentCell.Value = "";//.GetData("{0:S}");
                        this.BeginEdit(false);
                        this.CurrentCell.Value = BeforeText;
                    }
                }
                else if (e.KeyData == (Keys.Shift|Keys.ShiftKey) || e.KeyData == Keys.ShiftKey || e.KeyData == Keys.LShiftKey)
                {
                    _posAfterEdit = Direction.NoPositioning;
                }
                else if (e.KeyData == (Keys.V|Keys.Control) && e.Control)
                {
                    _posAfterEdit = Direction.NoPositioning;
                    if (this.CurrentCell != null)
                    {
                        _isEditing = true;
                        if (this.EditingControl == null)
                        {
                            BeforeText = this.CurrentCell.Value as String;
                        }
                        this.CurrentCell.Value = Clipboard.GetText();//.GetData("{0:S}");
                        this.BeginEdit(false);
                        this.CurrentCell.Value = BeforeText;
                    }
                }
                else if (e.KeyData ==(Keys.C|Keys.Control) && e.Control)
                {
                    _posAfterEdit = Direction.NoPositioning;
                    if (this.CurrentCell != null)
                    {
                        Clipboard.SetText(this.CurrentCell.Value as String);
                    }
                }
                else if (e.KeyData == (Keys.LButton|Keys.ShiftKey|Keys.Control))
                {
                    _posAfterEdit = Direction.NoPositioning;
                }
                else if (e.KeyData == (Keys.LButton | Keys.Shift | Keys.Back)) //Shift+Tab
                {
                    _isEditing = false;
                    _posAfterEdit = Direction.NoPositioning;
                    if (GoLeft(true) == false)
                    {
                        if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                        {
                            this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                            this.CurrentCell.Selected = true;
                        }
                        return;
                    }
                }
                
                else //다른 키일때
                {
                    _posAfterEdit = Direction.NoPositioning;
                    if (this.CurrentCell != null && (this.EditingControl == null))
                    {

                        //V_Data..CurrentCell
                        if (Now.Col >= 0)
                        {
                            if (_editables[Now.Col])
                            {
                                _isEditing = true;
                                DataGridViewTextBoxCell cell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
                                if (cell == null)
                                {
                                    _isEditing = false;
                                    base.OnKeyDown(e);
                                    return;
                                }
                                if (this.EditingControl == null)
                                {
                                    //this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
                                    BeforeText = this.CurrentCell.Value as String;
                                }

                                if ((int)e.KeyData >= (int)Keys.A && (int)e.KeyData <= (int)Keys.Z)
                                {

                                    if (Win32APIs.GetKeyState(Keys.CapsLock) == false)
                                    {
                                        this.CurrentCell.Value = ((char)('a' + ((int)e.KeyData - (int)'A'))).ToString();
                                    }
                                    else
                                    {
                                        this.CurrentCell.Value = e.KeyCode.ToString();
                                    }
                                }
                                else if ((int)e.KeyData >= (int)Keys.D0 && (int)e.KeyData <= (int)Keys.D9)
                                {
                                    this.CurrentCell.Value = ((int)e.KeyData - (int)Keys.D0).ToString();

                                }

                                // V_Data.CurrentCell.Selected = false;
                                this.BeginEdit(false);
                                _isEditActivated = true;
                                if (this.EditingControl != null)
                                {
                                    this.EditingControl.PreviewKeyDown += _editingControlEvent;
                                    this.CurrentCell.Value = BeforeText;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                _isEditing = false;
            }
            
            base.OnKeyDown(e);
             */
        }

        void EditingControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (Control.ModifierKeys == Keys.None)
            {
                if (e.KeyData == Keys.Up)
                {
                    this.EditingControl.PreviewKeyDown -= _editingControlEvent;//1회성.
                    GoUpInEdit();
                    return;
                }
                else if (e.KeyData == Keys.Down)
                {
                    this.EditingControl.PreviewKeyDown -= _editingControlEvent;//1회성.
                    GoDownInEdit();
                    return;
                }
                else if (e.KeyData == Keys.Right)
                {

                }
                else if (e.KeyData == Keys.Left)
                {

                }
                else if (e.KeyData == Keys.Enter)
                {
                    if (_actionOnEnterOnEditMode == EnterActions.EditOnThePosition)
                    {
                        Before.Set(Now);
                        _goBack = true;
                    }
                    else if (_actionOnEnterOnEditMode == EnterActions.EditNextRow)
                    {

                    }
                    else //_actionOnEnterOnEditMode == EnterActions.EditNextColumn
                    {

                    }
                }
                else if (e.KeyData == Keys.Escape)
                {

                }
                else if (e.KeyData == Keys.Tab)
                {

                }
                else
                {

                    return;
                }
            }
            try
            {
                this.EditingControl.PreviewKeyDown -= _editingControlEvent;//1회성.
            }
            catch { }
            //this.EndEdit();
        }


        String BeforeText = "";



        bool GoRight(bool isEditableOnly = false)
        {
            if (_editables.Count <= (Now.Col + 1)) //오른쪽으로 이동할 칸이 없을 때 
            { //go next row
                if ((Now.Row + 1) < Rows.Count) //마지막 라인이 아니라면
                {
                    Now.Row++;
                    //BeforeNow.Col++;
                    bool isMoved = false;
                    if (isEditableOnly)
                    {
                        for (int i = 0; i < _editables.Count; i++)
                        {
                            if (_editables[i]==true)
                            {

                                Now.Col = i;

                                isMoved = true;

                                break;
                            }
                        }

                        if (isMoved == false) //더이상 편집할 라인이 없을 때..
                        {
                            InvokeLostFocus(this, null);

                            //Now.Col = -1;
                            Now.Row = Rows.Count - 1;
                        }
                        else
                        {
                            /*
                            if (Now.Col >= 0 && Now.Row >= 0 && Now.Row<this.Rows.Count && Now.Col<this.Columns.Count)
                            {
                                this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
                                this.CurrentCell.Selected = true;
                            }
                             */
                            return false;
                        }
                    }
                    else
                    {
                        Now.Col = 0;
                        return false;
                    }

                }
                else //마지막라인이면 끝냄..
                {
                    //InvokeLostFocus(this, null);
                    //Now.Col = -1;
                    //Now.Row = -1;
                }
            }
            else //go next col
            {
                if (isEditableOnly)
                {
                    for (int i = (Now.Col + 1); i < _editables.Count; i++)
                    {
                        if (_editables[i]==true)
                        {
                            //V_Data.CurrentCell = (Rows[Now.Row].Cells[i] as DataGridViewTextBoxCell);
                            Cell(Now).Selected = true;
                            Now.Col = i;
                            break;
                        }
                    }
                }
                else
                {

                    Now.Col++;
                }
            }
            return true;
        }

        bool GoLeft(bool isEditableOnly = false)
        {

            bool isSelected = false;
            if (isEditableOnly)
            {
                for (int i = (Now.Col - 1); i >= 0; i--)
                {
                    if (_editables[i] == true)
                    {
                        //V_Data.CurrentCell = (Rows[Now.Row].Cells[i] as DataGridViewTextBoxCell);
                        this.CurrentCell.Selected = true;
                        Now.Col = i;
                        isSelected = true;
                        break;
                    }
                }

                if (isSelected == false)
                {
                    Now.Row--;
                    if ((Now.Row - 1) < 0)
                    {
                        Now.Row = 0;
                        return false;
                    }
                    //BeforeNow.Col++;
                    bool isMoved = false;
                    for (int i = _editables.Count - 1; i > 0; i--)
                    {
                        if (_editables[i] == true)
                        {
                            Now.Col = i;
                            isMoved = true;
                            break;
                        }
                    }

                    if (isMoved == false) //더이상 편집할 라인이 없을 때..
                    {
                        //InvokeLostFocus(this, null);
                        //Now.Col = -1;
                        Now.Row = 0;// -1;
                        return false;
                    }
                    else
                    {
                        /*
                        if (Now.Col >= 0 && Now.Row >= 0 && Now.Row < this.Rows.Count && Now.Col < this.Columns.Count)
                        {
                            this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewTextBoxCell);
                            this.CurrentCell.Selected = true;
                        }
                            */
                        return false;
                    }

                }
                return true;
            }
            else
            {
                if (Now.Col > 0)
                {
                    isSelected = true;
                    Now.Col--;
                    return true;
                }
                else
                {
                    if (Now.Row == 0)
                    {
                        Now.Col = 0;
                        return true;
                    }
                    else
                    {
                        Now.Row--;
                        Now.Col = this.Columns.Count - 1;
                        return false;
                    }
                }
            }


        }

        void GoDownInEdit()
        {
            CurrentCell.Selected = false;
            Before.Set(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
            Now.Set(Before);
            if (Now.Row < (Rows.Count - 1))
            {
                Now.Row++;
                this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);
                this.CurrentCell.Selected = true;

            }
            else
            {
                //Now.Row = Rows.Count-1;
            }

            this.CurrentCell.Selected = true;
        }

        void GoUpInEdit()
        {
            CurrentCell.Selected = false;
            Before.Set(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
            Now.Set(Before);

            if (Now.Row > 0)
            {
                Now.Row--;
                this.CurrentCell = (Rows[Now.Row].Cells[Now.Col] as DataGridViewCell);

                this.CurrentCell.Selected = true;
            }
            else
            {
                //Now.Row = -1;
            }

            this.CurrentCell.Selected = true;

        }


    }

}
