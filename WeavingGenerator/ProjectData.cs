using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace WeavingGenerator
{
  public class ProjectData
  {
    // idx
    private int idx;
    // Item Name
    private string name;
    // Create Date
    private string reg_dt;
    // PROJECTID
    private string projectID;
    // 광택 :FD, SD, BR (없음, 약함, 강함)
    private string optionMetal;
    // 인쇄 메모
    private string memo;

    // 기본 정보
    //private BasicInfo _basicInfo;
    // 조직 정보
    private Pattern _patt;
    // 경사 배열
    private Warp _warp;
    // 위사 배열
    private Weft _weft;
    // 물설 관리
    private PhysicalProperty _physicalProperty;
    //2025-02-05 soonchol
    private bool _yarnDyed;
    private String _dyeColor;

    public int Idx
    {
      get { return idx; }
      set { idx = value; }
    }
    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    public string Reg_dt
    {
      get { return reg_dt; }
      set { reg_dt = value; }
    }
    public string ProjectID
    {
      get { return projectID; }
      set { projectID = value; }
    }
    public string OptionMetal
    {
      get { return optionMetal; }
      set { optionMetal = value; }
    }
    public string Memo
    {
      get { return memo; }
      set { memo = value; }
    }

    //public BasicInfo BasicInfo
    //{
    //    get { return _basicInfo; }
    //    set { _basicInfo = value; }
    //}
    public Weft Weft
    {
      get { return _weft; }
      set { _weft = value; }
    }

    public Warp Warp
    {
      get { return _warp; }
      set { _warp = value; }
    }

    public Pattern Pattern
    {
      get { return _patt; }
      set { _patt = value; }
    }

    public PhysicalProperty PhysicalProperty
    {
      get { return _physicalProperty; }
      set { _physicalProperty = value; }
    }

    //2025-02-05 soonchol
    public bool YardDyed
    {
      get { return _yarnDyed; }
      set { _yarnDyed = value; }
    }

    public String DyeColor
    {
      get { return _dyeColor; }
      set { _dyeColor = value; }
    }

    public static Color GetDyeColor(String dyeColor)
    {
      //"#255,128,128,128"
      String src = dyeColor.Replace("#", "");
      //"255,128,128,128"
      String[] argb = src.Split(',');

      if (argb.Length != 4)
      {
        return Color.White;
      }

      int a = 255, r = 255, g = 255, b = 255;

      int.TryParse(argb[0], out r);
      int.TryParse(argb[1], out r);
      int.TryParse(argb[2], out g);
      int.TryParse(argb[3], out b);

      return Color.FromArgb(a, r, g, b);
    }

    public static String GetDyeColor(Color dyeColor)
    {
      return $"#{dyeColor.A},{dyeColor.R},{dyeColor.G},{dyeColor.B}";
    }

    public static String GetHexDyeColor(Color dyeColor)
    {
      return $"{dyeColor.R.ToString("X2")}{dyeColor.G.ToString("X2")}{dyeColor.B.ToString("X2")}";
    }

    public string SerializeJson()
    {
      var settings = new JsonSerializerSettings
      {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
      };

      var pattern = this.Pattern;

      var wrapper = new
      {
        Name = this.Name,
        ProjectID = this.ProjectID,
        OptionMetal = this.OptionMetal,
        Memo = Util.Base64Encode(this.Memo),
        YarnDyed = this.YardDyed ? "Y" : "N",
        DyeColor = this.DyeColor,
        Reg_dt = this.Reg_dt,
        Pattern = pattern != null ? new
        {
          Idx = pattern.Idx,
          Name = pattern.Name,
          XLength = pattern.XLength,
          YLength = pattern.YLength,
          Data = ConvertPatternData(pattern.Data)
        } : null,
        Warp = this.Warp,
        Weft = this.Weft,
        PhysicalProperty = this.PhysicalProperty
      };

      return JsonConvert.SerializeObject(wrapper, settings);
    }

    private List<List<int>> ConvertPatternData(int[,] data)
    {
      var result = new List<List<int>>();
      for (int i = 0; i < data.GetLength(0); i++)
      {
        var row = new List<int>();
        for (int j = 0; j < data.GetLength(1); j++)
        {
          row.Add(data[i, j]);
        }
        result.Add(row);
      }
      return result;
    }
  }

  public class BasicInfo
  {
    private int idx;
    // Item Name
    private string itemName;
    // Create Date
    private string createDate;
    // PROJECTID
    private string projectID;
    // 광택 :FD, SD, BR (없음, 약함, 강함)
    private string optionMetal;

    public int Idx
    {
      get { return idx; }
      set { idx = value; }
    }
    public string ItemName
    {
      get { return itemName; }
      set { itemName = value; }
    }

    public string CreateDate
    {
      get { return createDate; }
      set { createDate = value; }
    }

    public string ProjectID
    {
      get { return projectID; }
      set { projectID = value; }
    }

    public string OptionMetal
    {
      get { return optionMetal; }
      set { optionMetal = value; }
    }

  }

  // 조직 정보
  public class Pattern
  {
    private int idx;
    private string name;
    private int xLength;
    private int yLength;
    private int[,] data;

    public int Idx
    {
      get { return idx; }
      set { idx = value; }
    }
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    public int XLength
    {
      get { return xLength; }
      set { xLength = value; }
    }

    public int YLength
    {
      get { return yLength; }
      set { yLength = value; }
    }

    public int[,] Data
    {
      get { return data; }
      set
      {
        yLength = value.GetLength(0);
        xLength = value.GetLength(1);
        data = value;
      }
    }
  }

  public class SerializableProjectWrapper
  {
    public string Name { get; set; }
    public string ProjectID { get; set; }
    public string OptionMetal { get; set; }
    public string Memo { get; set; }
    public string YarnDyed { get; set; }
    public string DyeColor { get; set; }
    public string Reg_dt { get; set; }

    public object Pattern { get; set; }
    public Warp Warp { get; set; }
    public Weft Weft { get; set; }
    public PhysicalProperty PhysicalProperty { get; set; }
  }

  public class WRepeat
  {
    private int startIdx;
    private int endIdx;
    private int repeatCnt;

    public int StartIdx
    {
      get { return startIdx; }
      set { startIdx = value; }
    }
    public int EndIdx
    {
      get { return endIdx; }
      set { endIdx = value; }
    }
    public int RepeatCnt
    {
      get { return repeatCnt; }
      set { repeatCnt = value; }
    }
  }
  //경사 정보
  public class WInfo
  {
    private int idx;
    private string name;
    private string hexColor;
    private int idxYarn;


    public string HexColor
    {
      get { return hexColor; }
      set { hexColor = value; }
    }
    public void SetColor(Color color)
    {
      hexColor = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
      Trace.WriteLine("hex color : " + hexColor);
    }

    public Color GetColor()
    {
      int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
      int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
      int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);
      return Color.FromArgb(r, g, b);
    }

    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    [DisplayName("Idx")]
    public int Idx
    {
      get { return idx; }
      set { idx = value; }
    }


    public int IdxYarn
    {
      get { return idxYarn; }
      set { idxYarn = value; }
    }

  }
  public class WArray
  {
    private int idx;
    private int count;

    public int Idx
    {
      get { return idx; }
      set { idx = value; }
    }
    public int Count
    {
      get { return count; }
      set { count = value; }
    }
  }

  // 경사 배열
  public class Warp
  {
    private int density; // 밀도
    private int warpCount = 0;
    List<WInfo> wInfoList = new List<WInfo>();
    List<WArray> wArrayList = new List<WArray>();
    List<WRepeat> wRepeatList = new List<WRepeat>();

    public int Density
    {
      get { return density; }
      set { density = value; }
    }

    public int WarpCount
    {
      get { return warpCount; }
      set { warpCount = value; }
    }

    [JsonProperty("WarpInfoList")]
    public List<WInfo> WarpInfoList
    {
      get { return wInfoList; }
      set { wInfoList = value; }
    }

    [JsonProperty("WarpArray")]
    public List<WArray> WarpArray
    {
      get { return wArrayList; }
      set { wArrayList = value; }
    }

    [JsonProperty("Repeat")]
    public List<WRepeat> WarpRepeat
    {
      get { return wRepeatList; }
      set { wRepeatList = value; }
    }

    public void AddWInfo(WInfo wInfo)
    {
      wInfoList.Add(wInfo);
    }
    public WInfo GetWInfo(int i)
    {
      if (wInfoList.Count <= 0)
      {
        return null;
      }
      for (int j = 0; j < wInfoList.Count; j++)
      {
        WInfo winfo = wInfoList[j];
        if (winfo.Idx == i)
        {
          return winfo;
        }
      }
      return null;
    }
    public void SetWInfoList(List<WInfo> wInfoList)
    {
      this.wInfoList = wInfoList;
    }
    public int GetWInfoLength()
    {
      return wInfoList.Count();
    }
    public List<WInfo> GetWInfoList()
    {
      return wInfoList;
    }



    public void AddWArray(WArray array)
    {
      this.wArrayList.Add(array);
    }
    public void SetWArrayList(List<WArray> list)
    {
      this.wArrayList = list;
    }
    public List<WArray> GetWArrayList()
    {
      return wArrayList;
    }

    public int GetWArrayLength()
    {
      int cnt = 0;
      for (int i = 0; i < wArrayList.Count; i++)
      {
        WArray info = wArrayList[i];
        cnt += info.Count;
      }
      return cnt;
    }

    public int[] GetWArrayInt()
    {
      ///////////////////////////////////////////////////////////////////
      // 반복 설정
      ///////////////////////////////////////////////////////////////////
      List<int> idxList = new List<int>();
      // 1. 인덱스를 추가 반복
      for (int i = 0; i < wArrayList.Count; i++)
      {
        idxList.Add(i);
      }
      // 2. 인덱스를 반복
      for (int i = 0; i < wRepeatList.Count; i++)
      {
        WRepeat r = wRepeatList[i];
        int start = r.StartIdx;
        int end = r.EndIdx;
        int cnt = r.RepeatCnt;
        SetWRepeatList(idxList, start, end, cnt);
      }


      List<int> returnList = new List<int>();

      for (int i = 0; i < idxList.Count; i++)
      {
        int idx = idxList[i];
        WArray info = wArrayList[idx];
        int warpIdx = info.Idx;
        int warpCnt = info.Count;
        for (int j = 0; j < warpCnt; j++)
        {
          returnList.Add(warpIdx);
        }
      }

      //
      int[] arr = new int[returnList.Count];
      for (int i = 0; i < returnList.Count; i++)
      {
        arr[i] = returnList[i];
      }
      return arr;
    }
    private void SetWRepeatList(List<int> idxList, int s, int e, int n)
    {
      List<int> tempList = new List<int>();
      int sPos = -1;
      int ePos = -1;

      for (int i = 0; i < idxList.Count; i++)
      {
        int idx = idxList[i];

        if (idx >= s && idx <= e)
        {
          if (sPos == -1)
          {
            sPos = i;
            ePos = i;
          }
          ePos = Math.Max(ePos, i);
          tempList.Add(idx);
        }
      }

      //Trace.Write("\r\n========== tempList ==============\r\n");
      for (int i = 0; i < tempList.Count; i++)
      {
        int idx = tempList[i];
        //Trace.Write(idx + "\t");
      }
      //Trace.Write("\r\n");
      //Trace.WriteLine("sPos : " + sPos + ", ePos : " + ePos);

      idxList.RemoveRange(sPos, (ePos - sPos) + 1);
      //Trace.WriteLine("remove idxList.Count : " + idxList.Count);
      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < tempList.Count; j++)
        {
          int idx = tempList[j];
          idxList.Insert(sPos + j, idx);
        }
      }

    }
    public void SetWRepeatList(List<WRepeat> list)
    {
      this.wRepeatList = list;
    }
    public List<WRepeat> GetWRepeatList()
    {
      return wRepeatList;
    }
  }
  // 위사 배열
  public class Weft
  {
    private int density;
    private int weftCount = 0;

    List<WInfo> wInfoList = new List<WInfo>();
    List<WArray> wArrayList = new List<WArray>();
    List<WRepeat> wRepeatList = new List<WRepeat>();

    public int Density
    {
      get { return density; }
      set { density = value; }
    }

    [JsonProperty("WeftCount")]
    public int WeftCount
    {
      get { return weftCount; }
      set { weftCount = value; }
    }

    [JsonProperty("WeftInfoList")]
    public List<WInfo> WeftInfoList
    {
      get { return wInfoList; }
      set { wInfoList = value; }
    }

    [JsonProperty("WeftArray")]
    public List<WArray> WeftArray
    {
      get { return wArrayList; }
      set { wArrayList = value; }
    }

    [JsonProperty("Repeat")]
    public List<WRepeat> WeftRepeat
    {
      get { return wRepeatList; }
      set { wRepeatList = value; }
    }

    public void AddWInfo(WInfo wInfo)
    {
      wInfoList.Add(wInfo);
    }
    public WInfo GetWInfo(int i)
    {
      if (wInfoList.Count <= 0)
      {
        return null;
      }
      for (int j = 0; j < wInfoList.Count; j++)
      {
        WInfo winfo = wInfoList[j];
        if (winfo.Idx == i)
        {
          return winfo;
        }
      }
      return null;
    }
    public void SetWInfoList(List<WInfo> wInfoList)
    {
      this.wInfoList = wInfoList;
    }
    public int GetWInfoLength()
    {
      return wInfoList.Count();
    }
    public List<WInfo> GetWInfoList()
    {
      return wInfoList;
    }


    public void AddWArray(WArray array)
    {
      this.wArrayList.Add(array);
    }
    public void SetWArrayList(List<WArray> list)
    {
      this.wArrayList = list;
    }
    public List<WArray> GetWArrayList()
    {
      return wArrayList;
    }
    public int GetWArrayLength()
    {
      int cnt = 0;
      for (int i = 0; i < wArrayList.Count; i++)
      {
        WArray info = wArrayList[i];
        cnt += info.Count;
      }
      return cnt;
    }
    public void RemoveAllWArray()
    {
      wArrayList.Clear();
    }

    public int[] GetWArrayInt()
    {
      ///////////////////////////////////////////////////////////////////
      // 반복 설정
      ///////////////////////////////////////////////////////////////////
      List<int> idxList = new List<int>();
      // 1. 인덱스를 추가 반복
      for (int i = 0; i < wArrayList.Count; i++)
      {
        idxList.Add(i);
      }
      // 2. 인덱스를 반복
      for (int i = 0; i < wRepeatList.Count; i++)
      {
        WRepeat r = wRepeatList[i];
        int start = r.StartIdx;
        int end = r.EndIdx;
        int cnt = r.RepeatCnt;
        SetWRepeatList(idxList, start, end, cnt);
      }


      List<int> returnList = new List<int>();

      for (int i = 0; i < idxList.Count; i++)
      {
        int idx = idxList[i];
        WArray info = wArrayList[idx];
        int weftIdx = info.Idx;
        int weftCnt = info.Count;
        for (int j = 0; j < weftCnt; j++)
        {
          returnList.Add(weftIdx);
        }
      }

      //
      int[] arr = new int[returnList.Count];
      for (int i = 0; i < returnList.Count; i++)
      {
        arr[i] = returnList[i];
      }
      return arr;
    }



    public void AddWRepeat(WRepeat r)
    {
      this.wRepeatList.Add(r);
    }
    private void SetWRepeatList(List<int> idxList, int s, int e, int n)
    {
      List<int> tempList = new List<int>();
      int sPos = -1;
      int ePos = -1;

      for (int i = 0; i < idxList.Count; i++)
      {
        int idx = idxList[i];

        if (idx >= s && idx <= e)
        {
          if (sPos == -1)
          {
            sPos = i;
            ePos = i;
          }
          ePos = Math.Max(ePos, i);
          tempList.Add(idx);
        }
      }

      //Trace.Write("\r\n========== tempList ==============\r\n");
      for (int i = 0; i < tempList.Count; i++)
      {
        int idx = tempList[i];
        //Trace.Write(idx + "\t");
      }
      //Trace.Write("\r\n");
      //Trace.WriteLine("sPos : " + sPos + ", ePos : " + ePos);

      idxList.RemoveRange(sPos, (ePos - sPos) + 1);
      //Trace.WriteLine("remove idxList.Count : " + idxList.Count);
      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < tempList.Count; j++)
        {
          int idx = tempList[j];
          idxList.Insert(sPos + j, idx);
        }
      }

    }
    public void SetWRepeatList(List<WRepeat> list)
    {
      this.wRepeatList = list;
    }
    public List<WRepeat> GetWRepeatList()
    {
      return wRepeatList;
    }
  }

  //물성관리
  public class PhysicalProperty
  {
    // 굽힘강도 (위사/경사)
    // 외부의 힘에 의해 원단이 굽는 부분(모서리)에서 전체 굽힘 강도 힘의 몇 %를 사용할 지 조절합니다
    // 0 ~ 100
    // bendingWarp, bendingWeft
    private int bendingWarp = 0;
    private int bendingWeft = 0;
    // 내부 댐핑
    // 내부 댐핑은 의상이 찰랑거릴 때 늘어나거나 줄어드는 속도에 대한 반발력의 세기를 조절하기 위해 사용합니다.
    // 내부 댐핑 값이 높으면 의상은 물속에 있는 것처럼 느리게 움직이고, 낮으면 매우 빠르게 찰랑거립니다.
    // Internal Damping
    private int internalDamping = 0;
    // 마찰 계수
    // 의상의 마찰력을 조절하기 위해 사용합니다. 마찰력은 의상과 의상 사이의 마찰에만 영향을 줍니다
    // Friction
    private int friction = 0;
    // 밀도 조절
    // 밀도는 단위(m²) 면적당 무게 비율을 나타내기 위해 사용하며 값이 높을수록 원단이 무거워집니다.
    // 0 ~ 100
    // Density
    private int density = 0;
    // 신축성
    // 위사 강도, 경사 강도, 바이어스 신축성은 패턴창을 기준으로 가로, 세로, 대각선 방향의 신축에 대한 반발력의 세기를 나타내기 위해 사용합니다.
    // 바이어스 신축성을 위사, 경사 강도와 같은 비율로 높이면 면이나 데님과 같이 구김이 잘 가는 소재를 표현 할 수 있고,
    // 바이어스 신축성을 위사, 경사 강도에 비해 낮추면 저지, 실크와 같이 신축성이 좋은 소재를 표현 할 수 있습니다.
    // Stretch-Weft/Warp
    private int stretchWeft = 0;
    private int stretchWarp = 0;
    // 좌굴점 강도
    // 굽힘 강도 위사/ 경사와 함께 사용되며, 외부의 힘에 의해 원단이 굽는 부분(모서리)에서 전체 굽힘 강도 힘의 몇 %를 사용할 지 조절합니다.
    // 예를 들어, 굽힘 강도 값이 60일 때 좌굴점 강도를 80으로 입력하면 원단이 굽는 부분의 실제 강도는 굽힘 강도값 60의 80%인 48이 됩니다.
    // 즉 좌굴점 강도의 값이 높을수록 원단의 모서리 부분이 잘 굽지 않고, 값이 낮을수록 잘 꺾이게 됩니다.
    // Buckling Stiffness-Weft/Warp
    private int bucklingStiffnessWeft = 0;
    private int bucklingStiffnessWarp = 0;


    public int BendingWarp
    {
      get { return bendingWarp; }
      set { bendingWarp = value; }
    }
    public int BendingWeft
    {
      get { return bendingWeft; }
      set { bendingWeft = value; }
    }
    public int InternalDamping
    {
      get { return internalDamping; }
      set { internalDamping = value; }
    }
    public int Friction
    {
      get { return friction; }
      set { friction = value; }
    }
    public int Density
    {
      get { return density; }
      set { density = value; }
    }
    public int StretchWeft
    {
      get { return stretchWeft; }
      set { stretchWeft = value; }
    }
    public int StretchWarp
    {
      get { return stretchWarp; }
      set { stretchWarp = value; }
    }
    public int BucklingStiffnessWeft
    {
      get { return bucklingStiffnessWeft; }
      set { bucklingStiffnessWeft = value; }
    }
    public int BucklingStiffnessWarp
    {
      get { return bucklingStiffnessWarp; }
      set { bucklingStiffnessWarp = value; }
    }
  }

  public class Yarn
  {
    private int idx = 0;
    private string name;
    private string weight = "";
    private string unit = "";
    private string type = "";
    private string textured = "";
    private string image = "";
    private string metal = "";
    private string reg_dt = "";

    public int Idx
    {
      get { return idx; }
      set { idx = value; }
    }

    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    public string Weight
    {
      get { return weight; }
      set { weight = value; }
    }
    public string Unit
    {
      get { return unit; }
      set { unit = value; }
    }
    public string Type
    {
      get { return type; }
      set { type = value; }
    }
    public string Textured
    {
      get { return textured; }
      set { textured = value; }
    }
    public string Metal
    {
      get { return metal; }
      set { metal = value; }
    }
    public string Image
    {
      get { return image; }
      set { image = value; }
    }
    public string Reg_dt
    {
      get { return reg_dt; }
      set { reg_dt = value; }
    }

  }
}
