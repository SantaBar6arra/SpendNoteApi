using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class VerificationCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(36)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [Index(IsUnique = true)]
        public int User_Id { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }
    }
}
