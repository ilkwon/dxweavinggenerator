using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraEditors;
using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeavingGenerator.ProjectDatas;

namespace WeavingGenerator
{

  public delegate void DialogUpdateDensityEventHandler(object sender, int newIdx);
  
  public partial class DialogDensity : XtraForm
  {
    private Controllers controllers => Controllers.Instance;
    public DialogUpdateDensityEventHandler dialogUpdateDensityEventHandler = null;
    
    private ProjectController _projectController => controllers.CurrentProjectController;
    public DialogDensity()
    {
      InitializeComponent();      
    }

    private void DialogDensity_Load(object sender, EventArgs e)
    {
      ProjectData data = _projectController.GetProjectData();
      if (data == null) return;
      
      Warp warp = data.Warp;
      if (warp == null) return;
      
      Weft weft = data.Weft;
      if (weft == null) return;
      
      // 경사 밀도
      spinEdit_WarpDensity.Text = warp.Density.ToString();

      // 위사 밀도
      spinEdit_WeftDensity.Text = weft.Density.ToString();
    }
    private void simpleButton_Save_Click(object sender, EventArgs e)
    {
      string warpDensity = spinEdit_WarpDensity.Text;
      string weftDensity = spinEdit_WeftDensity.Text;

      if (string.IsNullOrEmpty(warpDensity))
      {
        XtraMessageBox.Show("경사 밀도를 입력해주세요.");
        spinEdit_WarpDensity.Focus();
        return;
      }
      if (string.IsNullOrEmpty(weftDensity))
      {
        XtraMessageBox.Show("위사 밀도를 입력해주세요.");
        spinEdit_WeftDensity.Focus();
        return;
      }

      int nWarpDensity = Util.ToInt(warpDensity);
      int nWeftDensity = Util.ToInt(weftDensity);

      ProjectData data = _projectController.GetProjectData();
      if (data == null)
      {
        return;
      }

      Warp warp = data.Warp;
      if (warp == null)
      {
        return;
      }
      Weft weft = data.Weft;
      if (weft == null)
      {
        return;
      }

      int oldWarpDensity = warp.Density;
      int oldWeftDensity = weft.Density;
      if (oldWarpDensity != nWarpDensity || oldWeftDensity != nWeftDensity)
      {
        // 경사 밀도
        warp.Density = nWarpDensity;
        // 위사 밀도
        weft.Density = nWeftDensity;

        // 이벤트
        if (dialogUpdateDensityEventHandler != null)
        {
          dialogUpdateDensityEventHandler(this, data.Idx);
        }
      }

      this.Close();
    }

    private void simpleButton_Cancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
