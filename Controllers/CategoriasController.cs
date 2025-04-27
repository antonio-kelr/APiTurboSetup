using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriasController(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }
        [HttpGet("status")]

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada.");
            }

            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> CreateCategoria(Categoria categoria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCategoria = await _categoriaRepository.GetByNomeAsync(categoria.Nome);
            if (existingCategoria != null)
                return Conflict("Já existe uma categoria com este nome.");

            var newCategoria = await _categoriaRepository.AddAsync(categoria);
            return CreatedAtAction(nameof(GetCategoria), new { id = newCategoria.Id }, newCategoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, Categoria categoria)
        {
            if (id != categoria.Id)
                return BadRequest("ID da categoria não corresponde.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCategoria = await _categoriaRepository.GetByIdAsync(id);
            if (existingCategoria == null)
                return NotFound("Categoria não encontrada.");

            var sameNameDifferentId = await _categoriaRepository.GetByNomeAsync(categoria.Nome);
            if (sameNameDifferentId != null && sameNameDifferentId.Id != id)
                return Conflict("Já existe outra categoria com este nome.");

            await _categoriaRepository.UpdateAsync(categoria);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var success = await _categoriaRepository.DeleteAsync(id);
            if (!success)
                return NotFound("Categoria não encontrada.");

            return NoContent();
        }
    }
}