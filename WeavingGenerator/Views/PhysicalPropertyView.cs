using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeavingGenerator.ProjectDatas;

namespace WeavingGenerator.Views
{
  public class PhysicalPropertyView : IPropertyViewGroup
  {
    public void BuildLayout(LayoutControl layout, Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding, int paddingTopItem, int paddingTopGroup, EventHandler<ItemCustomDrawEventArgs> groupCustomDraw)
    {

    }
    public void SetProjectData(ProjectData data)
    {

    }
    public void ApplyProjectData(ProjectData data)
    {
    }
  }
}
