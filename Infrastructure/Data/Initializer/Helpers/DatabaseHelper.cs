using System.Data;
using Dapper;

namespace Infrastructure.Data.Initializer.Helpers
{
    public static class DatabaseHelper
    {
        public static bool TableExists(IDbConnection connection, string tableName, IDbTransaction transaction = null)
        {
            var checkTableSql = @"
                SELECT EXISTS (
                    SELECT FROM information_schema.tables 
                    WHERE table_schema = 'dbo' 
                    AND table_name = @TableName
                );";

            return connection.ExecuteScalar<bool>(checkTableSql, new { TableName = tableName }, transaction);
        }
    }
}
