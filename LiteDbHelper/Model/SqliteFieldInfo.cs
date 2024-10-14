/***
*	Title："轻量数据库" 项目
*		主题：Sqlite的字段信息
*	Description：
*		功能：XXX
*	Date：2022
*	Version：0.1版本
*	Author：Coffee
*	Modify Recoder：
*/

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDBHelper.Model
{
    public class SqliteFieldInfo
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段数据类型
        /// </summary>
        public SqliteType DataType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 是否不为为空(默认false表示为空)
        /// </summary>
        public bool IsNotEmpty { get; set; } = false;

        /// <summary>
        /// 是否为主键(默认false不是主键)
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// 是否为自增字段（默认false不是自增字段）
        /// </summary>
        public bool IsAutoIncrement { get; set; } = false;




        /// <summary>
        /// 构造函数
        /// </summary>
        public SqliteFieldInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="dataType">字段数据类型</param>
        /// <param name="length">字段长度</param>
        /// <param name="isNotEmpty">是否不为为空(默认false表示为空)</param>
        /// <param name="isPrimaryKey">是否为主键(默认false不是主键)</param>
        /// <param name="isAutoIncrement">是否为自增字段（默认false不是自增字段）</param>
        public SqliteFieldInfo(string name, SqliteType dataType, int length, bool isNotEmpty=false, 
            bool isPrimaryKey=false, bool isAutoIncrement=false)
        {
            this.Name = name;
            this.DataType = dataType;
            this.Length = length;
            this.IsNotEmpty = isNotEmpty;
            this.IsPrimaryKey = isPrimaryKey;
            this.IsAutoIncrement = isAutoIncrement;

        }



    }//Class_end



}
