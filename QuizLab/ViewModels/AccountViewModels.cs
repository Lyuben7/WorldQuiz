using System.ComponentModel.DataAnnotations;

namespace QuizLab.ViewModels;

public sealed class RegisterViewModel
{
    [Required, StringLength(40, MinimumLength = 3)]
    [RegularExpression(@"^[\p{L}\p{N}_-]+$", ErrorMessage = "Използвай букви, цифри, _ или -.")]
    [Display(Name = "Потребителско име")]
    public string UserName { get; set; } = "";

    [Required, EmailAddress, StringLength(254)]
    [Display(Name = "Имейл")]
    public string Email { get; set; } = "";

    [Required, StringLength(100, MinimumLength = 10)]
    [DataType(DataType.Password)]
    [Display(Name = "Парола (поне 10 знака)")]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Повтори паролата")]
    public string ConfirmPassword { get; set; } = "";
}

public sealed class LoginViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Имейл")]
    public string Email { get; set; } = "";
    [Required, DataType(DataType.Password)]
    [Display(Name = "Парола")]
    public string Password { get; set; } = "";
    public string? ReturnUrl { get; set; }
}

public sealed class AchievementViewModel
{
    public IReadOnlyList<Models.QuizAttempt> Attempts { get; init; } = [];
    public int CompletedCount => Attempts.Count;
    public int PerfectCount => Attempts.Count(a => a.Score == a.Total);
    public int AnsweredCount => Attempts.Sum(a => a.Total);
    public int CorrectCount => Attempts.Sum(a => a.Score);
}
