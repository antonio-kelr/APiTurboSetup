using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;
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
    }
} 