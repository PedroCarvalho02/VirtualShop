using System.Threading.Tasks;
using VirtualShopMinimalAPI.Models;

namespace VirtualShopMinimalAPI.Services
{
    public interface IProductService
    {
        Task<IResult> AddProduct(Product product);
        Task<IResult> GetProducts();
        Task<IResult> SearchProducts(string? name);
        Task<IResult> DeleteProduct(int id);
        Task<IResult> UpdateProduct(int id, Product updatedProduct);
    }
}