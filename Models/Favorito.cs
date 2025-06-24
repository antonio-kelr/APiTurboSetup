using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APiTurboSetup.Models
{
    public class Favorito
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        // Propriedades de navegação
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; }
    }
} 