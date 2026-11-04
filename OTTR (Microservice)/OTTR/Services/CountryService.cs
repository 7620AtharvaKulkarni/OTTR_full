using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OTTR.Models;
using OTTR.Repositories;

namespace OTTR.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetListOfCountryInfo();
    }

    public class CountryService : ICountryService
    {
        private readonly ICountryRepository countryRepository;
        private readonly ILogger<CountryService> logger;

        public CountryService(ICountryRepository countryRepository,ILoggerFactory logger)
        {
            this.countryRepository = countryRepository;
            this.logger = logger.CreateLogger<CountryService>();
        }

        //Get the List of All Country Information
        public async Task<IEnumerable<Country>> GetListOfCountryInfo()
        {
            try
            {
                logger.LogInformation("GetListOfCountryInfo for get list of Country");
                var listOfCountryInfo = await countryRepository.GetAllCountryInformation();
                return listOfCountryInfo;
            }
            catch(Exception e)
            { 
                logger.LogError("Error occured", e);
                return null;
            }
        }
    }
}
