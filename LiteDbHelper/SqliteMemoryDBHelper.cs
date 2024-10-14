/***
*	Title："轻量数据库" 项目
*		主题：：Sqlite数据库帮助类【使用 Microsoft.Data.Sqlite 】
*	Description：
*		功能：XXX
*	Date：2022
*	Version：0.1版本
*	Author：Coffee
*	Modify Recoder：
*/

using LiteDBHelper.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace LiteDBHelper
{
    public class SqliteMemoryDBHelper
    {
        #region   基础参数

        //数据库连接字符串
        private SqliteConnectionStringBuilder _ConnStr;

        //获取到数据库连接字符串
        public string ConnStr { get { return _ConnStr.ConnectionString; } }

        //SqlHelper实例
        private SqliteMemoryDBSqlHelper _SqlHelper;

        //获取到SqlHelper实例
        public SqliteMemoryDBSqlHelper SqlHelper { get { return _SqlHelper; } }

        #endregion


        #region   构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connnection">连接字符串</param>
        public SqliteMemoryDBHelper(string connnection)
        {
            if (string.IsNullOrEmpty(connnection)) return;

            _ConnStr.ConnectionString = connnection;

            InstanceSqlHelper(_ConnStr.ConnectionString);
        }

        /// <summary>
        /// 构造函数（内存数据库）
        /// </summary>
        /// <param name="memoryDBName">内容数据库的名称（比如：memoryDB）</param>
        /// <param name="defaultTimeout">命令执行的默认超时时间（默认为：10秒）</param>
        /// <param name="isUseSharePool">是否使用共享连接池（默认true表示使用）</param>
        public SqliteMemoryDBHelper(string memoryDBName, int defaultTimeout = 10, bool isUseSharePool = true)
        {
            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = memoryDBName,
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Shared,
                //DefaultTimeout = defaultTimeout,
                //Pooling=isUseSharePool
            };

            _ConnStr = connectionString;

            InstanceSqlHelper(_ConnStr.ConnectionString);
        }


        #endregion

        #region   创建Sqlite数据库、表及其字段

        /// <summary>
        /// 创建sqlite内存数据库
        /// </summary>
        /// <returns></returns>
        public ResultInfo CreateSqliteDataBase(ref SqliteConnection connection)
        {
            ResultInfo resultInfo = new ResultInfo();

            try
            {
                connection = new SqliteConnection(_ConnStr.ConnectionString);
                connection.Open();

                resultInfo.ResultStatus = ResultStatus.Success;
                resultInfo.Message = $"内存数据库创建成功！";
            }
            catch (Exception ex)
            {
                resultInfo.ResultStatus = ResultStatus.Error;
                resultInfo.Message = $"{ex.Message}";
            }

            return resultInfo;
        }



        /// <summary>
        /// 创建sqlite表（可自定义字段类型）
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="fieldList">字段列表</param>
        /// <returns></returns>
        public ResultInfo CreateSqliteTable(string tableName, List<SqliteFieldInfo> fieldList,ref SqliteConnection connection)
        {
            ResultInfo resultInfo = new ResultInfo();

            if (string.IsNullOrEmpty(tableName) || fieldList == null || fieldList.Count < 1)
            {
                resultInfo.SetContent(ResultStatus.Error, "内容为空,请检查！", null);
                return resultInfo;
            }

            connection = new SqliteConnection(_ConnStr.ConnectionString);

            try
            {
                //创建表的命令
                string cmdText1 = $"CREATE TABLE IF NOT EXISTS {tableName} ( \n";
                string cmdText2 = string.Empty;

                //给表添加字段
                if (fieldList != null && fieldList.Count >= 1)
                {
                    int fieldCount = fieldList.Count;

                    SqliteFieldInfo fieldInfo = new SqliteFieldInfo();
                    for (int i = 0; i < fieldCount; i++)
                    {
                        fieldInfo = fieldList[i];

                        if (i < fieldCount - 1)
                        {
                            cmdText2 += AddField(fieldInfo) + ",\n";
                        }
                        if (i == fieldCount - 1)
                        {
                            cmdText2 += AddField(fieldInfo) + "\n";
                        }

                    }
                    cmdText2 += $");";
                }

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    using (SqliteCommand cmd = new SqliteCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = cmdText1 + cmdText2;
                        cmd.ExecuteNonQuery();
                    }
                }

                resultInfo.SetContent(ResultStatus.Success, $"创建：{tableName} 表成功", null);
            }
            catch (Exception ex)
            {
                resultInfo.SetContent(ResultStatus.Error, $"{ex.Message}", null);
            }

            return resultInfo;
        }

        #endregion

        #region   私有方法

        /// <summary>
        /// 实例化SqlHelper
        /// </summary>
        /// <param name="dbConnection">数据库连接字符串</param>
        private void InstanceSqlHelper(string dbConnection)
        {
            if (string.IsNullOrEmpty(dbConnection)) return;

            if (_SqlHelper == null)
            {
                _SqlHelper = new SqliteMemoryDBSqlHelper(dbConnection);
            }
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="fieldInfo">字段信息</param>
        /// <returns>返回添加字段结果</returns>
        private string AddField(SqliteFieldInfo fieldInfo)
        {
            string str = null;

            if (fieldInfo == null) return null;

            //设置指定类型的字段到表中
            str = $" {fieldInfo.Name}";

            if (fieldInfo.Length > 0)
            {
                str += $" {fieldInfo.DataType}({fieldInfo.Length})";
            }
            else
            {
                str+= $" {fieldInfo.DataType}";
            }

            if (fieldInfo.IsNotEmpty)
            {
                str += $" NOT NULL";
            }

            if (fieldInfo.IsPrimaryKey)
            {
                str += $" PRIMARY KEY";
            }

            if (fieldInfo.IsAutoIncrement)
            {
                str += $" AUTOINCREMENT";
            }

            return str;
        }


        #endregion



    }//Class_end


}
