using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Models.DTOs
{
    public class AdicionarProdutoCarrinhoDTO
    {
        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int Quantidade { get; set; }
    }
} 