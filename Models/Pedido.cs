using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APiTurboSetup.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime DataPedido { get; set; }

        [Required]
        public string Status { get; set; } = "Pendente";

        public int? EnderecoId { get; set; }

        // Propriedades de navegação
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("EnderecoId")]
        public virtual Endereco? Endereco { get; set; }

        public virtual ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }
} 