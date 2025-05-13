using CefSharp;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.DataAccess.Json;

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
using System.Threading.Tasks;
using System.Windows.Forms;
using WeavingGenerator.ProjectDatas;
using WeavingGenerator.Views;

//using WeavingGenerator.ViewModels;
namespace WeavingGenerator
{
  public partial class MainForm : DevExpress.XtraEditors.XtraForm
  {
    public static String Default_DyeColor = "#255,255,255,255";
    /// DB URL
    private DBConn db;
    private DialogConfirmSave dialogSave;
    bool IsModified = false;

    ///////////////////////////////////////////////////////////////////////
    /// 패턴 목록 (json 파일에서 읽음)
    ///////////////////////////////////////////////////////////////////////
    //2025-01-22 soonchol (protected -> public)
    public List<Pattern> patternList = new List<Pattern>();


    public static string WWWROOT_PATH = System.Windows.Forms.Application.StartupPath + "\\wwwroot";
    public static string CLO_PATH = System.Windows.Forms.Application.StartupPath + "\\clo";

    //public static string UPLOAD_URL = "http://127.0.0.1:8080/upload";
    //public static string VIEWER_URL = "http://127.0.0.1:8080/viewer";

    public static string UPLOAD_URL = "https://211.38.52.16/upload";
    public static string VIEWER_URL = "https://211.38.52.16/viewer";

    private ProjectData _projectData;  // 선택된 프로젝트
    private WeaveViewer _weave2DViewer = null;
    private Task taskLoadedObject; // ⭐ 추가
    private ProjectDataView _dataView;
    public ProjectController ProjectCtrl => Controllers.Instance.CurrentProjectController;    
    //-----------------------------------------------------------------------
    public MainForm()
    {
      //WindowsFormsSettings.DefaultFont = new System.Dawing.Font("맑은고딕", 10);
      this.Font = new System.Drawing.Font("맑은고딕", 11);

      // DB 세팅
      db = DBConn.Instance;

      InitializeComponent();

      this.FormClosing += MainForm_FormClosing;
    }

    //-----------------------------------------------------------------------       
    private void InitPropertyView()
    {
      _dataView = new ProjectDataView();

      // 필드에 있는 프로젝트 데이터 로딩
      _projectData = ProjectCtrl.GetProjectData();

      // layoutControl_Property는 DevExpress의 LayoutControl 객체라고 가정
      _dataView.BuildLayout(
        layoutControl_Property,
        CreatePadding,        // Func<int, Padding>
        PADDING_TOP_ITEM,     // int
        PADDING_TOP_GROUP,    // int
        Group_CustomDraw,     // EventHandler<ItemCustomDrawEventArgs>
        null //new EventHandlers
        //{
        //  TextEdit_Name_Changed = TextEdit_Name_Changed,
        //  ComboBoxEdit_Scale_Changed = ComboBoxEdit_Scale_Changed,
        //  Check_YarnDyed_Changed = Check_YarnDyed_Changed
        //}
      );

      _dataView.SetProjectData(_projectData);

      // 그런 다음 이벤트 연결
      _dataView.textEdit_Name.TextChanged += TextEdit_Name_Changed;
      _dataView.comboBoxEdit_Scale.SelectedIndexChanged += ComboBoxEdit_Scale_Changed;
      _dataView.checkEdit_YarnDyed.CheckedChanged += Check_YarnDyed_Changed;
      _dataView.colorEdit_DyeColor.Click += ColorEdit_DyeColor_Click;
    }

    //-----------------------------------------------------------------------
    private void ColorEdit_DyeColor_Click(object sender, EventArgs e)
    {
      if (_dataView.checkEdit_YarnDyed.Checked == true) return;

      ColorEdit colorEdit = (ColorEdit)sender;
      Color oldColor = colorEdit.Color;
      Color newColor = DoShowColorDialog(oldColor);
      colorEdit.Color = newColor;

      //if (chk.Checked == false) return;
      //patternViewer1.SetYarnImage(chk.Checked);

      ProjectData proj = ProjectCtrl.GetProjectData();      
      if (proj != null)
      {
        proj.DyeColor = ProjectData.GetDyeColor(newColor);
        _weave2DViewer.SetYarnDyeColor(proj.DyeColor);
      }

      _weave2DViewer.SetProjectData(ProjectController.SelectedProjectIdx, ProjectCtrl.GetProjectData());
      ThreadViewerRepaint();
    }

    //-----------------------------------------------------------------------
    private void Check_YarnDyed_Changed(object sender, EventArgs e)
    {
      {
        CheckEdit chk = (CheckEdit)sender;
        //if (chk.Checked == false) return;
        //patternViewer1.SetYarnImage(chk.Checked);

        ProjectData proj = ProjectCtrl.GetProjectData();
        
        if (proj != null)
        {
          proj.YarnDyed = chk.Checked;

          if (proj.YarnDyed == true)
          {
            _weave2DViewer.SetYarnDyeColor(null);
          }
          else
          {
            if (proj.DyeColor == null || proj.DyeColor == "")
              proj.DyeColor = Default_DyeColor;

            _weave2DViewer.SetYarnDyeColor(proj.DyeColor);
          }
        }

        _weave2DViewer.SetProjectData(ProjectController.SelectedProjectIdx, ProjectCtrl.GetProjectData());
        ThreadViewerRepaint();
      }
    }

