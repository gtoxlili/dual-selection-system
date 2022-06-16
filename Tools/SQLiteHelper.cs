using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace Tools
{
    public static class SqLiteHelper
    {
        //需要添加引用--框架，System.Configuration，添加再using，用来读取配置文件的数据库链接字符串
        private static string conStr
        {
            get
            {
                byte[] aesKey = Encoding.UTF8.GetBytes("一念通天神魔无惧");
                return Tools.AesDecrypt(ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString, aesKey);
            }
        }

        /// <summary>
        ///     执行一个查询,并返回查询结果
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string commandText, CommandType commandType,
            SQLiteParameter[] parameters)
        {
            DataTable data = new DataTable(); //实例化DataTable，用于装载查询结果集
            using (SQLiteConnection conn = new SQLiteConnection(conStr))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(commandText, conn))
                {
                    cmd.CommandType = commandType; //设置command的CommandType为指定的CommandType
                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                        foreach (SQLiteParameter parameter in parameters)
                            cmd.Parameters.Add(parameter);

                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd); //实例化DbDataAdapter

                    adapter.Fill(data); //填充DataTable
                }
            }

            return data;
        }

        /// <summary>
        ///     执行一个查询,并返回查询结果
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <returns>返回查询结果集</returns>
        public static DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, CommandType.Text, null);
        }

        /// <summary>
        ///     执行一个查询,并返回查询结果
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns>返回查询结果集</returns>
        public static DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            return ExecuteDataTable(commandText, commandType, null);
        }


        /// <summary>
        ///     对数据库执行增删改操作
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="pms">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns>返回执行操作受影响的行数</returns>
        public static int ExecuteNonQuery(string sql, CommandType commandType, SQLiteParameter[] pms)
        {
            using (SQLiteConnection conn = new SQLiteConnection(conStr))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.CommandType = commandType;
                    if (pms != null) cmd.Parameters.AddRange(pms);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///     对数据库执行增删改操作
        /// </summary>
        /// <param name="commandText">要执行的查询SQL文本命令</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, null);
        }

        /// <summary>
        ///     对数据库执行增删改操作
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, commandType, null);
        }


        /// <summary>
        ///     查询单个结果，一般和聚合函数 一起使用
        /// </summary>
        /// <param name="sql">查询的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="pms">SQL参数</param>
        /// <returns>返回查询对象，查询结果第一行第一列</returns>
        public static object ExecuteScalar(string sql, CommandType commandType, params SQLiteParameter[] pms)
        {
            using (SQLiteConnection conn = new SQLiteConnection(conStr))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.CommandType = commandType;
                    if (pms != null) cmd.Parameters.AddRange(pms);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        ///     从数据库中检索单个值（例如一个聚合值）。
        /// </summary>
        /// <param name="commandText">要执行的查询SQL文本命令</param>
        /// <returns></returns>
        public static object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text, null);
        }

        /// <summary>
        ///     从数据库中检索单个值（例如一个聚合值）。
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns></returns>
        public static object ExecuteScalar(string commandText, CommandType commandType)
        {
            return ExecuteScalar(commandText, commandType, null);
        }


        /// <summary>
        ///     查询多行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="pms">SQL参数</param>
        /// <returns>返回SqlDataReader对象</returns>
        public static SQLiteDataReader ExcuteReader(string sql, params SQLiteParameter[] pms)
        {
            //这里不能用using，不然在返回SqlDataReader时候会报错，因为返回时候已经在using中关闭了。
            //事实上，在使用数据库相关类中，SqlConnection是必须关闭的，但是其他可以选择关闭，因为CG回自动回收
            SQLiteConnection conn = new SQLiteConnection(conStr);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                if (pms != null) cmd.Parameters.AddRange(pms);
                try
                {
                    conn.Open();
                    //传入System.Data.CommandBehavior.CloseConnection枚举是为了让在外面使用完毕SqlDataReader后，只要关闭了SqlDataReader就会关闭对应的SqlConnection
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch
                {
                    conn.Close();
                    conn.Dispose();
                    throw;
                }
            }
        }
    }
}