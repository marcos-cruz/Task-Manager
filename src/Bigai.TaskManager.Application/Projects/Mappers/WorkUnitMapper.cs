using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Application.Projects.Mappers;

public static class WorkUnitMapper
{
    public static WorkUnitDto AsDto(this WorkUnit workUnit)
    {
        return new WorkUnitDto(workUnit.Id,
                               workUnit.ProjectId,
                               workUnit.UserId,
                               workUnit.Title,
                               workUnit.Description,
                               workUnit.DueDate,
                               workUnit.Status,
                               workUnit.Priority);
    }
}