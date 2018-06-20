using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DockingActions;
using System.Drawing;
using System.Windows.Forms;
using DataHandling;

namespace DockingActions
{
    public class DockingRoot:DockingContainer
    {
        Dictionary<String, DockingContainer> _successors = new Dictionary<String, DockingContainer>();
        IconCenter _iconCenter;
        IconSides[] _iconSide;
        TabControl[] _tabSide = new TabControl[4];
        TabControl left, right, top, bottom;
        SelectionArea _selection;
        
        Form _mdiForm;
        enum Sides{ Bottom=0, Left=1, Right=2, Top=3};
        Control _initObj;
        int _initContentId;
        String _initName = "Root";
        public DockingRoot(Control initObj, int contentId, Form mdiForm, String initName="Root")
            : base(null,null,initName,initObj, contentId, DockStyle.Fill)
        {
            _initObj = initObj;
            _initContentId = contentId;
            _mdiForm = mdiForm;
            _initName = initName;

            _iconCenter = new IconCenter(this);
            _iconSide = new IconSides[4];
            _iconSide[(int)Sides.Bottom] = new IconSides(this, IconSides.ButtonPosition.B_Bottom);
            _iconSide[(int)Sides.Left] = new IconSides(this, IconSides.ButtonPosition.B_Left);
            _iconSide[(int)Sides.Right] = new IconSides(this, IconSides.ButtonPosition.B_Right);
            _iconSide[(int)Sides.Top] = new IconSides(this, IconSides.ButtonPosition.B_Top);

            _tabSide[(int)Sides.Bottom] = bottom = new TabControl();
            _tabSide[(int)Sides.Left] = left = new TabControl();
            _tabSide[(int)Sides.Right] = right = new TabControl();
            _tabSide[(int)Sides.Top] = top = new TabControl();
            
            bottom.Alignment = TabAlignment.Bottom;
            left.Alignment = TabAlignment.Left;
            right.Alignment = TabAlignment.Right;
            top.Alignment = TabAlignment.Top;

            setTabsInit();

            _selection = new SelectionArea();
            //this.Controls.Add(_selection);
            _selection.Hide();
            OnBlankContent = setInitObj;
            showTitleBar(false, _initName);
        }

        internal void setInitObj()
        {
            Content = _initObj;
            _id = _initContentId;
            this.Controls.Add(_initObj);
            showTitleBar(false, _initName);
        }

        internal Form getMdiForm() { return _mdiForm; }

        internal void addToTab(DockingContainer child, DockStyle dock)
        {
            switch (dock)
            {
                case DockStyle.Left:
                    addToTab(Sides.Left, child);
                    break;
                case DockStyle.Right:
                    addToTab(Sides.Right, child);
                    break;

                case DockStyle.Top:
                    addToTab(Sides.Top, child);
                    break;
                case DockStyle.Bottom:
                    addToTab(Sides.Bottom, child);
                    break;
            }
        }

        void addToTab(Sides side, DockingContainer child)
        {
            if (child.getContainerType() == ContainerType.SingleContent)
            {
                addAChildWithSingleContentToTabContainer(_tabSide[(int)side], child);
            }
            else //tabContents
            {
                addAChildWithTabContentToTabContainer(_tabSide[(int)side], child);
            }
        }

        void setTabsInit()
        {
            for (int i = 0; i < _tabSide.Length; i++)
            {
                _iconSide[i].Hide();
                _tabSide[i].Hide();
                   
                this.Controls.Add(_iconSide[i]);
                this.Controls.Add(_tabSide[i]);
            }
            _iconCenter.Hide();
            this.Controls.Add(_iconCenter);
        }

        internal void Connect(String name, DockingContainer aSuccessor)
        {
            _successors.Add(name, aSuccessor);
        }
        internal void Disconnect(String name)
        {
            _successors.Remove(name);
        }



        void showSideIcon()
        {
            int iconWidth = _iconSide[(int)Sides.Top].Width;
            int iconHeight = _iconSide[(int)Sides.Top].Height;

            int margin = 30;
            Point leftTop = new Point(margin, margin);
            Point half = new Point((this.Width-iconWidth)/2,(this.Height-iconHeight)/2);
            Point rightBottom = new Point((this.Width - iconWidth-margin), (this.Height - iconHeight - margin));

            
            _iconSide[(int)Sides.Top].SetBounds(half.X, leftTop.Y, 0, 0, System.Windows.Forms.BoundsSpecified.Location);
            _iconSide[(int)Sides.Bottom].SetBounds(half.X, rightBottom.Y, 0, 0, System.Windows.Forms.BoundsSpecified.Location);
            _iconSide[(int)Sides.Left].SetBounds(leftTop.X, half.Y, 0, 0, System.Windows.Forms.BoundsSpecified.Location);
            _iconSide[(int)Sides.Right].SetBounds(rightBottom.X, half.Y, 0, 0, System.Windows.Forms.BoundsSpecified.Location);

            for (int i = 0; i < _iconSide.Length; i++)
            {
                _iconSide[i].Show();
                _iconSide[i].BringToFront();
            }

        }

