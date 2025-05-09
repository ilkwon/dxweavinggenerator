using Jm.DBConn;
using System.Collections.Generic;
using System;

namespace WeavingGenerator
{
  public partial class Yarn
  {
    public static class DAO
    {
      //---------------------------------------------------------------------
      public static void CreateTable()
      {
        DBConn.Instance.create("create_tb_yarn");
      }
      //---------------------------------------------------------------------
      public static List<Yarn> SelectAll()
      {
        var list = new List<Yarn>();
        var dataResult = DBConn.Instance.select("select_yarn_list", new());

        if (dataResult?.Count > 0)
        {
          foreach (var row in dataResult.Data)
          {
            list.Add(new Yarn
            {
              Idx = Convert.ToInt32(row["IDX"]),
              Name = Util.StripSlashes(row["NAME"].ToString()),
              Weight = row["WEIGHT"].ToString(),
              Unit = row["UNIT"].ToString(),
              Type = row["TYPE"].ToString(),
              Textured = row["TEXTURED"].ToString(),
              Metal = row["METAL"].ToString(),
              Image = row["IMAGE"].ToString(),
              Reg_dt = row["REG_DT"].ToString()
            });
          }
        }

        return list;
      }
      //---------------------------------------------------------------------
      public static int Insert(Yarn yarn)
      {
        var paramMap = new Dictionary<string, object>
        {
          { "@name", Util.AddSlashes(yarn.Name) },
          { "@weight", yarn.Weight ?? "50" },
          { "@unit", yarn.Unit ?? "Denier" },
          { "@type", yarn.Type ?? "장섬유" },
          { "@textured", yarn.Textured ?? "Filament" },
          { "@metal", yarn.Metal },
          { "@image", yarn.Image },
          { "@reg_dt", DateTime.Now.ToString("yyyyMMddhhmmss") }
        };

        DBConn.Instance.insert("insert_tb_yarn", paramMap);

        var result = DBConn.Instance.select("select_last_insert_rowid", new());
        return result?.Count > 0 ? Convert.ToInt32(result.Data[0]["IDX"]) : -1;
      }
      //---------------------------------------------------------------------
      public static bool Update(Yarn yarn)
      {
        var paramMap = new Dictionary<string, object>
        {
          { "@name", Util.AddSlashes(yarn.Name) },
          { "@weight", yarn.Weight ?? "50" },
          { "@unit", yarn.Unit ?? "Denier" },
          { "@type", yarn.Type ?? "장섬유" },
          { "@textured", yarn.Textured ?? "Filament" },
          { "@metal", yarn.Metal },
          { "@image", yarn.Image },
          { "@idx", yarn.Idx }
        };

        return DBConn.Instance.update("update_tb_yarn_by_idx", paramMap) > 0;
      }
      //---------------------------------------------------------------------
      public static void SoftDelete(int idx)
      {
        var paramMap = new Dictionary<string, object> { { "@idx", idx } };
        DBConn.Instance.update("soft_delete_tb_yarn_by_idx", paramMap);
      }
    }
  }
}
