using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APiTurboSetup.Models
{
    public class ProdutoImagem
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "A URL da imagem é obrigatória")]
        [StringLength(500, ErrorMessage = "A URL deve ter no máximo 500 caracteres")]
        public string Url { get; set; } = string.Empty;
        
        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
        public string? Titulo { get; set; }
        
        // Ordem de exibição da imagem (para permitir ordenação)
        public int Ordem { get; set; }
        
        // Chave estrangeira
        public int ProdutoId { get; set; }
        
        // Propriedade de navegação - uma imagem pertence a um produto
        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; }
    }
} 