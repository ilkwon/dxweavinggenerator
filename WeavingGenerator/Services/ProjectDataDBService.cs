using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.XtraEditors;
using Jm.DBConn;
using WeavingGenerator.Utils;

namespace WeavingGenerator.Services
{
  public static class ProjectDataDBService
  {
    public static List<ProjectData> LoadFromDatabase()
    {
      List<ProjectData> list = [];

      try
      {
        var paramMap = new Dictionary<string, object>();
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
        Trace.WriteLine(ex.ToString());
        XtraMessageBox.Show("Error", "프로젝트 로딩 중 오류 발생");
      }

      return list;

    }
  }
}
