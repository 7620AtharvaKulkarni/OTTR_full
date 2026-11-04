using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly ApplicationDbContext context;
        public RegionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Get the List of Region
        public async Task<IEnumerable<Region>> GetAllRegionInformation()
        {
            var listOfRegion = context.Regions.Where(x => x.IsActive == true).OrderBy(y => y.RegionName);
            return listOfRegion;
        }

        //Get Region Name by Region Id
        public int GetRegionIdByRegionName(string name)
        {
            try
            {
               
                var regionName = context.Regions.Where(x => (x.RegionName.Trim().ToLower() == name.ToLower().Trim()) && (x.IsActive == true)).FirstOrDefault();
                if (regionName != null)
                {
                    return regionName.RegionID;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        //Get Region Name by Region Id
        public async Task<string> GetRegionNameByRegionId(int? regionId)
        {
            try
            {
                var regionName = context.Regions.Where(x => (x.RegionID == regionId) && (x.IsActive == true)).Select(y => y.RegionName).FirstOrDefault();
                return regionName;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public bool checkPoleAndRegion(int poleId ,int regionId)
        {
            try
            {
                var regionName = context.Regions.Where(x => (x.RegionID == regionId) && (x.IsActive == true) && (x.PoleID == poleId)).Any();
                if (regionName)
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
                throw e;
            }
        }

        //Get the List of All Region Information by Pole ID
        public async Task<IEnumerable<Region>> GetRegionNameByPoleId(int poleID)
        {
            try
            {
                var region = context.Regions.Where(x => (x.PoleID == poleID) && (x.IsActive == true)).ToList();
                //logger.LogInformation("GetRegionNameByPoleId and return to the Region service layer");
                return region;
            }
            catch (Exception e)
            {
               // logger.LogError("Error occured", e);
                return null;
            }
        }

        //Get Region Name by Region Id

    }
}
