using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public class AttributeToRepository : IAttributeToRepository
    {
        private readonly ApplicationDbContext context;
        public AttributeToRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Get the List of AttributeTo Information
        public async Task<IEnumerable<AttributeTo>> GetAllAttributeToInformation()
        {
            var listOfAttributeToInfo = context.AttributeTo.Where(x => x.IsActive == true).OrderBy(y => y.AttributeToName);
            return listOfAttributeToInfo;
        }

        //Get AttributeTo Name by AttributeTo Id
        public async Task<string> GetAttributeToNameByAttributeToId(int attributeToId)
        {
            try
            {
                var attributeToName = context.AttributeTo.Where(x => (x.AttributeToId == attributeToId) && (x.IsActive == true)).Select(y => y.AttributeToName).FirstOrDefault();
                return attributeToName;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int GetAttributeToIdByAttributeToName(string name)
        {
            try
            {
                if (name.ToUpper().Equals("DELAY")) {
                    name = "Delayed-On Hold";
                }
                if (name.ToUpper().Equals("ONTIME"))
                {
                    name = "On Time";
                }
               
                    var attributeToName = context.AttributeTo.Where(x => (x.AttributeToName.ToUpper().Equals(name.ToUpper()))
                && (x.IsActive == true)).FirstOrDefault();

                if (attributeToName != null)
                {
                    return attributeToName.AttributeToId;
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

    }
}