        public override void addChild(DockingContainer child, DockStyle dock, bool isRemainSize = true)
        {
            if (_id == _initContentId)
            {
                this.Controls.Remove(_initObj);
                Content = child;
                this.Controls.Add(child);
                child.Dock = DockStyle.Fill;
                //this.Name = child.Name;
                //this._id = child._id;
                //showTitleBar(true);
                this.setContainerType(ContainerType.SingleContent);
                /*
                this.Controls.Remove(_initObj);
                Content = child.Content;
                this.Controls.Add(child.Content);
                this.Name = child.Name;
                this._id = child._id;
                showTitleBar(true);
                setType( = ContainerType.SingleContent;
                 */
            }
            else
            {
                base.addChild(child, dock, isRemainSize);
            }
        }

        protected override void PopChildFromThis(DockingContainer child)
        {
            
            base.PopChildFromThis(child);
           
        }

        internal void DragBegin(DockingContainer movingObj, int x, int y)
        {
            showSideIcon();
            
        }

        internal void Dragging(DockingContainer movingObj, int x, int y)
        {
            
            int maxDepth = 0;
            int depth;
            DockingContainer selectedSuccessor = null;
            this.Invalidate();
            foreach(DockingContainer d in _successors.Values){
            
            //for (int i = 0; i < _successors.Count; i++)
            //{
                
                if (CoodinateHandling.isEntered(d, 0, 0) && d.isInPopup()==false)
                {
                    if ((depth = d.getDepth()) > maxDepth)
                    {
                        maxDepth = depth;
                        selectedSuccessor = d;
                    }
                }
            }
            if (maxDepth > 0)
            {
                Rectangle rect = CoodinateHandling.FromClientToClient(selectedSuccessor, this);
                Point center = CoodinateHandling.getCenter(rect, _iconCenter.Width, _iconCenter.Height);

                ShowSelection(checkIfOverCenterBtns(), selectedSuccessor, movingObj);
                _iconCenter.SetBounds(center.X, center.Y, 0, 0, BoundsSpecified.Location);
                _iconCenter.Show();
                _iconCenter.BringToFront();
                _iconCenter.label1.Text = "" + selectedSuccessor.getDepth() + "/" + selectedSuccessor.Name + "/" + selectedSuccessor.getContainerType().ToString();

            }
            else
            {
                _selection.Hide();
            }
            
        }

        IconCenter.ButtonPosition checkIfOverCenterBtns()
        {
           // Point pt = CoodinateHandling.FromScreenToClient(, , _iconCenter);
            for (int i=0; i<_iconCenter._buttonList.Length; i++)
            {
                if (CoodinateHandling.isEntered(_iconCenter._buttonList[i]))
                {
                    return _iconCenter._buttonPosList[i];
                }
            }
            return IconCenter.ButtonPosition.NotOnTheButton;
        }

        IconCenter.ButtonPosition _posToDock = IconCenter.ButtonPosition.NotOnTheButton;
        DockingContainer _selectedDockingContainer = null;
        void ShowSelection(IconCenter.ButtonPosition pos, DockingContainer dockParent, DockingContainer popup)
        {
            if (pos == IconCenter.ButtonPosition.NotOnTheButton)
            {
                _selection.Hide();
                return;
            }
            _selectedDockingContainer = dockParent;
            _posToDock = pos;
            Size popupSize = popup.ClientSize;
            Rectangle dockScreen = CoodinateHandling.GetScreenRect(dockParent);
            Size parentSize = dockParent.Parent.ClientSize;
            Rectangle selectionRect = new Rectangle(dockScreen.Location, dockScreen.Size);
            switch (pos)
            {
                case IconCenter.ButtonPosition.B_BottomHalf:
                    selectionRect.Y += selectionRect.Height = dockScreen.Height / 2;
                    break;
                case IconCenter.ButtonPosition.B_BottomRemain:
                    if (dockScreen.Height + popupSize.Height > parentSize.Height)
                    {
                        selectionRect.Y += selectionRect.Height = dockScreen.Height / 2;
                    }
                    else
                    {
                        selectionRect.Height = popupSize.Height;
                        selectionRect.Y += dockScreen.Height - popupSize.Height;
                    }
                    break;
                case IconCenter.ButtonPosition.B_Center:
                    //그대로
                    break;
                case IconCenter.ButtonPosition.B_LeftHalf:
                    selectionRect.Width /= 2;
                    break;
                case IconCenter.ButtonPosition.B_LeftRemain:
                    if (dockScreen.Width + popupSize.Width > parentSize.Width) selectionRect.Width /= 2;
                    else selectionRect.Width = popupSize.Width;
                    break;
                case IconCenter.ButtonPosition.B_RightHalf:
                    selectionRect.X += selectionRect.Width = selectionRect.Width / 2;
                    break;
                case IconCenter.ButtonPosition.B_RightRemain:
                    if (dockScreen.Width + popupSize.Width > parentSize.Width) selectionRect.Width /= 2;
                    else selectionRect.Width = popupSize.Width;
                    selectionRect.X += dockScreen.Width - selectionRect.Width;
                    break;
                case IconCenter.ButtonPosition.B_TopHalf:
                    selectionRect.Height /= 2;
                    break;
                case IconCenter.ButtonPosition.B_TopRemain:
                    if (dockScreen.Height + popupSize.Height > parentSize.Height) selectionRect.Height = selectionRect.Height /= 2;
                    else selectionRect.Height = popupSize.Height;
                    break;
                case IconCenter.ButtonPosition.NotOnTheButton:
                    _selection.Hide();
                    break;
            }
            _selection.SetBounds(selectionRect.X, selectionRect.Y, selectionRect.Width, selectionRect.Height);
            _selection.Show();
            _selection.BringToFront();
            this.Refresh();
        }

