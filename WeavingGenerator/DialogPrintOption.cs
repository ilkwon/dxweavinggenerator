using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeavingGenerator
{
  public partial class DialogPrintOption : DevExpress.XtraEditors.XtraForm
  {
    private Controllers controllers => Controllers.Instance;
    MainForm mainForm;
    
    ProjectData data;
    public DialogPrintOption(MainForm main)
    {
      InitializeComponent();         
      mainForm = main;
    }

    private void DialogPrintOption_Load(object sender, EventArgs e)
    {
      ///////////////////////////////////////////////////////////////////
      //
      ///////////////////////////////////////////////////////////////////
      data = controllers.ProjectController.GetProjectData();
      if (data == null)
      {
        XtraMessageBox.Show("프로젝트 생성 후 이용해주세요..");
        return;
      }

      ///////////////////////////////////////////////////////////////////
      //
      ///////////////////////////////////////////////////////////////////
      string appid = mainForm.GetAPPID();
      memoEdit_Memo.Text = data.Memo;
      memoEdit_Memo.SelectionLength = 0;
      simpleButton_Ok.DialogResult = DialogResult.OK;
    }

    private void simpleButton_Ok_Click(object sender, EventArgs e)
    {
      //Trace.WriteLine("MEMO : " + memoEdit_Memo.Text);
      data.Memo = memoEdit_Memo.Text;
      mainForm.SetPrintOptionWarpVisible(checkEdit_Visible.Checked);
      this.Close();
    }

    private void simpleButton_Cancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
