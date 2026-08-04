using AutoMapper;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;

namespace Pruner.Host.Profiles
{
    /// <summary>
    /// 全局配置映射（Form → Entity）
    /// </summary>
    public class DsGlobalConfigProfile : Profile
    {
        public DsGlobalConfigProfile()
        {
            CreateMap<DsGlobalConfigForm, DsGlobalConfig>();
        }
    }
}
