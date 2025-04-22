using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using CefSharp.DevTools.CSS;
using DevExpress.XtraDiagram.Base;
using DevExpress.Utils;
using DevExpress.XtraGrid.Drawing;

namespace WeavingGenerator
{
    public partial class DialogWeftArray : DevExpress.XtraEditors.XtraForm
    {
        // 경위사 배열 자료 구조
        List<ArrayTempData> arrayTempList;
        
        // 반복 자료 구조 (데이터 추가/수정/삭제 시 리스트 업데이트)
        List<RepeatTempData> repeatTempList;
        // 테이블 형식으로 변환 (GridView 에 표시하기 위한 자료 구조 )
        List<List<RepeatTempData>> repeatMatrix;



        // GridControl의 데이터 테이블
        DataTable dt;

        // 배열 개수
        int nCol = 4;

        // 각 셀의 폭 및 높이 (고정)
        int CELL_WIDTH_NAME = 120;
        int CELL_WIDTH_COLOR = 80;
        int CELL_WIDTH_COUNT = 80;
        int CELL_WIDTH = 50;
        int CELL_HEIGHT = 30;

        // 이동 가능 셀
        bool IsMovable = false;
        // 셀 정보
        int MOVE_ROW = -1;
        string MOVE_COL = "";


        // 팝업 메뉴
        DevExpress.Utils.Menu.DXMenuItem menuAdd;
        DevExpress.Utils.Menu.DXMenuItem menuInsert;
        DevExpress.Utils.Menu.DXMenuItem menuDelete;
        DevExpress.Utils.Menu.DXMenuItem menuCopy;
        DevExpress.Utils.Menu.DXMenuItem menuPaste;
        DevExpress.Utils.Menu.DXMenuItem menuRepeat;
        GridViewMenu menu;
        // 팝업 복사 데이터
        List<int> copyIndexList = new List<int>();


        MainForm mainForm;
        ProjectData data;

        List<WInfo> wInfoList = new List<WInfo>();

        bool isModified = false;

        public DialogWeftArray(MainForm main, ProjectData data)
        {
            this.mainForm = main;
            this.data = data;
            InitializeComponent();
        }

        private void DialogWeftArray_Load(object sender, EventArgs e)
        {
            if(data == null)
            {
                return;
            }

            ///////////////////////////////////////////////////////////////////
            // 시작 - 테스트 자료
            ///////////////////////////////////////////////////////////////////
            //arrayTempList = new List<ArrayModelData>();
            //arrayTempList.Add(new ArrayTempData(0, 0, "FF0000", new List<string> { "2", "", "", "" }));
            //arrayTempList.Add(new ArrayTempData(1, 1, "000000", new List<string> { "", "1", "", "" }));
            //arrayTempList.Add(new ArrayTempData(2, 0, "FF0000", new List<string> { "", "", "", "9" }));
            //arrayTempList.Add(new ArrayTempData(3, 1, "000000", new List<string> { "", "", "3", "" }));
            arrayTempList = ToArrayTempList(data.Weft);


            //repeatTempList = new List<RepeatModelData>();
            //repeatTempList.Add(new RepeatModelData(0, 1, 2));
            //repeatTempList.Add(new RepeatModelData(1, 2, 3));
            //repeatTempList.Add(new RepeatModelData(0, 2, 4));
            //repeatTempList.Add(new RepeatModelData(1, 3, 5));
            //repeatTempList.Add(new RepeatModelData(2, 3, 6));
            //repeatTempList.Add(new RepeatModelData(1, 2, 7));
            //repeatTempList.Add(new RepeatModelData(0, 1, 8));

            repeatTempList = ToRepeatTempData(data.Weft);

            ///////////////////////////////////////////////////////////////////
            // 끝 - 테스트 자료
            ///////////////////////////////////////////////////////////////////

            //gridView1.ShownEditor += gridView1_ShownEditor;
            // 해더 텍스트 가운데 정렬
            gridView1.RowCellStyle += GridView1_RowCellStyle;
            // 그리드 설정
            gridView1.PopupMenuShowing += GridView1_PopupMenuShowing;
            // 그리드뷰에 세로줄 그리기
            gridControl1.PaintEx += GridControl1_PaintEx;
            // 반복 그리기
            gridView1.CustomDrawCell += GridView1_CustomDrawCell;
            // 데이터 변경 이벤트
            gridView1.CellValueChanging += GridView1_CellValueChanging;
            gridView1.CellValueChanged += GridView1_CellValueChanged;

            gridView1.KeyUp += GridView1_KeyUp;

            gridView1.ShowingEditor += GridView1_ShowingEditor;

			gridView1.RowCellClick += GridView1_RowCellClick;

            // 3. 팝업 메뉴
            menuAdd = new DevExpress.Utils.Menu.DXMenuItem("추가", new EventHandler(Menu_ItemAddClick));
            menuInsert = new DevExpress.Utils.Menu.DXMenuItem("삽입", new EventHandler(Menu_ItemInsertClick));
            menuDelete = new DevExpress.Utils.Menu.DXMenuItem("삭제", new EventHandler(Menu_ItemDeleteClick));
            menuCopy = new DevExpress.Utils.Menu.DXMenuItem("복사", new EventHandler(Menu_ItemCopyClick));
            menuPaste = new DevExpress.Utils.Menu.DXMenuItem("붙여넣기", new EventHandler(Menu_ItemPasteClick));
            menuRepeat = new DevExpress.Utils.Menu.DXMenuItem("반복", new EventHandler(Menu_ItemRefeatClick));

            menuPaste.Enabled = false;
            menuRepeat.Enabled = true;

            menu = new GridViewMenu(gridView1);
            menu.Items.Add(menuAdd);
            menu.Items.Add(menuInsert);
            menu.Items.Add(menuDelete);
            menu.Items.Add(menuCopy);
            menu.Items.Add(menuPaste);
            menu.Items.Add(menuRepeat);
            // 4. view 데이터
            initListView();







			/*
            ///////////////////////////////////////////////////////////////////
            // ComboBox 설정 - 경사 정보
            ///////////////////////////////////////////////////////////////////
            //Weft weft = data.Weft;



            
            ///////////////////////////////////////////////////////////////////
            // 경사 배열
            ///////////////////////////////////////////////////////////////////
            List<WArray> list = weft.GetWArrayList();
            for (int i = 0; i < list.Count; i++)
            {
                WArray arr = list[i];
                int idx = arr.Idx;
                int cnt = arr.Count;
                WInfo info = weft.GetWInfo(idx);
                Color c = info.GetColor();

                ListViewItem newitem = new ListViewItem(new String[] { i + "", cnt + "", "경사 " + (idx), "" });
                //listView1.Items.Add(newitem);

                newitem.UseItemStyleForSubItems = false;

                ListViewItem.ListViewSubItem subItem = newitem.SubItems[3];
                subItem.BackColor = c;
            }
            */

            bool IsEnableBtnCopy = GetEnableBtnCopy();

            if(IsEnableBtnCopy == true)
            {
                simpleButton_Copy.Enabled = true;
            }
        }


