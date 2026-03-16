using System;
using System.Collections.Generic;
using System.Text;

namespace Pruner.Host
{
    public interface ITenantProvider
    {
        Guid GetTenantId();
    }
}
