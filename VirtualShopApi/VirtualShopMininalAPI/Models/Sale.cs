namespace VirtualShopMinimalAPI.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } 
        public ICollection<SaleProduct>? SaleProducts { get; set; } 
    }
}