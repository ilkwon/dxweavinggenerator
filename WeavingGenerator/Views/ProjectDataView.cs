using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;
using WeavingGenerator.ProjectDatas;
using Padding = DevExpress.XtraLayout.Utils.Padding;

namespace WeavingGenerator.Views
{
  /// <summary>
  /// ProjectData와 UI 사이의 연결을 담당하는 뷰 클래스
  /// </summary>
  public class ProjectDataView
  {
    public TextEdit textEdit_Name;
    public TextEdit textEdit_BasicInfoRegDt;
    public ComboBoxEdit comboBoxEdit_Scale;
    public CheckEdit checkEdit_YarnDyed;
    public ColorPickEdit colorEdit_DyeColor;
    public void BuildLayout(
      LayoutControl layout,
      Func<int, Padding> createPadding,
      int paddingTopItem,
      int paddingTopGroup,
      EventHandler<ItemCustomDrawEventArgs> groupCustomDraw,
      EventHandlers handlers)
    {
      layout.BeginUpdate();
      CreateBasicInfoGroup(layout, createPadding, paddingTopItem, groupCustomDraw, handlers);
      layout.EndUpdate();
    }

    private void CreateBasicInfoGroup(
      LayoutControl layout,
      Func<int, Padding> createPadding,
      int paddingTopItem,
      EventHandler<ItemCustomDrawEventArgs> groupCustomDraw,
      EventHandlers handlers)
    {

      LayoutControlGroup group;
      LayoutControlItem item;

      group = new LayoutControlGroup
      {
        Text = "기본정보",
        CaptionImageOptions = { Image = Properties.Resources.icon_Basic_16 },
        GroupStyle = DevExpress.Utils.GroupStyle.Title,
        TextVisible = true
      };
      
      group.CustomDraw += groupCustomDraw;
      //group.OptionsItemText.TextToControlDistance = 5; // 여백 설정 (선택)
      //group.GroupBordersVisible = true; // ✅ 타이틀 표시 여부 결정
      //group.AppearanceGroup.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
      //group.AppearanceGroup.ForeColor = Color.White;
      //group.AppearanceGroup.Options.UseFont = true;
      //group.AppearanceGroup.Options.UseForeColor = true;
      layout.Root.Add(group);


      // ▶ 프로젝트 명
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      var label = new LabelControl { Text = "프로젝트 명" };
      item.Control = label;
      item.TextVisible = false;

      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      textEdit_Name = new TextEdit();
      textEdit_Name.TextChanged += handlers?.TextEdit_Name_Changed;
      item.Control = textEdit_Name;
      item.TextVisible = false;

      // ▶ 생성일자
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      var labelDate = new LabelControl { Text = "생성일자" };
      item.Control = labelDate;
      item.TextVisible = false;

      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      textEdit_BasicInfoRegDt = new TextEdit { ReadOnly = true };
      item.Control = textEdit_BasicInfoRegDt;
      item.TextVisible = false;

      // ▶ 확대 비율
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      var labelScale = new LabelControl { Text = "확대" };
      item.Control = labelScale;
      item.TextVisible = false;

      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      comboBoxEdit_Scale = new ComboBoxEdit();
      comboBoxEdit_Scale.Properties.Items.AddRange(new object[] {
        "x01 (기본)", "x02", "x03", "x04", "x05", "x06", "x07", "x08"
      });
      comboBoxEdit_Scale.SelectedIndex = 0;
      comboBoxEdit_Scale.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      comboBoxEdit_Scale.SelectedIndexChanged += handlers?.ComboBoxEdit_Scale_Changed;
      item.Control = comboBoxEdit_Scale;
      item.TextVisible = false;
      // ▶ 염색 색상
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      var labelColor = new LabelControl { Text = "염색 색상" };
      item.Control = labelColor;
      item.TextVisible = true;

      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      colorEdit_DyeColor = new ColorPickEdit();
      
      colorEdit_DyeColor.Properties.ColorDialogOptions.ShowPreview = true;
      colorEdit_DyeColor.ReadOnly = false;
      //colorEdit_DyeColor.Click += (s, e) =>
      //{
      //  if (checkEdit_YarnDyed.Checked) return;
      //  var oldColor = colorEdit_DyeColor.Color;
      //  var newColor = ShowColorDialog(oldColor);
      //  colorEdit_DyeColor.Color = newColor;
      //};
      item.Control = colorEdit_DyeColor;
      item.TextVisible = false;

      // ▶ 선염 여부
      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      checkEdit_YarnDyed = new CheckEdit { Text = "선염", Checked = true };
      checkEdit_YarnDyed.CheckedChanged += (s, e) =>
      {
        colorEdit_DyeColor.Enabled = !checkEdit_YarnDyed.Checked;
      };
      checkEdit_YarnDyed.CheckedChanged += handlers?.Check_YarnDyed_Changed;
      item.Control = checkEdit_YarnDyed;
      item.TextVisible = false;
    }
    private Color ShowColorDialog(Color color)
    {
      using (var dialog = new ColorDialog())
      {
        dialog.Color = color;
        if (dialog.ShowDialog() == DialogResult.OK)
          return dialog.Color;
        return color;
      }
    }

    public void LoadData(ProjectData data)
    {
      if (data == null) return;

      textEdit_Name.Text = data.Name;
      textEdit_BasicInfoRegDt.Text = Util.ToDateHuman(data.Reg_dt);
      comboBoxEdit_Scale.SelectedItem = data.Scale;
      checkEdit_YarnDyed.Checked = data.YarnDyed;
      colorEdit_DyeColor.Enabled = !data.YarnDyed;
      colorEdit_DyeColor.Color = ProjectData.GetDyeColor(data.DyeColor);
    }

    public void SetData(ProjectData data)
    {
      if (data == null) return;

      data.Name = textEdit_Name.Text;
      data.Reg_dt = textEdit_BasicInfoRegDt.Text;
      data.Scale = comboBoxEdit_Scale.SelectedItem?.ToString();
      data.YarnDyed = checkEdit_YarnDyed.Checked;
      data.DyeColor = ProjectData.GetDyeColor(colorEdit_DyeColor.Color);
    }
  }

  public class EventHandlers
  {
    public EventHandler TextEdit_Name_Changed;
    public EventHandler ComboBoxEdit_Scale_Changed;
    public EventHandler Check_YarnDyed_Changed;
  }
}
