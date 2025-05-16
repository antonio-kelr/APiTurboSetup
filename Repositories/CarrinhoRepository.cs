using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;

namespace APiTurboSetup.Repositories
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Carrinho> ObterCarrinhoAtivoPorUsuario(int userId)
        {
            return await _context.Carrinhos
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Ativo);
        }

        public async Task<Carrinho> CriarCarrinho(int userId)
        {
            var carrinho = new Carrinho
            {
                UserId = userId,
                DataCriacao = DateTime.UtcNow,
                Ativo = true,
                Total = 0
            };

            _context.Carrinhos.Add(carrinho);
            await _context.SaveChangesAsync();
            return carrinho;
        }

        public async Task<ItemCarrinho> AdicionarItem(Carrinho carrinho, int produtoId, int quantidade, decimal precoUnitario)
        {
            var item = new ItemCarrinho
            {
                CarrinhoId = carrinho.Id,
                ProdutoId = produtoId,
                Quantidade = quantidade,
                PrecoUnitario = precoUnitario,
                Subtotal = quantidade * precoUnitario
            };

            _context.ItensCarrinho.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<ItemCarrinho> AtualizarItem(ItemCarrinho item, int quantidade)
        {
            item.Quantidade += quantidade;
            item.Subtotal = item.Quantidade * item.PrecoUnitario;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Carrinho> AtualizarTotal(Carrinho carrinho)
        {
            carrinho.Total = carrinho.Itens.Sum(i => i.Subtotal);
            await _context.SaveChangesAsync();
            return carrinho;
        }
    }
} 