using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator.WeavingPlan.Dtos
{
  public class RepeatUnitDto
  {
    public int Idx { get; set; }
    public string Name { get; set; }
    public int XLength { get; set; }
    public int YLength { get; set; }
    public int[][] Data { get; set; }
  }
}
