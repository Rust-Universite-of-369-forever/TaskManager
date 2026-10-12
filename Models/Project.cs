using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class Project
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
