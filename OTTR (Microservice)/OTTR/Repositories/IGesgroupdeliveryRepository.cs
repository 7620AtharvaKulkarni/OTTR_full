using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public interface IGesgroupdeliveryRepository
    {
        Task<IEnumerable<Gesgroupdelivery>> GetAllRegionInformation();
        Task<string> GetRegionNameByRegionId(int regionId);
        int GetRegionIdByRegionName(string name);

        
    }
}
