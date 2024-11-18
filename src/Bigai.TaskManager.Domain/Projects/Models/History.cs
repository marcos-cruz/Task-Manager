using Bigai.TaskManager.Domain.Projects.Services;

namespace Bigai.TaskManager.Domain.Projects.Models;

public sealed class History
{
    public int Id { get; set; }
    public int? ProjectId { get; private set; } = default!;
    public int WorkUnitId { get; private set; } = default!;
    public int? UserId { get; private set; } = default!;
    public DateTime UpdateDate { get; private set; } = DateTime.Now;
    public string ChangedData { get; private set; } = default!;

    public History() { }

    public static History Create(WorkUnit existingWorkUnit, WorkUnit changedValues, ISerializeService serializeService)
    {
        return new History()
        {
            ProjectId = existingWorkUnit.ProjectId,
            WorkUnitId = existingWorkUnit.Id,
            UserId = existingWorkUnit.UserId,
            UpdateDate = DateTime.Now,
            ChangedData = serializeService.WorkUnitToJson(changedValues)
        };
    }

    public void AssignToUser(int userId)
    {
        UserId = userId;
    }
}