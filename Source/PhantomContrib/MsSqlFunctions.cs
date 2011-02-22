using System.Runtime.CompilerServices;
using System.Data.SqlClient;

namespace PhantomContrib.Core.Builtins
{
    [CompilerGlobalScope]
    public sealed class MsSqlFunctions
    {
        public static void MsSql_Create_Database(string databaseName, string connectionString)
        {
            if (MsSql_Database_Exists(databaseName, connectionString))
                return;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("create database " + databaseName, connection);
                connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
        }

        public static bool MsSql_Database_Exists(string databaseName, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("select count(1) from sys.databases where name = '" + databaseName + "'", connection);
                connection.Open();
                int count = (int)sqlCommand.ExecuteScalar();
                return count > 0;
            }
        }

        public static void MsSql_Delete_Database(string databaseName, string connectionString)
        {
            if (!MsSql_Database_Exists(databaseName, connectionString))
                return;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("drop database " + databaseName, connection);
                connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
