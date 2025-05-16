using System;
using System.Collections.Generic;
using System.Linq;
using WeavingGenerator.ProjectDatas;
using WeavingGenerator.WeavingPlan.Dtos;
using WeavingGenerator.WeavingPlan.Dtos.WeavingGenerator.WeavingPlan.Dtos;

namespace WeavingGenerator.WeavingPlan.Mappers
{
  /// <summary>
  /// 직물 조직 설계서 출력을 위한 내부 데이터를 외부 출력용 DTO(WeavingPlanSheetDto)로 변환하는 매퍼 클래스입니다.
  /// 
  /// 이 클래스는 JSON 문자열을 직접 생성하지 않으며,
  /// 오직 인쇄 출력 및 직렬화에 적합한 데이터 구조로 변환하는 역할만 담당합니다.
  ///
  /// 굳이 중간자(DTO Mapper) 클래스를 둔 이유는 다음과 같습니다:
  /// - 여러 내부 클래스 데이터들(ProjectData, Warp, Weft, RepeatUnit 등)을 외부 출력 구조와 분리
  /// - 외부 노출을 제한하고, 역할과 책임을 명확히 구분하기 위함
  /// [주의]
  /// - JSON 문자열 생성은 Newtonsoft.Json 등의 외부 직렬화 도구에서 별도로 수행하십시오.
  /// - 이 클래스는 직렬화가 아닌 DTO 생성만 담당함.
  /// </summary>

  public static class WeavingPlanSheetMapper
  {
    public static WeavingPlanSheetDto Map(ProjectData data, List<Yarn> yarns, string imagePath)
    {
      if (data == null) return null;

      var dto = new WeavingPlanSheetDto
      {
        ProjectID = data.ProjectID,
        Name = data.Name,
        Memo = data.Memo,
        ImageSrc = Util.EscapeStringValue(imagePath),
        Reg_dt = FromDate(data.Reg_dt),
        Pattern = MapPattern(data.Pattern),
        Warp = MapWarp(data.Warp, yarns),
        Weft = MapWeft(data.Weft, yarns),
        // comment by ilkwon. 25.05.16
        // 기존의 json 하드코딩에는 없었던 필드 
        ///PhysicalProperty = MapPhysicalProperty(data.PhysicalProperty)
      };

      return dto;
    }

    private static PhysicalPropertyDto MapPhysicalProperty(PhysicalProperty property)
    {
      if (property == null) return null;

      return new PhysicalPropertyDto
      {
        BendingWeft = property.BendingWeft,
        BendingWarp = property.BendingWarp,
        InternalDamping = property.InternalDamping,
        Friction = property.Friction,
        Density = property.Density,
        StretchWeft = property.StretchWeft,
        StretchWarp = property.StretchWarp,
        BucklingStiffnessWeft = property.BucklingStiffnessWeft,
        BucklingStiffnessWarp = property.BucklingStiffnessWarp
      };
    }

    //-----------------------------------------------------------------------
    private static WeftDto MapWeft(Weft weft, List<Yarn> yarns)
    {
      if (weft == null) return null;

      return new WeftDto
      {
        Density = weft.Density,
        WeftInfoList = weft.GetWInfoList()
                         .Select(info => MapYarnInfo(info, yarns))
                         .ToList()
      };
    }
    //-----------------------------------------------------------------------
    private static WarpDto MapWarp(Warp warp, List<Yarn> yarns)
    {
      if (warp == null) return null;

      return new WarpDto
      {
        Density = warp.Density,
        WarpInfoList = warp.GetWInfoList()
                        .Select(info => MapYarnInfo(info, yarns))
                        .ToList()
      };
    }
    //-----------------------------------------------------------------------
    private static YarnInfoDto MapYarnInfo(WInfo info, List<Yarn> yarns)
    {
      var yarn = Find(info, yarns);      
      if (yarn == null) return null;
      return new YarnInfoDto
      {
        Idx = info.Idx,
        Name = yarn?.Name ?? "-",
        IdxYarn = info.IdxYarn.ToString(),
        Weight = yarn?.Weight ?? "250",
        Unit = yarn?.Unit ?? "Denier",
        Type = yarn?.Type ?? "",
        Textured = yarn?.Textured ?? "",
        HexColor = info.HexColor
      };
    }
    //-----------------------------------------------------------------------
    private static Yarn Find(WInfo info, List<Yarn> yarns)
    {      
      for (int i = 0; i < yarns.Count; i++)
      {
        if (yarns[i].Idx == info.IdxYarn)
        {
          return yarns[i];          
        }
      }
      return null;
    }
    //-----------------------------------------------------------------------
    private static string FromDate(string raw)
    {
      try {
        return DateTime.ParseExact(raw, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
      } catch {
        return raw;
      }
    }
    //-----------------------------------------------------------------------
    private static RepeatUnitDto MapPattern(Pattern pattern)
    {
      if (pattern == null) return null;
      
      int rows = pattern.Data.GetLength(0);
      int cols = pattern.Data.GetLength(1);      
      int[][] converted = new int[rows][];

      for (int i = 0; i < rows; i++) 
      {
        converted[i] = new int[cols];
        for (int j = 0; j<cols; j++)
        {
          converted[i][j] = pattern.Data[i, j];
        }
      }
      
      return new RepeatUnitDto 
      {
        Idx = pattern.Idx,
        Name = pattern.Name,
        XLength = pattern.XLength,
        YLength = pattern.YLength,
        Data = converted
      };  
    }
    //-----------------------------------------------------------------------
  }
}
