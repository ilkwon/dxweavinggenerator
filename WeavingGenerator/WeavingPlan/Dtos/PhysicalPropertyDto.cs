using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator.WeavingPlan.Dtos
{
  namespace WeavingGenerator.WeavingPlan.Dtos
  {
    /// <summary>
    /// CLO 3D 또는 직물 시뮬레이션용 설계표에 포함되는 물성 정보 DTO 클래스입니다.
    /// Bending, Friction, Density 등 시뮬레이션을 위한 핵심 물성값을 담고 있습니다.
    /// </summary>
    public class PhysicalPropertyDto
    {
      public int BendingWeft { get; set; }
      public int BendingWarp { get; set; }
      public int InternalDamping { get; set; }
      public int Friction { get; set; }
      public int Density { get; set; }
      public int StretchWeft { get; set; }
      public int StretchWarp { get; set; }
      public int BucklingStiffnessWeft { get; set; }
      public int BucklingStiffnessWarp { get; set; }
    }
  }

}
