namespace WeavingGenerator
{
    partial class DialogPrintOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogPrintOption));
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            memoEdit_Memo = new DevExpress.XtraEditors.MemoEdit();
            checkEdit_Visible = new DevExpress.XtraEditors.CheckEdit();
            simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Ok = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)memoEdit_Memo.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)checkEdit_Visible.Properties).BeginInit();
            SuspendLayout();
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_3D_16;
            labelControl3.Location = new System.Drawing.Point(35, 34);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 13;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(67, 30);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(87, 28);
            labelControl2.TabIndex = 12;
            labelControl2.Text = "인쇄 옵션";
            // 
            // memoEdit_Memo
            // 
            memoEdit_Memo.Location = new System.Drawing.Point(31, 78);
            memoEdit_Memo.Name = "memoEdit_Memo";
            memoEdit_Memo.Size = new System.Drawing.Size(631, 271);
            memoEdit_Memo.TabIndex = 14;
            // 
            // checkEdit_Visible
            // 
            checkEdit_Visible.EditValue = true;
            checkEdit_Visible.Location = new System.Drawing.Point(31, 371);
            checkEdit_Visible.Name = "checkEdit_Visible";
            checkEdit_Visible.Properties.Caption = "경/위사 정보 표시";
            checkEdit_Visible.Size = new System.Drawing.Size(255, 24);
            checkEdit_Visible.TabIndex = 15;
            // 
            // simpleButton_Cancel
            // 
            simpleButton_Cancel.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton_Cancel.Appearance.Options.UseFont = true;
            simpleButton_Cancel.Location = new System.Drawing.Point(355, 423);
            simpleButton_Cancel.Name = "simpleButton_Cancel";
            simpleButton_Cancel.Size = new System.Drawing.Size(120, 33);
            simpleButton_Cancel.TabIndex = 18;
            simpleButton_Cancel.Text = "취소";
            simpleButton_Cancel.Click += simpleButton_Cancel_Click;
            // 
            // simpleButton_Ok
            // 
            simpleButton_Ok.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            simpleButton_Ok.Appearance.Options.UseFont = true;
            simpleButton_Ok.Location = new System.Drawing.Point(220, 423);
            simpleButton_Ok.Name = "simpleButton_Ok";
            simpleButton_Ok.Size = new System.Drawing.Size(120, 33);
            simpleButton_Ok.TabIndex = 17;
            simpleButton_Ok.Text = "인쇄";
            simpleButton_Ok.Click += simpleButton_Ok_Click;
            // 
            // DialogPrintOption
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(688, 473);
            Controls.Add(simpleButton_Cancel);
            Controls.Add(simpleButton_Ok);
            Controls.Add(checkEdit_Visible);
            Controls.Add(memoEdit_Memo);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogPrintOption.IconOptions.Image");
            Name = "DialogPrintOption";
            Padding = new System.Windows.Forms.Padding(20);
            Text = "3D텍스쳐";
            Load += DialogPrintOption_Load;
            ((System.ComponentModel.ISupportInitialize)memoEdit_Memo.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)checkEdit_Visible.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.MemoEdit memoEdit_Memo;
        private DevExpress.XtraEditors.CheckEdit checkEdit_Visible;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Ok;
    }
}