using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator.WeavingPlan.Dtos
{
  public class YarnInfoDto
  {
    public int Idx { get; set; }
    public string Name { get; set; }
    public string IdxYarn { get; set; }
    public string Weight { get; set; }
    public string Unit { get; set; }
    public string Type { get; set; }
    public string Textured { get; set; }
    public string HexColor { get; set; }
  }
}