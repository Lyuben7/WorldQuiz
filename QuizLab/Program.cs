var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<QuizLab.Services.IQuizService, QuizLab.Services.MySqlQuizService>();
builder.Services.AddScoped<QuizLab.Services.IAccountService, QuizLab.Services.MySqlAccountService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<QuizLab.Models.AccountUser>, Microsoft.AspNetCore.Identity.PasswordHasher<QuizLab.Models.AccountUser>>();
builder.Services.AddAuthentication("QuizLabCookie").AddCookie("QuizLabCookie", options =>
{
    options.LoginPath = "/Account/Login";
    options.Cookie.Name = "QuizLab.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
