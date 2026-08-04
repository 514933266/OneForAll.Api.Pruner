using AutoMapper;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;

namespace Pruner.Host.Profiles
{
    /// <summary>
    /// 文件删除配置映射（Form → Entity）
    /// </summary>
    public class DsDeleteFileConfigProfile : Profile
    {
        public DsDeleteFileConfigProfile()
        {
            CreateMap<DsDeleteFileConfigForm, DsDeleteFileConfig>();
        }
    }
}
