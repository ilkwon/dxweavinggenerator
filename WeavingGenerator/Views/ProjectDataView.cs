using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeavingGenerator.Views
{
  public class ProjectDataView
  {
    private readonly BasicView _basic;
    public ProjectDataView(BasicView view) { _basic = view; }

    public void Show(ProjectData data)
    {
      _basic.Name.Text = data.Name;
      _basic.RegDate.Text = data.Reg_dt;
      _basic.YarnDyed.Checked = data.YarnDyed;
      _basic.DyeColor.Color = Util.ToColor(data.DyeColor);
      // _basic.Scale.SelectedIndex = 0; // 설정이 있다면 여기에 추가
    }
  }
}
