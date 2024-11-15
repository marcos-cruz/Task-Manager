using Bigai.TaskManager.Domain.Projects.Enums;

namespace Bigai.TaskManager.Domain.Projects.Models;

public sealed class WorkUnit
{
    private readonly IList<History> _workUnitsHistory = new List<History>();

    public int Id { get; set; }
    public int? ProjectId { get; set; }
    public int? UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; }
    public Status? Status { get; set; }
    public Priority Priority { get; set; }
    public IReadOnlyCollection<History> Historys
    {
        get { return _workUnitsHistory.ToArray(); }

        init
        {
            _workUnitsHistory = value.ToArray();
        }
    }

    public WorkUnit() { }

    private WorkUnit(string title, string description, DateTime dueDate, Priority priority)
    {
        ProjectId = null;
        UserId = null;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = Enums.Status.Pending;
        Priority = priority;
    }

    public static WorkUnit Create(string title, string description, DateTime dueDate, Priority priority)
    {
        return new WorkUnit(title, description, dueDate, priority);
    }

    public void AssignToProject(int projectId)
    {
        ProjectId = projectId;
    }

    public void AssignToUser(int userId)
    {
        UserId = userId;
    }

    public void ChangeStatus(Status status)
    {
        Status = status;
    }

    public WorkUnit? GetDelta(WorkUnit workUnitUpdating)
    {
        WorkUnit? delta = null;

        string? title = Title != workUnitUpdating.Title ? workUnitUpdating.Title : null;
        string? description = Description != workUnitUpdating.Description ? workUnitUpdating.Description : null;
        DateTime? dueDate = DueDate != workUnitUpdating.DueDate ? workUnitUpdating.DueDate : null;
        Status? status = Status != workUnitUpdating.Status ? workUnitUpdating.Status : null;

        bool isUpdating = title != null || description != null || dueDate != null || status != null;
        if (isUpdating)
        {
            delta = new WorkUnit
            {
                Title = string.IsNullOrEmpty(title) ? "" : title,
                Description = string.IsNullOrEmpty(description) ? "" : description,
                DueDate = dueDate,
                Status = status
            };
        }

        return delta;
    }

    public void AddHistory(History history)
    {
        _workUnitsHistory.Add(history);
    }
}