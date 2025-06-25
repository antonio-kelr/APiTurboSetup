using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoController(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetAll()
        {
            var pedidos = await _pedidoRepository.GetAllAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetById(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
                return NotFound();
            return Ok(pedido);
        }

        [HttpGet("usuario/{userId}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetByUserId(int userId)
        {
            var pedidos = await _pedidoRepository.GetByUserIdAsync(userId);
            return Ok(pedidos);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Pedido pedido)
        {
            await _pedidoRepository.AddAsync(pedido);
            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Pedido pedido)
        {
            if (id != pedido.Id)
                return BadRequest();
            await _pedidoRepository.UpdateAsync(pedido);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _pedidoRepository.DeleteAsync(id);
            return NoContent();
        }
    }
} 