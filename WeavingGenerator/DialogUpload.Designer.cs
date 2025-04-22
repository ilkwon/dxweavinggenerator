namespace WeavingGenerator
{
    partial class DialogUpload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogUpload));
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Ok = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Find = new DevExpress.XtraEditors.SimpleButton();
            textEdit_Path = new DevExpress.XtraEditors.TextEdit();
            progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
            ((System.ComponentModel.ISupportInitialize)textEdit_Path.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)progressBarControl1.Properties).BeginInit();
            SuspendLayout();
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_3D_16;
            labelControl3.Location = new System.Drawing.Point(37, 46);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 13;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(69, 42);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(60, 28);
            labelControl2.TabIndex = 12;
            labelControl2.Text = "업로드";
            // 
            // simpleButton_Cancel
            // 
            simpleButton_Cancel.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton_Cancel.Appearance.Options.UseFont = true;
            simpleButton_Cancel.Location = new System.Drawing.Point(294, 203);
            simpleButton_Cancel.Name = "simpleButton_Cancel";
            simpleButton_Cancel.Size = new System.Drawing.Size(120, 33);
            simpleButton_Cancel.TabIndex = 21;
            simpleButton_Cancel.Text = "취소";
            simpleButton_Cancel.Click += simpleButton_Cancel_Click;
            // 
            // simpleButton_Ok
            // 
            simpleButton_Ok.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton_Ok.Appearance.Options.UseFont = true;
            simpleButton_Ok.Location = new System.Drawing.Point(159, 203);
            simpleButton_Ok.Name = "simpleButton_Ok";
            simpleButton_Ok.Size = new System.Drawing.Size(120, 33);
            simpleButton_Ok.TabIndex = 20;
            simpleButton_Ok.Text = "업로드";
            simpleButton_Ok.Click += simpleButton_Ok_Click;
            // 
            // simpleButton_Find
            // 
            simpleButton_Find.Location = new System.Drawing.Point(456, 85);
            simpleButton_Find.Name = "simpleButton_Find";
            simpleButton_Find.Size = new System.Drawing.Size(85, 31);
            simpleButton_Find.TabIndex = 19;
            simpleButton_Find.Text = "Browse";
            simpleButton_Find.Click += simpleButton_Find_Click;
            // 
            // textEdit_Path
            // 
            textEdit_Path.Enabled = false;
            textEdit_Path.Location = new System.Drawing.Point(30, 88);
            textEdit_Path.Name = "textEdit_Path";
            textEdit_Path.Properties.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textEdit_Path.Properties.Appearance.Options.UseFont = true;
            textEdit_Path.Size = new System.Drawing.Size(411, 26);
            textEdit_Path.TabIndex = 17;
            // 
            // progressBarControl1
            // 
            progressBarControl1.Location = new System.Drawing.Point(30, 144);
            progressBarControl1.Name = "progressBarControl1";
            progressBarControl1.Size = new System.Drawing.Size(510, 30);
            progressBarControl1.TabIndex = 22;
            // 
            // DialogUpload
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(587, 265);
            Controls.Add(progressBarControl1);
            Controls.Add(simpleButton_Cancel);
            Controls.Add(simpleButton_Ok);
            Controls.Add(simpleButton_Find);
            Controls.Add(textEdit_Path);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogUpload.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "DialogUpload";
            Padding = new System.Windows.Forms.Padding(20);
            Text = "업로드";
            Load += DialogUpload_Load;
            ((System.ComponentModel.ISupportInitialize)textEdit_Path.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)progressBarControl1.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Ok;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Find;
        private DevExpress.XtraEditors.TextEdit textEdit_Path;
        private DevExpress.XtraEditors.ProgressBarControl progressBarControl1;
    }
}