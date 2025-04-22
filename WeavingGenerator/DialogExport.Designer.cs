namespace WeavingGenerator
{
    partial class DialogExport
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
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            textEdit_Path = new DevExpress.XtraEditors.TextEdit();
            simpleButton_Find = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Ok = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)textEdit_Path.Properties).BeginInit();
            SuspendLayout();
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_3D_16;
            labelControl3.Location = new System.Drawing.Point(46, 48);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 11;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(78, 44);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(80, 28);
            labelControl2.TabIndex = 10;
            labelControl2.Text = "내보내기";
            // 
            // labelControl1
            // 
            labelControl1.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.LineVisible = true;
            labelControl1.Location = new System.Drawing.Point(52, 106);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(30, 20);
            labelControl1.TabIndex = 13;
            labelControl1.Text = "경로";
            // 
            // textEdit_Path
            // 
            textEdit_Path.Enabled = false;
            textEdit_Path.Location = new System.Drawing.Point(103, 103);
            textEdit_Path.Name = "textEdit_Path";
            textEdit_Path.Properties.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textEdit_Path.Properties.Appearance.Options.UseFont = true;
            textEdit_Path.Size = new System.Drawing.Size(325, 26);
            textEdit_Path.TabIndex = 12;
            // 
            // simpleButton_Find
            // 
            simpleButton_Find.Location = new System.Drawing.Point(443, 100);
            simpleButton_Find.Name = "simpleButton_Find";
            simpleButton_Find.Size = new System.Drawing.Size(85, 31);
            simpleButton_Find.TabIndex = 14;
            simpleButton_Find.Text = "찾기";
            simpleButton_Find.Click += simpleButton_Find_Click;
            // 
            // simpleButton_Cancel
            // 
            simpleButton_Cancel.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton_Cancel.Appearance.Options.UseFont = true;
            simpleButton_Cancel.Location = new System.Drawing.Point(284, 166);
            simpleButton_Cancel.Name = "simpleButton_Cancel";
            simpleButton_Cancel.Size = new System.Drawing.Size(120, 33);
            simpleButton_Cancel.TabIndex = 16;
            simpleButton_Cancel.Text = "취소";
            simpleButton_Cancel.Click += simpleButton_Cancel_Click;
            // 
            // simpleButton_Ok
            // 
            simpleButton_Ok.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton_Ok.Appearance.Options.UseFont = true;
            simpleButton_Ok.Location = new System.Drawing.Point(149, 166);
            simpleButton_Ok.Name = "simpleButton_Ok";
            simpleButton_Ok.Size = new System.Drawing.Size(120, 33);
            simpleButton_Ok.TabIndex = 15;
            simpleButton_Ok.Text = "내보내기";
            simpleButton_Ok.Click += simpleButton_Ok_Click;
            // 
            // DialogExport
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(561, 227);
            Controls.Add(simpleButton_Cancel);
            Controls.Add(simpleButton_Ok);
            Controls.Add(simpleButton_Find);
            Controls.Add(labelControl1);
            Controls.Add(textEdit_Path);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = Properties.Resources.main_16;
            Name = "DialogExport";
            Padding = new System.Windows.Forms.Padding(20);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "3D텍스쳐";
            ((System.ComponentModel.ISupportInitialize)textEdit_Path.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit textEdit_Path;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Find;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Ok;
    }
}