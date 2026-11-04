using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTTR.Models;

namespace OTTR.Repositories
{
    public class AuditOttrMonitoringRepository : IAuditOttrMonitoringRepository
    {
        private readonly ApplicationDbContext context;
        public AuditOttrMonitoringRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        //Adding New Audit Ottr Monitoring Information
        public async Task<AuditOttrMonitoring> AddAuditOttrMonitoringInformation(AuditOttrMonitoring auditOttrMonitoring)
        {
            try
            {
                context.AuditOttrMonitorings.Add(auditOttrMonitoring);
                await context.SaveChangesAsync();
                return auditOttrMonitoring;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