    private void ComboBoxEdit_Scale_Changed(object sender, EventArgs e)
    {
      // 1. 현재 선택된 프로젝트가 유효한지 확인
      if (ProjectController.SelectedProjectIdx < 0)
        return;

      // 2. 현재 프로젝트 데이터 가져오기
      var data = ProjectCtrl.GetProjectData();
      if (data == null) return;

      int oldScale = _weave2DViewer.GetViewScale();
      if (_dataView.comboBoxEdit_Scale.SelectedIndex == (oldScale - 1))
      {
        return;
      }
      // 3. 현재 선택된 스케일 값을 가져와서 저장
      var selected = _dataView.comboBoxEdit_Scale.SelectedItem?.ToString();
      if (!string.IsNullOrEmpty(selected))
      {
        data.Scale = selected;
      }

      ShowProgressForm();

      int viewScale = _dataView.comboBoxEdit_Scale.SelectedIndex + 1;
      _weave2DViewer.SetViewScale(viewScale, isRepaintScale);

      CloseProgressForm();

    }
    //-----------------------------------------------------------------------
    private void TextEdit_Name_Changed(object sender, EventArgs e)
    {
      if (ProjectController.SelectedProjectIdx < 0)
        return;

      ProjectData currentProject = ProjectCtrl.GetProjectData();
      if (currentProject == null) return;

      //Trace.WriteLine("textEdit_Name : " + textEdit_Name.Text);
      if (_dataView?.textEdit_Name == null)
        return;
      
      currentProject.Name = _dataView.textEdit_Name.Text;
      UpdateProjectButton(currentProject.Idx, currentProject.Name);
    }

