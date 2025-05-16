using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeavingGenerator.WeavingPlan.Dtos.WeavingGenerator.WeavingPlan.Dtos;

namespace WeavingGenerator.WeavingPlan.Dtos
{
  /// <summary>
  /// WeavingPlanSheetDtoEx는 테스트 목적의 확장형 클래스입니다.
  /// 이에 대응하는 매핑 로직도 별도로 구성해야 합니다. 예: WeavingPlanSheetMapperEx
  /// </summary>
  public class WeavingPlanSheetDtoEx
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

    // 확장된 산업 설계표용 구성
    public List<UsedYarnDto> UsedYarns { get; set; }             // 1. 사용원사
    public List<DeliveryInfoDto> Delivery { get; set; }          // 2. 인도내역
    public DimensionSpecDto DimensionSpec { get; set; }          // 3. 직폭규격
    public YarnRepeatDto YarnRepeat { get; set; }                // 4. 경위사 배열
    public List<ProcessConsumptionDto> ProcessConsumption { get; set; } // 5. 공정소요량
    public List<ComparisonDto> Comparison { get; set; }          // 7. 비교표
  }

  public class UsedYarnDto
  {
    public string Type { get; set; } // 경사 / 위사
    public string Name { get; set; }
    public float TM { get; set; }
    public string TwistDirection { get; set; } // S or Z
    public int Count { get; set; }
  }

  public class DeliveryInfoDto
  {
    public string Type { get; set; } // 경사 / 위사
    public int Threads { get; set; }
    public int BodyRepeat { get; set; }
    public int BodyCount { get; set; }
    public int Total { get; set; }
  }

  public class DimensionSpecDto
  {
    public float RawWidth { get; set; }
    public float FinishedWidth { get; set; }
    public float ShrinkRate { get; set; }
    public float Weight { get; set; }
  }

  public class YarnRepeatDto
  {
    public List<YarnSequence> Warp { get; set; }
    public List<YarnSequence> Weft { get; set; }
  }

  public class YarnSequence
  {
    public string Name { get; set; }
    public int Repeat { get; set; }
    public int Count { get; set; }
  }

  public class ProcessConsumptionDto
  {
    public string Type { get; set; }
    public string TensionWeaving { get; set; }
    public float NetWeight { get; set; }
    public int Draft { get; set; }
    public int Finish { get; set; }
    public int GRS { get; set; }
  }

  public class ComparisonDto
  {
    public string YarnName { get; set; }
    public float WeightPerYard { get; set; }
  }
}
