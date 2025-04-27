using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APiTurboSetup.Repositories
{
    public class ProdutoImagemRepository : BaseRepository<ProdutoImagem>, IProdutoImagemRepository
    {
        public ProdutoImagemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProdutoImagem>> GetByProdutoIdAsync(int produtoId)
        {
            return await _dbSet.Where(pi => pi.ProdutoId == produtoId)
                              .OrderBy(pi => pi.Ordem)
                              .ToListAsync();
        }

        // Sobrescrevendo o método para incluir a navegação para Produto
        public override async Task<IEnumerable<ProdutoImagem>> GetAllAsync()
        {
            return await _dbSet.Include(pi => pi.Produto)
                              .OrderBy(pi => pi.ProdutoId)
                              .ThenBy(pi => pi.Ordem)
                              .ToListAsync();
        }

        public override async Task<ProdutoImagem?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(pi => pi.Produto)
                              .FirstOrDefaultAsync(pi => pi.Id == id);
        }
    }
} 