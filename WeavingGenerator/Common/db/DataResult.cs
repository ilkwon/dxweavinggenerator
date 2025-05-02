using System.Collections.Generic;

namespace Jm.DBConn
{
    public class DataResult
    {
        public int Count { get; set; }

        public List<Dictionary<string, object>> Data { get; set; }
    }
}
