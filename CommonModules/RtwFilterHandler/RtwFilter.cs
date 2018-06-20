using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RtwFilterHandler
{
    
    public class RtwFilter
    {
        Dictionary<String, HandlingTypeForFilter> _fieldType = new Dictionary<string, HandlingTypeForFilter>();
        ConditionAdder _conditionAdder = ConditionAdder.OR;
        String _queryCondition = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="handlingTypeForFilter"></param>
        public RtwFilter(String fieldName, HandlingTypeForFilter handlingTypeForFilter = HandlingTypeForFilter.String, ConditionAdder condAdd= ConditionAdder.OR)
        {
            if (fieldName.Length > 0)
            {
                if (handlingTypeForFilter == HandlingTypeForFilter.String) _fieldType[fieldName] = HandlingTypeForFilter.String;
                else if (handlingTypeForFilter == HandlingTypeForFilter.Number) _fieldType[fieldName] = HandlingTypeForFilter.Number;
            }
            else
            {
                throw new Exception("fieldName은 비어있을 수 없습니다. DB의 Field를 적으세요");
            }
            _conditionAdder = condAdd;
        }
        public RtwFilter(Dictionary<String, HandlingTypeForFilter> fieldNameAndTypePairs, ConditionAdder condAdd = ConditionAdder.OR)
        {
            if (fieldNameAndTypePairs.Count > 1)
            {
                _fieldType = fieldNameAndTypePairs;
            }
            _conditionAdder = condAdd;
        }
        
        public ConditionAdder U_ConditionAdder { get { return _conditionAdder; } set { _conditionAdder = value; } }
        public String U_QueryCondition { get { return _queryCondition; } }

        public void setMultiFields(Dictionary<String, HandlingTypeForFilter> fieldNameAndTypePairs)
        {
            _fieldType = fieldNameAndTypePairs;
        }

        public String getQueryCondition(Object value, Boolean saveIt = true, Dictionary<String, Object[]> fieldNameAndValuePair=null)
        {
            String queryCondition="";
            int added = 0;
            String temp="";
            foreach (String fieldName in _fieldType.Keys)
            {

                if (_fieldType[fieldName] == HandlingTypeForFilter.Number)
                {
                    if (fieldNameAndValuePair != null && fieldNameAndValuePair.Keys.Contains(fieldName))
                    {
                        temp = getQueryCondition(false, HandlingTypeForFilter.Number, fieldName, fieldNameAndValuePair[fieldName]);
                        if (temp == null || temp.Length == 0) continue;
                    }
                    else if(value==null || value.ToString().Length==0)
                    {
                        continue;
                    }
                    if (added != 0) queryCondition += " "+_conditionAdder.ToString()+" ";
                    if (fieldNameAndValuePair != null && fieldNameAndValuePair.Keys.Contains(fieldName))
                    {
                        queryCondition += temp;
                    }
                    else
                    {
                        queryCondition += fieldName + "=" + value.ToString();
                    }
                    added++;
                }
                else //string
                {
                    if (value == null || value.ToString().Length == 0) continue;
                    if (added != 0) queryCondition += " " + _conditionAdder.ToString() + " ";
                    queryCondition += fieldName + " LIKE '%%" + value.ToString() + "%%'";// getQueryCondition(false, HandlingTypeForFilter.String, fieldName, fieldNameAndValuePair[fieldName]);//fieldName + " LIKE '%%" + value.ToString()+"%%'";
                    added++;
                }
            }
            queryCondition = (queryCondition.Length>0) ? " ("+ queryCondition +") ":"";
            if (saveIt) _queryCondition = queryCondition;
            return queryCondition;
        }
        public String getQueryCondition(Boolean saveIt,HandlingTypeForFilter type, String fieldName, params Object[] values)
        {
            String queryCondition = "";
            int added = 0;

            foreach (Object value in values)
            {
                if (type == HandlingTypeForFilter.Number)
                {
                    if (added != 0) queryCondition += " " + _conditionAdder.ToString() + " ";
                    queryCondition += fieldName + "=" + value.ToString();
                    added++;
                }
                else //string
                {
                    if (added != 0) queryCondition += " " + _conditionAdder.ToString() + " ";
                    queryCondition += fieldName + " LIKE '%%" + value.ToString() + "%%'";
                    added++;
                }
                /*
                if (_fieldType.Values.ElementAt(0) == HandlingTypeForFilter.Number)
                {
                    if (added != 0) queryCondition += " " + _conditionAdder.ToString() + " ";
                    queryCondition += _fieldType.Keys.ElementAt(0) + "=" + value.ToString();
                    added++;
                }
                else //string
                {
                    if (added != 0) queryCondition += " " + _conditionAdder.ToString() + " ";
                    queryCondition += _fieldType.Keys.ElementAt(0) + " LIKE '%%" + value.ToString() + "%%'";
                    added++;
                }
                 */
            }
            queryCondition = (queryCondition.Length>0) ? " (" + queryCondition + ") ": "";
            if (saveIt) _queryCondition = queryCondition;
            return queryCondition;
        }

        public void ClearCondition()
        {
            _queryCondition = "";
        }
    }

}
