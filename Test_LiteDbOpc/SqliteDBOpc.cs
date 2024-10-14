/***
*	Title："轻量数据库" 项目
*		主题：这是关于Sqlite数据库的常用操作演示
*	Description：
*		功能：XXX
*	Date：2022
*	Version：0.1版本
*	Author：Coffee
*	Modify Recoder：
*/

using LiteDBHelper;
using Microsoft.Data.Sqlite;
using LiteDBHelper.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading;

namespace Test_LiteDbOpc
{
    internal class SqliteDBOpc
    {
        #region   基础参数

        private static string sqliteFilePathAndName = $"E:\\SqliteDB\\{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.db";
      

        //连接命令
        private static SqliteCommand cmd = null; 
        //数据库连接
        private static SqliteConnection connection;

        #endregion

        public static void Demo()
        {
            Console.WriteLine("\n\n\n------------------开始执行【本地】Sqlite数据库的创建、CRUD操作--------------------------------");


            #region   创建Sqlite数据库

            Console.WriteLine($"---------------开始创建Sqlite数据库---------------");

            ////创建基础无密码的sqlite数据库文件
            //SqliteDBHelper sqliteDBHelper = new SqliteDBHelper(sqliteFilePathAndName, SqliteOpenMode.ReadWriteCreate, SqliteCacheMode.Shared);
            //ResultInfo result = sqliteDBHelper.CreateSqliteDataBase(sqliteFilePathAndName);
            //Console.WriteLine($"创建基础无密码的Sqlite数据库情况：{result.ResultPrint()}");

            //创建有密码的sqlite数据库文件
            SqliteDBHelper sqliteDBHelper = new SqliteDBHelper(sqliteFilePathAndName, "123456",
                SqliteOpenMode.ReadWriteCreate, SqliteCacheMode.Shared);
            ResultInfo result = sqliteDBHelper.CreateSqliteDataBase(sqliteFilePathAndName);
            Console.WriteLine($"创建有密码的Sqlite数据库情况：{result.ResultPrint()}");

            //修改数据库密码
            ResultInfo modifyResult = sqliteDBHelper.ModifySqliteDBPassword(sqliteFilePathAndName, "123456", "987654qw");

            Console.WriteLine($"修改sqlite数据库密码情况：{modifyResult.ResultPrint()}");


            #endregion


            #region   创建Sqlite指定数据库的表

            //创建表方式2
            List<SqliteFieldInfo> fieldNameList2 = new List<SqliteFieldInfo>()
            {
                new SqliteFieldInfo("Id",SqliteType.Integer,0,true,true,true),
                new SqliteFieldInfo("ImageType",SqliteType.Text,16),
                new SqliteFieldInfo("AddDate",SqliteType.Text,20),
                new SqliteFieldInfo("ImagePath",SqliteType.Text,255),
                new SqliteFieldInfo("IsDisable",SqliteType.Text,5),
                new SqliteFieldInfo("Position",SqliteType.Real,20)

            };
            ResultInfo result2_2 = sqliteDBHelper.CreateSqliteTable("ImageInfo", fieldNameList2);

            Console.WriteLine($"创建Access数据库的表情况：{result2_2.ResultPrint()}");


            #endregion


            #region   插入数据
            Console.WriteLine($"\n---------------开始执行插入数据---------------");

            //插入sqlite数据库中ImageInfo表数据
            string insertSql = null;
            List<string> sqlList = new List<string>();
            for (int i = 0; i < 10; i++)
            {

                insertSql = $"Insert Into ImageInfo (ImageType,AddDate,ImagePath,IsDisable,Position) " +
                   $"Values ('jpg','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','E:\\MyProject\\Image\\{i}','{false}','{i}.25879456123');";

                sqlList.Add(insertSql);


                ////执行单条sql语句
                //int affectRow = sqliteDBHelper.SqlHelper.ExecuteSql(insertSql);

                //Console.WriteLine($"{i} 插入数据行数：{affectRow}");
            }

            //执行多条sql语句（通过事务方式）
            sqliteDBHelper.SqlHelper.ExecuteSqlByTransaction(sqlList);

            Console.WriteLine($"------------插入 {sqlList.Count} 条数据完成----------");
            #endregion

            #region   查询数据
            Console.WriteLine($"\n---------------开始查询所有数据---------------");

            string querySql = $"Select Id,ImageType,AddDate,ImagePath,IsDisable,[Position] From ImageInfo ";
            DataTable dt = sqliteDBHelper.SqlHelper.ExecuteDataTable(querySql);

            PrintDatatable(dt);

            Console.WriteLine($"---------------查询数据完成---------------");

            #endregion

            #region   更新数据
            Console.WriteLine($"\n---------------开始更新数据---------------");

            string updateSql = $"update ImageInfo set ImageType='png',AddDate='2022-04-17 10:36:36'," +
                $"ImagePath='D:\\TestProject\\image\\11',IsDisable='True',[Position]='36.258941' where Id=1  ";
            int updateData = sqliteDBHelper.SqlHelper.ExecuteSql(updateSql);

            Console.WriteLine($"---------------更新 {updateData} 条数据完成---------------");

            Console.WriteLine($"\n---------------更新数据后_开始查询所有数据---------------");

            string querySql2 = $"Select Id,ImageType,AddDate,ImagePath,IsDisable,[Position] From ImageInfo ";
            DataTable dt2 = sqliteDBHelper.SqlHelper.ExecuteDataTable(querySql2);
           
            PrintDatatable(dt2);

            Console.WriteLine($"---------------更新数据后_查询数据完成---------------");

            #endregion

            #region   删除数据

            Console.WriteLine($"\n---------------开始删除数据---------------");

            string deleteSql = $"delete from  ImageInfo where Id=10";
            int deleteData = sqliteDBHelper.SqlHelper.ExecuteSql(deleteSql);

            Console.WriteLine($"---------------删除 {updateData} 条数据完成---------------");

            Console.WriteLine($"\n--------------删除数据后_开始查询所有数据---------------");

            string querySql3 = $"Select Id,ImageType,AddDate,ImagePath,IsDisable,[Position] From ImageInfo ";
            DataTable dt3 = sqliteDBHelper.SqlHelper.ExecuteDataTable(querySql3);

            PrintDatatable(dt3);

            Console.WriteLine($"---------------删除数据后_查询数据完成---------------");

            #endregion

            #region   分页查询
            Console.WriteLine($"\n---------------开始分页查询数据---------------");

            int pageIndex = 3, pageSize = 3;

            DataTable pageDt = sqliteDBHelper.SqlHelper.GetPageContent(ref pageIndex,ref pageSize, "Id", 
                "Id,ImageType,AddDate,ImagePath,IsDisable,[Position]", "ImageInfo", "where ImageType='jpg'", "Id ASC", 
                out int pageCount, out int total);

            PrintDatatable(pageDt);

            Console.WriteLine($"---------------分页查询完成___当前查询第 {pageIndex} 页 ，每页 {pageSize} 条，共 {pageCount} 页、{total} 条数据---------------");
            #endregion

            #region   获取数据库中包含的所有表

            Console.WriteLine($"\n---------------开始获取数据库中用户创建的所有表名称及其包含的列名称---------------");

            DataTable getAllTableInfo = sqliteDBHelper.SqlHelper.GetUserCreateAllTableInfo();

            List<string> getAllTableName = sqliteDBHelper.SqlHelper.GetAllTableName(getAllTableInfo);
            if (getAllTableName != null && getAllTableName.Count > 0)
            {
                string tableName = null;
                string columnName = null;
                for (int i = 0; i < getAllTableName.Count; i++)
                {
                    tableName = getAllTableName[i];
                    Console.WriteLine($"\n表的名称为：{tableName}");

                    List<string> getAllColumnName = sqliteDBHelper.SqlHelper.GetAllTableName(tableName);
                    if (getAllColumnName != null && getAllColumnName.Count > 0)
                    {
                        Console.WriteLine($"{tableName}表包含的所有列名称为：");
                        for (int j = 0; j < getAllColumnName.Count; j++)
                        {
                            columnName = getAllColumnName[j];
                            Console.WriteLine($"列的名称为：{columnName}");
                        }
                    }

                }
            }

            Console.WriteLine($"--------------获取数据库中用户创建的所有表名称及其包含的列名称完成---------------");


            #endregion

            #region   获取最大Id和判断是否存在

            Console.WriteLine($"\n---------------获取到最大Id---------------");
            int maxId = sqliteDBHelper.SqlHelper.GetMaxID("ImageInfo", "Id");

            Console.WriteLine($"\n---------------获取到的最大Id是：{maxId}---------------");


            Console.WriteLine($"\n---------------判断是否存在---------------");
            string sql = $"select * from ImageInfo where ImageType='png'";
            object ot = sqliteDBHelper.SqlHelper.Exists(sql);

            Console.WriteLine($"\n---------------判断是否存在的结果是：{ot.ToString()}---------------\n\n\n");

            #endregion

        }

