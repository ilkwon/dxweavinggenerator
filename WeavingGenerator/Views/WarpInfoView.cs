using DevExpress.Mvvm.POCO;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeavingGenerator.ProjectDatas;

namespace WeavingGenerator.Views
{
  public class WarpInfoView : IPropertyViewGroup
  {
    public SimpleButton btnWarp;
    public SimpleButton btnWarpArray;
    public SpinEdit spinWarpDensity;

    public event EventHandler BtnWarpClicked;
    public event EventHandler BtnWarpArrayClicked;
    public event EventHandler DensityChanged;

    public void BuildLayout(LayoutControl layout
      , Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding
      , int paddingTopItem, int paddingTopGroup
      , EventHandler<ItemCustomDrawEventArgs> groupCustomDraw)
    {
      var group = new LayoutControlGroup
      {
        Text = "경사정보",
        CaptionImageOptions = { Image = Properties.Resources.icon_Basic_16 },
        GroupStyle = DevExpress.Utils.GroupStyle.Title,
        TextVisible = true
      };
      group.CustomDraw += groupCustomDraw;
      layout.Root.Add(group);

      LayoutControlItem item;
      // 경사 설정 버튼
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      btnWarp = new SimpleButton { Text = ".." };
      btnWarp.Click += (s, e) => BtnWarpClicked?.Invoke(s, e);
      item.Control = btnWarp;
      item.Text = "경사 설정";

      // 경사 배열 버튼
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      btnWarpArray = new SimpleButton { Text = ".." };
      btnWarpArray.Click += (s, e) => BtnWarpArrayClicked?.Invoke(s, e);
      item.Control = btnWarpArray;
      item.Text = "경사 배열";

      // 밀도 라벨
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      var label = new LabelControl { Text = "밀도" };
      item.Control = label;
      item.TextVisible = false;

      // 밀도 SpinEdit
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      spinWarpDensity = new SpinEdit();
      spinWarpDensity.Properties.IsFloatValue = false;
      spinWarpDensity.Properties.MinValue = 10;
      spinWarpDensity.Properties.MaxValue = 300;
      spinWarpDensity.ValueChanged += (s, e) => DensityChanged?.Invoke(s, e);      
      item.Control = spinWarpDensity;
      item.TextVisible = false;
    }
    public void LoadData(ProjectData data)
    {
      if (data?.Warp != null)
        spinWarpDensity.Value = data.Warp.Density;
    }
    public void SetData(ProjectData data) 
    {
      if (data?.Warp != null)
        data.Warp.Density = Util.ToInt(spinWarpDensity.Text);
    }
  }
}
