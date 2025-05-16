namespace WeavingGenerator.Views.Controls
{
  partial class ProjectCard
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
      dockManager1 = new DevExpress.XtraBars.Docking.DockManager(components);
      pictureEditThumb = new DevExpress.XtraEditors.PictureEdit();
      labelProjectName = new DevExpress.XtraEditors.LabelControl();
      labelDescript = new DevExpress.XtraEditors.LabelControl();
      ((System.ComponentModel.ISupportInitialize)dockManager1).BeginInit();
      ((System.ComponentModel.ISupportInitialize)pictureEditThumb.Properties).BeginInit();
      SuspendLayout();
      // 
      // dockManager1
      // 
      dockManager1.Form = this;
      dockManager1.TopZIndexControls.AddRange(new string[] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "System.Windows.Forms.StatusBar", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl", "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl", "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl" });
      // 
      // pictureEditThumb
      // 
      pictureEditThumb.Location = new System.Drawing.Point(3, 3);
      pictureEditThumb.Name = "pictureEditThumb";
      pictureEditThumb.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
      pictureEditThumb.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
      pictureEditThumb.Size = new System.Drawing.Size(115, 115);
      pictureEditThumb.TabIndex = 0;
      // 
      // labelControl1
      // 
      labelProjectName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      labelProjectName.Location = new System.Drawing.Point(3, 119);
      labelProjectName.Name = "labelControl1";
      labelProjectName.Size = new System.Drawing.Size(262, 28);
      labelProjectName.TabIndex = 1;
      labelProjectName.Text = "labelProjectName";
      // 
      // labelControl2
      // 
      labelDescript.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      labelDescript.Location = new System.Drawing.Point(124, 3);
      labelDescript.Name = "labelControl2";
      labelDescript.Size = new System.Drawing.Size(141, 115);
      labelDescript.TabIndex = 2;
      labelDescript.Text = "labelＭｏｒｅＩｎｆｏ\r\n";
      // 
      // ProjectCard
      // 
      AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      Controls.Add(labelDescript);
      Controls.Add(labelProjectName);
      Controls.Add(pictureEditThumb);
      Name = "ProjectCard";
      Size = new System.Drawing.Size(281, 156);
      ((System.ComponentModel.ISupportInitialize)dockManager1).EndInit();
      ((System.ComponentModel.ISupportInitialize)pictureEditThumb.Properties).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private DevExpress.XtraBars.Docking.DockManager dockManager1;
    private DevExpress.XtraEditors.PictureEdit pictureEditThumb;
    private DevExpress.XtraEditors.LabelControl labelProjectName;
    private DevExpress.XtraEditors.LabelControl labelDescript;
  }
}
