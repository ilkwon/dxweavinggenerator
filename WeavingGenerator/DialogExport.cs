using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeavingGenerator
{
    public partial class DialogExport : XtraForm
    {
        MainForm mainForm;

        public DialogExport(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void simpleButton_Find_Click(object sender, EventArgs e)
        {
            XtraSaveFileDialog dialog = new XtraSaveFileDialog();
            dialog.FileName = ""; // Default file name
            dialog.DefaultExt = ".png"; // Default file extension
            dialog.Filter = "Image Files (.png)|*.png"; // Filter files by extension

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetFullPath(dialog.FileName);
                Trace.WriteLine("path : " + path);
                textEdit_Path.Text = path;
            }
        }

        private void simpleButton_Ok_Click(object sender, EventArgs e)
        {
            if (textEdit_Path.Text == "")
            {
                return;
            }
            mainForm.Export2DDImage(textEdit_Path.Text);
            XtraMessageBox.Show("내보내기를 완료 했습니다.");
            //this.Close();
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
