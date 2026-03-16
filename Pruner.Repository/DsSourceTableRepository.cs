using Dapper;
using Microsoft.Data.SqlClient;
using Pruner.Domain.Repositorys;
using System;
using System.Threading.Tasks;

namespace Pruner.Repository
{
    /// <summary>
    /// 源数据库表操作仓储（Dapper）
    /// </summary>
    public class DsSourceTableRepository : IDsSourceTableRepository
    {
        /// <summary>
        /// 查找区间最大可删除id
        /// </summary>
        public async Task<long> GetMaxIdAsync(string sourceConn, string tableName, long minId, int topNum, string dateFieldName, DateTime cutoffDate, string customWhere)
        {
            if (topNum < 1) topNum = 1;

            var parameters = new DynamicParameters();
            parameters.Add("@MinId", minId);

            var sql = $"SELECT TOP {topNum} id FROM {tableName} WHERE Id > @MinId";
            if (!string.IsNullOrWhiteSpace(dateFieldName))
            {
                sql += $" AND {dateFieldName} < @CreateTime";
                parameters.Add("@CreateTime", cutoffDate.ToString("yyyy-MM-dd"));
            }

            if (!string.IsNullOrEmpty(customWhere))
                sql += $" AND {customWhere}";

            var topSql = $"SELECT ISNULL(MAX(id),0) AS maxId FROM ({sql} ORDER BY id ASC) AS subquery";

            using (var connection = new SqlConnection(sourceConn))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<long>(topSql, parameters);
            }
        }

        /// <summary>
        /// 删除指定id区间的数据
        /// </summary>
        public async Task<int> DelRangeAsync(string sourceConn, string tableName, long minId, long maxId, string dateFieldName, DateTime cutoffDate, string customWhere)
        {
            if (minId > maxId)
                return 0;

            var parameters = new DynamicParameters();
            parameters.Add("@MinId", minId);
            parameters.Add("@MaxId", maxId);

            var sql = $"DELETE FROM {tableName} WHERE Id >= @MinId AND Id <= @MaxId";
            if (!string.IsNullOrWhiteSpace(dateFieldName))
            {
                sql += $" AND {dateFieldName} < @CreateTime";
                parameters.Add("@CreateTime", cutoffDate.ToString("yyyy-MM-dd"));
            }

            if (!string.IsNullOrEmpty(customWhere))
                sql += $" AND {customWhere}";

            using (var connection = new SqlConnection(sourceConn))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
