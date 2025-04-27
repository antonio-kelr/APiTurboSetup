using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APiTurboSetup.Models
{
    public class Produto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(150, ErrorMessage = "O slug deve ter no máximo 150 caracteres")]
        public string? Slug { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }
        
        [Required(ErrorMessage = "O preço do produto é obrigatório")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }
        
        // Chave estrangeira
        public int CategoriaId { get; set; }
        
        // Propriedade de navegação - um produto pertence a uma categoria
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }
        
        // Propriedade de navegação - um produto pode ter várias imagens
        public virtual ICollection<ProdutoImagem>? Imagens { get; set; }
    }
} 