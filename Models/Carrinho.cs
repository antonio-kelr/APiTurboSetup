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
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        public DateTime? DataFinalizacao { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public bool Ativo { get; set; }

        // Relacionamentos
        public virtual User User { get; set; }
        public virtual ICollection<ItemCarrinho> Itens { get; set; }
    }
} 