using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.XtraEditors;
using Jm.DBConn;


namespace WeavingGenerator.Services
{
  public static class ProjectDataRepository
  {
    public static List<ProjectData> ListDAOProjectData()
    {
      List<ProjectData> list = new List<ProjectData>();

      try
      {
        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        DataResult dataResult = DBConn.Instance.select("select_project_list", paramMap);
        if (dataResult != null && dataResult.Count > 0)
        {
          foreach (var row in dataResult.Data)
          {
            int idx = Convert.ToInt32(row["IDX"]);
            string projectDataString = row["PROJECT_DATA"].ToString();

            ProjectData data = ProjectDataParser.Parse(projectDataString);
            data.Idx = idx;
            list.Add(data);
          }
        }
      }
      catch (Exception ex)
      {        
        XtraMessageBox.Show("ListDAOProjectData Exception: " + ex.Message);
      }

      return list;
    }

    public static ProjectData GetDAOProjectData(int idx)
    {
      Dictionary<string, object> paramMap = new Dictionary<string, object>();
      paramMap.Add("IDX", idx);

      DataResult dataResult = DBConn.Instance.select("select_tb_project_by_idx", paramMap);
      if (dataResult != null && dataResult.Count > 0)
      {
        string projectDataString = dataResult.Data[0]["PROJECT_DATA"].ToString();

        ProjectData data = ProjectDataParser.Parse(projectDataString);
        data.Idx = idx;

        return data;
      }

      return null;
    }

    public static int SaveDAOProjectData(string name, string projectJson)
    {
      int idx = -1;

      Dictionary<string, object> paramMap = new Dictionary<string, object>();
      paramMap.Add("name", name);
      paramMap.Add("reg_dt", DateTime.Now.ToString("yyyyMMddHHmmss"));
      paramMap.Add("project_data", projectJson);

      DBConn.Instance.insert("insert_tb_project", paramMap);

      DataResult result = DBConn.Instance.select("select_last_insert_rowid", new Dictionary<string, object>());
      if (result != null && result.Count > 0)
      {
        idx = Convert.ToInt32(result.Data[0]["IDX"]);
      }

      return idx;
    }

    public static void UpdateDAOProjectData(int idx, ProjectData data)
    {
      Dictionary<string, object> paramMap = new Dictionary<string, object>();
      paramMap.Add("name", data.Name);
      paramMap.Add("project_data", data.SerializeJson());
      paramMap.Add("idx", idx);

      DBConn.Instance.update("update_tb_project_by_idx", paramMap);
    }
  }
}
