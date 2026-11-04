using OTTR.Models;
using OTTR.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Services
{
    public interface IInternalExternalAttributeToService
    {
        Task<IEnumerable<InternalExternalAttributeTo>> GetListOfAttributeToInfo();
        Task<string> GetAttributeToNameByAttributeToId(int attributeToId);
    }

    public class InternalExternalAttributeToService : IInternalExternalAttributeToService
    {
        private readonly IInternalExternalAttributeToRepository attributeToRepository;
        public InternalExternalAttributeToService(IInternalExternalAttributeToRepository attributeToRepository)
        {
            this.attributeToRepository = attributeToRepository;
            
        }

        //Get the List of All AttributeTo Information
        public async Task<IEnumerable<InternalExternalAttributeTo>> GetListOfAttributeToInfo()
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
