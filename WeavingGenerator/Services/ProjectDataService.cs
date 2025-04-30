using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator.Services
{
    /// <summary>
    /// ProjectData에 대한 조회 관련 기능을 제공하는 정적 서비스 클래스
    /// 상태를 가지지 않으며, 반복 사용 가능한 순수 함수로 구성.
    /// </summary>
    public static class ProjectDataService
    {
        /// <summary>
        /// 리스트에서 지정된 Idx 값을 가진 ProjectData 객체 반환.
        /// </summary>
        public static ProjectData GetProjectData(int idx, List<ProjectData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ProjectData obj = list[i];
                if (obj.Idx == idx)
                {
                    return obj;
                }
            }
            return null;
        }
    }
}
