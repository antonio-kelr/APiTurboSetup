using System.Security.Cryptography;
using System.Text;
using APiTurboSetup.Data;
using APiTurboSetup.Models;
using APiTurboSetup.Models.DTOs;
using APiTurboSetup.Services;
using Microsoft.EntityFrameworkCore;

namespace APiTurboSetup.Repositories
{
    public class TrocaEmailRepository : ITrocaEmailRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public TrocaEmailRepository(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<(bool Sucesso, string Mensagem)> SolicitarTrocaEmailAsync(TrocaEmailRequest request)
        {
            Console.WriteLine($"TrocaEmailRepository - Iniciando solicitação de troca de email");
            Console.WriteLine($"Email atual: {request.EmailAtual}");
            Console.WriteLine($"Novo email: {request.NovoEmail}");

            if (string.IsNullOrEmpty(request.EmailAtual))
            {
                Console.WriteLine("Email atual está vazio");
                return (false, "Email atual não pode ser vazio");
            }

            if (string.IsNullOrEmpty(request.NovoEmail))
            {
                Console.WriteLine("Novo email está vazio");
                return (false, "Novo email não pode ser vazio");
            }

            // Verificar se o usuário existe e a senha está correta
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.EmailAtual);

            if (usuario == null)
            {
                Console.WriteLine($"Usuário não encontrado com o email: {request.EmailAtual}");
                return (false, "Usuário não encontrado");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.Senha))
            {
                Console.WriteLine("Senha incorreta");
                return (false, "Senha incorreta");
            }

            // Verificar se o novo email já está em uso
            var emailEmUso = await _context.Users
                .AnyAsync(u => u.Email == request.NovoEmail);

            if (emailEmUso)
            {
                Console.WriteLine($"Novo email já está em uso: {request.NovoEmail}");
                return (false, "Este email já está em uso");
            }

            // Gerar código de verificação
            var codigo = GerarCodigoVerificacao();
            var expiracao = DateTime.UtcNow.AddMinutes(1);

            Console.WriteLine($"Código gerado: {codigo}");
            Console.WriteLine($"Expira em: {expiracao}");

            // Remover solicitações antigas
            var solicitacoesAntigas = await _context.TrocasEmail
                .Where(t => t.EmailAtual == request.EmailAtual)
                .ToListAsync();
            _context.TrocasEmail.RemoveRange(solicitacoesAntigas);

            // Criar nova solicitação
            var trocaEmail = new TrocaEmail
            {
                EmailAtual = request.EmailAtual,
                NovoEmail = request.NovoEmail,
                Codigo = codigo,
                Expiracao = expiracao,
                Utilizado = false
            };

            await _context.TrocasEmail.AddAsync(trocaEmail);
            await _context.SaveChangesAsync();

            try
            {
                // Enviar email com o código
                Console.WriteLine($"Tentando enviar email para: {request.EmailAtual}");
                await _emailService.EnviarCodigoVerificacaoAsync(request.EmailAtual, codigo);
                Console.WriteLine("Email enviado com sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
                return (false, $"Erro ao enviar email: {ex.Message}");
            }

            return (true, "Código de verificação enviado com sucesso");
        }

        public async Task<(bool Sucesso, string Mensagem)> ConfirmarTrocaEmailAsync(string emailAtual, string codigo)
        {
            // Buscar solicitação pendente
            var trocaEmail = await _context.TrocasEmail
                .FirstOrDefaultAsync(t => 
                    t.EmailAtual == emailAtual && 
                    t.Codigo == codigo && 
                    !t.Utilizado);

            if (trocaEmail == null)
                return (false, "Solicitação de troca de email não encontrada");

            // Verificar se o código expirou
            if (DateTime.UtcNow > trocaEmail.Expiracao)
            {
                _context.TrocasEmail.Remove(trocaEmail);
                await _context.SaveChangesAsync();
                return (false, "Código de verificação expirado");
            }

            // Buscar usuário
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == emailAtual);

            if (usuario == null)
                return (false, "Usuário não encontrado");

            // Atualizar email
            usuario.Email = trocaEmail.NovoEmail;
            trocaEmail.Utilizado = true;
            await _context.SaveChangesAsync();

            // Enviar confirmação para o novo email
            await _emailService.EnviarEmailAsync(
                trocaEmail.NovoEmail,
                "Email Alterado com Sucesso",
                "Seu email foi alterado com sucesso. Se você não fez esta alteração, entre em contato conosco imediatamente."
            );

            return (true, "Email alterado com sucesso");
        }

        private string GerarCodigoVerificacao()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[3];
            rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").Substring(0, 6);
        }
    }
} 