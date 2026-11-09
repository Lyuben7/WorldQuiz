using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QuizLab.Services;
using QuizLab.ViewModels;

namespace QuizLab.Controllers
{
    public class QuizController : Controller
    {
        public const string DefaultQuizId = "continents-bg";
        private readonly IQuizService _quizService;
        private readonly IAccountService _accounts;

        public QuizController(IQuizService quizService, IAccountService accounts)
        {
            _quizService = quizService;
            _accounts = accounts;
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
        public async Task<IActionResult> Submit(QuizSubmitViewModel submit)
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

            if (order.Count != quiz.Questions.Count || order.Distinct().Count() != order.Count ||
                order.Any(index => index < 0 || index >= quiz.Questions.Count))
            {
                return BadRequest("Невалиден ред на въпросите.");
            }

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

            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                await _accounts.SaveAttemptAsync(userId, result.Id, result.Score, result.Total);
                result.SavedToProfile = true;
            }

            return View("Result", result);
        }
    }
}
