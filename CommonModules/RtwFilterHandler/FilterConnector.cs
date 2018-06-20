using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RtwFilterHandler
{
    public class FilterConnector
    {
        Dictionary<String, FilterConnectorItem> _filters = new Dictionary<string, FilterConnectorItem>();
        String _tableName;
        public FilterConnector(String tableName)
        {
            _tableName = tableName;
        }
        public void AddFilter(String filterName, bool enabled, RtwFilter filter)
        {
            _filters.Add(filterName, new FilterConnectorItem(filter, enabled, filterName));
        }
        public void SetFilter(String filterName, Object value)
        {
            _filters[filterName].Filter.getQueryCondition(value);
        }
        public RtwFilter this[String filterName]{
            get{ return _filters[filterName].Filter;}
        }
        public RtwFilter this[int index]
        {
            get
            {
                return _filters.Values.ElementAt(index).Filter;
            }
        }
        public List<Object> getIndexListFromStringList(List<String> strList, String partString)
        {
            List<Object> list = new List<object>();
            for (int i=0; i<strList.Count; i++)
            {
                 String str =strList[i];
                 if (str.ToLower().Contains(partString.ToLower())) list.Add(i);
            }
            return list;
        }
        
        public List<Object> getIndexListFromStringList(Dictionary<String,Object> strDic, String partString)
        {
            List<Object> list = new List<object>();
            for (int i = 0; i < strDic.Count; i++)
            {
                String str = strDic.Keys.ElementAt(i);
                if (str.ToLower().Contains(partString.ToLower())) list.Add(strDic.Values.ElementAt(i));
            }
            return list;
        }

        public List<Object> getIndexListFromStringList(Dictionary<Object, int> strDic, String partString)
        {
            List<Object> list = new List<object>();
            for (int i = 0; i < strDic.Count; i++)
            {
                String str = strDic.Keys.ElementAt(i).ToString();
                if (str.ToLower().Contains(partString.ToLower())) list.Add(strDic.Values.ElementAt(i));
            }
            return list;
        }

        public String GetQueryConditions()
        {
            String cond = "";
            int added = 0;
            
            foreach (FilterConnectorItem filter in _filters.Values)
            {
                if (filter.Enabled)
                {
                    String filtersCond = filter.Filter.U_QueryCondition;
                    if (filtersCond.Length > 0)
                    {
                        if (added != 0) cond += " AND ";
                        cond += filtersCond;
                        added++;
                    }
                }
            }
            return cond;
        }
        public String GetSelectQuery(List<String> fieldNames = null)
        {
            String query = "SELECT ";
            if (fieldNames == null) query += "* ";
            else
            {
                String fields = "";
                for (int i = 0; i < fieldNames.Count; i++)
                {
                    if (i != 0) fields += ",";
                    fields += fieldNames[i];
                }
            }
            query += " from " + _tableName + " ";
            String whereCond = GetQueryConditions();
            query += (whereCond.Length > 0) ? " WHERE " + whereCond : "";

            return query;
        }


    }

    public class FilterConnectorItem
    {
        public RtwFilter Filter;
        public Boolean Enabled;
        public String Name;
        public FilterConnectorItem(RtwFilter filter, Boolean enabled, String name)
        {
            Name = name;
            Enabled = enabled;
            Filter = filter;
        }
    }
}
