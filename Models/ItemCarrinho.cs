using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APiTurboSetup.Models
{
    public class ItemCarrinho
    {
        [Key]
        public int ItemCarrinhoId { get; set; }

        [Required]
        public int CarrinhoId { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int Quantidade { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        [NotMapped] // Calculado, não armazenado diretamente no banco
        public decimal Subtotal => Quantidade * PrecoUnitario;

        [Required]
        public string StatusItem { get; set; } = "ativo"; // "ativo" ou "removido"

        // Propriedade de navegação para o Carrinho pai
        [ForeignKey("CarrinhoId")]
        public virtual Carrinho? Carrinho { get; set; }

        // Se você tiver um modelo Produto, pode adicionar uma propriedade de navegação aqui também
        // public virtual Produto Produto { get; set; }
        // AQUI: Propriedade de navegação para o Produto (dados completos)

        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; } // Torna o Produto opcional (nullable)


    }
}

