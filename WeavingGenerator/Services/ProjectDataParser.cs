using DevExpress.CodeParser;
using DevExpress.Utils.About;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Utils.MVVM.Internal.ILReader;

namespace WeavingGenerator.Services
{
  public static class ProjectDataParser
  {
    public static string Default_DyeColor = "#255,255,255,255";

    public static ProjectData Parse(string json)
    {
      if (string.IsNullOrEmpty(json)) return null;

      JObject root = JObject.Parse(json);
      if (root == null) return null;

      var proj = new ProjectData();
      proj.Name = root["Name"]?.ToString();
      proj.ProjectID = root["ProjectID"]?.ToString();
      proj.Reg_dt = root["Reg_dt"]?.ToString();
      proj.OptionMetal = root["OptionMetal"]?.ToString() ?? "0";
      proj.Memo = ParseMemo(root["Memo"]?.ToString());

      proj.YarnDyed = ParseYarnDyed(root);
      proj.DyeColor = ParseDyeColor(root, proj.YarnDyed);

      // 
      proj.Pattern           = ParsePattern(root["Pattern"] as JObject);
      proj.Warp              = ParseWarp(root["Warp"] as JObject);
      proj.Weft              = ParseWeft(root["Weft"] as JObject);
      proj.PhysicalProperty = ParsePhysicalProperty(root["PhysicalProperty"] as JObject);

      return proj;
    }
    //-----------------------------------------------------------------------
    private static string ParseMemo(string base64)
    {
      if (string.IsNullOrEmpty(base64))
        return "중량:\r\n폭:\r\n혼용률:\r\n";

      return Util.Base64Decode(base64);
    }
    //-----------------------------------------------------------------------
    private static bool ParseYarnDyed(JObject root)
    {
      var value = root["YarnDyed"]?.ToString()?.Trim().ToUpperInvariant();
      return value == "Y" || value == "TRUE";
    }
    //-----------------------------------------------------------------------
    private static string ParseDyeColor(JObject root, bool yardDyed)
    {
      var token = root["DyeColor"];
      if (token == null)
        return yardDyed ? Default_DyeColor : null;

      return token.ToString();
    }
    //-----------------------------------------------------------------------
    private static Pattern ParsePattern(JObject objPattern)
    {
      var pattern = new Pattern
      {
        Idx = Convert.ToInt32(objPattern["Idx"]),
        Name = objPattern["Name"]?.ToString()
      };

      int col = Convert.ToInt32(objPattern["XLength"].ToString());
      int row = Convert.ToInt32(objPattern["YLength"].ToString());
      
      int[,] data = new int[row, col];
      JArray dataArray = (JArray)objPattern["Data"];
      for (int i = 0; i < dataArray.Count; i++)
      {
        JArray rowArray = (JArray)dataArray[i];
        for (int j = 0; j < rowArray.Count; j++)
        {
          data[i, j] = Convert.ToInt32(rowArray[j].ToString());
        }
      }
      pattern.Data = data;
      return pattern;
    }
    //-----------------------------------------------------------------------
    private static Warp ParseWarp(JObject jsonWarp)
    {
      var warp = new Warp();
      warp.Density = Util.JObjectToInt(jsonWarp, "Density", 50);

      // WarpInfoList
      JArray infoList = (JArray)jsonWarp["WarpInfoList"];
      foreach (JObject jObj in infoList)
      {
        var info = new WInfo
        {
          Idx = Convert.ToInt32(jObj["Idx"]),
          Name = jObj["Name"]?.ToString(),
          IdxYarn = Util.JObjectToInt(jObj, "IdxYarn"),
          HexColor = jObj["HexColor"]?.ToString()
        };
        warp.AddWInfo(info);
      }

      // 빈 리스트 기본값
      if (infoList.Count == 0)
      {
        warp.AddWInfo(new WInfo { Idx = 0, Name = "", IdxYarn = 0, HexColor = "FFFFFF" });
      }

      // WarpArray
      var warpArray = new List<WArray>();
      foreach (JObject jObj in (JArray)jsonWarp["WarpArray"])
      {
        warpArray.Add(new WArray
        {
          Idx = Convert.ToInt32(jObj["Idx"]),
          Count = Convert.ToInt32(jObj["Count"])
        });
      }
      warp.SetWArrayList(warpArray);

      // Repeat
      var warpRepeat = new List<WRepeat>();
      foreach (JObject jObj in (JArray)jsonWarp["Repeat"] ?? new JArray())
      {
        warpRepeat.Add(new WRepeat
        {
          StartIdx = Convert.ToInt32(jObj["StartIdx"]),
          EndIdx = Convert.ToInt32(jObj["EndIdx"]),
          RepeatCnt = Convert.ToInt32(jObj["RepeatCnt"])
        });
      }
      warp.SetWRepeatList(warpRepeat);

      return warp;
    }
    //-----------------------------------------------------------------------
    private static Weft ParseWeft(JObject jsonWeft)
    {
      var weft = new Weft();
      weft.Density = Util.JObjectToInt(jsonWeft, "Density", 50);

      // WeftInfoList
      JArray infoList = (JArray)jsonWeft["WeftInfoList"];
      foreach (JObject jObj in infoList)
      {
        var info = new WInfo
        {
          Idx = Convert.ToInt32(jObj["Idx"]),
          Name = jObj["Name"]?.ToString(),
          IdxYarn = Util.JObjectToInt(jObj, "IdxYarn"),
          HexColor = jObj["HexColor"]?.ToString()
        };
        weft.AddWInfo(info);
      }

      if (infoList.Count == 0)
      {
        weft.AddWInfo(new WInfo { Idx = 0, Name = "", IdxYarn = 0, HexColor = "FFFFFF" });
      }

      // WeftArray
      var weftArrayList = new List<WArray>();
      foreach (JObject jObj in (JArray)jsonWeft["WeftArray"])
      {
        weftArrayList.Add(new WArray
        {
          Idx = Convert.ToInt32(jObj["Idx"]),
          Count = Convert.ToInt32(jObj["Count"])
        });
      }
      weft.SetWArrayList(weftArrayList);

      // Repeat
      var weftRepeat = new List<WRepeat>();
      foreach (JObject jObj in (JArray)jsonWeft["Repeat"] ?? new JArray())
      {
        weftRepeat.Add(new WRepeat
        {
          StartIdx = Convert.ToInt32(jObj["StartIdx"]),
          EndIdx = Convert.ToInt32(jObj["EndIdx"]),
          RepeatCnt = Convert.ToInt32(jObj["RepeatCnt"])
        });
      }
      weft.SetWRepeatList(weftRepeat);

      return weft;
    }
    //-----------------------------------------------------------------------
    private static PhysicalProperty ParsePhysicalProperty(JObject objPhysical)
    {
      if (objPhysical == null)
        return new PhysicalProperty(); // 혹은 null 반환도 가능

      PhysicalProperty physical = new PhysicalProperty
      {
        BendingWarp = Convert.ToInt32(objPhysical["BendingWarp"]),
        BendingWeft = Convert.ToInt32(objPhysical["BendingWeft"]),
        InternalDamping = Convert.ToInt32(objPhysical["InternalDamping"]),
        Friction = Convert.ToInt32(objPhysical["Friction"]),
        Density = Convert.ToInt32(objPhysical["Density"]),
        StretchWeft = Convert.ToInt32(objPhysical["StretchWeft"]),
        StretchWarp = Convert.ToInt32(objPhysical["StretchWarp"]),
        BucklingStiffnessWeft = Convert.ToInt32(objPhysical["BucklingStiffnessWeft"]),
        BucklingStiffnessWarp = Convert.ToInt32(objPhysical["BucklingStiffnessWarp"])
      };

      return physical;
    }
    //-----------------------------------------------------------------------
  }
}
