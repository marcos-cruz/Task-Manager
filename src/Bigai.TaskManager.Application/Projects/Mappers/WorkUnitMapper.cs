using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Projects.Commands.UpdateWorkUnit;
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
                               workUnit.DueDate!.Value,
                               workUnit.Status!.Value,
                               workUnit.Priority);
    }

    public static IReadOnlyCollection<WorkUnitDto> AsDto(this IReadOnlyCollection<WorkUnit> workUnits)
    {
        var workUnitsDto = workUnits.Select(w => w.AsDto()).ToArray();

        return workUnitsDto;
    }

    public static WorkUnit AsEntity(this CreateWorkUnitCommand command)
    {
        var workUnit = WorkUnit.Create(command.Title, command.Description, command.DueDate, command.Priority);
        workUnit.AssignToProject(command.ProjectId);

        return workUnit;
    }

    public static WorkUnit AsEntity(this UpdateWorkUnitCommand command, WorkUnit existingWorkUnit)
    {
        return new WorkUnit()
        {
            Title = command.Title ?? existingWorkUnit.Title,
            Description = command.Description ?? existingWorkUnit.Description,
            DueDate = command.DueDate ?? existingWorkUnit.DueDate,
            Status = command.Status ?? existingWorkUnit.Status
        };
    }

}