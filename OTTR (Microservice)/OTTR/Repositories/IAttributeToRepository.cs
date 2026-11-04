using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public interface IAttributeToRepository
    {
        Task<IEnumerable<AttributeTo>> GetAllAttributeToInformation();
        Task<string> GetAttributeToNameByAttributeToId(int attributeToId);
      int   GetAttributeToIdByAttributeToName(string name);
    }
}
