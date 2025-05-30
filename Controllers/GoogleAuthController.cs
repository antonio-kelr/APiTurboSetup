using Microsoft.AspNetCore.Mvc;
using APiTurboSetup.Services;
using APiTurboSetup.Models;
using APiTurboSetup.Interfaces;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;

        public GoogleAuthController(IGoogleAuthService googleAuthService)
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

            var (success, message, token, userId) = await _googleAuthService.ValidateGoogleToken(model.token);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { token, message, userId });
        }
    }
} 