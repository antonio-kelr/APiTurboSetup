using System;
using System.Collections.Generic;

namespace APiTurboSetup.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; }
        public string? Role { get; set; } // Para controle de permissões (ex: "admin", "user")
    }
} 