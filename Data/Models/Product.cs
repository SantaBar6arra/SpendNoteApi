using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; }
        public virtual List List { get; set; }
        public virtual ProductCategory Category { get; set; }
        public decimal Price { get; set; }
        public bool IsBought { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? BuyUntilDate { get; set; }
    }
}
