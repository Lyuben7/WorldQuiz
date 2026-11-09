using MySqlConnector;
using QuizLab.Models;

namespace QuizLab.Services;

public sealed class MySqlQuizService(IConfiguration configuration) : IQuizService
{
    public IReadOnlyList<Quiz> GetAll()
    {
        using var connection = new MySqlConnection(configuration.GetConnectionString("QuizLab")
            ?? throw new InvalidOperationException("Missing QuizLab connection string."));
        connection.Open();
        using var command = new MySqlCommand("""
            SELECT q.id, q.title, qu.id, qu.question_text, qu.correct_index, a.option_text
            FROM quizzes q
            JOIN questions qu ON qu.quiz_id = q.id
            JOIN answer_options a ON a.question_id = qu.id
            ORDER BY q.id, qu.question_index, a.option_index
            """, connection);
        using var reader = command.ExecuteReader();
        var quizzes = new List<Quiz>();
        Quiz? quiz = null;
        QuizQuestion? question = null;
        var questionId = -1;
        while (reader.Read())
        {
            var id = reader.GetString(0);
            if (quiz?.Id != id)
            {
                quiz = new Quiz { Id = id, Title = reader.GetString(1) };
                quizzes.Add(quiz);
                questionId = -1;
            }
            var currentId = reader.GetInt32(2);
            if (questionId != currentId)
            {
                question = new QuizQuestion { Text = reader.GetString(3), CorrectIndex = reader.GetInt32(4) };
                quiz.Questions.Add(question);
                questionId = currentId;
            }
            question!.Options.Add(reader.GetString(5));
        }
        return quizzes;
    }

    public Quiz? GetById(string id) => GetAll().FirstOrDefault(q =>
        string.Equals(q.Id, id, StringComparison.OrdinalIgnoreCase));
}
