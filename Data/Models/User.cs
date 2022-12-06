using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(30)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public string PasswordHash { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Email { get; set; }
        public string Salt { get; set; }
    }
}
