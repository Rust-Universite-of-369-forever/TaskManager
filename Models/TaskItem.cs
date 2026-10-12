using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно.")]
    [StringLength(100)]
    public string Title { get; set; } = "";

    [StringLength(1000)]
    public string Description { get; set; } = "";

    [Required]
    public string Priority { get; set; } = "Средний";

    [Required]
    public string Status { get; set; } = "Новая";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Укажите срок выполнения.")]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);

    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }

    public bool IsOverdue =>
        Status != "Завершена" && DueDate.Date < DateTime.Today;
}
