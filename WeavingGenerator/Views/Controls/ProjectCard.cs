using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeavingGenerator.Views.Controls
{
  public partial class ProjectCard : DevExpress.XtraEditors.XtraUserControl
  {
    public ProjectCard()
    {
      InitializeComponent();
    }
    public string ProjectName
    {
      get => labelProjectName.Text;
      set => labelProjectName.Text = value;
    }

    public void SetThumbnail(string path)
    {
      try
      {
        pictureEditThumb.Image = Image.FromFile(path);
      }
      catch
      {
        // 기본 이미지 대체 가능
        pictureEditThumb.Image = Properties.Resources.icon_Basic_16;
      }
    }

    public event EventHandler Clicked;    
    public event EventHandler RightClicked;
    private void OnCardClick(object sender, EventArgs e)
    {
      Clicked?.Invoke(this, EventArgs.Empty);
    }

  }
}
