using System.Collections.Generic;
using System.Threading.Tasks;
using OTTR.Models;

namespace OTTR.Repositories
{
    public interface IAuditOttrMonitoringRepository
    {
        Task<AuditOttrMonitoring> AddAuditOttrMonitoringInformation(AuditOttrMonitoring auditOttrMonitoring);
    }
}