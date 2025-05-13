using Jm.DBConn;
using System;
using System.Collections.Generic;
using WeavingGenerator;

public static class AppSetting
{
  public static string APPID { get; set; }
  public static string ProductName => "WeavingGenerator";
  public static string DatabaseFileName => "weaving_ver1.db";
  public static string SqlMappingFile => "Resource/sql_acc.xml";
  //---------------------------------------------------------------------
  public static void CreateAppTableAndInsertAppId()
  {
    DBConn.Instance.create("create_tb_app");

    string appid = Util.GenerateUUID();
    DBConn.Instance.insert("insert_tb_app", new Dictionary<string, object>
    {
      { "@appid", appid },
      { "@reg_dt", DateTime.Now.ToString("yyyyMMddHHmmss") }
    });
  }
  //--------------------------------------------------------------------

  public static string GetAppId()
  {
    return APPID;
  }
}
