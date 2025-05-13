//using DevExpress.XtraEditors;
//using DevExpress.XtraEditors.Controls;
//using DevExpress.XtraLayout;
//using System;

//namespace WeavingGenerator.Views
//{
//  public class BasicView
//  {
//    /// <summary>
//    /// 프로젝트 이름
//    /// </summary>
//    public TextEdit textEdit_Name { get; }
    
//    /// <summary>
//    /// 등록일
//    /// </summary>
//    public TextEdit textEdit_BasicInfoRegDt { get; }
    
//    /// <summary>
//    /// 선염 여부
//    /// </summary>
//    public ComboBoxEdit comboBoxEdit_Scale { get; }
    
//    /// <summary>
//    /// 염색 컬러
//    /// </summary>
//    public ColorPickEdit colorEdit_DyeColor { get; }

//    /// <summary>
//    /// 직물 미리보기 스케일
//    /// </summary>
//    public CheckEdit checkEdit_YarnDyed { get; }

//    public BasicView()
//    {
//      // 컨트롤 생성
//      textEdit_Name = new TextEdit();
//      textEdit_BasicInfoRegDt = new TextEdit { ReadOnly = true };

//      comboBoxEdit_Scale = new ComboBoxEdit();
//      comboBoxEdit_Scale.Properties.Items.AddRange(new object[] {
//        "x01 (기본)", "x02", "x03", "x04", "x05", "x06", "x07", "x08"
//      });
//      comboBoxEdit_Scale.SelectedIndex = 0;
//      comboBoxEdit_Scale.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

//      colorEdit_DyeColor = new ColorPickEdit();
//      colorEdit_DyeColor.ReadOnly = true;
//      colorEdit_DyeColor.Properties.ColorDialogOptions.ShowPreview = true;

//      checkEdit_YarnDyed = new CheckEdit { Text = "선염", Checked = true };
//    }

//    void CreateBasicInfoLayout(LayoutControlGroup group, Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding, int paddingTop, int paddingGroup)
//    {

//      var item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      var label = new LabelControl();
//      label.Text = "프로젝트 명";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      _basic.textEdit_Name = new TextEdit();
//      textEdit_Name.TextChanged += textEdit_Name_TextChanged;
//      item.Control = textEdit_Name;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "생성일자";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      textEdit_BasicInfoRegDt = new TextEdit();
//      textEdit_BasicInfoRegDt.ReadOnly = true;
//      item.Control = textEdit_BasicInfoRegDt;
//      item.TextVisible = false;

//      /*
//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "광택";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      comboBoxEdit_OptionMetal = new ComboBoxEdit();
//      comboBoxEdit_OptionMetal.Properties.Items.AddRange(new object[] {
//          "FD",
//          "SD",
//          "BR"
//      });
//      comboBoxEdit_OptionMetal.SelectedIndexChanged += ComboBoxEdit_OptionMetal_SelectedIndexChanged;
//      comboBoxEdit_OptionMetal.SelectedIndex = 0;
//      comboBoxEdit_OptionMetal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
//      item.Control = comboBoxEdit_OptionMetal;
//      item.TextVisible = false;
//      */



//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "확대";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      comboBoxEdit_Scale = new ComboBoxEdit();
//      comboBoxEdit_Scale.Properties.Items.AddRange(new object[] {
//                "x01 (기본)",
//                "x02",
//                "x03",
//                "x04",
//                "x05",
//                "x06",
//                "x07",
//                "x08"
//            });
//      comboBoxEdit_Scale.SelectedIndexChanged += ComboBoxEdit_Scale_SelectedIndexChanged;
//      comboBoxEdit_Scale.SelectedIndex = 0;
//      comboBoxEdit_Scale.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
//      item.Control = comboBoxEdit_Scale;
//      item.TextVisible = false;




//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      checkEdit_YarnImage = new CheckEdit();
//      checkEdit_YarnImage.Text = "원사이미지 적용";
//      checkEdit_YarnImage.CheckedChanged += checkYarnImageCheckedChanged;
//      item.Control = checkEdit_YarnImage;
//      item.TextVisible = false;

//      /*
//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM, PADDING_TOP_GROUP);
//      simpleButton_Json = new SimpleButton();
//      simpleButton_Json.Text = "JSON DATA";
//      simpleButton_Json.Click += btnJsonClick;
//      item.Control = simpleButton_Json;
//      item.TextVisible = false;
//      */
//      emptyItem = new EmptySpaceItem();
//      emptyItem.AllowHotTrack = false;
//      emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
//      emptyItem.TextSize = new System.Drawing.Size(0, 0);
//      layoutControl_Property.AddItem(emptyItem);

