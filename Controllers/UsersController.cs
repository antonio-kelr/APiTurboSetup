using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            // Verificar se já existe usuário com este email
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                return Conflict("Já existe um usuário com este email.");

            // Criptografa a senha antes de salvar
            user.Senha = BCrypt.Net.BCrypt.HashPassword(user.Senha);
            user.Ativo = true;
            user.Role = "user"; // Role padrão

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
        public async Task<ActionResult<User>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            // Verifica se a senha está correta usando BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Senha, user.Senha))
            {
                return Unauthorized("Email ou senha inválidos");
            }

            if (!user.Ativo)
            {
                return Unauthorized("Usuário inativo");
            }

            return Ok(user);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
} 