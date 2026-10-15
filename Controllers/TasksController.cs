using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Filters;
using TaskManager.Models;

namespace TaskManager.Controllers;

[ServiceFilter(typeof(AuthorizationFilter))]
[ServiceFilter(typeof(ExecutionTimeFilter))]
public class TasksController : Controller
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db) => _db = db;

    public IActionResult Index(string? status, int? userId)
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = HttpContext.Session.GetInt32("UserId");

        IQueryable<TaskItem> query = _db.Tasks
            .Include(t => t.AssignedUser)
            .OrderByDescending(t => t.CreatedAt);

        if (role != "Администратор")
            query = query.Where(t => t.AssignedUserId == currentUserId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status == status);

        if (role == "Администратор" && userId.HasValue)
            query = query.Where(t => t.AssignedUserId == userId);

        ViewBag.Users = _db.Users.Where(u => u.Role == "Сотрудник").ToList();
        ViewBag.IsAdmin = role == "Администратор";

        return View(query.ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("Role") != "Администратор")
            return RedirectToAction("AccessDenied", "Account");

        ViewBag.Users = _db.Users.Where(u => u.Role == "Сотрудник").ToList();
        return View(new TaskItem { DueDate = DateTime.Today.AddDays(1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TaskItem task)
    {
        if (HttpContext.Session.GetString("Role") != "Администратор")
            return RedirectToAction("AccessDenied", "Account");

        if (task.DueDate.Date < DateTime.Today)
            ModelState.AddModelError(nameof(task.DueDate),
                "Срок выполнения не может быть раньше сегодняшнего дня.");

        if (task.AssignedUserId.HasValue &&
            !_db.Users.Any(u => u.Id == task.AssignedUserId &&
                                 u.Role == "Сотрудник"))
            ModelState.AddModelError(nameof(task.AssignedUserId),
                "Такого исполнителя не существует.");

        if (!ModelState.IsValid)
        {
            ViewBag.Users = _db.Users.Where(u => u.Role == "Сотрудник").ToList();
            return View(task);
        }

        task.CreatedAt = DateTime.Now;
        _db.Tasks.Add(task);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Администратор")
            return RedirectToAction("AccessDenied", "Account");

        var task = _db.Tasks.Find(id);
        if (task == null) return NotFound();

        ViewBag.Users = _db.Users.Where(u => u.Role == "Сотрудник").ToList();
        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(TaskItem task)
    {
        if (HttpContext.Session.GetString("Role") != "Администратор")
            return RedirectToAction("AccessDenied", "Account");

        if (task.DueDate.Date < DateTime.Today)
            ModelState.AddModelError(nameof(task.DueDate),
                "Срок выполнения не может быть раньше сегодняшнего дня.");

        if (!ModelState.IsValid)
        {
            ViewBag.Users = _db.Users.Where(u => u.Role == "Сотрудник").ToList();
            return View(task);
        }

        var existing = _db.Tasks.Find(task.Id);
        if (existing == null) return NotFound();

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Priority = task.Priority;
        existing.Status = task.Status;
        existing.DueDate = task.DueDate;
        existing.AssignedUserId = task.AssignedUserId;

        _db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Администратор")
            return RedirectToAction("AccessDenied", "Account");

        var task = _db.Tasks.Find(id);
        if (task == null) return NotFound();

        return View(task);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Администратор")
            return RedirectToAction("AccessDenied", "Account");

        var task = _db.Tasks.Find(id);
        if (task != null)
        {
            _db.Tasks.Remove(task);
            _db.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeStatus(int id, string status)
    {
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");

        var task = _db.Tasks.Find(id);
        if (task == null) return NotFound();

        if (role != "Администратор" && task.AssignedUserId != currentUserId)
            return Forbid();

        var allowed = new[] { "Новая", "В работе", "Завершена" };
        if (!allowed.Contains(status))
            return BadRequest("Недопустимый статус.");

        task.Status = status;
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RequestTask(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");

        if (role != "Сотрудник" || userId == null)
            return Forbid();

        var task = _db.Tasks.Find(id);
        if (task == null) return NotFound();

        if (task.AssignedUserId == null)
        {
            task.AssignedUserId = userId;
            _db.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }
}
