using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APiTurboSetup.Models;
using APiTurboSetup.Interfaces;
using System.Security.Claims;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoRepository _enderecoRepository;

        public EnderecoController(IEnderecoRepository enderecoRepository)
        {
            _enderecoRepository = enderecoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Endereco>>> GetEnderecos()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var enderecos = await _enderecoRepository.GetByUserIdAsync(userId);

            return Ok(enderecos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Endereco>> GetEndereco(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var endereco = await _enderecoRepository.GetByIdAsync(id);

            if (endereco == null || endereco.UserId != userId)
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
            if (!await _enderecoRepository.ValidarLimiteEnderecos(userId))
            {
                return BadRequest("Limite máximo de 2 endereços atingido");
            }

            // Se for o primeiro endereço, definir como principal
            var enderecosExistentes = await _enderecoRepository.CountByUserIdAsync(userId);

            if (enderecosExistentes == 0)
            {
                endereco.TipoEndereco = "Principal";
            }
            else if (string.IsNullOrEmpty(endereco.TipoEndereco))
            {
                endereco.TipoEndereco = "Secundário";
            }

            var enderecoCriado = await _enderecoRepository.AddAsync(endereco);

            return CreatedAtAction(nameof(GetEndereco), new { id = enderecoCriado.Id }, enderecoCriado);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Endereco>> UpdateEndereco(int id, Endereco endereco)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var enderecoExistente = await _enderecoRepository.GetByIdAsync(id);

            if (enderecoExistente == null || enderecoExistente.UserId != userId)
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

            var enderecoAtualizado = await _enderecoRepository.UpdateAsync(enderecoExistente);
            return Ok(enderecoAtualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var endereco = await _enderecoRepository.GetByIdAsync(id);

            if (endereco == null || endereco.UserId != userId)
            {
                return NotFound("Endereço não encontrado");
            }

            await _enderecoRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/tipo")]
        public async Task<IActionResult> AlterarTipoEndereco(int id, [FromBody] string tipoEndereco)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var endereco = await _enderecoRepository.GetByIdAsync(id);

            if (endereco == null || endereco.UserId != userId)
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
                var enderecosUsuario = await _enderecoRepository.GetByUserIdAsync(userId);
                var outroEndereco = enderecosUsuario.FirstOrDefault(e => e.Id != id);

                if (outroEndereco != null)
                {
                    outroEndereco.TipoEndereco = "Secundário";
                    await _enderecoRepository.UpdateAsync(outroEndereco);
                }
            }

            endereco.TipoEndereco = tipoEndereco;
            await _enderecoRepository.UpdateAsync(endereco);

            return NoContent();
        }

        private bool EnderecoExists(int id)
        {
            return _enderecoRepository.GetByIdAsync(id).Result != null;
        }
    }
} 