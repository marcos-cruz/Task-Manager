using Bigai.TaskManager.Domain.Projects.Enums;

namespace Bigai.TaskManager.Domain.Projects.Models;

public sealed class Task
{
    public int Id { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTimeOffset DueDate { get; set; }
    public Status Status { get; set; }

    private Task(string title, string description, DateTimeOffset dueDate)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = Status.Pending;
    }

    public static Task Create(string title, string description, DateTimeOffset dueDate)
    {
        return new Task(title, description, dueDate);
    }
}