    //-----------------------------------------------------------------------
    private void MainForm_Load(object sender, EventArgs e)
    {
      ShowProgressForm();

      //string appPath = Application.StartupPath;
      //Trace.WriteLine("Application Path : " + appPath);

      //weave2DViewer = new WeaveViewer(this, pictureBox1);
      _weave2DViewer = new WeaveViewer(this);
      panel1.Controls.Add(_weave2DViewer);

      _weave2DViewer.Location = new System.Drawing.Point(0, 0);
      _weave2DViewer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      _weave2DViewer.Name = "pictureBox1";
      //2025-01-20 soonchol
      //weave2DViewer.Size = new System.Drawing.Size(250, 250);
      _weave2DViewer.Size = new System.Drawing.Size(2 * WeaveViewer.PPI, 2 * WeaveViewer.PPI);
      //weave2DViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      //weave2DViewer.AutoSize = true;
      _weave2DViewer.TabIndex = 0;
      _weave2DViewer.TabStop = false;
      _weave2DViewer.Dock = DockStyle.None;

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
      if (ProjectCtrl.ProjectDataList.Count > 0)
      {
        ProjectData obj = ProjectCtrl.ProjectDataList[0];

        tempRunIdx = obj.Idx;
        tempRunWData = obj;

        StartInitProject(); // ← Task 기반으로 대체
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
      if (ProjectCtrl.ProjectDataList.Count <= 0)
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
    public
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
    //-----------------------------------------------------------------------
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

    //-----------------------------------------------------------------------
    private void CloseProgressForm()
    {
      SplashScreenManager.CloseForm(false);
    }

    //-----------------------------------------------------------------------
    private void InitProjectView()
    {
      layoutControl_Project.BeginUpdate();

      lcgProject = new LayoutControlGroup();
      lcgProject.Text = "직물 프로젝트";
      lcgProject.GroupStyle = DevExpress.Utils.GroupStyle.Title;
      //lcgProject.AppearanceGroup.Font = WindowsFormsSettings.DefaultFont;

      for (int i = 0; i < ProjectCtrl.ProjectDataList.Count; i++)
      {
        ProjectData wData = ProjectCtrl.ProjectDataList[i];

        LayoutControlItem item = lcgProject.AddItem();
        item.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 10, 3);
        item.TextVisible = false;

        var btn = new ProjectButton
        {
          Idx = wData.Idx,
          Name = wData.Name,
          Text = wData.Name
        };
        btn.Appearance.Options.UseBackColor = true;
        btn.GroupIndex = 0;
        btn.Click += btnProjectClick;

        item.Control = btn;
      }

      layoutControl_Project.Root.Add(lcgProject);

      layoutControl_Project.EndUpdate();
    }

    //-----------------------------------------------------------------------
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

      for (int i = 0; i < ProjectCtrl.ProjectDataList.Count; i++)
      {
        ProjectData wData = ProjectCtrl.ProjectDataList[i];

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

    //-----------------------------------------------------------------------
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

    //-----------------------------------------------------------------------
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

    //-----------------------------------------------------------------------
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

    //-----------------------------------------------------------------------
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
    //-----------------------------------------------------------------------
    public DevExpress.XtraLayout.Utils.Padding CreatePadding(int top)
    {
      return CreatePadding(top, 3);
    }
    //-----------------------------------------------------------------------
    public DevExpress.XtraLayout.Utils.Padding CreatePadding(int top, int bottom)
    {
      return new DevExpress.XtraLayout.Utils.Padding(3, 3, top, bottom);
    }
    //-----------------------------------------------------------------------
    private void InitPropertyView_old()
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
    //-----------------------------------------------------------------------
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
      Color c = ColorTranslator.FromHtml("#707070");

      using (SolidBrush brush = new SolidBrush(c))
      {
        e.Cache.FillRectangle(brush, e.Bounds);
      }
      // add ilkwon 25.04.29
      // 필수: DevExpress 컨트롤에서 그리기 완료 표시
      e.Handled = true; // ⭐⭐⭐ 이거 안 넣으면 계속 무한 호출될 수 있음
    }



    ///////////////////////////////////////////////////////////////////////
    // 시작 - 외부 인터페이스
    ///////////////////////////////////////////////////////////////////////

    public void UpdateProjectData()
    {
      ProjectData obj = ProjectCtrl.GetProjectData();
      ProjectData.DAO.Update(ProjectController.SelectedProjectIdx, obj);
      //SetWeaveViewer(SELECTED_IDX, obj);
      SetProjectData(ProjectController.SelectedProjectIdx, obj);
    }


    public string GetDiffFilePath()
    {
      return _weave2DViewer.GetDiffFilePath();
    }
    public string GetNormFilePath()
    {
      return _weave2DViewer.GetNormFilePath();
    }
    public void ReloadMapWeave3DViewer()
    {
      weave3DViewer.ExecuteScriptAsync("ReloadMap2('" + ProjectController.SelectedProjectIdx + "', '" + (nCall++) + "');");
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
    //---------------------------------------------------------------------
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

    //---------------------------------------------------------------------
    bool isVisibleWarpOfPrint = false;
    public void SetPrintOptionWarpVisible(bool b)
    {
      isVisibleWarpOfPrint = b;
    }

    //---------------------------------------------------------------------
    // 시작 - DAO    
    private void InitDAO()
    {
      try
      {
        AppInitializer.Initialize();

      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
        XtraMessageBox.Show("DB 초기화 중 오류 발생: " + ex.Message);
      }
    }
    //---------------------------------------------------------------------
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
      user_pattern.Data = new int[2, 2] { { 1, 0 }, { 0, 1 } };

      patternList.Add(user_pattern);

      ///////////////////////////////////////////////////////////////////
      // ComboBox 설정 (순서가 바뀌면 이벤트 함수에서 오류)
      ///////////////////////////////////////////////////////////////////
      //comboBoxPattern.DisplayMember = "Name";
      //comboBoxPattern.ValueMember = "Idx";
      //comboBoxPattern.DataSource = patternList;


    }


    ///////////////////////////////////////////////////////////////////////
    /// 저장 프로세스
    ///////////////////////////////////////////////////////////////////////
    private void ResetViewer()
    {
      // 뷰어 초기화
      //patternViewer1.ResetViewer();
      _weave2DViewer.ResetViewer();

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
    //-----------------------------------------------------------------------
    public int CreateProject(string name)
    {
      ProjectData data = ProjectCtrl.CreateDefaultProjectData(name);
      var json = data.SerializeJson();
      int idx = ProjectData.DAO.Insert(name, json);
      data.Idx = idx;

      ProjectCtrl.ProjectDataList.Insert(0, data);

      UpdateProjectView();
      //      SetProjectData(idx, data);

      return idx;
    }
    //-----------------------------------------------------------------------
    public void OpenProject(int idx)
    {
      ProjectData data = ProjectCtrl.GetProjectData(idx);
      SetSelectedProjectButton(idx);
      SetProjectData(idx, data);
    }

    //-----------------------------------------------------------------------
    private void SaveProject()
    {
      IsModified = false;

      string name = textEdit_Name.Text;
      //string optionMetal = comboBoxEdit_OptionMetal.Text;
      string optionMetal = "FD";
      if (string.IsNullOrEmpty(name))
      {
        //오류창
        return;
      }

      ProjectData data = ProjectCtrl.GetProjectData();
      //data.Name = name;
      _dataView.ApplyProjectData(data);
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

      ProjectData.DAO.Update(ProjectController.SelectedProjectIdx, data);

      SaveAllProject();
    }
    //-----------------------------------------------------------------------
    private void SaveAllProject()
    {
      IsModified = false;

      var prjList = ProjectCtrl.ProjectDataList;
      for (int i = 0; i < prjList.Count; i++)
      {
        ProjectData obj = prjList[i];
        int idx = obj.Idx;

        ProjectData.DAO.Update(idx, obj);
      }
    }

    //---------------------------------------------------------------------
    public void RemoveProject(int idx)
    {
      ProjectCtrl.Remove(idx);
      ProjectData.DAO.Delete(idx);
      RemoveProjectButton(idx);

      if (idx == ProjectController.SelectedProjectIdx)
      {
        ProjectController.SelectedProjectIdx = -1;
        ResetViewer();
      }
    }

    //---------------------------------------------------------------------
    public void UpdateProject()
    {
      //wDataList = ListDAOWeavingData();
      UpdateProjectView();
      if (ProjectController.SelectedProjectIdx != -1)
      {
        OpenProject(ProjectController.SelectedProjectIdx);
      }
    }

    //---------------------------------------------------------------------
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

    //---------------------------------------------------------------------
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
        QuitProcess();
      }
    }

