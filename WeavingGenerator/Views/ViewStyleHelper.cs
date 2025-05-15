using DevExpress.XtraLayout;
using System.Drawing;

public static class ViewStyleHelper
{
  public static void DrawGroupGray(object sender, ItemCustomDrawEventArgs e)
  {
    using var brush = new SolidBrush(Color.FromArgb(112, 112, 112));
    e.Cache.FillRectangle(brush, e.Bounds);
    e.Handled = false;
  }

  public static void Group_CustomDraw(object sender, ItemCustomDrawEventArgs e)
  {
    Color c = ColorTranslator.FromHtml("#707070");

    using (SolidBrush brush = new SolidBrush(c))
    {
      e.Cache.FillRectangle(brush, e.Bounds);
    }
    
    e.Handled = false;
  }
}
