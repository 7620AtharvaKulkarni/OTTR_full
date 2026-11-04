using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTTR.Models;
using OTTR.Repositories;

namespace OTTR.Services
{
    public interface IAuditOttrMonitoringService
    {
        Task<AuditOttrMonitoring> AddAuditOttrMonitoringInfo(OttrMonitoring ottrMonitoring);
    }
    public class AuditOttrMonitoringService : IAuditOttrMonitoringService
    {
        private readonly IAuditOttrMonitoringRepository auditOttrMonitoringRepository;

        public AuditOttrMonitoringService(IAuditOttrMonitoringRepository auditOttrMonitoringRepository)
        {
            this.auditOttrMonitoringRepository = auditOttrMonitoringRepository;
        }

        //Adding New Audit Ottr Monitoring Information
        public async Task<AuditOttrMonitoring> AddAuditOttrMonitoringInfo(OttrMonitoring ottrMonitoring)
        {
            try
            {
                AuditOttrMonitoring auditOttrMonitoring = new AuditOttrMonitoring(ottrMonitoring);
                return await auditOttrMonitoringRepository.AddAuditOttrMonitoringInformation(auditOttrMonitoring);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}