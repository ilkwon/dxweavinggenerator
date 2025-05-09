using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DevExpress.XtraEditors;
using Jm.DBConn;
using Newtonsoft.Json;
//using WeavingGenerator.Models;
using WeavingGenerator.Services;
using WeavingGenerator.Views;

namespace WeavingGenerator
{
  public class ProjectController
  {
    private const string Default_DyeColor = "#255,255,255,255";


    private static int _selectedProjectIdx = -1;
    public static int SelectedProjectIdx
    {
      get => _selectedProjectIdx;
      set => _selectedProjectIdx = value;
    }
    //-----------------------------------------------------------------------
    private List<ProjectData> prjList = new();
    public ProjectController()
    {
      prjList = ProjectDataRepository.ListDAOProjectData();
    }
    public List<ProjectData> ProjectDataList => prjList;
    public ProjectData GetProjectData() => GetProjectData(_selectedProjectIdx);
    public ProjectData GetProjectData(int idx)
    {
      for (int i = 0; i < prjList.Count; i++)
      {
        ProjectData data = prjList[i];
        if (data.Idx == idx)
          return data;
      }
      return null;
    }

    public void RemoveProjectData(int idx)
    {
      for (int i = 0; i < prjList.Count; i++)
      {
        var data = prjList[i];
        if (data.Idx == idx)
        {
          prjList.RemoveAt(i);
          break;
        }
      }
    }

    //-----------------------------------------------------------------------
    public ProjectData GetDAOProjectData(int idx)
    {
      var paramMap = new Dictionary<string, object> { { "@idx", idx } };
      var dataResult = DBConn.Instance.select("select_tb_project_by_idx", paramMap);
      if (dataResult?.Count > 0)
      {
        string json = dataResult.Data[0]["PROJECT_DATA"].ToString();
        var data = ProjectDataParser.Parse(json);
        data.Idx = idx;
        return data;
      }
      return null;
    }
    //-----------------------------------------------------------------------
    public int SaveDAOProjectData(string name, string projectJson)
    {
      int idx = -1;
      var paramMap = new Dictionary<string, object>
      {
        { "@name", name },
        { "@reg_dt", DateTime.Now.ToString("yyyyMMddHHmmss") },
        { "@project_data", projectJson }
      };

      DBConn.Instance.insert("insert_tb_project", paramMap);
      var result = DBConn.Instance.select("select_last_insert_rowid", new());
      if (result?.Count > 0)
        idx = Convert.ToInt32(result.Data[0]["IDX"]);
      return idx;
    }
    //-------------------------------------------------------------------
    public void RemoveDAOWeavingData(int idx)
    {
      try
      {
        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("@idx", idx);
        DBConn.Instance.delete("delete_tb_project_by_idx", paramMap);
      }
      catch (Exception ex)
      {
        Trace.Write(ex.ToString());
        XtraMessageBox.Show("Error", "Msg Box Title");
      }
    }

