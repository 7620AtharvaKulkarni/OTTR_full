using OTTR.Models;
using OTTR.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Services
{
    public interface IGesgroupdeliveryService
    {
        Task<IEnumerable<Gesgroupdelivery>> GetListOfRegion();
        Task<string> GetRegionNameByRegionId(int regionId);
    }

    public class GesgroupdeliveryService : IGesgroupdeliveryService
    {
        private readonly IGesgroupdeliveryRepository gesgroupdeliveryRepository;
        public GesgroupdeliveryService(IGesgroupdeliveryRepository regionRepository)
        {
            this.gesgroupdeliveryRepository = regionRepository;
            
        }

        //Get the List of All Regions
        public async Task<IEnumerable<Gesgroupdelivery>> GetListOfRegion()
        {
            try
            {
                var listOfRegion = await gesgroupdeliveryRepository.GetAllRegionInformation();
                return listOfRegion;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Get Region Name by Region Id
        public async Task<string> GetRegionNameByRegionId(int regionId)
        {
            try
            {
                var regionName = await gesgroupdeliveryRepository.GetRegionNameByRegionId(regionId);
                return regionName;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
