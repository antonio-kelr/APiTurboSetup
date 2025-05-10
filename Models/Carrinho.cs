using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace APiTurboSetup.Models
{
    public class Carrinho
    {
        [Key]
        public int CarrinhoId { get; set; }

        [Required]
        public int UsuarioId { get; set; } // Assumindo int, pode ser string se preferir GUID

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime DataAtualizacao { get; set; } = DateTime.Now; // Inicializa com DataCriacao

        [Required]
        public string Status { get; set; } = "ativo"; // "ativo", "finalizado", "abandonado"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } // Este campo pode ser atualizado via triggers ou lógica de aplicação

        public virtual List<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();

        [NotMapped] // Propriedade computada, não mapeada para o banco de dados diretamente
        public decimal ValorTotal => Itens?.Sum(i => i.Subtotal) ?? 0;

        // Propriedade de navegação para o Usuário (se você tiver um modelo User)
        // [ForeignKey("UsuarioId")]
        // public virtual User Usuario { get; set; }
    }
} 