//      //2025-02-05 soonchol
//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      colorEdit_DyeColor = new ColorPickEdit();
//      colorEdit_DyeColor.Text = "염색 색상";
//      colorEdit_DyeColor.Properties.ColorDialogOptions.ShowPreview = true;
//      colorEdit_DyeColor.Click += colorDyeColorClicked;
//      colorEdit_DyeColor.ReadOnly = true;
//      item.Control = colorEdit_DyeColor;
//      item.TextVisible = true;
//      item.Text = "염색 색상";

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      checkEdit_YarnDyed = new CheckEdit();
//      checkEdit_YarnDyed.Text = "선염";
//      checkEdit_YarnDyed.Checked = true;
//      checkEdit_YarnDyed.CheckedChanged += checkYarnDyedCheckedChanged;
//      item.Control = checkEdit_YarnDyed;
//      item.TextVisible = false;

//      ///////////////////////////////////////////////////////////////////
//      group = new LayoutControlGroup();
//      group.Text = "경사정보";
//      group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
//      group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
//      group.CustomDraw += Group_CustomDraw;
//      layoutControl_Property.Root.Add(group);

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      simpleButton_Warp = new SimpleButton();
//      simpleButton_Warp.Text = "..";
//      simpleButton_Warp.Click += btnWarpClick;
//      item.Control = simpleButton_Warp;
//      item.Text = "경사 설정";

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      simpleButton_WarpArray = new SimpleButton();
//      simpleButton_WarpArray.Text = "..";
//      simpleButton_WarpArray.Click += btnWarpArrayClick;
//      item.Control = simpleButton_WarpArray;
//      item.Text = "경사 배열";

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "밀도";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      spinEdit_WarpDensity = new SpinEdit();
//      spinEdit_WarpDensity.Properties.IsFloatValue = false;
//      spinEdit_WarpDensity.Properties.MinValue = 10;
//      spinEdit_WarpDensity.Properties.MaxValue = 300;
//      spinEdit_WarpDensity.ValueChanged += SpinEdit_WarpDensity_ValueChanged;
//      item.Control = spinEdit_WarpDensity;
//      item.TextVisible = false;

//      emptyItem = new EmptySpaceItem();
//      emptyItem.AllowHotTrack = false;
//      emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
//      emptyItem.TextSize = new System.Drawing.Size(0, 0);
//      layoutControl_Property.AddItem(emptyItem);

//      ///////////////////////////////////////////////////////////////////
//      group = new LayoutControlGroup();
//      group.Text = "위사정보";
//      group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
//      group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
//      group.CustomDraw += Group_CustomDraw;
//      layoutControl_Property.Root.Add(group);

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      simpleButton_Weft = new SimpleButton();
//      simpleButton_Weft.Text = "..";
//      simpleButton_Weft.Click += btnWeftClick;
//      item.Control = simpleButton_Weft;
//      item.Text = "위사 설정";

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      simpleButton_WeftArray = new SimpleButton();
//      simpleButton_WeftArray.Text = "..";
//      simpleButton_WeftArray.Click += btnWeftArrayClick;
//      item.Control = simpleButton_WeftArray;
//      item.Text = "위사 배열";

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "밀도";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      spinEdit_WeftDensity = new SpinEdit();
//      spinEdit_WeftDensity.Properties.IsFloatValue = false;
//      spinEdit_WeftDensity.Properties.MinValue = 10;
//      spinEdit_WeftDensity.Properties.MaxValue = 300;
//      spinEdit_WeftDensity.ValueChanged += SpinEdit_WeftDensity_ValueChanged;
//      item.Control = spinEdit_WeftDensity;
//      item.TextVisible = false;


//      emptyItem = new EmptySpaceItem();
//      emptyItem.AllowHotTrack = false;
//      emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
//      emptyItem.TextSize = new System.Drawing.Size(0, 0);
//      layoutControl_Property.AddItem(emptyItem);


//      ///////////////////////////////////////////////////////////////////
//      group = new LayoutControlGroup();
//      group.Text = "조직정보";
//      group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
//      group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
//      group.CustomDraw += Group_CustomDraw;
//      layoutControl_Property.Root.Add(group);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "조직";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      textEdit_Pattern = new TextEdit();
//      textEdit_Pattern.ReadOnly = true;
//      item.Control = textEdit_Pattern;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      simpleButton_Pattern = new SimpleButton();
//      simpleButton_Pattern.Text = "조직 설정";
//      simpleButton_Pattern.Click += btnPatternClick;
//      item.Control = simpleButton_Pattern;
//      item.TextVisible = false;

