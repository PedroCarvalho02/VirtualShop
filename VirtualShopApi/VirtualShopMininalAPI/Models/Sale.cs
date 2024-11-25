namespace VirtualShopMinimalAPI.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataVenda { get; set; }
    }
}