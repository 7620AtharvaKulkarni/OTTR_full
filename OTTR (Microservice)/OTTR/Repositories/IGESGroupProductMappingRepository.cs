using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public interface IGESGroupProductMappingRepository
    {
        Task<IEnumerable<GESGroupProductMapping>> GetAllGESGroupProductMappingInformation();
        bool checkGroupIdAndProductId(int gesGroupId, int ProductId);
    }
}
