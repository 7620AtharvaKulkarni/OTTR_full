using OTTR.Models;
using OTTR.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetListOfProductInfo();
        Task<string> GetProductNameByProductId(int productId);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
            
        }

        //Get the List of All Product Information
        public async Task<IEnumerable<Product>> GetListOfProductInfo()
        {
            try
            {
                var listOfProduct = await productRepository.GetAllProductInformation();
                return listOfProduct;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Get Product Name by Product Id
        public async Task<string> GetProductNameByProductId(int productId)
        {
            try
            {
                var productName = await productRepository.GetProductNameByProductId(productId);
                return productName;
            }
            catch(Exception e)
            {
                return null;
            }
        }


        public int GetProductIdByProductName(string name)
        {
            try
            {
                var productName = productRepository.GetProductIdByProductName(name);
                return productName;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
