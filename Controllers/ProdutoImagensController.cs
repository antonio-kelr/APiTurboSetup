using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoImagensController : ControllerBase
    {
        private readonly IProdutoImagemRepository _produtoImagemRepository;
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoImagensController(IProdutoImagemRepository produtoImagemRepository, IProdutoRepository produtoRepository)
        {
            _produtoImagemRepository = produtoImagemRepository;
            _produtoRepository = produtoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoImagem>>> GetProdutoImagens()
        {
            var imagens = await _produtoImagemRepository.GetAllAsync();
            return Ok(imagens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoImagem>> GetProdutoImagem(int id)
        {
            var imagem = await _produtoImagemRepository.GetByIdAsync(id);

            if (imagem == null)
            {
                return NotFound("Imagem não encontrada.");
            }

            return Ok(imagem);
        }

        [HttpGet("produto/{produtoId}")]
        public async Task<ActionResult<IEnumerable<ProdutoImagem>>> GetImagensByProduto(int produtoId)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null)
                return NotFound("Produto não encontrado.");

            var imagens = await _produtoImagemRepository.GetByProdutoIdAsync(produtoId);
            return Ok(imagens);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoImagem>> CreateProdutoImagem(ProdutoImagem produtoImagem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produto = await _produtoRepository.GetByIdAsync(produtoImagem.ProdutoId);
            if (produto == null)
                return BadRequest("Produto inválido.");

            var newImagem = await _produtoImagemRepository.AddAsync(produtoImagem);
            return CreatedAtAction(nameof(GetProdutoImagem), new { id = newImagem.Id }, newImagem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProdutoImagem(int id, ProdutoImagem produtoImagem)
        {
            if (id != produtoImagem.Id)
                return BadRequest("ID da imagem não corresponde.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingImagem = await _produtoImagemRepository.GetByIdAsync(id);
            if (existingImagem == null)
                return NotFound("Imagem não encontrada.");

            var produto = await _produtoRepository.GetByIdAsync(produtoImagem.ProdutoId);
            if (produto == null)
                return BadRequest("Produto inválido.");

            await _produtoImagemRepository.UpdateAsync(produtoImagem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProdutoImagem(int id)
        {
            var success = await _produtoImagemRepository.DeleteAsync(id);
            if (!success)
                return NotFound("Imagem não encontrada.");

            return NoContent();
        }
    }
} 