using CefSharp;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.DataAccess.Json;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.Utils.Helpers;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ColorPick.Picker;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using Jm.DBConn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace WeavingGenerator
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public static String Default_DyeColor = "#255,255,255,255";

        ///////////////////////////////////////////////////////////////////////
        /// DB URL
        ///////////////////////////////////////////////////////////////////////
        string connStr;
        public DBConn db;
        ///////////////////////////////////////////////////////////////////////
        /// Save Process
        ///////////////////////////////////////////////////////////////////////
        DialogConfirmSave dialogSave;
        bool IsModified = false;

        ///////////////////////////////////////////////////////////////////////
        /// 패턴 목록 (json 파일에서 읽음)
        ///////////////////////////////////////////////////////////////////////
        //2025-01-22 soonchol (protected -> public)
        public List<Pattern> patternList = new List<Pattern>();

        ///////////////////////////////////////////////////////////////////////
        /// 직물 정보 목록
        ///////////////////////////////////////////////////////////////////////
        List<ProjectData> prjList = new List<ProjectData>();


        ///////////////////////////////////////////////////////////////////////
        /// WeavingData
        ///////////////////////////////////////////////////////////////////////
        int SELECTED_IDX = -1;
        //ProjectData SELECTED_PRJ = null;


        public static string WWWROOT_PATH   = System.Windows.Forms.Application.StartupPath + "\\wwwroot";
        public static string CLO_PATH       = System.Windows.Forms.Application.StartupPath + "\\clo";

        //public static string UPLOAD_URL = "http://127.0.0.1:8080/upload";
        //public static string VIEWER_URL = "http://127.0.0.1:8080/viewer";

        public static string UPLOAD_URL = "https://211.38.52.16/upload";
        public static string VIEWER_URL = "https://211.38.52.16/viewer";
        string APPID = ""; // 앱설치 시 생성되어 DB에 저장

        WeaveViewer weave2DViewer = null;

        //화면 깜빡임 방지
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        public MainForm()
        {
            //WindowsFormsSettings.DefaultFont = new System.Drawing.Font("맑은고딕", 10);
            this.Font = new System.Drawing.Font("맑은고딕", 10);
            InitializeComponent();
            ///////////////////////////////////////////////////////////////////
            // Exit Event
            ///////////////////////////////////////////////////////////////////
            this.FormClosing += MainForm_FormClosing;

            //DB 커넥션 세팅
            db = DBConn.Instance;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            ShowProgressForm();

            //string appPath = Application.StartupPath;
            //Trace.WriteLine("Application Path : " + appPath);

            //weave2DViewer = new WeaveViewer(this, pictureBox1);
            weave2DViewer = new WeaveViewer(this);
            panel1.Controls.Add(weave2DViewer);

            weave2DViewer.Location = new System.Drawing.Point(0, 0);
            weave2DViewer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            weave2DViewer.Name = "pictureBox1";
            //2025-01-20 soonchol
            //weave2DViewer.Size = new System.Drawing.Size(250, 250);
            weave2DViewer.Size = new System.Drawing.Size(2 * WeaveViewer.PPI, 2 * WeaveViewer.PPI);
            //weave2DViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            //weave2DViewer.AutoSize = true;
            weave2DViewer.TabIndex = 0;
            weave2DViewer.TabStop = false;
            weave2DViewer.Dock = DockStyle.None;

            ///////////////////////////////////////////////////////////////////
            /// 3D 뷰어 생성(ChromiumWebBrowser)
            ///////////////////////////////////////////////////////////////////
            InitChromiumWebBrowser();

            ///////////////////////////////////////////////////////////////////
            /// Database 생성
            ///////////////////////////////////////////////////////////////////
            InitDAO();

            ///////////////////////////////////////////////////////////////////
            /// Pattern List 로딩 (기본/변형 조직 30종)
            ///////////////////////////////////////////////////////////////////
            InitPatternList();


            ///////////////////////////////////////////////////////////////////
            /// 프로젝트 목록 조회
            ///////////////////////////////////////////////////////////////////
            prjList = ListDAOProjectData();

            ///////////////////////////////////////////////////////////////////
            /// WeavingData List 로딩 후 프로젝트 리그트업
            ///////////////////////////////////////////////////////////////////
            InitProjectView();

            ////////////////////////////////////////////////////////////////////
            /// 프로퍼티 컨트롤 생성
            ////////////////////////////////////////////////////////////////////
            InitPropertyView();

            ///////////////////////////////////////////////////////////////////
            /// WeavingData 설정 (임시)
            ///////////////////////////////////////////////////////////////////
            //SetWeavingData(json);

            ///////////////////////////////////////////////////////////////////
            /// 킥오프에 사용 (임시)
            ///////////////////////////////////////////////////////////////////
            if (prjList.Count > 0)
            {
                ProjectData obj = prjList[0];

                tempRunIdx = obj.Idx;
                tempRunWData = obj;

                ThreadInitProject();
            }
            else
            {
                //CloseProgressForm();

                //DialogNewProject dialog = new DialogNewProject(this);
                //dialog.dialogNewProjectEventHandler += new DialogNewProjectEventHandler(EventNewProject);
                //dialog.ShowDialog();
            }


            splitContainer1.SplitterPosition = splitContainer1.Width / 2;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (prjList.Count <= 0)
            {
                //DialogNewProject dialog = new DialogNewProject(this);
                //dialog.dialogNewProjectEventHandler += new DialogNewProjectEventHandler(EventNewProject);
                //dialog.ShowDialog();
            }
        }

        // 프로젝트 버튼
        public class ProjectButton : CheckButton
        {
            public int Idx { get; set; }
        }
        LayoutControlGroup lcgProject;

        //기본정보
        TextEdit textEdit_Name;
        TextEdit textEdit_BasicInfoRegDt;
        //ComboBoxEdit comboBoxEdit_OptionMetal;
        ComboBoxEdit comboBoxEdit_Scale;
        CheckEdit checkEdit_YarnImage;
        //2025-02-05 soonchol
        ColorPickEdit colorEdit_DyeColor;
        CheckEdit checkEdit_YarnDyed;
        //경사
        SimpleButton simpleButton_Warp;
        SimpleButton simpleButton_WarpArray;
        SpinEdit spinEdit_WarpDensity;
        //위사
        SimpleButton simpleButton_Weft;
        SimpleButton simpleButton_WeftArray;
        SpinEdit spinEdit_WeftDensity;
        //조직
        TextEdit textEdit_Pattern;
        SimpleButton simpleButton_Pattern;
        //물성
        /*
        TextEdit textEdit_BendingWeft;
        TextEdit textEdit_BendingWarp;
        TextEdit textEdit_InternalDamping;
        TextEdit textEdit_Friction;
        TextEdit textEdit_Density;
        TextEdit textEdit_StretchWeft;
        TextEdit textEdit_StretchWarp;
        TextEdit textEdit_BucklingStiffnessWeft;
        TextEdit textEdit_BucklingStiffnessWarp;
        */
        NumericUpDown textEdit_BendingWeft;
        NumericUpDown textEdit_BendingWarp;
        NumericUpDown textEdit_InternalDamping;
        NumericUpDown textEdit_Friction;
        NumericUpDown textEdit_Density;
        NumericUpDown textEdit_StretchWeft;
        NumericUpDown textEdit_StretchWarp;
        NumericUpDown textEdit_BucklingStiffnessWeft;
        NumericUpDown textEdit_BucklingStiffnessWarp;

        TrackBarControl trackBar_BendingWarp;
        TrackBarControl trackBar_BendingWeft;
        TrackBarControl trackBar_InternalDamping;
        TrackBarControl trackBar_Friction;
        TrackBarControl trackBar_Density;
        TrackBarControl trackBar_StretchWeft;
        TrackBarControl trackBar_StretchWarp;
        TrackBarControl trackBar_BucklingStiffnessWeft;
        TrackBarControl trackBar_BucklingStiffnessWarp;

        //SimpleButton simpleButton_Json;

        //DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;

        private void ShowProgressForm()
        {
            //splashScreenManager1.ShowWaitForm();
            //splashScreenManager1.SetWaitFormCaption("Please wait");
            //SplashScreenManager.CloseForm();
            if (SplashScreenManager.Default == null || SplashScreenManager.Default.IsSplashFormVisible == false)
            {
                SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            }
        }
        private void CloseProgressForm()
        {
            SplashScreenManager.CloseForm(false);
        }

        private void InitProjectView()
        {
            layoutControl_Project.BeginUpdate();

            lcgProject = new LayoutControlGroup();
            lcgProject.Text = "직물 프로젝트";
            lcgProject.GroupStyle = DevExpress.Utils.GroupStyle.Title;
            //lcgProject.AppearanceGroup.Font = WindowsFormsSettings.DefaultFont;

            for (int i = 0; i < prjList.Count; i++)
            {
                ProjectData wData = prjList[i];

                LayoutControlItem item = lcgProject.AddItem();
                item.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 10, 3);
                item.TextVisible = false;

                ProjectButton btn = new ProjectButton();

                btn.Idx = wData.Idx;
                btn.Name = wData.Name;
                btn.Text = wData.Name;
                btn.Appearance.Options.UseBackColor = true;
                btn.GroupIndex = 0;
                btn.Click += btnProjectClick;

                item.Control = btn;
            }

            layoutControl_Project.Root.Add(lcgProject);

            layoutControl_Project.EndUpdate();
        }

        private void UpdateProjectView()
        {

            layoutControl_Project.BeginUpdate();

            ReadOnlyItemCollection items = layoutControl_Project.Items;
            foreach (BaseLayoutItem item in items)
            {
                LayoutControlItem lci = item as LayoutControlItem;
                if (lci != null)
                {
                    lci.Parent.Remove(lci);
                    lci.Control.Dispose();

                }
            }

            for (int i = 0; i < prjList.Count; i++)
            {
                ProjectData wData = prjList[i];

                LayoutControlItem item = lcgProject.AddItem();
                item.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 10, 3);
                item.TextVisible = false;

                ProjectButton btn = new ProjectButton();

                btn.Idx = wData.Idx;
                btn.Name = wData.Name;
                btn.Text = wData.Name;
                btn.Appearance.Options.UseBackColor = true;
                btn.GroupIndex = 0;
                btn.Click += btnProjectClick;

                item.Control = btn;
            }


            layoutControl_Project.EndUpdate();
        }
        private void SetSelectedProjectButton(int projectIdx)
        {
            foreach (var item in lcgProject.Items)
            {
                if (item is LayoutControlItem)
                {
                    LayoutControlItem lcItem = (LayoutControlItem)item;
                    if (lcItem.Control == null)
                    {
                        continue;
                    }
                    if (lcItem.Control.GetType() == typeof(ProjectButton))
                    {
                        ProjectButton btn = (ProjectButton)lcItem.Control;
                        //기본 색상으로 초기화
                        btn.Appearance.BackColor = SystemColors.Control;
                        int idx = btn.Idx;
                        if (idx == projectIdx)
                        {
                            btn.Appearance.BackColor = Color.White;
                            btn.Checked = true;
                        }
                    }
                }
            }
        }
        private void RemoveProjectButton(int projectIdx)
        {
            layoutControl_Project.BeginUpdate();

            foreach (var item in lcgProject.Items)
            {
                if (item is LayoutControlItem)
                {
                    LayoutControlItem lcItem = (LayoutControlItem)item;
                    ProjectButton btn = (ProjectButton)lcItem.Control;
                    int idx = btn.Idx;
                    if (idx == projectIdx)
                    {
                        btn.Dispose();
                        lcItem.Dispose();
                        break;
                    }

                }
            }

            layoutControl_Project.EndUpdate();
        }
        private void AddProjectButton(int projectIdx, string name)
        {
            layoutControl_Project.BeginUpdate();

            LayoutControlItem item = lcgProject.AddItem();
            item.TextVisible = false;

            ProjectButton btn = new ProjectButton();
            btn.Idx = projectIdx;
            btn.Text = name;
            btn.GroupIndex = 0;
            btn.Click += btnProjectClick;
            btn.Appearance.Options.UseBackColor = true;
            btn.Font = DefaultFont;
            item.Control = btn;

            layoutControl_Project.EndUpdate();
        }

        private void UpdateProjectButton(int projectIdx, string text)
        {
            layoutControl_Project.BeginUpdate();

            foreach (var item in lcgProject.Items)
            {
                if (item is LayoutControlItem)
                {
                    LayoutControlItem lcItem = (LayoutControlItem)item;
                    ProjectButton btn = (ProjectButton)lcItem.Control;
                    int idx = btn.Idx;
                    if (idx == projectIdx)
                    {
                        btn.Text = text;
                        break;
                    }

                }
            }
            layoutControl_Project.EndUpdate();
        }
        int PADDING_TOP_ITEM = 6;
        int PADDING_TOP_GROUP = 30;
        public DevExpress.XtraLayout.Utils.Padding CreatePadding(int top)
        {
            return CreatePadding(top, 3);
        }
        public DevExpress.XtraLayout.Utils.Padding CreatePadding(int top, int bottom)
        {
            return new DevExpress.XtraLayout.Utils.Padding(3, 3, top, bottom);
        }
        private void InitPropertyView()
        {
            LayoutControlGroup group;
            LayoutControlItem item;
            LabelControl label;

            EmptySpaceItem emptyItem;
            //Font font = new Font();
            
            layoutControl_Property.BeginUpdate();
            //layoutControl_Property.LookAndFeel.UseDefaultLookAndFeel = false;
            ///////////////////////////////////////////////////////////////////

            group = new LayoutControlGroup();
            group.Text = "기본정보";
            group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
            group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
            //skin 을 바꾸면 색상 적용 불가
            //group.AppearanceGroup.BorderColor = Color.White;
            //group.AppearanceGroup.BorderColor = Color.FromArgb(((int)(((byte)(1)))),((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            group.AppearanceGroup.Options.UseBorderColor = true;
            group.CustomDraw += Group_CustomDraw;
            //group.GroupStyle = GroupStyle.Card;
            layoutControl_Property.Root.Add(group);

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "프로젝트 명";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            textEdit_Name = new TextEdit();
            textEdit_Name.TextChanged += textEdit_Name_TextChanged;
            item.Control = textEdit_Name;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "생성일자";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            textEdit_BasicInfoRegDt = new TextEdit();
            textEdit_BasicInfoRegDt.ReadOnly = true;
            item.Control = textEdit_BasicInfoRegDt;
            item.TextVisible = false;

            /*
            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "광택";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            comboBoxEdit_OptionMetal = new ComboBoxEdit();
            comboBoxEdit_OptionMetal.Properties.Items.AddRange(new object[] {
                "FD",
                "SD",
                "BR"
            });
            comboBoxEdit_OptionMetal.SelectedIndexChanged += ComboBoxEdit_OptionMetal_SelectedIndexChanged;
            comboBoxEdit_OptionMetal.SelectedIndex = 0;
            comboBoxEdit_OptionMetal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            item.Control = comboBoxEdit_OptionMetal;
            item.TextVisible = false;
            */



            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "확대";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            comboBoxEdit_Scale = new ComboBoxEdit();
            comboBoxEdit_Scale.Properties.Items.AddRange(new object[] {
                "x01 (기본)",
                "x02",
                "x03",
                "x04",
                "x05",
                "x06",
                "x07",
                "x08"
            });
            comboBoxEdit_Scale.SelectedIndexChanged += ComboBoxEdit_Scale_SelectedIndexChanged;
            comboBoxEdit_Scale.SelectedIndex = 0;
            comboBoxEdit_Scale.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            item.Control = comboBoxEdit_Scale;
            item.TextVisible = false;




            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            checkEdit_YarnImage = new CheckEdit();
            checkEdit_YarnImage.Text = "원사이미지 적용";
            checkEdit_YarnImage.CheckedChanged += checkYarnImageCheckedChanged;
            item.Control = checkEdit_YarnImage;
            item.TextVisible = false;

            /*
            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM, PADDING_TOP_GROUP);
            simpleButton_Json = new SimpleButton();
            simpleButton_Json.Text = "JSON DATA";
            simpleButton_Json.Click += btnJsonClick;
            item.Control = simpleButton_Json;
            item.TextVisible = false;
            */
            emptyItem = new EmptySpaceItem();
            emptyItem.AllowHotTrack = false;
            emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
            emptyItem.TextSize = new System.Drawing.Size(0, 0);
            layoutControl_Property.AddItem(emptyItem);

            //2025-02-05 soonchol
            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            colorEdit_DyeColor = new ColorPickEdit();
            colorEdit_DyeColor.Text = "염색 색상";
            colorEdit_DyeColor.Properties.ColorDialogOptions.ShowPreview = true;
            colorEdit_DyeColor.Click += colorDyeColorClicked;
            colorEdit_DyeColor.ReadOnly = true;
            item.Control = colorEdit_DyeColor;
            item.TextVisible = true;
            item.Text = "염색 색상";

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            checkEdit_YarnDyed = new CheckEdit();
            checkEdit_YarnDyed.Text = "선염";
            checkEdit_YarnDyed.Checked = true;
            checkEdit_YarnDyed.CheckedChanged += checkYarnDyedCheckedChanged;
            item.Control = checkEdit_YarnDyed;
            item.TextVisible = false;

            ///////////////////////////////////////////////////////////////////
            group = new LayoutControlGroup();
            group.Text = "경사정보";
            group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
            group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
            group.CustomDraw += Group_CustomDraw;
            layoutControl_Property.Root.Add(group);

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            simpleButton_Warp = new SimpleButton();
            simpleButton_Warp.Text = "..";
            simpleButton_Warp.Click += btnWarpClick;
            item.Control = simpleButton_Warp;
            item.Text = "경사 설정";

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            simpleButton_WarpArray = new SimpleButton();
            simpleButton_WarpArray.Text = "..";
            simpleButton_WarpArray.Click += btnWarpArrayClick;
            item.Control = simpleButton_WarpArray;
            item.Text = "경사 배열";

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "밀도";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            spinEdit_WarpDensity = new SpinEdit();
            spinEdit_WarpDensity.Properties.IsFloatValue = false;
            spinEdit_WarpDensity.Properties.MinValue = 10;
            spinEdit_WarpDensity.Properties.MaxValue = 300;
            spinEdit_WarpDensity.ValueChanged += SpinEdit_WarpDensity_ValueChanged;
            item.Control = spinEdit_WarpDensity;
            item.TextVisible = false;

            emptyItem = new EmptySpaceItem();
            emptyItem.AllowHotTrack = false;
            emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
            emptyItem.TextSize = new System.Drawing.Size(0, 0);
            layoutControl_Property.AddItem(emptyItem);

            ///////////////////////////////////////////////////////////////////
            group = new LayoutControlGroup();
            group.Text = "위사정보";
            group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
            group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
            group.CustomDraw += Group_CustomDraw;
            layoutControl_Property.Root.Add(group);

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            simpleButton_Weft = new SimpleButton();
            simpleButton_Weft.Text = "..";
            simpleButton_Weft.Click += btnWeftClick;
            item.Control = simpleButton_Weft;
            item.Text = "위사 설정";

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            simpleButton_WeftArray = new SimpleButton();
            simpleButton_WeftArray.Text = "..";
            simpleButton_WeftArray.Click += btnWeftArrayClick;
            item.Control = simpleButton_WeftArray;
            item.Text = "위사 배열";

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "밀도";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            spinEdit_WeftDensity = new SpinEdit();
            spinEdit_WeftDensity.Properties.IsFloatValue = false;
            spinEdit_WeftDensity.Properties.MinValue = 10;
            spinEdit_WeftDensity.Properties.MaxValue = 300;
            spinEdit_WeftDensity.ValueChanged += SpinEdit_WeftDensity_ValueChanged;
            item.Control = spinEdit_WeftDensity;
            item.TextVisible = false;


            emptyItem = new EmptySpaceItem();
            emptyItem.AllowHotTrack = false;
            emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
            emptyItem.TextSize = new System.Drawing.Size(0, 0);
            layoutControl_Property.AddItem(emptyItem);


            ///////////////////////////////////////////////////////////////////
            group = new LayoutControlGroup();
            group.Text = "조직정보";
            group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
            group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
            group.CustomDraw += Group_CustomDraw;
            layoutControl_Property.Root.Add(group);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "조직";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            textEdit_Pattern = new TextEdit();
            textEdit_Pattern.ReadOnly = true;
            item.Control = textEdit_Pattern;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            simpleButton_Pattern = new SimpleButton();
            simpleButton_Pattern.Text = "조직 설정";
            simpleButton_Pattern.Click += btnPatternClick;
            item.Control = simpleButton_Pattern;
            item.TextVisible = false;

            emptyItem = new EmptySpaceItem();
            emptyItem.AllowHotTrack = false;
            emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
            emptyItem.TextSize = new System.Drawing.Size(0, 0);
            layoutControl_Property.AddItem(emptyItem);

            ///////////////////////////////////////////////////////////////////
            group = new LayoutControlGroup();
            group.Text = "물성정보";
            group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
            group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
            group.CustomDraw += Group_CustomDraw;
            layoutControl_Property.Root.Add(group);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "굽힘강도 위사 (Bending-Weft)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_BendingWeft = new TrackBarControl();
            trackBar_BendingWeft.Name = "BendingWeft";
            textEdit_BendingWeft = new NumericUpDown();
            textEdit_BendingWeft.Name = "BendingWeft";
            createPhysicalPropertyControl(item, trackBar_BendingWeft, textEdit_BendingWeft);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "굽힘강도 경사 (Bending-Warp)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_BendingWarp = new TrackBarControl();
            trackBar_BendingWarp.Name = "BendingWarp";
            textEdit_BendingWarp = new NumericUpDown();
            textEdit_BendingWarp.Name = "BendingWarp";
            createPhysicalPropertyControl(item, trackBar_BendingWarp, textEdit_BendingWarp);

            
            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "내부 댐핑 (Internal Damping)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_InternalDamping = new TrackBarControl();
            trackBar_InternalDamping.Name = "InternalDamping";
            textEdit_InternalDamping = new NumericUpDown();
            textEdit_InternalDamping.Name = "InternalDamping";
            createPhysicalPropertyControl(item, trackBar_InternalDamping, textEdit_InternalDamping);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "마찰 계수 (Friction)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_Friction = new TrackBarControl();
            trackBar_Friction.Name = "Friction";
            textEdit_Friction = new NumericUpDown();
            textEdit_Friction.Name = "Friction";
            createPhysicalPropertyControl(item, trackBar_Friction, textEdit_Friction);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "밀도 조절 (Density)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_Density = new TrackBarControl();
            trackBar_Density.Name = "Density";
            textEdit_Density = new NumericUpDown();
            textEdit_Density.Name = "Density";
            createPhysicalPropertyControl(item, trackBar_Density, textEdit_Density);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "신축성 위사 (Stretch-Weft)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_StretchWeft = new TrackBarControl();
            trackBar_StretchWeft.Name = "StretchWeft";
            textEdit_StretchWeft = new NumericUpDown();
            textEdit_StretchWeft.Name = "StretchWeft";
            createPhysicalPropertyControl(item, trackBar_StretchWeft, textEdit_StretchWeft);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "신축성 경사 (Stretch-Warp)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_StretchWarp = new TrackBarControl();
            trackBar_StretchWarp.Name = "StretchWarp";
            textEdit_StretchWarp = new NumericUpDown();
            textEdit_StretchWarp.Name = "StretchWarp";
            createPhysicalPropertyControl(item, trackBar_StretchWarp, textEdit_StretchWarp);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "좌굴점 강도 위사 (Buckling Stiffness-Weft)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_BucklingStiffnessWeft = new TrackBarControl();
            trackBar_BucklingStiffnessWeft.Name = "BucklingStiffnessWeft";
            textEdit_BucklingStiffnessWeft = new NumericUpDown();
            textEdit_BucklingStiffnessWeft.Name = "BucklingStiffnessWeft";
            createPhysicalPropertyControl(item, trackBar_BucklingStiffnessWeft, textEdit_BucklingStiffnessWeft);


            item = group.AddItem();
            item.Padding = CreatePadding(PADDING_TOP_ITEM);
            label = new LabelControl();
            label.Text = "좌굴점 강도 경사 (Buckling Stiffness-Warp)";
            item.Control = label;
            item.TextVisible = false;

            item = group.AddItem();
            //item.Padding = CreatePadding(PADDING_TOP_ITEM);
            trackBar_BucklingStiffnessWarp = new TrackBarControl();
            trackBar_BucklingStiffnessWarp.Name = "BucklingStiffnessWarp";
            textEdit_BucklingStiffnessWarp = new NumericUpDown();
            textEdit_BucklingStiffnessWarp.Name = "BucklingStiffnessWarp";
            createPhysicalPropertyControl(item, trackBar_BucklingStiffnessWarp, textEdit_BucklingStiffnessWarp);
            

            ///////////////////////////////////////////////////////////////////
            layoutControl_Property.EndUpdate();
        }

        private void createPhysicalPropertyControl(LayoutControlItem item, TrackBarControl trackBar, NumericUpDown textEdit)
        {
            item.Padding = CreatePadding(PADDING_TOP_ITEM);

            TableLayoutPanel tlp = new TableLayoutPanel();
            //tlp.BackColor = System.Drawing.Color.Red;
            tlp.Dock = DockStyle.Fill;
            
            tlp.ColumnCount = 2;
            tlp.RowCount = 1;
            tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));

            trackBar.Dock = DockStyle.Fill;
            trackBar.Properties.Maximum = 100;
            trackBar.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
            trackBar.EditValueChanged += new System.EventHandler(this.trackBarControl_EditValueChanged);
            //textEdit.EditValueChanged += new System.EventHandler(this.textEdit_EditValueChanged);
            textEdit.ValueChanged += new System.EventHandler(this.numberUpDown_ValueChanged);

            tlp.Controls.Add(trackBar, 0, 0);
            tlp.Controls.Add(textEdit, 1, 0);
            tlp.MinimumSize = new System.Drawing.Size(0, 40);
            item.Control = tlp;
            item.TextVisible = false;
        }

        private void Group_CustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            Color c = Util.ToColor("707070");
            e.Cache.FillRectangle(new SolidBrush(c), e.Bounds);
        }







        ///////////////////////////////////////////////////////////////////////
        // 시작 - 외부 인터페이스
        ///////////////////////////////////////////////////////////////////////
        public ProjectData GetProjectData()
        {
            return this.GetProjectData(SELECTED_IDX);
        }
        public void UpdateProjectData()
        {
            ProjectData obj = this.GetProjectData(SELECTED_IDX);
            UpdateDAOProjectData(SELECTED_IDX, obj);
            //SetWeaveViewer(SELECTED_IDX, obj);
            SetProjectData(SELECTED_IDX, obj);
        }
        public ProjectData GetProjectData(int idx)
        {
            List<ProjectData> list = this.GetProjectDataList();
            for (int i = 0; i < list.Count; i++)
            {
                ProjectData obj = list[i];
                if (obj.Idx == idx)
                {
                    return obj;
                }
            }
            return null;
        }
        public List<ProjectData> GetProjectDataList()
        {
            return prjList;
        }
        public string GetAPPID()
        {
            return APPID;
        }

        public string GetDiffFilePath()
        {
            return weave2DViewer.GetDiffFilePath();
        }
        public string GetNormFilePath()
        {
            return weave2DViewer.GetNormFilePath();
        }
        public void ReloadMapWeave3DViewer()
        {
            weave3DViewer.ExecuteScriptAsync("ReloadMap2('" + SELECTED_IDX + "', '" + (nCall++) + "');");
        }

        public string GetMetalFrom3DViewer()
        {
            var task = weave3DViewer.EvaluateScriptAsync("GetMetalValue();");
            task.Wait();
            JavascriptResponse response = task.Result;
            string ret = response.Result.ToString();
            if (!string.IsNullOrEmpty(ret))
            {
                return ret;
            }
            return "0";
        }
        public string GetDrapeFrom3DViewer()
        {
            var task = weave3DViewer.EvaluateScriptAsync("GetDrapeValue();");
            task.Wait();
            JavascriptResponse response = task.Result;
            string ret = response.Result.ToString();
            if (!string.IsNullOrEmpty(ret))
            {
                return ret;
            }
            return "0";
        }
        public string GetObjectFrom3DViewer()
        {
            var task = weave3DViewer.EvaluateScriptAsync("GetObjectValue();");
            task.Wait();
            JavascriptResponse response = task.Result;
            string ret = response.Result.ToString();
            if (!string.IsNullOrEmpty(ret))
            {
                return ret;
            }
            return "cube";
        }
        bool isVisibleWarpOfPrint = false;
        public void SetPrintOptionWarpVisible(bool b)
        {
            isVisibleWarpOfPrint = b;
        }
        ///////////////////////////////////////////////////////////////////////
        // 시작 - 외부 인터페이스
        ///////////////////////////////////////////////////////////////////////






        ///////////////////////////////////////////////////////////////////////
        // 시작 - DAO
        ///////////////////////////////////////////////////////////////////////

        private void InitDAO()
        {
            try
            {
                // 게시 제품이름
                string szProductName = "WeavingGenerator";
                // Database 이름
                string szDBFileName = "weaving_ver1.db";
                // Database 지정할 경로
                string szExecutablePath = String.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), szProductName);
                DirectoryInfo di = new DirectoryInfo(szExecutablePath);
                if (!di.Exists)
                {
                    // 생성
                    Directory.CreateDirectory(szExecutablePath);
                }
                               
                // Database 지정할 경로 + Database 이름
                string szDBFile = String.Format(@"{0}\{1}", szExecutablePath, szDBFileName);                

                DateTime dt = DateTime.Now;
                string reg_dt = dt.ToString("yyyyMMddhhmmss");

                // sqlite.db가 해당 경로 폴더 안에 있는지 체크
                if (!System.IO.File.Exists(szDBFile))
                {
                    SQLiteConnection.CreateFile(szDBFile);

                    db.Init(null, szDBFile, "Resource/sql_acc.xml", null, null, null);
                    ///////////////////////////////////////////////////////////
                    // APP
                    ///////////////////////////////////////////////////////////
                    DBConn.Instance.create("create_tb_app");

                    this.APPID = Util.GenerateUUID();
                    Trace.WriteLine("strUUID : " + this.APPID);

                    // INSERT
                    Dictionary<string, object> param = new Dictionary<string, object>
                    {
                        { "@appid", this.APPID },
                        { "@reg_dt", reg_dt }
                    };
                    DBConn.Instance.insert("insert_tb_app", param);

                    ///////////////////////////////////////////////////////////
                    // TB_PROJECT                    
                    ///////////////////////////////////////////////////////////
                    DBConn.Instance.create("create_tb_project");


                    ///////////////////////////////////////////////////////////
                    // TB_YARN
                    ///////////////////////////////////////////////////////////
                    DBConn.Instance.create("create_tb_yarn");
                }

                db.Init(null, szDBFile, "Resource/sql_acc.xml", null, null, null);                

                // ilkwon test code end -----------------------------------------
                this.APPID = GetDAOAPPID();
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show(ex.Message);
            }
            
        }

        //-------------------------------------------------------------------
        private string GetDAOAPPID()
        {
            string appid = "";

            Dictionary<string, object> paramMap = new Dictionary<string, object>();
            DataResult dataResult = DBConn.Instance.select("select_tb_appid", paramMap);            
            if (dataResult != null && dataResult.Count > 0)
            {
                appid = dataResult.Data[0]["APPID"].ToString();
            }                           
            return appid;
        }

        //-------------------------------------------------------------------
        public ProjectData GetDAOProjectData(int idx)
        {
            ProjectData data = null;

            Dictionary<string, object> paramMap = new Dictionary<string, object>();
            paramMap.Add("IDX", idx);
            DataResult dataResult = DBConn.Instance.select("select_tb_project_by_idx", paramMap);
            if (dataResult != null && dataResult.Count > 0)
            {
                int projectIdx = Convert.ToInt32(dataResult.Data[0]["IDX"]);
                string projectName = dataResult.Data[0]["NAME"].ToString();
                string projectData = dataResult.Data[0]["PROJECT_DATA"].ToString();

                data = MainForm.ParseProjectData(projectData);
                data.Idx = idx;
                Console.WriteLine($"[프로젝트] IDX: {projectIdx}, NAME: {projectName}");
            }

            return data;
        }

        //-------------------------------------------------------------------
        public List<ProjectData> ListDAOProjectData()
        {
            List<ProjectData> list = new List<ProjectData>();
            
            try
            {
                Dictionary<string, object> paramMap = new Dictionary<string, object>(); // 파라미터 없음
                DataResult dataResult = DBConn.Instance.select("select_project_list", paramMap);
                if (dataResult != null && dataResult.Count > 0)
                {
                    foreach (var row in dataResult.Data)
                    {
                        int idx = Convert.ToInt32(row["IDX"]);
                        string projectDataString = row["PROJECT_DATA"].ToString();

                        ProjectData data = MainForm.ParseProjectData(projectDataString);
                        data.Idx = idx;
                        list.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
            }

            return list;
        }

        //-------------------------------------------------------------------
        private int SaveDAOProjectData(string name, string projectData)
        {
            int idx = -1;
            DateTime dt = DateTime.Now;
            string reg_dt = dt.ToString("yyyyMMddHHmmss"); // HHmmss 대문자(HH)로 24시간 표기

            try
            {
                // INSERT
                Dictionary<string, object> paramMap = new Dictionary<string, object>();
                paramMap.Add("name", name);
                paramMap.Add("reg_dt", reg_dt);
                paramMap.Add("project_data", projectData);

                DBConn.Instance.insert("insert_tb_project", paramMap);

                // LAST_INSERT_ROWID()
                Dictionary<string, object> emptyParam = new Dictionary<string, object>();
                DataResult dataResult = DBConn.Instance.select("select_last_insert_rowid", emptyParam);

                if (dataResult != null && dataResult.Count > 0)
                {
                    idx = Convert.ToInt32(dataResult.Data[0]["IDX"]);
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
            }
            
            return idx;
        }

        //-------------------------------------------------------------------
        public void UpdateDAOProjectData(int idx, ProjectData data)
        {
            string name = data.Name;
            string reg_dt = data.Reg_dt;
            string jsonData = MainForm.ParseJson(data);

          
            try
            {
                Dictionary<string, object> paramMap = new Dictionary<string, object>();
                paramMap.Add("name", name);
                paramMap.Add("project_data", jsonData);
                paramMap.Add("idx", idx);

                DBConn.Instance.update("update_tb_project_by_idx", paramMap);
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
            }
        }

        //-------------------------------------------------------------------
        public void RemoveDAOWeavingData(int idx)
        {
            try
            {
                Dictionary<string, object> paramMap = new Dictionary<string, object>();
                paramMap.Add("idx", idx);
                DBConn.Instance.delete("delete_tb_project_by_idx", paramMap);
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
            }
        }

        //-------------------------------------------------------------------
        public int SaveDAOYarn(Yarn yarn)
        {
            int idx = -1;
            DateTime dt = DateTime.Now;
            try
            {
                string name = yarn.Name;
                string weight = yarn.Weight;
                string unit = yarn.Unit;
                string type = yarn.Type;
                string textured = yarn.Textured;
                string image = yarn.Image;
                string metal = yarn.Metal;
                string reg_dt = dt.ToString("yyyyMMddhhmmss");

                if (string.IsNullOrEmpty(weight)) weight = "50";
                if (string.IsNullOrEmpty(unit)) unit = "Denier";
                if (string.IsNullOrEmpty(type)) type = "장섬유";
                if (string.IsNullOrEmpty(textured)) textured = "Filament";

                name = Util.AddSlashes(name);

                Dictionary<string, object> paramMap = new Dictionary<string, object>();
                paramMap.Add("name", name);
                paramMap.Add("weight", weight);
                paramMap.Add("unit", unit);
                paramMap.Add("type", type);
                paramMap.Add("textured", textured);
                paramMap.Add("metal", metal);
                paramMap.Add("image", image);
                paramMap.Add("reg_dt", reg_dt);

                int nRow = DBConn.Instance.insert("insert_tb_yarn", paramMap);
                if (nRow == 0)
                {
                    // 갱신 안됨...
                }
                // 마지막 Insert ID 가져오기
                Dictionary<string, object> emptyParam = new Dictionary<string, object>();
                DataResult dataResult = DBConn.Instance.select("select_last_insert_rowid", emptyParam);

                if (dataResult != null && dataResult.Count > 0)
                {
                    idx = Convert.ToInt32(dataResult.Data[0]["IDX"]);
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
            }
            
            return idx;
        }
        //-------------------------------------------------------------------
        public bool UpdateDAOYarn(Yarn yarn)
        {           
            DateTime dt = DateTime.Now;
            try
            {
                int idx = yarn.Idx;
                string name = yarn.Name;
                string weight = yarn.Weight;
                string unit = yarn.Unit;
                string type = yarn.Type;
                string textured = yarn.Textured;
                string metal = yarn.Metal;
                string image = yarn.Image;
                string reg_dt = yarn.Reg_dt;

                if (string.IsNullOrEmpty(weight)) weight = "50";
                if (string.IsNullOrEmpty(unit)) unit = "Denier";
                if (string.IsNullOrEmpty(type)) type = "장섬유";
                if (string.IsNullOrEmpty(textured)) textured = "Filament";

                name = Util.AddSlashes(name);

                Dictionary<string, object> paramMap = new Dictionary<string, object>();
                paramMap.Add("name", name);
                paramMap.Add("weight", weight);
                paramMap.Add("unit", unit);
                paramMap.Add("type", type);
                paramMap.Add("textured", textured);
                paramMap.Add("metal", metal);
                paramMap.Add("image", image);
                paramMap.Add("idx", idx);

                int nCount = DBConn.Instance.update("update_tb_yarn_by_idx", paramMap);
                if (nCount == 0)
                {
                    // 갱신 안됨...
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
                return false;
            }
      
            return true;
        }

        //-------------------------------------------------------------------
        public List<Yarn> ListDAOYarn()
        {
            List<Yarn> list = new List<Yarn>();

            try
            {
                Dictionary<string, object> paramMap = new Dictionary<string, object>(); // 파라미터 없음
                DataResult dataResult = DBConn.Instance.select("select_yarn_list", paramMap);

                if (dataResult != null && dataResult.Count > 0)
                {
                    foreach (var row in dataResult.Data)
                    {
                        Yarn yarn = new Yarn();
                        yarn.Idx = Convert.ToInt32(row["IDX"]);
                        yarn.Name = Util.StripSlashes(row["NAME"].ToString());
                        yarn.Weight = row["WEIGHT"].ToString();
                        yarn.Unit = row["UNIT"].ToString();
                        yarn.Type = row["TYPE"].ToString();
                        yarn.Textured = row["TEXTURED"].ToString();
                        yarn.Metal = row["METAL"].ToString();
                        yarn.Image = row["IMAGE"].ToString();
                        yarn.Reg_dt = row["REG_DT"].ToString();
                        list.Add(yarn);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
            }
            
            return list;
        }
        //-----------------------------------------------------------
        public void RemoveDAOYarn(int idx)
        {
            try
            {
                Dictionary<string, object> paramMap = new Dictionary<string, object>();
                paramMap.Add("idx", idx);

                int affectedRows = DBConn.Instance.update("soft_delete_tb_yarn_by_idx", paramMap);
                if (affectedRows == 0)
                {
                    // 삭체된게 없음.
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                XtraMessageBox.Show("Error", "Msg Box Title");
            }

        }
        ///////////////////////////////////////////////////////////////////////
        // 끝 - DAO
        ///////////////////////////////////////////////////////////////////////



        private void InitPatternList()
        {
            string json = Properties.Resources.PatternList;
            //Trace.WriteLine(json);

            JObject obj = JObject.Parse(json);

            JArray list = (JArray)obj["PatternList"];
            for (int x = 0; x < list.Count; x++)
            {
                JObject objPattern = (JObject)list[x];

                int idx = Convert.ToInt32(objPattern["Idx"].ToString());
                string name = objPattern["Name"].ToString();
                int Col = Convert.ToInt32(objPattern["XLength"].ToString());
                int Row = Convert.ToInt32(objPattern["YLength"].ToString());
                int[,] data = new int[Row, Col];
                JArray dataObj = (JArray)objPattern["Data"];
                for (int i = 0; i < dataObj.Count; i++)
                {
                    JArray temp = (JArray)dataObj[i];
                    for (int j = 0; j < temp.Count; j++)
                    {
                        data[i, j] = Convert.ToInt32(temp[j].ToString());
                    }
                }

                Pattern pattern = new Pattern();
                pattern.Idx = idx;
                pattern.Name = name;
                pattern.Data = data;

                patternList.Add(pattern);
            }

            //2025-01-24 soonchol
            //add user defined pattern at the last
            Pattern user_pattern = new Pattern();
            user_pattern.Idx = -1;
            user_pattern.Name = "USER";
            user_pattern.XLength = 2;
            user_pattern.YLength = 2;
            user_pattern.Data = new int[2, 2] { {1, 0}, {0, 1} };

            patternList.Add(user_pattern);

            ///////////////////////////////////////////////////////////////////
            // ComboBox 설정 (순서가 바뀌면 이벤트 함수에서 오류)
            ///////////////////////////////////////////////////////////////////
            //comboBoxPattern.DisplayMember = "Name";
            //comboBoxPattern.ValueMember = "Idx";
            //comboBoxPattern.DataSource = patternList;


        }

        public void checkbox_CheckedChanged(object sender, EventArgs e)
        {
            /*
            MyRadioButton btn = (MyRadioButton)sender;
            if (btn.Checked == false) return;

            //Trace.WriteLine("btn.Idx : " + btn.Idx);
            int idx = btn.Idx;

            string json = GetDAOWeavingData(idx);
            if (string.IsNullOrEmpty(json)) return;

            WeavingData data = ParseWeavingData(json);
            if (data == null) return;

            SetWeavingData(idx, data);
            */
        }


        ///////////////////////////////////////////////////////////////////////
        /// 저장 프로세스
        ///////////////////////////////////////////////////////////////////////
        private void ResetViewer()
        {
            // 뷰어 초기화
            //patternViewer1.ResetViewer();
            weave2DViewer.ResetViewer();

            weave3DViewer.ExecuteScriptAsync("ReloadMap('" + (nCall++) + "');");

            // 컨트롤 초기화
        }
        private void SetMetalness(string v)
        {
            // FD, SD, BR (없음, 약함, 강함)
            if (weave3DViewer != null)
            {
                if (IsLoaded3DObject == true)
                {
                    weave3DViewer.ExecuteScriptAsync("SetMetalness('" + v + "');");
                }
            }
        }
        public int CreateProject(string name)
        {
            ProjectData data = CreateDefaultProjectData(name);

            string json = ParseJson(data);
            int idx = SaveDAOProjectData(name, json);
            data.Idx = idx; // 입력 후 설정

            prjList.Insert(0, data);

            UpdateProjectView();
            SetProjectData(idx, data);

            return idx;
        }
        public void OpenProject(int idx)
        {
            ProjectData data = this.GetProjectData(idx);
            SetSelectedProjectButton(idx);
            SetProjectData(idx, data);
        }
        private void SaveProject()
        {
            IsModified = false;

            ///////////////////////////////////////////////////////////////////
            string name = textEdit_Name.Text;
            //string optionMetal = comboBoxEdit_OptionMetal.Text;
            string optionMetal = "FD";
            if (string.IsNullOrEmpty(name))
            {
                //오류창
                return;
            }

            ProjectData data = this.GetProjectData(SELECTED_IDX);

            data.Name = name;
            data.OptionMetal = optionMetal;

            data.Warp.Density = Util.ToInt(spinEdit_WarpDensity.Text);
            data.Weft.Density = Util.ToInt(spinEdit_WeftDensity.Text);

            /*
            data.PhysicalProperty.BendingWarp = Util.ToInt(textEdit_BendingWarp.Text);
            data.PhysicalProperty.BendingWeft = Util.ToInt(textEdit_BendingWeft.Text);
            data.PhysicalProperty.InternalDamping = Util.ToInt(textEdit_InternalDamping.Text);
            data.PhysicalProperty.Friction = Util.ToInt(textEdit_Friction.Text);
            data.PhysicalProperty.Density = Util.ToInt(textEdit_Density.Text);
            data.PhysicalProperty.StretchWeft = Util.ToInt(textEdit_StretchWeft.Text);
            data.PhysicalProperty.StretchWarp = Util.ToInt(textEdit_StretchWarp.Text);
            data.PhysicalProperty.BucklingStiffnessWeft = Util.ToInt(textEdit_BucklingStiffnessWeft.Text);
            data.PhysicalProperty.BucklingStiffnessWarp = Util.ToInt(textEdit_BucklingStiffnessWarp.Text);
            */
            data.PhysicalProperty.BendingWeft = (int)(textEdit_BendingWeft.Value);
            data.PhysicalProperty.BendingWarp = (int)(textEdit_BendingWarp.Value);
            data.PhysicalProperty.InternalDamping = (int)(textEdit_InternalDamping.Value);
            data.PhysicalProperty.Friction = (int)(textEdit_Friction.Value);
            data.PhysicalProperty.Density = (int)(textEdit_Density.Value);
            data.PhysicalProperty.StretchWeft = (int)(textEdit_StretchWeft.Value);
            data.PhysicalProperty.StretchWarp = (int)(textEdit_StretchWarp.Value);
            data.PhysicalProperty.BucklingStiffnessWeft = (int)(textEdit_BucklingStiffnessWeft.Value);
            data.PhysicalProperty.BucklingStiffnessWarp = (int)(textEdit_BucklingStiffnessWarp.Value);

            UpdateDAOProjectData(SELECTED_IDX, data);

            SaveAllProject();
        }
        private void SaveAllProject()
        {
            IsModified = false;

            for (int i = 0; i < prjList.Count; i++)
            {
                ProjectData obj = prjList[i];
                int idx = obj.Idx;

                UpdateDAOProjectData(idx, obj);
            }
        }
        public void RemoveProject(int idx)
        {
            for (int i = 0; i < prjList.Count; i++)
            {
                ProjectData w = prjList[i];
                if (w.Idx == idx)
                {
                    prjList.RemoveAt(i);
                    break;
                }
            }
            RemoveDAOWeavingData(idx);
            //InitProjectView();
            RemoveProjectButton(idx);

            if (idx == SELECTED_IDX)
            {
                SELECTED_IDX = -1;
                ResetViewer();
            }
        }
        public void UpdateProject()
        {
            //wDataList = ListDAOWeavingData();
            UpdateProjectView();
            if (SELECTED_IDX != -1)
            {
                OpenProject(SELECTED_IDX);
            }
        }
        private void DataRecevieEvent(string data)
        {
            if (data.Equals("Y"))
            {
                SaveProject();
            }
            else
            {
                IsModified = false;
            }
            dialogSave.Close();

            ExitApp();
        }

        private void ExitApp()
        {
            IsModified = false;
            if (IsModified == true)
            {
                dialogSave = new DialogConfirmSave();
                dialogSave.StartPosition = FormStartPosition.Manual;
                dialogSave.Location = GetChildFormLocation();
                dialogSave.DataPassEvent += new DialogConfirmSave.DataPassEventHandler(DataRecevieEvent);
                dialogSave.ShowDialog();
            }
            else
            {
                System.Windows.Forms.Application.Exit();

                try
                {
                    if (this.server != null && this.server.IsRunning)
                    {
                        this.server.Stop();
                        this.server = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[MainForm] server.Stop() "+  ex.Message);                    
                }
            }

        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitApp();
        }




        ///////////////////////////////////////////////////////////////////////
        // 시작 - 메인 메뉴 
        ///////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////
        // File 메인 메뉴 
        ///////////////////////////////////////////////////////////////////////





        int nCall = 0;
        //private System.Windows.Forms.Timer timer;
        //private int timerCount = 0;
        ///////////////////////////////////////////////////////////////////////
        // 3D Viewer Reload Thread
        ///////////////////////////////////////////////////////////////////////
        Thread threadGenerateImage = null;
        CancellationTokenSource ctsGenerateImage;
        
        public void ThreadViewerRepaint()
        {
            if (ctsGenerateImage!=null)
            {
                ctsGenerateImage.Cancel();
                threadGenerateImage?.Join();
                ctsGenerateImage.Dispose();
                ctsGenerateImage = null;
            }

            ctsGenerateImage = new CancellationTokenSource();
            threadGenerateImage = new Thread(() => ThreadRunGenerateImage(ctsGenerateImage.Token));
            threadGenerateImage.Start();
            //Trace.WriteLine("thread_generate START............ ");
        }
        private void ThreadRunGenerateImage(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            if (IsLoaded3DObject == true)
            {
                if (this.InvokeRequired) // 현재 스레드가 this(Form1) 요소를 만든 스레드를 경유해야 하는지 확인
                {
                    this.BeginInvoke(
                        (System.Action)(() =>
                        {
                            ShowProgressForm();

                            //patternViewer1.Generate3DImage("");
                            weave2DViewer.RepaintCanvas();
                            weave3DViewer.ExecuteScriptAsync("ReloadMap2('" + SELECTED_IDX + "', '" + (nCall++) + "');");

                            CloseProgressForm();
                        }));
                }
            }
        
        }

        ///////////////////////////////////////////////////////////////////////
        // Pattern
        ///////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////////////////
        // 끝 - 메인 메뉴 
        ///////////////////////////////////////////////////////////////////////








        ///////////////////////////////////////////////////////////////////////
        // 시작 - 직물 정보 설정 
        ///////////////////////////////////////////////////////////////////////

        public void SetJsonData(string jsonData)
        {
            ProjectData data = MainForm.ParseProjectData(jsonData);
            SetProjectData(SELECTED_IDX, data);
        }


        public void SetProjectData(int idx, ProjectData data)
        {
            if (data == null) return;

            this.SELECTED_IDX = idx;

            ///////////////////////////////////////////////////////////////////
            // 시작 - 컨트롤 설정
            ///////////////////////////////////////////////////////////////////

            // 프로젝트 버튼
            SetSelectedProjectButton(idx);

            // 기본 정보
            textEdit_Name.Text = data.Name;
            textEdit_BasicInfoRegDt.Text = Util.ToDateHuman(data.Reg_dt);

            // 경사 정보 
            spinEdit_WarpDensity.Text = data.Warp.Density.ToString();

            // 위사 정보
            spinEdit_WeftDensity.Text = data.Weft.Density.ToString();

            // 조직 정보
            textEdit_Pattern.Text = data.Pattern.Name;

            // 물성 정보
            /*
            textEdit_BendingWeft.Text = data.PhysicalProperty.BendingWeft.ToString();
            textEdit_BendingWarp.Text = data.PhysicalProperty.BendingWarp.ToString();
            textEdit_InternalDamping.Text = data.PhysicalProperty.InternalDamping.ToString();
            textEdit_Friction.Text = data.PhysicalProperty.Friction.ToString();
            textEdit_Density.Text = data.PhysicalProperty.Density.ToString();
            textEdit_StretchWeft.Text = data.PhysicalProperty.StretchWeft.ToString();
            textEdit_StretchWarp.Text = data.PhysicalProperty.StretchWarp.ToString();
            textEdit_BucklingStiffnessWeft.Text = data.PhysicalProperty.BucklingStiffnessWeft.ToString();
            textEdit_BucklingStiffnessWarp.Text = data.PhysicalProperty.BucklingStiffnessWarp.ToString();
            */
            textEdit_BendingWeft.Value = data.PhysicalProperty.BendingWeft;
            textEdit_BendingWarp.Value = data.PhysicalProperty.BendingWarp;
            textEdit_InternalDamping.Value = data.PhysicalProperty.InternalDamping;
            textEdit_Friction.Value = data.PhysicalProperty.Friction;
            textEdit_Density.Value = data.PhysicalProperty.Density;
            textEdit_StretchWeft.Value = data.PhysicalProperty.StretchWeft;
            textEdit_StretchWarp.Value = data.PhysicalProperty.StretchWarp;
            textEdit_BucklingStiffnessWeft.Value = data.PhysicalProperty.BucklingStiffnessWeft;
            textEdit_BucklingStiffnessWarp.Value = data.PhysicalProperty.BucklingStiffnessWarp;

            trackBar_BendingWarp.Value = data.PhysicalProperty.BendingWarp;
            trackBar_BendingWeft.Value = data.PhysicalProperty.BendingWeft;
            trackBar_InternalDamping.Value = data.PhysicalProperty.InternalDamping;
            trackBar_Friction.Value = data.PhysicalProperty.Friction;
            trackBar_Density.Value = data.PhysicalProperty.Density;
            trackBar_StretchWeft.Value = data.PhysicalProperty.StretchWeft;
            trackBar_StretchWarp.Value = data.PhysicalProperty.StretchWarp;
            trackBar_BucklingStiffnessWeft.Value = data.PhysicalProperty.BucklingStiffnessWeft;
            trackBar_BucklingStiffnessWarp.Value = data.PhysicalProperty.BucklingStiffnessWarp;
            ///////////////////////////////////////////////////////////////////
            // 끝 - 컨트롤 설정
            ///////////////////////////////////////////////////////////////////


            // 확대 기능을 제한
            // 배열의 총 개수가 25000 이상이면 x01 ~ 0x3 까지로 제한
            int[] warpArr = data.Warp.GetWArrayInt();
            int[] weftArr = data.Weft.GetWArrayInt();

            int nWidth = warpArr.Length;
            int nHeight = weftArr.Length;

            int cnt = nWidth * nHeight;
            if (cnt > 50000)
            {
                SetFullScale(false);
            }
            else
            {
                SetFullScale(true);
            }

            //2025-02-05 soonchol
            colorEdit_DyeColor.Color = ProjectData.GetDyeColor(data.DyeColor);
            checkEdit_YarnDyed.Checked = data.YardDyed;
            //checkYarnDyedCheckedChanged(checkEdit_YarnDyed, null);


            ///////////////////////////////////////////////////////////////////
            // 뷰어 설정
            ///////////////////////////////////////////////////////////////////
            SetWeaveViewer(SELECTED_IDX, data);
        }
        private void SetFullScale(bool full)
        {
            int n = comboBoxEdit_Scale.SelectedIndex;

            if (full == false)
            {
                ///////////////////////////////////////////////////////////////
                // 총 개수가 25000 이면 x01 ~ x03 까지만 지원
                ///////////////////////////////////////////////////////////////
                isRepaintScale = false;
                comboBoxEdit_Scale.SelectedIndex = 0;
                isRepaintScale = true;

                comboBoxEdit_Scale.Properties.Items.Clear();
                comboBoxEdit_Scale.Properties.Items.AddRange(new object[] {
                    "x01 (기본)",
                    "x02",
                    "x03"
                });
            }
            else
            {
                comboBoxEdit_Scale.Properties.Items.Clear();
                comboBoxEdit_Scale.Properties.Items.AddRange(new object[] {
                    "x01 (기본)",
                    "x02",
                    "x03",
                    "x04",
                    "x05",
                    "x06",
                    "x07",
                    "x08"
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////
        // 끝 - 직물 정보 설정
        ///////////////////////////////////////////////////////////////////////


        private Pattern GetPattern(int idx)
        {
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
        private int GetPatternIndx(int idx)
        {
            int z = -1;
            for (int i = 0; i < patternList.Count; i++)
            {
                Pattern pattern = patternList[i];
                if (pattern.Idx == idx)
                {
                    z = i;
                }
            }
            return z;
        }
        public void SetPattern(int idx)
        {
            Pattern pattern = GetPattern(idx);
            if (pattern == null) return;

            ///////////////////////////////////////////////////////////////////
            //
            ///////////////////////////////////////////////////////////////////
            ProjectData data = this.GetProjectData(SELECTED_IDX);
            if (data == null) return;

            data.Pattern = pattern;

            ///////////////////////////////////////////////////////////////////
            // 컨트롤 설정
            ///////////////////////////////////////////////////////////////////
            textEdit_Pattern.Text = pattern.Name;

            SetWeaveViewer(SELECTED_IDX, data);
        }
        public void ResetViewScale()
        {
            if (comboBoxEdit_Scale.SelectedIndex == 0)
            {
                return;
            }
            comboBoxEdit_Scale.SelectedIndex = 0;
            weave2DViewer.SetViewScale(1);
        }
        public void Export2DDImage(string fullPath)
        {
            string path = System.IO.Path.GetDirectoryName(fullPath);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                // 생성
                Directory.CreateDirectory(path);
            }
            //ResetViewScale();
            weave2DViewer.Export2DDImage(fullPath);
        }

        string exportCloFabricPathHistory = "";
        public void ExportCloFabric(string fullPath)
        {
            /*
            string path = System.IO.Path.GetDirectoryName(fullPath);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                // 생성
                Directory.CreateDirectory(path);
            }
            */
            exportCloFabricPathHistory = fullPath;
            //
            string cFile = MainForm.CLO_PATH + "\\FABRIC.fab";
            //XtraMessageBox.Show(cFile);
            string dFile = GetDiffFilePath();
            string nFile = GetNormFilePath();

            CloFabric clo = new CloFabric();
            clo.setFabricFilePath(cFile);
            clo.setDiffFilePath(dFile);
            clo.setNormFilePath(nFile);
            clo.setExportFilePath(fullPath);


            int bendingWeft = (int)(textEdit_BendingWeft.Value);
            int bendingWarp = (int)(textEdit_BendingWarp.Value);
            int internalDamping = (int)(textEdit_InternalDamping.Value);
            int friction = (int)(textEdit_Friction.Value);
            int density = (int)(textEdit_Density.Value);
            int stretchWeft = (int)(textEdit_StretchWeft.Value);
            int stretchWarp = (int)(textEdit_StretchWarp.Value);
            int bucklingStiffnessWeft = (int)(textEdit_BucklingStiffnessWeft.Value);
            int bucklingStiffnessWarp = (int)(textEdit_BucklingStiffnessWarp.Value);

            clo.setPhysicalProperty(bendingWeft,
                bendingWarp,
                internalDamping,
                friction,
                density,
                stretchWeft,
                stretchWarp,
                bucklingStiffnessWeft,
                bucklingStiffnessWarp);

            Trace.WriteLine("bendingWeft : " + bendingWeft);
            Trace.WriteLine("bendingWarp : " + bendingWarp);
            Trace.WriteLine("internalDamping : " + internalDamping);
            Trace.WriteLine("friction : " + friction);
            Trace.WriteLine("density : " + density);
            Trace.WriteLine("stretchWeft : " + stretchWeft);
            Trace.WriteLine("stretchWarp : " + stretchWarp);
            Trace.WriteLine("bucklingStiffnessWeft : " + bucklingStiffnessWeft);
            Trace.WriteLine("bucklingStiffnessWarp : " + bucklingStiffnessWarp);

            // 파일 만들기
            string fabFilePath = clo.exportFabric();

            XtraMessageBox.Show("내보내기를 완료 했습니다.\n File : " + fabFilePath);
        }













        ///////////////////////////////////////////////////////////////////////
        ///  EVENT 시작
        ///////////////////////////////////////////////////////////////////////

        public void btnProjectClick(object sender, EventArgs e)
        {
            ProjectButton btn = (ProjectButton)sender;

            if (SELECTED_IDX == btn.Idx) return;
            int idx = btn.Idx;

            ProjectData data = this.GetProjectData(idx);
            if (data == null) return;

            SetProjectData(idx, data);
        }
        private void textEdit_Name_TextChanged(object sender, EventArgs e)
        {
            if (SELECTED_IDX < 0)
            {
                return;
            }
            ProjectData data = this.GetProjectData(SELECTED_IDX);
            if (data == null) return;

            //Trace.WriteLine("textEdit_Name : " + textEdit_Name.Text);
            data.Name = textEdit_Name.Text;
            UpdateProjectButton(SELECTED_IDX, data.Name);
        }

        private void SpinEdit_WarpDensity_ValueChanged(object sender, EventArgs e)
        {
            if (SELECTED_IDX < 0)
            {
                return;
            }
            ProjectData data = this.GetProjectData(SELECTED_IDX);
            if (data == null) return;

            Warp warp = data.Warp;
            if (warp == null) return;

            int oldValue = warp.Density;
            int newValue = Util.ToInt(spinEdit_WarpDensity.Text, 50);
            if (oldValue != newValue)
            {
                warp.Density = Util.ToInt(spinEdit_WarpDensity.Text, 50);
                SetWeaveViewer(SELECTED_IDX, data);
            }
        }
        private void SpinEdit_WeftDensity_ValueChanged(object sender, EventArgs e)
        {
            if (SELECTED_IDX < 0)
            {
                return;
            }
            ProjectData data = this.GetProjectData(SELECTED_IDX);
            if (data == null) return;

            Weft weft = data.Weft;
            if (weft == null) return;


            int oldValue = weft.Density;
            int newValue = Util.ToInt(spinEdit_WeftDensity.Text, 50);
            if (oldValue != newValue)
            {
                weft.Density = Util.ToInt(spinEdit_WeftDensity.Text, 50);
                SetWeaveViewer(SELECTED_IDX, data);
            }

        }

        //private void SpinBox_WarpDensity_LostFocus(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("spinBox_WarpDensity : " + spinBox_WarpDensity.Text);
        //}
        /*
        private void ComboBoxEdit_OptionMetal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_OptionMetal.SelectedIndex < 0)
            {
                return;
            }
            ComboBoxEdit cb = (ComboBoxEdit)sender;
            if (cb.SelectedIndex > -1)
            {
                string v = cb.SelectedItem.ToString();
                SetMetalness(v);
            }
        }
        */
        bool isRepaintScale = true;
        private void ComboBoxEdit_Scale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_Scale.SelectedIndex < 0)
            {
                return;
            }

            int oldScale = weave2DViewer.GetViewScale();
            if (comboBoxEdit_Scale.SelectedIndex == (oldScale - 1))
            {
                return;
            }

            ShowProgressForm();

            int viewScale = comboBoxEdit_Scale.SelectedIndex + 1;
            weave2DViewer.SetViewScale(viewScale, isRepaintScale);

            CloseProgressForm();

        }

        private void btnJsonClick(object sender, EventArgs e)
        {
            // 임시
            DialogJsonData dialog = new DialogJsonData(this);
            string json = ParseJson(this.GetProjectData(SELECTED_IDX));
            dialog.SetJsonData(json);
            dialog.ShowDialog();
        }
        private void btnWarpClick(object sender, EventArgs e)
        {
            DialogWarpInfo dialog = new DialogWarpInfo(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }
        private void btnWeftArrayClick(object sender, EventArgs e)
        {
            DialogWeftArray dialog = new DialogWeftArray(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }
        private void btnWeftClick(object sender, EventArgs e)
        {
            DialogWeftInfo dialog = new DialogWeftInfo(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }
        private void btnWarpArrayClick(object sender, EventArgs e)
        {
            DialogWarpArray dialog = new DialogWarpArray(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }
        private void btnPatternClick(object sender, EventArgs e)
        {
            DialogPatternList dialog = new DialogPatternList(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
            ThreadViewerRepaint();
        }
        private void checkYarnImageCheckedChanged(object sender, EventArgs e)
        {
            CheckEdit chk = (CheckEdit)sender;
            //if (chk.Checked == false) return;
            //patternViewer1.SetYarnImage(chk.Checked);
            weave2DViewer.SetYarnImage(chk.Checked);
            ThreadViewerRepaint();
        }

        //2025-02-05 soonchol
        private void colorDyeColorClicked(object sender, EventArgs e)
        {
            if (checkEdit_YarnDyed.Checked == true) return;

            ColorEdit colorEdit = (ColorEdit)sender;
            Color oldColor = colorEdit.Color;
            Color newColor = DoShowColorDialog(oldColor);
            colorEdit.Color = newColor;

            //if (chk.Checked == false) return;
            //patternViewer1.SetYarnImage(chk.Checked);

            ProjectData wData = GetProjectData();
            //2025-02-05 soonchol
            if (wData != null)
            {
                wData.DyeColor = ProjectData.GetDyeColor(newColor);
                weave2DViewer.SetYarnDyeColor(wData.DyeColor);
            }

            weave2DViewer.SetProjectData(SELECTED_IDX, GetProjectData());
            ThreadViewerRepaint();
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
                {
                    return color;
                }
            }
        }

        private void checkYarnDyedCheckedChanged(object sender, EventArgs e)
        {
            CheckEdit chk = (CheckEdit)sender;
            //if (chk.Checked == false) return;
            //patternViewer1.SetYarnImage(chk.Checked);

            ProjectData wData = GetProjectData();
            //2025-02-05 soonchol
            if (wData != null)
            {
                wData.YardDyed = chk.Checked;

                if (wData.YardDyed == true)
                {
                    weave2DViewer.SetYarnDyeColor(null);
                }
                else
                {
                    if (wData.DyeColor == null || wData.DyeColor == "")
                    {
                        wData.DyeColor = Default_DyeColor;
                    }

                    weave2DViewer.SetYarnDyeColor(wData.DyeColor);
                }

            }

            weave2DViewer.SetProjectData(SELECTED_IDX, GetProjectData());
            ThreadViewerRepaint();
        }


        private void barButtonItem_NewProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogNewProject dialog = new DialogNewProject(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.dialogNewProjectEventHandler += new DialogNewProjectEventHandler(EventNewProject);
            dialog.ShowDialog();
        }
        private void EventNewProject(object sender, int newIdx)
        {
            SetSelectedProjectButton(newIdx);
            //SetProjectData(tempRunIdx, tempRunWData);
        }
        private void barButtonItem_OpenProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogProjectList dialog = new DialogProjectList(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.dialogOpenProjectEventHandler += new DialogOpenProjectEventHandler(EventOpenProject);
            dialog.ShowDialog();
        }
        private void EventOpenProject(object sender, int openIdx)
        {
            OpenProject(openIdx);
        }

        private void barButtonItem_SaveProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveProject();
        }

        private void barButtonItem_Exit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExitApp();
        }

        private void barButtonItem_NewYarn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogNewYarn dialog = new DialogNewYarn(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_OpenYarn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogYarnList dialog = new DialogYarnList(this, -1);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_OpenPattern_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogPatternList dialog = new DialogPatternList(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_NewPattern_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogNewPattern dialog = new DialogNewPattern(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_Density_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogDensity dialog = new DialogDensity(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.dialogUpdateDensityEventHandler += new DialogUpdateDensityEventHandler(EventUpdateDensity);
            dialog.ShowDialog();
        }
        private void EventUpdateDensity(object sender, int newIdx)
        {
            ProjectData data = this.GetProjectData(newIdx);
            if (data == null) return;

            spinEdit_WarpDensity.Text = data.Warp.Density.ToString();
            spinEdit_WeftDensity.Text = data.Weft.Density.ToString();
            SetWeaveViewer(newIdx, data);
        }
        private void barButtonItem_OpenWarp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogWarpInfo dialog = new DialogWarpInfo(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_OpenWarpArray_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogWarpArray dialog = new DialogWarpArray(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_OpenWeft_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogWeftInfo dialog = new DialogWeftInfo(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_OpenWeftArray_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogWeftArray dialog = new DialogWeftArray(this, this.GetProjectData(SELECTED_IDX));
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_Export_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogExport dialog = new DialogExport(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();

        }
        private void barButtonItem_Upload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogUpload dialog = new DialogUpload(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }

        private void barButtonItem_Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DialogPrintOption dialog = new DialogPrintOption(this);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string diffFilePath = MainForm.WWWROOT_PATH + "\\diff_" + SELECTED_IDX + ".png";
            string printFilePath = MainForm.WWWROOT_PATH + "\\diff_" + SELECTED_IDX + "_Print.png";

            // 100 px * 100 px , 96 dpi 는 인쇄시 약 1인치에 해당함
            ProjectData weaveData = GetProjectData();

            ResizeImageFile(diffFilePath, printFilePath);


            List<Yarn> yarnList = ListDAOYarn();

            //BasicInfo binfo = weaveData.BasicInfo;
            Pattern pattern = weaveData.Pattern;
            Warp warp = weaveData.Warp;
            Weft weft = weaveData.Weft;
            PhysicalProperty pProperty = weaveData.PhysicalProperty;

            string json = "";
            string jsonProject = "";
            string projectName = weaveData.Name;
            string projectID = weaveData.ProjectID;
            string memo = weaveData.Memo;
            string projectReg_dt = weaveData.Reg_dt;
            try
            {
                projectReg_dt = DateTime.ParseExact(projectReg_dt, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception)
            {
                
            }

            //printFilePath = MainForm.WWWROOT_PATH + "\\diff_" + SELECTED_IDX + "_Print.png";
            printFilePath = Util.EscapeStringValue(printFilePath);

            jsonProject = "   " +
                "   \"Name\":\"" + projectName + "\"," +
                "   \"ProjectID\":\"" + projectID + "\"," +
                "   \"Memo\":\"" + memo + "\"," +
                "   \"ImageSrc\":\"" + printFilePath + "\"," +
                "   \"Reg_dt\":\"" + projectReg_dt + "\" " +
                "";


            ///////////////////////////////////////////////////////////////////
            // Pattern
            ///////////////////////////////////////////////////////////////////
            string jsonPattern = "";
            if (pattern != null)
            {
                int idx = pattern.Idx;
                string name = pattern.Name;
                int Col = pattern.XLength;
                int Row = pattern.YLength;
                int[,] Data = pattern.Data;

                string strData = "";
                for (int i = 0; i < Data.GetLength(0); i++)
                {
                    string temp = "";
                    for (int j = 0; j < Data.GetLength(1); j++)
                    {
                        if (j > 0)
                        {
                            temp = temp + ",";
                        }
                        temp = temp + Data[i, j];
                    }
                    temp = "[" + temp + "]";

                    if (i > 0)
                    {
                        strData = strData + ",";
                    }
                    strData = strData + temp;
                }
                strData = "[" + strData + "]";

                jsonPattern = "" +
                    "," +
                    "\"Pattern\":{" +
                    "   \"Idx\":" + idx + "," +
                    "   \"Name\":\"" + name + "\"," +
                    "   \"XLength\":" + Col + "," +
                    "   \"YLength\":" + Row + "," +
                    "   \"Data\":" + strData +
                    "}";
            }
            ///////////////////////////////////////////////////////////////////
            // Warp
            ///////////////////////////////////////////////////////////////////
            string jsonWarp = "";
            if (warp != null)
            {
                int density = warp.Density;
                int warpCount = warp.GetWInfoLength();
                List<WInfo> listWarpInfo = warp.GetWInfoList();
                List<WArray> listWarpArrayInfo = warp.GetWArrayList();
                List<WRepeat> listRepeat = warp.GetWRepeatList();

                string strWarpInfoList = "";
                for (int i = 0; i < listWarpInfo.Count; i++)
                {
                    WInfo warpInfo = listWarpInfo[i];
                    int idx = warpInfo.Idx;
                    string name = warpInfo.Name;
                    string hexColor = warpInfo.HexColor;
                    int idxYarn = warpInfo.IdxYarn;

                    string yarnName = "-";
                    string yarnWeight = "250"; //기본값
                    string yarnUnit = "Denier";
                    string yarnType = "";
                    string yarnTextured = "";

                    for (int j = 0; j < yarnList.Count; j++)
                    {
                        Yarn yarn = yarnList[j];
                        if (yarn.Idx == idxYarn)
                        {
                            yarnName = yarn.Name;
                            yarnWeight = yarn.Weight;
                            yarnUnit = yarn.Unit;
                            yarnType = yarn.Type;
                            yarnTextured = yarn.Textured;
                            break;
                        }
                    }

                    string temp = "{" +
                        "   \"Idx\":" + idx + "," +
                        "   \"Name\":\"" + yarnName + "\"," +
                        "   \"IdxYarn\":\"" + idxYarn + "\"," +
                        "   \"Weight\":\"" + yarnWeight + "\"," +
                        "   \"Unit\":\"" + yarnUnit + "\"," +
                        "   \"Type\":\"" + yarnType + "\"," +
                        "   \"Textured\":\"" + yarnTextured + "\"," +
                        "   \"HexColor\":\"" + hexColor + "\"" +
                        "}";

                    if (i > 0)
                    {
                        strWarpInfoList = strWarpInfoList + ",";
                    }
                    strWarpInfoList = strWarpInfoList + temp;
                }
                strWarpInfoList = "[" + strWarpInfoList + "]";


                jsonWarp = "" +
                    "," +
                    "\"Warp\":{" +
                    "   \"Density\":" + density + "," +
                    "   \"WarpInfoList\": " + strWarpInfoList + "" +
                    "}";
            }


            ///////////////////////////////////////////////////////////////////
            // Weft
            ///////////////////////////////////////////////////////////////////
            string jsonWeft = "";
            if (weft != null)
            {
                int density = weft.Density;
                int weftCount = weft.GetWInfoLength();
                List<WInfo> listWeftInfo = weft.GetWInfoList();
                List<WArray> listWeftArrayInfo = weft.GetWArrayList();
                List<WRepeat> listRepeat = warp.GetWRepeatList();

                string strWeftInfoList = "";
                for (int i = 0; i < listWeftInfo.Count; i++)
                {
                    WInfo weftInfo = listWeftInfo[i];
                    int idx = weftInfo.Idx;
                    string name = weftInfo.Name;
                    string hexColor = weftInfo.HexColor;
                    int idxYarn = weftInfo.IdxYarn;

                    string yarnName = "-";
                    string yarnWeight = "250"; //기본값
                    string yarnUnit = "Denier";
                    string yarnType = "";
                    string yarnTextured = "";

                    for (int j = 0; j < yarnList.Count; j++)
                    {
                        Yarn yarn = yarnList[j];
                        if (yarn.Idx == idxYarn)
                        {
                            yarnName = yarn.Name;
                            yarnWeight = yarn.Weight;
                            yarnUnit = yarn.Unit;
                            yarnType = yarn.Type;
                            yarnTextured = yarn.Textured;
                            break;
                        }
                    }

                    string temp = "{" +
                        "   \"Idx\":" + idx + "," +
                        "   \"Name\":\"" + yarnName + "\"," +
                        "   \"IdxYarn\":\"" + idxYarn + "\"," +
                        "   \"Weight\":\"" + yarnWeight + "\"," +
                        "   \"Unit\":\"" + yarnUnit + "\"," +
                        "   \"Type\":\"" + yarnType + "\"," +
                        "   \"Textured\":\"" + yarnTextured + "\"," +
                        "   \"HexColor\":\"" + hexColor + "\"" +
                        "}";

                    if (i > 0)
                    {
                        strWeftInfoList = strWeftInfoList + ",";
                    }
                    strWeftInfoList = strWeftInfoList + temp;
                }
                strWeftInfoList = "[" + strWeftInfoList + "]";


                jsonWeft = "" +
                    "," +
                    "\"Weft\":{" +
                    "   \"Density\":" + density + "," +
                    "   \"WeftInfoList\": " + strWeftInfoList + "" +
                    "}";

                json = "{" +
                    jsonProject +
                    jsonPattern +
                    jsonWarp +
                    jsonWeft +
                    "}";

                JsonDataSource jds = new JsonDataSource();
                jds.JsonSource = new CustomJsonSource(json);
                jds.Fill();

                XtraReportPrint report1 = new XtraReportPrint(isVisibleWarpOfPrint);
                report1.DataSource = jds;
                report1.CreateDocument();

                PrintPreviewFormEx form = new PrintPreviewFormEx();
                form.StartPosition = FormStartPosition.Manual;
                form.SaveState = false;
                form.Location = GetChildFormLocation();
                form.PrintingSystem = report1.PrintingSystem;
                form.ShowDialog();

                ///////////////////////////////////////////////////////////////
                // Bind a Report to JSON Data (Runtime Sample)
                // https://docs.devexpress.com/XtraReports/400380/detailed-guide-to-devexpress-reporting/bind-reports-to-data/json-data/bind-a-report-to-json-data-runtime-sample
                //
                // // Bind a Report to Multiple Data Sources
                // https://docs.devexpress.com/XtraReports/5042/detailed-guide-to-devexpress-reporting/bind-reports-to-data/bind-a-report-to-multiple-data-sources
                //
                // 색상값
                // https://docs.devexpress.com/XtraReports/119473/detailed-guide-to-devexpress-reporting/use-expressions/expressions-tasks-and-solutions/conditionally-change-a-controls-appearance
                //////////////////////////////////////////////////////////////
            }
        }


        private void barButtonItem_Link_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProjectData data = this.GetProjectData(SELECTED_IDX);
            if (data == null)
            {
                XtraMessageBox.Show("프로젝트 생성 후 이용해주세요..", "Error");
                return;
            }
            string appid = this.APPID;
            string projectid = data.ProjectID;

            string url = VIEWER_URL + "?APPID=" + appid + "&PROJECTID=" + projectid;
            System.Diagnostics.Process.Start(url);
        }


        private void barButtonItem_CloFabric_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*
            string cFile = MainForm.CLO_PATH + "\\FABRIC.fab";
            //XtraMessageBox.Show(aFile, "Error");
            string dFile = GetDiffFilePath();
            string nFile = GetNormFilePath();

            CloFabric clo = new CloFabric();
            clo.setFabricFilePath(cFile);
            clo.setDiffFilePath(dFile);
            clo.setNormFilePath(nFile);
            clo.setExportFilePath("C:\\mfafa\\VMF\\temp");
            

            int bendingWeft = Util.ToInt(textEdit_BendingWeft.Text);
            int bendingWarp = Util.ToInt(textEdit_BendingWarp.Text);
            int internalDamping = Util.ToInt(textEdit_InternalDamping.Text);
            int friction = Util.ToInt(textEdit_Friction.Text);
            int density = Util.ToInt(textEdit_Density.Text);
            int stretchWeft = Util.ToInt(textEdit_StretchWeft.Text);
            int stretchWarp = Util.ToInt(textEdit_StretchWarp.Text);
            int bucklingStiffnessWeft = Util.ToInt(textEdit_BucklingStiffnessWeft.Text);
            int bucklingStiffnessWarp = Util.ToInt(textEdit_BucklingStiffnessWarp.Text);

            
            clo.setPhysicalProperty(bendingWeft,
                bendingWarp,
                internalDamping,
                friction,
                density,
                stretchWeft,
                stretchWarp,
                bucklingStiffnessWeft,
                bucklingStiffnessWarp);

            Trace.WriteLine("bendingWeft : " + bendingWeft);
            Trace.WriteLine("bendingWarp : " + bendingWarp);
            Trace.WriteLine("internalDamping : " + internalDamping);
            Trace.WriteLine("friction : " + friction);
            Trace.WriteLine("density : " + density);
            Trace.WriteLine("stretchWeft : " + stretchWeft);
            Trace.WriteLine("stretchWarp : " + stretchWarp);
            Trace.WriteLine("bucklingStiffnessWeft : " + bucklingStiffnessWeft);
            Trace.WriteLine("bucklingStiffnessWarp : " + bucklingStiffnessWarp);

            // 파일 만들기
            clo.exportFabric();
            */

            DialogCloFabric dialog = new DialogCloFabric(this, exportCloFabricPathHistory);
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = GetChildFormLocation();
            dialog.ShowDialog();
        }
        private void trackBarControl_EditValueChanged(object sender, EventArgs e)
        {
            TrackBarControl trackBarControl = sender as TrackBarControl;
            string name = trackBarControl.Name;
            //string value = trackBarControl.EditValue.ToString();
            int value = Util.ToInt(trackBarControl.EditValue.ToString());
            //textEdit1.Text = trackBarControl.EditValue.ToString();
            if (name.Equals("BendingWeft"))
            {
                textEdit_BendingWeft.Value = value;
            }
            else if (name.Equals("BendingWarp"))
            {
                textEdit_BendingWarp.Value = value;
            }
            else if (name.Equals("InternalDamping"))
            {
                textEdit_InternalDamping.Value = value;
            }
            else if (name.Equals("Friction"))
            {
                textEdit_Friction.Value = value;
            }
            else if (name.Equals("Density"))
            {
                textEdit_Density.Value = value;
            }
            else if (name.Equals("StretchWeft"))
            {
                textEdit_StretchWeft.Value = value;
            }
            else if (name.Equals("StretchWarp"))
            {
                textEdit_StretchWarp.Value = value;
            }
            else if (name.Equals("BucklingStiffnessWeft"))
            {
                textEdit_BucklingStiffnessWeft.Value = value;
            }
            else if (name.Equals("BucklingStiffnessWarp"))
            {
                textEdit_BucklingStiffnessWarp.Value = value;
            }
        }
        private void textEdit_EditValueChanged(object sender, EventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            string name = textEdit.Name;
            int value = Int32.Parse(textEdit.Text);

            if (name.Equals("BendingWeft"))
            {
                trackBar_BendingWeft.Value = value;
            }
            else if (name.Equals("BendingWarp"))
            {
                trackBar_BendingWarp.Value = value;
            }
            else if (name.Equals("InternalDamping"))
            {
                trackBar_InternalDamping.Value = value;
            }
            else if (name.Equals("Friction"))
            {
                trackBar_Friction.Value = value;
            }
            else if (name.Equals("Density"))
            {
                trackBar_Density.Value = value;
            }
            else if (name.Equals("StretchWeft"))
            {
                trackBar_StretchWeft.Value = value;
            }
            else if (name.Equals("StretchWarp"))
            {
                trackBar_StretchWarp.Value = value;
            }
            else if (name.Equals("BucklingStiffnessWeft"))
            {
                trackBar_BucklingStiffnessWeft.Value = value;
            }
            else if (name.Equals("BucklingStiffnessWarp"))
            {
                trackBar_BucklingStiffnessWarp.Value = value;
            }
            /*
            int result = Int32.Parse(textEdit1.Text);
            trackBarControl1.Value = result;
            */
        }
        private void numberUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown textEdit = sender as NumericUpDown;
            string name = textEdit.Name;
            int value = (int)(textEdit.Value);

            if (name.Equals("BendingWeft"))
            {
                trackBar_BendingWeft.Value = value;
            }
            else if (name.Equals("BendingWarp"))
            {
                trackBar_BendingWarp.Value = value;
            }
            else if (name.Equals("InternalDamping"))
            {
                trackBar_InternalDamping.Value = value;
            }
            else if (name.Equals("Friction"))
            {
                trackBar_Friction.Value = value;
            }
            else if (name.Equals("Density"))
            {
                trackBar_Density.Value = value;
            }
            else if (name.Equals("StretchWeft"))
            {
                trackBar_StretchWeft.Value = value;
            }
            else if (name.Equals("StretchWarp"))
            {
                trackBar_StretchWarp.Value = value;
            }
            else if (name.Equals("BucklingStiffnessWeft"))
            {
                trackBar_BucklingStiffnessWeft.Value = value;
            }
            else if (name.Equals("BucklingStiffnessWarp"))
            {
                trackBar_BucklingStiffnessWarp.Value = value;
            }
            /*
            int result = Int32.Parse(textEdit1.Text);
            trackBarControl1.Value = result;
            */
        }
        ///////////////////////////////////////////////////////////////////////
        ///  EVENT 끝
        ///////////////////////////////////////////////////////////////////////






        private void SetWeaveViewer(int idx, ProjectData data)
        {
            weave2DViewer.SetProjectData(idx, data);
            ThreadViewerRepaint();
        }

        public static ProjectData ParseProjectData(string json)
        {
            if (string.IsNullOrEmpty(json) == true)
            {
                return null;
            }
            JObject rootJObj = JObject.Parse(json);
            if (rootJObj == null)
            {
                return null;
            }

            ///////////////////////////////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////
            ProjectData prjTemp = new ProjectData();

            string projectName = rootJObj["Name"].ToString();
            string projectID = rootJObj["ProjectID"].ToString();
            string projectReg_dt = rootJObj["Reg_dt"].ToString();

            string optionMetal = (rootJObj["OptionMetal"] == null) ? "0" : rootJObj["OptionMetal"].ToString();
            string memo_default = "중량:\r\n폭:\r\n혼용률:\r\n";
            string memo = (rootJObj["Memo"] == null) ? memo_default : Util.Base64Decode(rootJObj["Memo"].ToString());

            prjTemp.Name = projectName;
            prjTemp.ProjectID = projectID;
            prjTemp.OptionMetal = optionMetal;
            prjTemp.Memo = memo;
            prjTemp.Reg_dt = projectReg_dt;

            //2025-02-05 soonchol
            if (rootJObj["YarnDyed"] == null || rootJObj["YarnDyed"].ToString() == null || rootJObj["YarnDyed"].ToString() == "")
            {
                prjTemp.YardDyed = true;
            }
            else
            {
                prjTemp.YardDyed = (rootJObj["YarnDyed"].ToString() == "Y");
            }

            if (rootJObj["DyeColor"] == null)
            {
                if (prjTemp.YardDyed ==  true)
                {
                    prjTemp.DyeColor = Default_DyeColor;
                }
                else
                {
                    prjTemp.DyeColor = null;
                }
            }
            else
            {
                prjTemp.DyeColor = rootJObj["DyeColor"].ToString();
            }

            ///////////////////////////////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////
            //JObject objBInfo = (JObject)rootJObj["BasicInfo"];
            //Trace.WriteLine(pattern.ToString());
            //string itemName = objBInfo["Name"].ToString();
            //string projectID = objBInfo["ProjectID"].ToString();
            //string createDate = objBInfo["CreateDate"].ToString();

            //BasicInfo binfo = new BasicInfo();
            //binfo.ItemName = itemName;
            //binfo.ProjectID = projectID;
            //binfo.CreateDate = createDate;

            ///////////////////////////////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////
            JObject objPattern = (JObject)rootJObj["Pattern"];
            //Trace.WriteLine(pattern.ToString());
            int idx = Convert.ToInt32(objPattern["Idx"].ToString());
            string name = objPattern["Name"].ToString();
            int Col = Convert.ToInt32(objPattern["XLength"].ToString());
            int Row = Convert.ToInt32(objPattern["YLength"].ToString());
            int[,] data = new int[Row, Col];
            JArray dataObj = (JArray)objPattern["Data"];
            for (int i = 0; i < dataObj.Count; i++)
            {
                JArray temp = (JArray)dataObj[i];
                for (int j = 0; j < temp.Count; j++)
                {
                    data[i, j] = Convert.ToInt32(temp[j].ToString());
                }
            }

            Pattern pattern = new Pattern();
            pattern.Idx = idx;
            pattern.Name = name;
            pattern.Data = data;

            ///////////////////////////////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////
            JObject jsonWarp = (JObject)rootJObj["Warp"];
            int warpCount = Convert.ToInt32(jsonWarp["WarpCount"].ToString());

            Warp warp = new Warp();

            WInfo warpInfo = null;

            warp.Density = Util.JObjectToInt(jsonWarp, "Density", 50);

            JArray jsonWarpInfoList = (JArray)jsonWarp["WarpInfoList"];
            for (int i = 0; i < jsonWarpInfoList.Count; i++)
            {
                JObject jObj = (JObject)jsonWarpInfoList[i];

                warpInfo = new WInfo();
                warpInfo.Idx = Convert.ToInt32(jObj["Idx"].ToString());
                warpInfo.Name = jObj["Name"].ToString();
                warpInfo.IdxYarn = Util.JObjectToInt(jObj, "IdxYarn");
                warpInfo.HexColor = jObj["HexColor"].ToString();

                warp.AddWInfo(warpInfo);
            }
            if (jsonWarpInfoList.Count <= 0)
            {
                warpInfo = new WInfo();
                warpInfo.Idx = 0;
                warpInfo.Name = "";
                warpInfo.IdxYarn = 0;
                warpInfo.HexColor = "FFFFFF";

                warp.AddWInfo(warpInfo);
            }

            // 
            JArray jsonWarpArray = (JArray)jsonWarp["WarpArray"];
            List<WArray> listWarpArray = new List<WArray>();
            for (int i = 0; i < jsonWarpArray.Count; i++)
            {
                JObject jObj = (JObject)jsonWarpArray[i];

                WArray info = new WArray();
                info.Idx = Convert.ToInt32(jObj["Idx"].ToString());
                info.Count = Convert.ToInt32(jObj["Count"].ToString());

                listWarpArray.Add(info);
            }
            warp.SetWArrayList(listWarpArray);


            // 
            JArray jsonWarpRepeat = (JArray)jsonWarp["Repeat"];
            List<WRepeat> listWarpRepeat = new List<WRepeat>();
            if (jsonWarpRepeat != null)
            {
                for (int i = 0; i < jsonWarpRepeat.Count; i++)
                {
                    JObject jObj = (JObject)jsonWarpRepeat[i];

                    WRepeat info = new WRepeat();
                    info.StartIdx = Convert.ToInt32(jObj["StartIdx"].ToString());
                    info.EndIdx = Convert.ToInt32(jObj["EndIdx"].ToString());
                    info.RepeatCnt = Convert.ToInt32(jObj["RepeatCnt"].ToString());

                    listWarpRepeat.Add(info);
                }
            }
            warp.SetWRepeatList(listWarpRepeat);


            ///////////////////////////////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////
            JObject jsonWeft = (JObject)rootJObj["Weft"];
            int weftCount = Convert.ToInt32(jsonWeft["WeftCount"].ToString());

            Weft weft = new Weft();

            weft.Density = Util.JObjectToInt(jsonWeft, "Density", 50);

            JArray jsonWeftInfoList = (JArray)jsonWeft["WeftInfoList"];
            for (int i = 0; i < jsonWeftInfoList.Count; i++)
            {
                JObject jObj = (JObject)jsonWeftInfoList[i];

                WInfo weftInfo = new WInfo();
                weftInfo.Idx = Convert.ToInt32(jObj["Idx"].ToString());
                weftInfo.Name = jObj["Name"].ToString();
                weftInfo.IdxYarn = Util.JObjectToInt(jObj, "IdxYarn");
                weftInfo.HexColor = jObj["HexColor"].ToString();

                weft.AddWInfo(weftInfo);
            }
            if (jsonWarpInfoList.Count <= 0)
            {
                WInfo weftInfo = new WInfo();
                weftInfo.Idx = 0;
                weftInfo.Name = "";
                weftInfo.IdxYarn = 0;
                weftInfo.HexColor = "FFFFFF";

                weft.AddWInfo(weftInfo);
            }

            // 
            JArray jsonWeftArray = (JArray)jsonWeft["WeftArray"];
            List<WArray> listWeftArray = new List<WArray>();
            for (int i = 0; i < jsonWeftArray.Count; i++)
            {
                JObject jObj = (JObject)jsonWeftArray[i];

                WArray info = new WArray();
                info.Idx = Convert.ToInt32(jObj["Idx"].ToString());
                info.Count = Convert.ToInt32(jObj["Count"].ToString());

                listWeftArray.Add(info);
            }
            weft.SetWArrayList(listWeftArray);


            // 
            JArray jsonWeftRepeat = (JArray)jsonWeft["Repeat"];
            List<WRepeat> listWeftRepeat = new List<WRepeat>();
            if (jsonWeftRepeat != null)
            {
                for (int i = 0; i < jsonWeftRepeat.Count; i++)
                {
                    JObject jObj = (JObject)jsonWeftRepeat[i];

                    WRepeat info = new WRepeat();
                    info.StartIdx = Convert.ToInt32(jObj["StartIdx"].ToString());
                    info.EndIdx = Convert.ToInt32(jObj["EndIdx"].ToString());
                    info.RepeatCnt = Convert.ToInt32(jObj["RepeatCnt"].ToString());

                    listWeftRepeat.Add(info);
                }
            }
            weft.SetWRepeatList(listWeftRepeat);


            ///////////////////////////////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////
            JObject objPhysicalProperty = (JObject)rootJObj["PhysicalProperty"];
            //Trace.WriteLine(pattern.ToString());
            int BendingWarp = Convert.ToInt32(objPhysicalProperty["BendingWarp"].ToString());
            int BendingWeft = Convert.ToInt32(objPhysicalProperty["BendingWeft"].ToString());
            int InternalDamping = Convert.ToInt32(objPhysicalProperty["InternalDamping"].ToString());
            int Friction = Convert.ToInt32(objPhysicalProperty["Friction"].ToString());
            int Density = Convert.ToInt32(objPhysicalProperty["Density"].ToString());
            int StretchWeft = Convert.ToInt32(objPhysicalProperty["StretchWeft"].ToString());
            int StretchWarp = Convert.ToInt32(objPhysicalProperty["StretchWarp"].ToString());
            int BucklingStiffnessWeft = Convert.ToInt32(objPhysicalProperty["BucklingStiffnessWeft"].ToString());
            int BucklingStiffnessWarp = Convert.ToInt32(objPhysicalProperty["BucklingStiffnessWarp"].ToString());

            PhysicalProperty pProperty = new PhysicalProperty();
            pProperty.BendingWarp = BendingWarp;
            pProperty.BendingWeft = BendingWeft;
            pProperty.InternalDamping = InternalDamping;
            pProperty.Friction = Friction;
            pProperty.Density = Density;
            pProperty.StretchWeft = StretchWeft;
            pProperty.StretchWarp = StretchWarp;
            pProperty.BucklingStiffnessWeft = BucklingStiffnessWeft;
            pProperty.BucklingStiffnessWarp = BucklingStiffnessWarp;



            //weaveData.BasicInfo = binfo;
            prjTemp.Pattern = pattern;
            prjTemp.Warp = warp;
            prjTemp.Weft = weft;
            prjTemp.PhysicalProperty = pProperty;

            return prjTemp;
        }
        public static string ParseJson(ProjectData weaveData)
        {
            if (weaveData == null)
            {
                return "";
            }

            //BasicInfo binfo = weaveData.BasicInfo;
            Pattern pattern = weaveData.Pattern;
            Warp warp = weaveData.Warp;
            Weft weft = weaveData.Weft;
            PhysicalProperty pProperty = weaveData.PhysicalProperty;

            string json = "";

            ///////////////////////////////////////////////////////////////////
            // Project
            ///////////////////////////////////////////////////////////////////
            string jsonProject = "";
            string projectName = weaveData.Name;
            string projectID = weaveData.ProjectID;
            string optionMetal = weaveData.OptionMetal;
            string memo = Util.Base64Encode(weaveData.Memo);
            string projectReg_dt = weaveData.Reg_dt;
            //2025-02-05 soonchol
            string yarnDyed = weaveData.YardDyed ? "Y" : "N";
            string dyeColor = weaveData.DyeColor;

            jsonProject = "   " +
                "   \"Name\":\"" + projectName + "\"," +
                "   \"ProjectID\":\"" + projectID + "\"," +
                "   \"OptionMetal\":\"" + optionMetal + "\"," +
                "   \"Memo\":\"" + memo + "\"," +
                "   \"YarnDyed\":\"" + yarnDyed + "\"," +
                "   \"DyeColor\":\"" + dyeColor + "\"," +
                "   \"Reg_dt\":\"" + projectReg_dt + "\" " +
                "";

            ///////////////////////////////////////////////////////////////////
            // BasicInfo
            ///////////////////////////////////////////////////////////////////
            /*
            string jsonBinfo = "";
            if (binfo != null)
            {
                string name = binfo.ItemName;
                projectID = binfo.ProjectID;
                string reg_dt = binfo.CreateDate;

                jsonBinfo = "   " +
                    "\"BasicInfo\":{" +
                    "   \"Name\":\"" + name + "\"," +
                    "   \"ProjectID\":\"" + projectID + "\"," +
                    "   \"CreateDate\":\"" + reg_dt + "\" " +
                    "}";
            }
            */


            ///////////////////////////////////////////////////////////////////
            // Pattern
            ///////////////////////////////////////////////////////////////////
            string jsonPattern = "";
            if (pattern != null)
            {
                int idx = pattern.Idx;
                string name = pattern.Name;
                int Col = pattern.XLength;
                int Row = pattern.YLength;
                int[,] Data = pattern.Data;

                string strData = "";
                for (int i = 0; i < Data.GetLength(0); i++)
                {
                    string temp = "";
                    for (int j = 0; j < Data.GetLength(1); j++)
                    {
                        if (j > 0)
                        {
                            temp = temp + ",";
                        }
                        temp = temp + Data[i, j];
                    }
                    temp = "[" + temp + "]";

                    if (i > 0)
                    {
                        strData = strData + ",";
                    }
                    strData = strData + temp;
                }
                strData = "[" + strData + "]";

                jsonPattern = "" +
                    "," +
                    "\"Pattern\":{" +
                    "   \"Idx\":" + idx + "," +
                    "   \"Name\":\"" + name + "\"," +
                    "   \"XLength\":" + Col + "," +
                    "   \"YLength\":" + Row + "," +
                    "   \"Data\":" + strData +
                    "}";
            }


            ///////////////////////////////////////////////////////////////////
            // Warp
            ///////////////////////////////////////////////////////////////////
            string jsonWarp = "";
            if (warp != null)
            {
                int density = warp.Density;
                int warpCount = warp.GetWInfoLength();
                List<WInfo> listWarpInfo = warp.GetWInfoList();
                List<WArray> listWarpArrayInfo = warp.GetWArrayList();
                List<WRepeat> listRepeat = warp.GetWRepeatList();

                string strWarpInfoList = "";
                for (int i = 0; i < listWarpInfo.Count; i++)
                {
                    WInfo warpInfo = listWarpInfo[i];
                    int idx = warpInfo.Idx;
                    string name = warpInfo.Name;
                    string hexColor = warpInfo.HexColor;
                    int idxYarn = warpInfo.IdxYarn;

                    string temp = "{" +
                        "   \"Idx\":" + idx + "," +
                        "   \"Name\":\"" + name + "\"," +
                        "   \"IdxYarn\":\"" + idxYarn + "\"," +
                        "   \"HexColor\":\"" + hexColor + "\"" +
                        "}";

                    if (i > 0)
                    {
                        strWarpInfoList = strWarpInfoList + ",";
                    }
                    strWarpInfoList = strWarpInfoList + temp;
                }
                strWarpInfoList = "[" + strWarpInfoList + "]";

                string strArray = "";
                for (int i = 0; i < listWarpArrayInfo.Count; i++)
                {
                    WArray info = listWarpArrayInfo[i];
                    int idx = info.Idx;
                    int cnt = info.Count;
                    string temp = "{" +
                        "   \"Idx\":" + idx + "," +
                        "   \"Count\":" + cnt +
                        "}";
                    if (i > 0)
                    {
                        strArray = strArray + ",";
                    }
                    strArray = strArray + temp;
                }
                strArray = "[" + strArray + "]";

                string strRepeat = "";
                for (int i = 0; i < listRepeat.Count; i++)
                {
                    WRepeat repeat = listRepeat[i];
                    int sidx = repeat.StartIdx;
                    int eidx = repeat.EndIdx;
                    int cnt = repeat.RepeatCnt;
                    string temp = "{" +
                        "   \"StartIdx\":" + sidx + "," +
                        "   \"EndIdx\":" + eidx + "," +
                        "   \"RepeatCnt\":" + cnt +
                        "}";
                    if (i > 0)
                    {
                        strRepeat = strRepeat + ",";
                    }
                    strRepeat = strRepeat + temp;
                }
                strRepeat = "[" + strRepeat + "]";


                jsonWarp = "" +
                    "," +
                    "\"Warp\":{" +
                    "   \"Density\":" + density + "," +
                    "   \"WarpCount\":2," +
                    "   \"WarpInfoList\": " + strWarpInfoList + "," +
                    "   \"WarpArray\":" + strArray + "," +
                    "   \"Repeat\":" + strRepeat + "" +
                    "}";
            }


            ///////////////////////////////////////////////////////////////////
            // Weft
            ///////////////////////////////////////////////////////////////////
            string jsonWeft = "";
            if (weft != null)
            {
                int density = weft.Density;
                int weftCount = weft.GetWInfoLength();
                List<WInfo> listWeftInfo = weft.GetWInfoList();
                List<WArray> listWeftArrayInfo = weft.GetWArrayList();
                List<WRepeat> listRepeat = weft.GetWRepeatList();

                string strWeftInfoList = "";
                for (int i = 0; i < listWeftInfo.Count; i++)
                {
                    WInfo weftInfo = listWeftInfo[i];
                    int idx = weftInfo.Idx;
                    string name = weftInfo.Name;
                    string hexColor = weftInfo.HexColor;
                    int idxYarn = weftInfo.IdxYarn;

                    string temp = "{" +
                        "   \"Idx\":" + idx + "," +
                        "   \"Name\":\"" + name + "\"," +
                        "   \"IdxYarn\":\"" + idxYarn + "\"," +
                        "   \"HexColor\":\"" + hexColor + "\"" +
                        "}";

                    if (i > 0)
                    {
                        strWeftInfoList = strWeftInfoList + ",";
                    }
                    strWeftInfoList = strWeftInfoList + temp;
                }
                strWeftInfoList = "[" + strWeftInfoList + "]";


                string strArray = "";
                for (int i = 0; i < listWeftArrayInfo.Count; i++)
                {
                    WArray info = listWeftArrayInfo[i];
                    int idx = info.Idx;
                    int cnt = info.Count;
                    string temp = "{" +
                        "   \"Idx\":" + idx + "," +
                        "   \"Count\":" + cnt +
                        "}";
                    if (i > 0)
                    {
                        strArray = strArray + ",";
                    }
                    strArray = strArray + temp;
                }
                strArray = "[" + strArray + "]";



                string strRepeat = "";
                for (int i = 0; i < listRepeat.Count; i++)
                {
                    WRepeat repeat = listRepeat[i];
                    int sidx = repeat.StartIdx;
                    int eidx = repeat.EndIdx;
                    int cnt = repeat.RepeatCnt;
                    string temp = "{" +
                        "   \"StartIdx\":" + sidx + "," +
                        "   \"EndIdx\":" + eidx + "," +
                        "   \"RepeatCnt\":" + cnt +
                        "}";
                    if (i > 0)
                    {
                        strRepeat = strRepeat + ",";
                    }
                    strRepeat = strRepeat + temp;
                }
                strRepeat = "[" + strRepeat + "]";

                jsonWeft = "" +
                    "," +
                    "\"Weft\":{" +
                    "   \"Density\":" + density + "," +
                    "   \"WeftCount\":2," +
                    "   \"WeftInfoList\": " + strWeftInfoList + "," +
                    "   \"WeftArray\":" + strArray + "," +
                    "   \"Repeat\":" + strRepeat + "" +
                    "}";
            }

            ///////////////////////////////////////////////////////////////////
            // PhysicalProperty
            ///////////////////////////////////////////////////////////////////
            string jsonPhysicalProperty = "";
            if (pProperty != null)
            {
                int bendingWeft = pProperty.BendingWeft;
                int bendingWarp = pProperty.BendingWarp;
                int internalDamping = pProperty.InternalDamping;
                int friction = pProperty.Friction;
                int density = pProperty.Density;
                int stretchWeft = pProperty.StretchWeft;
                int stretchWarp = pProperty.StretchWarp;
                int bucklingStiffnessWeft = pProperty.BucklingStiffnessWeft;
                int bucklingStiffnessWarp = pProperty.BucklingStiffnessWarp;

                jsonPhysicalProperty = "" +
                    "," +
                    "\"PhysicalProperty\":{" +
                    "   \"BendingWeft\":" + bendingWeft + "," +
                    "   \"BendingWarp\":" + bendingWarp + "," +
                    "   \"InternalDamping\":" + internalDamping + "," +
                    "   \"Friction\":" + friction + "," +
                    "   \"Density\":" + density + "," +
                    "   \"StretchWeft\":" + stretchWeft + "," +
                    "   \"StretchWarp\":" + stretchWarp + "," +
                    "   \"BucklingStiffnessWeft\":" + bucklingStiffnessWeft + "," +
                    "   \"BucklingStiffnessWarp\":" + bucklingStiffnessWarp + " " +
                    "}";
            }

            json = "{" +
                jsonProject +
                //jsonBinfo +
                jsonPattern +
                jsonWarp +
                jsonWeft +
                jsonPhysicalProperty +
                "}";
            //Trace.WriteLine("SAVE====================>\r\n" + json);
            return json;
        }

        //---------------------------------------------------------------------
        public string CreateDefaultJsonProjectData(string name)
        {
            string path = Path.Combine(Application.StartupPath, "Resource", "json", "default_project.json");
            string jsonTemplate = File.ReadAllText(path, Encoding.UTF8);

            var project = JsonConvert.DeserializeObject<ProjectData>(jsonTemplate);
            project.Name = name;
            project.ProjectID = Util.GenerateUUID();
            project.Memo = Util.Base64Encode("중량:\r\n폭:\r\n혼용률:\r\n");
            project.DyeColor = Default_DyeColor;
            project.Reg_dt = DateTime.Now.ToString("yyyyMMddhhmmss");
            
            return JsonConvert.SerializeObject(project, Formatting.Indented);
        }
        //---------------------------------------------------------------------
        public ProjectData CreateDefaultProjectData(string name)
        {
            string json = this.CreateDefaultJsonProjectData(name);
            return ParseProjectData(json);
        }
        //---------------------------------------------------------------------

        ///////////////////////////////////////////////////////////////////////
        // 시작 - Thread
        ///////////////////////////////////////////////////////////////////////
        Thread threadInitview = null;
        int tempRunIdx = -1;
        ProjectData tempRunWData;

        // 프로젝트 첫 로딩에 데이터 셋팅
        public void ThreadInitProject()
        {
            threadInitview = new Thread(new ThreadStart(ThreadRunInitProject));
            threadInitview.Start();
        }
        private void ThreadRunInitProject()
        {
            while (IsLoaded3DObject == false)
            {
                Thread.Sleep(1000);
            }
            if (this.InvokeRequired) // 현재 스레드가 this(Form1) 요소를 만든 스레드를 경유해야 하는지 확인
            {
                this.BeginInvoke(
                    (System.Action)(() =>
                    {
                        SetProjectData(tempRunIdx, tempRunWData);
                        CloseProgressForm();
                    }));
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// motify by ilkwon 2025.04.23 기존의 코드는 닷넷5로 작성되었으나 
        /// ms의 지원이 끝난관계로  닷넷5에서 닷넷8로 업그레이드.
        /// 닷넷 5이후 Thread  처리시 Abort는 금지되었다.
        /// NET Core 및 .NET 5 이상에서 아예 지원되지 않음.
        /// 안전쓰레드 : Cancel Token 발행 -> Join -> Dispose
        /// </summary>
        private Thread threadLoadedObject;
        private CancellationTokenSource ctsLoadedObject;
        public void Thread3DViewerLoadedObject()
        {
            if (threadLoadedObject != null && threadLoadedObject.IsAlive)
                return; // 이미 실행 중이면 다시 실행하지 않음

            if (ctsLoadedObject != null)
            {
                ctsLoadedObject.Cancel();
                threadLoadedObject?.Join();
                ctsLoadedObject.Dispose();
                ctsLoadedObject = null;
            }
            
            ctsLoadedObject = new CancellationTokenSource(); // 새 토큰 생성
            threadLoadedObject = new Thread(() => ThreadRunLoadedObject(ctsLoadedObject.Token));
            threadLoadedObject.Start();
        }

        //---------------------------------------------------------------------
        private void ThreadRunLoadedObject(CancellationToken token)
        {
            while (!token.IsCancellationRequested && IsLoaded3DObject == false)
            {                
                Thread.Sleep(1000);
                try
                {
                    var task = weave3DViewer.EvaluateScriptAsync("IsLoadedObject();");
                    task.Wait(token);
                    JavascriptResponse response = task.Result;
                    if (!string.IsNullOrEmpty(response.Result.ToString()))
                    {
                        IsLoaded3DObject = true;
                        //Trace.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>> Loaded Object : " + response.Result.ToString());
                        if (this.InvokeRequired) // 현재 스레드가 this(Form1) 요소를 만든 스레드를 경유해야 하는지 확인
                        {
                            this.BeginInvoke(
                                (System.Action)(() =>
                                {
                                    CloseProgressForm();
                                    if (prjList.Count <= 0)
                                    {
                                        DialogNewProject dialog = new DialogNewProject(this);
                                        dialog.StartPosition = FormStartPosition.Manual;
                                        dialog.Location = GetChildFormLocation();
                                        dialog.dialogNewProjectEventHandler += new DialogNewProjectEventHandler(EventNewProject);
                                        dialog.ShowDialog();
                                    }

                                    // ❗ 단 한 번만 호출됨
                                    weave3DViewer.ExecuteScriptAsync($"ReloadMap2('{SELECTED_IDX}', '{nCall++}');");
                                }));
                        }

                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    // 정상적인 Cancel
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ThreadRunLoadedObject] 예외 발생: " + ex.Message);
                    break; // 반복 중단
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////
        // 끝 - Thread
        ///////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////////////////
        // 시작 - http 서버
        ///////////////////////////////////////////////////////////////////////
        private WebServer server = null;
        private bool IsLoaded3DObject = false;
        private void InitChromiumWebBrowser()
        {
            try
            {
                if (this.server != null && this.server.IsRunning)
                {
                    this.server.Stop();
                    this.server = null;
                }
                this.server = new WebServer();
                this.server.AddBindingAddress("http://localhost:9999/"); // 포트 번호까지 반드시 설정합니다.
                this.server.RootPath = WWWROOT_PATH;
                this.server.ActionRequested += server_ActionRequested;

                this.server.Start();


                weave3DViewer.LoadingStateChanged += BrowserLoadingStateChanged;
                weave3DViewer.Load("http://localhost:9999/viewer.html");
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
            }
        }
        private void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                //Trace.WriteLine(">>>>>>>>>>>>>>>>>>>>> BrowserLoadingStateChanged : is Loaded Viewer ");
                Thread3DViewerLoadedObject();
            }
        }
        private void server_ActionRequested(object sender, ActionRequestedEventArgs e)
        {
            e.Server.WriteDefaultAction(e.Context);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }
        ///////////////////////////////////////////////////////////////////////
        // 끝 - http 서버
        ///////////////////////////////////////////////////////////////////////





        public static bool isNumber(string strValue)
        {
            return Regex.IsMatch(strValue, @"[-+]?\d*\.?\d+");
        }






        public bool ResizeImageFile(string pathSrc, string pathDest)
        {
            int newWidth = 0;
            int newHeight = 0;
            try
            {
                byte[] byteArr = File.ReadAllBytes(pathSrc);

                using (var stream = new System.IO.MemoryStream(byteArr))
                {
                    Bitmap bitmap = new Bitmap(stream);

                    ///////////////////////////////////////////////////////////
                    // 100 px * 100 px , 96 dpi 는 인쇄시 약 1인치에 해당함
                    // 생성된 이미지는 600 px 을 1인치로 가정하여 생성
                    // 1/6 로 축소하면 실제 1인치가 됨
                    ///////////////////////////////////////////////////////////
                    newWidth = bitmap.Width / 6;
                    newHeight = bitmap.Height / 6;

                    bitmap = ResizeImage(bitmap, newWidth, newHeight);

                    int nX = 1;
                    int nY = 1;


                    ///////////////////////////////////////////////////////////
                    // 인쇄 페이지에 가득차게 바둑판 형식으로 그림
                    ///////////////////////////////////////////////////////////
                    if (newWidth < 600)
                    {
                        nX = (int)(600 / newWidth);
                    }
                    if (newHeight < 600)
                    {
                        nY = (int)(600 / newHeight);
                    }


                    Bitmap bm = new Bitmap(newWidth * nX, newHeight * nY);
                    bitmap.SetResolution(96.0F, 96.0F);


                    Graphics g = Graphics.FromImage(bm);

                    System.Drawing.Rectangle rectSrc, rectDest;

                    for (int i = 0; i < nY; i++)
                    {
                        for (int j = 0; j < nX; j++)
                        {
                            rectSrc = new System.Drawing.Rectangle(
                                0,
                                0,
                                bitmap.Width,
                                bitmap.Height);
                            rectDest = new System.Drawing.Rectangle(
                                j * bitmap.Width,
                                i * bitmap.Height,
                                bitmap.Width,
                                bitmap.Height);
                            g.DrawImage(bitmap, rectDest, rectSrc, GraphicsUnit.Pixel);
                        }
                    }

                    //bitmap.Save("C:\\\\Users\\\\user1\\\\Desktop\\\\test_dest_dhkim.png");
                    //bitmap.Dispose();
                    Trace.WriteLine("pathDest : " + pathDest);
                    bm.Save(pathDest, ImageFormat.Png);
                    bm.Dispose();
                    bitmap.Dispose();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        public Point GetChildFormLocation()
        {
            Point p = new Point(this.Location.X + 100, this.Location.Y + 100);
            return p;
        }
    }


    public class FormFile
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FilePath { get; set; }
        public Stream Stream { get; set; }
    }
}
