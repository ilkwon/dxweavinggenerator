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
        GroupStyle = DevExpress.Utils.GroupStyle.Title,
        TextVisible = true
      };

      group.CustomDraw += groupCustomDraw;
      layout.Root.Add(group);

      LayoutControlItem item;

      // 위사 설정 버튼
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      btnWeft = new SimpleButton { Text = ".." };
      btnWeft.Click += (s, e) => BtnWeftClicked?.Invoke(s, e);
      item.Control = btnWeft;
      item.Text = "위사 설정";

      // 위사 배열 버튼
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      btnWeftArray = new SimpleButton { Text = ".." };
      btnWeftArray.Click += (s, e) => BtnWeftArrayClicked?.Invoke(s, e);
      item.Control = btnWeftArray;
      item.Text = "위사 배열";

      // 밀도 라벨
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      var label = new LabelControl { Text = "밀도" };
      item.Control = label;
      item.TextVisible = false;

      // 밀도
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      spinWeftDensity = new SpinEdit();
      spinWeftDensity.Properties.IsFloatValue = false;
      spinWeftDensity.Properties.MinValue = 10;
      spinWeftDensity.Properties.MaxValue = 300;
      spinWeftDensity.ValueChanged += (s, e) => DensityChanged?.Invoke(s, e);      
      item.Control = spinWeftDensity;
      item.TextVisible = false;
    }
    //-----------------------------------------------------------------------
    public void LoadData(ProjectData data)
    {
      if (data?.Weft != null) {
        spinWeftDensity.Value = data.Weft.Density;
      }
    }
    //-----------------------------------------------------------------------
    public void SetData(ProjectData data)
    {
      if (data?.Weft != null) {
        data.Weft.Density = Util.ToInt(spinWeftDensity.Text);
      }
    }
    //-----------------------------------------------------------------------
  }
}
