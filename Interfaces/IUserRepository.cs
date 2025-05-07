using APiTurboSetup.Models;
using System.Threading.Tasks;

namespace APiTurboSetup.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailAndSenhaAsync(string email, string senha);
    }
} 