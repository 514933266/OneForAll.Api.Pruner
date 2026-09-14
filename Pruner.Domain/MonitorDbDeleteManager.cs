using Microsoft.AspNetCore.Http;
using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pruner.Domain
{
    /// <summary>
    /// 监控数据库数据删除
    /// </summary>
    public class MonitorDbDeleteManager : BaseManager, IMonitorDbDeleteManager
    {
        private readonly IDsGlobalConfigRepository _globalConfigRepository;
        private readonly IDsDeleteConfigRepository _deleteConfigRepository;
        private readonly IDsRunningLogRepository _runningLogRepository;
        private readonly IDsSourceTableRepository _sourceTableRepository;

        public MonitorDbDeleteManager(
            IHttpContextAccessor httpContextAccessor,
            IDsGlobalConfigRepository globalConfigRepository,
            IDsDeleteConfigRepository deleteConfigRepository,
            IDsRunningLogRepository runningLogRepository,
            IDsSourceTableRepository sourceTableRepository):base(httpContextAccessor)
        {
            _globalConfigRepository = globalConfigRepository;
            _deleteConfigRepository = deleteConfigRepository;
            _runningLogRepository = runningLogRepository;
            _sourceTableRepository = sourceTableRepository;
        }

        /// <summary>
        /// 执行数据删除任务
        /// </summary>
        /// <returns>删除的总记录数</returns>
        public async Task<int> HandleDeleteAsync()
        {
            var delNum = 0;
            var globalConfigs = (await _globalConfigRepository.GetListAsync(w => w.IsEnabled)).ToList();
            var delConfigs = (await _deleteConfigRepository.GetListAsync(w =>
                w.IsEnabled && (w.WaitingTime == null || w.WaitingTime < DateTime.UtcNow.Date.AddDays(1)))).ToList();

            if (globalConfigs.Any() && delConfigs.Any())
            {
                foreach (var globalConfig in globalConfigs)
                {
                    delNum += await HandleDeleteAsync(globalConfig, delConfigs);
                    await AddRunningLogAsync("执行完成", globalConfig.SourceDbName, "已执行数据删除任务");
                }
            }

            // 删除1天前的运行日志
            var oldLogs = await _runningLogRepository.GetListAsync(w => w.CreateTime < DateTime.UtcNow.AddDays(-7));
            if (oldLogs.Any())
            {
                await _runningLogRepository.DeleteRangeAsync(oldLogs);
            }

            return delNum;
        }

        /// <summary>
        /// 处理单个全局配置下的所有表删除
        /// </summary>
        private async Task<int> HandleDeleteAsync(DsGlobalConfig globalConfig, List<DsDeleteConfig> allDelConfigs)
        {
            var delNum = 0;
            var delConfigs = allDelConfigs.Where(w => w.GlobalConfigId == globalConfig.Id).ToList();

            if (!delConfigs.Any())
                return 0;

            foreach (var config in delConfigs)
            {
                try
                {
                    // 如果当前时间小于延迟时间，则不执行删除表数据
                    if (config.WaitingTime != null && DateTime.UtcNow < config.WaitingTime)
                        continue;

                    var maxDelCount = config.MaxDelCount > 0 ? config.MaxDelCount : globalConfig.MaxDelCount;
                    var cutoffDate = DateTime.UtcNow.Date.AddDays(-config.KeepDays);

                    long minId = config.LastDelId;

                    // 如果配置了最大删除id且超过该值，则不再删除表数据
                    if (config.KeepId > 0 && minId >= config.KeepId)
                        continue;

                    if (config.IsLastDelIdEnabled)
                    {
                        // 启用LastDelId：按id区间逐批向后推进删除（要求id与时间严格同序）
                        delNum += await HandleRangeDeleteAsync(globalConfig, config, maxDelCount, cutoffDate);
                    }
                    else
                    {
                        // 停用LastDelId：每次直接按查询条件删除一批数据（适用于雪花id等非严格递增id，避免漏删）
                        delNum += await HandleDirectDeleteAsync(globalConfig, config, maxDelCount, cutoffDate);
                    }

                    await _deleteConfigRepository.UpdateAsync(config);
                }
                catch (Exception ex)
                {
                    await AddRunningLogAsync("删除失败", config.TableName, $"删除表{globalConfig.SourceDbName}.{config.TableName}数据出错，{ex.Message}");
                }
            }

            return delNum;
        }

        /// <summary>
        /// 按id区间逐批推进删除（启用LastDelId）
        /// </summary>
        private async Task<int> HandleRangeDeleteAsync(DsGlobalConfig globalConfig, DsDeleteConfig config, int maxDelCount, DateTime cutoffDate)
        {
            long minId = config.LastDelId;

            // 查找区间最大可删除id
            var maxId = await _sourceTableRepository.GetMaxIdAsync(
                globalConfig.SourceConn, config.TableName, minId, maxDelCount, config.DateFieldName, cutoffDate, config.CustonWhere);

            if (maxId <= 0)
            {
                config.WaitingTime = DateTime.UtcNow.AddDays(1);
                return 0;
            }

            // 防止删除超过拦截id
            maxId = maxId > config.KeepId && config.KeepId > 0 ? config.KeepId : maxId;

            var effected = await _sourceTableRepository.DelRangeAsync(
                globalConfig.SourceConn, config.TableName, minId, maxId, config.DateFieldName, cutoffDate, config.CustonWhere);
            if (effected <= 0)
            {
                config.WaitingTime = DateTime.UtcNow.AddDays(1);
                return 0;
            }

            config.LastDelId = maxId;
            config.LastDelTime = DateTime.UtcNow;
            await AddRunningLogAsync("删除成功", config.TableName,
                $"{globalConfig.SourceDbName}.{config.TableName}成功删除{effected}条数据, id范围：({minId},{maxId})");
            return effected;
        }

        /// <summary>
        /// 按查询条件直接删除一批数据（停用LastDelId）
        /// </summary>
        private async Task<int> HandleDirectDeleteAsync(DsGlobalConfig globalConfig, DsDeleteConfig config, int maxDelCount, DateTime cutoffDate)
        {
            var effected = await _sourceTableRepository.DelTopAsync(
                globalConfig.SourceConn, config.TableName, maxDelCount, config.KeepId, config.DateFieldName, cutoffDate, config.CustonWhere);
            if (effected <= 0)
            {
                config.WaitingTime = DateTime.UtcNow.AddDays(1);
                return 0;
            }

            config.LastDelTime = DateTime.UtcNow;
            await AddRunningLogAsync("删除成功", config.TableName,
                $"{globalConfig.SourceDbName}.{config.TableName}成功删除{effected}条数据, 按查询条件直接删除");
            return effected;
        }

        /// <summary>
        /// 记录运行日志
        /// </summary>
        private async Task AddRunningLogAsync(string typeName, string tableName, string description)
        {
            await _runningLogRepository.AddAsync(new DsRunningLog()
            {
                TypeName = typeName,
                TableName = tableName,
                Description = description,
                CreateTime = DateTime.UtcNow
            });
        }
    }
}
