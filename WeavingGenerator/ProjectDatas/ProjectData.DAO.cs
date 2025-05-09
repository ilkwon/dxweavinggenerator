using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.XtraEditors;
using Jm.DBConn;
using WeavingGenerator.ProjectDatas;


namespace WeavingGenerator.ProjectDatas
{
  public partial class ProjectData
  {
    public static class DAO
    {
      //---------------------------------------------------------------------
      public static void CreateTable()
      {
        DBConn.Instance.create("create_tb_project");
      }
      //---------------------------------------------------------------------
      public static List<ProjectData> SelectAll()
      {
        var result = DBConn.Instance.select("select_project_list", new());
        if (result?.Count <= 0) return new List<ProjectData>();

        return result.Data.Select(row =>
        {
          var data = ProjectData.JsonParser.Parse(row["PROJECT_DATA"]?.ToString());
          data.Idx = Convert.ToInt32(row["IDX"]);
          return data;
        }).ToList();
      }
      //---------------------------------------------------------------------
      public static ProjectData SelectById(int idx)
      {
        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("IDX", idx);

        DataResult dataResult = DBConn.Instance.select("select_tb_project_by_idx", paramMap);
        if (dataResult != null && dataResult.Count > 0)
        {
          string projectDataString = dataResult.Data[0]["PROJECT_DATA"].ToString();

          ProjectData data = ProjectData.JsonParser.Parse(projectDataString);
          data.Idx = idx;

          return data;
        }

        return null;
      }
      //---------------------------------------------------------------------
      public static void Delete(int idx)
      {
        var paramMap = new Dictionary<string, object> { { "@idx", idx } };
        DBConn.Instance.delete("delete_tb_project_by_idx", paramMap);
      }

      //---------------------------------------------------------------------
      public static int Insert(string name, string projectJson)
      {
        int idx = -1;

        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("@name", name);
        paramMap.Add("@reg_dt", DateTime.Now.ToString("yyyyMMddHHmmss"));
        paramMap.Add("@project_data", projectJson);

        DBConn.Instance.insert("insert_tb_project", paramMap);

        DataResult result = DBConn.Instance.select("select_last_insert_rowid", new Dictionary<string, object>());
        if (result != null && result.Count > 0)
        {
          idx = Convert.ToInt32(result.Data[0]["IDX"]);
        }

        return idx;
      }
      //---------------------------------------------------------------------
      public static void Update(int idx, ProjectData data)
      {
        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        paramMap.Add("name", data.Name);
        paramMap.Add("project_data", data.SerializeJson());
        paramMap.Add("idx", idx);

        DBConn.Instance.update("update_tb_project_by_idx", paramMap);
      }
      //---------------------------------------------------------------------
    }
  }
}
