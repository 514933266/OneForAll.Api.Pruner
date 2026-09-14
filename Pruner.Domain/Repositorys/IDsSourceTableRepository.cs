using System;
using System.Threading.Tasks;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 源数据库表操作仓储（Dapper）
    /// </summary>
    public interface IDsSourceTableRepository
    {
        /// <summary>
        /// 查找区间最大可删除id
        /// </summary>
        /// <param name="sourceConn">源库连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="minId">最小id</param>
        /// <param name="topNum">最大查询数量</param>
        /// <param name="dateFieldName">日期字段名</param>
        /// <param name="cutoffDate">截止日期</param>
        /// <param name="customWhere">自定义条件</param>
        /// <returns>最大可删除id</returns>
        Task<long> GetMaxIdAsync(string sourceConn, string tableName, long minId, int topNum, string dateFieldName, DateTime cutoffDate, string customWhere);

        /// <summary>
        /// 删除指定id区间的数据
        /// </summary>
        /// <param name="sourceConn">源库连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="minId">最小id</param>
        /// <param name="maxId">最大id</param>
        /// <param name="dateFieldName">日期字段名</param>
        /// <param name="cutoffDate">截止日期</param>
        /// <param name="customWhere">自定义条件</param>
        /// <returns>删除行数</returns>
        Task<int> DelRangeAsync(string sourceConn, string tableName, long minId, long maxId, string dateFieldName, DateTime cutoffDate, string customWhere);

        /// <summary>
        /// 按查询条件直接删除一批数据（不依赖id区间推进，适用于雪花id等非严格递增id）
        /// </summary>
        /// <param name="sourceConn">源库连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="topNum">每批最大删除数量</param>
        /// <param name="maxId">删除id上限（大于0时只删除 Id &lt;= maxId 的数据，0表示不限制）</param>
        /// <param name="dateFieldName">日期字段名</param>
        /// <param name="cutoffDate">截止日期</param>
        /// <param name="customWhere">自定义条件</param>
        /// <returns>删除行数</returns>
        Task<int> DelTopAsync(string sourceConn, string tableName, int topNum, long maxId, string dateFieldName, DateTime cutoffDate, string customWhere);
    }
}
