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
  public class PatternInfoView : IPropertyViewGroup
  {
    public TextEdit textPatternName;
    public SimpleButton btnSetPattern;

    public event EventHandler BtnSetPatternClicked;

    public void BuildLayout(LayoutControl layout
      , Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding
      , int paddingTopItem, int paddingTopGroup
      , EventHandler<ItemCustomDrawEventArgs> groupCustomDraw)
    {
      var group = new LayoutControlGroup
      {
        Text = "조직정보",
        CaptionImageOptions = { Image = Properties.Resources.icon_Basic_16 },
        GroupStyle = DevExpress.Utils.GroupStyle.Title,
        TextVisible = true
      };
      group.CustomDraw += groupCustomDraw;
      layout.Root.Add(group);

      LayoutControlItem item;
      // ▶ 조직 이름 텍스트
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      textPatternName = new TextEdit { ReadOnly = true };
      item.Control = textPatternName;
      item.Text = "조직 이름";

      // ▶ 조직 설정 버튼
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      btnSetPattern = new SimpleButton { Text = ".." };
      btnSetPattern.Click += (s, e) => BtnSetPatternClicked?.Invoke(s, e);
      item.Control = btnSetPattern;
      item.Text = "조직 설정";
    }
    //-----------------------------------------------------------------------
    public void LoadData(ProjectData data)
    {
      if (data?.Pattern != null) {
        textPatternName.Text = data.Pattern.Name;
      }
    }
    //-----------------------------------------------------------------------
    public void SetData(ProjectData data)
    {
      if (data?.Pattern != null) {
        data.Pattern.Name = textPatternName.Text;
      }
    }
    //-----------------------------------------------------------------------
  }
}
