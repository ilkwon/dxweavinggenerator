using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSpreadsheet.Model;
using System;

using WeavingGenerator.ProjectDatas;

namespace WeavingGenerator.Views
{
  public class WeftInfoView : IPropertyViewGroup
  {
    public SimpleButton btnWeft;
    public SimpleButton btnWeftArray;
    public SpinEdit spinWeftDensity;

    public event EventHandler BtnWeftClicked;
    public event EventHandler BtnWeftArrayClicked;
    public event EventHandler DensityChanged;

    public void BuildLayout(LayoutControl layout
      , Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding
      , int paddingTopItem, int paddingTopGroup
      , EventHandler<ItemCustomDrawEventArgs> groupCustomDraw)
    {
      var group = new LayoutControlGroup
      {
        Text = "위사정보",
        CaptionImageOptions = { Image = Properties.Resources.icon_Basic_16 },
        GroupStyle = DevExpress.Utils.GroupStyle.Title
      };

      group.CustomDraw += groupCustomDraw;
      
    }

    public void SetProjectData(ProjectData data)
    {

    }
    public void ApplyProjectData(ProjectData data)
    {
    }
  }
}
