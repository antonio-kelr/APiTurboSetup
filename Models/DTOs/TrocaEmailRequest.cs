using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Models.DTOs
{
    public class TrocaEmailRequest
    {
        [Required(ErrorMessage = "O email atual é obrigatório")]
        [EmailAddress(ErrorMessage = "Email atual inválido")]
        public required string EmailAtual { get; set; }

        [Required(ErrorMessage = "O novo email é obrigatório")]
        [EmailAddress(ErrorMessage = "Novo email inválido")]
        public required string NovoEmail { get; set; }

        [Required(ErrorMessage = "A senha atual é obrigatória")]
        public required string SenhaAtual { get; set; }
    }

    public class ConfirmarTrocaEmailRequest
    {
        [Required(ErrorMessage = "O código de verificação é obrigatório")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "O código deve ter 6 dígitos")]
        public required string Codigo { get; set; }
    }
} 