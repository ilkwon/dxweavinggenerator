using System;
using System.Xml;
using System.Data;

using System.Data.SQLite;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Jm.DBConn
{
    public delegate void SqlDBConnCloseHandler(int dbConnType);
    public class DBConn
    {
        public SqlDBConnCloseHandler sqlConnCloseHandler;

        private string dbServer;
        private XmlDocument sqlDocument;
        private static DBConn instance;

        //0이면 SQL SERVER, LocalDB 모두 연결 실패, 1이면 mdb 만성공, 2면 mdb, SQL SERVER 모두 연결 성공
        public int dbConnType = 0;

        private SQLiteConnection localDBConn;
        private SqlConnection sqlDBConn;

        public bool localDBState = false;
        public bool sqlConnState = false;

        public delegate void DBConnectDelegate(int dbConnType);

        public event DBConnectDelegate dbConnectEvent;
        //-------------------------------------------------------------------------
        public static DBConn Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBConn();
                }
                return instance;
            }
        }
        //-------------------------------------------------------------------------
        private bool useLogging = true;
        private DBConn()
        {
            if (useLogging)
                FileLogger.InitLog();
        }
        //-------------------------------------------------------------------------
        public void Init(string serverIP, string mdbPath, string xmlPath, string dbName, string uid, string password)
        {
            if (!string.IsNullOrEmpty(serverIP))
                dbServer = serverIP;
            
            initXMLFile(xmlPath);
            initMDBConnect(mdbPath, dbName, uid, password);            
            FileLogger.info("dbConnType : [" + dbConnType + "]");
        }
        
        //-------------------------------------------------------------------------
        public void setDbconnType()
        {
            if (localDBState && sqlConnState)
            {
                dbConnType = 2;
                FileLogger.info("dbConnType : [" + dbConnType + "] mdb, SQL SERVER 모두 연결 성공");
            }
            else
            {
                if (localDBState || sqlConnState)
                {
                    dbConnType = 1;
                    FileLogger.info("dbConnType : [" + dbConnType + "]");
                    FileLogger.info("mdbConnState : [" + localDBState + "]");
                    FileLogger.info("sqlConnState : [" + sqlConnState + "]");
                }
                else
                {
                    dbConnType = 0;
                    string msg = "dbConnType : [" + dbConnType + "] SQL SERVER, LocalDB 모두 연결 실패";
                    FileLogger.info(msg);
                    //UnityEngine.Debug.Log(msg);
                }
            }
        }
        //-------------------------------------------------------------------------
        public void initSQLConnect(string dbName, string uid, string password)
        {
            try
            {
                sqlDBConn = new SqlConnection();
                sqlDBConn.ConnectionString = "Data Source = " + dbServer + ";"
                                           + "user id = " + uid + ";"
                                           + "password = " + password + ";"
                                           + "Initial Catalog = " + dbName + ";";

                sqlDBConn.StateChange += new StateChangeEventHandler(StateChangeHandler);
                if (sqlDBConn.State == ConnectionState.Closed)
                {
                    sqlDBConn.Open();
                    sqlConnState = true;
                    setDbconnType();
                }
            }
            catch (Exception ex)
            {                                
                sqlConnState = false;                
                FileLogger.info(ex.StackTrace.ToString());
                //MessageBox.Show(ex.Message);                
            }

            if (dbConnectEvent != null)
            {
                FileLogger.debug("dispatch db connect finish event...");
                dbConnectEvent(dbConnType);
            }

        }
        //-------------------------------------------------------------------------
        private void StateChangeHandler(object sender, StateChangeEventArgs e)
        {   
            FileLogger.debug("SQL Server State changed.");
            if (e.CurrentState == ConnectionState.Broken || e.CurrentState == ConnectionState.Closed)
            {
                FileLogger.error("connect state has been disconnected...");
            }
            else
            {
                FileLogger.debug("sql server state : [" + e.CurrentState + "]");
            }
        }
        //-------------------------------------------------------------------------
        public void initMDBConnect(string localDB_path, string dbName, string uid, string password)
        {
            try
            {
                string connectionString = null;
                FileLogger.debug("localDB_path : [" + localDB_path + "]");

                connectionString = "Data Source=" + localDB_path + ";Pooling=true;FailIfMissing=false";
                localDBConn = new SQLiteConnection(connectionString);

                if (localDBConn.State == ConnectionState.Closed)
                {
                    FileLogger.info("LocalDB open");
                    localDBConn.Open();
                    localDBState = true;                    
                    setDbconnType();
                }

            }
            catch (Exception ex)
            {
                localDBState = false;
                //UnityEngine.Debug.Log(ex.Message);
                FileLogger.info(ex.StackTrace);
            }

            if (!string.IsNullOrEmpty(dbName) &&
                !string.IsNullOrEmpty(uid) &&
                !string.IsNullOrEmpty(password))
            {
                initSQLConnect(dbName, uid, password);
            }                            
        }
        //-------------------------------------------------------------------------
        public void closeMDB()
        {
            if (localDBConn.State == ConnectionState.Open)
            {
                localDBConn.Close();
                localDBState = false;
            }
        }

        public void closeSQL()
        {
            if (sqlDBConn.State == ConnectionState.Open)
            {
                sqlDBConn.Close();
                sqlConnState = false;
            }
        }
        //-------------------------------------------------------------------------
        private void initXMLFile(string xmlPath)
        {
            FileLogger.info("xmlPath : [" + xmlPath + "]");
            try
            {
                sqlDocument = new XmlDocument();
                sqlDocument.Load(xmlPath);
            }
            catch (Exception e)
            {
                FileLogger.info(e.StackTrace.ToString());
                MessageBox.Show("XML 파일을 읽을 수 없습니다.  [" + xmlPath + "]에 파일이 위치하는지 확인하세요.");
            }
        }
        //-------------------------------------------------------------------------
        private int ExcuteLocalDB(String sql)
        {
            try
            {
                SQLiteCommand cmd = localDBConn.CreateCommand();
                cmd.Connection = localDBConn;
                cmd.CommandText = sql;

                return cmd.ExecuteNonQuery();
            }
            catch (InvalidOperationException e)
            {
                string msg = "SQLite EXCEPTION : [" + e.Message + "]";
                Console.WriteLine(msg);
                FileLogger.info(msg);
            }

            return 0;
        }
        //-------------------------------------------------------------------------
        private int ExcuteSqlServer(String sql)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlDBConn;
                cmd.CommandText = sql;

                return cmd.ExecuteNonQuery();
            }
            catch (InvalidOperationException e)
            {
                string errorMessage = e.Message;
                string msg = "InvalidOperationException : [" + errorMessage + "]";

                FileLogger.error(msg);
                //UnityEngine.Debug.Log(msg);

                if (errorMessage.IndexOf("DBNETLIB") != -1 || errorMessage.IndexOf("네트워크 오류") != -1
                    || errorMessage.IndexOf("통신") != -1 || errorMessage.IndexOf("Network") != -1)
                {
                    FileLogger.info("SQL SERVER CONNECTION IS CLOSED. change db connection state.");
                    if (localDBState)
                        dbConnType = 1;
                }
            }

            return 0;
        }
        //-------------------------------------------------------------------------
        string GetQueryNode(string id, Dictionary<string, Object> param, XmlNode queryNode)
        {
            string query = queryNode.InnerText;

            if (param != null)
            {
                foreach (string key in param.Keys)
                {
                    if (param[key].GetType() == typeof(int))
                    {
                        string value = ((int)(param[key])).ToString();
                        query = query.Replace(key, value);
                    }
                    if (param[key].GetType() == typeof(string))
                    {
                        string value = (string)(param[key]);
                        query = query.Replace(key, "'" + value + "'");
                    }
                }
            }
            //Console.WriteLine(string.Format("id:{0}:query packet : {1}bytes" , id , System.Text.Encoding.Default.GetByteCount(query)));
            return query;
        }
        //-------------------------------------------------------------------------
        int ExcuteQuery(string query)
        {
            if (dbConnType == 1)
            {
                int localDB = ExcuteLocalDB(query);
                return localDB;

            }
            else if (dbConnType == 2)
            {
                //sqlDBConn execute
                int sqlUpdate = ExcuteSqlServer(query);
                //mdbConn execute
                return ExcuteLocalDB(query);
            }

            return 0;
        }
        //-------------------------------------------------------------------------
        public int create(string id)
        {
            if (dbConnType == 0)
            {
                Console.WriteLine("현재 연결된 DB가 없습니다.");
                return 0;
            }

            XmlNodeList createNodes = sqlDocument.SelectNodes("sqlMap/create");
            if (createNodes.Count == 0)
            {
                throw new Exception("create 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in createNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 create 쿼리가 없습니다.");
            }

            string query = queryNode.InnerText.Trim();
            FileLogger.infoSQL("SQL : [" + query + "]");

            return ExcuteQuery(query);
        }
        //-------------------------------------------------------------------------
        public int insert(string id, Dictionary<string, Object> param)
        {
            if (dbConnType == 0)
            {
                //any db not connected
                Console.WriteLine("현재 연결된 DB가 없습니다.");
                return 0;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/insert");
            if (insertNodes.Count == 0)
            {
                throw new Exception("insert 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 insert 쿼리가 없습니다.");
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            if (dbConnType == 1)
            {
                int mdbInsert = ExcuteLocalDB(query);
                return mdbInsert;
            }
            else if (dbConnType == 2)
            {
                //sqlDBConn execute
                int ret = ExcuteSqlServer(query);

                //mdbConn execute
                return ExcuteLocalDB(query);
            }

            return 0;
        }
        //-------------------------------------------------------------------------
        public int insertSingleDB(string id, Dictionary<string, Object> param, int flag)
        {
            if (dbConnType == 0)
            {
                //any db not connected
                Console.WriteLine("현재 연결된 DB가 없습니다.");
                return 0;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/insert");
            if (insertNodes.Count == 0)
            {
                throw new Exception("insert 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 insert 쿼리가 없습니다.");
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            if (flag == 1) //mdb만 insert
            {
                //mdbConn execute
                int mdbInsert = ExcuteLocalDB(query);
                FileLogger.debug("MDB만 INSERT");
                return mdbInsert;
            }
            else if (flag == 2)   //sql server db만 insert
            {
                //sqlDBConn execute
                FileLogger.debug("SQLSERVER만 INSERT");
                int sqlInsert = ExcuteSqlServer(query);
                return sqlInsert;
            }
            return 0;
        }

        //-------------------------------------------------------------------------
        // 임시: sqlserver 커넥션일 경우에만 실행
        // return값을 반환하는 프로시져 호출
        // 별도의 validation처리는 하지 않음
        public int procedure(string id, Dictionary<string, Object> param)
        {
            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/procedure");
            if (insertNodes.Count == 0)
            {
                throw new Exception("procedure 로 선언된 프로시져가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 procedure가 없습니다.");
            }

            string query = queryNode.InnerText;
            FileLogger.info(query);

            SqlCommand cmd = new SqlCommand(query, sqlDBConn);
            cmd.CommandType = CommandType.StoredProcedure;

            //parameter setting
            if (param != null)
            {
                foreach (string key in param.Keys)
                {
                    string value = (param[key]).ToString();
                    SqlParameter pInput = new SqlParameter(key, value);
                    cmd.Parameters.Add(pInput);
                }
            }

            cmd.Parameters.Add("@returnVal", SqlDbType.Int);
            cmd.Parameters["@returnVal"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            int returnVal = 0;
            returnVal = Convert.ToInt32(cmd.Parameters["@returnVal"].Value);

            FileLogger.info("StoredProcedure return value : " + returnVal);

            return returnVal;
        }

        //-------------------------------------------------------------------------
        public DataResult select(string id, Dictionary<string, Object> param)
        {
            DataResult result = null;
            IDbCommand cmd = null;

            if (dbConnType == 0)
            {
                //any db not connected
                //UnityEngine.Debug.Log("현재 연결된 DB가 없습니다.");
                return null;
            }

            if (dbConnType == 1)
            {
                //mdbConn execute
                cmd = new SQLiteCommand();
                cmd.Connection = localDBConn;
            }
            else if (dbConnType == 2)
            {
                //sqlDBConn execute
                cmd = new SqlCommand();
                cmd.Connection = sqlDBConn;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/select");
            if (insertNodes.Count == 0)
            {
                throw new Exception("select 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 select 쿼리가 없습니다.");
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            cmd.CommandText = query;

            try
            {

                IDataReader reader = cmd.ExecuteReader();
                result = new DataResult();
                using (DataTable dt = new DataTable())
                {
                    try
                    {
                        dt.Load(reader);
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.GetType().Name + ":" + ex.Message);
                    }

                    int rowCount = dt.Rows.Count;
                    result.Count = rowCount;
                    if (rowCount > 0)
                    {

                        List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
                        for (int i = 0; i < rowCount; i++)
                        {
                            Dictionary<string, object> resultItem = new Dictionary<string, object>();

                            DataRow item = dt.Rows[i];
                            foreach (DataColumn column in item.Table.Columns)
                            {
                                string key = column.ColumnName;
                                resultItem.Add(key, item[key]);
                            }
                            resultList.Add(resultItem);
                        }
                        result.Data = resultList;
                    }
                }

                return result;
            }
            catch (InvalidOperationException oex)
            {
                FileLogger.error(oex.Message);
                Console.WriteLine("InvalidOperationException:" + oex.Message);

                return result;
            }
            catch (Exception ex)
            {
                FileLogger.error(ex.Message);
                Console.WriteLine("Exception:" + ex.Message);
                return result;
            }
        }
        //-------------------------------------------------------------------------
        public DataResult selectSingleDB(string id, Dictionary<string, Object> param, int flag)
        {
            DataResult result = null;
            IDbCommand cmd = null;

            if (dbConnType == 0)
            {
                //any db not connected
                Console.WriteLine("현재 연결된 DB가 없습니다.");
                return null;
            }

            if (flag == 1)  //MDB
            {
                FileLogger.debug("LocalDB만 SELECT");
                //mdbConn execute
                cmd = localDBConn.CreateCommand();
                cmd.Connection = localDBConn;
            }
            else if (flag == 2) //SQL DB
            {
                FileLogger.debug("SQLSERVER만 SELECT");
                //sqlDBConn execute
                cmd = new SqlCommand();
                cmd.Connection = sqlDBConn;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/select");
            if (insertNodes.Count == 0)
            {
                throw new Exception("select 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 select 쿼리가 없습니다.");
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;

            try
            {
                result = new DataResult();
                IDataReader reader = cmd.ExecuteReader();

                using (DataTable dt = new DataTable())
                {
                    dt.Load(reader);
                    reader.Close();

                    int rowCount = dt.Rows.Count;
                    result.Count = rowCount;
                    if (rowCount > 0)
                    {

                        List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
                        for (int i = 0; i < rowCount; i++)
                        {
                            Dictionary<string, object> resultItem = new Dictionary<string, object>();

                            DataRow item = dt.Rows[i];
                            foreach (DataColumn column in item.Table.Columns)
                            {
                                string key = column.ColumnName;
                                resultItem.Add(key, item[key]);
                            }
                            resultList.Add(resultItem);
                        }
                        result.Data = resultList;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                FileLogger.info(e.StackTrace.ToString());
                FileLogger.info("EXCEPTION : [" + e.Message + "]");
                return result;
            }

        }
        //-------------------------------------------------------------------------
        public int update(string id, Dictionary<string, Object> param)
        {
            if (dbConnType == 0)
            {
                //any db not connected
                Console.WriteLine("현재 연결된 DB가 없습니다.");
                return 0;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/update");
            if (insertNodes.Count == 0)
            {
                throw new Exception("update 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                string log = id + " 이름으로 선언된 update 쿼리가 없습니다.";
                //UnityEngine.Debug.Log(log);
                throw new Exception(log);
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            if (dbConnType == 1)
            {
                //mdbConn execute
                int mdbUpdate = ExcuteLocalDB(query);
                return mdbUpdate;

            }
            else if (dbConnType == 2)
            {
                //sqlDBConn execute
                int sqlUpdate = ExcuteSqlServer(query);
                //mdbConn execute
                return ExcuteLocalDB(query);
            }

            return 0;
        }
        //-------------------------------------------------------------------------
        public int updateSingleDB(string id, Dictionary<string, Object> param, int flag)
        {
            if (dbConnType == 0)
            {
                //any db not connected
                //UnityEngine.Debug.Log("현재 연결된 DB가 없습니다.");
                return 0;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/update");
            if (insertNodes.Count == 0)
            {
                throw new Exception("update 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 update 쿼리가 없습니다.");
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            if (flag == 1)
            {
                FileLogger.debug("LocalDB만 UPDATE");
                //mdbConn execute
                int mdbUpdate = ExcuteLocalDB(query);
                return mdbUpdate;

            }
            else if (flag == 2)
            {
                FileLogger.debug("SQLSERVER만 UPDATE");
                //sqlDBConn execute
                int sqlUpdate = ExcuteSqlServer(query);
                return sqlUpdate;
            }
            return 0;
        }
        //-------------------------------------------------------------------------
        public int delete(string id, Dictionary<string, Object> param)
        {
            if (dbConnType == 0)
            {
                //any db not connected
                //UnityEngine.Debug.Log("현재 연결된 DB가 없습니다.");
                return 0;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/delete");
            if (insertNodes.Count == 0)
            {
                throw new Exception("update 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 delete 쿼리가 없습니다.");
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            if (dbConnType == 1)
            {
                //mdbConn execute
                int mdbDelete = ExcuteLocalDB(query);
                return mdbDelete;
            }
            else if (dbConnType == 2)
            {
                //mdbConn execute
                int ret = ExcuteSqlServer(query);
                //sqlDBConn execute
                return ExcuteLocalDB(query);
            }

            return 0;
        }
        //-------------------------------------------------------------------------
        public int deleteSingleDB(string id, Dictionary<string, Object> param, int flag)
        {
            if (dbConnType == 0)
            {
                //any db not connected
                //UnityEngine.Debug.Log("현재 연결된 DB가 없습니다.");
                return 0;
            }

            XmlNodeList insertNodes = sqlDocument.SelectNodes("sqlMap/delete");
            if (insertNodes.Count == 0)
            {
                throw new Exception("update 로 선언된 쿼리가 없습니다.");
            }

            XmlNode queryNode = null;

            foreach (XmlNode item in insertNodes)
            {
                if (item.Attributes["id"].Value.Equals(id))
                {
                    queryNode = item;
                    break;
                }
            }

            if (queryNode == null)
            {
                throw new Exception(id + " 이름으로 선언된 delete 쿼리가 없습니다.");
            }

            string query = GetQueryNode(id, param, queryNode);
            FileLogger.infoSQL("SQL : [" + query + "]");

            if (flag == 1)
            {
                FileLogger.debug("MDB만 DELETE");
                //mdbConn execute
                int mdbDelete = ExcuteLocalDB(query);
                return mdbDelete;
            }
            else if (flag == 2)
            {
                FileLogger.debug("SQLSERVER만 DELETE");
                //mdbConn execute
                int sqlDelete = ExcuteSqlServer(query);
                return sqlDelete;
            }

            return 0;
        }
    }
    //-------------------------------------------------------------------------

}