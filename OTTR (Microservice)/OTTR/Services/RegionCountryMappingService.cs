using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OTTR.Models;
using OTTR.Repositories;

namespace OTTR.Services
{
    public interface IRegionCountryMappingService
    {
        Task<IEnumerable<Country>> GetListOfCountriesByRegionId(int regionId);
    }
    public class RegionCountryMappingService : IRegionCountryMappingService
    {
        private readonly IRegionCountryMappingRepository regionCountryMappingRepository;
        private readonly ICountryRepository countryRepository;
        private readonly ILogger<RegionCountryMappingService> logger;

        public RegionCountryMappingService(IRegionCountryMappingRepository regionCountryMappingRepository, ICountryRepository countryRepository,ILoggerFactory logger)
        {
            this.regionCountryMappingRepository = regionCountryMappingRepository;
            this.countryRepository = countryRepository;
            this.logger = logger.CreateLogger<RegionCountryMappingService>();
        }

        //Get the List of All Country Information by Region Id
        public async Task<IEnumerable<Country>> GetListOfCountriesByRegionId(int regionId)
        {
            try
            {
                logger.LogInformation(" GetListOfCountriesByRegionId for get list of RegionCountryMappingService");
                var listOfCountryIds = await regionCountryMappingRepository.GetAllCountryIdsByRegionId(regionId);
                var listOfCountries = await countryRepository.GetListOfCountriesByCountryIds(listOfCountryIds);
                return listOfCountries;
            }
            catch(Exception e)
            { 
                logger.LogError("Error occured", e);
                return null;
            }
        }
    }
}
