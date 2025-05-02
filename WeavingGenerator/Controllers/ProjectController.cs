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
    public List<ProjectData> ProjectList => prjList;
    private List<ProjectData> prjList = new();
    private int SELECTED_IDX = -1;
    //-----------------------------------------------------------------------
    public ProjectController()
    {
      prjList = ProjectDataRepository.ListDAOProjectData();
    }
    public ProjectData GetProjectData() => GetProjectData(SELECTED_IDX);
    public ProjectData GetProjectData(int idx)
    {
      return ProjectDataService.GetProjectData(idx, prjList);
    }
    
    //-----------------------------------------------------------------------
    public int CreateProject(string name)
    {
      var data = CreateDefaultProjectData(name);
      var json = data.SerializeJson();
      int idx = SaveDAOProjectData(name, json);
      data.Idx = idx;

      prjList.Insert(0, data);
      
      return idx;
    }
    //-----------------------------------------------------------------------
    public ProjectData OpenProject(int idx)
    {
      return GetProjectData(idx);
      //SetSelectedProjectButton(idx); // UI 관련 부분은 나중에 분리
      //SetProjectData(idx, data);
    }
    //-----------------------------------------------------------------------
    private void SaveAllProject()
    {
      foreach (var obj in prjList)
      {
        UpdateDAOProjectData(obj.Idx, obj);
      }
    }
    //-----------------------------------------------------------------------
    public void RemoveProject(int idx)
    {
      prjList.RemoveAll(p => p.Idx == idx);
      RemoveDAOWeavingData(idx);
      RemoveProjectButton(idx);

      if (idx == SELECTED_IDX)
      {
        SELECTED_IDX = -1;
        ResetViewer();
      }
    }
    //-----------------------------------------------------------------------
    public void UpdateProject()
    {
      UpdateProjectView();
      if (SELECTED_IDX != -1)
      {
        var data = OpenProject(SELECTED_IDX);

      }
    }
    //-----------------------------------------------------------------------
    public ProjectData GetDAOProjectData(int idx)
    {
      var paramMap = new Dictionary<string, object> { { "IDX", idx } };
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
    private int SaveDAOProjectData(string name, string projectJson)
    {
      int idx = -1;
      var paramMap = new Dictionary<string, object>
      {
        { "name", name },
        { "reg_dt", DateTime.Now.ToString("yyyyMMddHHmmss") },
        { "project_data", projectJson }
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
        paramMap.Add("idx", idx);
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
        { "name", data.Name },
        { "project_data", data.SerializeJson() },
        { "idx", idx }
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
      return ProjectDataParser.Parse(CreateDefaultProjectDataFromJsonFile(name));
    }

    private void SetSelectedProjectButton(int idx) { /* ... */ }
    private void RemoveProjectButton(int idx) { /* ... */ }
    private void ResetViewer() { /* ... */ }
    private void UpdateProjectView() { /* ... */ }
  }
}
