using Jm.DBConn;
using System.Collections.Generic;
using System;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using System.Linq;
using System.Diagnostics;

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
                { "@IDX", 1 },
                { "@NAME", "직물 프로젝트" },
                { "@REG_DT", "20250425" }
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
        public static List<T> To<T>(this DataResult result, string columnName = null) where T : new()
        {
            var list = new List<T>();

            if (result == null || result.Data == null)
                return list;

            // string 타입이면 별도 처리
            if (typeof(T) == typeof(string))
            {
                foreach (var row in result.Data)
                {
                    object value = null;

                    if (!string.IsNullOrEmpty(columnName))
                    {
                        string key = columnName.ToUpper();
                        if (row.ContainsKey(key) && row[key] != DBNull.Value)
                        {
                            value = row[key];
                        }
                    }
                    else
                    {
                        // columnName이 없으면 첫 번째 값을 사용
                        value = row.Values.FirstOrDefault(v => v != null && v != DBNull.Value);
                    }

                    if (value != null)
                    {
                        list.Add((T)(object)value.ToString());
                    }
                }
                return list;
            }

            // 일반 클래스 타입이면 기존 방식
            foreach (var row in result.Data)
            {
                var item = new T();
                foreach (var prop in typeof(T).GetProperties())
                {
                    string key = prop.Name.ToUpper();
                    if (row.ContainsKey(key) && row[key] != DBNull.Value)
                    {
                        try
                        {
                            var value = Convert.ChangeType(row[key], prop.PropertyType);
                            prop.SetValue(item, value);
                        }
                        catch (Exception ex)
                        {
                            // 변환 실패해도 무시하고 넘어가기
                            Trace.WriteLine($"[DataResult.To<T>] 변환 실패: {key}, {ex.Message}");
                        }
                    }
                }
                list.Add(item);
            }

            return list;
        }
    }

}