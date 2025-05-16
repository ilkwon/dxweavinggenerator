using System;
using System.Collections.Generic;
using WeavingGenerator.ProjectDatas;
using WeavingGenerator.WeavingPlan.Dtos;

namespace WeavingGenerator.WeavingPlan.Mappers
{
  /// <summary>
  /// 직조공장용 확장 설계표 DTO(WeavingPlanSheetDtoEx)로 변환하는 매퍼 클래스입니다.
  /// 조직/원사/배합정보 외에 실제 생산관리 항목들을 포함합니다.
  /// </summary>
  public static class WeavingPlanSheetMapperEx
  {
    public static WeavingPlanSheetDtoEx Map(ProjectData data, List<Yarn> yarns, string imagePath)
    {
      if (data == null) return null;

      var dto = new WeavingPlanSheetDtoEx
      {
        ProjectID = data.ProjectID,
        Name = data.Name,
        Memo = data.Memo,
        ImageSrc = Util.EscapeStringValue(imagePath),
        Reg_dt = FormatDate(data.Reg_dt),

        Pattern = MapPattern(data.Pattern),
        Warp = MapWarp(data.Warp, yarns),
        Weft = MapWeft(data.Weft, yarns),

        // 아래는 이후 단계에서 확장 구현 예정
        UsedYarns = MapUsedYarns(data.Warp, data.Weft),
        Delivery = MapDelivery(data.Warp, data.Weft),
        DimensionSpec = MapDimensionSpec(),
        YarnRepeat = MapYarnRepeat(data.Warp, data.Weft),
        
        ProcessConsumption = MapProcessConsumption(),
        Comparison = MapComparison(data.Warp, data.Weft),

        // 시뮬레이션용 항목은 일단 제외
        // PhysicalProperty = MapPhysicalProperty(data.PhysicalProperty)
      };

      return dto;
    }

    private static List<DeliveryInfoDto> MapDelivery(Warp warp, Weft weft)
    {
      return new List<DeliveryInfoDto>
      {
        new DeliveryInfoDto { Type = "경사", Threads = warp?.GetWInfoLength() ?? 0, BodyRepeat = 1, BodyCount = 1, Total = warp?.GetWInfoLength() ?? 0 },
        new DeliveryInfoDto { Type = "위사", Threads = weft?.GetWInfoLength() ?? 0, BodyRepeat = 1, BodyCount = 1, Total = weft?.GetWInfoLength() ?? 0 }
      };
    }

    private static DimensionSpecDto MapDimensionSpec()
    {
      return new DimensionSpecDto
      {
        RawWidth = 150f,
        FinishedWidth = 145f,
        ShrinkRate = 3.3f,
        Weight = 170f
      };
    }

    private static YarnRepeatDto MapYarnRepeat(Warp warp, Weft weft)
    {
      return new YarnRepeatDto
      {
        Warp = warp?.GetWInfoList()?.ConvertAll(w => new YarnSequence { Name = w.Name, Repeat = 1, Count = 1 }) ?? new List<YarnSequence>(),
        Weft = weft?.GetWInfoList()?.ConvertAll(w => new YarnSequence { Name = w.Name, Repeat = 1, Count = 1 }) ?? new List<YarnSequence>()
      };
    }

    private static List<ProcessConsumptionDto> MapProcessConsumption()
    {
      return new List<ProcessConsumptionDto>
      {
        new ProcessConsumptionDto { Type = "경사", TensionWeaving = "110", NetWeight = 80f, Draft = 10, Finish = 12, GRS = 15 },
        new ProcessConsumptionDto { Type = "위사", TensionWeaving = "90", NetWeight = 70f, Draft = 9, Finish = 11, GRS = 14 }
      };
    }

    private static List<ComparisonDto> MapComparison(Warp warp, Weft weft)
    {
      var result = new List<ComparisonDto>();

      foreach (var w in warp?.GetWInfoList() ?? new List<WInfo>())
      {
        result.Add(new ComparisonDto { YarnName = w.Name, WeightPerYard = 50f });
      }

      foreach (var w in weft?.GetWInfoList() ?? new List<WInfo>())
      {
        result.Add(new ComparisonDto { YarnName = w.Name, WeightPerYard = 45f });
      }

      return result;
    }

    private static List<UsedYarnDto> MapUsedYarns(Warp warp, Weft weft)
    {
      var result = new List<UsedYarnDto>();

      void AddYarns(List<WInfo> infos, string type)
      {
        foreach (var info in infos)
        {
          result.Add(new UsedYarnDto
          {
            Type = type,
            Name = info.Name,
            TM = 0, // TODO: TM 정보 필요시 확장
            TwistDirection = "", // TODO: 실제 값 필요
            Count = 1 // TODO: 본수/배열 관련 정보 필요
          });
        }
      }

      if (warp != null)
        AddYarns(warp.GetWInfoList(), "경사");

      if (weft != null)
        AddYarns(weft.GetWInfoList(), "위사");

      return result;
    }

    private static string FormatDate(string raw)
    {
      try { return DateTime.ParseExact(raw, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss"); }
      catch { return raw; }
    }

    private static RepeatUnitDto MapPattern(Pattern pattern)
    {
      if (pattern == null) return null;

      int rows = pattern.Data.GetLength(0);
      int cols = pattern.Data.GetLength(1);
      int[][] data = new int[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new int[cols];
        for (int j = 0; j < cols; j++)
        {
          data[i][j] = pattern.Data[i, j];
        }
      }

      return new RepeatUnitDto
      {
        Idx = pattern.Idx,
        Name = pattern.Name,
        XLength = pattern.XLength,
        YLength = pattern.YLength,
        Data = data
      };
    }

    private static WarpDto MapWarp(Warp warp, List<Yarn> yarns)
    {
      if (warp == null) return null;
      return new WarpDto
      {
        Density = warp.Density,
        WarpInfoList = warp.GetWInfoList()?.ConvertAll(info => MapYarnInfo(info, yarns))
      };
    }

    private static WeftDto MapWeft(Weft weft, List<Yarn> yarns)
    {
      if (weft == null) return null;
      return new WeftDto
      {
        Density = weft.Density,
        WeftInfoList = weft.GetWInfoList()?.ConvertAll(info => MapYarnInfo(info, yarns))
      };
    }

    private static YarnInfoDto MapYarnInfo(WInfo info, List<Yarn> yarns)
    {
      var yarn = yarns?.Find(y => y.Idx == info.IdxYarn);
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
  }
}
