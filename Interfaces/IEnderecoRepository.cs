using APiTurboSetup.Models;

namespace APiTurboSetup.Interfaces
{
    public interface IEnderecoRepository
    {
        Task<IEnumerable<Endereco>> GetAllAsync();
        Task<Endereco?> GetByIdAsync(int id);
        Task<IEnumerable<Endereco>> GetByUserIdAsync(int userId);
        Task<Endereco> AddAsync(Endereco endereco);
        Task<Endereco> UpdateAsync(Endereco endereco);
        Task<bool> DeleteAsync(int id);
        Task<bool> ValidarLimiteEnderecos(int userId);
        Task<int> CountByUserIdAsync(int userId);
    }
} 