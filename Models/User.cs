using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF inválido")]
        public required string Cpf { get; set; }

        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public required DateOnly DataNascimento { get; set; }

        public required string Genero { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; }
        public string? Role { get; set; } // Para controle de permissões (ex: "admin", "user")

        // Propriedade de navegação para os endereços
        public virtual ICollection<Endereco>? Enderecos { get; set; }
    }
} 