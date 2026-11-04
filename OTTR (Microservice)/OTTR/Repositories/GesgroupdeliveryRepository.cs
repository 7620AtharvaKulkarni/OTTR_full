using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public class GesgroupdeliveryRepository : IGesgroupdeliveryRepository
    {
        private readonly ApplicationDbContext context;
        public GesgroupdeliveryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Get the List of Region
        public async Task<IEnumerable<Gesgroupdelivery>> GetAllRegionInformation()
        {
            var listOfRegion = context.GesGroupDelivery.Where(x => x.IsActive == true).OrderBy(y => y.RegionName);
            return listOfRegion;
        }

        //Get Region Name by Region Id
        public int GetRegionIdByRegionName(string name)
        {
            try
            {
                var regionName = context.GesGroupDelivery.Where(x => (x.RegionName == name) && (x.IsActive == true)).FirstOrDefault();
                if (regionName != null)
                {
                    return regionName.RegionId;
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
        public async Task<string> GetRegionNameByRegionId(int regionId)
        {
            try
            {
                var regionName = context.GesGroupDelivery.Where(x => (x.RegionId == regionId) && (x.IsActive == true)).Select(y => y.RegionName).FirstOrDefault();
                return regionName;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
