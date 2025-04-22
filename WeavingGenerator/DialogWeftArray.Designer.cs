namespace WeavingGenerator
{
    partial class DialogWeftArray
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogWeftArray));
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            textEdit_AddCount = new DevExpress.XtraEditors.TextEdit();
            btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
            btn_OK = new DevExpress.XtraEditors.SimpleButton();
            btn_Add = new DevExpress.XtraEditors.SimpleButton();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            simpleButton_Copy = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Bottom = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Down = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Up = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Top = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit_AddCount.Properties).BeginInit();
            SuspendLayout();
            // 
            // gridControl1
            // 
            gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl1.Location = new System.Drawing.Point(23, 64);
            gridControl1.MainView = gridView1;
            gridControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(820, 375);
            gridControl1.TabIndex = 0;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.DetailHeight = 420;
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            // 
            // textEdit_AddCount
            // 
            textEdit_AddCount.Location = new System.Drawing.Point(852, 62);
            textEdit_AddCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textEdit_AddCount.Name = "textEdit_AddCount";
            textEdit_AddCount.Size = new System.Drawing.Size(96, 24);
            textEdit_AddCount.TabIndex = 9;
            // 
            // btn_Cancel
            // 
            btn_Cancel.Location = new System.Drawing.Point(444, 455);
            btn_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new System.Drawing.Size(96, 26);
            btn_Cancel.TabIndex = 8;
            btn_Cancel.Text = "취소";
            btn_Cancel.Click += btn_Cancel_Click;
            // 
            // btn_OK
            // 
            btn_OK.Location = new System.Drawing.Point(320, 455);
            btn_OK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btn_OK.Name = "btn_OK";
            btn_OK.Size = new System.Drawing.Size(96, 26);
            btn_OK.TabIndex = 7;
            btn_OK.Text = "확인";
            btn_OK.Click += btn_OK_Click;
            // 
            // btn_Add
            // 
            btn_Add.Location = new System.Drawing.Point(852, 93);
            btn_Add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btn_Add.Name = "btn_Add";
            btn_Add.Size = new System.Drawing.Size(96, 26);
            btn_Add.TabIndex = 6;
            btn_Add.Text = "추가";
            btn_Add.Click += btn_Add_Click;
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_Array_16;
            labelControl3.Location = new System.Drawing.Point(24, 20);
            labelControl3.Margin = new System.Windows.Forms.Padding(2);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 38;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(50, 17);
            labelControl2.Margin = new System.Windows.Forms.Padding(2);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(80, 28);
            labelControl2.TabIndex = 37;
            labelControl2.Text = "위사배열";
            // 
            // simpleButton_Copy
            // 
            simpleButton_Copy.Enabled = false;
            simpleButton_Copy.Location = new System.Drawing.Point(638, 455);
            simpleButton_Copy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Copy.Name = "simpleButton_Copy";
            simpleButton_Copy.Size = new System.Drawing.Size(96, 26);
            simpleButton_Copy.TabIndex = 39;
            simpleButton_Copy.Text = "경사와동일";
            simpleButton_Copy.Click += simpleButton_Copy_Click;
            // 
            // simpleButton_Bottom
            // 
            simpleButton_Bottom.Location = new System.Drawing.Point(723, 16);
            simpleButton_Bottom.Margin = new System.Windows.Forms.Padding(4);
            simpleButton_Bottom.Name = "simpleButton_Bottom";
            simpleButton_Bottom.Size = new System.Drawing.Size(120, 33);
            simpleButton_Bottom.TabIndex = 44;
            simpleButton_Bottom.Text = "최하단으로 내리기";
            simpleButton_Bottom.Click += simpleButton_Bottom_Click;
            // 
            // simpleButton_Down
            // 
            simpleButton_Down.Location = new System.Drawing.Point(595, 16);
            simpleButton_Down.Margin = new System.Windows.Forms.Padding(4);
            simpleButton_Down.Name = "simpleButton_Down";
            simpleButton_Down.Size = new System.Drawing.Size(120, 33);
            simpleButton_Down.TabIndex = 43;
            simpleButton_Down.Text = "아래로 내리기";
            simpleButton_Down.Click += simpleButton_Down_Click;
            // 
            // simpleButton_Up
            // 
            simpleButton_Up.Location = new System.Drawing.Point(467, 16);
            simpleButton_Up.Margin = new System.Windows.Forms.Padding(4);
            simpleButton_Up.Name = "simpleButton_Up";
            simpleButton_Up.Size = new System.Drawing.Size(120, 33);
            simpleButton_Up.TabIndex = 42;
            simpleButton_Up.Text = "위로 올리기";
            simpleButton_Up.Click += simpleButton_Up_Click;
            // 
            // simpleButton_Top
            // 
            simpleButton_Top.Location = new System.Drawing.Point(339, 16);
            simpleButton_Top.Margin = new System.Windows.Forms.Padding(4);
            simpleButton_Top.Name = "simpleButton_Top";
            simpleButton_Top.Size = new System.Drawing.Size(120, 33);
            simpleButton_Top.TabIndex = 41;
            simpleButton_Top.Text = "최상단으로 올리기";
            simpleButton_Top.Click += simpleButton_Top_Click;
            // 
            // DialogWeftArray
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(959, 503);
            Controls.Add(simpleButton_Bottom);
            Controls.Add(simpleButton_Down);
            Controls.Add(simpleButton_Up);
            Controls.Add(simpleButton_Top);
            Controls.Add(simpleButton_Copy);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            Controls.Add(textEdit_AddCount);
            Controls.Add(btn_Cancel);
            Controls.Add(btn_OK);
            Controls.Add(btn_Add);
            Controls.Add(gridControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogWeftArray.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "DialogWeftArray";
            Padding = new System.Windows.Forms.Padding(20);
            Text = "배열설정";
            Load += DialogWeftArray_Load;
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit_AddCount.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.TextEdit textEdit_AddCount;
        private DevExpress.XtraEditors.SimpleButton btn_Cancel;
        private DevExpress.XtraEditors.SimpleButton btn_OK;
        private DevExpress.XtraEditors.SimpleButton btn_Add;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Copy;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Bottom;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Down;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Up;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Top;
    }
}