    //-----------------------------------------------------------------------   
    public string CreateDefaultProjectDataFromJsonFile(string name)
    {
      string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "json", "default_project.json");
      string template = File.ReadAllText(path, Encoding.UTF8);
      var data = JsonConvert.DeserializeObject<ProjectData>(template);
      data.Name = name;
      data.ProjectID = Util.GenerateUUID();
      data.Memo = Util.Base64Encode("중량:\\r\\n폭:\\r\\n혼용률:\\r\\n");
      data.DyeColor = Default_DyeColor;
      data.Reg_dt = DateTime.Now.ToString("yyyyMMddHHmmss");
      return JsonConvert.SerializeObject(data, Formatting.Indented);
    }
    //-----------------------------------------------------------------------
    public ProjectData CreateDefaultProjectData(string name)
    {
      string str = CreateDefaultProjectDataFromJsonFile(name);
      return ProjectDataParser.Parse(str);
    }
    public List<Yarn> ListDAOYarn()
    {
      List<Yarn> list = new List<Yarn>();

      try
      {
        Dictionary<string, object> paramMap = new Dictionary<string, object>(); // 파라미터 없음
        DataResult dataResult = DBConn.Instance.select("select_yarn_list", paramMap);

        if (dataResult != null && dataResult.Count > 0)
        {
          foreach (var row in dataResult.Data)
          {
            Yarn yarn = new Yarn();
            yarn.Idx = Convert.ToInt32(row["IDX"]);
            yarn.Name = Util.StripSlashes(row["NAME"].ToString());
            yarn.Weight = row["WEIGHT"].ToString();
            yarn.Unit = row["UNIT"].ToString();
            yarn.Type = row["TYPE"].ToString();
            yarn.Textured = row["TEXTURED"].ToString();
            yarn.Metal = row["METAL"].ToString();
            yarn.Image = row["IMAGE"].ToString();
            yarn.Reg_dt = row["REG_DT"].ToString();
            list.Add(yarn);
          }
        }
      }
      catch (Exception ex)
      {
        Trace.Write(ex.ToString());
        XtraMessageBox.Show("Error", "Msg Box Title");
      }

      return list;
    }
    //-----------------------------------------------------------
    public bool UpdateDAOYarn(Yarn yarn)
    {
      DateTime dt = DateTime.Now;
      try
      {
        int idx = yarn.Idx;
        string name = yarn.Name;
        string weight = yarn.Weight;
        string unit = yarn.Unit;
        string type = yarn.Type;
        string textured = yarn.Textured;
        string metal = yarn.Metal;
        string image = yarn.Image;
        string reg_dt = yarn.Reg_dt;

        if (string.IsNullOrEmpty(weight)) weight = "50";
        if (string.IsNullOrEmpty(unit)) unit = "Denier";
        if (string.IsNullOrEmpty(type)) type = "장섬유";
        if (string.IsNullOrEmpty(textured)) textured = "Filament";

        name = Util.AddSlashes(name);

        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("@name", name);
        paramMap.Add("@weight", weight);
        paramMap.Add("@unit", unit);
        paramMap.Add("@type", type);
        paramMap.Add("@textured", textured);
        paramMap.Add("@metal", metal);
        paramMap.Add("@image", image);
        paramMap.Add("@idx", idx);

        int nCount = DBConn.Instance.update("update_tb_yarn_by_idx", paramMap);
        if (nCount == 0)
        {
          // 갱신 안됨...
        }
      }
      catch (Exception ex)
      {
        Trace.Write(ex.ToString());
        XtraMessageBox.Show("Error", "Msg Box Title");
        return false;
      }

      return true;
    }
    //-----------------------------------------------------------
    public void RemoveDAOYarn(int idx)
    {
      try
      {
        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("@idx", idx);

        int affectedRows = DBConn.Instance.update("soft_delete_tb_yarn_by_idx", paramMap);
        if (affectedRows == 0)
        {
          // 삭체된게 없음.
        }
      }
      catch (Exception ex)
      {
        Trace.Write(ex.ToString());
        XtraMessageBox.Show("Error", "Msg Box Title");
      }

    }
    //-------------------------------------------------------------------
    public int SaveDAOYarn(Yarn yarn)
    {
      int idx = -1;
      DateTime dt = DateTime.Now;
      try
      {
        string name = yarn.Name;
        string weight = yarn.Weight;
        string unit = yarn.Unit;
        string type = yarn.Type;
        string textured = yarn.Textured;
        string image = yarn.Image;
        string metal = yarn.Metal;
        string reg_dt = dt.ToString("yyyyMMddhhmmss");

        if (string.IsNullOrEmpty(weight)) weight = "50";
        if (string.IsNullOrEmpty(unit)) unit = "Denier";
        if (string.IsNullOrEmpty(type)) type = "장섬유";
        if (string.IsNullOrEmpty(textured)) textured = "Filament";

        name = Util.AddSlashes(name);

        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("@name", name);
        paramMap.Add("@weight", weight);
        paramMap.Add("@unit", unit);
        paramMap.Add("@type", type);
        paramMap.Add("@textured", textured);
        paramMap.Add("@metal", metal);
        paramMap.Add("@image", image);
        paramMap.Add("@reg_dt", reg_dt);

        int nRow = DBConn.Instance.insert("insert_tb_yarn", paramMap);
        if (nRow == 0)
        {
          // 갱신 안됨...
        }
        // 마지막 Insert ID 가져오기
        Dictionary<string, object> emptyParam = new Dictionary<string, object>();
        DataResult dataResult = DBConn.Instance.select("select_last_insert_rowid", emptyParam);

        if (dataResult != null && dataResult.Count > 0)
        {
          idx = Convert.ToInt32(dataResult.Data[0]["IDX"]);
        }
      }
      catch (Exception ex)
      {
        Trace.Write(ex.ToString());
        XtraMessageBox.Show("Error", "Msg Box Title");
      }

      return idx;
    }

    //-------------------------------------------------------------------
    public void UpdateDAOProjectData(int idx, ProjectData data)
    {
      string name = data.Name;
      string reg_dt = data.Reg_dt;
      string jsonData = data.SerializeJson();

      try
      {
        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("@name", name);
        paramMap.Add("@project_data", jsonData);
        paramMap.Add("@idx", idx);

        DBConn.Instance.update("update_tb_project_by_idx", paramMap);
      }
      catch (Exception ex)
      {
        Trace.Write(ex.ToString());
        XtraMessageBox.Show("Error", "Msg Box Title");
      }
    }
  }
}
