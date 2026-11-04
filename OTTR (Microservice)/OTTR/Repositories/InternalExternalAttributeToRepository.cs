using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public class InternalExternalAttributeToRepository : IInternalExternalAttributeToRepository
    {
        private readonly ApplicationDbContext context;
        public InternalExternalAttributeToRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Get the List of AttributeTo Information
        public async Task<IEnumerable<InternalExternalAttributeTo>> GetAllAttributeToInformation()
        {
            var listOfAttributeToInfo = context.InternalExternalAttributeTo.Where(x => x.IsActive == true).OrderBy(y => y.InternalExternalAttributeToName).ToList();
            return listOfAttributeToInfo;
        }

        //Get AttributeTo Name by AttributeTo Id
        public async Task<string> GetAttributeToNameByAttributeToId(int attributeToId)
        {
            try
            {
                var attributeToName = context.InternalExternalAttributeTo.Where(x => (x.InternalExternalAttributeToId == attributeToId) && (x.IsActive == true)).Select(y => y.InternalExternalAttributeToName).FirstOrDefault();
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
                var attributeToName = context.InternalExternalAttributeTo.Where(x => (x.InternalExternalAttributeToName.ToUpper().Equals(name.ToUpper()))
                && (x.IsActive == true)).Select(y => y.InternalExternalAttributeToId).FirstOrDefault();
                return attributeToName;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

    }
}
