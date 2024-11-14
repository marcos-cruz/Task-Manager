namespace Bigai.TaskManager.Domain.Projects.Models;

public sealed class History
{
    public int Id { get; set; }
    public int? ProjectId { get; private set; } = default!;
    public int? TaskId { get; private set; } = default!;
    public int? UserId { get; private set; } = default!;
    public DateTime UpdateDate { get; private set; } = DateTime.Now;
    public string ChangedData { get; private set; } = default!;

    public History() { }

    public static History Create(WorkUnit workUnit, string changedData)
    {
        return new History()
        {
            ProjectId = workUnit.ProjectId,
            UserId = workUnit.UserId,
            UpdateDate = DateTime.Now,
            ChangedData = changedData
        };
    }
}