using Jm.DBConn;
using System.Collections.Generic;
using System;
using DevExpress.DataProcessing.InMemoryDataProcessor;

namespace Jm.DBConn
{

    /// <summary>
    /// usage
    /*-----------------------------------------------------------------
    DataResult result = new DataResult
    {
        Count = 1,
        Data = new List<Dictionary<string, object>> {
            new Dictionary<string, object> {
                { "IDX", 1 },
                { "NAME", "직물 프로젝트" },
                { "REG_DT", "20250425" }
            }
        }
    };
    
    *------------------------------------------------------------------
    public class Project {
    public int Idx { get; set; }
    public string Name { get; set; }
    public string Reg_dt { get; set; }

    var list = result.To<Project>();

    *-------------------------------------------------------------------
    DataResult raw = DBConn.Instance.select("selectAllProjects", null);
    List<Project> projects = raw.To<Project>();
    foreach (var p in projects)
    {
        Console.WriteLine($"[{p.Idx}] {p.Name} ({p.Reg_dt})");
    }
    *-------------------------------------------------------------------
    */
    /// </summary>
    public static class DataResultExtensions
    {
        public static List<T> To<T>(this DataResult result) where T : new()
        {
            var list = new List<T>();
            foreach (var row in result.Data)
            {
                var item = new T();
                foreach (var prop in typeof(T).GetProperties())
                {
                    string key = prop.Name.ToUpper(); // 대소문자 구분없게 처리
                    if (row.ContainsKey(key) && row[key] != DBNull.Value)
                    {
                        var value = Convert.ChangeType(row[key], prop.PropertyType);
                        prop.SetValue(item, value);
                    }
                }
                list.Add(item);
            }
            return list;
        }
    }
}