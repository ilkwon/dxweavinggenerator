using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using WeavingGenerator.ProjectDatas;

namespace WeavingGenerator.Views
{
  public class PhysicalPropertyView : IPropertyViewGroup
  {
    public event EventHandler BendingWeftChanged;
    public event EventHandler BendingWarpChanged;
    public event EventHandler InternalDampingChanged;
    public event EventHandler FrictionChanged;
    public event EventHandler DensityChanged;
    public event EventHandler StretchWeftChanged;
    public event EventHandler StretchWarpChanged;
    public event EventHandler BucklingStiffnessWeftChanged;
    public event EventHandler BucklingStiffnessWarpChanged;

    // 필드 영역 시작
    public TrackBarControl trackBar_BendingWeft;
    public NumericUpDown textEdit_BendingWeft;
    public TrackBarControl trackBar_BendingWarp;
    public NumericUpDown textEdit_BendingWarp;
    public TrackBarControl trackBar_InternalDamping;
    public NumericUpDown textEdit_InternalDamping;
    public TrackBarControl trackBar_Friction;
    public NumericUpDown textEdit_Friction;
    public TrackBarControl trackBar_Density;
    public NumericUpDown textEdit_Density;
    public TrackBarControl trackBar_StretchWeft;
    public NumericUpDown textEdit_StretchWeft;
    public TrackBarControl trackBar_StretchWarp;
    public NumericUpDown textEdit_StretchWarp;
    public TrackBarControl trackBar_BucklingStiffnessWeft;
    public NumericUpDown textEdit_BucklingStiffnessWeft;
    public TrackBarControl trackBar_BucklingStiffnessWarp;
    public NumericUpDown textEdit_BucklingStiffnessWarp;

    public string GroupName => "물성정보";

    private readonly Dictionary<string, NumericUpDown> numericFields;

    public PhysicalPropertyView()
    {
      numericFields = new Dictionary<string, NumericUpDown>();
      _changeEvents = new Dictionary<NumericUpDown, EventHandler>();
      _trackToNumeric = new Dictionary<TrackBarControl, NumericUpDown>();
      // InitializeChangeEvents()는 BuildLayout 이후로 이동
    }

    public void BuildLayout(LayoutControl layout,
      Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding,
      int paddingTopItem, int paddingTopGroup,
      EventHandler<ItemCustomDrawEventArgs> groupCustomDraw)
    {
      var group = new LayoutControlGroup
      {
        Text = GroupName,
        CaptionImageOptions = { Image = Properties.Resources.icon_Basic_16 },
        GroupStyle = DevExpress.Utils.GroupStyle.Title,
        TextVisible = true
      };
      group.CustomDraw += groupCustomDraw;
      layout.Root.Add(group);

      AddAllItems(group, createPadding, paddingTopItem);
      InitializeChangeEvents();
      AttachAllEvents();
    }

    private void AddAllItems(LayoutControlGroup group, Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding, int paddingTopItem)
    {
      AddItem(group, createPadding, paddingTopItem, "굽힘강도 위사 (Bending-Weft)", out trackBar_BendingWeft, out textEdit_BendingWeft, "BendingWeft");
      AddItem(group, createPadding, paddingTopItem, "굽힘강도 경사 (Bending-Warp)", out trackBar_BendingWarp, out textEdit_BendingWarp, "BendingWarp");
      AddItem(group, createPadding, paddingTopItem, "내부 댐핑 (Internal Damping)", out trackBar_InternalDamping, out textEdit_InternalDamping, "InternalDamping");
      AddItem(group, createPadding, paddingTopItem, "마찰 계수 (Friction)", out trackBar_Friction, out textEdit_Friction, "Friction");
      AddItem(group, createPadding, paddingTopItem, "밀도 조절 (Density)", out trackBar_Density, out textEdit_Density, "Density");
      AddItem(group, createPadding, paddingTopItem, "신축성 위사 (Stretch-Weft)", out trackBar_StretchWeft, out textEdit_StretchWeft, "StretchWeft");
      AddItem(group, createPadding, paddingTopItem, "신축성 경사 (Stretch-Warp)", out trackBar_StretchWarp, out textEdit_StretchWarp, "StretchWarp");
      AddItem(group, createPadding, paddingTopItem, "좌굴점 강도 위사 (BucklingStiffness-Weft)", out trackBar_BucklingStiffnessWeft, out textEdit_BucklingStiffnessWeft, "BucklingStiffnessWeft");
      AddItem(group, createPadding, paddingTopItem, "좌굴점 강도 경사 (BucklingStiffness-Warp)", out trackBar_BucklingStiffnessWarp, out textEdit_BucklingStiffnessWarp, "BucklingStiffnessWarp");
    }

