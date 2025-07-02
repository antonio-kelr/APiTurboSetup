using APiTurboSetup.Models;

namespace APiTurboSetup.Interfaces
{
    public interface ICarrinhoRepository
    {
        Task<Carrinho> ObterCarrinhoAtivoPorUsuario(int userId);
        Task<Carrinho> CriarCarrinho(int userId);
        Task<ItemCarrinho> AdicionarItem(Carrinho carrinho, int produtoId, int quantidade, decimal precoUnitario);
        Task<ItemCarrinho> AtualizarItem(ItemCarrinho item, int quantidade);
        Task<Carrinho> AtualizarTotal(Carrinho carrinho);
        Task<bool> RemoverItem(int carrinhoId, int produtoId);
        Task<bool> LimparCarrinho(int carrinhoId);
    }
} 