using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace OTTR.Repositories
{
    public class GESGroupRepository : IGESGroupRepository
    {
        private readonly ApplicationDbContext context;
        public GESGroupRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Get the List of GES Group
        public async Task<IEnumerable<GESGroup>> GetAllGESGroupInformation()
        {
            // var listOfGESGroup = context.GESGroups.Where(x => x.IsActive == true).OrderBy(y => y.GESGroupName).ToList();
            // var listOfGESGroup = context.GESGroups.Where(x => x.IsActive == true).OrderBy(y => y.GESGroupName).GroupBy(p => p.GESGroupName)
            //.Select(g => g.First())
            //.ToList();

            var listOfGESGroup = context.GESGroups.Where(x => x.IsActive == true).OrderBy(y => y.GESGroupName).ToLookup(p => p.GESGroupName).Select(coll => coll.First()).ToList();
            //var listOfGESGroup = context.GESGroups.FromSqlRaw("SELECT * FROM dbo.Blogs").ToList();

            return listOfGESGroup;
        }

        
        //Get GESGroup Id  by GESGroup Name
        public  int GetGESGroupIdByGESGroupName(string name)
        {
            var id = context.GESGroups.Where(x => x.IsActive == true && x.GESGroupName.ToUpper().Equals(name.ToUpper())).FirstOrDefault().GESGroupId;
            return id;
        }
        public int GetGESGroupIdByGESSubGroupName(string name)
        {
            var id = context.GESGroups.Where(x => x.IsActive == true && x.GESSubGroupName.ToUpper().Equals(name.ToUpper())).FirstOrDefault().GESGroupId;
            return id;
        }

        public int GetGESGroupIDByCountryAndSubGroup(string Lob,string subGroup)
        {
            Lob = Lob.Replace(" ", string.Empty);
            //string sqlScript = "Select GES_Group_Id from tblOTTR_GESGroup where RePlace(GES_Group,' ','')='" + Lob+"'and GES_Sub_Group ='"+subGroup+"'";
            //var group = context.GESGroups.FromSqlRaw(sqlScript).Select(x=>x.GESGroupId).FirstOrDefault();

            var group = context.GESGroups.Where(x => x.IsActive == true && x.GESGroupName.Replace(" ", "").Equals(Lob) && x.GESSubGroupName.ToUpper().Equals(subGroup.ToUpper())).FirstOrDefault();
            if (group != null)
            {
                return group.GESGroupId;
            }
            else
            {
                return 0;
            }
           
        }
        public List<int> GetAllGESGroupIdByGESGroupName(string name)
        {
           
           var id = context.GESGroups.Where(x => x.IsActive == true && x.GESGroupName.ToUpper().Equals(name.ToUpper())).Select(x=>x.GESGroupId).ToList();
            return id;
        }
        //Get GESGroup Name by GESGroup Id
        public async Task<string> GetGESGroupNameByGESGroupId(int gesGroupId)
        {
            try
            {
                var gesGroupName = context.GESGroups.Where(x => (x.GESGroupId == gesGroupId) && (x.IsActive == true)).Select(y => y.GESGroupName).FirstOrDefault();
                return gesGroupName;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public async Task<string> GetGESSubGroupNameByGESGroupId(int gesGroupId)
        {
            try
            {
                var gesGroupName = context.GESGroups.Where(x => (x.GESGroupId == gesGroupId) && (x.IsActive == true)).Select(y => y.GESSubGroupName).FirstOrDefault();
                return gesGroupName;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
        public async Task<IEnumerable<GESGroup>> GetGESubGroupByGESGroupName(string name)
        {
            try
            {
               
                    var gesGroupName = context.GESGroups.Where(x => (x.GESGroupName == name) && (x.IsActive == true)).ToList();
                    return gesGroupName;
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<GESGroupmodel>> GetSubGroupWithGroup()
        {
            try
            {

                var gesGroupName = context.GESGroups.Where(x => x.IsActive == true).OrderBy(y => y.GESGroupName).ToLookup(p => p.GESGroupName).Select(coll => coll.First()).ToList();
                List<GESGroupmodel> groups = new List<GESGroupmodel>();
                foreach (var gesgroup in gesGroupName)
                {
                    GESGroupmodel groupmodel = new GESGroupmodel();
                    groupmodel.GroupName = gesgroup.GESGroupName;
                    //var SubGroup  =context.GESGroups.Where(x => (x.GESGroupName == name) && (x.IsActive == true)).ToList().Select(x=>x.GESSubGroupName);
                    var subGroup = context.GESGroups.Where(x => (x.GESGroupName == gesgroup.GESGroupName) && (x.IsActive == true)).ToList().Select(x => new { x.GESSubGroupName, x.GESGroupId }).ToList();
                    List<GESSubGroupmodel> subGroupModelList = new List<GESSubGroupmodel>();
                    foreach(var sub in subGroup)
                    {
                        GESSubGroupmodel subG = new GESSubGroupmodel();
                        subG.SubgroupName = sub.GESSubGroupName;
                        subG.SubgroupId = sub.GESGroupId;
                        subGroupModelList.Add(subG);
                    }
                    groupmodel.GesSubGroup = subGroupModelList;
                    groups.Add(groupmodel);
                }


                return groups;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
    }
}
