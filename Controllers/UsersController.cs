using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using APiTurboSetup.Validations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BCrypt.Net;
// Adicionando para IConfiguration, se necessário, mas TokenService já o usa.
// using Microsoft.Extensions.Configuration;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService; // Injetando TokenService
        // private readonly IConfiguration _configuration; // Não é mais necessário aqui diretamente
        private readonly IGoogleAuthService _googleAuthService;

        public UsersController(IUserRepository userRepository, TokenService tokenService /*, IConfiguration configuration*/, IGoogleAuthService googleAuthService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            // _configuration = configuration; 
            _googleAuthService = googleAuthService;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar CPF
            if (!CpfValidation.IsValid(user.Cpf))
                return BadRequest("CPF inválido");

            // Verificar se já existe usuário com este email
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                return Conflict("Já existe um usuário com este email.");

            // Criptografa a senha antes de salvar
            user.Senha = BCrypt.Net.BCrypt.HashPassword(user.Senha);
            user.Ativo = true;
            
            // Define role baseado no nome do usuário ou mantém o que foi enviado
            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = user.Nome.ToLower().Contains("admin") ? "admin" : "user";
            }

            var newUser = await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound("Usuário não encontrado.");

            // Verificar se o novo email já está em uso por outro usuário
            var userWithSameEmail = await _userRepository.GetByEmailAsync(user.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != id)
                return Conflict("Já existe outro usuário com este email.");

            // Se a senha foi alterada, criptografa a nova senha
            if (!string.IsNullOrEmpty(user.Senha) && user.Senha != existingUser.Senha)
            {
                user.Senha = BCrypt.Net.BCrypt.HashPassword(user.Senha);
            }
            else
            {
                // Mantém a senha existente se não foi alterada
                user.Senha = existingUser.Senha;
            }

            // Atualizar propriedades
            existingUser.Nome = user.Nome;
            existingUser.Email = user.Email;
            existingUser.Telefone = user.Telefone;
            existingUser.Ativo = user.Ativo;
            existingUser.Role = user.Role;
            existingUser.DataNascimento = user.DataNascimento;
            existingUser.Genero = user.Genero;
            existingUser.Cpf = user.Cpf;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userRepository.DeleteAsync(id);
            if (!success)
                return NotFound("Usuário não encontrado.");

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] GoogleAuthModel model)
        {
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