    //---------------------------------------------------------------------
    private void QuitProcess()
    {
      try
      {
        // 서버 정리
        if (server != null && server.IsRunning)
        {
          server.Stop();
          server = null;
        }

        // 3D 뷰어 정리
        if (weave3DViewer != null)
        {
          weave3DViewer.Dispose();
          weave3DViewer = null;
        }

        CleanupLoadObjectTask();
        CancelInitProjectView();
        Cef.Shutdown();
      }
      catch (Exception ex)
      {
        Console.WriteLine("ExitApp 예외 발생: " + ex.Message);
      }
      finally
      {
        // 최종적으로 애플리케이션 종료
        System.Windows.Forms.Application.Exit();
      }
    }

    //---------------------------------------------------------------------
    private void CancelInitProjectView()
    {
      if (ctsInitView != null)
      {
        ctsInitView.Cancel(); // 안전하게 취소 요청
        try
        {
          if (taskInitView != null)
          {
            if (!taskInitView.Wait(500)) // 최대 0.5초만 대기
            {
              Console.WriteLine("InitView Task가 제시간에 종료되지 않았습니다.");
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine("InitView Task 종료 대기 중 예외: " + ex.Message);
        }
        finally
        {
          ctsInitView.Dispose();
          ctsInitView = null;
          taskInitView = null;
        }
      }
    }

    //---------------------------------------------------------------------
    private void CleanupLoadObjectTask()
    {
      if (ctsGenerateImage != null)
      {
        ctsGenerateImage.Cancel();

        try
        {
          if (taskGenerateImage != null)
          {
            if (!taskGenerateImage.Wait(300)) // 최대 0.3초만 대기
              Console.WriteLine("이미지 생성 Task가 제시간에 종료되지 않음.");
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine("taskGenerateImage 종료 중 예외: " + ex.Message);
        }

        ctsGenerateImage.Dispose();
        ctsGenerateImage = null;
        taskGenerateImage = null;
      }
    }

    //---------------------------------------------------------------------
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      ExitApp();
    }
    //---------------------------------------------------------------------
    int nCall = 0;
    private Task taskGenerateImage;
    private CancellationTokenSource ctsGenerateImage;

    public void ThreadViewerRepaint()
    {
      if (ctsGenerateImage != null)
      {
        ctsGenerateImage.Cancel();
        ctsGenerateImage.Dispose();
        ctsGenerateImage = null;
      }

      ctsGenerateImage = new CancellationTokenSource();
      var token = ctsGenerateImage.Token;

      taskGenerateImage = Task.Run(() => RunGenerateImageAsync(token), token);

      //Trace.WriteLine("thread_generate START............ ");
    }
    //---------------------------------------------------------------------
    private async Task RunGenerateImageAsync(CancellationToken token)
    {
      if (token.IsCancellationRequested) return;

      if (IsLoaded3DObject)
      {
        // UI 접근 필요 → UI 스레드에서 실행
        await this.InvokeAsync(() =>
        {
          ShowProgressForm();

          // patternViewer1.Generate3DImage(""); // 필요시 복구
          _weave2DViewer.RepaintCanvas();
          weave3DViewer.ExecuteScriptAsync("ReloadMap2('" + ProjectController.SelectedProjectIdx + "', '" + (nCall++) + "');");

          CloseProgressForm();
        });
      }
    }
    ///////////////////////////////////////////////////////////////////////
    // 시작 - 직물 정보 설정 
    ///////////////////////////////////////////////////////////////////////

    public void SetJsonData(string jsonData)
    {
      //ProjectData data = MainForm.ParseProjectData(jsonData);
      ProjectData data = ProjectData.JsonParser.Parse(jsonData);
      SetProjectData(ProjectController.SelectedProjectIdx, data);
    }

    public void SetProjectData(int idx, ProjectData data)
    {
      if (data == null) return;

      ProjectController.SelectedProjectIdx = idx;

      // 프로젝트 버튼 하이라이트
      SetSelectedProjectButton(idx);

      // 기본 정보
      _dataView.textEdit_Name.Text = data.Name;
      _dataView.textEdit_BasicInfoRegDt.Text = Util.ToDateHuman(data.Reg_dt);

      // 경사 정보
 //     spinEdit_WarpDensity.Text = data.Warp.Density.ToString();
 //
 //     // 위사 정보
 //     spinEdit_WeftDensity.Text = data.Weft.Density.ToString();
 //
 //     // 조직 정보
 //     textEdit_Pattern.Text = data.Pattern.Name;
 //
 //     // 물성 정보
 //     textEdit_BendingWeft.Value = data.PhysicalProperty.BendingWeft;
 //     textEdit_BendingWarp.Value = data.PhysicalProperty.BendingWarp;
 //     textEdit_InternalDamping.Value = data.PhysicalProperty.InternalDamping;
 //     textEdit_Friction.Value = data.PhysicalProperty.Friction;
 //     textEdit_Density.Value = data.PhysicalProperty.Density;
 //     textEdit_StretchWeft.Value = data.PhysicalProperty.StretchWeft;
 //     textEdit_StretchWarp.Value = data.PhysicalProperty.StretchWarp;
 //     textEdit_BucklingStiffnessWeft.Value = data.PhysicalProperty.BucklingStiffnessWeft;
 //     textEdit_BucklingStiffnessWarp.Value = data.PhysicalProperty.BucklingStiffnessWarp;
 //
 //     trackBar_BendingWarp.Value = data.PhysicalProperty.BendingWarp;
 //     trackBar_BendingWeft.Value = data.PhysicalProperty.BendingWeft;
 //     trackBar_InternalDamping.Value = data.PhysicalProperty.InternalDamping;
 //     trackBar_Friction.Value = data.PhysicalProperty.Friction;
 //     trackBar_Density.Value = data.PhysicalProperty.Density;
 //     trackBar_StretchWeft.Value = data.PhysicalProperty.StretchWeft;
 //     trackBar_StretchWarp.Value = data.PhysicalProperty.StretchWarp;
 //     trackBar_BucklingStiffnessWeft.Value = data.PhysicalProperty.BucklingStiffnessWeft;
 //     trackBar_BucklingStiffnessWarp.Value = data.PhysicalProperty.BucklingStiffnessWarp;
 //
 //     // 확대 제한 여부 계산
 //     int[] warpArr = data.Warp.GetWArrayInt();
 //     int[] weftArr = data.Weft.GetWArrayInt();
 //     int nWidth = warpArr.Length;
 //     int nHeight = weftArr.Length;
 //     int cnt = nWidth * nHeight;
 //
 //     if (cnt > 50000)
 //     {
 //       //SetFullScale(false);
 //     }
 //     else
 //     {
 //       //SetFullScale(true);
 //     }
 //
 //     // 색상, 선염 여부 (이건 기존 방식 유지)
 //     colorEdit_DyeColor.Color = ProjectData.GetDyeColor(data.DyeColor);
 //     checkEdit_YarnDyed.Checked = data.YarnDyed;
 
      // 2D/3D 뷰어 반영
      SetWeaveViewer(idx, data);
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

    public void SetPattern(int idx)
    {
      Pattern pattern = GetPattern(idx);
      if (pattern == null) return;

      ///////////////////////////////////////////////////////////////////
      //
      ///////////////////////////////////////////////////////////////////
      ProjectData data = ProjectCtrl.GetProjectData();
      if (data == null) return;

      data.Pattern = pattern;

      ///////////////////////////////////////////////////////////////////
      // 컨트롤 설정
      ///////////////////////////////////////////////////////////////////
      textEdit_Pattern.Text = pattern.Name;

      SetWeaveViewer(ProjectController.SelectedProjectIdx, data);
    }
    public void ResetViewScale()
    {
      if (comboBoxEdit_Scale.SelectedIndex == 0)
      {
        return;
      }
      comboBoxEdit_Scale.SelectedIndex = 0;
      _weave2DViewer.SetViewScale(1);
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
      _weave2DViewer.Export2DDImage(fullPath);
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
      string dFile = _weave2DViewer.GetDiffFilePath();
      string nFile = _weave2DViewer.GetNormFilePath();

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

      if (ProjectController.SelectedProjectIdx == btn.Idx) return;
      int idx = btn.Idx;

      ProjectData data = ProjectCtrl.GetProjectData(idx);
      if (data == null) return;

      SetProjectData(idx, data);
    }

    //-----------------------------------------------------------------------
    private void textEdit_Name_TextChanged(object sender, EventArgs e)
    {
      if (ProjectController.SelectedProjectIdx < 0)
      {
        return;
      }
      ProjectData data = ProjectCtrl.GetProjectData();
      if (data == null) return;

      //Trace.WriteLine("textEdit_Name : " + textEdit_Name.Text);
      data.Name = textEdit_Name.Text;
      UpdateProjectButton(ProjectController.SelectedProjectIdx, data.Name);
    }

    private void SpinEdit_WarpDensity_ValueChanged(object sender, EventArgs e)
    {
      if (ProjectController.SelectedProjectIdx < 0)
      {
        return;
      }
      ProjectData data = ProjectCtrl.GetProjectData();
      if (data == null) return;

      Warp warp = data.Warp;
      if (warp == null) return;

      int oldValue = warp.Density;
      int newValue = Util.ToInt(spinEdit_WarpDensity.Text, 50);
      if (oldValue != newValue)
      {
        warp.Density = Util.ToInt(spinEdit_WarpDensity.Text, 50);
        SetWeaveViewer(ProjectController.SelectedProjectIdx, data);
      }
    }
    private void SpinEdit_WeftDensity_ValueChanged(object sender, EventArgs e)
    {
      if (ProjectController.SelectedProjectIdx < 0)
      {
        return;
      }
      ProjectData data = ProjectCtrl.GetProjectData();
      if (data == null) return;

      Weft weft = data.Weft;
      if (weft == null) return;


      int oldValue = weft.Density;
      int newValue = Util.ToInt(spinEdit_WeftDensity.Text, 50);
      if (oldValue != newValue)
      {
        weft.Density = Util.ToInt(spinEdit_WeftDensity.Text, 50);
        SetWeaveViewer(ProjectController.SelectedProjectIdx, data);
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

      int oldScale = _weave2DViewer.GetViewScale();
      if (comboBoxEdit_Scale.SelectedIndex == (oldScale - 1))
      {
        return;
      }

      ShowProgressForm();

      int viewScale = comboBoxEdit_Scale.SelectedIndex + 1;
      _weave2DViewer.SetViewScale(viewScale, isRepaintScale);

      CloseProgressForm();

    }

    private void btnJsonClick(object sender, EventArgs e)
    {
      // 임시
      DialogJsonData dialog = new DialogJsonData(this);

      string json = ProjectCtrl.GetProjectData().SerializeJson();
      dialog.SetJsonData(json);
      dialog.ShowDialog();
    }
    private void btnWarpClick(object sender, EventArgs e)
    {
      DialogWarpInfo dialog = new DialogWarpInfo(this, ProjectCtrl.GetProjectData());
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = GetChildFormLocation();
      dialog.ShowDialog();
    }
    private void btnWeftArrayClick(object sender, EventArgs e)
    {
      DialogWeftArray dialog = new DialogWeftArray(this, ProjectCtrl.GetProjectData());
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = GetChildFormLocation();
      dialog.ShowDialog();
    }
    private void btnWeftClick(object sender, EventArgs e)
    {
      DialogWeftInfo dialog = new DialogWeftInfo(this, ProjectCtrl.GetProjectData());
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = GetChildFormLocation();
      dialog.ShowDialog();
    }
    private void btnWarpArrayClick(object sender, EventArgs e)
    {
      DialogWarpArray dialog = new DialogWarpArray(this, ProjectCtrl.GetProjectData());
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
      _weave2DViewer.SetYarnImage(chk.Checked);
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

      ProjectData wData = ProjectCtrl.GetProjectData();
      //2025-02-05 soonchol
      if (wData != null)
      {
        wData.DyeColor = ProjectData.GetDyeColor(newColor);
        _weave2DViewer.SetYarnDyeColor(wData.DyeColor);
      }

      _weave2DViewer.SetProjectData(ProjectController.SelectedProjectIdx, ProjectCtrl.GetProjectData());
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

      ProjectData wData = ProjectCtrl.GetProjectData();
      //2025-02-05 soonchol
      if (wData != null)
      {
        wData.YarnDyed = chk.Checked;

        if (wData.YarnDyed == true)
        {
          _weave2DViewer.SetYarnDyeColor(null);
        }
        else
        {
          if (wData.DyeColor == null || wData.DyeColor == "")
          {
            wData.DyeColor = Default_DyeColor;
          }

          _weave2DViewer.SetYarnDyeColor(wData.DyeColor);
        }

      }

      _weave2DViewer.SetProjectData(ProjectController.SelectedProjectIdx, ProjectCtrl.GetProjectData());
      ThreadViewerRepaint();
    }

    // menu.new_project
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

    // menu.open
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
      DialogNewYarn dialog = new DialogNewYarn();
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
      DialogDensity dialog = new DialogDensity();
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = GetChildFormLocation();
      dialog.dialogUpdateDensityEventHandler += new DialogUpdateDensityEventHandler(EventUpdateDensity);
      dialog.ShowDialog();
    }
    private void EventUpdateDensity(object sender, int newIdx)
    {
      ProjectData data = ProjectCtrl.GetProjectData(newIdx);
      if (data == null) return;

      spinEdit_WarpDensity.Text = data.Warp.Density.ToString();
      spinEdit_WeftDensity.Text = data.Weft.Density.ToString();
      SetWeaveViewer(newIdx, data);
    }
    private void barButtonItem_OpenWarp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      DialogWarpInfo dialog = new DialogWarpInfo(this, ProjectCtrl.GetProjectData());
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = GetChildFormLocation();
      dialog.ShowDialog();
    }

    private void barButtonItem_OpenWarpArray_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      DialogWarpArray dialog = new DialogWarpArray(this, ProjectCtrl.GetProjectData());
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = GetChildFormLocation();
      dialog.ShowDialog();
    }

    private void barButtonItem_OpenWeft_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      DialogWeftInfo dialog = new DialogWeftInfo(this, ProjectCtrl.GetProjectData());
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = GetChildFormLocation();
      dialog.ShowDialog();
    }

