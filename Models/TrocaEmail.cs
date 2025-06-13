using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Models
{
    public class TrocaEmail
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string EmailAtual { get; set; }
        
        [Required]
        public string NovoEmail { get; set; }
        
        [Required]
        public string Codigo { get; set; }
        
        [Required]
        public DateTime Expiracao { get; set; }
        
        public bool Utilizado { get; set; }
    }
} 