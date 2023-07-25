using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ClassLibrary_FQY
{
    public class SqlSeverHelper
    {
        /// <summary>
        /// 连接数据库，并执行操作
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="queryString">要执行操作的语句</param>
        /// <returns>返回收影响的行数</returns>
        public static int CreateCommand(string connectionString, string queryString)
        {
            //使用using块，当代码退出 using 块时，连接会自动关闭
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 设置连接、命令，然后执行不带查询的命令
        /// 执行 Transact-SQL INSERT、DELETE、UPDATE 和 SET 语句等命令
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">sql语句</param>
        /// <param name="commandType">commandText的属性</param>
        /// <param name="parameters">设置参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
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
        public static object ExecuteScalar(string connectionString, string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
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
        public static SqlDataReader ExecuteReader(string connectionString, string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(commandText, conn);
            cmd.CommandType = commandType;
            cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteReader();
        }


        /************************************异步操作数据库************************************/

        public static async Task<int> CreateCommandAsync(SqlConnection conn, SqlCommand cmd)
        {
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return 0;
        }
    }
}