        private bool GetEnableBtnCopy()
        {
            Warp warp = data.Warp;
            Weft weft = data.Weft;

            List<WInfo> warpInfoList = warp.GetWInfoList();
            List<WInfo> weftInfoList = weft.GetWInfoList();

            if (warpInfoList.Count <= 0)
            {
                return false;
            }
            //경사, 위사의 원사 수가 같은 경우
            int cntWarpInfo = warpInfoList.Count;
            int cntWeftInfo = weftInfoList.Count;

            if(warpInfoList.Count == weftInfoList.Count)
            {
                return true;
            }

            return false;
        }

        // 뷰 데이터 초기화
        private void initListView()
        {
            if (dt != null)
            {
                dt.Columns.Clear();
                dt.Rows.Clear();
                dt.Clear();
                dt = null;
                gridView1.Columns.Clear();
                gridControl1.DataSource = null;
                gridControl1.RefreshDataSource();

            }

            dt = new DataTable();

            DataColumn col;

            col = new DataColumn();
            col.DataType = typeof(string);
            col.Caption = "Name";
            col.ColumnName = "Name";
            dt.Columns.Add(col);

            col = new DataColumn();
            col.DataType = typeof(string);
            col.Caption = "Color";
            col.ColumnName = "Color";
            dt.Columns.Add(col);

            col = new DataColumn();
            col.DataType = typeof(string);
            col.Caption = "Count";
            col.ColumnName = "Count";
            dt.Columns.Add(col);

            nCol = arrayTempList[0].Values.Count;
            for (int i = 0; i < nCol; i++)
            {
                string columnName = (i + 1).ToString();
                col = new DataColumn();
                col.DataType = typeof(string);
                col.Caption = columnName;
                col.ColumnName = columnName;
                dt.Columns.Add(col);
            }

            // 5. 데이터
            for (int i = 0; i < arrayTempList.Count; i++)
            {
                ArrayTempData arr = arrayTempList[i];
                string name = arr.Name;
                int count = arr.Count;
                List<string> list = arr.Values;

                DataRow row = dt.NewRow();
                row["Name"] = name;
                row["Color"] = "";
                row["Count"] = count;

                for (int j = 0; j < list.Count; j++)
                {
                    string v = list[j].ToString();
                    row[(j + 1) + ""] = v;
                }
                dt.Rows.Add(row);

                //Trace.WriteLine("DATA ROW : " + row);
            }

            // 6. 반복 데이터
            // 반복 데이터 row col 갱신
            repeatMatrix = ToRepeatMetrix(repeatTempList);
            for (int i = 0; i < repeatMatrix.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["Name"] = "반복";
                row["Color"] = "";
                row["Count"] = "";

                List<RepeatTempData> cols = repeatMatrix[i];
                for (int j = 0; j < cols.Count; j++)
                {
                    RepeatTempData r = cols[j];
                    if (r == null)
                    {
                        row[(j + 1) + ""] = "";
                    }
                    else
                    {
                        row[(j + 1) + ""] = r.RepeatCnt;
                    }
                }
                dt.Rows.Add(row);
            }
            gridControl1.DataSource = dt;
            gridControl1.RefreshDataSource();
            gridView1.RefreshData();



            // 정렬 금지
            gridView1.OptionsCustomization.AllowSort = false;
            // 컬럼 폭 자동 설정 
            gridView1.OptionsView.ColumnAutoWidth = false;
            // 행 높이
            gridView1.RowHeight = CELL_HEIGHT;

            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                // 컬럼 타이틀 텍스트 중앙
                gridView1.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                // 컬럼 폭 설정 및 고정
                GridColumn column = gridView1.Columns[i];
                string columnName = column.FieldName;
                if (columnName == "Name")
                {
                    column.Width = CELL_WIDTH_NAME;
                    column.OptionsColumn.FixedWidth = true;
                    column.OptionsColumn.AllowSize = false;
                }
                else if (columnName == "Color")
                {
                    //2025-02-05 soonchol
                    if(data.YardDyed == true)
                    {
                        column.Width = CELL_WIDTH_COLOR;
                    }
                    else
                    {
                        column.Visible = false;
                    }

                    column.OptionsColumn.FixedWidth = true;
                    column.OptionsColumn.AllowSize = false;
                }
                else if (columnName == "Count")
                {
                    column.Width = CELL_WIDTH_COUNT;
                    column.OptionsColumn.FixedWidth = true;
                    column.OptionsColumn.AllowSize = false;
                }
                else
                {
                    column.Width = CELL_WIDTH;
                    column.OptionsColumn.FixedWidth = true;
                    column.OptionsColumn.AllowSize = false;
                }

            }
            // merge
            //gridView1.CellMerge += GridView1_CellMerge;
            //gridView1.OptionsView.AllowCellMerge = true;
            //gridView1.ColumnWidthChanged += GridView1_ColumnWidthChanged;
            gridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView1.OptionsView.ShowIndicator = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;


        }

