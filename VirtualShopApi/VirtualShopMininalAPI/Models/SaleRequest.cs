namespace VirtualShopMinimalAPI.Models
{
    public class SaleRequest
    {
        public Sale? Sale { get; set; }
        public int[]? ProductIds { get; set; }
        public int[]? Quantidades { get; set; }
    }
}