using DevExpress.DataProcessing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraWaitForm;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WeavingGenerator
{
    public partial class DialogPatternList : XtraForm
    {
        MainForm mainForm;

        JObject objPattern;
        DataTable tb = new DataTable();

        string COLUMN_NAME = "조직명";

        ///////////////////////////////////////////////////////////////////////
        /// 패턴 목록 (json 파일에서 읽음)
        ///////////////////////////////////////////////////////////////////////
        List<Pattern> patternList = new List<Pattern>();

        int oldSelectedIdx = -1;
        int selectedIdx = -1;

        public DialogPatternList(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void DialogPatternList_Load(object sender, EventArgs e)
        {


            ///////////////////////////////////////////////////////////////////
            // 뷰 설정
            ///////////////////////////////////////////////////////////////////
            gridView1.OptionsView.ShowIndicator = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            //gridView1.OptionsSelection.EnableAppearanceFocusedRow = true;
            gridView1.OptionsSelection.MultiSelect = false;
            //gridView1.ClearSelection();

            //gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            // 정렬 금지
            gridView1.OptionsCustomization.AllowSort = false;
            // 컬럼 폭 자동 설정 
            gridView1.OptionsView.ColumnAutoWidth = true;

            gridView1.OptionsCustomization.AllowSort = false;

            ///////////////////////////////////////////////////////////////////
            // 시작 - 데이터
            ///////////////////////////////////////////////////////////////////

            tb.Columns.Add(COLUMN_NAME);


            ProjectData wData = mainForm.GetProjectData();
            if (wData != null)
            {
                selectedIdx = wData.Pattern.Idx;
                oldSelectedIdx = selectedIdx;
            }


            initListView();

            // 
            gridView1.ShowingEditor += GridView1_ShowingEditor;
            // 순서 주의
            gridView1.SelectionChanged += GridView1_SelectionChanged;
        }


        private void initListView()
        {
            // 원사 정보 갱신
            tb.Rows.Clear();

            //2025-01-22 soonchol
            //string json = Properties.Resources.PatternList;
            //objPattern = JObject.Parse(json);
            //JArray list = (JArray)objPattern["PatternList"];

            patternList = mainForm.patternList;

            for (int x = 0; x < patternList.Count - 1; x++)
            {
                //JObject objPattern = (JObject)list[x];

                //string strIdx = objPattern["Idx"].ToString();
                //int idx = Convert.ToInt32(strIdx);
                //string name = objPattern["Name"].ToString();
                //int Col = Convert.ToInt32(objPattern["XLength"].ToString());
                //int Row = Convert.ToInt32(objPattern["YLength"].ToString());
                //int[,] data = new int[Row, Col];
                //JArray dataObj = (JArray)objPattern["Data"];
                //for (int i = 0; i < dataObj.Count; i++)
                //{
                //    JArray temp = (JArray)dataObj[i];
                //    for (int j = 0; j < temp.Count; j++)
                //    {
                //        data[i, j] = Convert.ToInt32(temp[j].ToString());
                //    }
                //}
                //string strName = name + " (" + Col + " x " + Row + ")";

                Pattern patt = patternList[x];
                string strName = patt.Name + " (" + patt.XLength + " x " + patt.YLength + ")";
                tb.Rows.Add(strName);


                ///////////////////////////////////////////////////////////////
                //Pattern patt = new Pattern();
                //patt.Idx = idx;
                //patt.Name = name;
                //patt.Data = data;

                //patternList.Add(patt);
            }
            gridControl1.DataSource = tb;

            //2025-01-24 soonchol
            //load user defined pattern
            ProjectData wData = mainForm.GetProjectData();

            if (wData.Pattern.Idx == -1)
            {
                Pattern patt = patternList[patternList.Count - 1];
                patt.Name = wData.Pattern.Name;
                patt.XLength = wData.Pattern.XLength;
                patt.YLength = wData.Pattern.YLength;
                patt.Data = wData.Pattern.Data;
                tb.Rows.Add($"{patt.Name} ({patt.XLength} x {patt.YLength})");
            }
            else
            {
                Pattern patt = patternList[patternList.Count - 1];
                patt.Name = "USER";
                patt.XLength = 2;
                patt.YLength = 2;
                patt.Data = new int[2, 2] { { 1, 0 }, { 0, 1 } };
                tb.Rows.Add($"{patt.Name} ({patt.XLength} x {patt.YLength})");
            }

        }

        private void DialogPatternList_Shown(object sender, EventArgs e)
        {
            //2025-01-24 soonchol
            //set current pattern
            if (selectedIdx == -1)
            {
                int realIdx = gridView1.RowCount - 1;
                gridView1.SelectRow(realIdx);
                gridView1.FocusedRowHandle = realIdx;
                gridView1.MakeRowVisible(gridView1.FocusedRowHandle);
            }
            else
            {
                gridView1.SelectRow(selectedIdx);
                gridView1.FocusedRowHandle = selectedIdx;
                gridView1.MakeRowVisible(gridView1.FocusedRowHandle);
            }

            //gridView1.SelectRow(selectedIdx);
            //gridView1.FocusedRowHandle = selectedIdx;
            //gridView1.MakeRowVisible(gridView1.FocusedRowHandle);
        }

        ///////////////////////////////////////////////////////////////////////
        // 시작 - EVENT
        ///////////////////////////////////////////////////////////////////////
        private void GridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            return;
        }
        private void GridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //2025-01-24 soonchol
            //last one is user defined pattern
            if (gridView1.FocusedRowHandle == gridView1.DataRowCount - 1)
            {
                selectedIdx = -1;
            }
            else
            {
                selectedIdx = gridView1.FocusedRowHandle;
            }

            if (selectedIdx == -1)
            {
                //textEdit_Name.Enabled = true;
                //textEdit_SizeX.Enabled = true;
                //textEdit_SizeY.Enabled = true;
                spinEdit_SizeY.Enabled = true;
                spinEdit_SizeX.Enabled = true;
                //gridControl1.Enabled = false;
            }
            else
            {
                //textEdit_Name.Enabled = false;
                //textEdit_SizeX.Enabled = false;
                //textEdit_SizeY.Enabled = false;
                spinEdit_SizeY.Enabled = false;
                spinEdit_SizeX.Enabled = false;
                //gridControl1.Enabled = true;
            }

            //selectedIdx = gridView1.FocusedRowHandle;

            //2025-01-24 soonchol
            //if (oldSelectedIdx != selectedIdx)
            {
                //Trace.WriteLine("selectedIdx : " + selectedIdx);
                panel1.Refresh();
            }

            //2025-01-31 soonchol
            Pattern patt = GetPattern(selectedIdx);

            if (patt != null)
            {
                spinEdit_SizeX.EditValueChanged -= spinEdit_SizeX_EditValueChanged;
                spinEdit_SizeY.EditValueChanged -= spinEdit_SizeY_EditValueChanged;
                textEdit_Name.EditValue = $"{patt.Name}";
                spinEdit_SizeX.Text = patt.XLength.ToString();
                spinEdit_SizeY.Text = patt.YLength.ToString();
                spinEdit_SizeX.EditValueChanged += spinEdit_SizeX_EditValueChanged;
                spinEdit_SizeY.EditValueChanged += spinEdit_SizeY_EditValueChanged;
            }
        }
        ///////////////////////////////////////////////////////////////////////
        // 끝 - EVENT
        ///////////////////////////////////////////////////////////////////////


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Pattern patt = GetPattern(selectedIdx);

            if (patt == null)
            {
                return;
            }

            //2025-01-31 soonchol
            //textEdit_Name.Text = patt.Name;
            //textEdit_SizeX.Text = patt.XLength.ToString();
            //textEdit_SizeY.Text = patt.YLength.ToString();

            int Col = patt.XLength;
            int Row = patt.YLength;
            int[,] Data = patt.Data;
            int width = panel1.Width;
            int height = panel1.Height;

            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
            g.DrawRectangle(new Pen(Color.Gray, 1), new Rectangle(0, 0, width - 1, height - 1));

            int padding = 10;
            int w = width - padding;
            int h = height - padding;

            int wRect = w / Col;
            int hRect = h / Row;

            //2025-02-04 square! not rectangle
            int whRect = Math.Min(wRect, hRect);

            int x = padding / 2;
            int y = padding / 2;

            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(x, y, w, h));

            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    int temp = Data[i, j];
                    if (temp == 1)
                    {
                        //g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(x, y, wRect, hRect));
                        g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(x, y, whRect, whRect));
                    }

                    //g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x, y, wRect, hRect));
                    //x += wRect;
                    g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x, y, whRect, whRect));
                    x += whRect;
                }
                x = padding / 2;
                //y += hRect;
                y += whRect;
            }
            //g.Dispose();
            gridView1.Focus();
        }


        private Pattern GetPattern(int idx)
        {
            //2025-01-24 soonchol
            //-1 is user defined pattern
            //if (selectedIdx < 0 || selectedIdx > (patternList.Count - 1))
            //{
            //    return null;
            //}

            for (int i = 0; i < patternList.Count; i++)
            {
                Pattern pattern = patternList[i];
                if (pattern.Idx == idx)
                {
                    return pattern;
                }
            }
            return null;
        }

        private void simpleButton_Ok_Click(object sender, EventArgs e)
        {
            Pattern patt = GetPattern(selectedIdx);
            if (patt == null)
            {
                return;
            }

            //2025-01-22 soonchol
            //if (oldSelectedIdx != selectedIdx)
            //2025-01-24 soonchol
            if (selectedIdx == -1)
            {
                mainForm.SetPattern(-1);
            }
            else
            {
                mainForm.SetPattern(selectedIdx);
            }

            this.Close();
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Point PatternHit(Pattern patt, int x, int y)
        {
            int Col = patt.XLength;
            int Row = patt.YLength;
            int width = panel1.Width;
            int height = panel1.Height;

            //2025-02-04 square! not rectangle
            int padding = 10;
            int w = width - padding;
            int h = height - padding;

            if (x <= (padding / 2) || x >= (width - padding / 2) || y <= (padding / 2) || y >= (height - padding / 2))
            {
                return new Point(-1, -1);
            }

            int wRect = w / Col;
            int hRect = h / Row;
            int whRect = Math.Min(wRect, hRect);

            int ix = (x - padding / 2) / whRect;
            int iy = (y - padding / 2) / whRect;

            return new Point(ix, iy);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectedIdx != -1)
            {
                return;
            }

            Pattern patt = GetPattern(selectedIdx);

            if (patt == null)
            {
                return;
            }

            Point point = PatternHit(patt, e.X, e.Y);

            if (point.X < 0 || point.Y < 0)
            {
                return;
            }

            if(point.X >= patt.Data.GetLength(1))
            {
                return;
            }

            if (point.Y >= patt.Data.GetLength(0))
            {
                return;
            }

            if (patt.Data[point.Y, point.X] == 0)
            {
                patt.Data[point.Y, point.X] = 1;
            }
            else
            {
                patt.Data[point.Y, point.X] = 0;
            }

            panel1.Invalidate();
        }

        private void cbUser_CheckedChanged(object sender, EventArgs e)
        {
            //if (cbUser.Checked)
            //{
            //    textEdit_Name.Enabled = true;
            //    textEdit_SizeX.Enabled = true;
            //    textEdit_SizeY.Enabled = true;
            //    gridControl1.Enabled = false;
            //}
            //else
            //{
            //    textEdit_Name.Enabled = false;
            //    textEdit_SizeX.Enabled = false;
            //    textEdit_SizeY.Enabled = false;
            //    gridControl1.Enabled = true;
            //}
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            int realIdx = gridView1.RowCount - 1;
            gridView1.SelectRow(realIdx);
            gridView1.FocusedRowHandle = realIdx;
            gridView1.MakeRowVisible(gridView1.FocusedRowHandle);
        }

        private void spinEdit_SizeX_EditValueChanged(object sender, EventArgs e)
        {
            Pattern patt = GetPattern(selectedIdx);

            if (patt == null)
            {
                return;
            }

            int[,] old_data = new int[patt.YLength, patt.XLength];

            for (int i = 0; i < patt.YLength; i++)
            { 
                for (int j = 0; j < patt.XLength; j++)
                {
                    old_data[i, j] = (int)patt.Data[i, j];
                }
            }

            int newX = 2;
            int newY = 2;

            Int32.TryParse(spinEdit_SizeX.EditValue.ToString(), out newX);
            Int32.TryParse(spinEdit_SizeY.EditValue.ToString(), out newY);

            patt.Data = new int[newY, newX];

            for (int i = 0; i < patt.YLength; i++)
            {
                for (int j = 0; j < patt.XLength; j++)
                {
                    patt.Data[i, j] = 0;
                }
            }

            for (int i = 0; i < Math.Min(old_data.GetLength(0), patt.Data.GetLength(0)); i++)
            {
                for (int j = 0; j < Math.Min(old_data.GetLength(1), patt.Data.GetLength(1)); j++)
                {
                    patt.Data[i, j] = old_data[i, j];
                }
            }

            //if (patt.Data[point.Y, point.X] == 0)
            //{
            //    patt.Data[point.Y, point.X] = 1;
            //}
            //else
            //{
            //    patt.Data[point.Y, point.X] = 0;
            //}
            //

            //2025-01-31 soonchol
            spinEdit_SizeX.EditValueChanged -= spinEdit_SizeX_EditValueChanged;
            spinEdit_SizeY.EditValueChanged -= spinEdit_SizeY_EditValueChanged;
            textEdit_Name.EditValue = $"{patt.Name}";
            spinEdit_SizeX.Text = patt.XLength.ToString();
            spinEdit_SizeY.Text = patt.YLength.ToString();
            tb.Rows.RemoveAt(tb.Rows.Count - 1);
            string strName = patt.Name + " (" + patt.XLength + " x " + patt.YLength + ")";
            tb.Rows.Add(strName);
            gridView1.UnselectRow(tb.Rows.Count - 2);
            spinEdit_SizeX.EditValueChanged += spinEdit_SizeX_EditValueChanged;
            spinEdit_SizeY.EditValueChanged += spinEdit_SizeY_EditValueChanged;

            panel1.Invalidate();
        }

        private void spinEdit_SizeY_EditValueChanged(object sender, EventArgs e)
        {
            spinEdit_SizeX_EditValueChanged(sender, e);
        }
    }
}
