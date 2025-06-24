using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;

namespace APiTurboSetup.Repositories
{
    public class FavoritoRepository : IFavoritoRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoritoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Favorito> AdicionarFavorito(int userId, int produtoId)
        {
            var favorito = new Favorito
            {
                UserId = userId,
                ProdutoId = produtoId
            };

            await _context.Favoritos.AddAsync(favorito);
            await _context.SaveChangesAsync();
            return favorito;
        }

        public async Task<bool> RemoverFavorito(int userId, int produtoId)
        {
            var favorito = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProdutoId == produtoId);

            if (favorito == null)
                return false;

            _context.Favoritos.Remove(favorito);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Favorito>> ListarFavoritos(int userId)
        {
            return await _context.Favoritos
                .Where(f => f.UserId == userId)
                .Include(f => f.Produto)
                .Include(f => f.Produto.Imagens)
                .ToListAsync();
        }
    }
} 