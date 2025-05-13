using System.Data.SQLite;
using System;
using WeavingGenerator.ProjectDatas;
using WeavingGenerator;
using System.IO;
using Jm.DBConn;
using DevExpress.XtraMap;

public static class AppInitializer
{
  //---------------------------------------------------------------------
  /// <summary>
  /// 어플리케이션 로컬디비 생성, App Id,
  /// </summary>
  public static void Initialize()
  {
    string appPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
      AppSetting.ProductName
    );
    Directory.CreateDirectory(appPath);

    string dbPath = Path.Combine(appPath, AppSetting.DatabaseFileName);
    bool isNew = !File.Exists(dbPath);

    if (isNew)
    {
      SQLiteConnection.CreateFile(dbPath);
    }

    DBConn.Instance.Init(null, dbPath, AppSetting.SqlMappingFile, null, null, null);

    if (isNew)
    {
      AppSetting.CreateAppTableAndInsertAppId();
      ProjectData.DAO.CreateTable();
      Yarn.DAO.CreateTable();
    }

    SetupAppid();    
  }

  private static void SetupAppid()
  {
    var result = DBConn.Instance.select("select_tb_appid", new());
    AppSetting.APPID = result?.Count > 0 ? result.Data[0]["APPID"].ToString() : "";
  }
}
