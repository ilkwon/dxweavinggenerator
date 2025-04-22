namespace WeavingGenerator
{
    partial class DialogProjectList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogProjectList));
            simpleButton_Ok = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            simpleButton_Open = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Delete = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Add = new DevExpress.XtraEditors.SimpleButton();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            SuspendLayout();
            // 
            // simpleButton_Ok
            // 
            simpleButton_Ok.Location = new System.Drawing.Point(228, 520);
            simpleButton_Ok.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Ok.Name = "simpleButton_Ok";
            simpleButton_Ok.Size = new System.Drawing.Size(96, 26);
            simpleButton_Ok.TabIndex = 8;
            simpleButton_Ok.Text = "확인";
            simpleButton_Ok.Click += simpleButton_Ok_Click;
            // 
            // simpleButton_Cancel
            // 
            simpleButton_Cancel.Location = new System.Drawing.Point(344, 520);
            simpleButton_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Cancel.Name = "simpleButton_Cancel";
            simpleButton_Cancel.Size = new System.Drawing.Size(96, 26);
            simpleButton_Cancel.TabIndex = 9;
            simpleButton_Cancel.Text = "취소";
            simpleButton_Cancel.Click += simpleButton_Cancel_Click;
            // 
            // labelControl1
            // 
            labelControl1.ImageOptions.Image = Properties.Resources.icon_Project_16;
            labelControl1.Location = new System.Drawing.Point(22, 27);
            labelControl1.Margin = new System.Windows.Forms.Padding(2);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(16, 18);
            labelControl1.TabIndex = 9;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(49, 22);
            labelControl2.Margin = new System.Windows.Forms.Padding(2);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(40, 28);
            labelControl2.TabIndex = 8;
            labelControl2.Text = "열기";
            // 
            // simpleButton_Open
            // 
            simpleButton_Open.Location = new System.Drawing.Point(413, 27);
            simpleButton_Open.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Open.Name = "simpleButton_Open";
            simpleButton_Open.Size = new System.Drawing.Size(96, 26);
            simpleButton_Open.TabIndex = 7;
            simpleButton_Open.Text = "선택열기";
            simpleButton_Open.Click += simpleButton_Open_Click;
            // 
            // simpleButton_Delete
            // 
            simpleButton_Delete.Location = new System.Drawing.Point(293, 27);
            simpleButton_Delete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Delete.Name = "simpleButton_Delete";
            simpleButton_Delete.Size = new System.Drawing.Size(96, 26);
            simpleButton_Delete.TabIndex = 6;
            simpleButton_Delete.Text = "선택삭제";
            simpleButton_Delete.Click += simpleButton_Delete_Click;
            // 
            // simpleButton_Add
            // 
            simpleButton_Add.Location = new System.Drawing.Point(575, 27);
            simpleButton_Add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Add.Name = "simpleButton_Add";
            simpleButton_Add.Size = new System.Drawing.Size(96, 26);
            simpleButton_Add.TabIndex = 5;
            simpleButton_Add.Text = "추가";
            simpleButton_Add.Click += simpleButton_Add_Click;
            // 
            // gridControl1
            // 
            gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            gridControl1.Location = new System.Drawing.Point(20, 72);
            gridControl1.MainView = gridView1;
            gridControl1.Margin = new System.Windows.Forms.Padding(2);
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(651, 419);
            gridControl1.TabIndex = 5;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.DetailHeight = 274;
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            // 
            // DialogProjectList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(699, 574);
            Controls.Add(simpleButton_Ok);
            Controls.Add(simpleButton_Cancel);
            Controls.Add(labelControl1);
            Controls.Add(gridControl1);
            Controls.Add(labelControl2);
            Controls.Add(simpleButton_Open);
            Controls.Add(simpleButton_Delete);
            Controls.Add(simpleButton_Add);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogProjectList.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "DialogProjectList";
            Padding = new System.Windows.Forms.Padding(20);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "프로젝트관리";
            Load += DialogWeavingList_Load;
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton simpleButton_Add;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Ok;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Open;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Delete;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}