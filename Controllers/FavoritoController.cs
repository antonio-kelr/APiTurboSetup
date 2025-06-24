using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritoController : ControllerBase
    {
        private readonly IFavoritoRepository _favoritoRepository;

        public FavoritoController(IFavoritoRepository favoritoRepository)
        {
            _favoritoRepository = favoritoRepository;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Token inválido ou expirado");
            }
            return int.Parse(userIdClaim.Value);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarFavorito([FromBody] AdicionarFavoritoRequest request)
        {
            try
            {
                var userId = GetUserId();
                var favorito = await _favoritoRepository.AdicionarFavorito(userId, request.ProdutoId);
                return Ok(new { message = "Produto adicionado aos favoritos com sucesso" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Favoritos_UserId_ProdutoId") == true)
            {
                return BadRequest(new { message = "Produto já está nos favoritos" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao adicionar produto aos favoritos", error = ex.Message });
            }
        }

        [HttpDelete("{produtoId}")]
        public async Task<IActionResult> RemoverFavorito(int produtoId)
        {
            try
            {
                var userId = GetUserId();
                var removido = await _favoritoRepository.RemoverFavorito(userId, produtoId);
                
                if (!removido)
                    return NotFound(new { message = "Produto não encontrado nos favoritos" });

                return Ok(new { message = "Produto removido dos favoritos com sucesso" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao remover produto dos favoritos", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarFavoritos()
        {
            try
            {
                var userId = GetUserId();
                var favoritos = await _favoritoRepository.ListarFavoritos(userId);
                return Ok(favoritos);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao listar favoritos", error = ex.Message });
            }
        }
    }

    public class AdicionarFavoritoRequest
    {
        public int ProdutoId { get; set; }
    }
} 