using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;

namespace APiTurboSetup.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly ApplicationDbContext _context;

        public EnderecoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Endereco>> GetAllAsync()
        {
            return await _context.Enderecos
                .Where(e => e.Ativo)
                .ToListAsync();
        }

        public async Task<Endereco?> GetByIdAsync(int id)
        {
            return await _context.Enderecos
                .FirstOrDefaultAsync(e => e.Id == id && e.Ativo);
        }

        public async Task<IEnumerable<Endereco>> GetByUserIdAsync(int userId)
        {
            return await _context.Enderecos
                .Where(e => e.UserId == userId && e.Ativo)
                .ToListAsync();
        }

        public async Task<Endereco> AddAsync(Endereco endereco)
        {
            await _context.Enderecos.AddAsync(endereco);
            await _context.SaveChangesAsync();
            return endereco;
        }

        public async Task<Endereco> UpdateAsync(Endereco endereco)
        {
            _context.ChangeTracker.Clear();
            _context.Enderecos.Update(endereco);
            await _context.SaveChangesAsync();
            return endereco;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var endereco = await GetByIdAsync(id);
            if (endereco == null)
                return false;

            // Soft delete
            endereco.Ativo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidarLimiteEnderecos(int userId)
        {
            var count = await _context.Enderecos
                .Where(e => e.UserId == userId && e.Ativo)
                .CountAsync();
            
            return count < 2; // Máximo 2 endereços por usuário
        }

        public async Task<int> CountByUserIdAsync(int userId)
        {
            return await _context.Enderecos
                .Where(e => e.UserId == userId && e.Ativo)
                .CountAsync();
        }
    }
} 