        public static void MemoryDBDemo()
        {
            Console.WriteLine("\n\n\n------------------开始执行【内存】Sqlite数据库的创建、CRUD操作--------------------------------\n");

            /*注意：内存数据库只有在连接打开时才会持续存在【如果你一直需要它则需要一直保持连接处于开启状态】*/

            #region   创建Sqlite数据库

            Console.WriteLine($"---------------开始创建Sqlite【内存】数据库---------------");
            //创建内存数据库
            SqliteMemoryDBHelper sqliteDBHelper = new SqliteMemoryDBHelper("MemoryTestDB", 3, true);
            ResultInfo result = sqliteDBHelper.CreateSqliteDataBase(ref connection);
            Console.WriteLine($"创建基础无密码的Sqlite数据库情况：{result.ResultPrint()}");

            #endregion 


            #region   创建Sqlite指定数据库的表

            //创建表方式2
            List<SqliteFieldInfo> fieldNameList2 = new List<SqliteFieldInfo>()
            {
                new SqliteFieldInfo("Id",SqliteType.Integer,0,true,true,true),
                new SqliteFieldInfo("ImageType",SqliteType.Text,16),
                new SqliteFieldInfo("AddDate",SqliteType.Text,20),
                new SqliteFieldInfo("ImagePath",SqliteType.Text,255),
                new SqliteFieldInfo("IsDisable",SqliteType.Text,5),
                new SqliteFieldInfo("Position",SqliteType.Real,20)

            };
            ResultInfo result2_2 = sqliteDBHelper.CreateSqliteTable("ImageInfo", fieldNameList2,ref connection);

            Console.WriteLine($"创建Access数据库的表情况：{result2_2.ResultPrint()}");


            #endregion

            #region   插入数据
            Console.WriteLine($"\n---------------开始执行插入数据---------------");

            //插入sqlite数据库中ImageInfo表数据
            string insertSql = null;
            List<string> sqlList = new List<string>();
            for (int i = 0; i < 10; i++)
            {

                insertSql = $"Insert Into ImageInfo (ImageType,AddDate,ImagePath,IsDisable,Position) " +
                   $"Values ('jpg','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','E:\\MyProject\\Image\\{i}','{false}','{i}.25879456123');";

                sqlList.Add(insertSql);


                ////执行单条sql语句
                //int affectRow = sqliteDBHelper.SqlHelper.ExecuteSql(insertSql,ref sqliteConnection);

                //Console.WriteLine($"{i} 插入数据行数：{affectRow}");
            }

            //执行多条sql语句（通过事务方式）
            sqliteDBHelper.SqlHelper.ExecuteSqlByTransaction(sqlList, ref connection);

            Console.WriteLine($"------------插入 {sqlList.Count} 条数据完成----------");
            #endregion

            #region   查询数据
            Console.WriteLine($"\n---------------开始查询所有数据---------------");

            string querySql = $"Select Id,ImageType,AddDate,ImagePath,IsDisable,[Position] From ImageInfo ";
            DataTable dt = sqliteDBHelper.SqlHelper.ExecuteDataTable(querySql,ref connection);

            PrintDatatable(dt);

            Console.WriteLine($"---------------查询数据完成---------------");

            #endregion

            #region   更新数据
            Console.WriteLine($"\n---------------开始更新数据---------------");

            string updateSql = $"update ImageInfo set ImageType='png',AddDate='2022-04-17 10:36:36'," +
                $"ImagePath='D:\\TestProject\\image\\11',IsDisable='True',[Position]='36.258941' where Id=1  ";
            int updateData = sqliteDBHelper.SqlHelper.ExecuteSql(updateSql,ref connection);

            Console.WriteLine($"---------------更新 {updateData} 条数据完成---------------");

            Console.WriteLine($"\n---------------更新数据后_开始查询所有数据---------------");

            string querySql2 = $"Select Id,ImageType,AddDate,ImagePath,IsDisable,[Position] From ImageInfo ";
            DataTable dt2 = sqliteDBHelper.SqlHelper.ExecuteDataTable(querySql2,ref connection);

            PrintDatatable(dt2);

            Console.WriteLine($"---------------更新数据后_查询数据完成---------------");

            #endregion

            #region   删除数据

            Console.WriteLine($"\n---------------开始删除数据---------------");

            string deleteSql = $"delete from  ImageInfo where Id=10";
            int deleteData = sqliteDBHelper.SqlHelper.ExecuteSql(deleteSql,ref connection);

            Console.WriteLine($"---------------删除 {updateData} 条数据完成---------------");

            Console.WriteLine($"\n--------------删除数据后_开始查询所有数据---------------");

            string querySql3 = $"Select Id,ImageType,AddDate,ImagePath,IsDisable,[Position] From ImageInfo ";
            DataTable dt3 = sqliteDBHelper.SqlHelper.ExecuteDataTable(querySql3,ref connection);

            PrintDatatable(dt3);

            Console.WriteLine($"---------------删除数据后_查询数据完成---------------");

            #endregion

            #region   分页查询
            Console.WriteLine($"\n---------------开始分页查询数据---------------");

            int pageIndex = 3, pageSize = 3;

            DataTable pageDt = sqliteDBHelper.SqlHelper.GetPageContent(ref pageIndex, ref pageSize, "Id",
                "Id,ImageType,AddDate,ImagePath,IsDisable,[Position]", "ImageInfo", "where ImageType='jpg'", "Id ASC",
                out int pageCount, out int total,ref connection);

            PrintDatatable(pageDt);

            Console.WriteLine($"---------------分页查询完成___当前查询第 {pageIndex} 页 ，每页 {pageSize} 条，共 {pageCount} 页、{total} 条数据---------------");
            #endregion

            #region   获取数据库中包含的所有表

            Console.WriteLine($"\n---------------开始获取数据库中用户创建的所有表名称及其包含的列名称---------------");

            DataTable getAllTableInfo = sqliteDBHelper.SqlHelper.GetUserCreateAllTableInfo();

            List<string> getAllTableName = sqliteDBHelper.SqlHelper.GetAllTableName(getAllTableInfo);
            if (getAllTableName != null && getAllTableName.Count > 0)
            {
                string tableName = null;
                string columnName = null;
                for (int i = 0; i < getAllTableName.Count; i++)
                {
                    tableName = getAllTableName[i];
                    Console.WriteLine($"\n表的名称为：{tableName}");

                    List<string> getAllColumnName = sqliteDBHelper.SqlHelper.GetAllTableName(tableName);
                    if (getAllColumnName != null && getAllColumnName.Count > 0)
                    {
                        Console.WriteLine($"{tableName}表包含的所有列名称为：");
                        for (int j = 0; j < getAllColumnName.Count; j++)
                        {
                            columnName = getAllColumnName[j];
                            Console.WriteLine($"列的名称为：{columnName}");
                        }
                    }

                }
            }

            Console.WriteLine($"--------------获取数据库中用户创建的所有表名称及其包含的列名称完成---------------");


            #endregion

            #region   获取最大Id和判断是否存在

            Console.WriteLine($"\n---------------获取到最大Id---------------");
            int maxId = sqliteDBHelper.SqlHelper.GetMaxID("ImageInfo", "Id",ref connection);

            Console.WriteLine($"\n---------------获取到的最大Id是：{maxId}---------------");


            Console.WriteLine($"\n---------------判断是否存在---------------");
            string sql = $"select * from ImageInfo where ImageType='png'";
            object ot = sqliteDBHelper.SqlHelper.Exists(sql,ref connection);

            Console.WriteLine($"\n---------------判断是否存在的结果是：{ot.ToString()}---------------");

            #endregion


            /*最后才关闭连接释放资源*/
            connection?.Dispose();

            Console.WriteLine($"---------------Sqlite【内存】数据库的连接情况：{connection.State}---------------");

        }

        //输出DataTable内容
        private static void PrintDatatable(DataTable dt)
        {
            Console.WriteLine($"Id\tImageType\tAddDate\t\tImagePath\t\tIsDisable\tPosition\t");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    object str = dt.Rows[i][j];

                    Console.Write($"{str}\t");
                }
                Console.WriteLine();
            }
        }


    }//Class_end

}