        ///////////////////////////////////////////////////////////////////////
        // 2 : search 범위가 크다
        // 1 : search 범위가 작다
        // -1 : 포함되지 않는다.
        ///////////////////////////////////////////////////////////////////////
        private int Contains(RepeatTempData r, RepeatTempData search)
        {
            int s = r.StartIdx;
            int e = r.EndIdx;

            int s1 = search.StartIdx;
            int e1 = search.EndIdx;


            if(s1 > e)
            {
                return -1;
            }
            if(e1 < s)
            {
                return -1;
            }

            if(s1 >= s && e1 <= e)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        ///////////////////////////////////////////////////////////////////////
        // 리스트 형식을 -> Grid Table 형식으로 변환
        ///////////////////////////////////////////////////////////////////////
        private List<List<RepeatTempData>> ToRepeatMetrix(List<RepeatTempData> list)
        {
            /// 데이터 초기화
            int rowsCount = 0;
            List<List<RepeatTempData>> rows = new List<List<RepeatTempData>>();
            for (int i = 0; i < list.Count; i++)
            {
                List<RepeatTempData> cols = new List<RepeatTempData>();
                for (int j = 0; j < nCol; j++)
                {
                    cols.Add(null);
                }
                rows.Add(cols);
            }

            ///////////////////////////////////////////////////////////////////
            /// Flag 초기화
            for (int i = 0; i < list.Count; i++)
            {
                RepeatTempData r = list[i];
                r.Flag = false;
            }

            // 반복 행 개수 파악
            List < RepeatTempData > temp = new List<RepeatTempData>();

            for (int i = 0; i < list.Count; i++)
            {
                temp.Clear();


                RepeatTempData r1 = list[i];
                temp.Add(r1);
                if (r1.Flag == true) continue;
                r1.Flag = true;
                for (int j = 0; j < list.Count; j++)
                {
                    if(i == j)
                    {
                        continue;
                    }
                    RepeatTempData r2 = list[j];
                    if (r2.Flag == true) continue;
                    if (Contains(r1, r2) > 0)
                    {
                        temp.Add(r2);
                        r2.Flag = true;
                    }
                }

                // 길이가 작은것을 가장 앞으로 정렬
                temp.Sort((x, y) => {
                    int len1 = x.EndIdx - x.StartIdx;
                    int len2 = y.EndIdx - y.StartIdx;

                    return len1 - len2;
                });
                rowsCount = Math.Max(rowsCount, temp.Count);
                //Trace.WriteLine("\n===================================");
                for (int z = 0; z < temp.Count; z++)
                {
                    //Trace.WriteLine(">>> " + temp[z].ToString());
                    RepeatTempData r = temp[z];
                    int s = r.StartIdx;
                    int e = r.EndIdx;
                    rows[z][s] = r;
                }
            }
            rows.RemoveRange(rowsCount, rows.Count - rowsCount);

            return rows;
        }

        ///////////////////////////////////////////////////////////////////////
        // Grid Table 형식을 -> 리스트 형식으로 변환
        ///////////////////////////////////////////////////////////////////////
        private List<RepeatTempData> ToRepeatList(List<List<RepeatTempData>> rows)
        {
            /// 데이터 초기화
            List<RepeatTempData> list = new List<RepeatTempData>();

            for (int i = 0; i < rows.Count; i++)
            {
                List<RepeatTempData> cols = rows[i];
                for (int j = 0; j < cols.Count; j++)
                {
                    RepeatTempData r = cols[j];
                    if(r != null)
                    {
                        RepeatTempData temp = new RepeatTempData(r.StartIdx, r.EndIdx, r.RepeatCnt);
                        list.Add(temp);
                    }
                }
            }

            return list;
        }
        ///////////////////////////////////////////////////////////////////////
        /// 시작 - 이벤트
        ///////////////////////////////////////////////////////////////////////

        // 그리드뷰 해더의 텍스트 정렬
        private void GridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }
        // 팝업 이벤트
        private void GridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {

            if (e.HitInfo.HitTest == DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.RowCell)
            {

                int selectedRow = -1;
                string selectedCol = "";
                int selectedIdx = -1;

                GetSelectedRowColumn(ref selectedRow, ref selectedCol, ref selectedIdx);
                if (selectedRow != -1)
                {
                    e.Menu = menu;
                }
            }
        }
        // 세로줄 그리기
        private void GridControl1_PaintEx(object sender, PaintExEventArgs e)
        {
            GridViewInfo vi = gridView1.GetViewInfo() as GridViewInfo;
            Pen standardPen = e.Cache.GetPen(gridView1.PaintAppearance.HorzLine.BackColor);
            Pen pen;
            Point p1 = Point.Empty;
            Point p2 = Point.Empty;

            foreach (GridRowInfo ri in vi.RowsInfo)
            {   
                ///////////////////////////////////////////////////////////////
                // 시작 - 세로 선 그리기
                ///////////////////////////////////////////////////////////////
                if (ri.IsGroupRow)
                    continue;

                //Pen p = standardPen;

                if (ri.RowHandle == gridView1.FocusedRowHandle)
                    pen = e.Cache.GetPen(gridView1.PaintAppearance.FocusedRow.BackColor);
                else
                    pen = standardPen;
                
                //Trace.WriteLine("\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" );

                // Name, Count 컬럼의 오른쪽 세로 선 그리기
                foreach (GridColumn column in gridView1.Columns)
                {
                    //Trace.WriteLine("Column : " + column);
                    GridColumnInfoArgs gci = vi.ColumnsInfo[column];
                    if(gci == null)
                    {
                        continue;
                    }
                    p1 = new Point(gci.Bounds.Right - 1, ri.Bounds.Y);
                    p2 = new Point(gci.Bounds.Right - 1, ri.Bounds.Bottom - 1);

                    if (column.FieldName == "Name" || column.FieldName == "Color" || column.FieldName == "Count")
                    {
                        //Trace.WriteLine("column.FieldName : " + column.FieldName + ", " + p1 + ", " + p2);
                        e.Cache.DrawLine(pen, p1, p2);
                    }
                }
                // 가장 오른쪽 컬럼의 오른쪽 세로 선 그리기
                e.Cache.DrawLine(pen, p1, p2);


                ///////////////////////////////////////////////////////////////
                // 끝 - 세로 선 그리기
                ///////////////////////////////////////////////////////////////
            }
        }



        ///////////////////////////////////////////////////////////////////////
        // 반복 그리기
        ///////////////////////////////////////////////////////////////////////
        private void GridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            int nRow = e.RowHandle;
            string strColumn = e.Column.FieldName;
            Rectangle rect = e.Bounds;


            // Name, Count 컬럼 예외
            if (string.Equals(e.Column.FieldName, "Name") || string.Equals(e.Column.FieldName, "Count"))
            {
                return;
            }

            // 색상 그리기
            if (string.Equals(e.Column.FieldName, "Color") && nRow < arrayTempList.Count)
            {
                ArrayTempData arrayTemp = arrayTempList[nRow];
                string hexColor = arrayTemp.HexColor;
                Color c = Util.ToColor(hexColor);
                e.Cache.FillRectangle(c, e.Bounds);
                return;
            }

            // 경사 개수 체크
            if (nRow < arrayTempList.Count)
            {
                return;
            }

            // 셀에 데이터가 없으면 그리지 않음 (DataRow)
            if (string.IsNullOrEmpty(e.DisplayText))
            {
                return;
            }


            ///////////////////////////////////////////////////////////////////
            //  반복 그리기 (이중 배열 데이터를 이용)
            ///////////////////////////////////////////////////////////////////
            int xPos = 0;
            int yPos = 0;
            Color lineColor = this.ForeColor;


            int i = nRow - arrayTempList.Count;
            int col = Util.ToInt(strColumn) - 1;
            RepeatTempData repeat = repeatMatrix[i][col];

            int start = repeat.StartIdx;
            int end = repeat.EndIdx;
            int n = repeat.RepeatCnt;

            xPos = rect.X + 1;
            yPos = rect.Y;
            int wRepeat = ((end - start) + 1) * CELL_WIDTH - 1;
            int hRepeat = CELL_HEIGHT - 1;

