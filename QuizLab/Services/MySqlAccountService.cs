using Microsoft.AspNetCore.Identity;
using MySqlConnector;
using QuizLab.Models;

namespace QuizLab.Services;

public sealed class MySqlAccountService(
    IConfiguration configuration,
    IPasswordHasher<AccountUser> passwordHasher) : IAccountService
{
    private string ConnectionString => configuration.GetConnectionString("QuizLab")
        ?? throw new InvalidOperationException("Missing QuizLab connection string.");

    public async Task<AccountUser?> RegisterAsync(string userName, string email, string password)
    {
        var user = new AccountUser { UserName = userName.Trim(), Email = email.Trim().ToLowerInvariant() };
        var hash = passwordHasher.HashPassword(user, password);
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(
            "INSERT INTO users (user_name, email, password_hash) VALUES (@name, @email, @hash); SELECT LAST_INSERT_ID();", connection);
        command.Parameters.AddWithValue("name", user.UserName);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("hash", hash);
        try
        {
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            return user with { Id = id };
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            return null;
        }
    }

    public async Task<AccountUser?> AuthenticateAsync(string email, string password)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(
            "SELECT id, user_name, email, password_hash FROM users WHERE email = @email LIMIT 1", connection);
        command.Parameters.AddWithValue("email", email.Trim().ToLowerInvariant());
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;
        var user = new AccountUser
        {
            Id = reader.GetInt32(0), UserName = reader.GetString(1),
            Email = reader.GetString(2), PasswordHash = reader.GetString(3)
        };
        return passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed
            ? null : user;
    }

    public async Task SaveAttemptAsync(int userId, string quizId, int score, int total)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(
            "INSERT INTO quiz_attempts (user_id, quiz_id, score, total) VALUES (@user, @quiz, @score, @total)", connection);
        command.Parameters.AddWithValue("user", userId);
        command.Parameters.AddWithValue("quiz", quizId);
        command.Parameters.AddWithValue("score", score);
        command.Parameters.AddWithValue("total", total);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<QuizAttempt>> GetAttemptsAsync(int userId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand("""
            SELECT a.id, a.quiz_id, q.title, a.score, a.total, a.completed_at
            FROM quiz_attempts a JOIN quizzes q ON q.id = a.quiz_id
            WHERE a.user_id = @user ORDER BY a.completed_at DESC, a.id DESC
            """, connection);
        command.Parameters.AddWithValue("user", userId);
        await using var reader = await command.ExecuteReaderAsync();
        var attempts = new List<QuizAttempt>();
        while (await reader.ReadAsync())
            attempts.Add(new QuizAttempt(reader.GetInt64(0), reader.GetString(1), reader.GetString(2),
                reader.GetInt32(3), reader.GetInt32(4), reader.GetDateTime(5)));
        return attempts;
    }
}
