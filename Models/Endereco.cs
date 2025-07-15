using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace APiTurboSetup.Models
{
    public class Endereco
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve ter 8 dígitos")]
        public required string Cep { get; set; }

        [Required(ErrorMessage = "O logradouro é obrigatório")]
        [StringLength(200, ErrorMessage = "O logradouro deve ter no máximo 200 caracteres")]
        public required string Logradouro { get; set; }

        [Required(ErrorMessage = "O número é obrigatório")]
        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public required string Numero { get; set; }

        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string? Complemento { get; set; }

        [StringLength(100, ErrorMessage = "A referência deve ter no máximo 100 caracteres")]
        public string? Referencia { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório")]
        [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres")]
        public required string Bairro { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória")]
        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
        public required string Cidade { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter 2 caracteres")]
        public required string Estado { get; set; }

        public string? TipoEndereco { get; set; } // "Principal" ou "Secundário"

        [StringLength(50, ErrorMessage = "A identificação deve ter no máximo 50 caracteres")]
        public string? Identificacao { get; set; } // Ex: "Casa", "Trabalho", etc.o

        public bool Ativo { get; set; } = true;

        // Chave estrangeira para o usuário
        [Required]
        public int UserId { get; set; }

        // Propriedade de navegação para o usuário
        
        [ForeignKey("UserId")]
        [JsonIgnore] 
        
        public virtual User? User { get; set; }
    }
}