using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OTTR.Models;


namespace OTTR.Repositories
{
    public class RegionCountryMappingRepository : IRegionCountryMappingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<RegionCountryMappingRepository> logger;
        public RegionCountryMappingRepository(ApplicationDbContext context,ILoggerFactory logger)
        {
            this.context = context;
            this.logger = logger.CreateLogger<RegionCountryMappingRepository>();
        }

        //Get the List of All RegionCountryMapping Information by Region Id
        public async Task<List<int>> GetAllCountryIdsByRegionId(int regionId)
        {
            try
            {
            var listOfCountryIds = context.RegionCountryMapping.Where(x => (x.RegionID == regionId) && (x.IsActive == true)).OrderByDescending(y => y.ModifiedOn).Select( y => y.CountryID).ToList();
            logger.LogInformation("Get list of GetAllCountryIdsByRegionId and return to the RegionCountryMapping service layer");
            return listOfCountryIds;
            }
            catch(Exception e)
            {
                logger.LogError("Error occured", e);
                return null;
            }       
        }

        public bool checkRegionAndCountry(int regionId,int countryId)
        {
            try
            {
                var Country = context.RegionCountryMapping.Where(x => (x.RegionID == regionId) && (x.IsActive == true) && (x.CountryID == countryId)).Any();
                logger.LogInformation("Get list of GetAllCountryIdsByRegionId and return to the RegionCountryMapping service layer");
                if (Country)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return false;
            }
        }

    }
}
