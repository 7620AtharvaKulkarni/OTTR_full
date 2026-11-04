using System.Collections.Generic;
using System.Threading.Tasks;
using OTTR.Models;

namespace OTTR.Repositories
{
    public interface IOttrMonitoringRepository
    {
        Task<IEnumerable<OttrMonitoring>> GetAllOTTRInformation(int GESGroupID,string Country, List<string> listOfRoles);
        Task<OttrMonitoring> GetSpecificOttrInfo(int ottrId);
        Task<OttrMonitoring> AddOTTRInformation(OttrMonitoring ottrMonitoring);
        Task<OttrMonitoring> UpdateOttrInformation(OttrMonitoring ottrMonitoring);
        Task<List<OttrMonitoring>> AddOTTRInformationAll(List<OttrMonitoring> ottrMonitoring);
        Task<List<OttrMonitoring>> Delete(List<OttrMonitoring> ottrMonitoring);
        Task<List<OttrMonitoringModel>> GetAllOTTRInformationWithFilters(Dictionary<string, Filers> ottrFilters, int gesGroupID, string country, List<string> listOfRoles); 
        List<OttrMonitoring> GetOTTRList();
        // Task<IEnumerable<OttrMonitoring>> UpdateOTTRInformation(List<OttrMonitoring> ottrMonitoring);
        Task<List<OttrMonitoring>> GetAllOTTRInformation();
    }
}