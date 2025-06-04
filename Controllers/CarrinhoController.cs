using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APiTurboSetup.Models;
using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models.DTOs;
using System.Security.Claims;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarrinhoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;

        public CarrinhoController(
            ApplicationDbContext context,
            ICarrinhoRepository carrinhoRepository,
            IProdutoRepository produtoRepository)
        {
            _context = context;
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
        }

        [HttpPost("add-produto")]
        public async Task<IActionResult> AdicionarProduto([FromBody] AdicionarProdutoCarrinhoDTO dto)
        {
            try
            {
                Console.WriteLine("=== INÍCIO DA REQUISIÇÃO ===");
                Console.WriteLine($"ProdutoId recebido: {dto.ProdutoId}");
                Console.WriteLine($"Quantidade recebida: {dto.Quantidade}");

                // Obtém o ID do usuário do token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Console.WriteLine("ERRO: Token não contém o ID do usuário");
                    return Unauthorized("Token inválido ou expirado");
                }

                var userId = int.Parse(userIdClaim.Value);
                Console.WriteLine($"UserId extraído do token: {userId}");

                // Busca ou cria o carrinho ativo
                var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoPorUsuario(userId);
                Console.WriteLine($"Carrinho encontrado: {(carrinho != null ? "Sim" : "Não")}");

                if (carrinho == null)
                {
                    Console.WriteLine("Criando novo carrinho...");
                    carrinho = await _carrinhoRepository.CriarCarrinho(userId);
                    Console.WriteLine($"Novo carrinho criado com ID: {carrinho.Id}");
                }

                // Verifica se o produto existe
                var produto = await _produtoRepository.GetByIdAsync(dto.ProdutoId);
                Console.WriteLine($"Produto encontrado: {(produto != null ? "Sim" : "Não")}");
                
                if (produto == null)
                {
                    Console.WriteLine($"ERRO: Produto com ID {dto.ProdutoId} não encontrado");
                    return NotFound("Produto não encontrado");
                }

                // Verifica se o produto já está no carrinho
                var itemExistente = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == dto.ProdutoId);
                Console.WriteLine($"Item já existe no carrinho: {(itemExistente != null ? "Sim" : "Não")}");

                if (itemExistente != null)
                {
                    Console.WriteLine("Atualizando quantidade do item existente...");
                    await _carrinhoRepository.AtualizarItem(itemExistente, dto.Quantidade);
                }
                else
                {
                    Console.WriteLine("Adicionando novo item ao carrinho...");
                    await _carrinhoRepository.AdicionarItem(carrinho, dto.ProdutoId, dto.Quantidade, produto.Preco);
                }

                // Atualiza o total do carrinho
                await _carrinhoRepository.AtualizarTotal(carrinho);
                Console.WriteLine($"Total do carrinho atualizado: {carrinho.Total}");

                Console.WriteLine("=== FIM DA REQUISIÇÃO ===");
                return Ok(new { message = "Produto adicionado ao carrinho com sucesso" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { message = "Erro ao adicionar produto ao carrinho", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterCarrinho()
        {
            try
            {
                Console.WriteLine("=== INÍCIO DA REQUISIÇÃO GET CARRINHO ===");
                
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Console.WriteLine("ERRO: Token não contém o ID do usuário");
                    return Unauthorized("Token inválido ou expirado");
                }

                var userId = int.Parse(userIdClaim.Value);
                Console.WriteLine($"UserId extraído do token: {userId}");

                var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoPorUsuario(userId);
                Console.WriteLine($"Carrinho encontrado: {(carrinho != null ? "Sim" : "Não")}");

                if (carrinho == null)
                    return NotFound("Carrinho não encontrado");

                Console.WriteLine("=== FIM DA REQUISIÇÃO GET CARRINHO ===");
                return Ok(carrinho);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { message = "Erro ao obter carrinho", error = ex.Message });
            }
        }

        [HttpDelete("remover-produto/{produtoId}")]
        public async Task<IActionResult> RemoverProduto(int produtoId)
        {
            try
            {
                Console.WriteLine("=== INÍCIO DA REQUISIÇÃO REMOVER PRODUTO ===");
                Console.WriteLine($"ProdutoId recebido: {produtoId}");
                
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Console.WriteLine("ERRO: Token não contém o ID do usuário");
                    return Unauthorized("Token inválido ou expirado");
                }

                var userId = int.Parse(userIdClaim.Value);
                Console.WriteLine($"UserId extraído do token: {userId}");

                var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoPorUsuario(userId);
                Console.WriteLine($"Carrinho encontrado: {(carrinho != null ? "Sim" : "Não")}");

                if (carrinho == null)
                {
                    Console.WriteLine("Carrinho não encontrado para o usuário");
                    return NotFound("Carrinho não encontrado");
                }

                Console.WriteLine($"Carrinho ID: {carrinho.Id}");
                Console.WriteLine($"Total de itens no carrinho: {carrinho.Itens?.Count ?? 0}");
                
                // Verifica se o produto existe no carrinho antes de tentar remover
                var itemExiste = carrinho.Itens?.Any(i => i.ProdutoId == produtoId) ?? false;
                Console.WriteLine($"Produto {produtoId} existe no carrinho: {itemExiste}");

                if (!itemExiste)
                {
                    Console.WriteLine($"Produto {produtoId} não encontrado no carrinho");
                    return NotFound("Produto não encontrado no carrinho");
                }

                var sucesso = await _carrinhoRepository.RemoverItem(carrinho.Id, produtoId);
                if (!sucesso)
                {
                    Console.WriteLine($"Falha ao remover produto {produtoId} do carrinho");
                    return NotFound("Produto não encontrado no carrinho");
                }

                // Verifica se o produto foi realmente removido
                var carrinhoAtualizado = await _carrinhoRepository.ObterCarrinhoAtivoPorUsuario(userId);
                var produtoAindaExiste = carrinhoAtualizado?.Itens?.Any(i => i.ProdutoId == produtoId) ?? false;
                Console.WriteLine($"Produto {produtoId} ainda existe no carrinho após remoção: {produtoAindaExiste}");

                Console.WriteLine("=== FIM DA REQUISIÇÃO REMOVER PRODUTO ===");
                return Ok(new { 
                    message = "Produto removido do carrinho com sucesso",
                    carrinhoId = carrinho.Id,
                    produtoId = produtoId,
                    totalAtual = carrinhoAtualizado?.Total ?? 0
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { message = "Erro ao remover produto do carrinho", error = ex.Message });
            }
        }
    }
} 