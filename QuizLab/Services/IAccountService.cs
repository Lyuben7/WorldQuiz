using QuizLab.Models;

namespace QuizLab.Services;

public interface IAccountService
{
    Task<AccountUser?> RegisterAsync(string userName, string email, string password);
    Task<AccountUser?> AuthenticateAsync(string email, string password);
    Task SaveAttemptAsync(int userId, string quizId, int score, int total);
    Task<IReadOnlyList<QuizAttempt>> GetAttemptsAsync(int userId);
}
