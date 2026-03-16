using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Quartz;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Pruner.Public.Models;

namespace Pruner.Host.Controllers
{
    /// <summary>
    /// 项目启动类
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoleType.Admin)]
    public class StartupsController : Controller
    {
        private readonly string _assemblyName;
        private readonly ISchedulerFactory _schedulerFactory;

        public StartupsController(ISchedulerFactory schedulerFactory)
        {
            _assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            _schedulerFactory = schedulerFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        public bool Get()
        {
            return true;
        }
    }
}
