using APiTurboSetup.Models;
using APiTurboSetup.Interfaces;

namespace APiTurboSetup.Repositories
{
    public interface UsuarioRepositry : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }
} 