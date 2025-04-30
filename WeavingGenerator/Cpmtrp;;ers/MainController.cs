using DevExpress.XtraRichEdit.Model.History;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeavingGenerator.Services;

namespace WeavingGenerator.Controllers
{
  /// <summary>
  /// MainForm에서 사용되는 상태 데이터와 기능이나 로직을 담는 ViewModel 클래스. !UI와 기능을 분리하기 위함
  /// UI와 데이터 사이의 중간자 역할을 하며, 속성 변경 시 UI에 자동 반영되도록 구현되어 있습니다.
  /// INotifyPropertyChanged
  /// </summary>
  public class MainController
  {
    public List<ProjectData> ProjectList { get; private set; } = [];
    public bool LoadProjectListFromJson(string filePath)
    {
      ProjectList = JsonService.LoadFromFile(filePath);
      return ProjectList.Count > 0;
    }

    public ProjectData GetSelectProject(int idx)
    {
      return ProjectDataService.GetProjectData(idx, ProjectList);
    }  
  }
}
