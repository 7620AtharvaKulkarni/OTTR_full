using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext context;
        public ProductRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Get the List of Product Information
        public async Task<IEnumerable<Product>> GetAllProductInformation()
        {
            var listOfProduct = context.Products.Where(x => x.IsActive == true).OrderBy(y => y.ProductName);
            return listOfProduct;
        }

        //Get Product Name by Product Id
        public async Task<string> GetProductNameByProductId(int productId)
        {
            try
            {
                var productName = context.Products.Where(x => (x.ProductId == productId) && (x.IsActive == true)).Select(y => y.ProductName).FirstOrDefault();
                return productName;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        //Get Product Name by Product Id
        public  int GetProductIdByProductName(string name)
        {
            try
            {
                var productName = context.Products.Where(x => (x.ProductName.ToUpper().Equals(name.ToUpper())) && (x.IsActive == true)).FirstOrDefault();
                if (productName != null)
                {
                    return productName.ProductId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
