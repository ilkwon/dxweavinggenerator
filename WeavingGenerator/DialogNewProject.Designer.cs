namespace WeavingGenerator
{
    partial class DialogNewProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogNewProject));
            textEdit_Name = new DevExpress.XtraEditors.TextEdit();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)textEdit_Name.Properties).BeginInit();
            SuspendLayout();
            // 
            // textEdit_Name
            // 
            textEdit_Name.Location = new System.Drawing.Point(77, 86);
            textEdit_Name.Name = "textEdit_Name";
            textEdit_Name.Properties.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textEdit_Name.Properties.Appearance.Options.UseFont = true;
            textEdit_Name.Size = new System.Drawing.Size(344, 26);
            textEdit_Name.TabIndex = 4;
            // 
            // simpleButton1
            // 
            simpleButton1.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton1.Appearance.Options.UseFont = true;
            simpleButton1.Location = new System.Drawing.Point(115, 136);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(120, 33);
            simpleButton1.TabIndex = 5;
            simpleButton1.Text = "확인";
            simpleButton1.Click += btn_Ok_Click;
            // 
            // simpleButton2
            // 
            simpleButton2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton2.Appearance.Options.UseFont = true;
            simpleButton2.Location = new System.Drawing.Point(241, 136);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new System.Drawing.Size(120, 33);
            simpleButton2.TabIndex = 6;
            simpleButton2.Text = "취소";
            simpleButton2.Click += btn_Cancel_Click;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(65, 34);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(100, 28);
            labelControl2.TabIndex = 7;
            labelControl2.Text = "새로만들기";
            // 
            // labelControl1
            // 
            labelControl1.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.LineVisible = true;
            labelControl1.Location = new System.Drawing.Point(31, 89);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(30, 20);
            labelControl1.TabIndex = 8;
            labelControl1.Text = "이름";
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_Project_16;
            labelControl3.Location = new System.Drawing.Point(33, 38);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 9;
            // 
            // DialogNewProject
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(473, 190);
            Controls.Add(labelControl3);
            Controls.Add(labelControl1);
            Controls.Add(labelControl2);
            Controls.Add(simpleButton2);
            Controls.Add(simpleButton1);
            Controls.Add(textEdit_Name);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogNewProject.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "DialogNewProject";
            Padding = new System.Windows.Forms.Padding(20);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "프로젝트 관리";
            Load += DialogNewWeaving_Load;
            ((System.ComponentModel.ISupportInitialize)textEdit_Name.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.TextEdit textEdit_Name;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
    }
}