//      emptyItem = new EmptySpaceItem();
//      emptyItem.AllowHotTrack = false;
//      emptyItem.Size = new System.Drawing.Size(item.MinSize.Width, PADDING_TOP_GROUP);
//      emptyItem.TextSize = new System.Drawing.Size(0, 0);
//      layoutControl_Property.AddItem(emptyItem);

//      ///////////////////////////////////////////////////////////////////
//      group = new LayoutControlGroup();
//      group.Text = "물성정보";
//      group.CaptionImageOptions.Image = Properties.Resources.icon_Basic_16;
//      group.GroupStyle = DevExpress.Utils.GroupStyle.Title;
//      group.CustomDraw += Group_CustomDraw;
//      layoutControl_Property.Root.Add(group);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "굽힘강도 위사 (Bending-Weft)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_BendingWeft = new TrackBarControl();
//      trackBar_BendingWeft.Name = "BendingWeft";
//      textEdit_BendingWeft = new NumericUpDown();
//      textEdit_BendingWeft.Name = "BendingWeft";
//      createPhysicalPropertyControl(item, trackBar_BendingWeft, textEdit_BendingWeft);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "굽힘강도 경사 (Bending-Warp)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_BendingWarp = new TrackBarControl();
//      trackBar_BendingWarp.Name = "BendingWarp";
//      textEdit_BendingWarp = new NumericUpDown();
//      textEdit_BendingWarp.Name = "BendingWarp";
//      createPhysicalPropertyControl(item, trackBar_BendingWarp, textEdit_BendingWarp);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "내부 댐핑 (Internal Damping)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_InternalDamping = new TrackBarControl();
//      trackBar_InternalDamping.Name = "InternalDamping";
//      textEdit_InternalDamping = new NumericUpDown();
//      textEdit_InternalDamping.Name = "InternalDamping";
//      createPhysicalPropertyControl(item, trackBar_InternalDamping, textEdit_InternalDamping);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "마찰 계수 (Friction)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_Friction = new TrackBarControl();
//      trackBar_Friction.Name = "Friction";
//      textEdit_Friction = new NumericUpDown();
//      textEdit_Friction.Name = "Friction";
//      createPhysicalPropertyControl(item, trackBar_Friction, textEdit_Friction);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "밀도 조절 (Density)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_Density = new TrackBarControl();
//      trackBar_Density.Name = "Density";
//      textEdit_Density = new NumericUpDown();
//      textEdit_Density.Name = "Density";
//      createPhysicalPropertyControl(item, trackBar_Density, textEdit_Density);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "신축성 위사 (Stretch-Weft)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_StretchWeft = new TrackBarControl();
//      trackBar_StretchWeft.Name = "StretchWeft";
//      textEdit_StretchWeft = new NumericUpDown();
//      textEdit_StretchWeft.Name = "StretchWeft";
//      createPhysicalPropertyControl(item, trackBar_StretchWeft, textEdit_StretchWeft);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "신축성 경사 (Stretch-Warp)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_StretchWarp = new TrackBarControl();
//      trackBar_StretchWarp.Name = "StretchWarp";
//      textEdit_StretchWarp = new NumericUpDown();
//      textEdit_StretchWarp.Name = "StretchWarp";
//      createPhysicalPropertyControl(item, trackBar_StretchWarp, textEdit_StretchWarp);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "좌굴점 강도 위사 (Buckling Stiffness-Weft)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_BucklingStiffnessWeft = new TrackBarControl();
//      trackBar_BucklingStiffnessWeft.Name = "BucklingStiffnessWeft";
//      textEdit_BucklingStiffnessWeft = new NumericUpDown();
//      textEdit_BucklingStiffnessWeft.Name = "BucklingStiffnessWeft";
//      createPhysicalPropertyControl(item, trackBar_BucklingStiffnessWeft, textEdit_BucklingStiffnessWeft);


//      item = group.AddItem();
//      item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      label = new LabelControl();
//      label.Text = "좌굴점 강도 경사 (Buckling Stiffness-Warp)";
//      item.Control = label;
//      item.TextVisible = false;

//      item = group.AddItem();
//      //item.Padding = CreatePadding(PADDING_TOP_ITEM);
//      trackBar_BucklingStiffnessWarp = new TrackBarControl();
//      trackBar_BucklingStiffnessWarp.Name = "BucklingStiffnessWarp";
//      textEdit_BucklingStiffnessWarp = new NumericUpDown();
//      textEdit_BucklingStiffnessWarp.Name = "BucklingStiffnessWarp";
//      createPhysicalPropertyControl(item, trackBar_BucklingStiffnessWarp, textEdit_BucklingStiffnessWarp);


//      ///////////////////////////////////////////////////////////////////
//      layoutControl_Property.EndUpdate();
//    }
//  }
//}
