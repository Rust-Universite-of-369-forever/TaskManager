using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var password = new PasswordService();

        db.Users.AddRange(
            new User
            {
                Login = "admin",
                PasswordHash = password.Hash("admin123"),
                Role = "Администратор",
                FullName = "Главный администратор"
            },
            new User
            {
                Login = "employee1",
                PasswordHash = password.Hash("123456"),
                Role = "Сотрудник",
                FullName = "Иван Иванов"
            },
            new User
            {
                Login = "employee2",
                PasswordHash = password.Hash("123456"),
                Role = "Сотрудник",
                FullName = "Пётр Петров"
            }
        );

        db.SaveChanges();

        var employee1 = db.Users.First(u => u.Login == "employee1");

        db.Tasks.Add(new TaskItem
        {
            Title = "Подготовить отчёт",
            Description = "Подготовить тестовый отчёт по проекту.",
            Priority = "Высокий",
            Status = "Новая",
            CreatedAt = DateTime.Now,
            DueDate = DateTime.Today.AddDays(5),
            AssignedUserId = employee1.Id
        });

        db.SaveChanges();
    }
}
