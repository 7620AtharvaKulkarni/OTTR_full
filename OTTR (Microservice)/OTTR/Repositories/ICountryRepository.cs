using System.Collections.Generic;
using System.Threading.Tasks;
using OTTR.Models;

namespace OTTR.Repositories
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllCountryInformation();
        Task<IEnumerable<Country>> GetListOfCountriesByCountryIds(List<int> listOfCountryIds);
        Task<string>  GetCountryNameByCountryId(int? countryId);

        int GetCountryIdByCountryName(string CountryName);
    }
}
