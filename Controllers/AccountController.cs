using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Services;

namespace TaskManager.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly PasswordService _password;

    public AccountController(AppDbContext db, PasswordService password)
    {
        _db = db;
        _password = password;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string login, string password)
    {
        var user = _db.Users.FirstOrDefault(u => u.Login == login);

        if (user == null || !_password.Verify(password, user.PasswordHash))
        {
            ModelState.AddModelError("", "Неверный логин или пароль.");
            return View();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Login", user.Login);
        HttpContext.Session.SetString("Role", user.Role);

        return RedirectToAction("Index", "Tasks");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}
