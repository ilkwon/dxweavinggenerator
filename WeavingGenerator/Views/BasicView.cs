using DevExpress.XtraEditors;

namespace WeavingGenerator.Views
{
  public class BasicView
  {
    public TextEdit Name { get; }
    public TextEdit RegDate { get; }
    public CheckEdit YarnDyed { get; }
    public ColorPickEdit DyeColor { get; }
    public ComboBoxEdit Scale { get; }

    public BasicView(
      TextEdit name,
      TextEdit regDate,
      CheckEdit yarnDyed,
      ColorPickEdit dyeColor,
      ComboBoxEdit scale)
    {
      Name = name;
      RegDate = regDate;
      YarnDyed = yarnDyed;
      DyeColor = dyeColor;
      Scale = scale;
    }
  }
}
