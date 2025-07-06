using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Repositories
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Categoria?> GetByNomeAsync(string nome)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Nome.ToLower() == nome.ToLower());
        }

        public override async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _dbSet.Include(c => c.Produtos).ThenInclude(p => p.Imagens).ToListAsync();
        }

        public override async Task<Categoria?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(c => c.Produtos).ThenInclude(p => p.Imagens).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}