    private void barButtonItem_OpenWeftArray_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      DialogWeftArray dialog = new DialogWeftArray(this, ProjectCtrl.GetProjectData());
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

      string diffFilePath = MainForm.WWWROOT_PATH + "\\diff_" + ProjectController.SelectedProjectIdx + ".png";
      string printFilePath = MainForm.WWWROOT_PATH + "\\diff_" + ProjectController.SelectedProjectIdx + "_Print.png";

      // 100 px * 100 px , 96 dpi 는 인쇄시 약 1인치에 해당함
      ProjectData weaveData = ProjectCtrl.GetProjectData();

      ResizeImageFile(diffFilePath, printFilePath);


      List<Yarn> yarnList = Yarn.DAO.SelectAll();

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

    //-----------------------------------------------------------------------
    private void barButtonItem_Link_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      ProjectData data = ProjectCtrl.GetProjectData();
      if (data == null)
      {
        XtraMessageBox.Show("프로젝트 생성 후 이용해주세요..", "Error");
        return;
      }
      string appid = AppSetting.GetAppId();
      string projectid = data.ProjectID;

      string url = VIEWER_URL + "?APPID=" + appid + "&PROJECTID=" + projectid;
      System.Diagnostics.Process.Start(url);
    }

    //-----------------------------------------------------------------------
    private void barButtonItem_CloFabric_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      /*
      string cFile = MainForm.CLO_PATH + "\\FABRIC.fab";
      //XtraMessageBox.Show(aFile, "Error");
      string dFile = _weave2DViewer.GetDiffFilePath();
      string nFile = _weave2DViewer.GetNormFilePath();

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

    //-----------------------------------------------------------------------
    private void trackBarControl_EditValueChanged(object sender, EventArgs e)
    {
      TrackBarControl trackBarControl = sender as TrackBarControl;
      string name = trackBarControl.Name;
      int value = Util.ToInt(trackBarControl.EditValue.ToString());

      switch (name)
      {
        case "BendingWeft": textEdit_BendingWeft.Value = value; break;
        case "BendingWarp": textEdit_BendingWarp.Value = value; break;
        case "InternalDamping": textEdit_InternalDamping.Value = value; break;
        case "Friction": textEdit_Friction.Value = value; break;
        case "Density": textEdit_Density.Value = value; break;
        case "StretchWeft": textEdit_StretchWeft.Value = value; break;
        case "StretchWarp": textEdit_StretchWarp.Value = value; break;
        case "BucklingStiffnessWeft": textEdit_BucklingStiffnessWeft.Value = value; break;
        case "BucklingStiffnessWarp": textEdit_BucklingStiffnessWarp.Value = value; break;
      }
    }

    //---------------------------------------------------------------------
    private void textEdit_EditValueChanged(object sender, EventArgs e)
    {
      TextEdit textEdit = sender as TextEdit;
      string name = textEdit.Name;
      int value = Int32.Parse(textEdit.Text);

      switch (name)
      {
        case "BendingWeft": trackBar_BendingWeft.Value = value; break;
        case "BendingWarp": trackBar_BendingWarp.Value = value; break;
        case "InternalDamping": trackBar_InternalDamping.Value = value; break;
        case "Friction": trackBar_Friction.Value = value; break;
        case "Density": trackBar_Density.Value = value; break;
        case "StretchWeft": trackBar_StretchWeft.Value = value; break;
        case "StretchWarp": trackBar_StretchWarp.Value = value; break;
        case "BucklingStiffnessWeft": trackBar_BucklingStiffnessWeft.Value = value; break;
        case "BucklingStiffnessWarp": trackBar_BucklingStiffnessWarp.Value = value; break;
      }
    }

    //---------------------------------------------------------------------
    private void numberUpDown_ValueChanged(object sender, EventArgs e)
    {
      NumericUpDown numberUpDown = sender as NumericUpDown;
      string name = numberUpDown.Name;
      int value = (int)numberUpDown.Value;

      switch (name)
      {
        case "BendingWeft": trackBar_BendingWeft.Value = value; break;
        case "BendingWarp": trackBar_BendingWarp.Value = value; break;
        case "InternalDamping": trackBar_InternalDamping.Value = value; break;
        case "Friction": trackBar_Friction.Value = value; break;
        case "Density": trackBar_Density.Value = value; break;
        case "StretchWeft": trackBar_StretchWeft.Value = value; break;
        case "StretchWarp": trackBar_StretchWarp.Value = value; break;
        case "BucklingStiffnessWeft": trackBar_BucklingStiffnessWeft.Value = value; break;
        case "BucklingStiffnessWarp": trackBar_BucklingStiffnessWarp.Value = value; break;
      }
    }

    //---------------------------------------------------------------------
    private void SetWeaveViewer(int idx, ProjectData data)
    {
      _weave2DViewer.SetProjectData(idx, data);
      ThreadViewerRepaint();
    }

    //---------------------------------------------------------------------    
    // 시작 - Thread    
    private Task taskInitView;
    private CancellationTokenSource ctsInitView;
    private int tempRunIdx = -1;
    private ProjectData tempRunWData;

    public void StartInitProject()
    {
      // 이전 초기화 취소
      ctsInitView?.Cancel();
      ctsInitView?.Dispose();
      ctsInitView = new CancellationTokenSource();

      taskInitView = Task.Run(() => RunInitProjectAsync(ctsInitView.Token), ctsInitView.Token);
    }

    //---------------------------------------------------------------------
    private async Task RunInitProjectAsync(CancellationToken token)
    {
      try
      {
        // IsLoaded3DObject가 true 될 때까지 대기
        while (!IsLoaded3DObject)
        {
          if (token.IsCancellationRequested)
            return;

          await Task.Delay(500, token); // 0.5초 주기로 체크
        }

        // UI 접근은 InvokeAsync 사용
        await this.InvokeAsync(() =>
        {
          SetProjectData(tempRunIdx, tempRunWData);
          CloseProgressForm();
        });
      }
      catch (OperationCanceledException)
      {
        Console.WriteLine("초기화 Task가 취소되었습니다.");
      }
      catch (Exception ex)
      {
        Console.WriteLine("프로젝트 초기화 중 예외: " + ex.Message);
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
    //private Thread threadLoadedObject;
    private CancellationTokenSource ctsLoadedObject;
    public void Thread3DViewerLoadedObject()
    {
      if (ctsLoadedObject != null)
      {
        ctsLoadedObject.Cancel();
        //threadLoadedObject?.Join();
        ctsLoadedObject.Dispose();
        ctsLoadedObject = null;
      }

      ctsLoadedObject = new CancellationTokenSource(); // 새 토큰 생성
                                                       //taskLoadedObject = Task.Run(() => ThreadRunLoadedObject(ctsLoadedObject.Token));
      taskLoadedObject = Task.Run(async () => await ThreadRunLoadedObject(ctsLoadedObject.Token));
    }

    //---------------------------------------------------------------------
    private async Task ThreadRunLoadedObject(CancellationToken token)
    {
      while (!token.IsCancellationRequested && IsLoaded3DObject == false)
      {
        await Task.Delay(500, token); // Sleep 대신 비동기 대기

        try
        {
          var response = await weave3DViewer.EvaluateScriptAsync("IsLoadedObject();");

          if (!string.IsNullOrEmpty(response?.Result?.ToString()))
          {
            IsLoaded3DObject = true;

            if (this.InvokeRequired)
            {
              this.BeginInvoke((System.Action)(() =>
              {
                CloseProgressForm();
                if (ProjectCtrl?.ProjectDataList.Count <= 0)
                {
                  DialogNewProject dialog = new DialogNewProject(this);
                  dialog.StartPosition = FormStartPosition.Manual;
                  dialog.Location = GetChildFormLocation();
                  dialog.dialogNewProjectEventHandler += new DialogNewProjectEventHandler(EventNewProject);
                  dialog.ShowDialog();
                }

                weave3DViewer.ExecuteScriptAsync($"ReloadMap2('{ProjectController.SelectedProjectIdx}', '{nCall++}');");
              }));
            }

            break;
          }
        }
        catch (OperationCanceledException)
        {
          break; // Cancel 정상 처리
        }
        catch (Exception ex)
        {
          Console.WriteLine("[ThreadRunLoadedObject] 예외 발생: " + ex.Message);
          break;
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

    //-----------------------------------------------------------------------
    private void server_ActionRequested(object sender, ActionRequestedEventArgs e)
    {
      e.Server.WriteDefaultAction(e.Context);
    }

    //-----------------------------------------------------------------------
    private void trackBar2_Scroll(object sender, EventArgs e)
    {

    }

    //-----------------------------------------------------------------------
    public static bool isNumber(string strValue)
    {
      return Regex.IsMatch(strValue, @"[-+]?\d*\.?\d+");
    }

    //-----------------------------------------------------------------------
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

    //-----------------------------------------------------------------------
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

    //-----------------------------------------------------------------------
    public Point GetChildFormLocation()
    {
      Point p = new Point(this.Location.X + 100, this.Location.Y + 100);
      return p;
    }
  }

  //-----------------------------------------------------------------------
  public static class ControlExtensions
  {
    public static Task InvokeAsync(this Control control, Action action)
    {
      var tcs = new TaskCompletionSource<bool>();
      control.BeginInvoke(new MethodInvoker(() =>
      {
        try
        {
          action();
          tcs.SetResult(true);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }));
      return tcs.Task;
    }
  }
  
} /* namespace end */