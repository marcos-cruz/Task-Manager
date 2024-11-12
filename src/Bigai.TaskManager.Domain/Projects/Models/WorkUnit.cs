using Bigai.TaskManager.Domain.Projects.Enums;

namespace Bigai.TaskManager.Domain.Projects.Models;

public sealed class WorkUnit
{
    public int Id { get; set; }
    public int? ProjectId { get; private set; }
    public int? UserId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public DateTimeOffset DueDate { get; private set; }
    public Status Status { get; private set; }
    public Priority Priority { get; private set; }

    public WorkUnit() { }

    private WorkUnit(string title, string description, DateTimeOffset dueDate, Priority priority)
    {
        ProjectId = null;
        UserId = null;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = Status.Pending;
        Priority = priority;
    }

    public static WorkUnit Create(string title, string description, DateTimeOffset dueDate, Priority priority)
    {
        return new WorkUnit(title, description, dueDate, priority);
    }

    public void AssignToUser(int userId)
    {
        UserId = userId;
    }

    public void ChangeStatus(Status status)
    {
        Status = status;
    }
}