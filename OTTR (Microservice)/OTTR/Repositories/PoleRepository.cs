using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OTTR.Models;

namespace OTTR.Repositories
{
    public class PoleRepository : IPoleRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<PoleRepository> logger;
        public PoleRepository(ApplicationDbContext context,ILoggerFactory logger)
        {
            this.context = context;
            this.logger = logger.CreateLogger<PoleRepository>();
        }

        //Get the List of Pole Information
        public async Task<IEnumerable<Pole>> GetAllPoleInformation()
        {
            try
            {
                var listOfPoleInformation = context.Poles.Where(x => x.IsActive == true).OrderBy(y => y.PoleName).ToList();
                logger.LogInformation("GetAllPoleInformation and return to the Pole service layer");
                return listOfPoleInformation;
            }
            catch(Exception e)
            {
                logger.LogError("Error occured", e);
                return null;
            }
        }

        //Get LoOB Name by LOB Id
        public async Task<string> GetPoleNameByPoleId(int? poleId)
        {
            try
            {
                var pole = context.Poles.Where(x => (x.PoleID == poleId) && (x.IsActive == true)).FirstOrDefault();
                logger.LogInformation("GetPoleNameByPoleId and return to the Pole service layer");
                return pole.PoleName;
            }
            catch(Exception e)
            {
                logger.LogError("Error occured", e);
                return null;
            }
        }

        public  int GetPoleIdByPoleName(string PoleName)
        {
             var pole = context.Poles.Where(x => (x.PoleName.Trim().ToLower() == PoleName.Trim().ToLower()) && (x.IsActive == true)).FirstOrDefault();
                logger.LogInformation("GetPoleNameByPoleId and return to the Pole service layer");
            if (pole != null)
            {
                return pole.PoleID;
            }
            else
            {
                return 0;
            }
           
        }
    }
}