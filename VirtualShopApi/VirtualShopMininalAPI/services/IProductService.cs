using VirtualShopMinimalAPI.Models;

public interface IProductService
{
    Task<IResult> AddProduct(Product product);
    Task<IResult> GetProducts();
    Task<IResult> SearchProducts(string? name);
    Task<IResult> DeleteProduct(int id);
}