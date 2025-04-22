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
    public partial class DialogCloFabric : XtraForm
    {
        MainForm mainForm;
        string defaultPath = "";

        public DialogCloFabric(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }
        public DialogCloFabric(MainForm mainForm, string defaultPath)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.defaultPath = defaultPath;
        }

        private void simpleButton_Find_Click(object sender, EventArgs e)
        {
            XtraFolderBrowserDialog dialog = new XtraFolderBrowserDialog();
            dialog.SelectedPath = defaultPath;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetFullPath(dialog.SelectedPath);
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
            mainForm.ExportCloFabric(textEdit_Path.Text);
            this.Close();
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
