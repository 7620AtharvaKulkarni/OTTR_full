using System.Collections.Generic;
using System.Threading.Tasks;
using OTTR.Models;

namespace OTTR.Repositories
{
    public interface IPoleRepository
    {
        Task<IEnumerable<Pole>> GetAllPoleInformation();
        Task<string> GetPoleNameByPoleId(int? poleId);
        int GetPoleIdByPoleName(string PoleName);
    }
}
