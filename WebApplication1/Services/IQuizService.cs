using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IQuizService
    {
        IReadOnlyList<Quiz> GetAll();
        Quiz? GetById(string id);
    }
}
