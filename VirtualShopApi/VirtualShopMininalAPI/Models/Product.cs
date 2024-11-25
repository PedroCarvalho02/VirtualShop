namespace VirtualShopMinimalAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int UserId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}