using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class QuizService : IQuizService
    {
        private readonly IReadOnlyList<Quiz> _quizzes;

        public QuizService(IWebHostEnvironment environment)
        {
            var path = Path.Combine(environment.ContentRootPath, "Data", "quizzes.json");
            var json = File.ReadAllText(path);
            _quizzes = JsonSerializer.Deserialize<List<Quiz>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Quiz>();
        }

        public IReadOnlyList<Quiz> GetAll() => _quizzes;

        public Quiz? GetById(string id) =>
            _quizzes.FirstOrDefault(quiz => string.Equals(quiz.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}
