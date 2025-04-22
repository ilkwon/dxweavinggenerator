namespace WeavingGenerator
{
    partial class DialogYarnList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogYarnList));
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            simpleButton_Delete = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Add = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Open = new DevExpress.XtraEditors.SimpleButton();
            btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
            btn_OK = new DevExpress.XtraEditors.SimpleButton();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            SuspendLayout();
            // 
            // gridControl1
            // 
            gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl1.Location = new System.Drawing.Point(12, 59);
            gridControl1.MainView = gridView1;
            gridControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(1147, 428);
            gridControl1.TabIndex = 0;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.DetailHeight = 420;
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            gridView1.OptionsScrollAnnotations.ShowSelectedRows = DevExpress.Utils.DefaultBoolean.False;
            // 
            // simpleButton_Delete
            // 
            simpleButton_Delete.Location = new System.Drawing.Point(805, 19);
            simpleButton_Delete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Delete.Name = "simpleButton_Delete";
            simpleButton_Delete.Size = new System.Drawing.Size(96, 26);
            simpleButton_Delete.TabIndex = 1;
            simpleButton_Delete.Text = "선택삭제";
            simpleButton_Delete.Click += btn_Delete_Click;
            // 
            // simpleButton_Add
            // 
            simpleButton_Add.Location = new System.Drawing.Point(1062, 19);
            simpleButton_Add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Add.Name = "simpleButton_Add";
            simpleButton_Add.Size = new System.Drawing.Size(96, 26);
            simpleButton_Add.TabIndex = 2;
            simpleButton_Add.Text = "추가";
            simpleButton_Add.Click += btn_Add_Click;
            // 
            // simpleButton_Open
            // 
            simpleButton_Open.Location = new System.Drawing.Point(949, 19);
            simpleButton_Open.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Open.Name = "simpleButton_Open";
            simpleButton_Open.Size = new System.Drawing.Size(96, 26);
            simpleButton_Open.TabIndex = 3;
            simpleButton_Open.Text = "선택 열기";
            simpleButton_Open.Click += btn_Open_Click;
            // 
            // btn_Cancel
            // 
            btn_Cancel.Location = new System.Drawing.Point(597, 507);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new System.Drawing.Size(96, 26);
            btn_Cancel.TabIndex = 10;
            btn_Cancel.Text = "취소";
            btn_Cancel.Click += btn_Cancel_Click;
            // 
            // btn_OK
            // 
            btn_OK.Location = new System.Drawing.Point(475, 507);
            btn_OK.Name = "btn_OK";
            btn_OK.Size = new System.Drawing.Size(96, 26);
            btn_OK.TabIndex = 9;
            btn_OK.Text = "확인";
            btn_OK.Click += btn_OK_Click;
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_Yarn_16;
            labelControl3.Location = new System.Drawing.Point(15, 23);
            labelControl3.Margin = new System.Windows.Forms.Padding(2);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 32;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(41, 19);
            labelControl2.Margin = new System.Windows.Forms.Padding(2);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(40, 28);
            labelControl2.TabIndex = 31;
            labelControl2.Text = "열기";
            // 
            // DialogYarnList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(1171, 545);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            Controls.Add(btn_Cancel);
            Controls.Add(btn_OK);
            Controls.Add(simpleButton_Open);
            Controls.Add(simpleButton_Add);
            Controls.Add(simpleButton_Delete);
            Controls.Add(gridControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogYarnList.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "DialogYarnList";
            Padding = new System.Windows.Forms.Padding(20);
            Text = "원사관리";
            Load += DialogYarnList_Load;
            Shown += DialogYarnList_Shown;
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Delete;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Add;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Open;
        private DevExpress.XtraEditors.SimpleButton btn_Cancel;
        private DevExpress.XtraEditors.SimpleButton btn_OK;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}