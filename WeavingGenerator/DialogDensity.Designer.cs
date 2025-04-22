namespace WeavingGenerator
{
    partial class DialogDensity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogDensity));
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            spinEdit_WarpDensity = new DevExpress.XtraEditors.SpinEdit();
            spinEdit_WeftDensity = new DevExpress.XtraEditors.SpinEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            simpleButton_Save = new DevExpress.XtraEditors.SimpleButton();
            simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spinEdit_WarpDensity.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spinEdit_WeftDensity.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // labelControl3
            // 
            labelControl3.ImageOptions.Image = Properties.Resources.icon_Density_16;
            labelControl3.Location = new System.Drawing.Point(21, 26);
            labelControl3.Margin = new System.Windows.Forms.Padding(2);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(16, 18);
            labelControl3.TabIndex = 32;
            // 
            // labelControl2
            // 
            labelControl2.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new System.Drawing.Point(46, 19);
            labelControl2.Margin = new System.Windows.Forms.Padding(2);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(40, 28);
            labelControl2.TabIndex = 31;
            labelControl2.Text = "밀도";
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(spinEdit_WarpDensity);
            layoutControl1.Controls.Add(spinEdit_WeftDensity);
            layoutControl1.Location = new System.Drawing.Point(10, 52);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new System.Drawing.Size(398, 88);
            layoutControl1.TabIndex = 33;
            layoutControl1.Text = "layoutControl1";
            // 
            // spinEdit_WarpDensity
            // 
            spinEdit_WarpDensity.EditValue = new decimal(new int[] { 50, 0, 0, 0 });
            spinEdit_WarpDensity.Location = new System.Drawing.Point(81, 12);
            spinEdit_WarpDensity.Margin = new System.Windows.Forms.Padding(2);
            spinEdit_WarpDensity.Name = "spinEdit_WarpDensity";
            spinEdit_WarpDensity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinEdit_WarpDensity.Properties.IsFloatValue = false;
            spinEdit_WarpDensity.Properties.MaskSettings.Set("mask", "N00");
            spinEdit_WarpDensity.Properties.MaxValue = new decimal(new int[] { 300, 0, 0, 0 });
            spinEdit_WarpDensity.Properties.MinValue = new decimal(new int[] { 50, 0, 0, 0 });
            spinEdit_WarpDensity.Size = new System.Drawing.Size(305, 24);
            spinEdit_WarpDensity.StyleController = layoutControl1;
            spinEdit_WarpDensity.TabIndex = 6;
            // 
            // spinEdit_WeftDensity
            // 
            spinEdit_WeftDensity.EditValue = new decimal(new int[] { 50, 0, 0, 0 });
            spinEdit_WeftDensity.Location = new System.Drawing.Point(81, 52);
            spinEdit_WeftDensity.Margin = new System.Windows.Forms.Padding(2);
            spinEdit_WeftDensity.Name = "spinEdit_WeftDensity";
            spinEdit_WeftDensity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinEdit_WeftDensity.Properties.IsFloatValue = false;
            spinEdit_WeftDensity.Properties.MaskSettings.Set("mask", "N00");
            spinEdit_WeftDensity.Properties.MaxValue = new decimal(new int[] { 300, 0, 0, 0 });
            spinEdit_WeftDensity.Properties.MinValue = new decimal(new int[] { 50, 0, 0, 0 });
            spinEdit_WeftDensity.Size = new System.Drawing.Size(305, 24);
            spinEdit_WeftDensity.StyleController = layoutControl1;
            spinEdit_WeftDensity.TabIndex = 7;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem3, emptySpaceItem2, layoutControlItem1 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(398, 88);
            Root.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = spinEdit_WarpDensity;
            layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new System.Drawing.Size(378, 28);
            layoutControlItem3.Text = "경사 밀도";
            layoutControlItem3.TextSize = new System.Drawing.Size(57, 18);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.AllowHotTrack = false;
            emptySpaceItem2.Location = new System.Drawing.Point(0, 28);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new System.Drawing.Size(378, 12);
            emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = spinEdit_WeftDensity;
            layoutControlItem1.Location = new System.Drawing.Point(0, 40);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new System.Drawing.Size(378, 28);
            layoutControlItem1.Text = "위사 밀도";
            layoutControlItem1.TextSize = new System.Drawing.Size(57, 18);
            // 
            // simpleButton_Save
            // 
            simpleButton_Save.Location = new System.Drawing.Point(107, 160);
            simpleButton_Save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Save.Name = "simpleButton_Save";
            simpleButton_Save.Size = new System.Drawing.Size(96, 26);
            simpleButton_Save.TabIndex = 34;
            simpleButton_Save.Text = "저장";
            simpleButton_Save.Click += simpleButton_Save_Click;
            // 
            // simpleButton_Cancel
            // 
            simpleButton_Cancel.Location = new System.Drawing.Point(227, 160);
            simpleButton_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            simpleButton_Cancel.Name = "simpleButton_Cancel";
            simpleButton_Cancel.Size = new System.Drawing.Size(96, 26);
            simpleButton_Cancel.TabIndex = 35;
            simpleButton_Cancel.Text = "취소";
            simpleButton_Cancel.Click += simpleButton_Cancel_Click;
            // 
            // DialogDensity
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(430, 215);
            Controls.Add(simpleButton_Save);
            Controls.Add(simpleButton_Cancel);
            Controls.Add(layoutControl1);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("DialogDensity.IconOptions.Image");
            Margin = new System.Windows.Forms.Padding(2);
            Name = "DialogDensity";
            Padding = new System.Windows.Forms.Padding(20);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "밀도";
            Load += DialogDensity_Load;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spinEdit_WarpDensity.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)spinEdit_WeftDensity.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Save;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SpinEdit spinEdit_WarpDensity;
        private DevExpress.XtraEditors.SpinEdit spinEdit_WeftDensity;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}