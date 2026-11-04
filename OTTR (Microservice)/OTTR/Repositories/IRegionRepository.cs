using System.Collections.Generic;
using System.Threading.Tasks;
using OTTR.Models;

namespace OTTR.Repositories
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllRegionInformation();
        Task<string> GetRegionNameByRegionId(int? regionId);
        Task<IEnumerable<Region>> GetRegionNameByPoleId(int poleID);

        int GetRegionIdByRegionName(string regionName);
        bool checkPoleAndRegion(int poleId, int regionId);
    }
}
