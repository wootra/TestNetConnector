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
using System.Windows.Controls.Primitives;

namespace WpfHandlers
{
    public static class MenuItemHandler
    {
        /// <summary>
        /// 쉬운 폼으로 Menu를 추가한다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="func"></param>
        /// <param name="isChecked"></param>
        public static void AddContextMenu(this ContextMenu parent, String text, Action<MenuItem, String, object> func, object objToInsert=null, bool? isChecked = null)
        {
            MenuItem item = new MenuItem();
            item.Header = text;
            item.Click += new RoutedEventHandler(new Action<object, RoutedEventArgs>((sender, e) =>
            {
                MenuItem menuItem = sender as MenuItem;

                String txt = (menuItem).Header as String;
                func(menuItem, txt, objToInsert);
            }));
            
            if (isChecked != null)
            {
                item.IsCheckable = true;
                item.IsChecked = (isChecked == true);
            }

            parent.Items.Add(item);
            
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
                item.Header = text;
                item.Tag =  (objToInsert==null)? null : objToInsert.ElementAt(index);
                item.Click += new RoutedEventHandler(new Action<object, RoutedEventArgs>((sender, e) =>
                {
                    MenuItem menuItem = sender as MenuItem;
                    
                    object obj = menuItem.Tag;
                    String txt = (menuItem).Header as String;
                    func(menuItem, txt, obj);
                }));
                
                if (isChecked != null)
                {
                    if (isChecked.ElementAt(index) != null)
                    {
                        
                        item.IsCheckable = true;
                        item.IsChecked = (isChecked.ElementAt(index) == true);
                    }
                }

                parent.Items.Add(item);
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
        public static void Show(this ContextMenu parent, ICollection<String> textList, Action<MenuItem, String, object> func, ICollection<object> objToInsert, ICollection<bool?> isChecked = null)
        {
            SetMenus(parent, textList, func, objToInsert, isChecked);
            parent.Show();
        }

        /// <summary>
        /// 쉬운 폼으로 Menu를 추가한 뒤 바로 보여준다. 추가하기 전에 모든 Item을 지운다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="func"></param>
        /// <param name="isChecked">null이면 check하지 않는다. null이 아니면 그 index에 해당하는 check를 한다.</param>
        public static void Show(this ContextMenu parent, string text, Action<MenuItem, String, object> func, object objToInsert=null, bool? isChecked = null)
        {
            ClearContextMenus(parent);
            AddContextMenu(parent, text, func, objToInsert, isChecked);
            parent.Show();
        }

        /// <summary>
        /// ContextMenu를 바로 보여준다.
        /// </summary>
        /// <param name="menu"></param>
        public static void Show(this ContextMenu menu)
        {
            //_contextMenuSender = items;

            menu.Visibility = System.Windows.Visibility.Visible;
            //Point pt = System.Windows.Input.Mouse.GetPosition(this);
            //_rightMenu.HorizontalOffset = pt.X;
            //_rightMenu.VerticalOffset = pt.Y;
            menu.Placement = PlacementMode.MousePoint;
            menu.IsOpen = true;


        }

        public static void Hide(this ContextMenu menu, bool clearAllMenus=false)
        {
            menu.IsOpen = false;
            if (clearAllMenus)
            {
                menu.ClearContextMenus();
            }
        }


        /// <summary>
        /// 등록되었던 모든 Item들을 삭제한다.
        /// </summary>
        /// <param name="parent"></param>
        public static void ClearContextMenus(this ContextMenu parent){
            parent.Items.Clear();
        }

    }
}
