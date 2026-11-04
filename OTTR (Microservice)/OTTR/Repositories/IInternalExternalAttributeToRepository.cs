using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public interface IInternalExternalAttributeToRepository
    {
        Task<IEnumerable<InternalExternalAttributeTo>> GetAllAttributeToInformation();
        Task<string> GetAttributeToNameByAttributeToId(int IInternalExternalAttributeToId);
      int   GetAttributeToIdByAttributeToName(string name);
    }
}
