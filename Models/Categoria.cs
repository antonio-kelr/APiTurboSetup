using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        // Propriedade de navegação - uma categoria pode ter muitos produtos
        public virtual ICollection<Produto>? Produtos { get; set; }
    }
} 