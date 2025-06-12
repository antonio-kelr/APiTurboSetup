using System;
using System.Linq;
using System.Threading.Tasks;
using APiTurboSetup.Models;
using APiTurboSetup.Data;
using Microsoft.EntityFrameworkCore;

namespace APiTurboSetup.Validations
{
    public class EnderecoValidation
    {
        private readonly ApplicationDbContext _context;

        public EnderecoValidation(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidarLimiteEnderecos(int userId)
        {
            var quantidadeEnderecos = await _context.Enderecos
                .Where(e => e.UserId == userId && e.Ativo)
                .CountAsync();

            return quantidadeEnderecos < 2;
        }
    }
} 