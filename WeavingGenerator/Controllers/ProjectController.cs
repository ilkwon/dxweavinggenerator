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

namespace WeavingGenerator.Controllers
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
    public void UpdateDAOProjectData(int idx, ProjectData data)
    {
      var paramMap = new Dictionary<string, object>
      {
        { "@name", data.Name },
        { "@project_data", data.SerializeJson() },
        { "@idx", idx }
      };
      DBConn.Instance.update("update_tb_project_by_idx", paramMap);
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

    public ProjectData CreateDefaultProjectData(string name)
    {
      string str = CreateDefaultProjectDataFromJsonFile(name);
      return ProjectDataParser.Parse(str);
    }
  }
}
