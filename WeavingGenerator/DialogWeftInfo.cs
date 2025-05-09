using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ColorPick.Picker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using DevExpress.XtraGrid.Columns;
using System.Windows.Forms;

namespace WeavingGenerator
{
    public partial class DialogWeftInfo : DevExpress.XtraEditors.XtraForm
    {
        MainForm mainForm;
        ProjectData wData;
        //int SELECTED_IDX = -1;
        DataTable tb = new DataTable();
        //List<WInfo> listWeftInfo;
        List<Yarn> yarnList;

        string COLUMN_CHECK = "선택";
        string COLUMN_IDX = "번호";
        //string COLUMN_WEFTNAME = "경사명";
        string COLUMN_YARN = "원사";
        string COLUMN_COLOR = "색상";

        //int checkRowIndex = -1;

        // 자료 구조
        List<WInfoTemp> wInfoTempList;





        ///////////////////////////////////////////////////////////////////////
        //
        ///////////////////////////////////////////////////////////////////////
        DataTable tb_yarn = new DataTable();
        string COLUMN_YARN_CHECK = "선택";
        string COLUMN_YARN_IDX = "번호";
        string COLUMN_YARN_NAME = "원사명";
        string COLUMN_YARN_WEIGHT = "굵기";
        string COLUMN_YARN_UNIT = "단위";
        string COLUMN_YARN_TYPE = "종류";
        string COLUMN_YARN_TEXTURED = "가연여부";
        string COLUMN_YARN_METAL = "광택";
        string COLUMN_YARN_IMAGE = "이미지";
        string COLUMN_YARN_REG_DT = "등록일";


        Color colorOpt = Color.White;

        bool isModified = false;

        public DialogWeftInfo(MainForm main, ProjectData data)
        {
            this.mainForm = main;
            this.wData = data;

            InitializeComponent();
        }

        private void DialogWeftInfo_Load(object sender, EventArgs e)
        {
            colorEdit_Change.Color = colorOpt;
            colorEdit_Change.CustomDisplayText += ColorEdit_Change_CustomDisplayText;
            colorEdit_Change.Click += ColorEdit_Change_Click;
            colorEdit_Change.ReadOnly = true;

            //2025-02-05 soonchol
            if (wData.YarnDyed == false)
            {
                colorEdit_Change.Visible = false;
                simpleButton_Color.Visible = false;
            }

            ///////////////////////////////////////////////////////////////////
            // 데이터 설정
            ///////////////////////////////////////////////////////////////////
            if (wData == null)
            {
                return;
            }
            // 원사 정보 초기화
            yarnList = mainForm.ListDAOYarn();

            // 경사 정보 초기화
            wInfoTempList = ToWInfoTempList(wData.Weft);


            ///////////////////////////////////////////////////////////////////
            // 뷰 설정
            ///////////////////////////////////////////////////////////////////
            gridView1.OptionsView.ShowIndicator = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsSelection.MultiSelect = false;
            // 정렬 금지
            gridView1.OptionsCustomization.AllowSort = false;
            // 컬럼 폭 자동 설정 
            gridView1.OptionsView.ColumnAutoWidth = false;
            // 
            gridView1.ShowingEditor += GridView1_ShowingEditor;

            gridView1.CellValueChanging += GridView1_CellValueChanging;

            ///////////////////////////////////////////////////////////////////
            // 테이블 설정
            ///////////////////////////////////////////////////////////////////
            tb.Columns.Add(COLUMN_CHECK);
            tb.Columns.Add(COLUMN_IDX);
            //tb.Columns.Add(COLUMN_WeftNAME);
            tb.Columns.Add(COLUMN_YARN);

            //2025-02-05 soonchol
            if (wData.YarnDyed == true)
            {
                tb.Columns.Add(COLUMN_COLOR, typeof(Color));
            }

            initListView();

            RepositoryItemCheckEdit item = new RepositoryItemCheckEdit();
            item.ValueChecked = "Y";
            item.ValueUnchecked = "N";
            gridView1.Columns[COLUMN_CHECK].ColumnEdit = item;

            ///////////////////////////////////////////////////////////////////
            // 끝 - 데이터
            ///////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////////
            // 시작 - cell 에 컨트롤 적용
            ///////////////////////////////////////////////////////////////////
            RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
            repositoryItemButtonEdit.Click += btnYarnClick;
            //repositoryItemButtonEdit.ReadOnly = true;
            repositoryItemButtonEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            gridView1.Columns[COLUMN_YARN].ColumnEdit = repositoryItemButtonEdit;

            CustomColorPickEdit repositoryItemColorEdit = new CustomColorPickEdit();
            repositoryItemColorEdit.CustomDisplayText += ColorEdit_Change_CustomDisplayText;
            repositoryItemColorEdit.ReadOnly = true;
            repositoryItemColorEdit.Click += colorChangedClick;

            //2025-02-05 soonchol
            if (wData.YarnDyed == true)
            {
                gridView1.Columns[COLUMN_COLOR].ColumnEdit = repositoryItemColorEdit;
            }
            ///////////////////////////////////////////////////////////////////
            // 끝 - cell 에 컨트롤 적용
            ///////////////////////////////////////////////////////////////////

            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                // 컬럼 타이틀 텍스트 중앙
                gridView1.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                // 컬럼 폭 설정 및 고정
                GridColumn column = gridView1.Columns[i];
                string columnName = column.FieldName;
                if (columnName == COLUMN_CHECK)
                {
                    column.Width = 50;
                }
                else if (columnName == "번호")
                {
                    column.Width = 80;
                }
                else if (columnName == "원사")
                {
                    column.Width = 200;
                }
                else if (columnName == "색상")
                {
                    column.Width = 200;
                }
            }


