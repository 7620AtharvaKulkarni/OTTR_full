using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OTTR.Models;

namespace OTTR.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<CountryRepository> logger;
        public CountryRepository(ApplicationDbContext context, ILoggerFactory logger)
        {
            this.context = context;
            this.logger = logger.CreateLogger<CountryRepository>();
        }

        //Get the List of Country Information
        public async Task<IEnumerable<Country>> GetAllCountryInformation()
        {
            try
            {
                var listOfCountryInformation = context.Countries.Where(x => x.IsActive == true).OrderBy(y => y.CountryName);
                logger.LogInformation("GetAllCountryInformation and return to the Country service layer");
                return listOfCountryInformation;
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return null;
            }
        }

        //Get Country Name by CountryId
        public async Task<string> GetCountryNameByCountryId(int? countryId)
        {
            try
            {
                var country = context.Countries.Where(x => (x.CountryID == countryId) && (x.IsActive == true)).FirstOrDefault();
                logger.LogInformation("GetCountryNameByCountryId and return to the Country service layer");
                return country.CountryName;
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return null;
            }
        }

        public async Task<IEnumerable<Country>> GetListOfCountriesByCountryIds(List<int> listOfCountryIds)
        {
            try
            {
                var listOfCountries = context.Countries.Where(x => listOfCountryIds.Contains(x.CountryID) && (x.IsActive == true));
                logger.LogInformation("GetListOfCountriesByCountryIds and return to the Country service layer");
                return listOfCountries;
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return null;
            }
        }

        public int GetCountryIdByCountryName(string CountryName)
        {
            
                var country = context.Countries.Where(x => (x.CountryName.Trim().ToLower() == CountryName.Trim().ToLower()) && (x.IsActive == true)).FirstOrDefault();
                logger.LogInformation("GetCountryNameByCountryId and return to the Country service layer");
            if (country != null)
            {
                return country.CountryID;
            }
            else
            {
                return 0;
            }
           
        }

    }
}