using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using APiTurboSetup.Models;
using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace APiTurboSetup.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(TokenService tokenService, ApplicationDbContext context, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _context = context;
            _configuration = configuration;

            if (FirebaseApp.DefaultInstance == null)
            {
                var credentialPath = "authentication-apiturbo-firebase.json";
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });
            }
        }

        public async Task<(bool success, string message, string? token, int? userId)> ValidateGoogleToken(string firebaseIdToken)
        {
            try
            {
                Console.WriteLine("Validando token Firebase");

                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseIdToken);
                string email = decodedToken.Claims["email"]?.ToString() ?? "";
                string name = decodedToken.Claims["name"]?.ToString() ?? "Usuário";

                Console.WriteLine($"Token válido. Email: {email}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    Console.WriteLine("Criando novo usuário");

                    user = new User
                    {
                        Nome = name,
                        Email = email,
                        Senha = "",
                        Cpf = "00000000000",
                        DataNascimento = DateOnly.FromDateTime(DateTime.UtcNow),
                        Genero = "Não informado",
                        Role = "user",
                        Ativo = true
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                var token = _tokenService.GenerateToken(user);
                return (true, "Autenticação bem-sucedida", token, user.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao validar token Firebase: {ex.Message}");
                return (false, $"Erro ao validar token Firebase: {ex.Message}", null, null);
            }
        }
    }
}
