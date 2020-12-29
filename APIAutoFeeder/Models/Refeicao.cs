using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace APIAutoFeeder.Models
{
    [Table("tbRefeicao")]
    public class Refeicao
    {
        [Key]
        public int id { get; set; }
        public int numero { get; set; }
        public TimeSpan horario { get; set; }
        public int quantidade { get; set; }
        public bool ativo { get; set; }

        [Required]
        [ForeignKey("User")]
        public string userId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}