            Rectangle sRect = new Rectangle(xPos, yPos + 2, wRepeat, hRepeat);
            Pen pen = new Pen(lineColor);

            // 1. 좌측 세로줄
            Point p1 = new Point(xPos, yPos);
            Point p2 = new Point(xPos, yPos + CELL_HEIGHT - 2);
            e.Cache.DrawLine(pen, p1, p2);

            // 2. 우측 세로줄
            p1 = new Point(xPos + wRepeat - 2, yPos);
            p2 = new Point(xPos + wRepeat - 2, yPos + CELL_HEIGHT - 2);
            e.Cache.DrawLine(pen, p1, p2);


            // 3. 가로 줄
            int nLineArrow = 4;
            var point1 = new PointF(xPos, yPos + (CELL_HEIGHT - 5));
            var point2 = new PointF(xPos + (nLineArrow * 2), yPos + (CELL_HEIGHT - 5) - nLineArrow);
            var point3 = new PointF(xPos + (nLineArrow * 2), yPos + (CELL_HEIGHT - 5) + nLineArrow);
            e.Cache.FillPolygon(new[] { point1, point2, point3 }, lineColor);

            point1 = new PointF(xPos + wRepeat, yPos + CELL_HEIGHT - 5);
            point2 = new PointF(xPos + wRepeat - (nLineArrow * 2), yPos + CELL_HEIGHT - 5 - nLineArrow);
            point3 = new PointF(xPos + wRepeat - (nLineArrow * 2), yPos + CELL_HEIGHT - 5 + nLineArrow);
            e.Cache.FillPolygon(new[] { point1, point2, point3 }, lineColor);

