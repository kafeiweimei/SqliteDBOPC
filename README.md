# SqliteDBOPC
## 一、项目简介

​	使用C#开发的Sqlite数据库创建、操作（完整的Sqlite数据库、表创建、数据的增、删、查、改、获取数据库所有表和表包含的所有字段的使用）的项目。关于Sqlite的所有操作已经单独创建了专门的跨平台【.NETCore3.1】类库包含相应的帮助类，可以直接生成后拿到任何项目中直接使用，高效简单，省去了从头开发Sqlite数据库的时间，将更多的精力用于业务内容；该项目工程的主要功能如下：

①可以获取到关于Sqlite数据库的2种常用连接字符串【不加密、加密】（也可以自己传入）；
②可以代码直接创建Sqlite数据库；
③可以直接创建Sqlite数据库的表；
④实现了通用的sql语句执行帮助类（包含单条数据的插入、批量插入、事务处理、查询（指定内容查询、分页查询）、更新、删除等操作）；
⑤可以直接获取Sqlite数据库中的所有表名称及其表包含的所有列名称；
⑥包含了一个完整的Sqlite数据库、表创建、数据的增、删、查、改、获取数据库所有表和表包含的所有字段的使用示例。
⑦还包含了关于sqlite内存数据库的帮助类和完整数据库、表创建、数据的增、删、查、改、获取数据库所有表和表包含的所有字段的使用示例。

本项目的介绍文章是：[C#实现对Sqlite数据库的通用操作_c# sqlite-CSDN博客](https://coffeemilk.blog.csdn.net/article/details/124356278)

二、项目演示内容

### 2.1、项目示例截图

![Sqlite数据库操作使用示例](https://github.com/kafeiweimei/SqliteDBOPC/blob/main/Sqlite%E6%95%B0%E6%8D%AE%E5%BA%93%E6%93%8D%E4%BD%9C%E4%BD%BF%E7%94%A8%E7%A4%BA%E4%BE%8B.png)

![Sqlite数据库操作使用示例2](https://github.com/kafeiweimei/SqliteDBOPC/blob/main/Sqlite%E6%95%B0%E6%8D%AE%E5%BA%93%E6%93%8D%E4%BD%9C%E4%BD%BF%E7%94%A8%E7%A4%BA%E4%BE%8B2.png)

### 2.2、项目运行演示视频

<video src=".\Sqlite数据库操作的完整源码工程和示例.mp4">项目运行示例演示</video>

