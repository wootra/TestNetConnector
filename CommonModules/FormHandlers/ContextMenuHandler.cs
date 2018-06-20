using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;

namespace FormHandlers
{
    public static class ContextMenuHandler
    {

        /// <summary>
        /// 쉬운 폼으로 Menu를 추가한다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="func"></param>
        /// <param name="isChecked"></param>
        public static void AddContextMenu(this ContextMenu parent, String text, Action<MenuItem, String, object> func, object objToInsert = null, bool? isChecked = null)
        {
            MenuItem item = new MenuItem();
            item.Text = text;
            item.Click += new EventHandler(new Action<object, EventArgs>((sender, e) =>
            {
                MenuItem menuItem = sender as MenuItem;

                String txt = (menuItem).Text as String;
                func(menuItem, txt, objToInsert);
            }));

            if (isChecked != null)
            {
                //item.Checked.IsCheckable = true;
                item.Checked = (isChecked == true);
            }

            parent.MenuItems.Add(item);

        }

        /// <summary>
        /// 쉬운 폼으로 Menu를 추가한다. 추가하기 전에 모든 Item을 지운다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="func"></param>
        /// <param name="isChecked">null이면 check하지 않는다. null이 아니면 그 index에 해당하는 check를 한다.</param>
        public static void SetMenus(this ContextMenu parent, ICollection<String> textList, Action<MenuItem, String, object> func, ICollection<object> objToInsert, ICollection<bool?> isChecked = null)
        {
            ClearContextMenus(parent);
            int index = 0;
            if (objToInsert != null && objToInsert.Count < textList.Count)
            {
                throw new Exception("the size of objToInsert is less than the size of textList");
            }
            if (isChecked != null && isChecked.Count < textList.Count)
            {
                throw new Exception("the size of isChecked is less than the size of textList");
            }
            foreach (String text in textList)
            {
                MenuItem item = new MenuItem();
                item.Text = text;
                item.Tag = (objToInsert == null) ? null : objToInsert.ElementAt(index);
                item.Click += new EventHandler(new Action<object, EventArgs>((sender, e) =>
                {
                    MenuItem menuItem = sender as MenuItem;

                    object obj = menuItem.Tag;
                    String txt = (menuItem).Text as String;
                    func(menuItem, txt, obj);
                }));

                if (isChecked != null)
                {
                    if (isChecked.ElementAt(index) != null)
                    {


                        item.Checked = (isChecked.ElementAt(index) == true);
                    }
                }

                parent.MenuItems.Add(item);
                index++;
            }
        }


        /// <summary>
        /// 쉬운 폼으로 Menu를 추가한 뒤 바로 보여준다. 추가하기 전에 모든 Item을 지운다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="func"></param>
        /// <param name="isChecked">null이면 check하지 않는다. null이 아니면 그 index에 해당하는 check를 한다.</param>
        public static void Show(this ContextMenu parent, Control control, Point point, ICollection<String> textList, Action<MenuItem, String, object> func, ICollection<object> objToInsert, ICollection<bool?> isChecked = null)
        {
            SetMenus(parent, textList, func, objToInsert, isChecked);
            parent.Show(control, point);
        }

        /// <summary>
        /// 쉬운 폼으로 Menu를 추가한 뒤 바로 보여준다. 추가하기 전에 모든 Item을 지운다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="func"></param>
        /// <param name="isChecked">null이면 check하지 않는다. null이 아니면 그 index에 해당하는 check를 한다.</param>
        public static void Show(this ContextMenu parent, Control control, Point point, string text, Action<MenuItem, String, object> func, object objToInsert = null, bool? isChecked = null)
        {
            ClearContextMenus(parent);
            AddContextMenu(parent, text, func, objToInsert, isChecked);
            parent.Show(control, point);
        }



        public static void Hide(this ContextMenu menu, bool clearAllMenus = false)
        {

            if (clearAllMenus)
            {
                ClearContextMenus(menu);
            }
            menu.Dispose();
        }


        /// <summary>
        /// 등록되었던 모든 Item들을 삭제한다.
        /// </summary>
        /// <param name="parent"></param>
        public static void ClearContextMenus(this ContextMenu parent)
        {
            parent.MenuItems.Clear();
        }

    }
}
