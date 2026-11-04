using System.Collections.Generic;
using System.Threading.Tasks;
using OTTR.Models;

namespace OTTR.Repositories
{
    public interface IRegionCountryMappingRepository
    {
        Task<List<int>> GetAllCountryIdsByRegionId(int regionId);
        bool checkRegionAndCountry(int regionId, int countryId);
    }
}
