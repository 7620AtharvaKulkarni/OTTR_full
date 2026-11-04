using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public class GESGroupProductMappingRepository : IGESGroupProductMappingRepository
    {
        private readonly ApplicationDbContext context;
        public GESGroupProductMappingRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Get the List of GESGroupProductMapping Information
        public async Task<IEnumerable<GESGroupProductMapping>> GetAllGESGroupProductMappingInformation()
        {
            var listOfGESGroupProductMapping = context.GESGroupProductMappings;
            return listOfGESGroupProductMapping;
        }

        public bool checkGroupIdAndProductId(int gesGroupId, int ProductId)
        {
            try
            {
                var gesGroupProductMapping = context.GESGroupProductMappings.Where(x => (x.GESGroupId == gesGroupId) && (x.ProductId == ProductId)).Any();
                if (gesGroupProductMapping)
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
                return false;
            }
        }
    }
}
