using APiTurboSetup.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Interfaces
{
    public interface IProdutoImagemRepository : IBaseRepository<ProdutoImagem>
    {
        Task<IEnumerable<ProdutoImagem>> GetByProdutoIdAsync(int produtoId);
    }
} 