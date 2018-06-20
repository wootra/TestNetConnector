using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ReportTools
{
    using Properties;
    public class RdlcCreator
    {
        
        String ReportBase = Properties.Resources.Report2012;
        String FieldBase = Properties.Resources.Field;
        String FieldCellBase = Properties.Resources.FieldCell;
        String HeaderCellBase = Properties.Resources.HeaderCell;
        String TableColumn = Properties.Resources.TableColumn;
        String TableRow = Properties.Resources.TableRow;
        String ValueCell = Properties.Resources.ValueCell;
        String RightBox = Properties.Resources.RightBox;
        String TopBox = Properties.Resources.TopBox;
        String TablelixMember = Properties.Resources.TablelixMemberBase;
        String ADataSet = Properties.Resources.DataSet;
        String Tablix = Properties.Resources.Tablix;
        String DataSource = Properties.Resources.DataSource;
        String AChart = Properties.Resources.Chart;
        //String _dataSourceName="";
        //String _dataSetName;
        //String _commandText="";
        //String _tableName = "";
        //String _tableAdapterName = "";

        String _color1 = "#ffeeee";
        String _color2 = "#ffffff";
        String _fontColor1 = "#000000";
        String _fontColor2 = "#000000";

        float _reportWidth = 21;
        float _pageWidth = 21;
        float _margin = 0.5f;
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
            
            )
        {
            /*
            String CommandText="",
            String TableName="Table1",
            String TableAdapterName=""
            String DataSourceName="DummyDataSource",
            String DataSetName="DataSet1",
            
            _dataSourceName = DataSourceName;
            _dataSetName = DataSetName;
            _commandText = CommandText;
            _tableName = TableName;
            _tableAdapterName = TableAdapterName;
             */
        }

        
        /// <summary>
        /// 레포트의 전체 크기를 지정한다. 단위는 cm
        /// </summary>
        /// <param name="reportWidth">[:TotalWidth:]를 지정. 레포트표의 너비이다. cm단위, 기본21</param>
        /// <param name="pageWidth">[:PageWidth:]를 지정. cm단위. 용지의 너비.</param>
        public void setSize(float reportWidth = 21, float pageWidth = 21, float widMargin=0.5f)
        {
            _margin = widMargin;
            _reportWidth = (reportWidth >= pageWidth) ? reportWidth - (_margin * 2) : reportWidth;// -2.0f;
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
        /// <param name="colummWidth">컬럼의 너비. 단위는 픽셀</param>
        /// <param name="align">셀 안에서 위치할 가로상의 위치</param>
        public void addFields(String name, Type type, String text, int colummWidth=100, AnchorStyles align= AnchorStyles.Left)
        {
            _fieldName.Add(name);
            if (type == null) _fieldType.Add((typeof(String)).ToString());
            else _fieldType.Add(type.ToString());
            _fieldText.Add(text);
            _fieldWidth.Add((float)colummWidth);
            _fieldAlign.Add(align);
        }

        Dictionary<int, Dictionary<int, String>> _cellColors = new Dictionary<int, Dictionary<int, String>>();
        public void addValues(Dictionary<String,Object> values, Dictionary<int, String> fontColors=null){
            List<Object> val = new List<object>();
            for (int i = 0; i < _fieldName.Count; i++)
            {
                if (values.ContainsKey(_fieldName[i])) val.Add(values[_fieldName[i]]);
                else val.Add(null);
            }
            if(fontColors!=null) _cellColors[_valueList.Count] = fontColors;
            _valueList.Add(val);

        }

        public void addValues(List<Object> values, Dictionary<int, String> fontColors = null)
        {
            if (fontColors != null) _cellColors[_valueList.Count] = fontColors;
            _valueList.Add(values);
        }

        public void addValues(Object[] values, Dictionary<int, String> fontColors = null)
        {
            List<Object> val = new List<object>();
            val.AddRange(values);
            if (fontColors != null) _cellColors[_valueList.Count] = fontColors;
            _valueList.Add(val);
        }


        public void SetCellColor(int rowIndex, int colIndex, String color)
        {
            if (_cellColors.ContainsKey(rowIndex) == false || _cellColors[rowIndex]==null)
            {
                _cellColors[rowIndex] = new Dictionary<int, String>();
                _cellColors[rowIndex][colIndex] = color;
            }
            else
            {
                _cellColors[rowIndex][colIndex] = color;
            }

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

        /*

        public void addValueRowFontColor(String color1, String color2)
        {
            this._fontColor1 = color1;
            this._fontColor2 = color2;
        }
        */

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
            AddInfos("Name" , ReporterName);
            AddInfos("Start", StartDate);
            AddInfos("Finish", EndDate);

        }

        public float TotalWid
        {
            get { return _totalWid; }
        }

        float _totalWid = 0;

        DataSet _dataSet = new DataSet();
        public DataSet DataSet
        {
            get { return _dataSet; }
        }

        DataTable _dataTable = new DataTable();
        public DataTable DataTable
        {
            get { return _dataTable; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">파일이 저장될 위치와 .rdlc를 포함하는 이름이다.</param>
        public void MakeRdlc(String file)
        {
            
            
            float reportWidth = _reportWidth - (_fieldWidth.Count * 0.01f);

            _totalWid = 0;

            for (int i = 0; i < _fieldWidth.Count; i++) //전체의 너비를 가져온다.
            {
                _totalWid += _fieldWidth[i];
            }

            float reportGrowRate = reportWidth / _totalWid;
            _totalWid = reportWidth; //이제 전체 크기는 report크기와 같아졌다.

            for (int i = 0; i < _fieldWidth.Count; i++) //표의 너비를 조정한다.
            {
                _fieldWidth[i] = _fieldWidth[i] * reportGrowRate;
            }

            _dataSet = new System.Data.DataSet("DummyDataSet");
            _dataTable = new System.Data.DataTable("Table");
            
            

            float cellFontSize = 320 * reportGrowRate;
            #region Parts
            String HeaderCells = "";
            String FieldCells = "";
            String Fields = "";
            String TableColumns = "";
            String TableRows = "";
            String Titles = "";
            String Infos = "";
            String ColumnMembers = "";
            String RowMembers = "";
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
                                .Replace("[:TopCm:]", itemY.ToString())
                                .Replace("[:Font:]","9")
                                ;
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
                ColumnMembers += TablelixMember + "\r\n";    
            }
            #endregion
            #region values

            int rowNo = 0;

            if (_valueList.Count > 0)
            {
                
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
                        string fontColor = "Black";
                        if (_cellColors.ContainsKey(rowNo) && _cellColors[rowNo].ContainsKey(cellNo))
                        {
                            fontColor = _cellColors[rowNo][cellNo];
                        }
                        cells += ValueCell.Replace("[:BoxName:]", "" + rowNo + "_" + cellNo)
                            .Replace("[:BackColor:]", (rowNo % 2 == 0) ? _color1 : _color2)
                            .Replace("[:FontColor:]", fontColor)
                            .Replace("[:Value:]", val.ToString())
                            .Replace("[:FontSize:]", cellFontSize.ToString())// titleItems.ToString())
                            .Replace("[:TextAlign:]", _fieldAlign[i].ToString());

                        cellNo++;
                    }
                    TableRows += TableRow   .Replace("[:RowHeight:]",(30*reportGrowRate).ToString())
                                            .Replace("[/ValueCells/]", cells);
                    rowNo++;
                    RowMembers += TablelixMember + "\r\n";
                }
            }
            RowMembers += TablelixMember + "\r\n"; //for header row
            #endregion

            #region makeDataSources
            String DataSources = "";
            {
                string dataSourceName = "DummyDataSource";
                DataSources += DataSource
                                .Replace("[:DataSourceName:]", dataSourceName)
                                ;
            }
            #endregion


            #region makeDataSets
            String DataSets = "";
            
            {
                String tableName = "Tablix1";
                string dataSourceName = "DummyDataSource";
                string dataSetName = "DataSet1";
                string tableAdapterName = "";
                string commandText = "";

                DataSets += ADataSet.Replace("[:DataSetName:]", dataSetName)
                                    .Replace("[:TableName:]", tableName)
                                    .Replace("[:DataSourceName:]", dataSourceName)
                                    .Replace("[:TableAdapterName:]", tableAdapterName)
                                    .Replace("[:CommandText:]", commandText)
                                    .Replace("[/Fields/]", Fields);
            }
            #endregion


            #region ReportItems
            String ReportItems = "";
            //a table
            
            float headerHeight = 2.5f;
            {
                string dataSourceName = "DummyDataSource";
                String tableName = "Tablix1";
                string dataSetName = "DataSet1";
                itemY+=0.5f;
                String ATable = 
                  Tablix
                        .Replace("[:DataSourceName:]", dataSourceName)
                        .Replace("[:DataSetName:]", dataSetName)
                        .Replace("[:TableTop:]", (itemY).ToString())// titleItems.ToString())
                        .Replace("[:TableName:]", tableName)
                        .Replace("[/FieldCells/]", FieldCells)
                        .Replace("[/HeaderCells/]", HeaderCells)
                        .Replace("[/TableColumns/]", TableColumns)
                        .Replace("[/TableRows/]", TableRows)
                        .Replace("[/ColumnMembers/]", ColumnMembers)
                        .Replace("[:HeaderHeight:]", (headerHeight).ToString())
                        .Replace("[:TableLeft:]", "0")
                        .Replace("[:TableWidth:]", (_totalWid).ToString())
                        .Replace("[:TableHeight:]", (rowNo * 0.5 + headerHeight).ToString())
                        .Replace("[/RowMembers/]", RowMembers);
                
                
                ReportItems += ATable;
                itemY+=(rowNo*0.5f)+headerHeight;
                
                itemY += 0.5f;//공백
                /*
                ReportItems += AChart   .Replace("[:ChartName:]", "Sparkling1")
                                        .Replace("[:LabelFieldName:]", "Name")
                                        .Replace("[:ValueFieldName:]", "Fails")
                                        .Replace("[:DataSetName:]", dataSetName)
                                        .Replace("[:Top:]", itemY.ToString())
                                        .Replace("[:Left:]", "2")
                                        .Replace("[:Width:]", "10")
                                        .Replace("[:Height:]", "10")
                    ;
                 
                itemY += 10;
                */
                itemY += 0.5f;//공백
            }
            
            #endregion


            #region ReportBase
            String Report = ReportBase;
            //if (_dataSourceName.Length == 0) _dataSourceName = "DummyDataSource";
            float pageHeight = (_pageWidth)*(29.7f/21.0f);
            
            Report = Report
                    .Replace("[:TotalWidth:]", reportWidth.ToString())
                    .Replace("[:PageWidth:]", (_pageWidth).ToString())
                    .Replace("[:PageHeight:]", (pageHeight).ToString())
                    .Replace("[:BodyHeight:]", (pageHeight-2).ToString())
                    .Replace("[:Margin:]", (_margin).ToString())
                    .Replace("[/DataSources/]", DataSources)
                    .Replace("[/ReportItems/]", ReportItems)
                    .Replace("[/DataSets/]", DataSets)
                    .Replace("[/TitleItems/]", Titles + "\r\n" + Infos)
                    ;

            #endregion

            File.WriteAllText(file, Report);
        }

    }
}
