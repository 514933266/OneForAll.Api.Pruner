using AutoMapper;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;

namespace Pruner.Host.Profiles
{
    /// <summary>
    /// 文件迁移配置映射（Form → Entity）
    /// </summary>
    public class DsMoveFileConfigProfile : Profile
    {
        public DsMoveFileConfigProfile()
        {
            CreateMap<DsMoveFileConfigForm, DsMoveFileConfig>();
        }
    }
}
