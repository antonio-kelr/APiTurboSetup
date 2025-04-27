using APiTurboSetup.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Interfaces
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<IEnumerable<Produto>> GetByCategoriaIdAsync(int categoriaId);
        Task<Produto?> GetByNomeAsync(string nome);
        Task<Produto?> GetBySlugAsync(string slug);
    }
} 