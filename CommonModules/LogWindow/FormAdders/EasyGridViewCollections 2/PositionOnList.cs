using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormAdders
{
    public class PositionOnList
    {
        int _row = -1;
        int _col = -1;

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }

        public PositionOnList(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public void Set(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public void Set(PositionOnList pos)
        {
            Row = pos.Row;
            Col = pos.Col;
        }
        public static bool operator ==(PositionOnList pos1, PositionOnList pos2)
        {
            if (pos1.Row == pos2.Row && pos1.Col == pos2.Col) return true;
            else return false;
        }
        public static bool operator !=(PositionOnList pos1, PositionOnList pos2)
        {
            if (pos1.Row == pos2.Row && pos1.Col == pos2.Col) return false;
            else return true;
        }
        public bool Equals(PositionOnList pos)
        {
            if (this.Row == pos.Row && this.Col == pos.Col) return true;
            else return false;
        }

        public override bool Equals(Object obj)
        {
            PositionOnList pos = obj as PositionOnList;
            if (this.Row == pos.Row && this.Col == pos.Col) return true;
            else return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
