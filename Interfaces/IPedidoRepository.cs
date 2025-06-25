using APiTurboSetup.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Interfaces
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(int id);
        Task<IEnumerable<Pedido>> GetByUserIdAsync(int userId);
        Task AddAsync(Pedido pedido);
        Task UpdateAsync(Pedido pedido);
        Task DeleteAsync(int id);
    }
} 