            ///////////////////////////////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////
            initYarnList();
        }

        private void ColorEdit_Change_ColorChanged(object sender, EventArgs e)
        {
            ColorEdit colorEdit = (ColorEdit)sender;

            colorOpt = colorEdit.Color;

        }

        private void ColorEdit_Change_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            ColorPickEdit colorEdit = (sender as ColorPickEdit);
            if (colorEdit == null)
            {
                return;
            }
            Color c = colorEdit.Color;
            e.DisplayText = $"{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        protected Color DoShowColorDialog(Color color)
        {
            using (FrmColorPicker frm = new FrmColorPicker(new RepositoryItemColorPickEdit()))
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.SelectedColor = color;
                if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                {
                    return frm.SelectedColor;
                }
                else
                    return color;
            }
        }

        private void ColorEdit_Change_Click(object sender, EventArgs e)
        {
            ColorEdit colorEdit = (ColorEdit)sender;
            Color oldColor = colorEdit.Color;
            Color newColor = DoShowColorDialog(oldColor);
            colorEdit.Color = newColor;

            colorOpt = newColor;
        }

        private void initListView()
        {

            // 원사 정보 갱신
            if (yarnList != null)
            {
                yarnList.Clear();
            }
            tb.Rows.Clear();
            yarnList = mainForm.ListDAOYarn();


            // 데이터 갱신
            for (int i = 0; i < wInfoTempList.Count; i++)
            {
                WInfoTemp info = wInfoTempList[i];
                int idx = info.Idx;
                string name = info.Name;
                string hexcolor = info.HexColor;
                Color color = Util.ToColor(hexcolor);
                int rgb = color.ToArgb();


                int idxYarn = info.IdxYarn;
                string yarnName = GetYarnName(idxYarn);

                //tb.Rows.Add((idx + 1), name, yarnName, color);

                //2025-02-05 soonchol
                if (wData.YarnDyed == false)
                {
                    tb.Rows.Add(false, (idx + 1), yarnName);
                }
                else
                {
                    tb.Rows.Add(false, (idx + 1), yarnName, color);
                }
            }
            gridControl1.DataSource = tb;

        }

        ///////////////////////////////////////////////////////////////////////
        // 시작 - EVENT
        ///////////////////////////////////////////////////////////////////////
        private void GridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            string col = gridView1.FocusedColumn.FieldName;
            if (col == "번호")
            {
                e.Cancel = true;
                return;
            }
        }
        private void GridView2_ShowingEditor(object sender, CancelEventArgs e)
        {
            string col = gridView2.FocusedColumn.FieldName;
            if (col == COLUMN_YARN_CHECK)
            {
                e.Cancel = false;
                return;
            }
            e.Cancel = true;
            return;
        }
        private void GridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            /*
            if (e.Column.FieldName.Equals(COLUMN_CHECK))
            {
                //기존 check 를 uncheck 로 
                if (checkRowIndex > -1)
                {
                    int rowHandle = gridView1.GetRowHandle(checkRowIndex);
                    gridView1.SetRowCellValue(rowHandle, COLUMN_CHECK, "N");
                    checkRowIndex = -1;
                }

                if (e.Value.Equals("Y"))
                {
                    checkRowIndex = gridView1.GetDataSourceRowIndex(e.RowHandle);
                }
            }
            */
        }
        private void btnYarnClick(object sender, EventArgs e)
        {
            WInfoTemp wInfo = GetWInfoTemp(gridView1.FocusedRowHandle);
            int idxYarn = wInfo.IdxYarn;
            DialogYarnList dialog = new DialogYarnList(mainForm, idxYarn);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = mainForm.GetChildFormLocation();
            dialog.dialogOpenYarnEventHandler += new DialogOpenYarnEventHandler(this.openYarn);
            dialog.ShowDialog();
        }
        private void colorChangedClick(object sender, EventArgs e)
        {
            ColorEdit colorEdit = (ColorEdit)sender;
            Color oldColor = colorEdit.Color;
            Color newColor = DoShowColorDialog(oldColor);
            if (oldColor != newColor) 
            { 
                colorEdit.Color = newColor;

                WInfoTemp wInfo = GetWInfoTemp(gridView1.FocusedRowHandle);
                wInfo.SetColor(colorEdit.Color);

                //mainForm.UpdateProjectData();
                isModified = true;
            }
        }

        //int idxYarn = -1;
        private void openYarn(object sender, int idxYarn)
        {
            //Trace.WriteLine("Open Yarn : " + idxYarn);
            WInfoTemp wInfo = GetWInfoTemp(gridView1.FocusedRowHandle);
            int oldIdxYarn = wInfo.IdxYarn;
            // 원사 정보 초기화
            yarnList = mainForm.ListDAOYarn();
            if (oldIdxYarn != idxYarn)
            {
                string yarnName = GetYarnName(idxYarn);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gridView1.Columns[COLUMN_YARN], yarnName);

                wInfo.IdxYarn = idxYarn;

                isModified = true;
            }

            //mainForm.UpdateProjectData();
        }
        ///////////////////////////////////////////////////////////////////////
        // 끝 - EVENT
        ///////////////////////////////////////////////////////////////////////
        private string GetYarnName(int idx)
        {
            for (int i = 0; i < yarnList.Count; i++)
            {
                Yarn yarn = yarnList[i];
                if (yarn.Idx == idx)
                {
                    return yarn.Name;
                }
            }
            return "";
        }
        private void PrintTableColumn(int selectedIdx)
        {
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                StringBuilder sb = new StringBuilder();

                string str_idx = tb.Rows[i]["Idx"].ToString();
                int idx = Convert.ToInt32(str_idx);

                if (idx == selectedIdx)
                {
                    sb.Append("Idx : " + idx + "\t");

                    string name = tb.Rows[i]["Name"].ToString();
                    string color = tb.Rows[i]["Color"].ToString();
                    int rgb = Convert.ToInt32(color);
                    Color c = Color.FromArgb(rgb);
                    sb.Append("Name : " + name + "\tColor : " + Util.ToHexColor(c));
                }

                Trace.WriteLine(sb.ToString());
            }
        }
        private WInfoTemp GetWInfoTemp(int selectedIdx)
        {
            for (int i = 0; i < wInfoTempList.Count; i++)
            {
                if (i == selectedIdx)
                {
                    WInfoTemp info = wInfoTempList[i];
                    return info;
                }
            }
            return null;
        }
        private void DeleteWInfoTempList(int idx)
        {
            for (int i = 0; i < wInfoTempList.Count; i++)
            {
                WInfoTemp info = wInfoTempList[i];
                if (info.Idx == idx)
                {
                    wInfoTempList.RemoveAt(i);
                    break;
                }
            }
        }


        private void btn_Delete_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                string v = gridView1.GetRowCellValue(i, COLUMN_CHECK).ToString();
                string strIdx = gridView1.GetRowCellValue(i, COLUMN_IDX).ToString();

                if (v == "Y")
                {
                    //Trace.WriteLine("checked idx : " + strIdx);
                    int idx = Util.ToInt(strIdx);
                    idx = idx - 1;
                    if (idx < 0) idx = 0;
                    DeleteWInfoTempList(idx);
                }
            }
            initListView();

            isModified = true;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            Color randomColor = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));

            int newIdx = 0;

            for (int i = 0; i < wInfoTempList.Count; i++)
            {
                WInfoTemp temp = wInfoTempList[i];
                newIdx = Math.Max(newIdx, temp.Idx);
            }
            newIdx += 1;

            WInfoTemp wInfoTemp = new WInfoTemp();
            wInfoTemp.Idx = newIdx;
            wInfoTemp.Name = "경사#" + (newIdx + 1); // 1부터 시작
            wInfoTemp.IdxYarn = -1;
            wInfoTemp.SetColor(randomColor);

            wInfoTempList.Add(wInfoTemp);

            initListView();
        }



        private List<WInfoTemp> ToWInfoTempList(Weft weft)
        {
            List<WInfoTemp> list = new List<WInfoTemp>();

            if (weft == null)
            {
                return list;
            }

            List<WInfo> wInfoList = weft.GetWInfoList();
            if (wInfoList == null)
            {
                return list;
            }

            for (int i = 0; i < wInfoList.Count; i++)
            {
                WInfo wInfo = wInfoList[i];
                int idx = wInfo.Idx;
                string name = wInfo.Name;
                string hexColor = wInfo.HexColor;
                int idxYarn = wInfo.IdxYarn;
                string nameYarn = GetYarnName(idxYarn);

                WInfoTemp temp = new WInfoTemp();
                temp.Idx = idx;
                temp.Name = name;
                temp.HexColor = hexColor;
                temp.IdxYarn = idxYarn;
                temp.NameYarn = nameYarn;
                list.Add(temp);
            }
            return list;
        }


        private bool IsContains(int idx)
        {
            bool ret = false;
            for (int i = 0; i < wInfoTempList.Count; i++)
            {
                WInfoTemp info = wInfoTempList[i];
                int idxInfo = info.Idx;
                if (idxInfo == idx)
                {
                    return true;
                }
            }
            return ret;
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {

            List<WArray> arrList = wData.Weft.GetWArrayList();
            List<WRepeat> repList = wData.Weft.GetWRepeatList();
            List<WInfo> wInfoList = new List<WInfo>();

            if (wInfoTempList.Count <= 0)
            {
                XtraMessageBox.Show("위사를 1개 이상 설정해야 합니다.");
                return;
            }
            //Trace.WriteLine("\n===========================================");
            for (int i = 0; i < wInfoTempList.Count; i++)
            {
                WInfoTemp temp = wInfoTempList[i];
                //Trace.WriteLine(arr.ToString());

                WInfo wInfo = new WInfo();
                wInfo.Idx = temp.Idx;
                wInfo.Name = temp.Name;
                wInfo.IdxYarn = temp.IdxYarn;
                wInfo.HexColor = temp.HexColor;
                wInfoList.Add(wInfo);
            }



            ///////////////////////////////////////////////////////////////////
            // 경사/위사가 추가 된 경우
            // 1. 배열 추가
            ///////////////////////////////////////////////////////////////////

            for (int i = 0; i < wInfoTempList.Count; i++)
            {
                WInfoTemp temp = wInfoTempList[i];

                int idx = temp.Idx;

                ///////////////////////////////////////////////////////////////
                // 배열 추가
                ///////////////////////////////////////////////////////////////
                bool IsAdd = true;
                for (int j = 0; j < arrList.Count; j++)
                {
                    WArray arr = arrList[j];
                    int idxArr = arr.Idx;

                    if (idxArr == idx)
                    {
                        IsAdd = false;
                        break;
                    }
                }
                if (IsAdd == true)
                {
                    int newIdx = 0;
                    List<WArray> list2 = wData.Weft.GetWArrayList();
                    for (int z = 0; z < list2.Count; z++)
                    {
                        WArray info = list2[z];
                        newIdx = Math.Max(newIdx, info.Idx);
                    }
                    newIdx += 1;

                    WArray wArray = new WArray();
                    wArray.Idx = newIdx;
                    wArray.Count = 1;
                    wData.Weft.AddWArray(wArray);
                }
            }



            ///////////////////////////////////////////////////////////////////
            // 경사/위사가 삭제 된 경우
            // 1. 배열 삭제
            // 2. 반복 삭제
            ///////////////////////////////////////////////////////////////////
            if (arrList != null)
            {
                ///////////////////////////////////////////////////////////////
                // 배열 삭제
                ///////////////////////////////////////////////////////////////
                for (int i = (arrList.Count - 1); i > -1; i--)
                {
                    WArray arr = arrList[i];
                    int idx = arr.Idx;

                    if (IsContains(idx) == false)
                    {
                        arrList.RemoveAt(i);
                    }
                }


                ///////////////////////////////////////////////////////////////
                // 반복 삭제
                ///////////////////////////////////////////////////////////////
                int colCount = arrList.Count;

                if (repList != null)
                {
                    for (int i = repList.Count - 1; i >= 0; i--)
                    {
                        WRepeat rep = repList[i];
                        if (colCount <= rep.EndIdx)
                        {
                            repList.RemoveAt(i);
                        }
                    }
                }
            }

            wData.Weft.SetWInfoList(wInfoList);
            wData.Weft.SetWArrayList(arrList);
            wData.Weft.SetWRepeatList(repList);

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



        private void initYarnList()
        {
            gridView2.OptionsView.ShowIndicator = false;
            gridView2.OptionsView.ShowGroupPanel = false;
            gridView2.OptionsSelection.MultiSelect = false;
            // 정렬 금지
            gridView2.OptionsCustomization.AllowSort = false;
            // 컬럼 폭 자동 설정 
            gridView2.OptionsView.ColumnAutoWidth = false;

            gridView2.OptionsCustomization.AllowSort = false;


            gridView2.ShowingEditor += GridView2_ShowingEditor;
            //
            //gridView2.CellValueChanged += GridView1_CellValueChanged;
            // 체크박스 다중 체크 금지
            //gridView2.CellValueChanging += GridView1_CellValueChanging;

            ///////////////////////////////////////////////////////////////////
            // 시작 - 데이터
            ///////////////////////////////////////////////////////////////////

            tb_yarn.Columns.Add(COLUMN_YARN_CHECK);
            tb_yarn.Columns.Add(COLUMN_YARN_IDX);
            tb_yarn.Columns.Add(COLUMN_YARN_NAME);
            tb_yarn.Columns.Add(COLUMN_YARN_WEIGHT, typeof(double));
            tb_yarn.Columns.Add(COLUMN_YARN_UNIT);
            tb_yarn.Columns.Add(COLUMN_YARN_TYPE);
            tb_yarn.Columns.Add(COLUMN_YARN_TEXTURED);
            tb_yarn.Columns.Add(COLUMN_YARN_METAL);
            tb_yarn.Columns.Add(COLUMN_YARN_IMAGE, typeof(Image));
            tb_yarn.Columns.Add(COLUMN_YARN_REG_DT);

            // 원사 정보 갱신
            tb_yarn.Rows.Clear();

            for (int i = 0; i < yarnList.Count; i++)
            {
                Yarn info = yarnList[i];

                int idx = info.Idx;
                string name = info.Name;
                // fixed by ilkwon. 2025.04.23        
                //string weight  = info.Weight;                 
                double weight = double.TryParse(info.Weight, out var w) ? w : 0.0;
                string unit = info.Unit;
                string type = info.Type;
                string textured = info.Textured;
                string metal = info.Metal;
                string image = info.Image;
                string reg_dt = info.Reg_dt;

                string temp = Util.ToDateHuman(reg_dt);

                Image img = Util.GetYarnResource(textured);

                tb_yarn.Rows.Add(false, idx, name, weight, unit, type, textured, metal, img, temp);
            }
            gridControl2.DataSource = tb_yarn;



            RepositoryItemCheckEdit item = new RepositoryItemCheckEdit();
            item.ValueChecked = "Y";
            item.ValueUnchecked = "N";
            gridView2.Columns[COLUMN_YARN_CHECK].ColumnEdit = item;

            RepositoryItemTrackBar repositoryItemTrackBarMetal = new RepositoryItemTrackBar();
            repositoryItemTrackBarMetal.Minimum = 0;
            repositoryItemTrackBarMetal.Maximum = 3;
            repositoryItemTrackBarMetal.TickFrequency = 1;
            //repositoryItemComboBoxMetal.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //repositoryItemComboBoxMetal.Items.AddRange(new string[] { "0", "1", "2", "3"});

            gridView2.Columns[COLUMN_YARN_METAL].ColumnEdit = repositoryItemTrackBarMetal;


            for (int i = 0; i < gridView2.Columns.Count; i++)
            {
                // 컬럼 타이틀 텍스트 중앙
                gridView2.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                // 컬럼 폭 설정 및 고정
                GridColumn column = gridView2.Columns[2];
                string columnName = column.FieldName;
                if (columnName == COLUMN_YARN_CHECK)
                {
                    column.Width = 50;
                }
                else if (columnName == COLUMN_YARN_IDX)
                {
                    column.Width = 50;
                }
                else if (columnName == COLUMN_YARN_NAME)
                {
                    column.Width = 200;
                }
                else if (columnName == COLUMN_YARN_WEIGHT)
                {
                    column.Width = 100;
                }
                else if (columnName == COLUMN_YARN_UNIT)
                {
                    column.Width = 100;
                }
                else if (columnName == COLUMN_YARN_TYPE)
                {
                    column.Width = 100;
                }
                else if (columnName == COLUMN_YARN_TEXTURED)
                {
                    column.Width = 100;
                }
                else if (columnName == COLUMN_YARN_METAL)
                {
                    column.Width = 150;
                }
                else if (columnName == COLUMN_YARN_IMAGE)
                {
                    column.Width = 200;
                }
                else if (columnName == COLUMN_YARN_REG_DT)
                {
                    column.Width = 200;
                }
            }
        }

        private void AddYarn(int idx)
        {
            Random r = new Random();
            Color randomColor = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));

            int newIdx = 0;

            if (wInfoTempList.Count > 0)
            {
                for (int i = 0; i < wInfoTempList.Count; i++)
                {
                    WInfoTemp temp = wInfoTempList[i];
                    newIdx = Math.Max(newIdx, temp.Idx);
                }

                newIdx += 1;
            }

            WInfoTemp wInfoTemp = new WInfoTemp();
            wInfoTemp.Idx = newIdx;
            wInfoTemp.Name = "경사#" + (newIdx + 1); // 1부터 시작
            wInfoTemp.IdxYarn = idx;
            wInfoTemp.SetColor(randomColor);

            wInfoTempList.Add(wInfoTemp);
        }
        private void simpleButton_Add_Yarn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView2.DataRowCount; i++)
            {
                string v = gridView2.GetRowCellValue(i, COLUMN_YARN_CHECK).ToString();
                string strIdx = gridView2.GetRowCellValue(i, COLUMN_YARN_IDX).ToString();
                //Trace.WriteLine("checkeD : " + v);
                if (v == "Y")
                {
                    int idxYarn = Util.ToInt(strIdx);
                    AddYarn(idxYarn);

                    isModified = true;
                }
                //do something  
            }

            initListView();
        }

        private void simpleButton_Color_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            int nCnt = 0;
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                string v = gridView1.GetRowCellValue(i, COLUMN_CHECK).ToString();
                string strIdx = gridView1.GetRowCellValue(i, COLUMN_IDX).ToString();

                list.Add(v);
                if (v == "Y")
                {
                    WInfoTemp warpInfo = GetWInfoTemp(i);
                    warpInfo.SetColor(colorOpt);
                    nCnt++;
                }
            }

            if(nCnt <= 0)
            {
                XtraMessageBox.Show("위사를 1개 이상 선택해주세요...");
                return;
            }

            initListView();

            for (int i = 0; i < list.Count; i++)
            {
                string v = list[i].ToString();
                gridView1.SetRowCellValue(i, COLUMN_CHECK, v);
            }

            isModified = true;
        }
    }

}
