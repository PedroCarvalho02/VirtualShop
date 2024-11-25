namespace VirtualShopMinimalAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public decimal Preco { get; set; }
        public string? ImageUrl { get; set; } 
        public ICollection<SaleProduct>? SaleProducts { get; set; } 
    }
}