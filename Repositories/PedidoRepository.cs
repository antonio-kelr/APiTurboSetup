using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APiTurboSetup.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                    .ThenInclude(prod => prod.Imagens)
                .Include(p => p.User)
                .Include(p => p.Endereco)
                .ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                    .ThenInclude(prod => prod.Imagens)
                .Include(p => p.User)
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pedido>> GetByUserIdAsync(int userId)
        {
            return await _context.Pedidos
                .Where(p => p.UserId == userId)
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                    .ThenInclude(prod => prod.Imagens)
                .Include(p => p.User)
                .Include(p => p.Endereco)
                .ToListAsync();
        }

        public async Task AddAsync(Pedido pedido)
        {
            if (pedido.Itens != null)
            {
                foreach (var item in pedido.Itens)
                {
                    item.Produto = null;
                }
            }
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }
        }
    }
} 