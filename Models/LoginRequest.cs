using System;
using System.Collections.Generic;

namespace APiTurboSetup.Models
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }
}