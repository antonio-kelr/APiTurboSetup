using System.Threading.Tasks;

namespace APiTurboSetup.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<(bool success, string message, string? token, int? userId)> ValidateGoogleToken(string firebaseIdToken);
    }
} 