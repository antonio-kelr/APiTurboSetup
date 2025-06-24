using APiTurboSetup.Models;

namespace APiTurboSetup.Interfaces
{
    public interface IFavoritoRepository
    {
        Task<Favorito> AdicionarFavorito(int userId, int produtoId);
        Task<bool> RemoverFavorito(int userId, int produtoId);
        Task<IEnumerable<Favorito>> ListarFavoritos(int userId);
    }
} 