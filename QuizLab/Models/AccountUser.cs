namespace QuizLab.Models;

public sealed record AccountUser
{
    public int Id { get; init; }
    public string UserName { get; init; } = "";
    public string Email { get; init; } = "";
    public string PasswordHash { get; init; } = "";
}

public sealed record QuizAttempt(long Id, string QuizId, string QuizTitle, int Score, int Total, DateTime CompletedAt);
