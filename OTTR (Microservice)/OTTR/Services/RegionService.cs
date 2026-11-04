using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OTTR.Models;
using OTTR.Repositories;

namespace OTTR.Services
{
    public interface IRegionService
    {
        Task<IEnumerable<Region>> GetListOfRegionInfo();
        Task<IEnumerable<Region>> GetListOfRegionByPoleId(int poleID);
    }

    public class RegionService : IRegionService
    {
        private readonly IRegionRepository regionRepository;
        private readonly ILogger<RegionService> logger;

        public RegionService(IRegionRepository regionRepository,ILoggerFactory logger)
        {
            this.regionRepository = regionRepository;
            this.logger = logger.CreateLogger<RegionService>();
        }

        //Get the List of All Region Information
        public async Task<IEnumerable<Region>> GetListOfRegionInfo()
        {
            try
            {
                logger.LogInformation(" GetListOfCountriesByRegionId for get list of RegionService");
                var listOfRegionInfo = await regionRepository.GetAllRegionInformation();
                return listOfRegionInfo;
            }
            catch(Exception e)
            { 
                logger.LogError("Error occured", e);
                return null;
            }
        }

        //Get the List of All Region Information by Pole ID
        public async Task<IEnumerable<Region>> GetListOfRegionByPoleId(int poleID)
        {
            try
            {
                logger.LogInformation(" GetListOfRegionByPoleId for get list of RegionService");
                var listOfRegionInfo = await regionRepository.GetRegionNameByPoleId(poleID);
                return listOfRegionInfo;
            }
            catch(Exception e)
            { 
                logger.LogError("Error occured", e);
                return null;
            }
        }
    }
}
