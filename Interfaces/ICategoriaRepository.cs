using APiTurboSetup.Models;
using System.Threading.Tasks;

namespace APiTurboSetup.Interfaces
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<Categoria?> GetByNomeAsync(string nome);
    }
} 