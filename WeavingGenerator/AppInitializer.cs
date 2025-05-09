using System.Data.SQLite;
using System;
using WeavingGenerator.ProjectDatas;
using WeavingGenerator;
using System.IO;
using Jm.DBConn;

public static class AppInitializer
{
  //---------------------------------------------------------------------
  public static string Initialize()
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

    return AppSetting.GetAppId();
  }
}
