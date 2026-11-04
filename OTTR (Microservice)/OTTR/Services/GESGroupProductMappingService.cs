using OTTR.Models;
using OTTR.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Services
{
    public interface IGESGroupProductMappingService
    {
        Task<IEnumerable<GESGroupProductMapping>> GetListOfGESGroupProductMappingInfo();
    }

    public class GESGroupProductMappingService : IGESGroupProductMappingService
    {
        private readonly IGESGroupProductMappingRepository gesGroupProductMappingRepository;
        public GESGroupProductMappingService(IGESGroupProductMappingRepository gesGroupProductMappingRepository)
        {
            this.gesGroupProductMappingRepository = gesGroupProductMappingRepository;
            
        }

        //Get the List of All GESGroupProductMapping Information
        public async Task<IEnumerable<GESGroupProductMapping>> GetListOfGESGroupProductMappingInfo()
        {
            try
            {
                var listOfGESGroupProductMapping = await gesGroupProductMappingRepository.GetAllGESGroupProductMappingInformation();
                return listOfGESGroupProductMapping;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
