using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using QuizLab.Models;
using QuizLab.Services;
using QuizLab.ViewModels;

namespace QuizLab.Controllers;

public sealed class AccountController(IAccountService accounts) : Controller
{
    [HttpGet]
    public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction(nameof(Achievements)) : View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await accounts.RegisterAsync(model.UserName, model.Email, model.Password);
        if (user is null)
        {
            ModelState.AddModelError(nameof(model.Email), "Това потребителско име или имейл вече е регистриран.");
            return View(model);
        }
        await SignInAsync(user);
        return RedirectToAction(nameof(Achievements));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction(nameof(Achievements));
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await accounts.AuthenticateAsync(model.Email, model.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Имейлът или паролата са грешни.");
            return View(model);
        }
        await SignInAsync(user);
        return Url.IsLocalUrl(model.ReturnUrl) ? Redirect(model.ReturnUrl!) : RedirectToAction(nameof(Achievements));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("QuizLabCookie");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Achievements()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return RedirectToAction(nameof(Login), new { returnUrl = Url.Action(nameof(Achievements)) });
        var attempts = await accounts.GetAttemptsAsync(userId);
        return View(new AchievementViewModel { Attempts = attempts });
    }

    private async Task SignInAsync(AccountUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };
        await HttpContext.SignInAsync("QuizLabCookie", new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    }
}
