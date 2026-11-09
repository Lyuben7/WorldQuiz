using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class QuizController : Controller
    {
        public const string DefaultQuizId = "continents-bg";
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        public IActionResult Play(string? id, bool shuffle = false)
        {
            var quiz = _quizService.GetById(id ?? DefaultQuizId);
            if (quiz is null)
            {
                return NotFound();
            }

            var order = Enumerable.Range(0, quiz.Questions.Count).ToList();
            if (shuffle)
            {
                order = order.OrderBy(_ => Random.Shared.Next()).ToList();
            }

            var model = new QuizPlayViewModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Questions = order.Select(questionIndex =>
                {
                    var question = quiz.Questions[questionIndex];
                    return new QuizPlayQuestionViewModel
                    {
                        Index = questionIndex,
                        Text = question.Text,
                        Options = question.Options
                    };
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(QuizSubmitViewModel submit)
        {
            var quiz = _quizService.GetById(submit.Id);
            if (quiz is null)
            {
                return NotFound();
            }

            var answers = submit.Answers ?? new List<int?>();
            var order = submit.QuestionOrder is { Count: > 0 }
                ? submit.QuestionOrder
                : Enumerable.Range(0, quiz.Questions.Count).ToList();

            var items = order.Select((questionIndex, answerIndex) =>
            {
                var question = quiz.Questions[questionIndex];
                var selected = answerIndex < answers.Count ? answers[answerIndex] ?? -1 : -1;
                return new QuizResultItemViewModel
                {
                    Text = question.Text,
                    Options = question.Options,
                    SelectedIndex = selected,
                    CorrectIndex = question.CorrectIndex,
                    IsCorrect = selected == question.CorrectIndex
                };
            }).ToList();

            var result = new QuizResultViewModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Score = items.Count(item => item.IsCorrect),
                Total = items.Count,
                Items = items
            };

            return View("Result", result);
        }
    }
}