        internal void DragEnd(DockingContainer movingObj, int x, int y)
        {
            _iconCenter.Hide();
            _selection.Hide();
            foreach (Control c in _iconSide) c.Hide();

            DockItOn(movingObj);

        }
        void DockItOn(DockingContainer popup)
        {
            DockingContainer dockParent = _selectedDockingContainer;
            IconCenter.ButtonPosition pos = _posToDock;

            if (pos == IconCenter.ButtonPosition.NotOnTheButton)
            {
                return;
            }

            switch (pos)
            {
                case IconCenter.ButtonPosition.B_BottomHalf:
                    dockParent.addChild(popup, DockStyle.Bottom);
                    break;
                case IconCenter.ButtonPosition.B_BottomRemain:
                    dockParent.addChild(popup, DockStyle.Bottom, true);
                    break;
                case IconCenter.ButtonPosition.B_Center:
                    dockParent.addChild(popup, DockStyle.Fill);
                    break;
                case IconCenter.ButtonPosition.B_LeftHalf:
                    dockParent.addChild(popup, DockStyle.Left);
                    break;
                case IconCenter.ButtonPosition.B_LeftRemain:
                    dockParent.addChild(popup, DockStyle.Left, true);
                    break;
                case IconCenter.ButtonPosition.B_RightHalf:
                    dockParent.addChild(popup, DockStyle.Right);
                    break;
                case IconCenter.ButtonPosition.B_RightRemain:
                    dockParent.addChild(popup, DockStyle.Right, true);
                    break;
                case IconCenter.ButtonPosition.B_TopHalf:
                    dockParent.addChild(popup, DockStyle.Top);
                    break;
                case IconCenter.ButtonPosition.B_TopRemain:
                    dockParent.addChild(popup, DockStyle.Top, true);
                    break;
            }
            this.Connect(popup.Name, popup);
            popup.setNowInPopup(false);
            popup.hidePopup();

            this.Refresh();
        }

        void checkInHotArea(DockingContainer movingObj, int x, int y){
            
        }

        internal void setHoverButton(IconCenter.ButtonPosition pos)
        {
            
            switch(pos){
                case IconCenter.ButtonPosition.B_BottomHalf:
                    break;
                case IconCenter.ButtonPosition.B_BottomRemain:
                    break;
                case IconCenter.ButtonPosition.B_Center:
                    break;
                case IconCenter.ButtonPosition.B_LeftHalf:
                    break;
                case IconCenter.ButtonPosition.B_LeftRemain:
                    break;
                case IconCenter.ButtonPosition.B_RightHalf:
                    break;
                case IconCenter.ButtonPosition.B_RightRemain:
                    break;
                case IconCenter.ButtonPosition.B_TopHalf:
                    break;
                case IconCenter.ButtonPosition.B_TopRemain:
                    break;
                default:
                    break;
            }
        }
        
        internal void setHoverRootButton(IconSides.ButtonPosition pos)
        {

            switch (pos)
            {
                case IconSides.ButtonPosition.B_Bottom:
                    break;
                case IconSides.ButtonPosition.B_Left:
                    break;
                case IconSides.ButtonPosition.B_Right:
                    break;
                case IconSides.ButtonPosition.B_Top:
                    break;
                default:
                    break;
            }
        }

        internal void setLeaveButton()
        {

        }

    }
}
