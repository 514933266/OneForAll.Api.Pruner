using AutoMapper;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;

namespace Pruner.Host.Profiles
{
    /// <summary>
    /// 数据库删除配置映射（Form → Entity）
    /// </summary>
    public class DsDeleteConfigProfile : Profile
    {
        public DsDeleteConfigProfile()
        {
            CreateMap<DsDeleteConfigForm, DsDeleteConfig>();
        }
    }
}
