using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Models.DTOs
{
    public class TrocaSenhaRequest
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        public required string SenhaAtual { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public required string NovaSenha { get; set; }

        [Required(ErrorMessage = "A confirmação da nova senha é obrigatória")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem")]
        public required string ConfirmarNovaSenha { get; set; }
    }
} 