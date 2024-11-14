using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Domain.Projects.Services;

public interface ISerializeService
{
    public string WorkUnitToJson(WorkUnit workUnit);

    public WorkUnit? JsonToWorkUnit(string json);
}