using OTTR.Models;
using OTTR.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Services
{
    public interface IGESGroupService
    {
        Task<IEnumerable<GESGroup>> GetListOfGESGroupInfo();
        Task<string> GetGESGroupNameByGESGroupId(int gesGroupId);
        Task<IEnumerable<GESGroup>> GetGESubGroupByGESGroupName(string name);
        Task<List<GESGroupmodel>> GetSubGroupWithGroup();
    }

    public class GESGroupService : IGESGroupService
    {
        private readonly IGESGroupRepository gesGroupRepository;
        public GESGroupService(IGESGroupRepository gesGroupRepository)
        {
            this.gesGroupRepository = gesGroupRepository;
            
        }

        //Get the List of All GES Group Information
        public async Task<IEnumerable<GESGroup>> GetListOfGESGroupInfo()
        {
            try
            {
                var listOfGESGroup = await gesGroupRepository.GetAllGESGroupInformation();
                return listOfGESGroup;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Get GESGroup Name by GESGroup Id
        public async Task<string> GetGESGroupNameByGESGroupId(int gesGroupId)
        {
            try
            {
                var gesGroupName = await gesGroupRepository.GetGESGroupNameByGESGroupId(gesGroupId);
                return gesGroupName;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<GESGroup>> GetGESubGroupByGESGroupName(string groupName)
        {
            try
            {
                var gesGroupName = await gesGroupRepository.GetGESubGroupByGESGroupName(groupName);
                return gesGroupName;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<GESGroupmodel>> GetSubGroupWithGroup()
        {
            try
            {
                var gesGroupName = await gesGroupRepository.GetSubGroupWithGroup();
                return gesGroupName;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
    }
}
