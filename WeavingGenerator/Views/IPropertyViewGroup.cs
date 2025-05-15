using DevExpress.XtraLayout;
using System;
using WeavingGenerator.ProjectDatas;

namespace WeavingGenerator.Views
{
  public interface IPropertyViewGroup
  {
    void BuildLayout(LayoutControl layout, Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding, int paddingTopItem, int paddingTopGroup, EventHandler<ItemCustomDrawEventArgs> groupCustomDraw);
    void LoadData(ProjectData data);
    void SetData(ProjectData data);
  }
}