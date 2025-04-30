using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;

namespace WeavingGenerator.Services
{
  /// <summary>
  /// ProjectData 리스트의 JSON 입출력을 처리하는 정적 서비스 클래스.
  /// </summary>
  public static class JsonService
  {
    public static List<ProjectData> LoadFromFile(string filePath)
    {
      if (!File.Exists(filePath))
        return new List<ProjectData>();

      string json = File.ReadAllText(filePath);
      return JsonConvert.DeserializeObject<List<ProjectData>>(json) ?? new();
    }

    public static bool SaveToFile(string filePath, List<ProjectData> data, out string error)
    {
      try
      {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
        error = string.Empty;

        return true;

      } catch (IOException ex) {
        error = ex.Message;
        
        return false;
      }
    }
  }
}
