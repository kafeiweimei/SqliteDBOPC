﻿

using System;
using System.Collections.Generic;
using System.Data;
using Test_LiteDbOpc;

namespace Test_LiteDbOpc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            /*Sqlite数据库操作示例*/
            SqliteDBOpc.Demo();
            SqliteDBOpc.MemoryDBDemo();


            Console.ReadLine();
        }


    }
}
