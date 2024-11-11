using Bigai.TaskManager.Domain.Projects.Enums;

namespace Bigai.TaskManager.Domain.Projects.Models;

public sealed class Project
{
    private readonly IList<Task> _tasks = new List<Task>();

    public int Id { get; }
    public int UserId { get; private set; }
    public string? Name { get; private set; }
    public Priority Priority { get; private set; }
    public IReadOnlyCollection<Task> Tasks => _tasks.ToArray();

    public Project() { }

    private Project(int userId, string? name, Priority priority)
    {
        UserId = userId;
        Name = name;
        Priority = priority;
    }

    public static Project Create(int userId, string? name, Priority priority)
    {
        return new Project(userId, name, priority);
    }
}