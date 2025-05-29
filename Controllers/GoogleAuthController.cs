using Microsoft.AspNetCore.Mvc;
using APiTurboSetup.Services;
using APiTurboSetup.Models;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly GoogleAuthService _googleAuthService;

        public GoogleAuthController(GoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] GoogleAuthModel model)
        {
            Console.WriteLine($"Token recebido: {model?.token}"); // Debug

            if (model == null || string.IsNullOrEmpty(model.token))
            {
                return BadRequest("Token não fornecido");
            }

            var (success, message, token) = await _googleAuthService.ValidateGoogleToken(model.token);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { token, message });
        }
    }
} 