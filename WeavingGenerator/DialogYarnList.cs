using DevExpress.Utils.About;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WeavingGenerator
{
    public delegate void DialogOpenYarnEventHandler(object sender, int idx);

    public partial class DialogYarnList : DevExpress.XtraEditors.XtraForm
    {
        public DialogOpenYarnEventHandler dialogOpenYarnEventHandler = null;

        MainForm mainForm;
        //ProjectData wData;
        DataTable tb = new DataTable();
        //List<Yarn> listYarn;

        string COLUMN_CHECK = "선택";
        string COLUMN_IDX = "번호";
        string COLUMN_NAME = "원사명";
        string COLUMN_WEIGHT = "굵기";
        string COLUMN_UNIT = "단위";
        string COLUMN_TYPE = "종류";
        string COLUMN_TEXTURED = "가연여부";
        string COLUMN_METAL = "광택";
        string COLUMN_IMAGE = "이미지";
        string COLUMN_REG_DT = "등록일";

        int checkRowIndex = -1;

        RepositoryItemTextEdit rIMG = new RepositoryItemTextEdit();

        // 자료 구조
        List<YarnTemp> yarnTempList;

        int oldSelectedIdx = -1;
        int selectedIdx = -1;
        bool isModified = false;

        public DialogYarnList(MainForm main, int selectedIdx)
        {
            this.selectedIdx = selectedIdx;
            oldSelectedIdx = this.selectedIdx;
            InitializeComponent();
            this.mainForm = main;
        }

        private void DialogYarnList_Load(object sender, EventArgs e)
        {

            ///////////////////////////////////////////////////////////////////
            // 데이터 설정
            ///////////////////////////////////////////////////////////////////
            yarnTempList = ToYarnTempList(Controllers.Instance.ProjectController.ListDAOYarn());


            ///////////////////////////////////////////////////////////////////
            // 뷰 설정
            ///////////////////////////////////////////////////////////////////
            gridView1.OptionsView.ShowIndicator = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            //gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
            gridView1.OptionsSelection.MultiSelect = false;
            //gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            // 정렬 금지
            gridView1.OptionsCustomization.AllowSort = false;
            // 컬럼 폭 자동 설정 
            gridView1.OptionsView.ColumnAutoWidth = false;

            gridView1.OptionsCustomization.AllowSort = false;
            // 


            gridView1.ShowingEditor += GridView1_ShowingEditor;

            gridView1.CellValueChanged += GridView1_CellValueChanged;

            gridView1.CellValueChanging += GridView1_CellValueChanging;

            ///////////////////////////////////////////////////////////////////
            // 시작 - 데이터
            ///////////////////////////////////////////////////////////////////

            tb.Columns.Add(COLUMN_CHECK);
            tb.Columns.Add(COLUMN_IDX);
            tb.Columns.Add(COLUMN_NAME);
            tb.Columns.Add(COLUMN_WEIGHT, typeof(double));
            tb.Columns.Add(COLUMN_UNIT);
            tb.Columns.Add(COLUMN_TYPE);
            tb.Columns.Add(COLUMN_TEXTURED);
            tb.Columns.Add(COLUMN_METAL);
            tb.Columns.Add(COLUMN_IMAGE, typeof(Image));
            tb.Columns.Add(COLUMN_REG_DT);
            ///


            initListView();


            RepositoryItemCheckEdit item = new RepositoryItemCheckEdit();
            item.ValueChecked = "Y";
            item.ValueUnchecked = "N";
            gridView1.Columns[COLUMN_CHECK].ColumnEdit = item;

            ///////////////////////////////////////////////////////////////////
            // 시작 - cell 에 컨트롤 적용
            ///////////////////////////////////////////////////////////////////

            RepositoryItemComboBox repositoryItemComboBoxUnit = new RepositoryItemComboBox();
            repositoryItemComboBoxUnit.SelectedValueChanged += RepositoryItemComboBoxUnit_SelectedValueChanged;
            repositoryItemComboBoxUnit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            repositoryItemComboBoxUnit.Items.AddRange(new string[] { "Denier", "Dtex", "Ne", "Nm", "Lea" });
            gridView1.Columns[COLUMN_UNIT].ColumnEdit = repositoryItemComboBoxUnit;


            RepositoryItemComboBox repositoryItemComboBoxType = new RepositoryItemComboBox();
            repositoryItemComboBoxType.SelectedValueChanged += RepositoryItemComboBoxType_SelectedValueChanged;
            repositoryItemComboBoxType.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            repositoryItemComboBoxType.Items.AddRange(new string[] { "장섬유", "단섬유" });
            gridView1.Columns[COLUMN_TYPE].ColumnEdit = repositoryItemComboBoxType;


            RepositoryItemComboBox repositoryItemComboBoxTextured = new RepositoryItemComboBox();
            repositoryItemComboBoxTextured.SelectedValueChanged += RepositoryItemComboBoxTextured_SelectedValueChanged;
            repositoryItemComboBoxTextured.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            repositoryItemComboBoxTextured.Items.AddRange(new string[] { "Filament", "DTY", "Hi-multi", "Twist", "ATY", "ITY", "ITY_S", "ITY_Z", "NEP", "2Ply", "NEP", "Slub" });
            gridView1.Columns[COLUMN_TEXTURED].ColumnEdit = repositoryItemComboBoxTextured;

            //RepositoryItemComboBox repositoryItemComboBoxMetal = new RepositoryItemComboBox();
            //repositoryItemComboBoxMetal.SelectedValueChanged += RepositoryItemComboBoxMetal_SelectedValueChanged;
            //repositoryItemComboBoxMetal.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //repositoryItemComboBoxMetal.Items.AddRange(new string[] { "0", "1", "2", "3"});
            //gridView1.Columns[COLUMN_METAL].ColumnEdit = repositoryItemComboBoxMetal;

            RepositoryItemTrackBar repositoryItemTrackBarMetal = new RepositoryItemTrackBar();
            repositoryItemTrackBarMetal.ValueChanged += RepositoryItemTrackBarMetal_ValueChanged;
            repositoryItemTrackBarMetal.Minimum = 0;
            repositoryItemTrackBarMetal.Maximum = 3;
            repositoryItemTrackBarMetal.TickFrequency = 1;
            //repositoryItemComboBoxMetal.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //repositoryItemComboBoxMetal.Items.AddRange(new string[] { "0", "1", "2", "3"});

            gridView1.Columns[COLUMN_METAL].ColumnEdit = repositoryItemTrackBarMetal;


            gridView1.Columns[COLUMN_IMAGE].ColumnEdit = new RepositoryItemPictureEdit();

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
                    column.Width = 50;
                }
                else if (columnName == "원사명")
                {
                    column.Width = 200;
                }
                else if (columnName == "굵기")
                {
                    column.Width = 100;
                }
                else if (columnName == "단위")
                {
                    column.Width = 100;
                }
                else if (columnName == "종류")
                {
                    column.Width = 100;
                }
                else if (columnName == "가연여부")
                {
                    column.Width = 100;
                }
                else if (columnName == "광택")
                {
                    column.Width = 150;
                }
                else if (columnName == "이미지")
                {
                    column.Width = 200;
                }
                else if (columnName == "등록일")
                {
                    column.Width = 200;
                }
            }

        }


        private void initListView()
        {
            // 원사 정보 갱신
            tb.Rows.Clear();

            for (int i = 0; i < yarnTempList.Count; i++)
            {
                YarnTemp info = yarnTempList[i];

                int idx = info.Idx;
                string name = info.Name;
                string weight = info.Weight;
                string unit = info.Unit;
                string type = info.Type;
                string textured = info.Textured;
                string metal = info.Metal;
                string image = info.Image;
                string reg_dt = info.Reg_dt;

                string temp = Util.ToDateHuman(reg_dt);

                Image img = Util.GetYarnResource(textured);

                tb.Rows.Add(false, idx, name, weight, unit, type, textured, metal, img, temp);
            }
            gridControl1.DataSource = tb;
        }




        ///////////////////////////////////////////////////////////////////////
        // 시작 - EVENT
        ///////////////////////////////////////////////////////////////////////
        private void DialogYarnList_Shown(object sender, EventArgs e)
        {
            if(selectedIdx != -1)
            {
                int row = GetRowIdxByIdx(selectedIdx);
                if(row < 0)
                {
                    return;
                }
                gridView1.SelectRow(row);
                gridView1.FocusedRowHandle = row;
                gridView1.MakeRowVisible(gridView1.FocusedRowHandle);
            }
            //gridView1.Focus();
        }
        private void GridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            string col = gridView1.FocusedColumn.FieldName;
            if (col == "번호" || col == "등록일")
            {
                e.Cancel = true;
                return;
            }
        }

        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            Trace.WriteLine("gridView1.FocusedRowHandle : " + gridView1.FocusedRowHandle);
            if (e.Column.FieldName.Equals(COLUMN_NAME))
            {
                YarnTemp yarnTemp = GetYarnTemp(gridView1.FocusedRowHandle);
                yarnTemp.Name = e.Value.ToString();
            }
            else if (e.Column.FieldName.Equals(COLUMN_WEIGHT))
            {
                YarnTemp yarnTemp = GetYarnTemp(gridView1.FocusedRowHandle);
                yarnTemp.Weight = e.Value.ToString();
                isModified = true;
            }
        }

        private void GridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
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
        }
        private void RepositoryItemComboBoxTextured_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit editor = sender as ComboBoxEdit;
            if (editor != null)
            {
                int selected = gridView1.FocusedRowHandle;
                YarnTemp yarnTemp = GetYarnTemp(gridView1.FocusedRowHandle);
                string v = editor.SelectedItem.ToString();
                yarnTemp.Textured = v;
                initListView();
                gridView1.FocusedRowHandle = selected;
                isModified = true;
            }
        }
        private void RepositoryItemTrackBarMetal_ValueChanged(object sender, EventArgs e)
        {
            TrackBarControl tbc = sender as TrackBarControl;
            if (tbc != null)
            {
                int selected = gridView1.FocusedRowHandle;
                YarnTemp yarnTemp = GetYarnTemp(gridView1.FocusedRowHandle);
                string v = tbc.Value.ToString();
                yarnTemp.Metal = v;
            }
        }
        private void RepositoryItemComboBoxType_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit editor = sender as ComboBoxEdit;
            if (editor != null)
            {
                YarnTemp yarnTemp = GetYarnTemp(gridView1.FocusedRowHandle);
                string v = editor.SelectedItem.ToString();
                yarnTemp.Type = v;
                isModified = true;
                //initListView();
            }
        }

        private void RepositoryItemComboBoxUnit_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit editor = sender as ComboBoxEdit;
            if (editor != null)
            {
                YarnTemp yarnTemp = GetYarnTemp(gridView1.FocusedRowHandle);
                string v = editor.SelectedItem.ToString();
                if (string.Equals(v, "Denier") || string.Equals(v, "Dtex"))
                {
                    yarnTemp.Unit = v.ToString();
                    yarnTemp.Type = "장섬유";
                    isModified = true;
                    //initListView();
                }
                else if (string.Equals(v, "Ne") || string.Equals(v, "Nm") || string.Equals(v, "Lea"))
                {
                    //단섬유
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gridView1.Columns[COLUMN_TYPE], "단섬유");

                    yarnTemp.Unit = v.ToString();
                    yarnTemp.Type = "단섬유";
                    isModified = true;
                    //initListView();
                }
            }
        }


        private void btn_Add_Click(object sender, EventArgs e)
        {
            DialogNewYarn dialog = new DialogNewYarn();
            dialog.dialogNewYarnEventHandler += new DialogNewYarnEventHandler(EventNewYarn);
            dialog.ShowDialog();
        }

        private void EventNewYarn(object sender, int newIdx)
        {
            yarnTempList = ToYarnTempList(Controllers.Instance.ProjectController.ListDAOYarn());
            initListView();
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            int[] selectedRowHandles = gridView1.GetSelectedRows();
            if (selectedRowHandles.Length <= 0)
            {
                XtraMessageBox.Show("원사를 선택해주세요.");
                return;
            }

            string strIDX = gridView1.GetFocusedRowCellValue(COLUMN_IDX).ToString();
            if (string.IsNullOrEmpty(strIDX))
            {
                XtraMessageBox.Show("원사를 선택해주세요.");
                return;
            }

            int selectedIdx = Convert.ToInt32(strIDX);
            // 이벤트
            if (dialogOpenYarnEventHandler != null)
            {
                dialogOpenYarnEventHandler(this, selectedIdx);
            }
            this.Close();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (checkRowIndex < 0)
            {
                XtraMessageBox.Show("삭제 할 항목을 선택해주세요..");
                return;
            }
            if (XtraMessageBox.Show("선택항목을 삭제하시겠습니까?", "항목삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                yarnTempList.RemoveAt(checkRowIndex);
                initListView();
                isModified = true;
            }
        }


        private void btn_OK_Click(object sender, EventArgs e)
        {
            ///////////////////////////////////////////////////////////////////
            /// 시작 - 데이터 수정
            ///////////////////////////////////////////////////////////////////
            List<Yarn> yarnList = new List<Yarn>();

            for (int i = 0; i < yarnTempList.Count; i++)
            {
                YarnTemp temp = yarnTempList[i];

                Yarn yarn = new Yarn();
                yarn.Idx = temp.Idx;
                yarn.Name = temp.Name;
                yarn.Weight = temp.Weight;
                yarn.Unit = temp.Unit;
                yarn.Type = temp.Type;
                yarn.Textured = temp.Textured;
                yarn.Metal = temp.Metal;
                yarn.Image = temp.Image;
                yarn.Reg_dt = temp.Reg_dt;
                yarnList.Add(yarn);
            }

            // 업데이트
            List<Yarn> dbList = Controllers.Instance.ProjectController.ListDAOYarn();
            if (dbList != null)
            {
                for (int i = 0; i < dbList.Count; i++)
                {
                    Yarn dbYarn = dbList[i];
                    int dbIdx = dbYarn.Idx;

                    bool IsDel = true;
                    for (int j = 0; j < yarnList.Count; j++)
                    {
                        Yarn y = yarnList[j];
                        if (y.Idx == dbIdx)
                        {
                            IsDel = false;
                            break;
                        }
                    }
                    if (IsDel == true)
                    {
                        Controllers.Instance.ProjectController.RemoveDAOYarn(dbIdx);
                    }
                }
            }
            for (int i = 0; i < yarnList.Count; i++)
            {
                Yarn y = yarnList[i];
                Controllers.Instance.ProjectController.UpdateDAOYarn(y);
            }
            ///////////////////////////////////////////////////////////////////
            /// 끝 - 데이터 수정
            ///////////////////////////////////////////////////////////////////







            ///////////////////////////////////////////////////////////////////
            /// 시작 - 열기
            ///////////////////////////////////////////////////////////////////
            int[] selectedRowHandles = gridView1.GetSelectedRows();
            if (selectedRowHandles.Length <= 0)
            {
                XtraMessageBox.Show("원사를 선택해주세요.");
                return;
            }

            string strIDX = gridView1.GetFocusedRowCellValue(COLUMN_IDX).ToString();
            if (string.IsNullOrEmpty(strIDX))
            {
                XtraMessageBox.Show("원사를 선택해주세요.");
                return;
            }

            int selectedIdx = Convert.ToInt32(strIDX);
            // 이벤트
            if (dialogOpenYarnEventHandler != null)
            {
                dialogOpenYarnEventHandler(this, selectedIdx);
            }
            ///////////////////////////////////////////////////////////////////
            /// 끝 - 열기
            ///////////////////////////////////////////////////////////////////

            // 업데이트
            if (isModified == true)
            {
                mainForm.UpdateProjectData();
            }
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }










        private YarnTemp GetYarnTemp(int row)
        {
            return yarnTempList[row];
        }
        private int GetRowIdxByIdx(int idx)
        {
            for (int i = 0; i < yarnTempList.Count; i++)
            {
                YarnTemp yarn = yarnTempList[i];
                if(idx == yarn.Idx)
                {
                    return i;
                }
                
            }
            return -1;
        }

        private List<YarnTemp> ToYarnTempList(List<Yarn> yarnList)
        {
            List<YarnTemp> list = new List<YarnTemp>();

            if (yarnList == null)
            {
                return list;
            }

            for (int i = 0; i < yarnList.Count; i++)
            {
                Yarn yarn = yarnList[i];
                int idx = yarn.Idx;
                string name = yarn.Name;
                string weight = yarn.Weight;
                string unit = yarn.Unit;
                string type = yarn.Type;
                string textured = yarn.Textured;
                string metal = yarn.Metal;
                string image = yarn.Image;
                string reg_dt = yarn.Reg_dt;

                YarnTemp temp = new YarnTemp();
                temp.Idx = idx;
                temp.Name = name;
                temp.Weight = weight;
                temp.Unit = unit;
                temp.Type = type;
                temp.Textured = textured;
                temp.Metal = metal;
                temp.Image = image;
                temp.Reg_dt = reg_dt;
                list.Add(temp);
            }
            return list;
        }

    }


    public class YarnTemp
    {
        private int idx = 0;
        private string name;
        private string weight = "";
        private string unit = "";
        private string type = "";
        private string textured = "";
        private string metal = "";
        private string image = "";
        private string reg_dt = "";

        public int Idx
        {
            get { return idx; }
            set { idx = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Textured
        {
            get { return textured; }
            set { textured = value; }
        }
        public string Metal
        {
            get { return metal; }
            set { metal = value; }
        }
        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        public string Reg_dt
        {
            get { return reg_dt; }
            set { reg_dt = value; }
        }

    }
}
