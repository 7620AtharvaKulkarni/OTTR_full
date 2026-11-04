using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public interface IGESGroupRepository
    {
        Task<IEnumerable<GESGroup>> GetAllGESGroupInformation();
        Task<string> GetGESGroupNameByGESGroupId(int gesGroupId);
        Task<string> GetGESSubGroupNameByGESGroupId(int gesGroupId);
        int GetGESGroupIdByGESGroupName(string name);
        int GetGESGroupIdByGESSubGroupName(string name);

        int GetGESGroupIDByCountryAndSubGroup(string Country, string SubGroupName);

        Task<IEnumerable<GESGroup>> GetGESubGroupByGESGroupName(string name);

        Task<List<GESGroupmodel>> GetSubGroupWithGroup();

        



    }
}
