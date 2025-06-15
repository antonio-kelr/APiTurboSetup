using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using APiTurboSetup.Validations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Security.Claims;
using APiTurboSetup.Models.DTOs;
using APiTurboSetup.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        private readonly TrocaEmailRepository _trocaEmailRepository;

        public UsersController(
            IUserRepository userRepository, 
            TokenService tokenService /*, IConfiguration configuration*/, 
            IGoogleAuthService googleAuthService,
            TrocaEmailRepository trocaEmailRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            // _configuration = configuration; 
            _googleAuthService = googleAuthService;
            _trocaEmailRepository = trocaEmailRepository;
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
            if (!string.IsNullOrEmpty(user.Senha))
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
            existingUser.Senha = user.Senha;
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

        [HttpPost("login-email")]
        public async Task<IActionResult> LoginEmail([FromBody] Models.LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.GetByEmailAndSenhaAsync(request.Email, request.Senha);
            if (user == null)
                return BadRequest("Email ou senha incorretos");

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, userId = user.Id });
        }

        [HttpPost("solicitar-troca-email")]
        [Authorize]
        public async Task<IActionResult> SolicitarTrocaEmail([FromBody] TrocaEmailRequest request)
        {
            Console.WriteLine($"Request recebido: EmailAtual={request.EmailAtual}, NovoEmail={request.NovoEmail}, SenhaAtual=***");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }
                return BadRequest(ModelState);
            }

            // Se o email atual não foi fornecido no request, pegar do token
            if (string.IsNullOrEmpty(request.EmailAtual))
            {
                var emailAtual = User.FindFirst(ClaimTypes.Email)?.Value;
                Console.WriteLine($"Email do token: {emailAtual}");
                
                if (string.IsNullOrEmpty(emailAtual))
                    return BadRequest(new { mensagem = "Email não encontrado no token" });

                request.EmailAtual = emailAtual;
            }

            if (string.IsNullOrEmpty(request.SenhaAtual))
            {
                return BadRequest(new { mensagem = "A senha atual é obrigatória" });
            }

            // Buscar o usuário e verificar a senha atual (usando a mesma lógica do login)
            var usuario = await _userRepository.GetByEmailAndSenhaAsync(request.EmailAtual, request.SenhaAtual);
            if (usuario == null)
                return BadRequest(new { mensagem = "Senha atual incorreta" });

            Console.WriteLine($"Request final: EmailAtual={request.EmailAtual}, NovoEmail={request.NovoEmail}, SenhaAtual=***");

            var (sucesso, mensagem) = await _trocaEmailRepository.SolicitarTrocaEmailAsync(request);
            Console.WriteLine($"Resultado: Sucesso={sucesso}, Mensagem={mensagem}");

            if (!sucesso)
                return BadRequest(new { mensagem });

            return Ok(new { mensagem });
        }

        [HttpPost("confirmar-troca-email")]
        [Authorize]
        public async Task<IActionResult> ConfirmarTrocaEmail([FromBody] ConfirmarTrocaEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailAtual = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailAtual))
                return BadRequest(new { mensagem = "Email não encontrado no token" });

            var (sucesso, mensagem) = await _trocaEmailRepository.ConfirmarTrocaEmailAsync(emailAtual, request.Codigo);

            if (!sucesso)
                return BadRequest(new { mensagem });

            return Ok(new { mensagem });
        }

        [HttpPost("trocar-senha")]
        [Authorize]
        public async Task<IActionResult> TrocarSenha([FromBody] TrocaSenhaRequest request)
        {
            Console.WriteLine("Iniciando troca de senha...");
            Console.WriteLine($"Request recebido: SenhaAtual=***, NovaSenha=***, ConfirmarNovaSenha=***");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }
                return BadRequest(ModelState);
            }

            // Pegar o email do usuário do token
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine($"Email do token: {email}");

            if (string.IsNullOrEmpty(email))
                return BadRequest(new { mensagem = "Email não encontrado no token" });

            // Buscar o usuário e verificar a senha atual (usando a mesma lógica do login)
            var usuario = await _userRepository.GetByEmailAndSenhaAsync(email, request.SenhaAtual);
            Console.WriteLine($"Usuário encontrado: {usuario != null}");
            if (usuario != null)
            {
                Console.WriteLine($"Email do usuário: {usuario.Email}");
                Console.WriteLine($"Senha atual está correta: {BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.Senha)}");
            }

            if (usuario == null)
                return BadRequest(new { mensagem = "Senha atual incorreta" });

            // Verificar se a nova senha é diferente da atual
            if (request.SenhaAtual == request.NovaSenha)
                return BadRequest(new { mensagem = "A nova senha deve ser diferente da senha atual" });

            // Criptografar a nova senha
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);

            // Atualizar o usuário
            await _userRepository.UpdateAsync(usuario);

            return Ok(new { mensagem = "Senha alterada com sucesso" });
        }
    }
}