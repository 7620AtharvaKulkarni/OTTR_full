using OTTR.Models;
using OTTR.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Services
{
    public interface IAttributeToService
    {
        Task<IEnumerable<AttributeTo>> GetListOfAttributeToInfo();
        Task<string> GetAttributeToNameByAttributeToId(int attributeToId);
    }

    public class AttributeToService : IAttributeToService
    {
        private readonly IAttributeToRepository attributeToRepository;
        public AttributeToService(IAttributeToRepository attributeToRepository)
        {
            this.attributeToRepository = attributeToRepository;
            
        }

        //Get the List of All AttributeTo Information
        public async Task<IEnumerable<AttributeTo>> GetListOfAttributeToInfo()
        {
            try
            {
                var listOfAttributeTo = await attributeToRepository.GetAllAttributeToInformation();
                return listOfAttributeTo;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Get AttributeTo Name by AttributeTo Id
        public async Task<string> GetAttributeToNameByAttributeToId(int attributeToId)
        {
            try
            {
                var attributeToName = await attributeToRepository.GetAttributeToNameByAttributeToId(attributeToId);
                return attributeToName;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