    private void AddItem(LayoutControlGroup group, Func<int, DevExpress.XtraLayout.Utils.Padding> createPadding, int paddingTopItem, string labelText, out TrackBarControl trackBar, out NumericUpDown numeric, string name)
    {
      LayoutControlItem item;

      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);
      var label = new LabelControl { Text = labelText };
      item.Control = label;
      item.TextVisible = false;

      item = group.AddItem();
      item.Padding = createPadding(paddingTopItem);

      trackBar = new TrackBarControl
      {
        Name = name,
        Dock = DockStyle.Fill,
        Properties =
        {
          Minimum = 0,
          Maximum = 100,
          TickStyle = TickStyle.None
        }
      };

      numeric = new NumericUpDown
      {
        Name = name,
        Minimum = 0,
        Maximum = 100,
        Dock = DockStyle.Right,
        Width = 60
      };

      var panel = new Panel { Height = 36 };
      panel.Controls.Add(trackBar);
      panel.Controls.Add(numeric);

      item.Control = panel;
      item.TextVisible = false;

      numericFields[name] = numeric;
    }

    private void AttachAllEvents()
    {
      AttachBinding(trackBar_BendingWeft, textEdit_BendingWeft);
      AttachBinding(trackBar_BendingWarp, textEdit_BendingWarp);
      AttachBinding(trackBar_InternalDamping, textEdit_InternalDamping);
      AttachBinding(trackBar_Friction, textEdit_Friction);
      AttachBinding(trackBar_Density, textEdit_Density);
      AttachBinding(trackBar_StretchWeft, textEdit_StretchWeft);
      AttachBinding(trackBar_StretchWarp, textEdit_StretchWarp);
      AttachBinding(trackBar_BucklingStiffnessWeft, textEdit_BucklingStiffnessWeft);
      AttachBinding(trackBar_BucklingStiffnessWarp, textEdit_BucklingStiffnessWarp);
    }

    private readonly Dictionary<NumericUpDown, EventHandler> _changeEvents = new();
    private readonly Dictionary<TrackBarControl, NumericUpDown> _trackToNumeric = new();

    private void InitializeChangeEvents()
    {
      _changeEvents[textEdit_BendingWeft] = (s, e) => BendingWeftChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_BendingWarp] = (s, e) => BendingWarpChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_InternalDamping] = (s, e) => InternalDampingChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_Friction] = (s, e) => FrictionChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_Density] = (s, e) => DensityChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_StretchWeft] = (s, e) => StretchWeftChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_StretchWarp] = (s, e) => StretchWarpChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_BucklingStiffnessWeft] = (s, e) => BucklingStiffnessWeftChanged?.Invoke(this, EventArgs.Empty);
      _changeEvents[textEdit_BucklingStiffnessWarp] = (s, e) => BucklingStiffnessWarpChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AttachBinding(TrackBarControl trackBar, NumericUpDown numeric)
    {
      if (_changeEvents.TryGetValue(numeric, out var handler))
      {
        numeric.ValueChanged += handler;
      }

      _trackToNumeric[trackBar] = numeric;

      trackBar.ValueChanged += (s, e) =>
      {
        if (_trackToNumeric.TryGetValue((TrackBarControl)s, out var targetNumeric))
        {
          targetNumeric.Value = Math.Min(targetNumeric.Maximum, Math.Max(targetNumeric.Minimum, ((TrackBarControl)s).Value));
        }
      };

      numeric.ValueChanged += (s, e) =>
      {
        trackBar.Value = (int)numeric.Value;
      };
    }

    public void LoadData(ProjectData data)
    {
      var p = data?.PhysicalProperty;
      if (p == null) return;

      foreach (var prop in p.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
      {
        if (numericFields.TryGetValue(prop.Name, out var numeric))
        {
          var value = prop.GetValue(p);
          if (value is int intValue)
            numeric.Value = intValue;
        }
      }
    }

    public void SetData(ProjectData data)
    {
      var p = data?.PhysicalProperty;
      if (p == null) return;

      foreach (var prop in p.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
      {
        if (numericFields.TryGetValue(prop.Name, out var numeric))
        {
          prop.SetValue(p, (int)numeric.Value);
        }
      }
    }
  }
}
