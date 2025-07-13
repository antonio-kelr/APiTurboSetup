using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using APiTurboSetup.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public ProdutosController(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository)
        {
            _produtoRepository = produtoRepository;
            _categoriaRepository = categoriaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            var produtos = await _produtoRepository.GetAllAsync();
            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            return Ok(produto);
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<Produto>> GetProdutoBySlug(string slug)
        {
            var produto = await _produtoRepository.GetBySlugAsync(slug);

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            return Ok(produto);
        }


        [HttpGet("categoria/{nomeCategoria}")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosByNomeCategoria(string nomeCategoria)
        {
            var categoria = await _categoriaRepository.GetByNomeAsync(nomeCategoria);
            if (categoria == null)
                return NotFound($"Categoria '{nomeCategoria}' não encontrada.");

            var produtos = await _produtoRepository.GetByCategoriaIdAsync(categoria.Id);
            return Ok(produtos);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Produto>>> SearchProdutos([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("A consulta de busca não pode ser vazia.");
            }

            var produtos = await _produtoRepository.SearchAsync(query);

            if (produtos == null || !produtos.Any())
            {
                return NotFound("Nenhum produto encontrado para a consulta fornecida.");
            }

            return Ok(produtos);
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> CreateProduto(Produto produto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Gerar slug automaticamente se não for fornecido
            if (string.IsNullOrEmpty(produto.Slug))
            {
                produto.Slug = SlugUtils.GenerateSlug(produto.Nome);
            }
            // Validar formato do slug se fornecido
            else if (!SlugUtils.IsValidSlug(produto.Slug))
            {
                return BadRequest("O slug deve conter apenas letras minúsculas, números e hífens.");
            }

            var categoria = await _categoriaRepository.GetByIdAsync(produto.CategoriaId);
            if (categoria == null)
                return BadRequest("Categoria inválida.");

            var existingProdutoByNome = await _produtoRepository.GetByNomeAsync(produto.Nome);
            if (existingProdutoByNome != null)
                return Conflict("Já existe um produto com este nome.");

            // Verificar se existe produto com o mesmo slug
            var existingProdutoBySlug = await _produtoRepository.GetBySlugAsync(produto.Slug);
            if (existingProdutoBySlug != null)
                return Conflict("Já existe um produto com este slug.");

            var newProduto = await _produtoRepository.AddAsync(produto);
            return CreatedAtAction(nameof(GetProduto), new { id = newProduto.Id }, newProduto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(int id, Produto produto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduto = await _produtoRepository.GetEntityByIdAsync(id);
            if (existingProduto == null)
                return NotFound("Produto não encontrado.");

            // Gerar slug automaticamente se não for fornecido
            if (string.IsNullOrEmpty(produto.Slug))
            {
                produto.Slug = SlugUtils.GenerateSlug(produto.Nome);
            }
            // Validar formato do slug se fornecido
            else if (!SlugUtils.IsValidSlug(produto.Slug))
            {
                return BadRequest("O slug deve conter apenas letras minúsculas, números e hífens.");
            }

            var categoria = await _categoriaRepository.GetByIdAsync(produto.CategoriaId);
            if (categoria == null)
                return BadRequest("Categoria inválida.");

            var sameNameDifferentId = await _produtoRepository.GetByNomeAsync(produto.Nome);
            if (sameNameDifferentId != null && sameNameDifferentId.Id != id)
                return Conflict("Já existe outro produto com este nome.");

            // Verificar se existe produto com o mesmo slug
            var sameSlugDifferentId = await _produtoRepository.GetBySlugAsync(produto.Slug);
            if (sameSlugDifferentId != null && sameSlugDifferentId.Id != id)
                return Conflict("Já existe outro produto com este slug.");

            // Atualizar as propriedades do produto existente
            existingProduto.Nome = produto.Nome;
            existingProduto.Descricao = produto.Descricao;

            // Se o preço for alterado, salvar o preço antigo
            if (existingProduto.Preco != produto.Preco)
            {
                existingProduto.PrecoAntigo = existingProduto.Preco;
                existingProduto.Preco = produto.Preco;
            }

            existingProduto.CategoriaId = produto.CategoriaId;
            existingProduto.Slug = produto.Slug;

            var updatedProduto = await _produtoRepository.UpdateAsync(existingProduto);
            return Ok(updatedProduto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var success = await _produtoRepository.DeleteAsync(id);
            if (!success)
                return NotFound("Produto não encontrado.");

            return NoContent();
        }
    }
}