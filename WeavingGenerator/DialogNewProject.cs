using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WeavingGenerator
{

    public delegate void DialogNewProjectEventHandler(object sender, int newIdx);


    public partial class DialogNewProject : DevExpress.XtraEditors.XtraForm
    {
        public DialogNewProjectEventHandler dialogNewProjectEventHandler = null;

        MainForm mainForm;
        public DialogNewProject(MainForm main)
        {
            InitializeComponent();
            mainForm = main;
        }

        private void DialogNewWeaving_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            string v = textEdit_Name.Text;
            if (string.IsNullOrEmpty(v))
            {
                return;
            }
            
            int idx = mainForm.CreateProject(textEdit_Name.Text);            
            if(idx < 0)
            {
                XtraMessageBox.Show("프로젝트 생성 하지 못했습니다. 앱을 다시 시작 후 이용해주세요..");
                return;
            }
            var data = mainForm.ProjectController.GetProjectData(idx);
            mainForm.SetProjectData(idx, data);

            // 이벤트
            if (dialogNewProjectEventHandler != null)
            {
                dialogNewProjectEventHandler(this, idx);
            }
            
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}
