using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DevExpress.XtraEditors;
using Jm.DBConn;
using Newtonsoft.Json;

using WeavingGenerator.ProjectDatas;


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
    private List<ProjectData> projects = new();
    public ProjectController()
    {
      projects = ProjectData.DAO.SelectAll();
    }
    public List<ProjectData> ProjectDataList => projects;
    public ProjectData GetProjectData() => GetProjectData(_selectedProjectIdx);
    //-----------------------------------------------------------------------   
    public ProjectData GetProjectData(int idx)
    {
      for (int i = 0; i < projects.Count; i++)
      {
        ProjectData data = projects[i];
        if (data.Idx == idx)
          return data;
      }
      return null;
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
      return ProjectData.JsonParser.Parse(str);
    }
    //-----------------------------------------------------------------------
    public void Remove(int idx)
    {
      for (int i = 0; i < projects.Count; i++)
      {
        var data = projects[i];
        if (data.Idx == idx)
        {
          projects.RemoveAt(i);
          break;
        }
      }
    }
    //-----------------------------------------------------------------------

  }
}
