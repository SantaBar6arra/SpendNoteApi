using Data.Models;

namespace WebApi.Dtos
{
    public class IncomeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
    }
}
