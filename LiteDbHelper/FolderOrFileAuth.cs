/***
*	Title："轻量数据库" 项目
*		主题：文件夹或文件的权限
*	Description：
*		功能：XXX
*	Date：2022
*	Version：0.1版本
*	Author：Coffee
*	Modify Recoder：
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace LiteDBHelper
{
    public class FolderOrFileAuth
    {
        /// <summary>
        /// 设置用户对文件的访问权限
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="filePath">文件路径（比如：@"D:\\HalmEL\\2022-4-19.db"）</param>
        /// <param name="fileControlAuth">文件的控制权限（默认是完全控制）</param>
        /// <param name="accessControlType">文件的访问控制类型（默认允许访问）</param>
        public static void SetFilePermission(string userName,string filePath, 
            FileSystemRights fileControlAuth=FileSystemRights.FullControl,
            AccessControlType accessControlType=AccessControlType.Allow)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(filePath)) return;
            
            FileInfo fi = new FileInfo(filePath);
            FileSecurity fileSecurity = fi.GetAccessControl();
            fileSecurity.AddAccessRule(new FileSystemAccessRule(userName, fileControlAuth, accessControlType));
            fi.SetAccessControl(fileSecurity);
        }

        /// <summary>
        /// 设置用户对文件夹的访问权限
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="folderPath">文件夹路径（比如：@"D:\\HalmEL"）</param>
        /// <param name="fileControlAuth">文件夹的控制权限（默认是完全控制）</param>
        /// <param name="accessControlType">文件夹访问控制类型（默认允许访问）</param>
        public static void SetFolderPermission(string userName, string folderPath,
            FileSystemRights fileControlAuth = FileSystemRights.FullControl,
            AccessControlType accessControlType = AccessControlType.Allow)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(folderPath)) return;

            DirectoryInfo di = new DirectoryInfo(folderPath);
            DirectorySecurity directory = di.GetAccessControl();
            directory.AddAccessRule(new FileSystemAccessRule(userName, fileControlAuth, accessControlType));
            di.SetAccessControl(directory);
        }

    }//Class_end

}