            p1 = new Point(xPos, yPos + (CELL_HEIGHT - 5));
            p2 = new Point(xPos + wRepeat, yPos + CELL_HEIGHT - 5);
            e.Cache.DrawLine(pen, p1, p2);
        }
        private void GridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string value = e.Value.ToString();
            string v = (string)dt.Rows[MOVE_ROW][MOVE_COL];

            int result;
            bool isNumber = int.TryParse(value, out result);
            if(isNumber == false)
            {
                XtraMessageBox.Show("숫자를 입력해주세요. ");
                gridView1.SetRowCellValue(MOVE_ROW, MOVE_COL, v);
                return;
            }
            if (MOVE_ROW <= (arrayTempList.Count - 1))
            {
                if (result == 0)
                {
                    XtraMessageBox.Show("0 이상의 숫자를 입력해주세요. ");
                    gridView1.SetRowCellValue(MOVE_ROW, MOVE_COL, v);
                    return;
                }
            }
        }
        
        // 데이터 변경
        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //Trace.WriteLine("\n==========================");
            //Trace.WriteLine("CellValueChanged : " + MOVE_ROW + ", " + MOVE_COL);
            IsMovable = false;

            int row = MOVE_ROW;
            int col = Convert.ToInt32(MOVE_COL);
            string value = e.Value.ToString();

            UpdateListValue(MOVE_ROW, MOVE_COL, value);

            initListView();

            isModified = true;
        }

        // 컬럼 수정 및 이동
        private void GridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            int[] selectedRow = gridView1.GetSelectedRows();
            if(selectedRow == null || selectedRow.Length == 0)
            {
                e.Cancel = true;
                IsMovable = false;
                return;
            }
            int row = selectedRow[0];

            string col = gridView1.FocusedColumn.FieldName;
            if (col == "Name" || col == "Color" || col == "Count")
            {
                e.Cancel = true;
                IsMovable = false;
                return;
            }
            string v = (string)dt.Rows[row][col];
            if (string.IsNullOrEmpty(v))
            {
                e.Cancel = true;
                IsMovable = false;
                return;
            }



            if (row >= arrayTempList.Count)
            {
                e.Cancel = false;

                IsMovable = false;
                MOVE_ROW = row;
                MOVE_COL = col;
            }
            else
            {
                e.Cancel = false;

                IsMovable = true;
                MOVE_ROW = row;
                MOVE_COL = col;
            }

            //Trace.WriteLine(">> SELECTED_ROW : " + MOVE_ROW + ", SELECTED_COL : " + MOVE_COL);
        }
        // 키보드 위/아래 이동(컬럼 이동)
        private void GridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsMovable == false)
            {
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                if ((MOVE_ROW - 1) < 0)
                {
                    IsMovable = false;
                    return;
                }

                //Trace.WriteLine("Keys.Up : " + MOVE_ROW + ", " + MOVE_COL);


                string v = (string)dt.Rows[MOVE_ROW][MOVE_COL];

                // 1. DataTable 업데이트
                //dt.Rows[MOVE_ROW][MOVE_COL] = "";
                // 2. 리스트 데이터 업데이트
                UpdateListValue(MOVE_ROW, MOVE_COL, "");

                // 1. DataTable 업데이트
                //dt.Rows[MOVE_ROW - 1][MOVE_COL] = v;
                // 2. 리스트 데이터 업데이트
                UpdateListValue(MOVE_ROW - 1, MOVE_COL, v);

                MOVE_ROW -= 1;

                UpdateDataTableCount();
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (arrayTempList.Count <= (MOVE_ROW + 1))
                {
                    IsMovable = false;
                    return;
                }
                //Trace.WriteLine("Keys.Down : " + MOVE_ROW + ", " + MOVE_COL);
                string v = (string)dt.Rows[MOVE_ROW][MOVE_COL];

                // 1. 데이터 업데이트
                //dt.Rows[MOVE_ROW][MOVE_COL] = "";
                // 2. 리스트 데이터 업데이트
                UpdateListValue(MOVE_ROW, MOVE_COL, "");

                // 1. DataTable 업데이트
                //dt.Rows[MOVE_ROW + 1][MOVE_COL] = v;
                // 2. 리스트 데이터 업데이트
                UpdateListValue(MOVE_ROW + 1, MOVE_COL, v);


                MOVE_ROW += 1;

                UpdateDataTableCount();
            }

            //initListView();

            isModified = true;
        }

        private void GridView1_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            int row = e.RowHandle;
            string col = e.Column.FieldName;
            if (col == "Name" || col == "Color" || col == "Count")
            {
                IsMovable = false;
                return;
            }
            string v = (string)dt.Rows[row][col];
            if (string.IsNullOrEmpty(v))
            {
                IsMovable = false;
                return;
            }


            if (row >= arrayTempList.Count)
            {
                IsMovable = false;
                MOVE_ROW = row;
                MOVE_COL = col;
            }
            else
            {
                IsMovable = true;
                MOVE_ROW = row;
                MOVE_COL = col;
            }

        }


        private void btn_Add_Click(object sender, EventArgs e)
        {
            string strCnt = textEdit_AddCount.Text;
            int cnt = Util.ToInt(strCnt, 0);
            if (cnt <= 0)
            {
                XtraMessageBox.Show("숫자를 입력해주세요. ");
                textEdit_AddCount.Focus();
                return;
            }
            AddColumn(cnt);
            initListView();

            isModified = true;
        }
        ///////////////////////////////////////////////////////////////////////
        /// 팝업 메뉴
        ///////////////////////////////////////////////////////////////////////
        private void Menu_ItemAddClick(object sender, EventArgs e)
        {
            AddColumn(1);
            initListView();

            isModified = true;
        }
        private void Menu_ItemInsertClick(object sender, EventArgs e)
        {
            int selectedRow = -1;
            string selectedCol = "";
            int selectedIdx = -1;

            GetSelectedRowColumn(ref selectedRow, ref selectedCol, ref selectedIdx);

            if (selectedRow < 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(selectedCol))
            {
                return;
            }
            int result;
            bool isNumber = int.TryParse(selectedCol, out result);
            if (isNumber == false)
            {
                return;
            }
            int nCol = result - 1;
            AddColumn(nCol, 1);
            initListView();

            isModified = true;
        }
        private void Menu_ItemDeleteClick(object sender, EventArgs e)
        {
            GridCell[] arrCell = gridView1.GetSelectedCells();

            List<int> deleteIndexList = new List<int>();
            for (int i = 0; i < arrCell.Length; i++)
            {
                GridCell cell = arrCell[i];
                int idx = ColumnNameToIndex(cell.Column.ToString());
                //Trace.WriteLine("DELETE ColumnName : " + idx);
                if (!deleteIndexList.Contains(idx))
                {
                    deleteIndexList.Add(idx);
                }
            }

            deleteIndexList.Sort();

            for (int i = 0; i < arrayTempList.Count; i++)
            {
                ArrayTempData arr = arrayTempList[i];
                List<string> valueList = arr.Values;
                for (int j = (deleteIndexList.Count - 1); j > -1; j--)
                {
                    int idxDelete = deleteIndexList[j];
                    valueList.RemoveAt(idxDelete);
                }
            }


            ///////////////////////////////////////////////////////////////////
            // 배열이 삭제된 경우 해당 반복 삭제
            ///////////////////////////////////////////////////////////////////
            if (arrayTempList.Count > 0)
            {
                ArrayTempData arr = arrayTempList[0];
                int colCount = arr.Values.Count;

                for (int i = repeatMatrix.Count - 1; i > -1 ; i--)
                {
                    List<RepeatTempData> cols = repeatMatrix[i];
                    for (int j = cols.Count - 1; j > -1 ; j--)
                    {
                        RepeatTempData temp = cols[j];
                        if (temp != null)
                        {
                            if (colCount <= temp.EndIdx)
                            {
                                cols.RemoveAt(j);
                                cols[j] = null;
                            }
                        }
                    }
                }
                ///////////////////////////////////////////////////////////////
                // ** 기준 리스트 갱신
                ///////////////////////////////////////////////////////////////
                repeatTempList = ToRepeatList(repeatMatrix);
            }

            initListView();

            isModified = true;
        }

        

        private void Menu_ItemCopyClick(object sender, EventArgs e)
        {
            copyIndexList.Clear();

            GridCell[] arrCell = gridView1.GetSelectedCells();

            //Trace.WriteLine("\n=============================");
            for (int i = 0; i < arrCell.Length; i++)
            {
                GridCell cell = arrCell[i];

                int idx = ColumnNameToIndex(cell.Column.ToString());
                //Trace.WriteLine("ItemCopyClick ColumnName : " + idx);
                if (!copyIndexList.Contains(idx))
                {
                    copyIndexList.Add(idx);
                }
            }
            menuPaste.Enabled = true;
        }
        private int ColumnNameToIndex(string name)
        {
            if (name == "Name" || name == "Color" || name == "Count")
            {
                return -1;
            }
            int idx = Convert.ToInt32(name);
            idx = idx - 1;
            return idx;
        }
        private void Menu_ItemPasteClick(object sender, EventArgs e)
        {
            int selectedRow = -1;
            string selectedCol = "";
            int selectedIdx = -1;

            GetSelectedRowColumn(ref selectedRow, ref selectedCol, ref selectedIdx);
            if (selectedRow < 0)
            {
                return;
            }

            int idx = selectedIdx;

            copyIndexList.Sort();

            for (int i = 0; i < arrayTempList.Count; i++)
            {
                ArrayTempData arr = arrayTempList[i];

                List<string> temp = new List<string>();
                for (int j = 0; j < copyIndexList.Count; j++)
                {
                    int nColumn = copyIndexList[j];
                    temp.Add(arr.Values[nColumn]);
                }

                int n = idx;
                for (int j = 0; j < temp.Count; j++)
                {
                    arr.Values.Insert(n, temp[j]);
                    n = n + 1;
                }
            }

            initListView();
            copyIndexList.Clear();
            menuPaste.Enabled = false;

            isModified = true;
        }

        ///////////////////////////////////////////////////////////////////////
        /// 반복 팝업 메뉴
        ///////////////////////////////////////////////////////////////////////
        private void Menu_ItemRefeatClick(object sender, EventArgs e)
        {
            GridCell[] arrCell = gridView1.GetSelectedCells();
            int start = -1;
            int end = -1;
            
            for (int i = 0; i < arrCell.Length; i++)
            {
                GridCell cell = arrCell[i];

                int idx = ColumnNameToIndex(cell.Column.ToString());
                if (idx > -1)
                {
                    if (start == -1)
                    {
                        start = idx;
                        end = idx;
                    }
                    start = Math.Min(start, idx);
                    end = Math.Max(end, idx);
                }
            }

            //Trace.WriteLine("반복 팝업 메뉴 > min : " + start + ", max : " + end + ", IsValidRepeat : " + IsValidRepeat(start, end));

            if (IsValidRepeat(start, end) == true)
            {
                //RepeatTempData rep = new RepeatTempData(start, end, 2);
                //repeatTempList.Add(rep);
                AddRepeat(start, end, 2);
                initListView();

                isModified = true;
            }

        }
        private void AddRepeat(int s, int e, int r)
        {
            ///////////////////////////////////////////////////////////////
            // ** 기준 리스트 갱신
            ///////////////////////////////////////////////////////////////
            repeatTempList = ToRepeatList(repeatMatrix);

            RepeatTempData rep = new RepeatTempData(s, e, r);
            repeatTempList.Add(rep);
        }
        private bool IsValidRepeat(int s, int e)
        {
            ///////////////////////////////////////////////////////////////
            // ** 기준 리스트 갱신
            ///////////////////////////////////////////////////////////////
            repeatTempList = ToRepeatList(repeatMatrix);

            for (int i = 0; i < repeatTempList.Count; i++ )
            {
                RepeatTempData rep = repeatTempList[i];

                int s1 = rep.StartIdx;
                int e1 = rep.EndIdx;

                ///////////////////////////////////////////////////////////////
                // 부분 중첩인 경우 입력 불가
                ///////////////////////////////////////////////////////////////
                if(s > s1 && s <= e1)
                {
                    if(e > e1)
                    {
                        return false;
                    }
                }
                if (e >= s1 && e < e1)
                {
                    if (s < s1)
                    {
                        return false;
                    }
                }
                if (s == s1 && e == e1)
                {
                    return false;
                }
            }
            return true;
        }


        private void simpleButton_Top_Click(object sender, EventArgs e)
        {

            if (IsMovable == false)
            {
                return;
            }

            if ((MOVE_ROW - 1) < 0)
            {
                return;
            }

            //Trace.WriteLine("Keys.Down : " + MOVE_ROW + ", " + MOVE_COL);
            string v = (string)dt.Rows[MOVE_ROW][MOVE_COL];

            // 1. 이동 전 셀의 데이터 초기화
            UpdateListValue(MOVE_ROW, MOVE_COL, "");

            // 2. 이동 후 셀의 데이터 수정
            UpdateListValue(0, MOVE_COL, v);

            // 3. 셀 선택
            MOVE_ROW = 0;
            gridView1.ClearSelection();
            gridView1.SelectCell(MOVE_ROW, gridView1.Columns[MOVE_COL]);

            // 4. 테이블의 Count컬럼 숫자 업데이트
            UpdateDataTableCount();

            isModified = true;
        }

        private void simpleButton_Up_Click(object sender, EventArgs e)
        {
            if (IsMovable == false)
            {
                return;
            }

            if ((MOVE_ROW - 1) < 0)
            {
                return;
            }
            
            string v = (string)dt.Rows[MOVE_ROW][MOVE_COL];

            // 1. 이동 전 셀의 데이터 초기화
            UpdateListValue(MOVE_ROW, MOVE_COL, "");

            // 2. 이동 후 셀의 데이터 수정
            UpdateListValue(MOVE_ROW - 1, MOVE_COL, v);

            // 3. 셀 선택
            MOVE_ROW -= 1;
            gridView1.ClearSelection();
            gridView1.SelectCell(MOVE_ROW, gridView1.Columns[MOVE_COL]);

            UpdateDataTableCount();

            isModified = true;
        }

        private void simpleButton_Down_Click(object sender, EventArgs e)
        {
            if (IsMovable == false)
            {
                return;
            }

            if ((MOVE_ROW + 1) >= arrayTempList.Count)
            {
                return;
            }

            //Trace.WriteLine("Keys.Down : " + MOVE_ROW + ", " + MOVE_COL);
            string v = (string)dt.Rows[MOVE_ROW][MOVE_COL];

            // 1. 이동 전 셀의 데이터 초기화
            UpdateListValue(MOVE_ROW, MOVE_COL, "");

            // 2. 이동 후 셀의 데이터 수정
            UpdateListValue(MOVE_ROW + 1, MOVE_COL, v);

            // 3. 셀 선택
            MOVE_ROW += 1;
            gridView1.ClearSelection();
            gridView1.SelectCell(MOVE_ROW, gridView1.Columns[MOVE_COL]);

            UpdateDataTableCount();

            isModified = true;
        }

        private void simpleButton_Bottom_Click(object sender, EventArgs e)
        {
            if (IsMovable == false)
            {
                return;
            }

            if ((MOVE_ROW + 1) >= arrayTempList.Count)
            {
                return;
            }

            //Trace.WriteLine("Keys.Down : " + MOVE_ROW + ", " + MOVE_COL);
            string v = (string)dt.Rows[MOVE_ROW][MOVE_COL];

            // 1. 이동 전 셀의 데이터 초기화
            UpdateListValue(MOVE_ROW, MOVE_COL, "");

            // 2. 이동 후 셀의 데이터 수정
            UpdateListValue(arrayTempList.Count - 1, MOVE_COL, v);

            // 3. 셀 선택
            MOVE_ROW = arrayTempList.Count - 1;
            gridView1.ClearSelection();
            gridView1.SelectCell(MOVE_ROW, gridView1.Columns[MOVE_COL]);

            UpdateDataTableCount();

            isModified = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            //Trace.WriteLine("\n===========================================");
            for (int i = 0; i < arrayTempList.Count; i++)
            {
                ArrayTempData arr = arrayTempList[i];
                //Trace.WriteLine(arr.ToString());
            }
            //Trace.WriteLine("\n===========================================");
            for (int i = 0; i < repeatTempList.Count; i++)
            {
                RepeatTempData r = repeatTempList[i];
                //Trace.WriteLine(r.ToString());
            }




            //Trace.WriteLine("\n===========================================");
            List<WArray> arrInfoList = ToWArray(arrayTempList);
            for (int i = 0; i < arrInfoList.Count; i++)
            {
                WArray arr = arrInfoList[i];
                int idx = arr.Idx;
                int cnt = arr.Count;
                //Trace.WriteLine(idx + " : " + cnt);
            }
            if (arrayTempList == null || arrayTempList.Count == 0)
            {
                return;
            }
            ///////////////////////////////////////////////////////////////
            // ** 기준 리스트 갱신
            ///////////////////////////////////////////////////////////////
            repeatTempList = ToRepeatList(repeatMatrix);
            List<WRepeat> repeatList = ToRepeat(repeatTempList);
            for (int i = 0; i < repeatList.Count; i++)
            {
                WRepeat arr = repeatList[i];
                int sIdx = arr.StartIdx;
                int eIdx = arr.EndIdx;
                int cnt = arr.RepeatCnt;
                //Trace.WriteLine(sIdx + " ~ " + eIdx + " : " + cnt);
            }

            data.Weft.SetWArrayList(arrInfoList);
            data.Weft.SetWRepeatList(repeatList);

            if (isModified == true)
            {
                // 업데이트
                mainForm.UpdateProjectData();
            }
            this.Close();
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        ///////////////////////////////////////////////////////////////////////
        // 경사의 배열 반복 설정과 동일하게 (색상 제외)
        ///////////////////////////////////////////////////////////////////////
        private void simpleButton_Copy_Click(object sender, EventArgs e)
        {
            Weft weft = data.Weft;
            Warp warp = data.Warp;
            bool IsEnableBtnCopy = GetEnableBtnCopy();

            if (IsEnableBtnCopy == false)
            {
                XtraMessageBox.Show("경사의 사용 원사 개수와 동일 해야 합니다.");
                return;
            }

            // 중요) 경사 배열을 복사 하여 적용(색상은 제외)
            // 중요) 경사의 사용 원사 개수와 동일 해야 함
            // 중요) 경사 idx 와 원사 idx가 동일해야 함 (= 생성시 max + 1로 생성하여 개수가 같으면 idx도 동일함)
            arrayTempList = ToArrayTempList(weft, warp);
            repeatTempList = ToRepeatTempData(warp);
            initListView();

            isModified = true;
        }

        ///////////////////////////////////////////////////////////////////////
        /// 끝 - 이벤트
        ///////////////////////////////////////////////////////////////////////





        // 데이터 조회
        private void GetSelectedRowColumn(ref int row, ref string col, ref int colIdx)
        {
            int selectedRow = -1;
            string columnName = "";
            int columnIdx = -1;

            int[] selectedRows = gridView1.GetSelectedRows();
            if (selectedRows == null || selectedRows.Length == 0)
            {
                row = -1;
                col = "";
                columnIdx = -1;
                return;
            }
            selectedRow = selectedRows[0];

            GridColumn gridColumn = gridView1.FocusedColumn;

            columnName = gridColumn.FieldName;
            if (string.Equals(columnName, "Name") || string.Equals(columnName, "Color") || string.Equals(columnName, "Count"))
            {
                row = -1;
                col = "";
                columnIdx = -1;
                return;
            }
            columnIdx = Convert.ToInt32(columnName);
            columnIdx = columnIdx - 1;

            if (arrayTempList.Count <= selectedRow)
            {
                row = -1;
                col = "";
                columnIdx = -1;
                return;
            }

            string v = (string)dt.Rows[selectedRow][columnName];
            if (string.IsNullOrEmpty(v))
            {
                row = selectedRow;
                col = columnName;
                colIdx = columnIdx;
                return;
            }
            row = selectedRow;
            col = columnName;
            colIdx = columnIdx;
        }

        ///////////////////////////////////////////////////////////////////////
        // 컬럼 추가 
        ///////////////////////////////////////////////////////////////////////
        private void AddColumn(int n)
        {
            int lastIdx = arrayTempList[0].Values.Count;
            AddColumn(lastIdx, n);
        }
        private void AddColumn(int idxInsert, int n)
        {
            for (int i = 0; i < arrayTempList.Count; i++)
            {
                ArrayTempData arr = arrayTempList[i];

                for(int j = 0; j < n; j++)
                {
                    ///////////////////////////////////////////////////////////
                    // 첫번째 행 에만 데이터 입력
                    ///////////////////////////////////////////////////////////
                    if (i == 0)
                    {
                        arr.Values.Insert(idxInsert, "1");
                    }
                    else
                    {
                        arr.Values.Insert(idxInsert, "");
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        // 사용자가 입력한 배열/반복 데이터 업데이트
        ///////////////////////////////////////////////////////////////////////
        private void UpdateListValue(int r, string c, string v)
        {
            // 1. DataTable 업데이트
            dt.Rows[r][c] = v;


            int row = 0;
            int col = 0;
            // 2. 배열 리스트
            if (r <= (arrayTempList.Count - 1))
            {
                row = r;
                col = Convert.ToInt32(c);
                col = col -1;
                arrayTempList[row].Values[col] = v.ToString();
            }
            // 3. 반복 리스트
            else if (r > (arrayTempList.Count - 1))
            {
                row = (r - arrayTempList.Count);
                col = Convert.ToInt32(c);
                col = col - 1;

                if (string.IsNullOrEmpty(v) || v == "0")
                {
                    repeatMatrix[row].RemoveAt(col);
                    repeatMatrix[row][col] = null;
                }
                else
                {
                    int value = Convert.ToInt32(v);
                    RepeatTempData temp = repeatMatrix[row][col];
                    temp.RepeatCnt = value;
                }
                ///////////////////////////////////////////////////////////////
                // ** 기준 리스트 갱신
                ///////////////////////////////////////////////////////////////
                repeatTempList = ToRepeatList(repeatMatrix);
            }
        }
        // 데이터 테이블의 Count 갱신
        private void UpdateDataTableCount()
        {
            int rowCount = arrayTempList.Count;
            for (int i = 0; i < rowCount; i++)
            {
                int totalCnt = 0;
                for (int j = 0; j < nCol; j++)
                {
                    string v = dt.Rows[i][(j + 1) + ""].ToString();
                    if (!string.IsNullOrEmpty(v))
                    {
                        int cnt = Convert.ToInt32(v);
                        totalCnt += cnt;
                    }
                }
                dt.Rows[i]["Count"] = totalCnt + "";
            }
        }


        private List<ArrayTempData> ToArrayTempList(Weft weft)
        {
            List<ArrayTempData> list = new List<ArrayTempData>();
            if (weft == null)
            {
                return list;
            }

            List<WInfo> wInfoList = weft.GetWInfoList();
            if (wInfoList == null)
            {
                return list;
            }

            List<WArray> arrayInfo = weft.GetWArrayList();
            nCol = arrayInfo.Count;

            for (int i = 0; i < wInfoList.Count; i++)
            {
                WInfo wInfo = wInfoList[i];
                int idx = wInfo.Idx;
                string hexColor = wInfo.HexColor;

                List<string> valueList = new List<string>();
                for (int j = 0; j < arrayInfo.Count; j++)
                {
                    int tempIdx = arrayInfo[j].Idx;
                    int tempCount = arrayInfo[j].Count;
                    if (idx == tempIdx)
                    {
                        valueList.Add(tempCount.ToString());
                    }
                    else
                    {
                        valueList.Add("");
                    }
                }
                ArrayTempData arr = new ArrayTempData("위사" + (i+1), idx, hexColor, valueList);

                list.Add(arr);
            }
            return list;
        }
        /// 색상은 복사하지 않음
        private List<ArrayTempData> ToArrayTempList(Weft weft, Warp warp)
        {
            List<ArrayTempData> list = new List<ArrayTempData>();
            if (warp == null || weft == null)
            {
                return list;
            }

            List<WInfo> wInfoListWarp = warp.GetWInfoList();
            List<WInfo> wInfoListWeft = weft.GetWInfoList();
            if (wInfoListWarp == null || wInfoListWeft == null)
            {
                return list;
            }
            if (wInfoListWarp.Count !=  wInfoListWeft.Count)
            {
                return list;
            }

            List<WArray> arrayInfoWarp = warp.GetWArrayList();
            List<WArray> arrayInfoWeft = warp.GetWArrayList();
            nCol = arrayInfoWarp.Count;

            ///////////////////////////////////////////////////////////////////
            // 1. 경사 로 배열 데이터 생성
            ///////////////////////////////////////////////////////////////////
            for (int i = 0; i < wInfoListWarp.Count; i++)
            {
                WInfo wInfo = wInfoListWarp[i];
                int idx = wInfo.Idx;
                string hexColor = wInfo.HexColor;

                List<string> valueList = new List<string>();
                for (int j = 0; j < arrayInfoWarp.Count; j++)
                {
                    int tempIdx = arrayInfoWarp[j].Idx;
                    int tempCount = arrayInfoWarp[j].Count;
                    if (idx == tempIdx)
                    {
                        valueList.Add(tempCount.ToString());
                    }
                    else
                    {
                        valueList.Add("");
                    }
                }
                ArrayTempData arr = new ArrayTempData("위사" + (i + 1), idx, hexColor, valueList);

                list.Add(arr);
            }
            ///////////////////////////////////////////////////////////////////
            // 2. 위사 정보로 업데이트
            // list와 위사 정보 개수가 똑같아야 함.
            ///////////////////////////////////////////////////////////////////
            for (int i = 0; i < list.Count; i++)
            {
                WInfo wInfo = wInfoListWeft[i];
                ArrayTempData arr = list[i];
                arr.Idx = wInfo.Idx;
                arr.HexColor = wInfo.HexColor;
            }
            return list;
        }
        private List<WArray> ToWArray(List<ArrayTempData> arrayTemp)
        {
            List<WArray> list = new List<WArray>();


            if (arrayTemp == null || arrayTemp.Count == 0)
            {
                return list;
            }

            // 빈 데이터 입력
            int n = arrayTemp[0].Values.Count;
            for (int i = 0; i < n; i++)
            {
                list.Add(new WArray());
            }

            // 데이터 채움
            for (int i = 0; i < arrayTemp.Count; i++)
            {
                ArrayTempData temp = arrayTemp[i];
                int weftIdx = temp.Idx;

                List<string> valueList = temp.Values;
                for (int j = 0; j < n; j++)
                {
                    string value = valueList[j];
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        WArray weftArrayInfo = list[j];
                        weftArrayInfo.Idx = weftIdx;
                        weftArrayInfo.Count = Convert.ToInt32(value);
                    }
                }
            }
            return list;
        }
        private List<RepeatTempData> ToRepeatTempData(Weft weft)
        {
            List<RepeatTempData> list = new List<RepeatTempData>();
            if (weft == null)
            {
                return list;
            }

            List<WInfo> weftInfoList = weft.GetWInfoList();
            if (weftInfoList == null)
            {
                return list;
            }

            List<WRepeat> repeatList = weft.GetWRepeatList();
            if (repeatList == null)
            {
                return list;
            }
            for (int i = 0; i < repeatList.Count; i++)
            {
                WRepeat r = repeatList[i];
                int startIdx = r.StartIdx;
                int endIdx = r.EndIdx;
                int repeatCnt = r.RepeatCnt;

                RepeatTempData temp = new RepeatTempData(startIdx, endIdx, repeatCnt);
                list.Add(temp);
            }
            return list;
        }

        private List<RepeatTempData> ToRepeatTempData(Warp warp)
        {
            List<RepeatTempData> list = new List<RepeatTempData>();
            if (warp == null)
            {
                return list;
            }

            List<WInfo> weftInfoList = warp.GetWInfoList();
            if (weftInfoList == null)
            {
                return list;
            }

            List<WRepeat> repeatList = warp.GetWRepeatList();
            if (repeatList == null)
            {
                return list;
            }
            for (int i = 0; i < repeatList.Count; i++)
            {
                WRepeat r = repeatList[i];
                int startIdx = r.StartIdx;
                int endIdx = r.EndIdx;
                int repeatCnt = r.RepeatCnt;

                RepeatTempData temp = new RepeatTempData(startIdx, endIdx, repeatCnt);
                list.Add(temp);
            }
            return list;
        }

        private List<WRepeat> ToRepeat(List<RepeatTempData> repeatTemp)
        {
            List<WRepeat> list = new List<WRepeat>();

            if (repeatTemp == null || repeatTemp.Count == 0)
            {
                return list;
            }

            for (int i = 0; i < repeatTemp.Count; i++)
            {
                RepeatTempData r = repeatTemp[i];
                int startIdx = r.StartIdx;
                int endIdx = r.EndIdx;
                int repeatCnt = r.RepeatCnt;

                WRepeat repeat = new WRepeat();
                repeat.StartIdx = startIdx;
                repeat.EndIdx = endIdx;
                repeat.RepeatCnt = repeatCnt;

                list.Add(repeat);
            }
            return list;
        }
    }



    public class RepeatTempData
    {
        private int startIdx;
        private int endIdx;
        private int repeatCnt;

        private bool flag = false;

        public RepeatTempData(int sIndex, int eIndex, int repeat)
        {
            this.startIdx = sIndex;
            this.endIdx = eIndex;

            this.repeatCnt = repeat;
        }
        public int StartIdx
        {
            get { return startIdx; }
            set { startIdx = value; }
        }

        public int EndIdx
        {
            get { return endIdx; }
            set { endIdx = value; }
        }

        public int RepeatCnt
        {
            get { return repeatCnt; }
            set { repeatCnt = value; }
        }

        public bool Flag
        {
            get { return flag; }
            set { flag = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("S : " + startIdx + "\t E : " + endIdx + "\t R : " + repeatCnt);
            return sb.ToString();
        }
    }
    public class ArrayTempData
    {
        string name;

        int idx;
        string hexColor;

        int count;
        List<string> values;

        public ArrayTempData(string name)
        {
            this.name = name;
            values = new List<string>();
            values.Add("1");

            this.count = 1;
        }
        public ArrayTempData(string name, List<string> values)
        {
            this.name = name;
            this.values = values;

            this.count = 1;
        }
        public ArrayTempData(string name, int idx, string weftHexColor, List<string> values)
        {
            this.name = name;

            this.idx = idx;
            this.hexColor = weftHexColor;
            this.values = values;

            this.count = 1;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Idx
        {
            get { return idx; }
            set { idx = value; }
        }
        public string HexColor
        {
            get { return hexColor; }
            set { hexColor = value; }
        }
        public int Count
        {
            get
            {
                int cnt = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    string str = values[i];
                    if (!string.IsNullOrEmpty(str))
                    {
                        int c = Convert.ToInt32(str);
                        cnt += c;
                    }
                }
                return cnt;
            }
            set { count = value; }
        }
        public List<string> Values
        {
            get { return values; }
            set { values = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name + "[" + count + "]:\t");
            for (int i = 0; i < values.Count; i++)
            {
                string v = values[i].ToString();
                sb.Append(v + "\t");
            }

            return sb.ToString();
        }
    }
}
