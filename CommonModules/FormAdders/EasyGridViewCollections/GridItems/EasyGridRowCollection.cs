using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridRowCollection : DataGridViewRowCollection, ICollection<EasyGridRow>
    {
        DataGridView _view;
        public EasyGridRowCollection(DataGridView view):base(view)
        {
            _view = view;
        }

        public new EasyGridRow this[int index]
        {
            get
            {
                if (index < 0) return null;
                if(index<_view.Rows.Count) return _view.Rows[index] as EasyGridRow;
                return null;
            }
        }

        public void Insert(int beforeThisIndex, EasyGridRow row)
        {
            _view.Rows.Insert(beforeThisIndex, row);
        }

        public void InsertRange(int beforeThisIndex, params EasyGridRow[] rows)
        {
            _view.Rows.InsertRange(beforeThisIndex, rows);
        }


        public void Add(EasyGridRow item)
        {
            try
            {
                _view.Rows.Add(item as DataGridViewRow);
            }
            catch(Exception ex) {
                MessageBox.Show("EasyGridRowCollection:Add -" + ex.ToString());
            }
        }

        public void AddRange(params EasyGridRow[] item)
        {
            _view.Rows.AddRange(item);
        }

        public bool Contains(EasyGridRow item)
        {
            return _view.Rows.Contains(item as DataGridViewRow);
        }

        public void CopyTo(EasyGridRow[] array, int arrayIndex)
        {
            DataGridViewRow[] rows = new DataGridViewRow[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                rows[i] = array[i];
            }
            _view.Rows.CopyTo(rows, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Clear()
        {
            _view.Rows.Clear();
        }

        public bool Remove(EasyGridRow item)
        {
            if (_view.Rows.Contains(item))
            {
                _view.Rows.Remove(item as DataGridViewRow);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Count
        {
            get
            {
                return _view.Rows.Count;
            }
        }


        public void RemoveAt(int index)
        {
            _view.Rows.RemoveAt(index);
        }

        List<EasyGridRow> rows = new List<EasyGridRow>();
        public IEnumerator<EasyGridRow> GetEnumerator()
        {
            rows.Clear();

            for (int i = 0; i < _view.Rows.Count; i++)
            {
                rows.Add(_view.Rows[i] as EasyGridRow);
            }
            return rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }



}
