using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeavingGenerator.WeavingPlan.Dtos.WeavingGenerator.WeavingPlan.Dtos;

namespace WeavingGenerator.WeavingPlan.Dtos
{
  public class WeavingPlanSheetDto
  {
    public string Name { get; set; }
    public string ProjectID { get; set; }
    public string Memo { get; set; }
    public string ImageSrc { get; set; }
    public string Reg_dt { get; set; }

    public RepeatUnitDto Pattern { get; set; }
    public WarpDto Warp { get; set; }
    public WeftDto Weft { get; set; }
    
    public PhysicalPropertyDto PhysicalProperty { get; set; }
  }

  public class WarpDto
  {
    public int Density { get; set; }
    public List<YarnInfoDto> WarpInfoList { get; set; }
  }

  public class WeftDto
  {
    public int Density { get; set; }
    public List<YarnInfoDto> WeftInfoList { get; set; }
  }
}
