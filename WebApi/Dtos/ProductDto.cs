using System.Runtime.CompilerServices;

namespace WebApi.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ListId { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }  
        public bool IsBought { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? BuyUntilDate { get; set; }
    }
}
