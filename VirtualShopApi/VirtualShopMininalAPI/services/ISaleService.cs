using VirtualShopMinimalAPI.Models;

public interface ISaleService
{
    Task<IResult> RegisterSale(SaleRequest saleRequest);
}