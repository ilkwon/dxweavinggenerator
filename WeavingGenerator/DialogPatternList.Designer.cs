namespace WeavingGenerator
{
    partial class DialogPatternList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogPatternList));
            panel1 = new System.Windows.Forms.Panel();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            spinEdit_SizeY = new DevExpress.XtraEditors.SpinEdit();
            spinEdit_SizeX = new DevExpress.XtraEditors.SpinEdit();
            textEdit_Name = new DevExpress.XtraEditors.TextEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            simpleButton_Ok = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            btnUser = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spinEdit_SizeY.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spinEdit_SizeX.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit_Name.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.Control;
            panel1.Location = new System.Drawing.Point(16, 50);
            panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(158, 160);
            panel1.TabIndex = 6;
            panel1.Paint += panel1_Paint;
            panel1.MouseDown += panel1_MouseDown;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(55, 10);
            labelControl2.Margin = new System.Windows.Forms.Padding(2);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(32, 21);
            labelControl2.TabIndex = 13;
            labelControl2.Text = "열기";
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(spinEdit_SizeY);
            layoutControl1.Controls.Add(spinEdit_SizeX);
            layoutControl1.Controls.Add(textEdit_Name);
            layoutControl1.Location = new System.Drawing.Point(10, 226);
            layoutControl1.Margin = new System.Windows.Forms.Padding(2);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new System.Drawing.Size(173, 90);
            layoutControl1.TabIndex = 14;
            layoutControl1.Text = "layoutControl1";
            // 
            // spinEdit_SizeY
            // 
            spinEdit_SizeY.EditValue = new decimal(new int[] { 2, 0, 0, 0 });
            spinEdit_SizeY.Location = new System.Drawing.Point(57, 60);
            spinEdit_SizeY.Name = "spinEdit_SizeY";
            spinEdit_SizeY.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinEdit_SizeY.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            spinEdit_SizeY.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            spinEdit_SizeY.Properties.IsFloatValue = false;
            spinEdit_SizeY.Properties.MaskSettings.Set("mask", "N00");
            spinEdit_SizeY.Properties.MaxValue = new decimal(new int[] { 16, 0, 0, 0 });
            spinEdit_SizeY.Properties.MinValue = new decimal(new int[] { 2, 0, 0, 0 });
            spinEdit_SizeY.Size = new System.Drawing.Size(87, 20);
            spinEdit_SizeY.StyleController = layoutControl1;
            spinEdit_SizeY.TabIndex = 22;
            spinEdit_SizeY.EditValueChanged += spinEdit_SizeY_EditValueChanged;
            // 
            // spinEdit_SizeX
            // 
            spinEdit_SizeX.EditValue = new decimal(new int[] { 2, 0, 0, 0 });
            spinEdit_SizeX.Location = new System.Drawing.Point(57, 36);
            spinEdit_SizeX.Name = "spinEdit_SizeX";
            spinEdit_SizeX.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinEdit_SizeX.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            spinEdit_SizeX.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            spinEdit_SizeX.Properties.IsFloatValue = false;
            spinEdit_SizeX.Properties.MaskSettings.Set("mask", "N00");
            spinEdit_SizeX.Properties.MaxValue = new decimal(new int[] { 16, 0, 0, 0 });
            spinEdit_SizeX.Properties.MinValue = new decimal(new int[] { 2, 0, 0, 0 });
            spinEdit_SizeX.Size = new System.Drawing.Size(87, 20);
            spinEdit_SizeX.StyleController = layoutControl1;
            spinEdit_SizeX.TabIndex = 23;
            spinEdit_SizeX.EditValueChanged += spinEdit_SizeX_EditValueChanged;
            // 
            // textEdit_Name
            // 
            textEdit_Name.Enabled = false;
            textEdit_Name.Location = new System.Drawing.Point(57, 12);
            textEdit_Name.Margin = new System.Windows.Forms.Padding(2);
            textEdit_Name.Name = "textEdit_Name";
            textEdit_Name.Size = new System.Drawing.Size(87, 20);
            textEdit_Name.StyleController = layoutControl1;
            textEdit_Name.TabIndex = 4;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2, layoutControlItem3 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(156, 92);
            Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = textEdit_Name;
            layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new System.Drawing.Size(136, 24);
            layoutControlItem1.Text = "이름";
            layoutControlItem1.TextSize = new System.Drawing.Size(33, 14);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = spinEdit_SizeX;
            layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new System.Drawing.Size(136, 24);
            layoutControlItem2.Text = "Size X";
            layoutControlItem2.TextSize = new System.Drawing.Size(33, 14);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = spinEdit_SizeY;
            layoutControlItem3.Location = new System.Drawing.Point(0, 48);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new System.Drawing.Size(136, 24);
            layoutControlItem3.Text = "Size Y";
            layoutControlItem3.TextSize = new System.Drawing.Size(33, 14);
            // 
            // gridControl1
            // 
            gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            gridControl1.Location = new System.Drawing.Point(186, 50);
            gridControl1.MainView = gridView1;
            gridControl1.Margin = new System.Windows.Forms.Padding(2);
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(214, 232);
            gridControl1.TabIndex = 15;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.DetailHeight = 219;
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            // 
            // simpleButton_Ok
            // 
            simpleButton_Ok.Location = new System.Drawing.Point(128, 326);
            simpleButton_Ok.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            simpleButton_Ok.Name = "simpleButton_Ok";
            simpleButton_Ok.Size = new System.Drawing.Size(77, 21);
            simpleButton_Ok.TabIndex = 16;
            simpleButton_Ok.Text = "확인";
            simpleButton_Ok.Click += simpleButton_Ok_Click;
            // 
            // simpleButton_Cancel
            // 
            simpleButton_Cancel.Location = new System.Drawing.Point(216, 326);
            simpleButton_Cancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            simpleButton_Cancel.Name = "simpleButton_Cancel";
            simpleButton_Cancel.Size = new System.Drawing.Size(77, 21);
            simpleButton_Cancel.TabIndex = 17;
            simpleButton_Cancel.Text = "취소";
            simpleButton_Cancel.Click += simpleButton_Cancel_Click;
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_Pattern_16;
            labelControl3.Location = new System.Drawing.Point(30, 14);
            labelControl3.Margin = new System.Windows.Forms.Padding(2);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 16);
            labelControl3.TabIndex = 18;
            // 
            // btnUser
            // 
            btnUser.Location = new System.Drawing.Point(188, 287);
            btnUser.Name = "btnUser";
            btnUser.Size = new System.Drawing.Size(212, 23);
            btnUser.TabIndex = 19;
            btnUser.Text = "사용자정의";
            btnUser.UseVisualStyleBackColor = true;
            btnUser.Click += btnUser_Click;
            // 
            // DialogPatternList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(426, 367);
            Controls.Add(btnUser);
            Controls.Add(labelControl3);
            Controls.Add(simpleButton_Ok);
            Controls.Add(simpleButton_Cancel);
            Controls.Add(gridControl1);
            Controls.Add(layoutControl1);
            Controls.Add(labelControl2);
            Controls.Add(panel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogPatternList.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            Name = "DialogPatternList";
            Padding = new System.Windows.Forms.Padding(16);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "조직관리";
            Load += DialogPatternList_Load;
            Shown += DialogPatternList_Shown;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spinEdit_SizeY.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)spinEdit_SizeX.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit_Name.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit textEdit_Name;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Ok;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private System.Windows.Forms.Button btnUser;
        private DevExpress.XtraEditors.SpinEdit spinEdit_SizeY;
        private DevExpress.XtraEditors.SpinEdit spinEdit_SizeX;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
    }
}