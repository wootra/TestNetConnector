using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ReportTools
{
    using Properties;
    public class RdlcCreator
    {
        String ReportBase = Properties.Resources.Report2008Base;
        String FieldBase = Properties.Resources.Field;
        String FieldCellBase = Properties.Resources.FieldCell;
        String HeaderCellBase = Properties.Resources.HeaderCell;
        String TableColumn = Properties.Resources.TableColumn;
        String TableRow = Properties.Resources.TableRow;
        String ValueCell = Properties.Resources.ValueCell;
        String RightBox = Properties.Resources.RightBox;
        String TopBox = Properties.Resources.TopBox;

        String _dataSourceName="";
        String _dataSetName;
        String _commandText="";
        String _tableName = "";
        String _tableAdapterName = "";

        String _color1 = "#ffeeee";
        String _color2 = "#eeffee";
        String _fontColor1 = "#000000";
        String _fontColor2 = "#000000";

        float _reportWidth = 21;
        float _pageWidth = 21;
        List<String> _fieldName = new List<string>();
        List<String> _fieldType = new List<string>();
        List<String> _fieldText = new List<string>();
        List<float> _fieldWidth = new List<float>();
        List<AnchorStyles> _fieldAlign = new List<AnchorStyles>();

        List<List<Object>> _valueList = new List<List<object>>();

        IDictionary<String, int> _titleItems = new Dictionary<string,int>();
        IDictionary<String, String> _infoItems = new Dictionary<string, String>();
        /// <summary>
        /// DB나 데이터에 대한 메타데이터를 지정한다.
        /// </summary>
        /// <param name="DataSourceName">데이터소스의 이름. 쿼리를 지정하면 나온다.예: db_conn_str</param>
        /// <param name="DataSetName">데이터셋의 이름. 예:DataSet1_tbl_models</param>
        /// <param name="CommandText"> 초기에 가져올 값에 대한 쿼리문. 없어도 된다.</param>
        /// <param name="TableName">DB나 DataSet 상에서 테이블의 이름.</param>
        /// <param name="TableAdapterName">DB를 사용했을 때 테이블어뎁터를 사용했을 때 적어줌. 직접 쿼리한다면 없어도 됨. </param>
        public void InitRdlc(
            String DataSourceName="DummyDataSource",
            String DataSetName="DataSet1",
            String CommandText="",
            String TableName="Table1",
            String TableAdapterName=""
            )
        {
            _dataSourceName = DataSourceName;
            _dataSetName = DataSetName;
            _commandText = CommandText;
            _tableName = TableName;
            _tableAdapterName = TableAdapterName;
        }

        float _widMargin = 1.0f;
        /// <summary>
        /// 레포트의 전체 크기를 지정한다. 단위는 cm
        /// </summary>
        /// <param name="reportWidth">[:TotalWidth:]를 지정. 레포트표의 너비이다. cm단위, 기본21</param>
        /// <param name="pageWidth">[:PageWidth:]를 지정. cm단위. 용지의 너비.</param>
        public void setSize(float reportWidth = 21, float pageWidth = 21, float widMargin=1.0f)
        {
            _widMargin = widMargin;
            _reportWidth = reportWidth - _widMargin-2.0f;
            _pageWidth = pageWidth;
        }

        public void AddTitles(String title, int fontSize)
        {
            _titleItems[title] = fontSize;
        }

        public void AddInfos(String infoName, String info)
        {
            _infoItems[infoName] = info;
        }

        public void SetTitles(IDictionary<String, int> titles)
        {
            _titleItems = titles;
        }

        public void SetInfos(IDictionary<String,String> infos)
        {
            _infoItems = infos;
        }

        /// <summary>
        /// 레포트에 보여질 필드를 추가한다.
        /// </summary>
        /// <param name="name">DB상에서 필드의 이름</param>
        /// <param name="type">테이블에서 필드의 타입.</param>
        /// <param name="text">필드의 헤더에 들어갈 텍스트</param>
        /// <param name="cm">[:ColumnWidth:]지정. column의 너비. cm단위. -1로 하면 자동계산 </param>
        public void addFields(String name, Type type, String text, double colummWidth=-1, AnchorStyles align= AnchorStyles.Left)
        {
            _fieldName.Add(name);
            if (type == null) _fieldType.Add((typeof(String)).ToString());
            else _fieldType.Add(type.ToString());
            _fieldText.Add(text);
            _fieldWidth.Add((float)colummWidth);
            _fieldAlign.Add(align);
        }

        public void addValues(Dictionary<String,Object> values){
            List<Object> val = new List<object>();
            for (int i = 0; i < _fieldName.Count; i++)
            {
                if (values.ContainsKey(_fieldName[i])) val.Add(values[_fieldName[i]]);
                else val.Add(null);
            }
            _valueList.Add(val);
        }



        public void addValues(List<Object> values){
            _valueList.Add(values);
        }

        public void addValues(Object[] values)
        {
            List<Object> val = new List<object>();
            val.AddRange(values);
            _valueList.Add(val);
        }

        /// <summary>
        /// Value가 나타나는 Row의 색을 지정한다.
        /// </summary>
        /// <param name="color1">형식은 #ff1100과 같이 하면 된다.</param>
        /// <param name="color2">형식은 #ff1100과 같이 하면 된다.</param>
        public void setValueRowColor(String color1, String color2)
        {
            this._color1 = color1;
            this._color2 = color2;
        }

        public void addValueRowFontColor(String color1, String color2)
        {
            this._fontColor1 = color1;
            this._fontColor2 = color2;
        }

        /// <summary>
        /// 레포트의 기본 정보를 지정합니다.
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="ReporterName"></param>
        /// <param name="Title"></param>
        public void SetCommonField(
            String StartDate="00-00-00",
            String EndDate="00-00-00",
            String ReporterName="Reporter",
            String Title = "Test Report",
            String SubTitle = ""
            )
        {
            _titleItems.Clear();
            _infoItems.Clear();
            AddTitles(Title,21);
            AddTitles(SubTitle,18);
            AddInfos("이름" , ReporterName);
            AddInfos("시작시간", StartDate);
            AddInfos("종료시간", EndDate);

        }

        public float TotalWid
        {
            get { return _totalWid; }
        }

        float _totalWid = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">파일이 저장될 위치와 .rdlc를 포함하는 이름이다.</param>
        public void MakeRdlc(String file)
        {
            int numOfAutoWid = 0;
            _totalWid = 0;
            float reportWidth = _reportWidth;
            
            for (int i = 0; i < _fieldWidth.Count; i++)
            {


                if (_fieldWidth[i] < 0) numOfAutoWid++;
                else _totalWid += _fieldWidth[i];
            }




            if (_totalWid > reportWidth)
            {
                 //_reportWidth = _totalWid + (numOfAutoWid * 0.5f);
            }
            //만일 표의 너비가 일부 셀을 더한 것보다 작다면 표의 너비를 확장한다.
            else if (_totalWid < reportWidth && (numOfAutoWid == 0))
            {
                _totalWid -= _fieldWidth[_fieldWidth.Count - 1];
                _fieldWidth[_fieldWidth.Count - 1] = -1;
                numOfAutoWid = 1;
                //표의 너비가 모든 셀의 합보다 크다면 마지막 란을 표의 나머지를 채운다..
            }

            else if (_totalWid > reportWidth)
            {
                //_reportWidth = _pageWidth;
                _totalWid -= _fieldWidth[_fieldWidth.Count - 1];
                float temp = _fieldWidth[_fieldWidth.Count - 1];
                _fieldWidth[_fieldWidth.Count - 1] = -1;
                numOfAutoWid += 1;
                if (_totalWid > reportWidth)//이 경우는 마지막 란을 없앤 경우에도 레포트가 큰 경우이다.
                {
                    _fieldWidth[_fieldWidth.Count - 1] = temp;
                    _totalWid += temp;
                    numOfAutoWid = 0;
                   // _reportWidth = _totalWid;
                    //다시 복구하고 그냥 되는대로 둔다.
                }
            }

            
            if (numOfAutoWid > 0) //정해지지 않은 너비를 맞춘다.
            {
                float autoWid = (reportWidth - _totalWid) / numOfAutoWid;
                for (int i = 0; i < _fieldWidth.Count; i++)
                {
                    if (_fieldWidth[i] < 0)
                    {
                        _fieldWidth[i] = autoWid;
                        _totalWid += autoWid;
                    }
                }
            }

            
            float rate = _totalWid / reportWidth; //몇 배만큼 큰지..
            for (int i = 0; i < _fieldWidth.Count; i++)
            {
                _fieldWidth[i] = _fieldWidth[i] / rate;//그만큼의 사이즈로 크기를 줄인다.
            }
            _totalWid = reportWidth;

            #region Parts
            String HeaderCells = "";
            String FieldCells = "";
            String Fields = "";
            String TableColumns = "";
            String TableRows = "";
            String Titles = "";
            String Infos = "";
            //int titleItems = 0;
            float itemY = 0;
            for (int i = 0; i < _titleItems.Keys.Count; i++)
            {
                Titles += TopBox.Replace("[:Name:]", "Title"+i)
                                .Replace("[:Value:]", _titleItems.Keys.ElementAt(i))
                                .Replace("[:TotalWidth:]", reportWidth.ToString())
                                .Replace("[:Font:]", _titleItems.Values.ElementAt(i).ToString())
                                .Replace("[:TopCm:]", (i).ToString());
                itemY += 1;
                //titleItems++;
            }
            for (int i = 0; i < _infoItems.Count; i++)
            {
                Infos += RightBox.Replace("[:Name:]", "Info"+i)
                                .Replace("[:Value:]", _infoItems.Keys.ElementAt(i) + ":" + _infoItems.Values.ElementAt(i))
                                .Replace("[:TotalWidth:]", reportWidth.ToString())
                                .Replace("[:TopCm:]", itemY.ToString());
                itemY += 0.5f;
                //titleItems++;
            }

            for (int i = 0; i < _fieldName.Count; i++)
            {
                HeaderCells += HeaderCellBase.Replace("[:FieldText:]", _fieldText[i]);
                FieldCells += FieldCellBase
                    .Replace("[:FieldName:]", _fieldName[i]);
                Fields += FieldBase.Replace("[:TypeName:]", _fieldType[i])
                    .Replace("[:FieldName:]", _fieldName[i]);
                TableColumns += TableColumn.Replace("[:ColumnWidth:]", _fieldWidth[i].ToString());
                    
            }
            #endregion

            #region values
            if (_valueList.Count > 0)
            {
                int rowNo=0;

                foreach (List<Object> list in _valueList)
                {
                    String cells = "";
                    int cellNo=0;
                    for(int i=0; i<_fieldName.Count; i++)
                    //foreach (Object aValue in list)
                    {
                        
                        Object val;
                        try
                        {
                            val = (list[i] != null) ? list[i] : "null";
                        }
                        catch {
                            val = "null";
                        }
                        
                        cells += ValueCell.Replace("[:BoxName:]", "" + rowNo + "_" + cellNo)
                            .Replace("[:BackColor:]", (rowNo % 2 == 0) ? _color1 : _color2)
                            .Replace("[:FontColor:]", (rowNo % 2 == 0) ? _fontColor1 : _fontColor2)
                            .Replace("[:Value:]", val.ToString())
                            .Replace("[:TextAlign:]", _fieldAlign[i].ToString());
                        cellNo++;
                    }
                    TableRows += TableRow.Replace("[/ValueCells/]", cells);
                    rowNo++;
                }
            }

            #endregion


            #region ReportBase
            String Report = ReportBase;

            Report = Report.Replace("[:DataSourceName:]", _dataSourceName)
                    .Replace("[:PageWidth:]", (_pageWidth).ToString())
                    .Replace("[:TableLeft:]", "0")
                    .Replace("[:PageHeight:]", ((_pageWidth)*(29.7/21.0)).ToString())
                    .Replace("[:DataSetName:]", _dataSetName)
                    .Replace("[:CommandText:]", _commandText)
                    .Replace("[:TableName:]", _tableName)
                    .Replace("[:TableAdapterName:]", _tableAdapterName)
                    .Replace("[:TotalWidth:]", _reportWidth.ToString())
                    .Replace("[:TitleItemCount:]", (itemY+0.5f).ToString())// titleItems.ToString())
                    .Replace("[/Fields/]", Fields)
                    .Replace("[/TitleItems/]", Titles+"\r\n"+Infos)
                    .Replace("[/FieldCells/]", FieldCells)
                    .Replace("[/HeaderCells/]", HeaderCells)
                    .Replace("[/TableColumns/]", TableColumns)
                    .Replace("[/TableRows/]", TableRows);

            #endregion

            File.WriteAllText(file, Report);
        }

    }
}
