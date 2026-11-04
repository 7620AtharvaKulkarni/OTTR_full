using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OTTR.Repositories;
using OTTR.Services;
using OTTR.Models;

namespace OTTR.Services
{
    public interface IPoleService
    {
        Task<IEnumerable<Pole>> GetListOfPoleInfo();
    }

    public class PoleService : IPoleService
    {
        private readonly IPoleRepository poleRepository;
        private readonly ILogger<PoleService> logger;

        public PoleService(IPoleRepository poleRepository,ILoggerFactory logger)
        {
            this.poleRepository = poleRepository;
            this.logger = logger.CreateLogger<PoleService>();
        }

        //Get the List of All Pole Information
        public async Task<IEnumerable<Pole>> GetListOfPoleInfo()
        {
            try
            {
                logger.LogInformation("GetListOfPoleInfo for get list of Pole");
                var listOfPoleInfo = await poleRepository.GetAllPoleInformation();
                return listOfPoleInfo;
            }
            catch(Exception e)
            { 
                logger.LogError("Error occured", e);
                return null;
            }
        }
    }
}
