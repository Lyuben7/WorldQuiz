using QuizLab.Models;

namespace QuizLab.Services
{
    public interface IQuizService
    {
        IReadOnlyList<Quiz> GetAll();
        Quiz? GetById(string id);
    }
}
