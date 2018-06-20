using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace DataHandling
{
    public class ListHandler<T>
    {
        /// <summary>
        /// list havingList에서 선택된 list를 howManyLines 만큼 위로 이동한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="havingList"></param>
        /// <param name="howManyLines"></param>
        public static void MoveUp(IList<T> list, IList<T> havingList, int howManyLines)
        {
            Comparison<T> com = new Comparison<T>(new Func<T, T, int>((T a, T b) =>
            {
                return havingList.IndexOf(a) - havingList.IndexOf(b);
            }));
            //T[] temp = new T[list.Count];
            //list.CopyTo(temp, 0);
            List<T> tempList = new List<T>(list);// temp.ToList();
            tempList.Sort(com);
            list = tempList;// temp.ToList().Sort(.Sort(com);

            int index;
            int gotoIndex;
            for (int i = 0; i < list.Count; i++)
            {
                index = havingList.IndexOf(list[i]);
                gotoIndex = index - howManyLines;
                if (gotoIndex < i) gotoIndex = i; //가장 위로 가면 더이상 갈 수 없다.i열 이상 갈 수도 없다.
                if (index == gotoIndex) continue; //같은 라인에서 움직일 수 없다.

                havingList.Remove(list[i]);
                havingList.Insert(gotoIndex, list[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.

                //rows[i].Selected = true;
            }
        }

        /// <summary>
        /// list havingList에서 선택된 list를 howManyLines 만큼 위로 이동한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="havingList"></param>
        /// <param name="howManyLines"></param>
        public static void MoveUp(IList<T> list, IList havingList, int howManyLines)
        {
            Comparison<T> com = new Comparison<T>(new Func<T, T, int>((T a, T b) =>
            {
                return havingList.IndexOf(a) - havingList.IndexOf(b);
            }));
            //T[] temp = new T[list.Count];
            //list.CopyTo(temp, 0);
            List<T> tempList = new List<T>(list);// temp.ToList();
            tempList.Sort(com);
            
            //list = tempList;// temp.ToList().Sort(.Sort(com);

            int index;
            int gotoIndex;
            for (int i = 0; i < tempList.Count; i++)
            {
                index = havingList.IndexOf(tempList[i]);
                gotoIndex = index - howManyLines;
                if (gotoIndex < i) gotoIndex = i; //가장 위로 가면 더이상 갈 수 없다.i열 이상 갈 수도 없다.
                if (index == gotoIndex) continue; //같은 라인에서 움직일 수 없다.

                havingList.Remove(tempList[i]);
                havingList.Insert(gotoIndex, tempList[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.

                //rows[i].Selected = true;
            }
        }

        public static void MoveDown(IList<T> list, IList<T> havingList, int howManyLines)
        {
            if (list.Count == 0) return;
            Comparison<T> com = new Comparison<T>(new Func<T, T, int>((T a, T b) =>
            {
                return havingList.IndexOf(a) - havingList.IndexOf(b);
            }));

            //T[] temp = new T[list.Count];
            //list.CopyTo(temp, 0);
            List<T> tempList = new List<T>(list);// temp.ToList();
            tempList.Sort(com);
            //list = tempList;// temp.ToList().Sort(.Sort(com);


            int index;// = havingList.IndexOf(list[0]);
            int gotoIndex;// = index + howManyLines;

            //int rowIndex = -1;
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                index = havingList.IndexOf(tempList[i]);
                gotoIndex = index + howManyLines;
                if (gotoIndex < i) gotoIndex = i; //가장 위로 가면 더이상 갈 수 없다.i열 이상 갈 수도 없다.
                if (index == gotoIndex) continue; //같은 라인에서 움직일 수 없다.

                havingList.Remove(tempList[i]);
                havingList.Insert(gotoIndex, tempList[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.
            }
        }


        public static void MoveDown(IList<T> list, IList havingList, int howManyLines)
        {
            if (list.Count == 0) return;
            Comparison<T> com = new Comparison<T>(new Func<T, T, int>((T a, T b) =>
            {
                return havingList.IndexOf(a) - havingList.IndexOf(b);
            }));

            //T[] temp = new T[list.Count];
            //list.CopyTo(temp, 0);
            List<T> tempList = new List<T>(list);// temp.ToList();

            tempList.Sort(com);
           // list = tempList;// temp.ToList().Sort(.Sort(com);


            int index;// = havingList.IndexOf(list[0]);
            int gotoIndex;// = index + howManyLines;

            //int rowIndex = -1;
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                index = havingList.IndexOf(tempList[i]);
                gotoIndex = index + howManyLines;
                if (gotoIndex < i) gotoIndex = i; //가장 위로 가면 더이상 갈 수 없다.i열 이상 갈 수도 없다.
                if (index == gotoIndex) continue; //같은 라인에서 움직일 수 없다.

                havingList.Remove(tempList[i]);
                havingList.Insert(gotoIndex, tempList[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.
            }
        }


        public static void MoveWithFirstLine(List<T> list, List<T> havingList, int goLine)
        {
            if (list.Count == 0) return;
            Comparison<T> com = new Comparison<T>(new Func<T, T, int>((T a, T b) =>
            {
                return havingList.IndexOf(a) - havingList.IndexOf(b);
            }));

            list.Sort(com);
            int index = havingList.IndexOf(list[0]);
            //if(goLine<=rows[0].Index) index = rows[0].Index;
            //else index = rows[rows.Count-1].Index;

            int gotoIndex = goLine;// (goLine < index) ? goLine : goLine - 1;// (rows[0].Index <= goLine && rows[rows.Count - 1].Index >= goLine) ? rows[rows.Count].Index : goLine;

            if (gotoIndex < 0) gotoIndex = 0; //가장 위로 가면 더이상 갈 수 없다.
            if (gotoIndex > havingList.Count - list.Count)
                gotoIndex = havingList.Count - list.Count; //첫열은 아래열보다 내려갈 수 없다.
            //if (index == gotoIndex) return; //같은 라인에서 움직일 수 없다.
            //V_Data.ClearSelection();
            //V_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //V_Data.MultiSelect = false;
            //V_Data.MultiSelect = true;
            int offset = gotoIndex - index;

            for (int i = 0; i < list.Count; i++)
            {

                havingList.Remove(list[i]); //일단 모두 뺀다.

            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                havingList.Insert(gotoIndex, list[i]);
            }

        }
    }
}
