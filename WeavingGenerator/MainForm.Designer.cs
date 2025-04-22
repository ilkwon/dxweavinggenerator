using DevExpress.XtraEditors;

namespace WeavingGenerator
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            colorDialog1 = new System.Windows.Forms.ColorDialog();
            colorDialog2 = new System.Windows.Forms.ColorDialog();
            layoutControl_Project = new DevExpress.XtraLayout.LayoutControl();
            layoutControlGroup_Project = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControl_Property = new DevExpress.XtraLayout.LayoutControl();
            layoutControlGroup_Property = new DevExpress.XtraLayout.LayoutControlGroup();
            barManager1 = new DevExpress.XtraBars.BarManager(components);
            bar2 = new DevExpress.XtraBars.Bar();
            barSubItem_OpenProject = new DevExpress.XtraBars.BarSubItem();
            barButtonItem_NewProject = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_OpenProject = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_SaveProject = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_Exit = new DevExpress.XtraBars.BarButtonItem();
            barSubItem2 = new DevExpress.XtraBars.BarSubItem();
            barButtonItem_NewYarn = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_OpenYarn = new DevExpress.XtraBars.BarButtonItem();
            barSubItem3 = new DevExpress.XtraBars.BarSubItem();
            barButtonItem_Density = new DevExpress.XtraBars.BarButtonItem();
            barSubItem4 = new DevExpress.XtraBars.BarSubItem();
            barButtonItem_OpenWarp = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_OpenWarpArray = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_OpenWeft = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_OpenWeftArray = new DevExpress.XtraBars.BarButtonItem();
            barSubItem5 = new DevExpress.XtraBars.BarSubItem();
            barButtonItem_OpenPattern = new DevExpress.XtraBars.BarButtonItem();
            barSubItem6 = new DevExpress.XtraBars.BarSubItem();
            barButtonItem_Export = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_Upload = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_Print = new DevExpress.XtraBars.BarButtonItem();
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            barButtonItem_Link = new DevExpress.XtraBars.BarButtonItem();
            barSubItem7 = new DevExpress.XtraBars.BarSubItem();
            barButtonItem_CloFabric = new DevExpress.XtraBars.BarButtonItem();
            splitContainer1 = new SplitContainerControl();
            panelControl1 = new PanelControl();
            labelControl2 = new LabelControl();
            labelControl1 = new LabelControl();
            weave3DViewer = new CefSharp.WinForms.ChromiumWebBrowser();
            panel1 = new PanelControl();
            panelControl2 = new PanelControl();
            labelControl3 = new LabelControl();
            labelControl4 = new LabelControl();
            splitContainer2 = new SplitContainerControl();
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(WaitForm1), true, true);
            barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem_NewPattern = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl_Project).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup_Project).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl_Property).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup_Property).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel2).BeginInit();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer2.Panel1).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2.Panel2).BeginInit();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // layoutControl_Project
            // 
            layoutControl_Project.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl_Project.Location = new System.Drawing.Point(0, 0);
            layoutControl_Project.Margin = new System.Windows.Forms.Padding(94);
            layoutControl_Project.Name = "layoutControl_Project";
            layoutControl_Project.Root = layoutControlGroup_Project;
            layoutControl_Project.Size = new System.Drawing.Size(238, 758);
            layoutControl_Project.TabIndex = 0;
            layoutControl_Project.Text = "layoutControl1";
            // 
            // layoutControlGroup_Project
            // 
            layoutControlGroup_Project.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup_Project.GroupBordersVisible = false;
            layoutControlGroup_Project.Name = "Root";
            layoutControlGroup_Project.Size = new System.Drawing.Size(238, 758);
            layoutControlGroup_Project.TextVisible = false;
            // 
            // layoutControl_Property
            // 
            layoutControl_Property.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl_Property.Location = new System.Drawing.Point(0, 0);
            layoutControl_Property.Margin = new System.Windows.Forms.Padding(94);
            layoutControl_Property.Name = "layoutControl_Property";
            layoutControl_Property.Root = layoutControlGroup_Property;
            layoutControl_Property.Size = new System.Drawing.Size(350, 758);
            layoutControl_Property.TabIndex = 11;
            layoutControl_Property.Text = "layoutControl2";
            // 
            // layoutControlGroup_Property
            // 
            layoutControlGroup_Property.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup_Property.GroupBordersVisible = false;
            layoutControlGroup_Property.Name = "layoutControlGroup1";
            layoutControlGroup_Property.Size = new System.Drawing.Size(350, 758);
            layoutControlGroup_Property.TextVisible = false;
            // 
            // barManager1
            // 
            barManager1.AllowQuickCustomization = false;
            barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] { bar2 });
            barManager1.DockControls.Add(barDockControlTop);
            barManager1.DockControls.Add(barDockControlBottom);
            barManager1.DockControls.Add(barDockControlLeft);
            barManager1.DockControls.Add(barDockControlRight);
            barManager1.Form = this;
            barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { barSubItem_OpenProject, barSubItem2, barSubItem3, barSubItem4, barSubItem5, barSubItem6, barSubItem7, barButtonItem_NewProject, barButtonItem_OpenProject, barButtonItem_SaveProject, barButtonItem_Exit, barButtonItem_NewYarn, barButtonItem_OpenYarn, barButtonItem_OpenWarp, barButtonItem_OpenWarpArray, barButtonItem_OpenWeft, barButtonItem_OpenWeftArray, barButtonItem_OpenPattern, barButtonItem_Export, barButtonItem_Upload, barButtonItem_Link, barButtonItem_Density, barButtonItem_Print, barButtonItem_CloFabric });
            barManager1.MainMenu = bar2;
            barManager1.MaxItemId = 22;
            // 
            // bar2
            // 
            bar2.BarName = "Main menu";
            bar2.DockCol = 0;
            bar2.DockRow = 0;
            bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            bar2.FloatLocation = new System.Drawing.Point(-2004, 173);
            bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barSubItem_OpenProject), new DevExpress.XtraBars.LinkPersistInfo(barSubItem2), new DevExpress.XtraBars.LinkPersistInfo(barSubItem3), new DevExpress.XtraBars.LinkPersistInfo(barSubItem4), new DevExpress.XtraBars.LinkPersistInfo(barSubItem5), new DevExpress.XtraBars.LinkPersistInfo(barSubItem6), new DevExpress.XtraBars.LinkPersistInfo(barSubItem7) });
            bar2.OptionsBar.DisableCustomization = true;
            bar2.OptionsBar.DrawBorder = false;
            bar2.OptionsBar.MultiLine = true;
            bar2.OptionsBar.UseWholeRow = true;
            bar2.Text = "Main menu";
            // 
            // barSubItem_OpenProject
            // 
            barSubItem_OpenProject.Caption = "파일";
            barSubItem_OpenProject.Id = 0;
            barSubItem_OpenProject.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_NewProject), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_OpenProject), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_SaveProject), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_Exit) });
            barSubItem_OpenProject.Name = "barSubItem_OpenProject";
            // 
            // barButtonItem_NewProject
            // 
            barButtonItem_NewProject.Caption = "새로만들기";
            barButtonItem_NewProject.Id = 6;
            barButtonItem_NewProject.Name = "barButtonItem_NewProject";
            barButtonItem_NewProject.ItemClick += barButtonItem_NewProject_ItemClick;
            // 
            // barButtonItem_OpenProject
            // 
            barButtonItem_OpenProject.Caption = "열기";
            barButtonItem_OpenProject.Id = 7;
            barButtonItem_OpenProject.Name = "barButtonItem_OpenProject";
            barButtonItem_OpenProject.ItemClick += barButtonItem_OpenProject_ItemClick;
            // 
            // barButtonItem_SaveProject
            // 
            barButtonItem_SaveProject.Caption = "저장";
            barButtonItem_SaveProject.Id = 8;
            barButtonItem_SaveProject.Name = "barButtonItem_SaveProject";
            barButtonItem_SaveProject.ItemClick += barButtonItem_SaveProject_ItemClick;
            // 
            // barButtonItem_Exit
            // 
            barButtonItem_Exit.Caption = "종료";
            barButtonItem_Exit.Id = 9;
            barButtonItem_Exit.Name = "barButtonItem_Exit";
            barButtonItem_Exit.ItemClick += barButtonItem_Exit_ItemClick;
            // 
            // barSubItem2
            // 
            barSubItem2.Caption = "원사";
            barSubItem2.Id = 1;
            barSubItem2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_NewYarn), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_OpenYarn) });
            barSubItem2.Name = "barSubItem2";
            // 
            // barButtonItem_NewYarn
            // 
            barButtonItem_NewYarn.Caption = "새로만들기";
            barButtonItem_NewYarn.Id = 10;
            barButtonItem_NewYarn.Name = "barButtonItem_NewYarn";
            barButtonItem_NewYarn.ItemClick += barButtonItem_NewYarn_ItemClick;
            // 
            // barButtonItem_OpenYarn
            // 
            barButtonItem_OpenYarn.Caption = "열기";
            barButtonItem_OpenYarn.Id = 11;
            barButtonItem_OpenYarn.Name = "barButtonItem_OpenYarn";
            barButtonItem_OpenYarn.ItemClick += barButtonItem_OpenYarn_ItemClick;
            // 
            // barSubItem3
            // 
            barSubItem3.Caption = "밀도";
            barSubItem3.Id = 2;
            barSubItem3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_Density) });
            barSubItem3.Name = "barSubItem3";
            // 
            // barButtonItem_Density
            // 
            barButtonItem_Density.Caption = "밀도";
            barButtonItem_Density.Id = 20;
            barButtonItem_Density.Name = "barButtonItem_Density";
            barButtonItem_Density.ItemClick += barButtonItem_Density_ItemClick;
            // 
            // barSubItem4
            // 
            barSubItem4.Caption = "배열설정";
            barSubItem4.Id = 3;
            barSubItem4.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_OpenWarp), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_OpenWarpArray), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_OpenWeft), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_OpenWeftArray) });
            barSubItem4.Name = "barSubItem4";
            // 
            // barButtonItem_OpenWarp
            // 
            barButtonItem_OpenWarp.Caption = "경사";
            barButtonItem_OpenWarp.Id = 12;
            barButtonItem_OpenWarp.Name = "barButtonItem_OpenWarp";
            barButtonItem_OpenWarp.ItemClick += barButtonItem_OpenWarp_ItemClick;
            // 
            // barButtonItem_OpenWarpArray
            // 
            barButtonItem_OpenWarpArray.Caption = "경사배열";
            barButtonItem_OpenWarpArray.Id = 13;
            barButtonItem_OpenWarpArray.Name = "barButtonItem_OpenWarpArray";
            barButtonItem_OpenWarpArray.ItemClick += barButtonItem_OpenWarpArray_ItemClick;
            // 
            // barButtonItem_OpenWeft
            // 
            barButtonItem_OpenWeft.Caption = "위사";
            barButtonItem_OpenWeft.Id = 14;
            barButtonItem_OpenWeft.Name = "barButtonItem_OpenWeft";
            barButtonItem_OpenWeft.ItemClick += barButtonItem_OpenWeft_ItemClick;
            // 
            // barButtonItem_OpenWeftArray
            // 
            barButtonItem_OpenWeftArray.Caption = "위사배열";
            barButtonItem_OpenWeftArray.Id = 15;
            barButtonItem_OpenWeftArray.Name = "barButtonItem_OpenWeftArray";
            barButtonItem_OpenWeftArray.ItemClick += barButtonItem_OpenWeftArray_ItemClick;
            // 
            // barSubItem5
            // 
            barSubItem5.Caption = "조직";
            barSubItem5.Id = 4;
            barSubItem5.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_OpenPattern) });
            barSubItem5.Name = "barSubItem5";
            // 
            // barButtonItem_OpenPattern
            // 
            barButtonItem_OpenPattern.Caption = "열기";
            barButtonItem_OpenPattern.Id = 16;
            barButtonItem_OpenPattern.Name = "barButtonItem_OpenPattern";
            barButtonItem_OpenPattern.ItemClick += barButtonItem_OpenPattern_ItemClick;
            // 
            // barSubItem6
            // 
            barSubItem6.Caption = "3D텍스처";
            barSubItem6.Id = 5;
            barSubItem6.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_Export), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_Upload), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_Print) });
            barSubItem6.Name = "barSubItem6";
            // 
            // barButtonItem_Export
            // 
            barButtonItem_Export.Caption = "내보내기";
            barButtonItem_Export.Id = 17;
            barButtonItem_Export.Name = "barButtonItem_Export";
            barButtonItem_Export.ItemClick += barButtonItem_Export_ItemClick;
            // 
            // barButtonItem_Upload
            // 
            barButtonItem_Upload.Caption = "업로드";
            barButtonItem_Upload.Id = 18;
            barButtonItem_Upload.Name = "barButtonItem_Upload";
            barButtonItem_Upload.ItemClick += barButtonItem_Upload_ItemClick;
            // 
            // barButtonItem_Print
            // 
            barButtonItem_Print.Caption = "인쇄";
            barButtonItem_Print.Id = 21;
            barButtonItem_Print.Name = "barButtonItem_Print";
            barButtonItem_Print.ItemClick += barButtonItem_Print_ItemClick;
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlTop.Location = new System.Drawing.Point(12, 15);
            barDockControlTop.Manager = barManager1;
            barDockControlTop.Margin = new System.Windows.Forms.Padding(4);
            barDockControlTop.Size = new System.Drawing.Size(1306, 32);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlBottom.Location = new System.Drawing.Point(12, 805);
            barDockControlBottom.Manager = barManager1;
            barDockControlBottom.Margin = new System.Windows.Forms.Padding(4);
            barDockControlBottom.Size = new System.Drawing.Size(1306, 0);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlLeft.Location = new System.Drawing.Point(12, 47);
            barDockControlLeft.Manager = barManager1;
            barDockControlLeft.Margin = new System.Windows.Forms.Padding(4);
            barDockControlLeft.Size = new System.Drawing.Size(0, 758);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            barDockControlRight.Location = new System.Drawing.Point(1318, 47);
            barDockControlRight.Manager = barManager1;
            barDockControlRight.Margin = new System.Windows.Forms.Padding(4);
            barDockControlRight.Size = new System.Drawing.Size(0, 758);
            // 
            // barButtonItem_Link
            // 
            barButtonItem_Link.Caption = "3D뷰어이동";
            barButtonItem_Link.Id = 19;
            barButtonItem_Link.Name = "barButtonItem_Link";
            barButtonItem_Link.ItemClick += barButtonItem_Link_ItemClick;
            // 
            // barSubItem7
            // 
            barSubItem7.Caption = "물성연계";
            barSubItem7.Id = 22;
            barSubItem7.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem_CloFabric) });
            barSubItem7.Name = "barSubItem7";
            // 
            // barButtonItem_CloFabric
            // 
            barButtonItem_CloFabric.Caption = "CLO 3D 패브릭파일 생성";
            barButtonItem_CloFabric.Id = 23;
            barButtonItem_CloFabric.Name = "barButtonItem_CloFabric";
            barButtonItem_CloFabric.ItemClick += barButtonItem_CloFabric_ItemClick;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(12, 47);
            splitContainer1.Margin = new System.Windows.Forms.Padding(19);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panelControl1);
            splitContainer1.Panel1.Controls.Add(weave3DViewer);
            splitContainer1.Panel1.MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Appearance.BackColor = System.Drawing.SystemColors.Control;
            splitContainer1.Panel2.Appearance.Options.UseBackColor = true;
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Panel2.Controls.Add(panelControl2);
            splitContainer1.Panel2.MinSize = 400;
            splitContainer1.Size = new System.Drawing.Size(706, 758);
            splitContainer1.SplitterPosition = 11355;
            splitContainer1.TabIndex = 1;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(labelControl2);
            panelControl1.Controls.Add(labelControl1);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(400, 43);
            panelControl1.TabIndex = 1;
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(39, 7);
            labelControl2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(61, 23);
            labelControl2.TabIndex = 1;
            labelControl2.Text = "3D 뷰어";
            // 
            // labelControl1
            // 
            labelControl1.ImageOptions.Image = Properties.Resources.icon_3DViewer_16;
            labelControl1.Location = new System.Drawing.Point(7, 7);
            labelControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(16, 23);
            labelControl1.TabIndex = 0;
            // 
            // weave3DViewer
            // 
            weave3DViewer.ActivateBrowserOnCreation = false;
            weave3DViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            weave3DViewer.Location = new System.Drawing.Point(0, 0);
            weave3DViewer.Margin = new System.Windows.Forms.Padding(0);
            weave3DViewer.Name = "weave3DViewer";
            weave3DViewer.Padding = new System.Windows.Forms.Padding(0, 100, 0, 0);
            weave3DViewer.Size = new System.Drawing.Size(400, 758);
            weave3DViewer.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Appearance.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            panel1.Appearance.Options.UseBackColor = true;
            panel1.AutoScroll = true;
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 34);
            panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(232, 557);
            panel1.TabIndex = 3;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(labelControl3);
            panelControl2.Controls.Add(labelControl4);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl2.Location = new System.Drawing.Point(0, 0);
            panelControl2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(294, 43);
            panelControl2.TabIndex = 2;
            // 
            // labelControl3
            // 
            labelControl3.Location = new System.Drawing.Point(39, 7);
            labelControl3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(61, 23);
            labelControl3.TabIndex = 1;
            labelControl3.Text = "2D 뷰어";
            // 
            // labelControl4
            // 
            labelControl4.ImageOptions.Image = Properties.Resources.icon_2DViewer_16;
            labelControl4.Location = new System.Drawing.Point(7, 7);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(16, 23);
            labelControl4.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Right;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new System.Drawing.Point(718, 47);
            splitContainer2.Margin = new System.Windows.Forms.Padding(19);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Appearance.Options.UseBackColor = true;
            splitContainer2.Panel1.Controls.Add(layoutControl_Property);
            splitContainer2.Panel1.MinSize = 350;
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Appearance.Options.UseBackColor = true;
            splitContainer2.Panel2.Controls.Add(layoutControl_Project);
            splitContainer2.Panel2.MinSize = 250;
            splitContainer2.Size = new System.Drawing.Size(600, 758);
            splitContainer2.SplitterPosition = 595;
            splitContainer2.TabIndex = 1;
            // 
            // splashScreenManager1
            // 
            splashScreenManager1.ClosingDelay = 500;
            // 
            // barButtonItem1
            // 
            barButtonItem1.Caption = "barButtonItem1";
            barButtonItem1.Id = 22;
            barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem_NewPattern
            // 
            barButtonItem_NewPattern.Caption = "새로 만들기";
            barButtonItem_NewPattern.Id = 23;
            barButtonItem_NewPattern.Name = "barButtonItem_NewPattern";
            barButtonItem_NewPattern.ItemClick += barButtonItem_NewPattern_ItemClick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1330, 820);
            Controls.Add(splitContainer1);
            Controls.Add(splitContainer2);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("MainForm.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "MainForm";
            Padding = new System.Windows.Forms.Padding(12, 15, 12, 15);
            Text = "WeavingGenerator";
            Load += MainForm_Load;
            Shown += MainForm_Shown;
            ((System.ComponentModel.ISupportInitialize)layoutControl_Project).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup_Project).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl_Property).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup_Property).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel1).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel2).EndInit();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)panel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2.Panel1).EndInit();
            splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2.Panel2).EndInit();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ColorDialog colorDialog2;
        private DevExpress.XtraLayout.LayoutControl layoutControl_Project;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup_Project;
        private DevExpress.XtraLayout.LayoutControl layoutControl_Property;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup_Property;
        //private PanelControl panelControl1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarSubItem barSubItem_OpenProject;
        private DevExpress.XtraBars.BarSubItem barSubItem2;
        private DevExpress.XtraBars.BarSubItem barSubItem3;
        private DevExpress.XtraBars.BarSubItem barSubItem4;
        private DevExpress.XtraBars.BarSubItem barSubItem5;
        private DevExpress.XtraBars.BarSubItem barSubItem6;
        private DevExpress.XtraBars.BarSubItem barSubItem7;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_NewProject;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_OpenProject;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_SaveProject;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Exit;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_NewYarn;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_OpenYarn;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_OpenWarp;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_OpenWarpArray;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_OpenWeft;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_OpenWeftArray;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_OpenPattern;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Export;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Upload;
        private SplitContainerControl splitContainer1;
        private SplitContainerControl splitContainer2;
        private CefSharp.WinForms.ChromiumWebBrowser weave3DViewer;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Link;
        //private System.Windows.Forms.PictureBox pictureBox1;
        private PanelControl panelControl1;
        private LabelControl labelControl2;
        private LabelControl labelControl1;
        private PanelControl panelControl2;
        private LabelControl labelControl3;
        private LabelControl labelControl4;
        private DevExpress.XtraEditors.PanelControl panel1;
        //private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Density;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Print;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_NewPattern;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_CloFabric;
    }
}

