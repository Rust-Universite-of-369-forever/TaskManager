using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class User
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Login { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    [Required, StringLength(30)]
    public string Role { get; set; } = "Сотрудник";

    [StringLength(100)]
    public string FullName { get; set; } = "";

    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}
