using System.Data;
using System.Data.SQLite;
using System.Threading;

namespace ClassLibrary_FQY
{
    /// <summary>
    /// SQLite操作类
    /// </summary>
    public class SqLiteHelper
    {
        /// <summary>
        /// 数据库连接定义
        /// </summary>
        private SQLiteConnection dbConnection;

        /// <summary>
        /// SQL命令定义
        /// </summary>
        private SQLiteCommand dbCommand;

        /// <summary>
        /// 数据读取定义
        /// </summary>
        private SQLiteDataReader dataReader;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">连接SQLite库字符串</param>
        public SqLiteHelper(string connectionString)
        {
            try
            {
                dbConnection = new SQLiteConnection(connectionString);
                dbConnection.Open();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <param name="queryString">SQL命令字符串</param>
        /// <returns></returns>
        public int ExecuteQuery(string queryString)
        {
            int Count = 0;
            try
            {
                dbCommand = new SQLiteCommand(queryString, dbConnection);
                Count = dbCommand.ExecuteNonQuery();//返回受命令影响的行数
            }
            catch
            {

            }
            return Count;
        }

        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <returns>The query.</returns>
        /// <param name="queryString">SQL命令字符串</param>
        public SQLiteDataReader ExecuteReader(string queryString)
        {
            try
            {
                dbCommand = new SQLiteCommand(queryString, dbConnection);
                dataReader = dbCommand.ExecuteReader();
            }
            catch
            {

            }

            return dataReader;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseConnection()
        {
            //销毁Command
            if (dbCommand != null)
            {
                dbCommand.Cancel();
            }
            dbCommand = null;
            //销毁Reader
            if (dataReader != null)
            {
                dataReader.Close();
            }
            dataReader = null;
            //销毁Connection
            if (dbConnection != null)
            {
                dbConnection.Close();
            }
            dbConnection = null;
        }

        /// <summary>
        /// 读取整张数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public SQLiteDataReader ReadFullTable(string tableName)
        {
            string queryString = "SELECT rowid, * FROM " + tableName;
            return ExecuteReader(queryString);
        }

        /// <summary>
        /// 按照日期筛选读取整张数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public SQLiteDataReader ReadFullTable(string tableName, string StartTime, string EndTime)
        {
            string queryString = "select * from " + tableName + " where 日期时间 between " + StartTime + " and " + EndTime;
            return ExecuteReader(queryString);
        }

        /// <summary>
        /// 按照日期时间筛选表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="ColumnName">列名</param>
        /// <param name="StartTime">查询起始时间</param>
        /// <param name="EndTime">查询截止时间</param>
        /// <returns></returns>
        public SQLiteDataReader ScreenTablebyDatetime(string tableName, string ColumnName, string StartTime, string EndTime)
        {
            string queryString = "SELECT * FROM " + tableName + " WHERE " + ColumnName + " BETWEEN " + "'" + StartTime + "'" + " AND " + "'" + EndTime + "'";
            return ExecuteReader(queryString);
        }

        /// <summary>
        /// 向指定数据表中插入数据
        /// </summary>
        /// <param name="tableName">数据表名称</param>
        /// <param name="values">插入的数值</param>
        /// <returns></returns>
        public void InsertValues(string tableName, string[] values)
        {
            string queryString = "INSERT INTO " + tableName + " VALUES ( " + " ' " + values[0] + " ' ";
            for (int i = 1; i < values.Length; i++)
            {
                queryString += ", " + " ' " + values[i] + " ' ";
            }
            queryString += " )";
            ExecuteQuery(queryString);
        }

        /// <summary>
        /// 向指定数据表中插入数据,不包含日期时间，默认首列插入实时时间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        public void InsertValuesExceptforDatetime(string tableName, string[] values)
        {
            string strGetCurrentTime = "DATETIME('now','localtime')";
            string queryString = "INSERT INTO " + tableName + " VALUES ( " + strGetCurrentTime;
            for (int i = 0; i < values.Length; i++)
            {
                queryString += ", " + " ' " + values[i] + " ' ";
            }
            queryString += " )";
            ExecuteQuery(queryString);
        }

        /// <summary>
        /// 编辑SQL语句
        /// </summary>
        /// <param name="sql"></param>
        public void Sql_cmd(string sql)
        {
            ExecuteReader(sql);
        }

        /// <summary>
        /// 更新指定数据表内的数据
        /// </summary>
        /// <returns>The values.</returns>
        /// <param name="tableName">数据表名称</param>
        /// <param name="colNames">字段名</param>
        /// <param name="colValues">字段名对应的数据</param>
        /// <param name="key">关键字</param>
        /// <param name="value">关键字对应的值</param>
        /// <param name="operation">运算符：=,<,>,...，默认“=”</param>
        public void UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string value, string operation = "=")
        {
            //当字段名称和字段数值不对应时引发异常
            if (colNames.Length != colValues.Length)
            {
                throw new SQLiteException("colNames.Length!=colValues.Length");
            }

            string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + "'" + colValues[0] + "'";
            for (int i = 1; i < colValues.Length; i++)
            {
                queryString += ", " + colNames[i] + "=" + "'" + colValues[i] + "'";
            }
            queryString += " WHERE " + key + operation + "'" + value + "'";
            ExecuteQuery(queryString);
        }

        /// <summary>
        /// 删除指定数据表内的数据
        /// </summary>
        /// <returns>The values.</returns>
        /// <param name="tableName">数据表名称</param>
        /// <param name="colNames">字段名</param>
        /// <param name="colValues">字段名对应的数据</param>
        public void DeleteValuesOR(string tableName, string[] colNames, string[] colValues, string[] operations)
        {
            //当字段名称和字段数值不对应时引发异常
            if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
            {
                throw new SQLiteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
            }

            string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + "'" + colValues[0] + "'";
            for (int i = 1; i < colValues.Length; i++)
            {
                queryString += "OR " + colNames[i] + operations[0] + "'" + colValues[i] + "'";
            }
            ExecuteQuery(queryString);
        }

        /// <summary>
        /// 删除指定数据表内的数据
        /// </summary>
        /// <returns>The values.</returns>
        /// <param name="tableName">数据表名称</param>
        /// <param name="colNames">字段名</param>
        /// <param name="colValues">字段名对应的数据</param>
        public void DeleteValuesAND(string tableName, string[] colNames, string[] colValues, string[] operations)
        {
            //当字段名称和字段数值不对应时引发异常
            if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
            {
                throw new SQLiteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
            }

            string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + "'" + colValues[0] + "'";
            for (int i = 1; i < colValues.Length; i++)
            {
                queryString += " AND " + colNames[i] + operations[i] + "'" + colValues[i] + "'";
            }
            ExecuteQuery(queryString);
        }

        /// <summary>
        /// 删除指定数据表内的所有数据
        /// </summary>
        /// <param name="tableName">数据表名称</param>
        public void DeleteValues(string tableName)
        {
            string queryString = "DELETE  FROM " + tableName;
            ExecuteQuery(queryString);
        }

        /// <summary>
        /// 创建数据表
        /// </summary> +
        /// <returns>The table.</returns>
        /// <param name="tableName">数据表名</param>
        /// <param name="colNames">字段名</param>
        /// <param name="colTypes">字段名类型</param>
        public void CreateTable(string tableName, string[] colNames, string[] colTypes)
        {
            string queryString = "CREATE TABLE IF NOT EXISTS " + tableName + "( " + colNames[0] + " " + colTypes[0];
            for (int i = 1; i < colNames.Length; i++)
            {
                queryString += ", " + colNames[i] + " " + colTypes[i];
            }
            queryString += "  ) ";
            ExecuteQuery(queryString);
        }

        /// <summary>
        /// Reads the table.
        /// </summary>
        /// <returns>The table.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="items">Items.</param>
        /// <param name="colNames">Col names.</param>
        /// <param name="operations">Operations.</param>
        /// <param name="colValues">Col values.</param>
        public void ReadTable(string tableName, string[] items, string[] colNames, string[] operations, string[] colValues)
        {
            string queryString = "SELECT " + items[0];
            for (int i = 1; i < items.Length; i++)
            {
                queryString += ", " + items[i];
            }
            queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " + operations[0] + " " + colValues[0];
            for (int i = 0; i < colNames.Length; i++)
            {
                queryString += " AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
            }
            ExecuteQuery(queryString);
        }


        #region 2022.8.17新增静态方法

        private static readonly object obj = new object();
        /// <summary>
        /// 设置连接、命令，然后执行不带查询的命令
        /// 执行 Transact-SQL INSERT、DELETE、UPDATE 和 SET 语句等命令
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">sql语句</param>
        /// <param name="commandType">commandText的属性</param>
        /// <param name="parameters">设置参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string commandText, CommandType commandType = CommandType.Text, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (SQLiteCommand cmd = new SQLiteCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);
                    Monitor.Enter(obj);
                    int result = cmd.ExecuteNonQuery();
                    Monitor.Exit(obj);
                    return result;
                }
            }
        }

        /// <summary>
        /// 设置连接、命令，然后执行命令，只返回一个值
        /// 检索单个值 (例如，从数据库) 聚合值
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">sql语句</param>
        /// <param name="commandType">commandText的属性</param>
        /// <param name="parameters">设置参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, string commandText, CommandType commandType = CommandType.Text, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 设置连接、命令，然后使用查询执行命令并返回读取器
        /// 为了提高性能，ExecuteReader请使用 Transact-SQL sp_executesql 系统存储过程调用命令
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">sql语句</param>
        /// <param name="commandType">commandText的属性</param>
        /// <param name="parameters">设置参数</param>
        /// <returns></returns>
        public static SQLiteDataReader ExecuteReader(string connectionString, string commandText, CommandType commandType = CommandType.Text, params SQLiteParameter[] parameters)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteReader();
            }
        }


        #endregion











    }
}













