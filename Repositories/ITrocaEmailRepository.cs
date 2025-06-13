using APiTurboSetup.Models.DTOs;

namespace APiTurboSetup.Repositories
{
    public interface ITrocaEmailRepository
    {
        Task<(bool Sucesso, string Mensagem)> SolicitarTrocaEmailAsync(TrocaEmailRequest request);
        Task<(bool Sucesso, string Mensagem)> ConfirmarTrocaEmailAsync(string emailAtual, string codigo);
    }
} 