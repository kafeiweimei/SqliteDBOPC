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
    public class SqliteDBHelper
    {
        #region   基础参数

        //数据库连接字符串
        private SqliteConnectionStringBuilder _ConnStr;

        //获取到数据库连接字符串
        public string ConnStr { get { return _ConnStr.ConnectionString; } }

        //SqlHelper实例
        private SqliteDBSqlHelper _SqlHelper;

        //获取到SqlHelper实例
        public SqliteDBSqlHelper SqlHelper { get { return _SqlHelper; } }

        #endregion


        #region   构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connnection">连接字符串</param>
        public SqliteDBHelper(string connnection)
        {
            if (string.IsNullOrEmpty(connnection)) return;

            _ConnStr.ConnectionString = connnection;

            InstanceSqlHelper(_ConnStr.ConnectionString);
        }


        /// <summary>
        /// 构造函数（基础不加密连接）
        /// </summary>
        /// <param name="sqliteFilePathAndName">sqlite数据库文件的路径和名称（比如：@"D:\\HalmEL\\2022-4-19.db"）</param>
        /// <param name="sqliteOpenMode">sqlite数据库的打开模式</param>
        /// <param name="sqliteCacheMode">sqlite的缓存模式</param>
        /// <param name="defaultTimeout">默认超时时间（单位是：秒）</param>
        /// <param name="isUseSharePool">是否使用共享连接池（默认true表示使用）</param>
        public SqliteDBHelper(string sqliteFilePathAndName,SqliteOpenMode sqliteOpenMode, SqliteCacheMode sqliteCacheMode, 
            int defaultTimeout=30, bool isUseSharePool=true)
        {
            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource=sqliteFilePathAndName,
                Mode=sqliteOpenMode,
                Cache=sqliteCacheMode,
                //DefaultTimeout=defaultTimeout,
                //Pooling= isUseSharePool
            };

            _ConnStr = connectionString;

            InstanceSqlHelper(_ConnStr.ConnectionString);
        }


        /// <summary>
        /// 构造函数（带加密连接）
        /// </summary>
        /// <param name="sqliteFilePathAndName">sqlite数据库文件的路径和名称（比如：@"D:\\HalmEL\\2022-4-19.db"）</param>
        /// <param name="password">数据库密码</param>
        /// <param name="sqliteOpenMode">sqlite数据库的打开模式</param>
        /// <param name="sqliteCacheMode">sqlite的缓存模式</param>
        /// <param name="defaultTimeout">默认超时时间（单位是：秒）</param>
        /// <param name="isUseSharePool">是否使用共享连接池（默认true表示使用）</param>
        public SqliteDBHelper(string sqliteFilePathAndName, string password, SqliteOpenMode sqliteOpenMode, SqliteCacheMode sqliteCacheMode, 
            int defaultTimeout = 30, bool isUseSharePool = true)
        {
            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = sqliteFilePathAndName,
                Password = password,
                Mode = sqliteOpenMode,
                Cache = sqliteCacheMode,
                //DefaultTimeout = defaultTimeout,
                //Pooling = isUseSharePool
            };

            _ConnStr = connectionString;

            InstanceSqlHelper(_ConnStr.ConnectionString);
        }


        #endregion

        #region   创建Sqlite数据库、表及其字段

        /// <summary>
        /// 创建sqlite数据库
        /// </summary>
        ///<param name = "sqliteFilePathAndName" > sqlite数据库文件的路径和名称（比如：@"D:\\HalmEL\\2022-4-19.db"）</param>
        /// <returns></returns>
        public ResultInfo CreateSqliteDataBase(string sqliteFilePathAndName)
        {
            ResultInfo resultInfo = new ResultInfo();

            if (File.Exists(sqliteFilePathAndName))
            {
                resultInfo.ResultStatus = ResultStatus.Success;
                resultInfo.Message = $"{sqliteFilePathAndName} 文件已经存在！";

                return resultInfo;
            }
            try
            {
                //如果目录不存在，则创建目录
                string folder = Path.GetDirectoryName(sqliteFilePathAndName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);

                    //对登陆系统的当前用户授予该文件夹的完全访问权
                    FolderOrFileAuth.SetFolderPermission(Environment.UserName,folder);
                }

                if (!File.Exists(sqliteFilePathAndName))
                {
                    using (SqliteConnection sqliteConnection = new SqliteConnection(_ConnStr.ConnectionString))
                    {
                        sqliteConnection.Open();
                    };

                    //对登陆系统的当前用户授予该文件的完全访问权
                    FolderOrFileAuth.SetFilePermission(Environment.UserName, sqliteFilePathAndName);
                }

                resultInfo.ResultStatus = ResultStatus.Success;
                resultInfo.Message = $"{sqliteFilePathAndName} 文件创建成功！";
            }
            catch (Exception ex)
            {
                resultInfo.ResultStatus = ResultStatus.Error;
                resultInfo.Message = $"{ex.Message}";
            }

            return resultInfo;
        }

        /// <summary>
        /// 修改数据库密码
        /// </summary>
        /// <param name = "sqliteFilePathAndName" > sqlite数据库文件的路径和名称（比如：@"D:\\HalmEL\\2022-4-19.db"）</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public ResultInfo ModifySqliteDBPassword(string sqliteFilePathAndName, string oldPassword, string newPassword)
        {
            ResultInfo resultInfo = new ResultInfo();

            if (string.IsNullOrEmpty(sqliteFilePathAndName) || string.IsNullOrEmpty(oldPassword) ||
                string.IsNullOrEmpty(newPassword))
            {
                resultInfo.SetContent(ResultStatus.Error, $"内容为空", null);

                return resultInfo;
            }

            string baseConnectionString = $"Data Source={sqliteFilePathAndName}";
            var connectionString = new SqliteConnectionStringBuilder(baseConnectionString)
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = oldPassword
            };

            try
            {
                using (var connection = new SqliteConnection(connectionString.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT quote($newPassword);";
                        command.Parameters.AddWithValue("$newPassword", newPassword);
                        var quotedNewPassword = (string)command.ExecuteScalar();

                        command.CommandText = "PRAGMA rekey = " + quotedNewPassword;
                        command.Parameters.Clear();
                        command.ExecuteNonQuery();

                        resultInfo.SetContent(ResultStatus.Success, $"修改密码成功", null);
                    };
                }

                //重置当前连接密码为新密码
                _ConnStr.Password = newPassword;
                _SqlHelper = null;
                InstanceSqlHelper(_ConnStr.ConnectionString);
            }
            catch (Exception ex)
            {
                resultInfo.SetContent(ResultStatus.Error, $"{ex.Message}", null);
            }
           

            return resultInfo;
        }

        /// <summary>
        /// 创建sqlite表（可自定义字段类型）
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="fieldList">字段列表</param>
        /// <returns></returns>
        public ResultInfo CreateSqliteTable(string tableName, List<SqliteFieldInfo> fieldList)
        {
            ResultInfo resultInfo = new ResultInfo();

            if (string.IsNullOrEmpty(tableName) || fieldList == null || fieldList.Count < 1)
            {
                resultInfo.SetContent(ResultStatus.Error, "内容为空,请检查！", null);
                return resultInfo;
            }

            SqliteConnection sqliteConnection = new SqliteConnection(_ConnStr.ConnectionString);

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

                        if (i<fieldCount-1)
                        {
                            cmdText2 += AddField(fieldInfo)+",\n";
                        }
                        if (i==fieldCount-1)
                        {
                            cmdText2 += AddField(fieldInfo)+"\n";
                        }
                        
                    }
                    cmdText2 += $");";
                }

                if (sqliteConnection.State!=ConnectionState.Open)
                {
                    sqliteConnection.Open();
                    using (SqliteCommand cmd = new SqliteCommand())
                    {
                        cmd.Connection = sqliteConnection;
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
            finally
            {
                //关闭连接
                sqliteConnection.Close();
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
                _SqlHelper = new SqliteDBSqlHelper(dbConnection);
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
