namespace WeavingGenerator
{
    partial class DialogWarpInfo
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogWarpInfo));
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
            btn_Delete = new DevExpress.XtraEditors.SimpleButton();
            btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
            btn_OK = new DevExpress.XtraEditors.SimpleButton();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            gridControl2 = new DevExpress.XtraGrid.GridControl();
            gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            repositoryItemColorEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
            simpleButton_Add_Yarn = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            simpleButton_Color = new DevExpress.XtraEditors.SimpleButton();
            colorEdit_Change = new DevExpress.XtraEditors.ColorPickEdit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemButtonEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemColorEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemButtonEdit2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemColorEdit2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorEdit_Change.Properties).BeginInit();
            SuspendLayout();
            // 
            // gridControl1
            // 
            gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl1.Location = new System.Drawing.Point(18, 60);
            gridControl1.MainView = gridView1;
            gridControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl1.Name = "gridControl1";
            gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemButtonEdit1, repositoryItemColorEdit1 });
            gridControl1.Size = new System.Drawing.Size(552, 329);
            gridControl1.TabIndex = 4;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.DetailHeight = 420;
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            // 
            // repositoryItemButtonEdit1
            // 
            repositoryItemButtonEdit1.AutoHeight = false;
            repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            // 
            // repositoryItemColorEdit1
            // 
            repositoryItemColorEdit1.AutoHeight = false;
            repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
            // 
            // btn_Delete
            // 
            btn_Delete.Location = new System.Drawing.Point(474, 28);
            btn_Delete.Name = "btn_Delete";
            btn_Delete.Size = new System.Drawing.Size(96, 26);
            btn_Delete.TabIndex = 5;
            btn_Delete.Text = "선택삭제";
            btn_Delete.Click += btn_Delete_Click;
            // 
            // btn_Cancel
            // 
            btn_Cancel.Location = new System.Drawing.Point(652, 413);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new System.Drawing.Size(96, 26);
            btn_Cancel.TabIndex = 8;
            btn_Cancel.Text = "취소";
            btn_Cancel.Click += btn_Cancel_Click;
            // 
            // btn_OK
            // 
            btn_OK.Location = new System.Drawing.Point(516, 413);
            btn_OK.Name = "btn_OK";
            btn_OK.Size = new System.Drawing.Size(96, 26);
            btn_OK.TabIndex = 7;
            btn_OK.Text = "확인";
            btn_OK.Click += btn_OK_Click;
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_Array_16;
            labelControl3.Location = new System.Drawing.Point(17, 28);
            labelControl3.Margin = new System.Windows.Forms.Padding(2);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 34;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(42, 25);
            labelControl2.Margin = new System.Windows.Forms.Padding(2);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(40, 28);
            labelControl2.TabIndex = 33;
            labelControl2.Text = "경사";
            // 
            // gridControl2
            // 
            gridControl2.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl2.Location = new System.Drawing.Point(599, 60);
            gridControl2.MainView = gridView2;
            gridControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl2.Name = "gridControl2";
            gridControl2.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemButtonEdit2, repositoryItemColorEdit2 });
            gridControl2.Size = new System.Drawing.Size(661, 329);
            gridControl2.TabIndex = 35;
            gridControl2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView2 });
            // 
            // gridView2
            // 
            gridView2.DetailHeight = 420;
            gridView2.GridControl = gridControl2;
            gridView2.Name = "gridView2";
            // 
            // repositoryItemButtonEdit2
            // 
            repositoryItemButtonEdit2.AutoHeight = false;
            repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
            // 
            // repositoryItemColorEdit2
            // 
            repositoryItemColorEdit2.AutoHeight = false;
            repositoryItemColorEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repositoryItemColorEdit2.Name = "repositoryItemColorEdit2";
            // 
            // simpleButton_Add_Yarn
            // 
            simpleButton_Add_Yarn.Location = new System.Drawing.Point(1164, 23);
            simpleButton_Add_Yarn.Name = "simpleButton_Add_Yarn";
            simpleButton_Add_Yarn.Size = new System.Drawing.Size(96, 26);
            simpleButton_Add_Yarn.TabIndex = 36;
            simpleButton_Add_Yarn.Text = "선택추가";
            simpleButton_Add_Yarn.Click += simpleButton_Add_Yarn_Click;
            // 
            // labelControl1
            // 
            labelControl1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl1.ImageOptions.Image");
            labelControl1.Location = new System.Drawing.Point(600, 29);
            labelControl1.Margin = new System.Windows.Forms.Padding(2);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(16, 18);
            labelControl1.TabIndex = 38;
            // 
            // labelControl4
            // 
            labelControl4.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl4.Appearance.Options.UseFont = true;
            labelControl4.Location = new System.Drawing.Point(625, 26);
            labelControl4.Margin = new System.Windows.Forms.Padding(2);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(40, 28);
            labelControl4.TabIndex = 37;
            labelControl4.Text = "원사";
            // 
            // simpleButton_Color
            // 
            simpleButton_Color.Location = new System.Drawing.Point(275, 28);
            simpleButton_Color.Name = "simpleButton_Color";
            simpleButton_Color.Size = new System.Drawing.Size(96, 26);
            simpleButton_Color.TabIndex = 39;
            simpleButton_Color.Text = "색상적용";
            simpleButton_Color.Click += simpleButton_Color_Click;
            // 
            // colorEdit_Change
            // 
            colorEdit_Change.EditValue = System.Drawing.Color.Empty;
            colorEdit_Change.Location = new System.Drawing.Point(149, 29);
            colorEdit_Change.Name = "colorEdit_Change";
            colorEdit_Change.Properties.AutomaticColor = System.Drawing.Color.Black;
            colorEdit_Change.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            colorEdit_Change.Size = new System.Drawing.Size(118, 24);
            colorEdit_Change.TabIndex = 40;
            // 
            // DialogWarpInfo
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(1288, 469);
            Controls.Add(colorEdit_Change);
            Controls.Add(simpleButton_Color);
            Controls.Add(labelControl1);
            Controls.Add(labelControl4);
            Controls.Add(simpleButton_Add_Yarn);
            Controls.Add(gridControl2);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            Controls.Add(btn_Cancel);
            Controls.Add(btn_OK);
            Controls.Add(btn_Delete);
            Controls.Add(gridControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogWarpInfo.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "DialogWarpInfo";
            Padding = new System.Windows.Forms.Padding(20);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "배열설정";
            Load += DialogWarpInfo_Load;
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemButtonEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemColorEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl2).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemButtonEdit2).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemColorEdit2).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorEdit_Change.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit1;
        private DevExpress.XtraEditors.SimpleButton btn_Delete;
        //private DevExpress.XtraEditors.SimpleButton btn_Add;
        private DevExpress.XtraEditors.SimpleButton btn_Cancel;
        private DevExpress.XtraEditors.SimpleButton btn_OK;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraGrid.GridControl gridControl2;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit2;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Add_Yarn;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Color;
        private DevExpress.XtraEditors.ColorPickEdit colorEdit_Change;
    }
}