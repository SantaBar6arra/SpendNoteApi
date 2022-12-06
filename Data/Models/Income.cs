using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Income
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public virtual IncomeCategory Category { get; set; }
        public virtual User User { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
    }
}
