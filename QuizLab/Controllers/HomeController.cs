using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using QuizLab.Models;

namespace QuizLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuizLab.Services.IQuizService _quizzes;
        public HomeController(QuizLab.Services.IQuizService quizzes) => _quizzes = quizzes;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Hello()
        {
            return View(_quizzes.GetAll());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
