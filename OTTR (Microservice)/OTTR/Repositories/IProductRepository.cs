using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductInformation();
        Task<string> GetProductNameByProductId(int productId);

       int GetProductIdByProductName(string name);
    }
}