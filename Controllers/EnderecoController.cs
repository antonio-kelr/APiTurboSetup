using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APiTurboSetup.Models;
using APiTurboSetup.Validations;
using APiTurboSetup.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnderecoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EnderecoValidation _enderecoValidation;

        public EnderecoController(ApplicationDbContext context)
        {
            _context = context;
            _enderecoValidation = new EnderecoValidation(context);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Endereco>>> GetEnderecos()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var enderecos = await _context.Enderecos
                .Where(e => e.UserId == userId && e.Ativo)
                .ToListAsync();

            return Ok(enderecos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Endereco>> GetEndereco(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var endereco = await _context.Enderecos
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.Ativo);

            if (endereco == null)
            {
                return NotFound("Endereço não encontrado");
            }

            return Ok(endereco);
        }

        [HttpPost]
        public async Task<ActionResult<Endereco>> CreateEndereco(Endereco endereco)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            endereco.UserId = userId;

            // Validar limite de endereços
            if (!await _enderecoValidation.ValidarLimiteEnderecos(userId))
            {
                return BadRequest("Limite máximo de 2 endereços atingido");
            }

            // Se for o primeiro endereço, definir como principal
            var enderecosExistentes = await _context.Enderecos
                .Where(e => e.UserId == userId && e.Ativo)
                .CountAsync();

            if (enderecosExistentes == 0)
            {
                endereco.TipoEndereco = "Principal";
            }
            else if (string.IsNullOrEmpty(endereco.TipoEndereco))
            {
                endereco.TipoEndereco = "Secundário";
            }

            _context.Enderecos.Add(endereco);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEndereco), new { id = endereco.Id }, endereco);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Endereco>> UpdateEndereco(int id, Endereco endereco)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var enderecoExistente = await _context.Enderecos
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.Ativo);

            if (enderecoExistente == null)
            {
                return NotFound("Endereço não encontrado");
            }

            // Atualizar propriedades
            enderecoExistente.Cep = endereco.Cep;
            enderecoExistente.Logradouro = endereco.Logradouro;
            enderecoExistente.Numero = endereco.Numero;
            enderecoExistente.Complemento = endereco.Complemento;
            enderecoExistente.Referencia = endereco.Referencia;
            enderecoExistente.Bairro = endereco.Bairro;
            enderecoExistente.Cidade = endereco.Cidade;
            enderecoExistente.Estado = endereco.Estado;
            enderecoExistente.TipoEndereco = endereco.TipoEndereco;
            enderecoExistente.Identificacao = endereco.Identificacao;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(enderecoExistente);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnderecoExists(id))
                {
                    return NotFound("Endereço não encontrado");
                }
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var endereco = await _context.Enderecos
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.Ativo);

            if (endereco == null)
            {
                return NotFound("Endereço não encontrado");
            }

            // Soft delete
            endereco.Ativo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/tipo")]
        public async Task<IActionResult> AlterarTipoEndereco(int id, [FromBody] string tipoEndereco)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var endereco = await _context.Enderecos
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.Ativo);

            if (endereco == null)
            {
                return NotFound("Endereço não encontrado");
            }

            if (tipoEndereco != "Principal" && tipoEndereco != "Secundário")
            {
                return BadRequest("Tipo de endereço inválido. Use 'Principal' ou 'Secundário'");
            }

            // Se estiver alterando para Principal, atualizar o outro endereço para Secundário
            if (tipoEndereco == "Principal")
            {
                var outroEndereco = await _context.Enderecos
                    .FirstOrDefaultAsync(e => e.UserId == userId && e.Id != id && e.Ativo);

                if (outroEndereco != null)
                {
                    outroEndereco.TipoEndereco = "Secundário";
                }
            }

            endereco.TipoEndereco = tipoEndereco;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnderecoExists(int id)
        {
            return _context.Enderecos.Any(e => e.Id == id);
        }
    }
} 