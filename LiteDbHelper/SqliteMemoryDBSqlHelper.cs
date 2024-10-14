/***
*	Title："轻量数据库" 项目
*		主题：Sqlite内存数据库的sql帮助类
*	Description：
*		功能：sql的常用功能
*	Date：2021
*	Version：0.1版本
*	Author：Coffee
*	Modify Recoder：
*/

using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace LiteDBHelper
{
    public class SqliteMemoryDBSqlHelper
    {
        #region   基础参数
        //数据库连接字符串
        private string _ConnStr;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionStr">数据库连接字符串</param>
        public SqliteMemoryDBSqlHelper(string connectionStr)
        {
            _ConnStr = connectionStr;
        }

        #region 公用方法

        /// <summary>
        /// 获取最大的Id
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="IdFieldName">Id字段名称</param>
        /// <returns></returns>
        public int GetMaxID(string tableName, string IdFieldName,ref SqliteConnection connection)
        {
            string strsql = "select max(" + IdFieldName + ")+1 from " + tableName;
            object obj = GetSingle(strsql,ref connection);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public bool Exists(string strSql,ref SqliteConnection connection)
        {
            object obj = GetSingle(strSql,ref connection);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns></returns>
        public bool Exists(string strSql,ref SqliteConnection connection, params SqliteParameter[] cmdParms)
        {
            object obj = GetSingle(strSql,ref connection, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion


        #region 执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sql,ref SqliteConnection connection)
        {

            connection = new SqliteConnection(_ConnStr);
            SqliteCommand cmd = new SqliteCommand(sql, connection);
            try
            {
                if (connection.State!=ConnectionState.Open)
                {
                    connection.Open();
                }
               
                int rows = cmd.ExecuteNonQuery();
                cmd?.Dispose();
                return rows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 执行SQL语句(设置命令的执行等待时间)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public int ExecuteSqlByWaitTime(string sql, int Times,ref SqliteConnection connection)
        {
            connection = new SqliteConnection(_ConnStr);
            SqliteCommand cmd = new SqliteCommand(sql, connection);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                cmd.CommandTimeout = Times;
                int rows = cmd.ExecuteNonQuery();
                cmd?.Dispose();

                return rows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 执行多条SQL语句(通过事务方式)
        /// </summary>
        /// <param name="sqlList">sql语句列表</param>
        public void ExecuteSqlByTransaction(List<string> sqlList,ref SqliteConnection connection)
        {
            connection = new SqliteConnection(_ConnStr);
            SqliteCommand cmd = new SqliteCommand();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            cmd.Connection = connection;
            SqliteTransaction ta = connection.BeginTransaction();
            cmd.Transaction = ta;
            try
            {
                for (int i = 0; i < sqlList.Count; i++)
                {
                    string strSql = sqlList[i].ToString();
                    if (strSql.Trim().Length > 1)
                    {
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();
                    }
                }
                ta.Commit();
                cmd?.Dispose();
            }
            catch (Exception ex)
            {
                ta.Rollback();
                throw new Exception(ex.Message);
            }

            

        }


        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImage(string sql, byte[] fs,ref SqliteConnection connection)
        {

            connection = new SqliteConnection(_ConnStr);
            SqliteCommand cmd = new SqliteCommand(sql, connection);
            SqliteParameter myParameter = new SqliteParameter("@fs", System.Data.SqlDbType.Image);
            myParameter.Value = fs;
            cmd.Parameters.Add(myParameter);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                int rows = cmd.ExecuteNonQuery();
                cmd?.Dispose();
                myParameter = null;
                return rows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="sql">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object ExecuteScalar(string sql,ref SqliteConnection connection)
        {

            connection = new SqliteConnection(_ConnStr);
            SqliteCommand cmd = new SqliteCommand(sql, connection);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                object obj = cmd.ExecuteScalar();
                cmd?.Dispose();
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader(使用该方法必须手动关闭SqlDataReader和连接)
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqliteDataReader ExecuteReader(string sql, out SqliteCommand cmd, out SqliteConnection connection)
        {
            try
            {
                connection = new SqliteConnection(_ConnStr);
                cmd = new SqliteCommand(sql, connection);
                connection.Open();
                SqliteDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //finally //不能在此关闭，否则，返回的对象将无法使用
            //{
            // cmd.Dispose();
            // connection.Close();
            //}
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public DataTable ExecuteDataTable(string sql,ref SqliteConnection sqliteConnection)
        {
            DataTable dt = new DataTable();

            try
            {
                sqliteConnection = new SqliteConnection(_ConnStr);
                SqliteCommand cmd = new SqliteCommand(sql, sqliteConnection);
                if (sqliteConnection.State!=ConnectionState.Open)
                {
                    sqliteConnection.Open();
                }

                SqliteDataReader dr = cmd.ExecuteReader();
                dt = ConvertDataReaderToDataTable(dr);
                cmd?.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dt;
        }


        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="sqliteDataReader">sqliteDataReader</param>
        /// <returns></returns>
        public DataTable ConvertDataReaderToDataTable(SqliteDataReader sqliteDataReader)
        {
            if (sqliteDataReader == null) return null;

            try
            {
                DataTable dataTable = new DataTable();
                int intFieldCount = sqliteDataReader.FieldCount;
                for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
                {
                    dataTable.Columns.Add(sqliteDataReader.GetName(intCounter), sqliteDataReader.GetFieldType(intCounter));
                }

                dataTable.BeginLoadData();

                object[] objValues = new object[intFieldCount];
                while (sqliteDataReader.Read())
                {
                    sqliteDataReader.GetValues(objValues);
                    dataTable.LoadDataRow(objValues, true);
                }
                sqliteDataReader.Close();
                dataTable.EndLoadData();

                return dataTable;

            }
            catch (Exception ex)
            {
                throw new Exception("数据转换出错!", ex);
            }
        }

        #endregion


        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sql, ref SqliteConnection connection, params SqliteParameter[] cmdParms)
        {

            connection = new SqliteConnection(_ConnStr);
            SqliteCommand cmd = new SqliteCommand();
            try
            {
                PrepareCommand(cmd, connection, null, sql, cmdParms);
                int rows = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                cmd?.Dispose();
                return rows;
            }
            catch (Exception E)
            {
                throw new Exception(E.Message);
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqlList">SQL语句的哈希表（key为sql语句，value是该语句的OleDbParameter[]）</param>
        public void ExecuteSqlTran(Hashtable sqlList,ref SqliteConnection connection)
        {
            SqliteConnection conn = new SqliteConnection(_ConnStr);
            if (connection.State!=ConnectionState.Open)
            {
                connection.Open();
            }
            using (SqliteTransaction trans = conn.BeginTransaction())
            {
                SqliteCommand cmd = new SqliteCommand();
                try
                {
                    //循环
                    foreach (DictionaryEntry myDE in sqlList)
                    {
                        string cmdText = myDE.Key.ToString();
                        SqliteParameter[] cmdParms = (SqliteParameter[])myDE.Value;
                        PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                        int val = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        trans.Commit();
                    }

                    cmd?.Dispose();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }

        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="sql">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string sql,ref SqliteConnection connection, params SqliteParameter[] cmdParms)
        {

            connection = new SqliteConnection(_ConnStr);
            if (connection.State!=ConnectionState.Open)
            {
                connection.Open();
            }

            using (SqliteCommand cmd = new SqliteCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, sql, cmdParms);
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }


        /// <summary>
        /// 执行查询语句，返回SqlDataReader (使用该方法切记要手工关闭SqlDataReader和连接)
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqliteDataReader ExecuteReader(string sql,ref SqliteConnection connection, params SqliteParameter[] cmdParms)
        {
            connection = new SqliteConnection(_ConnStr);
            SqliteCommand cmd = new SqliteCommand();
            try
            {
                PrepareCommand(cmd, connection, null, sql, cmdParms);
                SqliteDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            //finally //不能在此关闭，否则，返回的对象将无法使用
            //{
            // cmd.Dispose();
            // connection.Close();
            //}
        }

        /// <summary>
        /// 执行一个查询,并返回查询结果
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql,ref SqliteConnection connection, CommandType commandType, SqliteParameter[] parameters)
        {
             DataTable data = new DataTable();//实例化DataTable，用于装载查询结果集
             connection = new SqliteConnection(_ConnStr);
            
             using (SqliteCommand command = new SqliteCommand(sql, connection))
             {
                 command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                 //如果同时传入了参数，则添加这些参数
                 if (parameters != null)
                 {
                     foreach (SqliteParameter parameter in parameters)
                     {
                         command.Parameters.Add(parameter);
                     }
                 }

                 SqliteDataReader dr = command.ExecuteReader();
                 data = ConvertDataReaderToDataTable(dr);
                
             }
            
            return data;
        }


        /// <summary>
        /// 准备命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="conn">数据库连接</param>
        /// <param name="trans">事务</param>
        /// <param name="cmdText">命令文本</param>
        /// <param name="cmdParms">命令参数</param>
        private void PrepareCommand(SqliteCommand cmd, SqliteConnection conn, SqliteTransaction trans, string cmdText, SqliteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqliteParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                      (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion


        #region   分页查询

        /// <summary>
        ///获取到分页内容
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">分页容量</param>
        /// <param name="primaryKey">主键</param>
        /// <param name="needShowFields">显示的字段</param>
        /// <param name="tableName">表名称</param>
        /// <param name="whereString">查询条件，若有条件限制则必须以where 开头</param>
        /// <param name="orderString">排序规则</param>
        /// <param name="pageCount">传出参数：总页数统计</param>
        /// <param name="recordCount">传出参数：总记录统计</param>
        /// <returns>装载记录的DataTable</returns>
        public DataTable GetPageContent(ref int pageIndex,ref int pageSize, string primaryKey, 
            string needShowFields, string tableName, string whereString, string orderString, 
            out int pageCount, out int recordCount,ref SqliteConnection connection)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            if (string.IsNullOrEmpty(needShowFields)) needShowFields = "*";
            if (string.IsNullOrEmpty(orderString)) orderString = primaryKey + " ASC ";

            try
            {
                connection = new SqliteConnection(_ConnStr);
                if (connection.State!=ConnectionState.Open)
                {
                    connection.Open();
                }

                string myVw = string.Format("{0} tempVw ", tableName);
                SqliteCommand cmdCount = new SqliteCommand($"SELECT COUNT(*) AS recordCount FROM {tableName} {whereString}", connection);

                recordCount = Convert.ToInt32(cmdCount.ExecuteScalar());
                cmdCount?.Dispose();
                if ((recordCount % pageSize) > 0)
                    pageCount = recordCount / pageSize + 1;
                else
                    pageCount = recordCount / pageSize;


                SqliteCommand cmd;
                if (pageIndex == 1)//第一页
                {
                    cmd = new SqliteCommand($"SELECT {needShowFields} FROM {tableName} {whereString} ORDER BY {orderString} LIMIT {pageSize} OFFSET {0};", connection);
                }
                else if (pageIndex >= pageCount)//超出总页数（显示最后一页）
                {
                    int totalNumber = pageSize * pageCount;
                    int startRow = (pageCount - 1) * pageSize;

                    if (totalNumber > recordCount)
                    {
                        pageIndex = pageCount;

                        cmd = new SqliteCommand($"SELECT {needShowFields} FROM {tableName} {whereString} ORDER BY {orderString} LIMIT {pageSize} OFFSET {startRow}", connection);
                    }
                    else
                    {
                        cmd = new SqliteCommand($"SELECT {needShowFields} FROM {tableName} {whereString} ORDER BY {orderString} LIMIT {pageSize} OFFSET {startRow}", connection);
                    }
                   
                }
                else
                {
                    int startRow = (pageIndex-1) * pageSize;
                    cmd = new SqliteCommand($"SELECT {needShowFields} FROM {tableName} {whereString} ORDER BY {orderString} LIMIT {pageSize} OFFSET {startRow}", connection);

                }

                SqliteDataReader dr = cmd.ExecuteReader();
                DataTable dt = ConvertDataReaderToDataTable(dr);
                cmd?.Dispose();

                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }


        #endregion


        #region   数据库的常用操作(获取所有表、表包含的所有列信息)

        /// <summary>
        /// 返回当前连接的数据库中的所有数据表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTableInfo()
        {
            DataTable dt = new DataTable();

            SqliteDataReader dr = ExecuteReader($"SELECT NAME FROM sqlite_master WHERE TYPE='table' ORDER BY NAME",
                out SqliteCommand cmd,out SqliteConnection connection);

            dt = ConvertDataReaderToDataTable(dr);

            cmd?.Dispose();

            return dt;
        }

        /// <summary>
        /// 返回当前连接的数据库中所有由用户创建的所有数据表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserCreateAllTableInfo()
        {
            DataTable dt = new DataTable();

            SqliteDataReader dr = ExecuteReader($"SELECT NAME FROM sqlite_master WHERE TYPE='table' AND NAME<>'sqlite_sequence' ORDER BY NAME",
                out SqliteCommand cmd, out SqliteConnection connection);

            dt = ConvertDataReaderToDataTable(dr);

            cmd?.Dispose();

            return dt;

        }

        /// <summary>
        /// 获取数据库中包含的所有表名称
        /// </summary>
        /// <param name="getAllTable">获取到的所有表</param>
        /// <returns></returns>
        public List<string> GetAllTableName(DataTable getAllTableInfo)
        {
            if (getAllTableInfo == null || getAllTableInfo.Rows.Count < 1) return null;

            List<string> tableNameList = new List<string>();
            for (int i = 0; i < getAllTableInfo.Rows.Count; i++)
            {
                string tmpTableName = getAllTableInfo.Rows[i]["NAME"].ToString();
                tableNameList.Add(tmpTableName);
            }

            return tableNameList;
        }

        /// <summary>
        /// 获取到指定表的所有列信息
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public List<string> GetAllTableName(string tableName)
        {
            List<string> tableNameList = new List<string>();

            try
            {
                string strSql = $"PRAGMA table_info({tableName});";
                SqliteDataReader dr = ExecuteReader(strSql,out SqliteCommand command,out SqliteConnection connection);
                DataTable dt = ConvertDataReaderToDataTable(dr);
                command?.Dispose();

                if (dt!=null && dt.Rows.Count>0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string tmpTableName = dt.Rows[i]["NAME"].ToString();
                        tableNameList.Add(tmpTableName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
         

            return tableNameList;

        }


        #endregion

    }//Class_end

} 
