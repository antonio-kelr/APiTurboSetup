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

        public async Task<bool> RemoverItem(int carrinhoId, int produtoId)
        {
            Console.WriteLine($"=== INÍCIO REMOÇÃO DE ITEM ===");
            Console.WriteLine($"Buscando item - CarrinhoId: {carrinhoId}, ProdutoId: {produtoId}");

            var item = await _context.ItensCarrinho
                .FirstOrDefaultAsync(i => i.CarrinhoId == carrinhoId && i.ProdutoId == produtoId);

            if (item == null)
            {
                Console.WriteLine($"Item não encontrado - CarrinhoId: {carrinhoId}, ProdutoId: {produtoId}");
                return false;
            }

            Console.WriteLine($"Item encontrado - Id: {item.Id}, Quantidade: {item.Quantidade}, Subtotal: {item.Subtotal}");
            Console.WriteLine("Removendo item do carrinho...");

            _context.ItensCarrinho.Remove(item);
            await _context.SaveChangesAsync();
            Console.WriteLine("Item removido com sucesso do banco de dados");

            // Atualiza o total do carrinho
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == carrinhoId);

            if (carrinho != null)
            {
                Console.WriteLine($"Atualizando total do carrinho - Total atual: {carrinho.Total}");
                await AtualizarTotal(carrinho);
                Console.WriteLine($"Novo total do carrinho: {carrinho.Total}");
            }
            else
            {
                Console.WriteLine($"Carrinho não encontrado para atualização - CarrinhoId: {carrinhoId}");
            }

            Console.WriteLine("=== FIM REMOÇÃO DE ITEM ===");
            return true;
        }
    }
} 