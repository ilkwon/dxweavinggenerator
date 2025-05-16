using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator.Common
{
  public static class Defines
  {
    // 전역 경로 또는 상수
    public static readonly string DefaultThumbnailPath = "Resources/default_thumbnail.png";
    public static readonly string ProjectFolderPath = "Projects/";

    // enum 정의
    public enum ViewMode
    {
      List,
      Card,
      Detail
    }

    // 전역 메시지
    public static class Messages
    {
      public const string ConfirmDelete = "정말 삭제하시겠습니까?";
      public const string ErrorLoadProject = "프로젝트를 불러오는 중 오류가 발생했습니다.";
    }

    // 예시 구조체 또는 공용 데이터 구조
    public struct ProjectInfo
    {
      public string Name;
      public string ThumbnailPath;
      public DateTime CreatedDate;
    }
  }
}
