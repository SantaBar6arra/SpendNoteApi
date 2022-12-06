using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ProductCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; }
        public virtual User User { get; set; }
    }
}
