using Newtonsoft.Json;

using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Services;

namespace Bigai.TaskManager.Infrastructure.Projects.Services;

public class SerializeService : ISerializeService
{
    public WorkUnit? JsonToWorkUnit(string json)
    {
        return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<WorkUnit>(json);
    }

    public string WorkUnitToJson(WorkUnit workUnit)
    {
        return JsonConvert.SerializeObject(workUnit);
    }
}