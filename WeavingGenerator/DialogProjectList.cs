using DevExpress.Utils.CodedUISupport;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static DevExpress.XtraEditors.Mask.MaskSettings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WeavingGenerator
{
    public delegate void DialogOpenProjectEventHandler(object sender, int idx);

    public partial class DialogProjectList : XtraForm
    {
        public DialogOpenProjectEventHandler dialogOpenProjectEventHandler = null;

        MainForm mainForm;
        //int idx = -1;
        //ProjectData wData;
        DataTable tb = new DataTable();

        string COLUMN_CHECK = "선택";
        string COLUMN_IDX = "번호";
        string COLUMN_NAME = "프로젝트명";
        string COLUMN_REG_DT = "등록일";

        int checkRowIndex = -1;

        // 자료 구조
        List<ProjectDataTemp> prjTempList;


        public DialogProjectList(MainForm main)
        {
            InitializeComponent();
            this.mainForm = main;
        }

        private void DialogWeavingList_Load(object sender, EventArgs e)
        {

            ///////////////////////////////////////////////////////////////////
            // 데이터 설정
            ///////////////////////////////////////////////////////////////////
            prjTempList = ToProjectDataTempList(mainForm.GetProjectDataList());


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
            tb.Columns.Add(COLUMN_REG_DT);

            initListView();

            RepositoryItemCheckEdit item = new RepositoryItemCheckEdit();
            item.ValueChecked = "Y";
            item.ValueUnchecked = "N";
            gridView1.Columns[COLUMN_CHECK].ColumnEdit = item;

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
                else if (columnName == COLUMN_IDX)
                {
                    column.Width = 50;
                }
                else if (columnName == COLUMN_NAME)
                {
                    column.Width = 200;
                }
                else if (columnName == COLUMN_REG_DT)
                {
                    column.Width = 200;
                }
            }
        }



        private void initListView()
        {
            // 원사 정보 갱신
            tb.Rows.Clear();

            for (int i = 0; i < prjTempList.Count; i++)
            {
                ProjectDataTemp info = prjTempList[i];

                int idx = info.Idx;
                string name = info.Name;
                string reg_dt = info.Reg_dt;

                string temp = Util.ToDateHuman(reg_dt);

                tb.Rows.Add(false, idx, name, temp);
            }
            gridControl1.DataSource = tb;
        }




        ///////////////////////////////////////////////////////////////////////
        // 시작 - EVENT
        ///////////////////////////////////////////////////////////////////////
        private void GridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            string col = gridView1.FocusedColumn.FieldName;
            if (col == COLUMN_IDX || col == COLUMN_REG_DT)
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
                ProjectDataTemp temp = GetProjectDataTemp(gridView1.FocusedRowHandle);
                temp.Name = e.Value.ToString();
            }
        }
        
        private void GridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName.Equals(COLUMN_CHECK))
            {
                //기존 check 를 uncheck 로 
                if(checkRowIndex > -1)
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
        
        private void simpleButton_Delete_Click(object sender, EventArgs e)
        {
            if (checkRowIndex < 0)
            {
                XtraMessageBox.Show("삭제 할 항목을 선택해주세요..");
                return;
            }
            if (XtraMessageBox.Show("선택항목을 삭제하시겠습니까?", "항목삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                prjTempList.RemoveAt(checkRowIndex);
                initListView();
            }
        }

        private void simpleButton_Open_Click(object sender, EventArgs e)
        {
            int[] selectedRowHandles = gridView1.GetSelectedRows();
            if (selectedRowHandles.Length <= 0)
            {
                XtraMessageBox.Show("프로젝트를 선택해주세요.");
                return;
            }

            string strIDX = gridView1.GetFocusedRowCellValue(COLUMN_IDX).ToString();
            if (string.IsNullOrEmpty(strIDX))
            {
                XtraMessageBox.Show("프로젝트를 선택해주세요.");
                return;
            }

            int selectedIdx = Convert.ToInt32(strIDX);
            // 이벤트
            if (dialogOpenProjectEventHandler != null)
            {
                dialogOpenProjectEventHandler(this, selectedIdx);
            }
            this.Close();
        }

        private void simpleButton_Add_Click(object sender, EventArgs e)
        {

            DialogNewProject dialog = new DialogNewProject(mainForm);
            dialog.dialogNewProjectEventHandler += new DialogNewProjectEventHandler(EventNewProjectData);
            dialog.ShowDialog();
        }
        private void EventNewProjectData(object sender, int newIdx)
        {
            ProjectData wData = mainForm.GetDAOProjectData(newIdx);
            if(wData == null)
            {
                return;
            }

            ProjectDataTemp temp = new ProjectDataTemp();
            temp.Idx = wData.Idx;
            temp.Name = wData.Name;
            temp.Reg_dt = wData.Reg_dt;
            temp.ProjectID = wData.ProjectID;
            temp.OptionMetal = wData.OptionMetal;

            temp.Pattern = wData.Pattern;
            temp.Warp = wData.Warp;
            temp.Weft = wData.Weft;
            temp.PhysicalProperty = wData.PhysicalProperty;

            prjTempList.Add(temp);

            initListView();
        }

        private void simpleButton_Ok_Click(object sender, EventArgs e)
        {
            ///////////////////////////////////////////////////////////////////
            /// 시작 - 데이터 수정
            ///////////////////////////////////////////////////////////////////
            //Trace.WriteLine("\n===========================================");
            List<ProjectData> list = new List<ProjectData>();

            for (int i = 0; i < prjTempList.Count; i++)
            {
                ProjectDataTemp temp = prjTempList[i];

                ProjectData data = new ProjectData();
                data.Idx = temp.Idx;
                data.Name = temp.Name;
                data.Reg_dt = temp.Reg_dt;
                data.ProjectID = temp.ProjectID;
                data.OptionMetal = temp.OptionMetal;

                data.Pattern = temp.Pattern;
                data.Warp = temp.Warp;
                data.Weft = temp.Weft;
                data.PhysicalProperty = temp.PhysicalProperty;
                list.Add(data);
            }

            // 삭제 데이터
            List<int> deleteList = new List<int>();
            List<ProjectData> dbList = mainForm.GetProjectDataList();
            if (dbList != null)
            {
                for (int i = 0; i < dbList.Count; i++)
                {
                    ProjectData dbData = dbList[i];
                    int dbIdx = dbData.Idx;

                    bool IsDel = true;
                    for (int j = 0; j < list.Count; j++)
                    {
                        ProjectData w = list[j];
                        if (w.Idx == dbIdx)
                        {
                            IsDel = false;
                            break;
                        }
                    }
                    if (IsDel == true)
                    {
                        //mainForm.RemoveProject(dbIdx);
                        deleteList.Add(dbIdx);
                    }
                }
            }
            for(int i =  0; i < deleteList.Count; i++)
            {
                int deleteIdx = deleteList[i];
                mainForm.RemoveProject(deleteIdx);
            }

            // 업데이트 데이터
            if (dbList != null)
            {
                for (int i = 0; i < dbList.Count; i++)
                {
                    ProjectData dbData = dbList[i];
                    int dbIdx = dbData.Idx;

                    for (int j = 0; j < list.Count; j++)
                    {
                        ProjectData w = list[j];
                        if (w.Idx == dbIdx)
                        {
                            dbData.Name = w.Name;
                            break;
                        }
                    }
                }
            }
            mainForm.UpdateProject();
            ///////////////////////////////////////////////////////////////////
            /// 끝 - 데이터 수정
            ///////////////////////////////////////////////////////////////////




            ///////////////////////////////////////////////////////////////////
            /// 시작 - 프로젝트 열기
            ///////////////////////////////////////////////////////////////////
            int[] selectedRowHandles = gridView1.GetSelectedRows();
            if (selectedRowHandles.Length <= 0)
            {
                XtraMessageBox.Show("프로젝트를 선택해주세요.");
                return;
            }

            string strIDX = gridView1.GetFocusedRowCellValue(COLUMN_IDX).ToString();
            if (string.IsNullOrEmpty(strIDX))
            {
                XtraMessageBox.Show("프로젝트를 선택해주세요.");
                return;
            }

            int selectedIdx = Convert.ToInt32(strIDX);
            // 이벤트
            if (dialogOpenProjectEventHandler != null)
            {
                dialogOpenProjectEventHandler(this, selectedIdx);
            }
            ///////////////////////////////////////////////////////////////////
            /// 끝 - 프로젝트 열기
            ///////////////////////////////////////////////////////////////////


            this.Close();
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        ///////////////////////////////////////////////////////////////////////
        // 끝 - EVENT
        ///////////////////////////////////////////////////////////////////////



        private ProjectDataTemp GetProjectDataTemp(int row)
        {
            return prjTempList[row];
        }

        private List<ProjectDataTemp> ToProjectDataTempList(List<ProjectData> prjList)
        {
            List<ProjectDataTemp> list = new List<ProjectDataTemp>();

            if (prjList == null)
            {
                return list;
            }

            for (int i = 0; i < prjList.Count; i++)
            {
                ProjectData wData = prjList[i];

                ProjectDataTemp temp = new ProjectDataTemp();
                temp.Idx = wData.Idx;
                temp.Name = wData.Name;
                temp.Reg_dt = wData.Reg_dt;
                temp.ProjectID = wData.ProjectID;
                temp.OptionMetal = wData.OptionMetal;

                temp.Pattern = wData.Pattern;
                temp.Warp = wData.Warp;
                temp.Weft = wData.Weft;
                temp.PhysicalProperty = wData.PhysicalProperty;

                list.Add(temp);
            }
            return list;
        }


    }



    public class ProjectDataTemp
    {
        // idx
        private int idx;
        // Item Name
        private string name;
        // Create Date
        private string reg_dt;
        // PROJECTID
        private string projectID;
        // 광택 :FD, SD, BR (없음, 약함, 강함)
        private string optionMetal;

        // 기본 정보
        //private BasicInfo _basicInfo;
        // 조직 정보
        private Pattern _patt;
        // 경사 배열
        private Warp _warp;
        // 위사 배열
        private Weft _weft;
        // 물설 관리
        private PhysicalProperty _physicalProperty;


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
        public string Reg_dt
        {
            get { return reg_dt; }
            set { reg_dt = value; }
        }
        public string ProjectID
        {
            get { return projectID; }
            set { projectID = value; }
        }
        public string OptionMetal
        {
            get { return optionMetal; }
            set { optionMetal = value; }
        }
        //public BasicInfo BasicInfo
        //{
        //    get { return _basicInfo; }
        //    set { _basicInfo = value; }
        //}
        public Weft Weft
        {
            get { return _weft; }
            set { _weft = value; }
        }

        public Warp Warp
        {
            get { return _warp; }
            set { _warp = value; }
        }

        public Pattern Pattern
        {
            get { return _patt; }
            set { _patt = value; }
        }

        public PhysicalProperty PhysicalProperty
        {
            get { return _physicalProperty; }
            set { _physicalProperty = value; }
        }

    }
}
