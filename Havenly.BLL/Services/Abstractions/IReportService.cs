using Havenly.BLL.ModelVMs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IReportService
    {
        Task<PlatformStatsVM> GetPlatformStats();
    }
}
