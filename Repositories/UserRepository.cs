using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;

namespace APiTurboSetup.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailAndSenhaAsync(string email, string senha)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            // Se a senha estiver vazia, é um usuário do Google que ainda não definiu senha
            if (string.IsNullOrEmpty(user.Senha))
                return null;

            // Verifica se a senha fornecida corresponde à senha criptografada
            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senha, user.Senha);
            if (!senhaCorreta)
                return null;

            return user;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Enderecos)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return false;

            // Excluir carrinhos do usuário
            var carrinhos = await _context.Carrinhos
                .Where(c => c.UserId == id)
                .ToListAsync();

            foreach (var carrinho in carrinhos)
            {
                // Excluir itens do carrinho
                var itens = await _context.ItensCarrinho
                    .Where(i => i.CarrinhoId == carrinho.Id)
                    .ToListAsync();
                _context.ItensCarrinho.RemoveRange(itens);

                // Excluir o carrinho
                _context.Carrinhos.Remove(carrinho);
            }

            // Excluir endereços do usuário
            if (user.Enderecos != null)
            {
                _context.Enderecos.RemoveRange(user.Enderecos);
            }

            // Excluir o usuário
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}